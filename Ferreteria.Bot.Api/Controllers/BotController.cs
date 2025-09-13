using Microsoft.AspNetCore.Mvc;
using Ferreteria.Bot.Api.Stores;

namespace Ferreteria.Bot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BotController : ControllerBase
    {
        [HttpPost("process")]
        public IActionResult Process([FromBody] BotRequest request)
        {
            // Log inicial
            Console.WriteLine($"[API] Mensaje recibido de {request.From}: {request.Message}");

            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { reply = "Mensaje vacío o inválido." });
            }

            // Simulamos baja confianza
            bool bajaConfianza = true;

            if (bajaConfianza)
            {
                // Guardamos en el mock de "handover"
                HandoverStore.Handovers.Add(request);

                Console.WriteLine($"[HANDOVER] Escalando mensaje de {request.From}: {request.Message}");
                return Ok(new { reply = "Lo siento, no estoy seguro. Un humano te atenderá pronto." });
            }

            // Respuesta normal
            var response = new
            {
                reply = $"Hola {request.From}, recibí tu mensaje: {request.Message}"
            };

            return Ok(response);
        }

        // Nuevo endpoint para ver los mensajes escalados
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
