using Discord.WebSocket;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace GlurrrBotReBuilt
{
    class Character
    {
        static string currentCharacter = "monika";

        public static async Task SendMessage(string message, ISocketMessageChannel channel, string format1 = "", string format2 = "")
        {
            using(var db = new LiteDatabase(Program.CHR_DATABASE))
            {
                var character = db.GetCollection<CharacterString>(currentCharacter);

                if(character.Exists(x => x.Tag == message))
                {
                    await channel.SendMessageAsync(character.FindOne(x => x.Tag == message).Line);
                }
                else
                {
                    var defaultChr = db.GetCollection<CharacterString>("default");
                    if(defaultChr.Exists(x => x.Tag == message))
                    {
                        await channel.SendMessageAsync(defaultChr.FindOne(x => x.Tag == message).Line);
                    }
                    else
                    {
                        await channel.SendMessageAsync("Empty response: " + message);
                    }
                }
            }
        }

        public static async Task SetCharacter(string character, ISocketMessageChannel channel)
        {
            using(var data = new LiteDatabase(Program.CHR_DATABASE))
            {
                if(data.CollectionExists(character))
                {
                    await SendMessage("changedchr", channel, currentCharacter);
                    currentCharacter = character;
                    Console.WriteLine("Character changed to " + character);
                }
                else
                {
                    await SendMessage("changechrfail", channel);
                    Console.WriteLine("Couldn't find specified .chr");
                }
            }
        }

        public static async Task AddCharacterLine(SocketMessage message)
        {
            string[] split = message.Content.Split('"');

            if(split.Length >= 6)
            {
                Console.WriteLine("Quotes found");
                Console.WriteLine(split[1] + " | " + split[3] + " | " + split[5]);

                using(var data = new LiteDatabase(Program.CHR_DATABASE))
                {
                    var collection = data.GetCollection<CharacterString>(split[1]);

                    if(collection.Exists(x => x.Tag == split[3]))
                    {
                        var chr = collection.FindOne(x => x.Tag == split[3]);
                        chr.Tag = split[3];
                        chr.Line = split[5];

                        Console.WriteLine("Edited :" + split[3] + ": to be :" + split[5] + ": with the character :" + split[1] + ":");
                        await message.Channel.SendMessageAsync("Edited :" + split[3] + ": to be :" + split[5] + ": with the character :" + split[1] + ":");
                    }
                    else
                    {
                        collection.Insert(new CharacterString() { Tag = split[3], Line = split[5] });

                        Console.WriteLine("Added :" + split[5] + ": under tag :" + split[3] + ": to the character :" + split[1] + ":");
                        await message.Channel.SendMessageAsync("Added :" + split[5] + ": under tag :" + split[3] + ": to the character :" + split[1] + ":");
                    }
                }
            }
        }
    }

    class CharacterString
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Line { get; set; }
    }
}
