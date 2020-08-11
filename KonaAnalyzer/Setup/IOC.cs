using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;

namespace KonaAnalyzer.Setup
{
    public class IOC
    {
        public IContainer Container { get; private set; }

        private static IOC _instance;
        public static IOC Instance => _instance ?? (_instance = new IOC());

        private IOC() { }

        public void Setup(IEnumerable<Type> modules, params Action<ContainerBuilder>[] actions)
        {
            var containerNull = Container == null;

            var builder = new ContainerBuilder();
            foreach (var item in actions)
            {
                item(builder);
            }
            foreach (var item in modules)
            {
                var activated = Activator.CreateInstance(item);
                builder.RegisterModule(activated as IModule);
            }

            if (containerNull)
            {
                Container = builder.Build();
            }
        }
        public static T Get<T>()
        {
            return Instance.Container.Resolve<T>();
        }
        public static T Get<T>(Type type)
        {
            return (T)Instance.Container.Resolve(type);
        }
        public static object Get(Type type)
        {
            return Instance.Container.Resolve(type);
        }
    }
}
 
