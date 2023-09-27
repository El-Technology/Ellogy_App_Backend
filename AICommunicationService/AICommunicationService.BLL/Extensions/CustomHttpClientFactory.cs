namespace AICommunicationService.BLL.Extensions
{
    public class CustomHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(100)
            };

            return httpClient;
        }
    }
}
