using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3
{
    public class IrcClient
    {
        public bool isConnected = false;

        public string HOME_CHANNEL;
        public string HOST;
        public int PORT;
        public string NICK;
        public string SYMBOL = "!";

        public string HOST_SERVER;

        string user_message;
        string nick_message;
        string join_message;
        string quit_message;

        NetworkStream stream;
        TcpClient irc;
        public StreamReader reader;
        public StreamWriter writer;

        public List<string> userList = new List<string>();

        public IrcClient(string home_channel, string host, int port, string nick)
        {
            HOME_CHANNEL = home_channel;
            HOST = host;
            PORT = port;
            NICK = nick;

            user_message = "USER " + NICK + " " + NICK + "_h" + " " + NICK + "_s" + " :/r/naruto \n";
            nick_message = "NICK " + NICK + "\r\n";
            join_message = "JOIN " + HOME_CHANNEL + "\r\n";
            quit_message = "QUIT " + HOME_CHANNEL + "\n";
        }

        public bool Connect()
        {
            irc = new TcpClient(HOST, PORT);
            stream = irc.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };

            try
            {
                messageSender(user_message);
                messageSender(nick_message);
                
                return true;    //Weee, we connected!
            }

            catch (SocketException se)
            {
                Console.Out.Write(se);
                return false;   //Boo, we didnt connect
            }
        }

        public void Join()
        {
            messageSender(join_message);
            isConnected = true;
        
        }

        public bool messageSender(string message)
        {
            try
            {
                writer.WriteLine(message);
                writer.Flush();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string messageReader()
        {
            return reader.ReadLine();
        }

        public void Disconnect()
        {
            userList.Clear();

            try
            {
                isConnected = false;
                writer.Close();
                reader.Close();
                irc.Close();
            }
            catch { }
        }
    }
}
