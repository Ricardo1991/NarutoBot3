using System;
using System.IO;
using System.Net.Sockets;

namespace NarutoBot3
{
    public class IRC_Client : IDisposable
    {
        public bool isConnected = false;

        public string HOME_CHANNEL;
        public string HOST;
        public char SYMBOL = '!';
        public int PORT;
        public string NICK;
        public string REALNAME;

        public string HOST_SERVER;

        private string user_message;
        private string nick_message;
        private string join_message;

        private NetworkStream stream;
        private TcpClient irc;
        public StreamReader reader;
        public StreamWriter writer;

        public IRC_Client(string home_channel, string host, int port, string nick, string realName)
        {
            HOME_CHANNEL = home_channel;
            HOST = host;
            PORT = port;
            NICK = nick;
            REALNAME = realName;

            user_message = "USER " + NICK + " " + NICK + "_h" + " " + NICK + "_s" + " :" + REALNAME + "\n";
            nick_message = "NICK " + NICK + "\r\n";
            join_message = "JOIN " + HOME_CHANNEL + "\r\n";
        }

        public bool Connect()
        {
            if (irc != null) irc.Close();

            irc = new TcpClient(HOST, PORT);
            stream = irc.GetStream();

            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };

            try{
                sendMessage(user_message);
                sendMessage(nick_message);
                
                return true;    //Weee, we connected!
            }
            catch (SocketException se){
                Console.Out.Write(se);
                return false;   //Boo, we didnt connect
            }
        }

        public void Join()
        {
            sendMessage(join_message);
            isConnected = true;
        }

        public void Join(string channel)
        {
            sendMessage("JOIN " + channel + "\r\n");
            isConnected = true;
        }

        public bool sendMessage(string message)
        {
            string temp;
            string header;
            string footer = "\r\n";
            bool isAction = false;

            if (string.IsNullOrWhiteSpace(message)) return false;

            header = message.Split(new char[]{':'} , 2)[0]+":";

            try
            {
                if (message.Length > 450)
                {
                    message = Useful.getBetween(message, header, "");
                    message = message.Replace(footer, String.Empty);

                    while (message.Length > (450 - header.Length - footer.Length))
                    {
                        if (message.Contains("\x01")){
                            temp = header + message.Substring(0, 450 - header.Length - footer.Length) + "\x01" + footer;
                            isAction = true;
                        }

                        else{
                            temp = header + message.Substring(0, 450 - header.Length - footer.Length) + footer;
                        }

                        message = message.Substring(450 - header.Length - footer.Length);

                        writer.WriteLine(temp);
                        writer.Flush();
                    }
                    if (isAction) message = message.Replace("\x01", String.Empty);
                    writer.WriteLine(header + message + footer);

                }
                else
                    writer.WriteLine(message);

                writer.Flush();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string readMessage()
        {
            return reader.ReadLine();
        }

        public void Disconnect(string quitMessage)
        {
            try
            {
                if (writer != null) sendMessage("QUIT "+quitMessage+"\r\n");

                isConnected = false;

                if (stream != null)
                    stream.Close();
                if(writer!=null)
                    writer.Close();
                if (reader != null)
                    reader.Close();
                if (irc != null)
                    irc.Close();
            }
            catch { }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(stream!=null)
                    stream.Close();
                if (writer != null)
                    writer.Close();
                if (reader != null)
                    reader.Close();
                if (irc != null)
                    irc.Close();
            }
        }
    }
}
