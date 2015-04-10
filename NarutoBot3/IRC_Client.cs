﻿using System;
using System.IO;
using System.Net.Sockets;

namespace NarutoBot3
{
    public class IRC_Client : IDisposable
    {
        public bool isConnected = false;

        public string HOME_CHANNEL;
        public string HOST;
        public int PORT;
        public string NICK;
        public string SYMBOL = "!";
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
                messageSender(user_message);
                messageSender(nick_message);
                
                return true;    //Weee, we connected!
            }
            catch (SocketException se){
                Console.Out.Write(se);
                return false;   //Boo, we didnt connect
            }
        }

        public void Join()
        {
            messageSender(join_message);
            isConnected = true;
        }

        public void Join(string channel)
        {
            messageSender("JOIN " + channel + "\r\n");
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

        public string readLine()
        {
            return reader.ReadLine();
        }

        public void Disconnect()
        {
            try
            {
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
