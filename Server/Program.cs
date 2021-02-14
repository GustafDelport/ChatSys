using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        public static void ProcessClientRequests(TcpClient client)
        {
            try
            {
                StreamReader reader = new StreamReader(client.GetStream());
                StreamWriter writer = new StreamWriter(client.GetStream());

                string message = string.Empty;

                while (!(message = reader.ReadLine()).Equals("Exit") || (message == null))
                {
                    Console.WriteLine(message);
                    writer.WriteLine("Message Recieved");
                    writer.Flush();
                }
                Console.WriteLine("Client Exited");
            }
            catch (IOException)
            {
                Console.WriteLine("Corupted Connection");
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        static void Main(string[] args)
        {
            TcpListener Server = new TcpListener(IPAddress.Parse(GetLocalIPv4(NetworkInterfaceType.Ethernet)), 2000);

            try
            {
                Server.Start();
                while (true)
                {
                    Console.WriteLine("Staring");
                    TcpClient client = Server.AcceptTcpClient();
                    Console.WriteLine("Client Atempting to Connect");
                    new Thread(() => ProcessClientRequests(client)).Start();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (Server != null)
                {
                    Server.Stop();
                }
            }
        }

        internal static string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties adapterProperties = item.GetIPProperties();
                    if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                    {
                        foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                output = ip.Address.ToString();
                                break;
                            }
                        }
                    }
                }
                if (output != "") { break; }
            }
            return output;
        }
    }
}
