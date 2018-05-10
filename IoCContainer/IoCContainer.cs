using System;
using System.Linq;
using System.Collections.Generic;
using static System.Activator;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Timers;

namespace IoCContainer
{
    public class IoCContainer
    {
        private Dictionary<Type, ResolveData> resolvers;

        private List<AppDomain> appDomains = new List<AppDomain>();

        private object ResolveInstance(Type type)
        {

            List<object> parameters = new List<object>();
            var resolveMethod = this.GetType().GetMethod("Resolve");

            var constructorInfo = type.GetConstructors().OrderBy(c => c.GetParameters().Length).FirstOrDefault();
            if (constructorInfo != null)
            {
                foreach (var param in constructorInfo.GetParameters())
                {
                    var paramInstance = resolveMethod.MakeGenericMethod(param.GetType()).Invoke(this, null);
                    parameters.Add(paramInstance);
                }
                return CreateInstance(type, parameters.ToArray(), null);
            }

            return CreateInstance(type, true);
        }

        public IoCContainer(Dictionary<Type, (Type[] types, LifeTime lifeTime, bool isSingle)> resolvers)
        {
            this.resolvers = resolvers != null ?
                resolvers.ToDictionary((KeyValuePair<Type, (Type[] types, LifeTime lifeTime, bool isSingle)> r) => r.Key, (KeyValuePair<Type, (Type[] types, LifeTime lifeTime, bool isSingle)> r) => new ResolveData(r.Value.types, r.Value.lifeTime, r.Value.isSingle)) :
                new Dictionary<Type, ResolveData>();
        }

        public T Resolve<T>()
        {
            if (resolvers.ContainsKey(typeof(T)))
            {
                List<object> instances = new List<object>();
                var typeData = resolvers[typeof(T)];
                switch (typeData.LifeTime)
                {
                    case LifeTime.Default:
                        instances.AddRange(typeData.Instances.Select(i => ResolveInstance(i.Key)));
                        break;
                    case LifeTime.Singleton:
                        foreach (var key in typeData.Instances.Keys.ToArray())
                        {
                            if (typeData.Instances[key] is null)
                            {
                                typeData.Instances[key] = ResolveInstance(key);
                            }
                        }
                        instances.AddRange(typeData.Instances.Select(i => i.Value ?? ResolveInstance(i.Key)));
                        break;
                }
                if (typeof(T).IsArray)
                {
                    return (T)(object)instances.ToArray();
                }

                return (T)instances.FirstOrDefault();
            }
            return default(T);
        }

        public IPlugin[] LoadPlugins(string path)
        {
            AppDomain.MonitoringIsEnabled = true;

            List<IPlugin> instances = new List<IPlugin>();

            foreach (var file in Directory.GetFiles(path, "*.dll"))
            {
                AppDomain appDomain = null;
                var assembly = Assembly.ReflectionOnlyLoadFrom(file);

                foreach (var type in assembly.GetTypes().Where(t => !t.IsInterface && t.GetInterfaces().Any(i => i.FullName == typeof(IPlugin).FullName)))
                {
                    if (appDomain is null)
                    {
                        appDomain = AppDomain.CreateDomain(assembly.FullName);
                        appDomains.Add(appDomain);
                    }

                    var instanceHandle = appDomain.CreateInstanceFrom(file, type.FullName);

                    instances.Add((IPlugin)instanceHandle.Unwrap());
                }
            }

            if (appDomains.Count > 0)
            {
                var timer = new System.Timers.Timer(1000);
                timer.Elapsed += Monitoring;
                timer.Start();
            }

            return instances.ToArray();
        }

        private void Monitoring(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<AppDomain> domainsToKill = new List<AppDomain>();
            appDomains.ForEach(appDomain =>
            {
                Debug.WriteLine($"allocated memory: {appDomain.MonitoringTotalAllocatedMemorySize}, processor time: {appDomain.MonitoringTotalProcessorTime}");
                if (appDomain.MonitoringTotalAllocatedMemorySize > 10000000 || appDomain.MonitoringTotalProcessorTime > TimeSpan.FromSeconds(1))
                {
                    domainsToKill.Add(appDomain);
                    Debug.WriteLine($"App domain {appDomain.FriendlyName} will be unloaded");
                }
                else
                {
                    Debug.WriteLine($"App domain {appDomain.FriendlyName} run ok");
                }
            });

            domainsToKill.ForEach(appDomain =>
            {
                appDomains.Remove(appDomain);
                AppDomain.Unload(appDomain);
            });

            if (appDomains.Count == 0)
            {
                ((Timer)sender).Stop();
                sender = null;
                Debug.WriteLine($"All app domains were unloaded");
            }
        }
    }
}
