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
using System.Text.RegularExpressions;

namespace GlurrrBotReBuilt
{
    class CommandHandler
    {
        public static async Task MessageRecieved(SocketMessage message)
        {
            string lastMessageHolder = message.Channel.GetMessagesAsync(2).Flatten().Result.ElementAt(1).Content.ToLower();

            if(message.Content.ToLower().Contains("monika"))
            {
                await ParseMessage(message);
            }
            else if(FixedCommands(message).Result)
            {
                return;
            }
            else
            {
                var lastMessage = message.Channel.GetMessagesAsync(2).Flatten().Result.ElementAt(1);
                if(message.Content.ToLower() == lastMessage.Content.ToLower())
                {
                    if(message.Id != lastMessage.Id)
                    {
                        await Character.WriteChat(message.Content, message.Channel);
                    }
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
                            if(!data.CollectionExists(split[1]))
                            {
                                await Character.SendMessage("cantfind", "Can't find that character", message.Channel, split[1]);
                                return;
                            }

                            var results = data.GetCollection<CharacterString>(split[1]).Find(Query.All()).ToList();

                            string builder = "";
                            foreach(CharacterString chr in results)
                            {
                                builder += chr.Tag + " | " + chr.Line + "\n";
                            }

                            Console.WriteLine(builder);
                            await Character.WriteChat(builder, message.Channel);
                        }
                    }
                    return;
                }
            }

            if(lower.Contains("roll"))
            {
                Console.WriteLine("Command: Dice Roll");
                if(Regex.Match(lower, @"d\d+").Success)
                {
                    string number = Regex.Match(lower, @"\d+").Value;
                    await Character.WriteChat("I rolled a " + new Random().Next(1, int.Parse(number) + 1) + "!", message.Channel);
                }
            }

            if(lower.Contains("thanks") || lower.Contains("thank"))
            {
                Console.WriteLine("Command: Thanks");
                await Character.SendMessage("thanks", "Got thanks", message.Channel);
            }

            if(lower.Contains("kys"))
            {
                Console.WriteLine("Command: KYS");
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
                Console.WriteLine("Command: Randome");
                await Randome.RunCommand(message);
            }

            if(lower.Contains("anime"))
            {
                Console.WriteLine("Command: Anime");
                await Character.SendMessage("anime", "", message.Channel);
            }

            if(lower.Contains("voice"))
            {
                Console.WriteLine("Command: Voice");
                await InsideVoice.RunCommand(message);
            }

            if(lower.Contains("play"))
            {
                Console.WriteLine("Command: Play");
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

            if(lower.Contains("poem"))
            {
                Console.WriteLine("Command: Poem");
                if(lower.Contains("read"))
                {
                    await PoemTime.ReadPoem(message);
                }
            }

            if(lower.Contains("user info"))
            {
                Console.WriteLine("Command: User Info");
                await message.Channel.SendMessageAsync("```Joined at: " + (message.Author as SocketGuildUser).JoinedAt + "\nAccount created at: " + message.Author.CreatedAt + "```");
            }

            if(lower.Contains("change game"))
            {
                Console.WriteLine("Command: Change Game");
                string[] split = message.Content.Split('"');
                if(split.Length < 3)
                {
                    await Character.SendMessage("noquotes", "No quotes entered", message.Channel);
                    return;
                }

                await Program.client.SetGameAsync(split[1]);
            }

            if(lower.Contains("run script"))
            {
                Console.WriteLine("Command: Run Script");

                string[] split = lower.Split('"');
                if(split.Length < 3)
                {
                    await Character.SendMessage("noquotes", "No quotes entered", message.Channel);
                    return;
                }

                Program.python.ExecutePython(@"Scripts/" + split[1], message);
            }
        }

        static async Task<bool> FixedCommands(SocketMessage message)
        {
            if(message.Content.ToLower() == "/leave")
            {
                var embed = new EmbedBuilder()
                {
                    Title = (message.Author as SocketGuildUser).Nickname + " has left",
                    Description = (message.Author as SocketGuildUser).Nickname + " has left the Discord and would like everyone to know they did. They are very triggered.",
                    Color = Color.DarkRed,
                    ImageUrl = "https://cdn.discordapp.com/attachments/359555790597783553/409532133346902016/sagiribakasmall.png",
                };

                await message.Channel.SendMessageAsync("", embed: embed);
                return true;
            }

            if(message.Content.ToLower() == "memejacker")
            {
                await message.Channel.SendFileAsync("memejacker.jpg");
            }

            return false;
        }
    }

    class CommandObject
    {
        public int Id{ get; set; }
        public string Command { get; set; }
        public string File { get; set; }
    }
}
