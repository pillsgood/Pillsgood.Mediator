using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Pillsgood.Mediator.Tests
{
    public class ServiceFactoryTests
    {
        public class Ping : ISignal<Pong>
        {
        }

        public class Pong
        {
            public string Message { get; set; }
        }

        [Test]
        public void Should_throw_given_no_handler()
        {
            var serviceFactory = new ServiceFactory(type => typeof(IEnumerable).IsAssignableFrom(type)
                ? Array.CreateInstance(type.GetGenericArguments().First(), 0)
                : null);

            var mediator = new Mediator(serviceFactory);

            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(new Ping()));

            exception.ShouldNotBeNull();
            exception.Message.ShouldStartWith("Handler was not found for signal");
        }
    }
}