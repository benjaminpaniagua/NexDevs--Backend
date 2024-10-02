namespace NexDevs.Models
{
    public class NetworkAPI
    {
        public HttpClient Initial()
        {
            //objeto HttpClient
            var client = new HttpClient();

            //URL API
            //client.BaseAddress = new Uri("https://localhost:7094/");

            return client;
        }
    }
}
