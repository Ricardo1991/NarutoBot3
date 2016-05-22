using NarutoBot3.Messages;
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

        private Message user_message;
        private Message nick_message;
        private Message join_message;

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

            user_message = new Messages.User(null, NICK + " " + NICK + "_h" + " " + NICK + "_s" + " :" + REALNAME);
            nick_message = new Nick(null, NICK);
            join_message = new Join(null, HOME_CHANNEL);
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
            sendMessage(new Join(null, channel));
            isConnected = true;
        }

        public bool sendMessage(Message message)
        {
            if (!message.isValid() || writer == null) return false;

            try
            {

                while (message.toString().Length > 420)
                {
                    var nextMessage = (Message) message.Clone();
                    message.body = message.body.Substring(0, 390);
                    nextMessage.body = nextMessage.body.Substring(390);

                    writer.WriteLine(message.toString());

                    message = nextMessage;
                }

                
               
                writer.WriteLine(message.toString());

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
            if (reader != null)
                return reader.ReadLine();
            else return string.Empty;
        }

        public void Disconnect(string quitMessage)
        {
            try
            {
                if (writer != null) sendMessage(new Quit(null, quitMessage));

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
