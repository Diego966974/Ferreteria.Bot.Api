using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class AIService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _openAiApiKey;

    public AIService(IHttpClientFactory httpClientFactory, string openAiApiKey)
    {
        _httpClientFactory = httpClientFactory;
        _openAiApiKey = openAiApiKey;
    }

    public async Task<AIResult> ProcesarMensajeAsync(string mensaje)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _openAiApiKey);

        var payload = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = $@"
Clasifica el siguiente mensaje como 'FAQ' o 'Pedido'.
Extrae las entidades: producto, cantidad, dirección, teléfono.
Devuelve SOLO un JSON con la clasificación y las entidades.
Mensaje: {mensaje}"
                }
             }
        };

        var jsonPayload = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", jsonPayload);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        // Extraer el JSON de la respuesta del modelo
        var doc = JsonDocument.Parse(responseContent);
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        // Si content viene nulo, devolvemos un AIResult vacío para evitar errores
        if (string.IsNullOrWhiteSpace(content))
        {
            return new AIResult
            {
                Clasificacion = "Desconocido",
                Producto = null,
                Cantidad = null,
                Direccion = null,
                Telefono = null,
                EsPedido = false
            };
        }

        // Deserializar el JSON que devuelve OpenAI
        var aiResult = JsonSerializer.Deserialize<AIResult>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (aiResult == null)
        {
            // Si no se pudo deserializar, devolvemos un objeto vacío
            return new AIResult
            {
                Clasificacion = "Error",
                EsPedido = false
            };
        }

        // Lógica simple: si hay producto y cantidad → pedido
        aiResult.EsPedido = !string.IsNullOrEmpty(aiResult.Producto) && !string.IsNullOrEmpty(aiResult.Cantidad);

        return aiResult;
    }
}

public class AIResult
{
    public string? Clasificacion { get; set; }
    public string? Producto { get; set; }
    public string? Cantidad { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public bool EsPedido { get; set; }
}

