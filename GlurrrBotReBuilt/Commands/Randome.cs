using Discord.WebSocket;
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
        public static async Task RunCommand(SocketMessage message)
        {
            Console.WriteLine("Running Randome");
            string lower = message.Content.ToLower();
            string[] split = lower.Split('"');

            if(lower.Contains("add"))
            {
                using(var data = new LiteDatabase(Program.DATABASE))
                {
                    var collection = data.GetCollection<RandomeObject>("randome");
                    collection.Insert(new RandomeObject() { Owner = message.Author.Id, Thing = split[1] });
                    Console.WriteLine("Created object: " + split[1] + " under ID:" + message.Author.Id);
                    await message.Channel.SendMessageAsync("Added " + split[1] + " to your Randome");
                }

                return;
            }

            if(lower.Contains("roll"))
            {
                using(var data = new LiteDatabase(Program.DATABASE))
                {
                    var list = data.GetCollection<RandomeObject>("randome").Find(Query.All()).ToList();
                    if(list.Count <= 0)
                    {
                        Console.WriteLine("List empty");
                        return;
                    }

                    RandomeObject winner = list[new Random().Next(0, list.Count)];
                    await message.Channel.SendMessageAsync(message.Channel.GetUserAsync(winner.Owner).Result.Username + "'s choice of " + winner.Thing + " wins!");
                }

                return;
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
                    data.DropCollection("randome");
                    Console.WriteLine("Cleared the Randome");
                    await message.Channel.SendMessageAsync("Cleared the Randome");
                }
            }
        }

        async static Task DisplayRandome(ISocketMessageChannel channel)
        {
            using(var data = new LiteDatabase(Program.DATABASE))
            {
                var results = data.GetCollection<RandomeObject>("randome").Find(Query.All()).ToList();
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
