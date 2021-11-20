using System.IO;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Pillsgood.Mediator.Tests;

public class SignalHandlerUnitTests
{
    public class Ping : ISignal
    {
        public string Message { get; set; }
    }

    public class PingHandler : SignalHandlerSync<Ping>
    {
        private readonly TextWriter _writer;

        public PingHandler(TextWriter writer)
        {
            _writer = writer;
        }

        protected override void Handle(Ping signal)
        {
            _writer.WriteLine(signal.Message + " Pong");
        }
    }

    [Test]
    public async Task Should_call_abstract_unit_handler()
    {
        var builder = new StringBuilder();
        var writer = new StringWriter(builder);

        ISignalHandler<Ping, Unit> handler = new PingHandler(writer);

        await handler.Handle(new Ping() { Message = "Ping" }, default);

        var result = builder.ToString();
        result.ShouldContain("Ping Pong");
    }
}