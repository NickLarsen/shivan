using System;
using System.Net;
using System.Net.Sockets;

namespace Shivan.Web
{
    public class HttpServer
    {
        public void Start()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5431);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try{
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while(true) {
                    //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
                }
            }
        }
    }
}
