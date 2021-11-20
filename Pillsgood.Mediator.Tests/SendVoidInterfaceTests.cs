using System.IO;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace Pillsgood.Mediator.Tests
{
    public class SendVoidInterfaceTests
    {
        public class Ping : ISignal
        {
            public string Message { get; set; }
        }

        public class PingHandler : ISignalHandler<Ping>
        {
            private readonly TextWriter _writer;

            public PingHandler(TextWriter writer) => _writer = writer;
            
            public Task<Unit> Handle(Ping signal, CancellationToken cancellationToken)
            {
                _writer.WriteAsync(signal.Message + " Pong");
                return Unit.Default.AsTask();
            }
        }

        [Test]
        public async Task Should_resolve_main_void_handler()
        {
            var builder = new StringBuilder();
            var writer = new StringWriter(builder);

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
                cfg.For<TextWriter>().Use(writer);
                cfg.For<IMediator>().Use<Mediator>();
            });


            var mediator = container.GetInstance<IMediator>();

            await mediator.Send(new Ping { Message = "Ping" });

            builder.ToString().ShouldBe("Ping Pong");
        }
    }
}