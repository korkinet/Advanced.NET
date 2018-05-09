using System;
using System.Collections.Generic;

namespace IoCContainer
{
    public class IoCBuilder
    {
        Dictionary<Type, (Type type, LifeTime lifeTime)> resolvers = new Dictionary<Type, (Type, LifeTime)>();

        public void RegisterType<RegType, ResType>(LifeTime lifeTime) where ResType : class, RegType
        {
            resolvers[typeof(RegType)] = (typeof(ResType), lifeTime);
        }

        public IoCContainer Build()
        {
            return new IoCContainer(resolvers);
        }
    }
}
