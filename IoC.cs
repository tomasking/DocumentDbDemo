namespace DocumentDbDemo
{
    using Autofac;
    using Listeners;
    using Repository;

    public class IoC
    {
        private static IContainer _container;

        public static IContainer CreateContainer()
        {
            if (_container == null)
            {
                var builder = new ContainerBuilder();

                builder.RegisterType<ProductRepository>().As<IProductRepository>().SingleInstance();
                builder.RegisterType<ProductListener>().As<IStartable>().SingleInstance();
                
                _container = builder.Build();
            }

            return _container;
        }

    }
}