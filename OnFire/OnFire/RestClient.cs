using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnFire.Net
{
    public class RestClient
    {
        public static HttpClient Client { get; private set; }
        public RestClient()
        {
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SetTcpKeepAlive(false, 0, 0);
            ServicePointManager.DefaultConnectionLimit = 100000;

            HttpClientHandler hch = new HttpClientHandler();
            hch.Proxy = null;
            hch.UseProxy = false;
            Client = new HttpClient(hch);
        }
        public async Task<T> Get<T>(string URL)
        {
            HttpResponseMessage result = await Client.GetAsync(URL);
            Stream stream = await result.Content.ReadAsStreamAsync();
            T responseObject = await JsonSerializer.DeserializeAsync<T>(stream);
            return responseObject;

        }

        public async Task<HttpResponseMessage> Put<T>(string URL, T Content)
        {
            string jsonString = JsonSerializer.Serialize(Content);
            HttpResponseMessage result = await Client.PutAsync(URL,
                new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json"));
            return result;

        }

        public async Task<HttpResponseMessage> Post<T>(string URL, T Content)
        {
            string jsonString = JsonSerializer.Serialize(Content);
            HttpResponseMessage result = await Client.PostAsync(URL,
                new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json"));
            return result;

        }

        public async Task<HttpResponseMessage> Delete(string URL)
        {
            HttpResponseMessage result = await Client.DeleteAsync(URL);
            return result;
        }
    }
}
