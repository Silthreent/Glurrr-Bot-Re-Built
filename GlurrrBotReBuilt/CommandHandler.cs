using Discord;
using Discord.WebSocket;
using GlurrrBotReBuilt.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GlurrrBotReBuilt
{
    class CommandHandler
    {
        public static async Task MessageRecieved(SocketMessage message)
        {
            if(message.Content.ToLower().Contains("monika"))
            {
                await ParseMessage(message);
            }
        }

        static async Task ParseMessage(SocketMessage message)
        {
            string lower = message.Content.ToLower();

            if(lower.Contains("randome"))
            {
                await Randome.RunCommand(message);
            }
            if(lower.Contains("anime"))
            {
                await message.Channel.SendMessageAsync("I love anime");
            }
        }
    }
}
