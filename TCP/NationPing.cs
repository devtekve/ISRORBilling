using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ISRORBilling.Tcp
{
    public class NationPing
    {
        private const int PingBackTcpPort = 12989;      //SANS Internet Storm Center: port 12989   //SG security scan: port 12989
        public Thread PingBackThread;
        public NationPing()
        {
            try
            {
                //Starting the TCP Listener thread.
                PingBackThread = new Thread(new ThreadStart(StartPingBackListen));
                PingBackThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("An PingBackThread Exception has occurred!" + e.ToString());
                PingBackThread.Abort();
            }
        }

        public void StartPingBackListen()
        {
            TcpListener tcpListener = new TcpListener(PingBackTcpPort);
            try
            {
                while (true)
                {
                    tcpListener.Start();
                    Socket soTcp = tcpListener.AcceptSocket();
                    Byte[] received = new Byte[14];
                    int bytesReceived = soTcp.Receive(received, received.Length, 0);
                    received[2] = 65;
                    received[3] = 67;
                    received[4] = 75;
                    soTcp.Send(received, received.Length, 0);
                }
            }
            catch (SocketException)
            {
            }
        }



    }
}
