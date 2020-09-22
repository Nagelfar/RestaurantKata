namespace Customer
{
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class IHttpClientExtensions
    {
        public static async Task<T> GetFromJsonAsync<T>(this HttpClient client, string uri)
        {
            var content = await client.GetStringAsync(uri);

            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}