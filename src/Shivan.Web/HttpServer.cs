using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Shivan.Web
{
    //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
    public class HttpServer
    {
        private static ManualResetEvent allDone = new ManualResetEvent(false);

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
                    allDone.Reset();
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    allDone.WaitOne();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();
            
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0) {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    Console.WriteLine("Read {0} bytes from socket. \nData: {1}", content.Length, content);
                    Send(handler, content);
                }
                else
                {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket) ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    public class StateObject {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
}
