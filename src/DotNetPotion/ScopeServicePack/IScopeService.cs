using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetPotion.ScopeServicePack
{
    public interface IScopeService
    {
        Task<T> Run<T>(IRequest<T> request);

        Task Run(IRequest request);

        Task Run(params INotification[] events);

        Task Run(Func<IServiceScope, Task> function);

        Task<T> Run<T>(Func<IServiceScope, Task<T>> function);

        void FireAndForget<T>(IRequest<T> command);

        void FireAndForget(IRequest command);

        void FireAndForget(params INotification[] @event);

        void FireAndForget(Func<IServiceScope, Task> function);

        void FireAndForget<T>(Func<IServiceScope, Task<T>> function);
    }
}