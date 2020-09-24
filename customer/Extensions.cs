namespace Customer
{
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Http;
    using System.Text;
    using System.Collections.Generic;

    public static class IHttpClientExtensions
    {
        public static async Task<T> GetFromJsonAsync<T>(this HttpClient client, string uri)
        {
            var content = await client.GetStringAsync(uri);

            return JsonConvert.DeserializeObject<T>(content);
        }

        public static Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, string uri, T body)
        {
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            return client.PostAsync(uri, content);
        }

        public static async Task<T> ReadContentAsJson<T>(this HttpContent content)
        {
            var stringContent = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringContent);
        }
    }

    public static class CollectionExtensions
    {
        public static List<T> AddToList<T>(this List<T> list, T item)
        {
            list.Add(item);
            return list;
        }
    }
}