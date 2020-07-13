using System;

namespace MainServer
{
    class mainProg
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
        }
    }
}
