using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GlurrrBotReBuilt
{
    class Program
    {
        const string VERSION_NUMBER = "0.0.0";

        public static DiscordSocketClient client;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            Console.WriteLine("Running GlurrrBot Re;Built V" + VERSION_NUMBER);

            client = new DiscordSocketClient();
            client.Log += Log;
            client.MessageReceived += MessageRecieved;

            try
            {
                using(StreamReader sr = new StreamReader("botcode.txt"))
                {
                    await client.LoginAsync(TokenType.Bot, sr.ReadLine());
                }
            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine("Code file not found");
                Console.WriteLine(e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine("Something crazy happened");
                Console.WriteLine(e.Message);
            }

            await client.StartAsync();

            string line;
            while(true)
            {
                line = Console.ReadLine();
                if(line == "kys")
                {
                    break;
                }
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());

            return Task.CompletedTask;
        }

        private async Task MessageRecieved(SocketMessage message)
        {
            if(message.Author.IsBot)
            {
                return;
            }

            Task.Run(() => CommandHandler.MessageRecieved(message));
        }
    }
}
