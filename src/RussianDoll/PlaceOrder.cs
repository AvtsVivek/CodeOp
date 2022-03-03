using System;
using Paramore.Brighter;

namespace PipesAndFilters
{
    public class PlaceOrderCommand : IRequest
    {
        public PlaceOrderCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

    public class PlaceOrderHandler : RequestHandler<PlaceOrderCommand>
    {
        [Logging(1)]
        [Filter(2, typeof(RetryHandler<>))]
        [Filter(3, typeof(ValidationHandler<>))]
        public override PlaceOrderCommand Handle(PlaceOrderCommand command)
        {
            Console.WriteLine($"Placing Order: {command.Id}");
            return base.Handle(command);
        }
    }

    public class RetryHandler<TRequest>
        : RequestHandler<TRequest> where TRequest : PlaceOrderCommand
    {
        public override TRequest Handle(TRequest command)
        {
            Console.WriteLine("Retry Handler Executing");

            try
            {
                return base.Handle(command);
            }
            catch (InvalidOperationException)
            {
                return base.Handle(command);
            }
        }
    }

    public class ValidationHandler<TRequest>
        : RequestHandler<TRequest> where TRequest : PlaceOrderCommand
    {
        public override TRequest Handle(TRequest command)
        {
            Console.WriteLine($"Validating {nameof(PlaceOrderCommand)}");

            if (command.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Invalid Order Id.");
            }

            return base.Handle(command);
        }
    }
}