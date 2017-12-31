﻿using Discord;
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
        static bool wishedNewYear = false;

        public static async Task MessageRecieved(SocketMessage message)
        {
            if(!wishedNewYear && DateTime.Today.Year == 2018)
            {
                await message.Channel.SendMessageAsync("Happy New Year! I hope 2018 is filled with more poems with my friends!");
                wishedNewYear = true;
            }

            if(message.Content.ToLower().Contains("monika"))
            {
                await ParseMessage(message);
            }
        }

        static async Task ParseMessage(SocketMessage message)
        {
            string lower = message.Content.ToLower();

            if(message.Author.Id == 134852512611172352)
            {
                Console.WriteLine("Master talked");
                if(lower.Contains("character"))
                {
                    Console.WriteLine("Modifying .chr");
                    if(lower.Contains("edit"))
                        await Character.AddCharacterLine(message);

                    return;
                }
            }

            if(lower.Contains("channel list"))
            {
                Console.WriteLine("-Channel List-");
                foreach(SocketGuildChannel channel in Program.client.GetGuild(285703929189367811).Channels)
                {
                    Console.WriteLine(channel.Name);
                }
                Console.WriteLine("Default: " + Program.client.GetGuild(285703929189367811).DefaultChannel);
            }

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
