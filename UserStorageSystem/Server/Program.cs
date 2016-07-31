namespace Server
{
    using System;
    using SystemConfigurator;

    // To start separate servers for Slaves, Master and Proxy
    // uncomment respective sections in App.Config.
    internal class Program
    {
        private static void Main(string[] args)
        {
            Configurator.StartServer();
            Console.WriteLine("[Server]:running.");
            Console.WriteLine("[Server]:press any key to shut down.");
            Console.ReadKey();
            Configurator.ShutDownServer();
        }
    }
}
