using System;
using Paramore.Brighter;

namespace PipesAndFilters
{
    public class LoggingAttribute : RequestHandlerAttribute
    {
        public LoggingAttribute(int step) : base(step) { }

        public override object[] InitializerParams()
        {
            return new object[] { Timing };
        }

        public override Type GetHandlerType()
        {
            return typeof(LoggingHandler<>);
        }
    }

    public class LoggingHandler<TRequest>
        : RequestHandler<TRequest> where TRequest : class, IRequest
    {
        public override TRequest Handle(TRequest command)
        {
            Console.WriteLine($"Generic Logging {typeof(TRequest)}");
            return base.Handle(command);
        }
    }
}