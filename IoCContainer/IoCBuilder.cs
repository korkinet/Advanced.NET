﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            return type.BaseType != null && (type.BaseType.FullName == baseType.FullName || IsDerivedFrom(type.BaseType, baseType));
        }

        private void RegisterType(Type regType, Type[] resTypes, LifeTime lifeTime, bool isSingle)
        {
            resolvers[regType] = (resTypes, lifeTime, isSingle);
        }

        public void RegisterType<RegType, ResType>(LifeTime lifeTime) where ResType : class, RegType
        {
            RegisterType(typeof(RegType), new[] { typeof(ResType) }, lifeTime, true);
        }

        public void RegisterByPath(string path, Type baseType)
        {
            if (!Directory.Exists(path))
            {
                return;
            }


            List<Type> types = new List<Type>();
            foreach (var file in Directory.GetFiles(path, "*.dll"))
            {
                try
                {
                    Assembly assembly = null;
                    var reflectedAssembly = Assembly.ReflectionOnlyLoadFrom(file);
                    foreach (var type in reflectedAssembly.GetTypes().Where(t => !t.IsInterface && (IsDerivedFrom(t, baseType) || t.GetInterfaces().Any(i => i.FullName == baseType.FullName))))
                    {
                        assembly = assembly ?? Assembly.LoadFrom(file);
                        types.Add(assembly.GetType(type.FullName));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load assembly, {ex.Message}");
                }
            }

            RegisterType(Array.CreateInstance(baseType, 0).GetType(), types.ToArray(), LifeTime.Default, false);
        }

        public IoCContainer Build()
        {
            return new IoCContainer(resolvers);
        }
    }
}