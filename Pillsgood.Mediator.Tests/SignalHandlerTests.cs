using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Pillsgood.Mediator.Tests
{
    public class SignalHandlerTests
    {
        public class Ping : ISignal<Pong>
        {
            public string Message { get; set; }
        }

        public class Pong
        {
            public string Message { get; set; }
        }

        public class PingHandler : SignalHandlerSync<Ping, Pong>
        {
            protected override Pong Handle(Ping signal)
            {
                return new Pong { Message = signal.Message + " Pong" };
            }
        }

        [Test]
        public async Task Should_call_abstract_handler()
        {
            ISignalHandler<Ping, Pong> handler = new PingHandler();

            var response = await handler.Handle(new Ping() { Message = "Ping" }, default);

            response.Message.ShouldBe("Ping Pong");
        }
    }
}