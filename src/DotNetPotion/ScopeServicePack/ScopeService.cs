using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetPotion.ScopeServicePack
{
    public class ScopeService : IScopeService
    {
        private readonly IServiceProvider _serviceProvider;

        public ScopeService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<T> Run<T>(IRequest<T> request)
        {
            return Run(async scope =>
            {
                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                return await mediator.Send(request);
            });
        }

        public Task Run(IRequest request)
        {
            return Run(async scope =>
            {
                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(request);
            });
        }

        public Task Run(params INotification[] events)
        {
            return Run(async scope =>
            {
                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                IEnumerable<Task> tasks = events.Select(@event => mediator.Publish(@event));
                await Task.WhenAll(tasks);
            });
        }

        public Task Run(Func<IServiceScope, Task> function)
        {
            return Task.Run(async () =>
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    await function(scope);
                }
            });
        }

        public Task<T> Run<T>(Func<IServiceScope, Task<T>> function)
        {
            return Task.Run(async () =>
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    return await function(scope);
                }
            });
        }

        public void FireAndForget<T>(IRequest<T> command)
        {
            _ = Run(command);
        }

        public void FireAndForget(IRequest command)
        {
            _ = Run(command);
        }

        public void FireAndForget(params INotification[] @event)
        {
            _ = Run(@event);
        }

        public void FireAndForget(Func<IServiceScope, Task> function)
        {
            _ = Run(function);
        }

        public void FireAndForget<T>(Func<IServiceScope, Task<T>> function)
        {
            _ = Run(function);
        }
    }
}