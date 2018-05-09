using System;
using IoCContainer;
using Models;

namespace RunApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder1 = new IoCBuilder();
            builder1.RegisterType<IService, Service>(LifeTime.Default);

            var container1 = builder1.Build();

            var obj1 = container1.Resolve<IService>();
            var obj2 = container1.Resolve<IService>();

            Console.WriteLine($"{nameof(obj1)} = {nameof(obj2)} : {obj1 == obj2}");

            var builder2 = new IoCBuilder();
            builder2.RegisterType<IService, Service>(LifeTime.Singleton);

            var container2 = builder2.Build();

            var obj3 = container2.Resolve<IService>();
            var obj4 = container2.Resolve<IService>();

            Console.WriteLine($"{nameof(obj3)} = {nameof(obj4)} : {obj3 == obj4}");

            Console.WriteLine($"{nameof(obj1)} = {nameof(obj3)} : {obj1 == obj3}");


            Console.ReadLine();
        }
    }
}
