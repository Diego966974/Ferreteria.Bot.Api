using System.Net.Http.Json;
using System.Threading.Tasks;

public class MessageRequest
{
    public string Message { get; set; }
    public string CustomerPhone { get; set; }
}

public class AIResponse
{
    public string Type { get; set; }
    public string Reply { get; set; }
    public string Product { get; set; }
    public int? Quantity { get; set; }
    public string Address { get; set; }
}

public class AIClient
{
    private readonly HttpClient _http;

    public AIClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<AIResponse?> ProcessMessage(string message, string phone = null)
    {
        var req = new MessageRequest { Message = message, CustomerPhone = phone };
        var resp = await _http.PostAsJsonAsync("/process", req);

        resp.EnsureSuccessStatusCode();

        return await resp.Content.ReadFromJsonAsync<AIResponse>();
    }
}
