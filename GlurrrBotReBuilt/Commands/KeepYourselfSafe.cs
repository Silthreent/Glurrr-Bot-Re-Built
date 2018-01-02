using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace GlurrrBotReBuilt.Commands
{
    class KeepYourselfSafe
    {
        public static async Task RunCommand(SocketMessage message)
        {
            Console.WriteLine("Keep Yourself Safe");

            string lower = message.Content.ToLower();

            if(message.MentionedUsers.Count > 0)
            {
                var msgs = await message.Channel.GetMessagesAsync().Flatten();
                foreach(RestUserMessage msg in msgs)
                {
                    if(msg.Author.Id == message.MentionedUsers.ElementAt(0).Id)
                    {
                        await msg.AddReactionAsync(new Emoji("🇰"));
                        await msg.AddReactionAsync(new Emoji("🇾"));
                        await msg.AddReactionAsync(new Emoji("🇸"));
                        break;
                    }
                }
            }

            if(lower.Contains("david"))
            {
                var msgs = await message.Channel.GetMessagesAsync().Flatten();
                foreach(RestUserMessage msg in msgs)
                {
                    if(msg.Author.Id == 135498846494130177)
                    {
                        await msg.AddReactionAsync(new Emoji("🇰"));
                        await msg.AddReactionAsync(new Emoji("🇾"));
                        await msg.AddReactionAsync(new Emoji("🇸"));
                        break;
                    }
                }
            }
            if(lower.Contains("pepper"))
            {
                var msgs = await message.Channel.GetMessagesAsync().Flatten();
                foreach(RestUserMessage msg in msgs)
                {
                    if(msg.Author.Id == 272468413970907136)
                    {
                        await msg.AddReactionAsync(new Emoji("🇰"));
                        await msg.AddReactionAsync(new Emoji("🇾"));
                        await msg.AddReactionAsync(new Emoji("🇸"));
                        break;
                    }
                }
            }
            if(lower.Contains("matt"))
            {
                var msgs = await message.Channel.GetMessagesAsync().Flatten();
                foreach(RestUserMessage msg in msgs)
                {
                    if(msg.Author.Id == 134852512611172352)
                    {
                        await msg.AddReactionAsync(new Emoji("🇰"));
                        await msg.AddReactionAsync(new Emoji("🇾"));
                        await msg.AddReactionAsync(new Emoji("🇸"));
                        break;
                    }
                }
            }
        }
    }
}
