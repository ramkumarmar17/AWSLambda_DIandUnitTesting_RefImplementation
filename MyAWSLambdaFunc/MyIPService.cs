using System.Net.Http;
using System.Net;

namespace MyAWSLambdaFunc
{
    public interface IMyIPService
    {
        string GetIPInfo(string ipAddress);
    }

    public class MyIPService : IMyIPService
    {
        private readonly string _urlTemplate;
        private readonly string _templateIPAddress = "0.0.0.0";

        public MyIPService(string urlTemplate)
        {
            _urlTemplate = urlTemplate;
			
			//Sample urlTemplate: https://ipinfo.io/0.0.0.0/geo
        }

        public string GetIPInfo(string ipAddress)
        {
            IPAddress ip;

            bool ValidateIP = IPAddress.TryParse(ipAddress, out ip);
            if (!ValidateIP)
                return "Invalid IP address";

            var requestUri = _urlTemplate.Replace(_templateIPAddress, ipAddress);

            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetStringAsync(requestUri).Result;
                return response;
            }
        }
    }
}
