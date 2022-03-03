using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sales
{
    public static class Extensions
    {
        public static async Task<T> Deserialize<T>(this HttpResponseMessage responseMessage)
        {
            return JsonConvert.DeserializeObject<T>(await responseMessage.Content.ReadAsStringAsync());
        }
    }
}