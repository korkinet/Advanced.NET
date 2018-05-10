using System;
using System.IO;
using IoCContainer;
using Models;

namespace RunApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // -------------- Default --------------
            var builder1 = new IoCBuilder();
            builder1.RegisterType<IService, Service>(LifeTime.Default);

            var container1 = builder1.Build();

            var obj1 = container1.Resolve<IService>();
            var obj2 = container1.Resolve<IService>();

            Console.WriteLine($"{nameof(obj1)} = {nameof(obj2)} : {obj1 == obj2}");

            // -------------- Singleton --------------

            var builder2 = new IoCBuilder();
            builder2.RegisterType<IService, Service>(LifeTime.Singleton);

            var container2 = builder2.Build();

            var obj3 = container2.Resolve<IService>();
            var obj4 = container2.Resolve<IService>();

            Console.WriteLine($"{nameof(obj3)} = {nameof(obj4)} : {obj3 == obj4}");

            Console.WriteLine($"{nameof(obj1)} = {nameof(obj3)} : {obj1 == obj3}");

            // -------------- Loaded by path --------------

            var builder3 = new IoCBuilder();
            builder3.RegisterByPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Models\\bin\\Debug"), typeof(object));
            var container3 = builder3.Build();

            var obj5 = container3.Resolve<object[]>();

            Console.WriteLine($"Loaded by path {(obj5 != null ? "succeeded" : "failed")}, number of instances {obj5?.Length}");

            // -------------- Plugin --------------

            try
            {
                var plugins = container3.LoadPlugins(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Plugins\\bin\\Debug"));
                Console.WriteLine($"{plugins.Length} plugins loaded");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load plugins - {ex.Message}");
                throw;
            }


            Console.ReadLine();
        }
    }
}
