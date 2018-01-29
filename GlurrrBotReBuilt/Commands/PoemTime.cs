using Discord.WebSocket;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace GlurrrBotReBuilt.Commands
{
    class PoemTime
    {
        public static async Task ReadPoem(SocketMessage message)
        {
            using(var db = new LiteDatabase(Program.DATABASE))
            {
                var collection = db.GetCollection<Poem>("Poems");
                var poem = collection.FindAll().ElementAt(new Random().Next(0, collection.Count()));
                await Character.WriteChat("```" + poem.Name + "\n\n" + poem.Text + "```", message.Channel);
            }
        }

        public static void NewPoem()
        {
            string poem = "";
            string line;
            string name = "";

            while(true)
            {
                line = Console.ReadLine();
                if(name == "")
                {
                    name = line;
                    continue;
                }

                if(line == "end")
                    break;

                poem += line + "\n";
            }

            using(var db = new LiteDatabase(Program.DATABASE))
            {
                var collection = db.GetCollection<Poem>("Poems");
                collection.Insert(new Poem() { Name = name, Text = poem });
            }

            Console.WriteLine("Added Poem " + name);
        }
    }

    public class Poem
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public string Text{ get; set; }
    }
}
