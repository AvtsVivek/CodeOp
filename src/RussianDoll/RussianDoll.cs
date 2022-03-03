using System;

namespace PipesAndFilters
{
    public class RussianDoll
    {
        public void Pump(string request)
        {
            Logging(request, o1 =>
                Validation(o1, o2 =>
                    Retry(o2, o3 =>
                        Receiver(o3, o4 => { }))));
        }

        private void Logging(string request, Action<string> nextFilter)
        {
            Console.WriteLine("Logging Request");
            nextFilter(request);
        }

        private void Validation(string request, Action<string> nextFilter)
        {
            if (request == "Hello World")
            {
                nextFilter(request);
            }
        }

        private void Retry(string request, Action<string> nextFilter)
        {
            try
            {
                nextFilter(request);
            }
            catch (InvalidOperationException)
            {
                nextFilter(request);
            }
        }

        private void Receiver(string request, Action<string> nextFilter)
        {
            Console.WriteLine($"Thanks for the request: {request}");

            nextFilter(request);
        }
    }
}