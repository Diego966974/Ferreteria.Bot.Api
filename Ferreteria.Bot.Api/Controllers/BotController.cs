using Microsoft.AspNetCore.Mvc;
using Ferreteria.Bot.Api.Stores;
using System.Threading.Tasks;

namespace Ferreteria.Bot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BotController : ControllerBase
    {
        private readonly AIClient _aiClient;

        public BotController(AIClient aiClient)
        {
            _aiClient = aiClient;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] BotRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { reply = "Mensaje vacío o inválido." });
            }

            // Llamada al servicio AI en Python
            var aiResponse = await _aiClient.ProcessMessage(request.Message, request.From);

            if (aiResponse == null || string.IsNullOrWhiteSpace(aiResponse.Reply))
            {
                // Mensaje no confiable → enviar a handover
                HandoverStore.Handovers.Add(request);
                return Ok(new { reply = "Lo siento, no estoy seguro. Un humano te atenderá pronto." });
            }

            // Respuesta confiable de AI
            return Ok(new { reply = aiResponse.Reply });
        }

        // Endpoint para ver los mensajes escalados
        [HttpGet("handovers")]
        public IActionResult GetHandovers()
        {
            return Ok(HandoverStore.Handovers);
        }
    }

    public class BotRequest
    {
        public required string Message { get; set; }
        public required string From { get; set; }
    }
}
