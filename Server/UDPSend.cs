using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Server
{
    class UDPSend
    {
        private const int outPort = 22333;
        private const string IP = "127.0.0.1";

        public static void Send(byte[] args)
        {
            UdpClient udpclient = new UdpClient();

            IPAddress multicastaddress = IPAddress.Parse(IP);

            IPEndPoint endPoint = new IPEndPoint(multicastaddress, outPort);

            udpclient.Connect(endPoint);

            udpclient.Send(args, args.Length);

            Console.WriteLine("Message sent to the broadcast address");
        }
    }
}