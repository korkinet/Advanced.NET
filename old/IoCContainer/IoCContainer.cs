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
                List<object> instances = new List<object>();
                var typeData = Resolvers[typeof(T)];
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
    }
}
