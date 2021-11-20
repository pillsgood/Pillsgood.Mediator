using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace Pillsgood.Mediator.Tests
{
    public class SendTests
    {
        public class Ping : ISignal<Pong>
        {
            public string Message { get; set; }
        }

        public class Pong
        {
            public string Message { get; set; }
        }

        public class PingHandler : ISignalHandler<Ping, Pong>
        {
            public Task<Pong> Handle(Ping signal, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Pong { Message = signal.Message + " Pong" });
            }
        }

        [Test]
        public async Task Should_resolve_main_handler()
        {
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType(typeof(PublishTests));
                    scanner.IncludeNamespaceContainingType<Ping>();
                    scanner.WithDefaultConventions();
                    scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
                });
                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
                cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
            });

            var mediator = container.GetInstance<IMediator>();

            var response = await mediator.Send(new Ping { Message = "Ping" });

            response.Message.ShouldBe("Ping Pong");
        }

        [Test]
        public async Task Should_resolve_main_handler_via_dynamic_dispatch()
        {
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType(typeof(PublishTests));
                    scanner.IncludeNamespaceContainingType<Ping>();
                    scanner.WithDefaultConventions();
                    scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
                });
                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
                cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
            });

            var mediator = container.GetInstance<IMediator>();

            object signal = new Ping { Message = "Ping" };
            var response = await mediator.Send(signal);

            var pong = response.ShouldBeOfType<Pong>();
            pong.Message.ShouldBe("Ping Pong");
        }

        [Test]
        public async Task Should_resolve_main_handler_by_specific_interface()
        {
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType(typeof(PublishTests));
                    scanner.IncludeNamespaceContainingType<Ping>();
                    scanner.WithDefaultConventions();
                    scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
                });
                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
                cfg.For<ISender>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
            });

            var mediator = container.GetInstance<ISender>();

            var response = await mediator.Send(new Ping { Message = "Ping" });

            response.Message.ShouldBe("Ping Pong");
        }
    }
}