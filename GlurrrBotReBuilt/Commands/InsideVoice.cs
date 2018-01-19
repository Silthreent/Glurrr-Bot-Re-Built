using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace GlurrrBotReBuilt.Commands
{
    class InsideVoice
    {
        static IAudioClient audioClient;

        public static async Task RunCommand(SocketMessage message)
        {
            string lower = message.Content.ToLower();

            if(lower.Contains("leave"))
            {
                if(audioClient != null)
                    audioClient.Dispose();
            }
        }

        public static async Task PlaySong(string song, SocketMessage message)
        {
            if(audioClient == null)
            {
                if((message.Author as IGuildUser).VoiceChannel != null)
                {
                    audioClient = await (message.Author as IGuildUser).VoiceChannel.ConnectAsync();
                }
                else
                {
                    await Character.SendMessage("novoice", "Not connected to voice channel and no voice to connect to", message.Channel);
                    return;
                }
            }

            await Character.SendMessage("playvoice", "Broadcasting on voice", message.Channel, song);

            var process = Process.Start(new ProcessStartInfo { FileName = "ffmpeg", Arguments = $"-i " + song + " -f s16le -ar 48000 -ac 2 pipe:1", UseShellExecute = false, RedirectStandardOutput = true });
            var output = process.StandardOutput.BaseStream;
            var discord = audioClient.CreatePCMStream(AudioApplication.Mixed, 128 * 1024);
            await output.CopyToAsync(discord);
            await discord.FlushAsync();
        }
    }
}
