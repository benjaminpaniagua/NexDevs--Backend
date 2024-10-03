namespace NexDevs.Models
{
    public class NetworkAPI
    {
        public HttpClient Initial()
        {
            //objeto HttpClient
            var client = new HttpClient();

            //URL API
            client.BaseAddress = new Uri("http://localhost:5146/");

            return client;
        }
    }
}
