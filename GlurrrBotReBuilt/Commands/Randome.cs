﻿using Discord.WebSocket;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace GlurrrBotReBuilt.Commands
{
    class Randome
    {
        static string loadedList = "randome_default";

        public static async Task RunCommand(SocketMessage message)
        {
            Console.WriteLine("Running Randome");
            string lower = message.Content.ToLower();

            if(lower.Contains("add"))
            {
                string[] split = lower.Split('"');
                if(split.Length < 3)
                {
                    await Character.SendMessage("noquotes", "No quotes entered", message.Channel);
                    return;
                }

                using(var data = new LiteDatabase(Program.DATABASE))
                {
                    var collection = data.GetCollection<RandomeObject>(loadedList);
                    collection.Insert(new RandomeObject() { Owner = message.Author.Id, Thing = split[1] });
                    Console.WriteLine("Created object: " + split[1] + " under ID:" + message.Author.Id + " on list: " + loadedList);
                    await Character.WriteChat("Added " + split[1] + " to your Randome " + loadedList.Substring(8), message.Channel);
                }

                return;
            }

            if(lower.Contains("delete"))
            {
                string[] split = lower.Split('"');
                if(split.Length < 3)
                {
                    await Character.SendMessage("noquotes", "No quotes entered", message.Channel);
                    return;
                }

                using(var data = new LiteDatabase(Program.DATABASE))
                {
                    if(lower.Contains("from my"))
                    {
                        if(data.GetCollection<RandomeObject>(loadedList).Delete(x => x.Owner == message.Author.Id && x.Thing == split[1]) > 0)
                        {
                            await Character.SendMessage("deletefromown", "Deleted {0} from own Randome", message.Channel, split[1]);
                        }
                        else
                        {
                            await Character.SendMessage("nodelete", "No instances of {0} found in own Randome", message.Channel, split[1]);
                        }
                    }
                    else
                    {
                        if(data.GetCollection<RandomeObject>(loadedList).Delete(x => x.Thing == split[1]) > 0)
                        {
                            await Character.SendMessage("deletefromrandomes", "Deleted {0} from Randomes", message.Channel, split[1]);
                        }
                        else
                        {
                            await Character.SendMessage("nodelete", "No instances of {0} found in Randomes", message.Channel, split[1]);
                        }
                    }
                }
            }

            if(lower.Contains("roll"))
            {
                using(var data = new LiteDatabase(Program.DATABASE))
                {
                    var list = data.GetCollection<RandomeObject>(loadedList).Find(Query.All()).ToList();
                    if(list.Count <= 0)
                    {
                        Console.WriteLine("List empty");
                        return;
                    }

                    RandomeObject winner = list[new Random().Next(0, list.Count)];
                    await Character.WriteChat(message.Channel.GetUserAsync(winner.Owner).Result.Username + "'s choice of " + winner.Thing + " wins!", message.Channel);
                }

                return;
            }

            if(lower.Contains("load"))
            {
                string[] split = lower.Split('"');
                if(split.Length < 3)
                {
                    await Character.SendMessage("noquotes", "No quotes entered", message.Channel);
                    return;
                }

                loadedList = "randome_" + split[1];
                await Character.SendMessage("loadedrandome", "Loaded Randome {0}", message.Channel, split[1]);
            }
            
            if(lower.Contains("list"))
            {
                await DisplayRandome(message.Channel);
                return;
            }

            if(lower.Contains("clear"))
            {
                using(var data = new LiteDatabase(Program.DATABASE))
                {
                    data.DropCollection(loadedList);
                    await Character.SendMessage("clearedrandome", "Cleared the Randome", message.Channel);
                }
            }
        }

        async static Task DisplayRandome(ISocketMessageChannel channel)
        {
            using(var data = new LiteDatabase(Program.DATABASE))
            {
                var results = data.GetCollection<RandomeObject>(loadedList).Find(Query.All()).ToList();
                if(results.Count() <= 0)
                {
                    Console.WriteLine("Empty list");
                    return;
                }

                Dictionary<ulong, List<string>> things = new Dictionary<ulong, List<string>>();

                foreach(RandomeObject o in results)
                {
                    if(!things.ContainsKey(o.Owner))
                    {
                        things.Add(o.Owner, new List<string>());
                    }

                    things[o.Owner].Add(o.Thing);
                }

                string builder = "";

                var embed = new EmbedBuilder()
                {
                    Title = "Randome List",
                    Color = Color.DarkGreen,
                };

                foreach(ulong i in things.Keys)
                {
                    foreach(string s in things[i])
                    {
                        builder += s + ", ";
                    }
                    builder = builder.Remove(builder.Length - 2);
                    embed.AddField(channel.GetUserAsync(i).Result.Username, builder, true);
                    builder = "";
                }

                Console.WriteLine(builder);
                await channel.SendMessageAsync("", false, embed);
            }
        }
    }

    class RandomeObject
    {
        public int Id { get; set; }
        public ulong Owner { get; set; }
        public string Thing { get; set; }
    }
}
