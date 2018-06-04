using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GlurrrBotReBuilt
{
    class PythonMethods
    {
        public static void WriteChat(string message, ISocketMessageChannel channel)
        {
            Task.Run(() => Character.WriteChat(message, channel));
        }
    }
}
