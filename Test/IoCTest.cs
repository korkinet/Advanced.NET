using IoCContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;

namespace Test
{
    [TestClass]
    public class IoCTest
    {
        [TestMethod]
        public void DefaultInstances()
        {
            var builder = new IoCBuilder();
            builder.RegisterType<IService, Service>(LifeTime.Default);

            var container = builder.Build();

            var obj1 = container.Resolve<IService>();
            var obj2 = container.Resolve<IService>();

            Assert.AreNotEqual(obj1, obj2);
        }

        [TestMethod]
        public void SingletonInstance()
        {
            var builder = new IoCBuilder();
            builder.RegisterType<IService, Service>(LifeTime.Singleton);

            var container = builder.Build();

            var obj1 = container.Resolve<IService>();
            var obj2 = container.Resolve<IService>();

            Assert.AreEqual(obj1, obj2);
        }
    }
}
