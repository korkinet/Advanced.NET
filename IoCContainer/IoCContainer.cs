using System;
using System.Linq;
using System.Collections.Generic;
using static System.Activator;

namespace IoCContainer
{
    public class IoCContainer
    {
        private Dictionary<Type, ResolveData> Resolvers;
        delegate dynamic dele(Type t);
        public IoCContainer(Dictionary<Type, (Type[] types, LifeTime lifeTime, bool isSingle)> resolvers)
        {
            Resolvers = resolvers != null ?
                resolvers.ToDictionary(r => r.Key, r => new ResolveData(r.Value.types, r.Value.lifeTime, r.Value.isSingle)) :
                new Dictionary<Type, ResolveData>();

            Func<dynamic> x = Resolve<dynamic>;
        }

        public T Resolve<T>()
        {
            if (Resolvers.ContainsKey(typeof(T)))
            {
                var typeData = Resolvers[typeof(T)];
                switch (typeData.LifeTime)
                {
                    case LifeTime.Default:
                        var instances = typeData.Instances.Select(i => ResolveInstance<T>(i.Key)).ToArray();
                        if (typeData.IsSingle)
                            return instances[0];
                        else
                            return instances;
                    case LifeTime.Singleton:
                        foreach (var key in typeData.Instances.Keys) {
                            typeData.Instances[key] = typeData.Instances[key] ?? ResolveInstance<T>(key);
                        }
                        var instances = typeData.Instances.Select(i => i.Value ?? ResolveInstance<T>(i.Key)).ToArray();
                        if (instances.ContainsKey(typeof(T))) {
                            return (T)instances[typeof(T)];
                        }
                        var instance = ResolveInstance<T>(typeData.type);
                        instances[typeof(T)] = instance;
                        return instance;
                }
            }
            return default(T);
        }

        private T ResolveInstance<T>(Type type)
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
                return (T)CreateInstance(type, parameters.ToArray(), null);
            }

            return (T)CreateInstance(type, true);
        }
    }
}
