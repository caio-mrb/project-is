using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class InitialConfiguration
{
    private static readonly HttpClient client = new HttpClient();

    public async Task InitializeContainerAsync()
    {
        try
        {
            // URL base da API do middleware
            string baseUrl = "http://localhost:60626/api/somiod";

            // 1. Criar aplicação "Lighting"
            string applicationName = "Lighting";
            string applicationUrl = baseUrl;
            string creationDateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string applicationBody = $"<Application>" +
                $"<Name>{applicationName}</Name>" +
                $"^<CreationDateTime>{creationDateTime}</CreationDateTime>" +
                $"</Application>";
            await PostToApiAsync(applicationUrl, applicationBody);

            // 2. Criar container "light_bulb" na aplicação "Lighting"
            string containerName = "light_bulb";
            string containerUrl = $"{baseUrl}/{applicationName}";
            string containerBody = $"<Container>" +
                $"<Name>{containerName}</Name>" +
                 $"^<CreationDateTime>{creationDateTime}</CreationDateTime>" +
                $"</Container>";
            await PostToApiAsync(containerUrl, containerBody);

             
            string notificationUrl = $"{baseUrl}/{applicationName}/{containerName}";
            string notificationBody = @"
                <Notification>
                    <Name>light_bulb_creation</Name>
                    <Event>1</event>" +
                    $"^<CreationDateTime>{creationDateTime}</CreationDateTime>" +
                    @"<Endpoint>http://localhost:5000/notify</Endpoint>
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
        // Cria o conteúdo da requisição com o corpo XML
        StringContent content = new StringContent(body, Encoding.UTF8, "application/xml");

        // Envia o POST para a API
        HttpResponseMessage response = await client.PostAsync(url, content);

        // Verifica se a resposta foi bem-sucedida
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro na requisição HTTP. Status: {response.StatusCode}, Detalhes: {error}");
        }
    }
}
