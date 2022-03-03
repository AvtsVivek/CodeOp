using SimpleCQRS;

namespace CQRSGui
{
    public static class ServiceLocator
    {
        public static MessageDispatcher MessageDispatcher { get; set; }
    }
}