using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCContainer
{
    class ResolveData
    {
        public LifeTime LifeTime { get; set; }
        public bool IsSingle { get; set; }
        public Dictionary<Type, Lazy<dynamic>> Instances { get; set; }

        public ResolveData(Type[] types, LifeTime lifeTime, bool isSingle, Func<dynamic> valuFactory = null)
        {
            LifeTime = lifeTime;
            IsSingle = isSingle;
            Instances = types.ToDictionary(t => t, t => new Lazy<dynamic>(valuFactory, true));
        }
    }
}