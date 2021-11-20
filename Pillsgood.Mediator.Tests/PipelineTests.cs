using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace Pillsgood.Mediator.Tests
{
    public class PipelineTests
    {
        public class Ping : ISignal<Pong>
        {
            public string Message { get; set; }
        }

        public class Pong
        {
            public string Message { get; set; }
        }

        public class Zing : ISignal<Zong>
        {
            public string Message { get; set; }
        }

        public class Zong
        {
            public string Message { get; set; }
        }

        public class PingHandler : ISignalHandler<Ping, Pong>
        {
            private readonly Logger _output;

            public PingHandler(Logger output)
            {
                _output = output;
            }

            public Task<Pong> Handle(Ping signal, CancellationToken cancellationToken)
            {
                _output.Messages.Add("Handler");
                return Task.FromResult(new Pong { Message = signal.Message + " Pong" });
            }
        }

        public class ZingHandler : ISignalHandler<Zing, Zong>
        {
            private readonly Logger _output;

            public ZingHandler(Logger output)
            {
                _output = output;
            }

            public Task<Zong> Handle(Zing signal, CancellationToken cancellationToken)
            {
                _output.Messages.Add("Handler");
                return Task.FromResult(new Zong { Message = signal.Message + " Zong" });
            }
        }

        public class OuterBehaviour : IPipelineBehaviour<Ping, Pong>
        {
            private readonly Logger _output;

            public OuterBehaviour(Logger output)
            {
                _output = output;
            }


            public async Task<Pong> Handle(Ping signal, HandleSignal<Pong> next, CancellationToken cancellationToken)
            {
                _output.Messages.Add("Outer before");
                var response = await next();
                _output.Messages.Add("Outer after");

                return response;
            }
        }

        public class InnerBehaviour : IPipelineBehaviour<Ping, Pong>
        {
            private readonly Logger _output;

            public InnerBehaviour(Logger output)
            {
                _output = output;
            }


            public async Task<Pong> Handle(Ping signal, HandleSignal<Pong> next, CancellationToken cancellationToken)
            {
                _output.Messages.Add("Inner before");
                var response = await next();
                _output.Messages.Add("Inner after");

                return response;
            }
        }

        public class InnerBehaviour<TSignal, TResponse> : IPipelineBehaviour<TSignal, TResponse>
            where TSignal : ISignal<TResponse>
        {
            private readonly Logger _output;

            public InnerBehaviour(Logger output)
            {
                _output = output;
            }

            public async Task<TResponse> Handle(
                TSignal signal,
                HandleSignal<TResponse> next,
                CancellationToken cancellationToken)
            {
                _output.Messages.Add("Inner generic before");
                var response = await next();
                _output.Messages.Add("Inner generic after");

                return response;
            }
        }

        public class OuterBehaviour<TSignal, TResponse> : IPipelineBehaviour<TSignal, TResponse>
            where TSignal : ISignal<TResponse>
        {
            private readonly Logger _output;

            public OuterBehaviour(Logger output)
            {
                _output = output;
            }

            public async Task<TResponse> Handle(
                TSignal signal,
                HandleSignal<TResponse> next,
                CancellationToken cancellationToken)
            {
                _output.Messages.Add("Outer generic before");
                var response = await next();
                _output.Messages.Add("Outer generic after");

                return response;
            }
        }

        public class ConstrainedBehaviour<TSignal, TResponse> : IPipelineBehaviour<TSignal, TResponse>
            where TSignal : Ping, ISignal<TResponse>
            where TResponse : Pong
        {
            private readonly Logger _output;

            public ConstrainedBehaviour(Logger output)
            {
                _output = output;
            }


            public async Task<TResponse> Handle(
                TSignal signal,
                HandleSignal<TResponse> next,
                CancellationToken cancellationToken)
            {
                _output.Messages.Add("Constrained before");
                var response = await next();
                _output.Messages.Add("Constrained after");

                return response;
            }
        }

        public class ConcreteBehaviour : IPipelineBehaviour<Ping, Pong>
        {
            private readonly Logger _output;

            public ConcreteBehaviour(Logger output)
            {
                _output = output;
            }

            public async Task<Pong> Handle(Ping signal, HandleSignal<Pong> next, CancellationToken cancellationToken)
            {
                _output.Messages.Add("Concrete before");
                var response = await next();
                _output.Messages.Add("Concrete after");

                return response;
            }
        }

        public class Logger
        {
            public IList<string> Messages { get; } = new List<string>();
        }

        [Test]
        public async Task Should_wrap_with_behavior()
        {
            var output = new Logger();
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType(typeof(PublishTests));
                    scanner.IncludeNamespaceContainingType<Ping>();
                    scanner.WithDefaultConventions();
                    scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
                });
                cfg.For<Logger>().Singleton().Use(output);
                cfg.For<IPipelineBehaviour<Ping, Pong>>().Add<OuterBehaviour>();
                cfg.For<IPipelineBehaviour<Ping, Pong>>().Add<InnerBehaviour>();
                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
                cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
            });

            var mediator = container.GetInstance<IMediator>();

            var response = await mediator.Send(new Ping { Message = "Ping" });

            response.Message.ShouldBe("Ping Pong");

            output.Messages.ShouldBe(new[]
            {
                "Outer before",
                "Inner before",
                "Handler",
                "Inner after",
                "Outer after"
            });
        }

        [Test]
        public async Task Should_wrap_generics_with_behavior()
        {
            var output = new Logger();
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType(typeof(PublishTests));
                    scanner.IncludeNamespaceContainingType<Ping>();
                    scanner.WithDefaultConventions();
                    scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
                });
                cfg.For<Logger>().Singleton().Use(output);

                cfg.For(typeof(IPipelineBehaviour<,>)).Add(typeof(OuterBehaviour<,>));
                cfg.For(typeof(IPipelineBehaviour<,>)).Add(typeof(InnerBehaviour<,>));

                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
                cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
            });

            container.GetAllInstances<IPipelineBehaviour<Ping, Pong>>();

            var mediator = container.GetInstance<IMediator>();

            var response = await mediator.Send(new Ping { Message = "Ping" });

            response.Message.ShouldBe("Ping Pong");

            output.Messages.ShouldBe(new[]
            {
                "Outer generic before",
                "Inner generic before",
                "Handler",
                "Inner generic after",
                "Outer generic after",
            });
        }

        [Test]
        public async Task Should_handle_constrained_generics()
        {
            var output = new Logger();
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType(typeof(PublishTests));
                    scanner.IncludeNamespaceContainingType<Ping>();
                    scanner.WithDefaultConventions();
                    scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
                });
                cfg.For<Logger>().Singleton().Use(output);

                cfg.For(typeof(IPipelineBehaviour<,>)).Add(typeof(OuterBehaviour<,>));
                cfg.For(typeof(IPipelineBehaviour<,>)).Add(typeof(InnerBehaviour<,>));
                cfg.For(typeof(IPipelineBehaviour<,>)).Add(typeof(ConstrainedBehaviour<,>));

                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
                cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
            });

            container.GetAllInstances<IPipelineBehaviour<Ping, Pong>>();

            var mediator = container.GetInstance<IMediator>();

            var response = await mediator.Send(new Ping { Message = "Ping" });

            response.Message.ShouldBe("Ping Pong");

            output.Messages.ShouldBe(new[]
            {
                "Outer generic before",
                "Inner generic before",
                "Constrained before",
                "Handler",
                "Constrained after",
                "Inner generic after",
                "Outer generic after",
            });

            output.Messages.Clear();

            var zingResponse = await mediator.Send(new Zing { Message = "Zing" });

            zingResponse.Message.ShouldBe("Zing Zong");

            output.Messages.ShouldBe(new[]
            {
                "Outer generic before",
                "Inner generic before",
                "Handler",
                "Inner generic after",
                "Outer generic after",
            });
        }

        [Test, Ignore("StructureMap does not mix concrete and open generics. Use constraints instead.")]
        public async Task Should_handle_concrete_and_open_generics()
        {
            var output = new Logger();
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType(typeof(PublishTests));
                    scanner.IncludeNamespaceContainingType<Ping>();
                    scanner.WithDefaultConventions();
                    scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
                });
                cfg.For<Logger>().Singleton().Use(output);

                cfg.For(typeof(IPipelineBehaviour<,>)).Add(typeof(OuterBehaviour<,>));
                cfg.For(typeof(IPipelineBehaviour<,>)).Add(typeof(InnerBehaviour<,>));
                cfg.For(typeof(IPipelineBehaviour<Ping, Pong>)).Add(typeof(ConcreteBehaviour));

                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
                cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
            });

            container.GetAllInstances<IPipelineBehaviour<Ping, Pong>>();

            var mediator = container.GetInstance<IMediator>();

            var response = await mediator.Send(new Ping { Message = "Ping" });

            response.Message.ShouldBe("Ping Pong");

            output.Messages.ShouldBe(new[]
            {
                "Outer generic before",
                "Inner generic before",
                "Concrete before",
                "Handler",
                "Concrete after",
                "Inner generic after",
                "Outer generic after",
            });

            output.Messages.Clear();

            var zingResponse = await mediator.Send(new Zing { Message = "Zing" });

            zingResponse.Message.ShouldBe("Zing Zong");

            output.Messages.ShouldBe(new[]
            {
                "Outer generic before",
                "Inner generic before",
                "Handler",
                "Inner generic after",
                "Outer generic after",
            });
        }
    }
}