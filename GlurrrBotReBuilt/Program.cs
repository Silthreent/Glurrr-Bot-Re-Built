using Discord;
using Discord.WebSocket;
using Discord.Audio;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using GlurrrBotReBuilt.Commands;

namespace GlurrrBotReBuilt
{
    class Program
    {
        public const string DATABASE = "GlurrrBot.db";
        public const string CHR_DATABASE = "GlurrrBotChr.db";
        public static PythonEngine python;

        const string VERSION_NUMBER = "0.0.1";

        public static DiscordSocketClient client;

        DateTime lastWelcome;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            Console.WriteLine("Running GlurrrBot Re;Built V" + VERSION_NUMBER);

            python = new PythonEngine();

            client = new DiscordSocketClient();
            client.Log += Log;
            client.MessageReceived += MessageRecieved;
            client.GuildAvailable += GuildAvailable;

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
                else if(line == "poem")
                {
                    Console.WriteLine("Poem Writing Mode");
                    PoemTime.NewPoem();
                }
                else if(line == "game")
                {
                    Console.Write("Changing Game: ");
                    line = Console.ReadLine();

                    await client.SetGameAsync(line);

                    Console.WriteLine("Game changed to " + line);
                }
                else
                {
                    Task.Run(() => Character.WriteChat(line, client.GetGuild(285703929189367811).DefaultChannel));
                }
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());

            return Task.CompletedTask;
        }

        private async Task GuildAvailable(SocketGuild guild)
        {
            Console.WriteLine("Guild available");
            if(lastWelcome.Year == 0001)
            {
                await Character.SendMessage("welcome", "Bot loaded", guild.DefaultChannel);
                lastWelcome = DateTime.Now;
            }

            if(lastWelcome.CompareTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1)) > 0)
            {
                await Character.SendMessage("welcome", "Bot loaded", guild.DefaultChannel);
                lastWelcome = DateTime.Now;
            }
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
