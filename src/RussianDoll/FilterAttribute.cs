using System;
using Paramore.Brighter;

namespace PipesAndFilters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class FilterAttribute : RequestHandlerAttribute
    {
        private readonly Type _handlerType;

        public FilterAttribute(int step, Type handlerType, HandlerTiming timing = HandlerTiming.Before) : base(step, timing)
        {
            _handlerType = handlerType;
        }

        public override Type GetHandlerType()
        {
            return _handlerType;
        }
    }
}