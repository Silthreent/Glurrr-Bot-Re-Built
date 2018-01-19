using Discord;
using Discord.WebSocket;
using Discord.Audio;
using GlurrrBotReBuilt.Commands;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            else
            {
                if(message.Content.ToLower() == message.Channel.GetMessagesAsync(2).Flatten().Result.ElementAt(1).Content.ToLower())
                {
                    await message.Channel.SendMessageAsync(message.Content);
                }
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

                    if(lower.Contains("list"))
                    {
                        string[] split = lower.Split('"');
                        if(split.Length < 3)
                        {
                            await Character.SendMessage("noquotes", "No quotes entered", message.Channel);
                            return;
                        }

                        using(var data = new LiteDatabase(Program.CHR_DATABASE))
                        {
                            var results = data.GetCollection<CharacterString>(split[1]).Find(Query.All()).ToList();

                            string builder = "";
                            foreach(CharacterString chr in results)
                            {
                                builder += chr.Tag + " | " + chr.Line + "\n";
                            }

                            Console.WriteLine(builder);
                            await message.Channel.SendMessageAsync(builder);
                        }
                    }
                    return;
                }
            }

            if(lower.Contains("thanks") || lower.Contains("thank"))
            {
                await Character.SendMessage("thanks", "Got thanks", message.Channel);
            }

            if(lower.Contains("kys"))
            {
                await KeepYourselfSafe.RunCommand(message);
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

            if(lower.Contains("voice"))
            {
                await InsideVoice.RunCommand(message);
            }

            if(lower.Contains("play"))
            {
                if(lower.Contains("your song") || lower.Contains("your reality"))
                {
                    await Character.SendMessage("yourreality", "Playing my song~", message.Channel);
                    await InsideVoice.PlaySong("YourReality.mp3", message);
                    return;
                }

                string[] split = lower.Split('"');
                if(split.Length < 3)
                {
                    await Character.SendMessage("noquotes", "No quotes entered", message.Channel);
                    return;
                }

                if(File.Exists(split[1] + ".mp3"))
                {
                    await Character.SendMessage("playsong", "Playing a song", message.Channel, split[1]);
                    await InsideVoice.PlaySong(split[1] + ".mp3", message);
                }
            }
        }
    }
}
