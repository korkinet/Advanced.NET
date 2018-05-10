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
        public Dictionary<Type, object> Instances { get; set; } = new Dictionary<Type, object>();

        public ResolveData(Type[] types, LifeTime lifeTime, bool isSingle)
        {
            LifeTime = lifeTime;
            IsSingle = isSingle;
            foreach (var type in types)
            {
                Instances[type] = null;
            }
        }
    }
}