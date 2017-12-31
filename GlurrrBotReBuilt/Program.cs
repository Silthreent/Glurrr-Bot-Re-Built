﻿using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace GlurrrBotReBuilt
{
    class Program
    {
        public const string DATABASE = "GlurrrBot.db";
        public const string CHR_DATABASE = "GlurrrBotChr.db";
        const string VERSION_NUMBER = "0.0.0";

        public static DiscordSocketClient client;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            Console.WriteLine("Running GlurrrBot Re;Built V" + VERSION_NUMBER);

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
                else
                {
                    await client.GetGuild(285703929189367811).TextChannels.ElementAt(0).SendMessageAsync(line);
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
            await Character.SendMessage("welcome", guild.TextChannels.ElementAt(0));
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
