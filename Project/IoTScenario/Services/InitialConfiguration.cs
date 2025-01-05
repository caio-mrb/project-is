using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public class InitialConfiguration
{
    private static readonly HttpClient client = new HttpClient();

    public async Task InitializeContainerAsync()
    {
        try
        {
            // URL base da API do middleware
            string baseUrl = "http://localhost:60626/api/somiod";

            // 1. Criar aplicação
            string applicationName = "Lighting";
            string applicationUrl = baseUrl;
            string applicationBody = $"<Application><Name>{applicationName}</Name></Application>";
            await PostToApiAsync(applicationUrl, applicationBody);

            // 2. Criar container
            string containerName = "Light_bulb";
            string containerUrl = $"{baseUrl}/{applicationName}";
            string containerBody = $"<Container><Name>{containerName}</Name></Container>";
            await PostToApiAsync(containerUrl, containerBody);

             
            string notificationUrl = $"{baseUrl}/{applicationName}/{containerName}";
            string notificationBody = @"
                <Notification>
                    <Event>Creation</Event>
                    <Endpoint>mqtt://localhost:5000/notify</Endpoint>
                    <Enabled>true</Enabled>
                </Notification>";
            await PostToApiAsync(notificationUrl, notificationBody);

            Console.WriteLine("Notificação configurada com sucesso.");
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
