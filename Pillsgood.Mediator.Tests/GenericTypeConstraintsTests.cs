using System;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace Pillsgood.Mediator.Tests
{
    public class GenericTypeConstraintsTests
    {
        public interface IGenericTypeSignalHandlerTestClass<TSignal> where TSignal : IBaseSignal
        {
            Type[] Handle(TSignal signal);
        }

        public abstract class
            GenericTypeSignalHandlerTestClass<TSignal> : IGenericTypeSignalHandlerTestClass<TSignal>
            where TSignal : IBaseSignal
        {
            public bool IsISignal { get; }


            public bool IsISignalT { get; }

            public bool IsIBaseSignal { get; }

            public GenericTypeSignalHandlerTestClass()
            {
                IsISignal = typeof(ISignal).IsAssignableFrom(typeof(TSignal));
                IsISignalT = typeof(TSignal).GetInterfaces()
                    .Any(x => x.GetTypeInfo().IsGenericType &&
                              x.GetGenericTypeDefinition() == typeof(ISignal<>));

                IsIBaseSignal = typeof(IBaseSignal).IsAssignableFrom(typeof(TSignal));
            }

            public Type[] Handle(TSignal signal)
            {
                return typeof(TSignal).GetInterfaces();
            }
        }

        public class GenericTypeConstraintPing : GenericTypeSignalHandlerTestClass<Ping>
        {
        }

        public class GenericTypeConstraintJing : GenericTypeSignalHandlerTestClass<Jing>
        {
        }

        public class Jing : ISignal
        {
            public string Message { get; set; }
        }

        public class JingHandler : ISignalHandler<Jing, Unit>
        {
            public Task<Unit> Handle(Jing signal, CancellationToken cancellationToken)
            {
                // empty handle
                return Unit.Default.AsTask();
            }
        }

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

        private readonly IMediator _mediator;

        public GenericTypeConstraintsTests()
        {
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType(typeof(GenericTypeConstraintsTests));
                    scanner.IncludeNamespaceContainingType<Ping>();
                    scanner.IncludeNamespaceContainingType<Jing>();
                    scanner.WithDefaultConventions();
                    scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
                });
                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
                cfg.For<IMediator>().Use<Mediator>();
            });

            _mediator = container.GetInstance<IMediator>();
        }

        [Test]
        public async Task Should_Resolve_Void_Return_Signal()
        {
            // Create Signal
            var jing = new Jing { Message = "Jing" };

            // Test mediator still works sending signal
            await _mediator.Send(jing);

            // Create new instance of type constrained class
            var genericTypeConstraintsVoidReturn = new GenericTypeConstraintJing();

            // Assert it is of type ISignal and ISignal<T>
            Assert.True(genericTypeConstraintsVoidReturn.IsISignal);
            Assert.True(genericTypeConstraintsVoidReturn.IsISignalT);
            Assert.True(genericTypeConstraintsVoidReturn.IsIBaseSignal);

            // Verify it is of ISignal and IBaseSignal and ISignal<Unit>
            var results = genericTypeConstraintsVoidReturn.Handle(jing);

            results.Length.ShouldBe(3);
            results.ShouldContain(typeof(ISignal<Unit>));
            results.ShouldContain(typeof(IBaseSignal));
            results.ShouldContain(typeof(ISignal));
        }

        [Test]
        public async Task Should_Resolve_Response_Return_Signal()
        {
            // Create Signal
            var ping = new Ping { Message = "Ping" };

            // Test mediator still works sending signal and gets response
            var pingResponse = await _mediator.Send(ping);
            pingResponse.Message.ShouldBe("Ping Pong");

            // Create new instance of type constrained class
            var genericTypeConstraintsResponseReturn = new GenericTypeConstraintPing();

            // Assert it is of type ISignal<T> but not ISignal
            Assert.False(genericTypeConstraintsResponseReturn.IsISignal);
            Assert.True(genericTypeConstraintsResponseReturn.IsISignalT);
            Assert.True(genericTypeConstraintsResponseReturn.IsIBaseSignal);

            // Verify it is of ISignal<Pong> and IBaseSignal, but not ISignal
            var results = genericTypeConstraintsResponseReturn.Handle(ping);

            results.Length.ShouldBe(2);

            results.ShouldContain(typeof(ISignal<Pong>));
            results.ShouldContain(typeof(IBaseSignal));
            results.ShouldNotContain(typeof(ISignal));
        }
    }
}