using System;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;

namespace PipesAndFilters
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<PlaceOrderHandler>();
            serviceCollection.AddTransient<LoggingHandler<PlaceOrderCommand>>();
            serviceCollection.AddTransient<ValidationHandler<PlaceOrderCommand>>();
            serviceCollection.AddTransient<RetryHandler<PlaceOrderCommand>>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var registry = new SubscriberRegistry();
            registry.Register<PlaceOrderCommand, PlaceOrderHandler>();

            var builder = CommandProcessorBuilder.With()
                .Handlers(new HandlerConfiguration(
                    subscriberRegistry: registry,
                    handlerFactory: new HandlerFactory(serviceProvider)
                ))
                .DefaultPolicy()
                .NoTaskQueues()
                .RequestContextFactory(new InMemoryRequestContextFactory());

            var commandProcessor = builder.Build();
            commandProcessor.Send(new PlaceOrderCommand(Guid.NewGuid()));
        }
    }


}
