using Discord.WebSocket;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace GlurrrBotReBuilt
{
    class PythonEngine
    {
        private readonly ScriptEngine engine;
        private readonly ScriptScope scope;

        public PythonEngine()
        {
            engine = Python.CreateEngine();

            dynamic innerScope = scope = engine.CreateScope();
            innerScope.form = this;
            innerScope.proxy = CreateProxy();
        }

        public void ExecutePython(string fileName, SocketMessage msg)
        {
            scope.SetVariable("message", msg);
            engine.ExecuteFile(fileName + ".py", scope);
            scope.RemoveVariable("message");
        }

        private object CreateProxy()
        {
            dynamic proxy = new ExpandoObject();
            proxy.WriteChat = new Action<string, ISocketMessageChannel>(PythonMethods.WriteChat);
            return proxy;
        }
    }
}
