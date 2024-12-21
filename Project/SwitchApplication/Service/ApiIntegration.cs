using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SwitchApplication.Service
{
    public class ApiIntegration
    {
        private static readonly HttpClient client = new HttpClient();
        public readonly string baseUrl = "http://localhost:60626/api/somiod";

        public async Task InitializeApplicationAsync()
        {     
            try
            {
                // 1. Criar aplicação "Lighting"
                string applicationName = "Switch";
                string applicationUrl = baseUrl;
                string creationDateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                string applicationBody = $"<Application>" +
                    $"^<CreationDateTime>{creationDateTime}</CreationDateTime>" +
                    $"</Application>";
                await PostToApiAsync(applicationUrl, applicationBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar os recursos: {ex.Message}");
            }
        }

        public async Task SendRecordAsync(string recordValue)
        {
            try
            {
                string applicationName = "lighting";
                string containerName = "light_bulb";
                string creationDateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");  // Exemplo de timestamp no formato "yyyyMMdd_HHmmss"
                string nameWithTimestamp = $"{applicationName}_{timestamp}";
                string applicationBody =
                            $"<Record>" +
                                $"<Content>{recordValue}</Content>" +
                            $"</Record>";
                string postUrl = baseUrl + $"/{applicationName}/{containerName}/record";
                await PostToApiAsync(postUrl, applicationBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar os recursos: {ex.Message}");
            }
        }

        private async Task PostToApiAsync(string url, string body)
        {
            StringContent content = new StringContent(body, Encoding.UTF8, "application/xml");

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro na requisição HTTP. Status: {response.StatusCode}, Detalhes: {error}");
            }
        }
    }
}
