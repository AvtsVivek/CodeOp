using System;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;

namespace PipesAndFilters
{
    internal class HandlerFactory : IAmAHandlerFactory
    {
        private readonly ServiceProvider _serviceProvider;

        public HandlerFactory(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IHandleRequests Create(Type handlerType)
        {
            return _serviceProvider.GetRequiredService(handlerType) as IHandleRequests;
        }

        public void Release(IHandleRequests handler) { }
    }
}