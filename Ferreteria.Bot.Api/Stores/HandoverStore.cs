using Ferreteria.Bot.Api.Controllers;
using System.Collections.Generic;

namespace Ferreteria.Bot.Api.Stores
{
    public static class HandoverStore
    {
        // Lista estática que guarda los mensajes escalados
        public static List<BotRequest> Handovers { get; } = new List<BotRequest>();
    }
}
