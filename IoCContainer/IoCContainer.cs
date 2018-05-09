using System;
using System.Linq;
using System.Collections.Generic;
using static System.Activator;

namespace IoCContainer
{
    public class IoCContainer
    {
        private Dictionary<Type, (Type type, LifeTime lifeTime)> Resolvers;
        private Dictionary<Type, dynamic> instances;

        public IoCContainer(Dictionary<Type, (Type type, LifeTime lifeTime)> resolvers)
        {
            Resolvers = resolvers ?? new Dictionary<Type, (Type, LifeTime)>();
            instances = new Dictionary<Type, dynamic>();
        }

        public T Resolve<T>()
        {
            if (Resolvers.ContainsKey(typeof(T)))
            {
                var typeData = Resolvers[typeof(T)];
                switch (typeData.lifeTime)
                {
                    case LifeTime.Default:
                        return ResolveInstance<T>(typeData.type);
                    case LifeTime.Singleton:
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
