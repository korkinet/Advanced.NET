using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace IoCContainer
{
    public class IoCBuilder
    {
        Dictionary<Type, (Type[] types, LifeTime lifeTime, bool isSingle)> resolvers = new Dictionary<Type, (Type[], LifeTime, bool)>();
        List<string> assembliesToLoad = new List<string>();

        private bool IsDerivedFrom(Type type, Type baseType)
        {
            return type.BaseType != null && (type.BaseType == baseType || IsDerivedFrom(type.BaseType, baseType));
        }

        public void RegisterType<RegType, ResType>(LifeTime lifeTime) where ResType : class, RegType
        {
            RegisterType(typeof(RegType), new[] { typeof(ResType) }, lifeTime, true);
        }

        private void RegisterType(Type regType, Type[] resTypes, LifeTime lifeTime, bool isSingle)
        {
            resolvers[regType] = (resTypes, lifeTime, isSingle);
        }

        public void RegisterByPath(string path, Type baseType)
        {
            List<Type> types = new List<Type>();
            foreach (var file in new DirectoryInfo(path).GetFiles("*.dll"))
            {
                try
                {
                    var assembly = Assembly.ReflectionOnlyLoadFrom(file.FullName);
                    foreach (var type in assembly.GetTypes())
                    {
                        if (IsDerivedFrom(type, baseType))
                        {
                            assembliesToLoad.Add(file.FullName);
                            types.Add(type);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load assembly, {ex.Message}");
                }
            }
            RegisterType(baseType, types.ToArray(), LifeTime.Default, false);
        }

        public IoCContainer Build()
        {
            assembliesToLoad.ForEach(assembly => Assembly.LoadFrom(assembly));
            return new IoCContainer(resolvers);
        }
    }
}