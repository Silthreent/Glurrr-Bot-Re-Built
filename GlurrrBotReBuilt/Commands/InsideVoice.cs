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

            if(lower.Contains("join"))
            {
                audioClient = await (message.Author as IGuildUser).VoiceChannel.ConnectAsync();
            }

            if(lower.Contains("leave"))
            {
                if(audioClient != null)
                    audioClient.Dispose();
            }

            if(lower.Contains("play"))
            {
                if(audioClient == null)
                    return;

                Console.WriteLine("Playing my song~");
                try
                {
                    var process = Process.Start(new ProcessStartInfo { FileName = "ffmpeg", Arguments = $"-i YourReality.mp3 -f s16le -ar 48000 -ac 2 pipe:1", UseShellExecute = false, RedirectStandardOutput = true });
                    var output = process.StandardOutput.BaseStream;
                    var discord = audioClient.CreatePCMStream(AudioApplication.Mixed);
                    await output.CopyToAsync(discord);
                    await discord.FlushAsync();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
