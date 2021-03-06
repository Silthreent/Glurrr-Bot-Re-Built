﻿using Discord.WebSocket;
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
        public static string currentCharacter = "monika";

        public static async Task SendMessage(string message, string consoleMessage, ISocketMessageChannel channel, string format1 = "", string format2 = "")
        {
            Console.WriteLine(string.Format(consoleMessage, format1, format2));

            using(var db = new LiteDatabase(Program.CHR_DATABASE))
            {
                var character = db.GetCollection<CharacterString>(currentCharacter);

                if(character.Exists(x => x.Tag == message))
                {
                    var options = character.Find(x => x.Tag == message);
                    var selectedOption = new Random().Next(0, options.Count());
                    await WriteChat(string.Format(options.ElementAt(selectedOption).Line, format1, format2), channel);
                }
                else
                {
                    var defaultChr = db.GetCollection<CharacterString>("default");
                    if(defaultChr.Exists(x => x.Tag == message))
                    {
                        await WriteChat(string.Format(defaultChr.FindOne(x => x.Tag == message).Line, format1, format2), channel);
                    }
                    else
                    {
                        await WriteChat("Empty response: " + message, channel);
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
                    await SendMessage("changedchr", "Changed chr from {0} to {1}", channel, currentCharacter, character);
                    currentCharacter = character;
                }
                else
                {
                    await SendMessage("changechrfail", "Character {0} not found", channel, character);
                }
            }
        }

        public static async Task AddCharacterLine(SocketMessage message, bool overwrite = true)
        {
            string[] split = message.Content.Split('"');

            if(message.Content.Contains("new"))
                overwrite = false;

            if(split.Length >= 6)
            {
                using(var data = new LiteDatabase(Program.CHR_DATABASE))
                {
                    var collection = data.GetCollection<CharacterString>(split[1]);

                    if(collection.Exists(x => x.Tag == split[3]) && overwrite == true)
                    {
                        var chr = collection.FindOne(x => x.Tag == split[3]);
                        chr.Tag = split[3];
                        chr.Line = split[5];
                        collection.Update(chr);

                        Console.WriteLine("Edited :" + split[3] + ": to be :" + split[5] + ": with the character :" + split[1] + ":");
                        await WriteChat("Edited :" + split[3] + ": to be :" + split[5] + ": with the character :" + split[1] + ":", message.Channel);
                    }
                    else
                    {
                        collection.Insert(new CharacterString() { Tag = split[3], Line = split[5] });

                        Console.WriteLine("Added :" + split[5] + ": under tag :" + split[3] + ": to the character :" + split[1] + ":");
                        await WriteChat("Added :" + split[5] + ": under tag :" + split[3] + ": to the character :" + split[1] + ":", message.Channel);
                    }
                }
            }
        }

        public static async Task WriteChat(string message, ISocketMessageChannel channel)
        {
            using(var typing = channel.EnterTypingState())
            {
                await Task.Delay(message.Length * 65);
                await channel.SendMessageAsync(message);
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
