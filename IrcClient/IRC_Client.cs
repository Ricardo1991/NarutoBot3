using IrcClient.Messages;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace IrcClient
{
    public class IRC_Client
    {
        public bool isConnected = false;

        public string HOME_CHANNEL;
        public string HOST;
        public char SYMBOL = '!';
        public int PORT;
        public string NICK;
        public string REALNAME;

        public string HOST_SERVER;

        private IrcMessage user_message;
        private IrcMessage nick_message;
        private IrcMessage join_message;

        private NetworkStream stream;
        private TcpClient irc;
        public StreamReader reader;
        public StreamWriter writer;

        public delegate void MessageReceived(string message);
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        public IRC_Client(string home_channel, string host, int port, string nick, string realName)
        {
            HOME_CHANNEL = home_channel;
            HOST = host;
            PORT = port;
            NICK = nick;
            REALNAME = realName;

            user_message = new Messages.User(NICK + " " + NICK + "_h" + " " + NICK + "_s" + " :" + REALNAME);
            nick_message = new Nick(NICK);
            join_message = new Join(HOME_CHANNEL);

            //Events for BGWorker
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_MainBotCycle);
            backgroundWorker.WorkerSupportsCancellation = true;

        }

        public bool Connect(MessageReceived messageDelegate)
        {
            if (irc != null) irc.Close();

            irc = new TcpClient(HOST, PORT);
            irc.ReceiveTimeout = 5000;
            stream = irc.GetStream();

            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };

            try
            {
                sendMessage(user_message);
                sendMessage(nick_message);
                backgroundWorker.RunWorkerAsync(messageDelegate);

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
            sendMessage(join_message);
            isConnected = true;
        }

        public void Join(string channel)
        {
            sendMessage(new Join(channel));
            isConnected = true;
        }

        public bool sendMessage(IrcMessage message)
        {
            if (!message.isValid() || writer == null) return false;

            try
            {
                while (message.toString().Length > 420)
                {
                    var nextMessage = (IrcMessage)message.Clone();
                    int cut = findCutSpace(message.body, 390);
                    message.body = message.body.Substring(0, cut);
                    nextMessage.body = nextMessage.body.Substring(cut);

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

        private int findCutSpace(string body, int startSearch)
        {
            for(int i = startSearch; i > startSearch-30 && i > 0; i--)
            {
                if(body[i] == ' ')
                    return i;
            }
            return startSearch;
        }

        public void backgroundWorker_MainBotCycle(object sender, DoWorkEventArgs e) //Main Loop
        {
            MessageReceived messageDelegate = (MessageReceived)e.Argument;

            while (!backgroundWorker.CancellationPending)
            {
                string buffer = "";
                string line;

                try
                {
                    if (reader != null)
                        buffer = reader.ReadLine();
                    else messageDelegate(string.Empty);

                    byte[] bytes = Encoding.UTF8.GetBytes(buffer);
                    line = Encoding.UTF8.GetString(bytes);

                    if (line.Length > 0) messageDelegate(line);
                }
                catch
                { }
            }
        }

        public void Disconnect(string quitMessage)
        {
            try
            {
                if (writer != null) sendMessage(new Quit(quitMessage));

                isConnected = false;
                backgroundWorker.CancelAsync();

                if (stream != null)
                    stream.Close();
                if (writer != null)
                    writer.Close();
                if (reader != null)
                    reader.Close();
                if (irc != null)
                    irc.Close();
            }
            catch { }
        }
    }
}