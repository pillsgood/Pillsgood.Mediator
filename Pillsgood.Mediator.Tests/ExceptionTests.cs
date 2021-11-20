using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace Pillsgood.Mediator.Tests;

public class ExceptionTests
{
    private readonly IMediator _mediator;

    public class Ping : ISignal<Pong>
    {
    }

    public class Pong
    {
    }

    public class VoidPing : ISignal
    {
    }

    public class Pinged : INotification
    {
    }

    public class AsyncPing : ISignal<Pong>
    {
    }

    public class AsyncVoidPing : ISignal
    {
    }

    public class AsyncPinged : INotification
    {
    }

    public class NullPing : ISignal<Pong>
    {
    }

    public class VoidNullPing : ISignal
    {
    }

    public class NullPinged : INotification
    {
    }

    public class NullPingHandler : ISignalHandler<NullPing, Pong>
    {
        public Task<Pong> Handle(NullPing signal, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Pong());
        }
    }

    public class VoidNullPingHandler : ISignalHandler<VoidNullPing, Unit>
    {
        public Task<Unit> Handle(VoidNullPing signal, CancellationToken cancellationToken)
        {
            return Task.FromResult(Unit.Default);
        }
    }

    public ExceptionTests()
    {
        var container = new Container(cfg =>
        {
            cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
            cfg.For<IMediator>().Use<Mediator>()
                .SelectConstructor(() => new Mediator(default!));
        });
        _mediator = container.GetInstance<IMediator>();
    }

    [Test]
    public async Task Should_throw_for_send()
    {
        await Should.ThrowAsync<InvalidOperationException>(async () => await _mediator.Send(new Ping()));
    }

    [Test]
    public async Task Should_throw_for_void_send()
    {
        await Should.ThrowAsync<InvalidOperationException>(async () => await _mediator.Send(new VoidPing()));
    }

    [Test]
    public async Task Should_not_throw_for_publish()
    {
        Exception ex = null;
        try
        {
            await _mediator.Publish(new Pinged());
        }
        catch (Exception e)
        {
            ex = e;
        }

        ex.ShouldBeNull();
    }

    [Test]
    public async Task Should_throw_for_async_send()
    {
        await Should.ThrowAsync<InvalidOperationException>(async () => await _mediator.Send(new AsyncPing()));
    }

    [Test]
    public async Task Should_throw_for_async_void_send()
    {
        await Should.ThrowAsync<InvalidOperationException>(async () => await _mediator.Send(new AsyncVoidPing()));
    }

    [Test]
    public async Task Should_not_throw_for_async_publish()
    {
        Exception ex = null;
        try
        {
            await _mediator.Publish(new AsyncPinged());
        }
        catch (Exception e)
        {
            ex = e;
        }

        ex.ShouldBeNull();
    }

    [Test]
    public async Task Should_throw_argument_exception_for_send_when_signal_is_null()
    {
        var container = new Container(cfg =>
        {
            cfg.Scan(scanner =>
            {
                scanner.AssemblyContainingType(typeof(NullPing));
                scanner.IncludeNamespaceContainingType<Ping>();
                scanner.WithDefaultConventions();
                scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
            });
            cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
            cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
        });
        var mediator = container.GetInstance<IMediator>();

        NullPing signal = null;

        await Should.ThrowAsync<ArgumentNullException>(async () => await mediator.Send(signal));
    }

    [Test]
    public async Task Should_throw_argument_exception_for_void_send_when_signal_is_null()
    {
        var container = new Container(cfg =>
        {
            cfg.Scan(scanner =>
            {
                scanner.AssemblyContainingType(typeof(VoidNullPing));
                scanner.IncludeNamespaceContainingType<Ping>();
                scanner.WithDefaultConventions();
                scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
            });
            cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
            cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
        });
        var mediator = container.GetInstance<IMediator>();

        VoidNullPing signal = null;

        await Should.ThrowAsync<ArgumentNullException>(async () => await mediator.Send(signal));
    }

    [Test]
    public async Task Should_throw_argument_exception_for_publish_when_signal_is_null()
    {
        var container = new Container(cfg =>
        {
            cfg.Scan(scanner =>
            {
                scanner.AssemblyContainingType(typeof(NullPinged));
                scanner.IncludeNamespaceContainingType<Ping>();
                scanner.WithDefaultConventions();
                scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
            });
            cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
            cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
        });
        var mediator = container.GetInstance<IMediator>();

        NullPinged notification = null;

        await Should.ThrowAsync<ArgumentNullException>(async () => await mediator.Publish(notification));
    }

    [Test]
    public async Task Should_throw_argument_exception_for_publish_when_signal_is_null_object()
    {
        var container = new Container(cfg =>
        {
            cfg.Scan(scanner =>
            {
                scanner.AssemblyContainingType(typeof(NullPinged));
                scanner.IncludeNamespaceContainingType<Ping>();
                scanner.WithDefaultConventions();
                scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
            });
            cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
            cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
        });
        var mediator = container.GetInstance<IMediator>();

        object notification = null;

        await Should.ThrowAsync<ArgumentNullException>(async () => await mediator.Publish(notification));
    }

    [Test]
    public async Task Should_throw_argument_exception_for_publish_when_signal_is_not_notification()
    {
        var container = new Container(cfg =>
        {
            cfg.Scan(scanner =>
            {
                scanner.AssemblyContainingType(typeof(NullPinged));
                scanner.IncludeNamespaceContainingType<Ping>();
                scanner.WithDefaultConventions();
                scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
            });
            cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
            cfg.For<IMediator>().Use<Mediator>()
                .SelectConstructor(() => new Mediator(default!));
        });
        var mediator = container.GetInstance<IMediator>();

        object notification = "totally not notification";

        await Should.ThrowAsync<ArgumentException>(async () => await mediator.Publish(notification));
    }

    public class PingException : ISignal
    {
    }

    public class PingExceptionHandler : ISignalHandler<PingException>
    {
        public Task<Unit> Handle(PingException signal, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    [Test]
    public async Task Should_throw_exception_for_non_generic_send_when_exception_occurs()
    {
        var container = new Container(cfg =>
        {
            cfg.Scan(scanner =>
            {
                scanner.AssemblyContainingType(typeof(NullPinged));
                scanner.IncludeNamespaceContainingType<Ping>();
                scanner.WithDefaultConventions();
                scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
            });
            cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
            cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
        });
        var mediator = container.GetInstance<IMediator>();

        object pingException = new PingException();

        await Should.ThrowAsync<NotImplementedException>(async () => await mediator.Send(pingException));
    }

    [Test]
    public async Task Should_throw_exception_for_non_signal_send()
    {
        var container = new Container(cfg =>
        {
            cfg.Scan(scanner =>
            {
                scanner.AssemblyContainingType(typeof(NullPinged));
                scanner.IncludeNamespaceContainingType<Ping>();
                scanner.WithDefaultConventions();
                scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
            });
            cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
            cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
        });
        var mediator = container.GetInstance<IMediator>();

        object nonSignal = new NonSignal();

        var argumentException =
            await Should.ThrowAsync<ArgumentException>(async () => await mediator.Send(nonSignal));
        argumentException.Message.ShouldStartWith("NonSignal does not implement ISignal");
    }

    public class NonSignal
    {
    }

    [Test]
    public async Task Should_throw_exception_for_generic_send_when_exception_occurs()
    {
        var container = new Container(cfg =>
        {
            cfg.Scan(scanner =>
            {
                scanner.AssemblyContainingType(typeof(NullPinged));
                scanner.IncludeNamespaceContainingType<Ping>();
                scanner.WithDefaultConventions();
                scanner.AddAllTypesOf(typeof(ISignalHandler<,>));
            });
            cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
            cfg.For<IMediator>().Use<Mediator>().SelectConstructor(() => new Mediator(default!));
        });
        var mediator = container.GetInstance<IMediator>();

        PingException pingException = new PingException();

        await Should.ThrowAsync<NotImplementedException>(async () => await mediator.Send(pingException));
    }
}