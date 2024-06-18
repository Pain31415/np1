using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter 'time' to get current time or 'date' to get current date:");
        string userInput = Console.ReadLine();

        try
        {
            IPAddress ipAddress = IPAddress.Parse("10.0.0.139");
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await sender.ConnectAsync(remoteEP);
                Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

                byte[] msg = Encoding.ASCII.GetBytes(userInput);

                await sender.SendAsync(new ArraySegment<byte>(msg), SocketFlags.None);

                byte[] bytes = new byte[1024];
                int bytesRec = await sender.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

                Console.WriteLine($"At {DateTime.Now.ToShortTimeString()} from {sender.RemoteEndPoint} received: {Encoding.ASCII.GetString(bytes, 0, bytesRec)}");

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}