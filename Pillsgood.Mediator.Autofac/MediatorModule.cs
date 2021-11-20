using Autofac;
using Pillsgood.Mediator.Publishers;

namespace Pillsgood.Mediator.Autofac;

public class MediatorModule : MediatorModule<CompositePublisher>
{
}

public class MediatorModule<TPublisher> : Module where TPublisher : IPublisher
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TPublisher>()
            .As<IPublisher>()
            .InstancePerLifetimeScope();
        builder.RegisterType<Mediator>()
            .As<IMediator>()
            .InstancePerLifetimeScope();
        builder.Register<ServiceFactory>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return type => c.Resolve(type);
        });
    }
}