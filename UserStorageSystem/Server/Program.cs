using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using UserStorage;
using SystemConfigurator;

namespace Server
{
    // To start separate servers for Slaves, Master and Proxy
    // uncomment respective sections in App.Config.
    class Program
    {
        static void Main(string[] args)
        {
            Configurator.StartServer();
            Console.WriteLine("[Server]:running.");
            Console.WriteLine("[Server]:press any key to shut down.");
            Console.ReadKey();
            Configurator.ShutDownServer();
        }
    }
}
