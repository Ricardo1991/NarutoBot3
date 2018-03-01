using Cleverbot.Net;
using IrcClient;
using IrcClient.Messages;
using NarutoBot3.Events;
using NarutoBot3.Properties;
using Newtonsoft.Json;
using RedditSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Web;
using System.Windows.Forms;
using System.Xml.Serialization;
using TweetSharp;

namespace NarutoBot3
{
    public class Bot
    {
        public UserList userlist = new UserList();
        private CleverbotSession cleverbotSession = null;
        private IRC_Client client;
        private ColorScheme currentColorScheme = new ColorScheme();
        private string lastImgurID;
        private RichTextBox OutputBox = null;
        private System.Timers.Timer pingServerTimer;
        private Questions qq;
        private Reddit reddit;
        private TwitterService service;
        private StatsManager stats = new StatsManager();
        private StringLibrary stringLib = new StringLibrary();
        private System.Timers.Timer timeoutTimer;

        private bool waitingForPong = false;

        public Bot(ref RichTextBox output)
        {
            OutputBox = output;
            Client = new IRC_Client(Settings.Default.Channel, Settings.Default.Server, Convert.ToInt32(Settings.Default.Port),
                Settings.Default.Nick, Settings.Default.RealName);

            qq = new Questions();

            userlist.LoadData();

            pingServerTimer = new System.Timers.Timer(Settings.Default.timeOutTimeInterval * 1000)
            {
                Enabled = true
            };
            pingServerTimer.Elapsed += new ElapsedEventHandler(PingSever);

            if (string.IsNullOrWhiteSpace(Settings.Default.cleverbotAPI))
            {
                Settings.Default.botThinkEnabled = false;
                Settings.Default.Save();
            }

            reddit = new Reddit(false);

            if (Settings.Default.redditUserEnabled)
            {
                try
                {
                    reddit.User = reddit.LogIn(Settings.Default.redditUser, Settings.Default.redditPass, true);
                }
                catch { }
            }

            if (string.IsNullOrWhiteSpace(Settings.Default.cleverbotAPI))
            {
                Settings.Default.botThinkEnabled = false;
                Settings.Default.Save();
            }

            if (Settings.Default.twitterEnabled)
                TwitterLogOn();
        }

        public event EventHandler<EventArgs> BotNickChanged;

        public event EventHandler<EventArgs> BotSilenced;

        public event EventHandler<EventArgs> BotUnsilenced;

        public event EventHandler<EventArgs> Connected;

        public event EventHandler<EventArgs> ConnectedWithServer;

        public event EventHandler<EventArgs> Created;

        public event EventHandler<EventArgs> DuplicatedNick;
        public event EventHandler<EventArgs> EnforceMirrorChanged;

        public event EventHandler<ModeChangedEventArgs> ModeChanged;

        public event EventHandler<PongEventArgs> PongReceived;

        public event EventHandler<EventArgs> Quit;

        public event EventHandler<EventArgs> Timeout;
        public event EventHandler<TopicChangedEventArgs> TopicChange;
        public event EventHandler<EventArgs> UpdateUserListSource;
                //To check for connection lost

        public IRC_Client Client
        {
            get
            {
                return client;
            }

            set
            {
                client = value;
            }
        }

        internal StringLibrary StringLib { get => stringLib; set => stringLib = value; }

        internal static UserList GetSavedUsers()
        {
            UserList ul = new UserList();
            ul.LoadData();

            return ul;
        }

        internal bool ChangeNick(string nick)
        {
            string oldnick = Client.NICK;
            Client.NICK = Settings.Default.Nick = nick;
            Settings.Default.Save();
            OnBotNickChanged(EventArgs.Empty);

            //do Nick change to server
            if (Client.isConnected)
            {
                IrcMessage message = new Nick(Client.NICK);
                SendMessage(message);

                userlist.SetUserOnlineStatus(nick, true);
                userlist.SetUserMode(nick, userlist.GetUserMode(oldnick));
                userlist.SetUserChannelVoice(nick, userlist.UserHasChannelVoice(oldnick));
                userlist.SetUserChannelOP(nick, userlist.UserHasChannelOP(oldnick));

                userlist.SetUserOnlineStatus(oldnick, false);

                return true;
            }
            else return false;
        }

        internal bool Connect()
        {
            if (Client.Connect(ProcessMessage))
            {
                pingServerTimer.Enabled = true;
                return true;
            }
            else
            {
                pingServerTimer.Enabled = false;
                return false;
            }
        }

        internal bool Disconnect(string quitMessage)
        {
            userlist.SaveData();

            userlist = new UserList();
            userlist.LoadData();

            pingServerTimer.Enabled = false;
            return client.Disconnect(quitMessage);
        }

        internal void GiveOps(string nick)
        {
            userlist.SetUserOperatorStatus(nick, true);
            userlist.SaveData();
        }

        internal void KickUser(string userToBeKicked)
        {
            IrcMessage message = new Kick(Client.HOME_CHANNEL, userToBeKicked);
            SendMessage(message);
        }

        internal void MuteUser(string nick)
        {
            userlist.SetUserMuteStatus(nick, true);
            userlist.SaveData();
        }

        internal void PingSever(object sender, EventArgs e)
        {
            if (!waitingForPong)
            {
                IrcMessage message = new Ping(Useful.GetTimestamp(DateTime.Now));

                SendMessage(message);

                waitingForPong = true;

                timeoutTimer = new System.Timers.Timer(Settings.Default.timeOutTimeInterval * 1000)
                {
                    Enabled = true
                };
                timeoutTimer.Elapsed += new ElapsedEventHandler(CheckIfTimeout);
            }
        }

        internal void PokeUser(string nick)
        {
            IrcMessage message = new ActionMessage(Client.HOME_CHANNEL, "stabs " + nick + " with a sharp knife");
            SendMessage(message);
        }

        internal void RedditLogin(string userName, string password)
        {
            reddit.User = reddit.LogIn(userName, password, true);
        }

        internal void SendMessage(IrcMessage message)
        {
            Client.SendMessage(message);

            if (message is Notice)
            {
                if (Client.NICK.Length > 15)
                {
                    WriteMessage(Client.NICK.Truncate(16) + ":" + message.body);
                }
                else if (Client.NICK.Length >= 8)                       //Write the Message on the bot console
                {
                    WriteMessage(Client.NICK + "\t: " + message.body);
                }
                else
                {
                    WriteMessage(Client.NICK + "\t\t: " + message.body);
                }
            }
            else if (message is ActionMessage)
            {
                WriteMessage("             * : " + message.body, currentColorScheme.OwnMessage);
            }
            else if (message is Privmsg)
            {
                string alignedNick = Client.NICK.Truncate(13);
                int tab = 15 - alignedNick.Length;

                for (int i = 0; i < tab; i++)
                    alignedNick = alignedNick + " ";

                WriteMessage(alignedNick + ": " + message.body, currentColorScheme.OwnMessage);
            }
        }

        internal void TakeOps(string nick)
        {
            userlist.SetUserOperatorStatus(nick, false);
            userlist.SaveData();
        }

        internal void TwitterLogOn()
        {
            if (Settings.Default.twitterEnabled)
            {
                try
                {
                    service = new TwitterService(Settings.Default.twitterConsumerKey, Settings.Default.twitterConsumerKeySecret);
                    service.AuthenticateWith(Settings.Default.twitterAccessToken, Settings.Default.twitterAccessTokenSecret);
                }
                catch
                {
                    Settings.Default.twitterEnabled = false;
                    Settings.Default.Save();
                }
            }
        }

        ////
        internal void UnmuteUser(string nick)
        {
            userlist.SetUserMuteStatus(nick, false);
            userlist.SaveData();
        }

        internal void UpdateTheme(ColorScheme newColorScheme)
        {
            if (newColorScheme != null)
                currentColorScheme = newColorScheme;
        }

        internal void WhoisUser(string nick)
        {
            IrcMessage message = new Whois(nick);
            SendMessage(message);
        }

        protected virtual void OnBotNickChanged(EventArgs e)
        {
            BotNickChanged?.Invoke(this, e);
        }

        protected virtual void OnConnect(EventArgs e)
        {
            Connected?.Invoke(this, e);
        }

        protected virtual void OnConnectedWithServer(EventArgs e)
        {
            ConnectedWithServer?.Invoke(this, e);
        }

        protected virtual void OnCreate(EventArgs e)
        {
            Created?.Invoke(this, e);
        }

        protected virtual void OnDuplicatedNick(EventArgs e)
        {
            DuplicatedNick?.Invoke(this, e);
        }

        protected virtual void OnEnforceMirrorChanged(EventArgs e)
        {
            EnforceMirrorChanged?.Invoke(this, e);
        }

        protected virtual void OnModeChange(ModeChangedEventArgs e)
        {
            ModeChanged?.Invoke(this, e);
        }

        protected virtual void OnPongReceived(PongEventArgs e)
        {
            PongReceived?.Invoke(this, e);
        }

        protected virtual void OnQuit(EventArgs e)
        {
            Quit?.Invoke(this, e);
        }

        protected virtual void OnSilence(EventArgs e)
        {
            BotSilenced?.Invoke(this, e);
        }

        protected virtual void OnTimeout(EventArgs e)
        {
            Timeout?.Invoke(this, e);
        }

        protected virtual void OnTopicChange(TopicChangedEventArgs e)
        {
            TopicChange?.Invoke(this, e);
        }
        protected virtual void OnUnsilence(EventArgs e)
        {
            BotUnsilenced?.Invoke(this, e);
        }
        protected virtual void OnUpdateUserListSource(UserJoinLeftMessageEventArgs e)
        {
            UpdateUserListSource?.Invoke(this, e);
        }
        private static string QuestionsRegex(string rest)
        {
            var someVariable1 = "you";
            var someVariable2 = "me";
            var someVariable3 = "you are";
            var someVariable4 = "my";
            var someVariable5 = "your";
            var someVariable6 = "myself";
            var someVariable7 = "yourself";

            var replacements = new Dictionary<string, string>()
            {
                    {"me",someVariable1},
                    {"you",someVariable2},
                    {"i am", someVariable3},
                    {"i'm", someVariable3},
                    {"your", someVariable4},
                    {"my", someVariable5},
                    {"yourself", someVariable6},
                    {"myself", someVariable7},
                    {"i", someVariable1}
            };

            try
            {
                var regex = new Regex("(?i)(\\b" + string.Join("\\b|\\b", replacements.Keys) + "\\b)");
                var replaced = regex.Replace(rest, m => replacements[m.Value]);
                return replaced;
            }
            catch
            {
                return rest;
            }
        }

        private bool AddBotOP(string nick, string targetUser)
        {
            IrcMessage message;

            targetUser = targetUser.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (!userlist.UserIsOperator(nick))
                return false;

            userlist.SetUserOperatorStatus(targetUser, true);
            message = new Notice(nick, targetUser + " was set as a bot operator!");
            SendMessage(message);
            return true;
        }

        private void AddCustomCommand(string CHANNEL, string args, string nick)
        {
            string[] splits;
            IrcMessage message;
            splits = args.Split(new char[] { ' ' }, 2);

            if (CustomCommand.commandExists(splits[0], StringLib.CustomCommands) == true)
            {
                message = new Privmsg(CHANNEL, "Command " + splits[0] + " already exists.");
                SendMessage(message);

                return;
            }

            StringLib.CustomCommands.Add(new CustomCommand(nick, splits[0], splits[1]));
            CustomCommand.saveCustomCommands(StringLib.CustomCommands);
        }

        private void AddFunk(string args, string nick)
        {
            if (userlist.UserIsMuted(nick) || !Settings.Default.funkEnabled) return;

            string[] splits;
            splits = args.Split(new char[] { ' ' }, 2);

            if (string.Compare(splits[0].ToLower(), "add") == 0)
                args = args.Replace("add ", string.Empty);

            //TODO: simplify this if
            if ((args.Contains("youtu.be") && (!args.Contains("?v=") && !args.Contains("&v=")))
                            || (args.Contains("youtube") && args.Contains("watch") && (args.Contains("?v=") || args.Contains("&v="))))
            {
                string id = YoutubeUseful.getYoutubeIdFromURL(args);
                string result = YoutubeUseful.getYoutubeInfoFromID(id);

                args = result + " : " + args;
            }

            StringLib.Funk.Add(args);

            StringLib.SaveLibrary("funk");
            stats.funk();
        }

        private void AddGreetings(string args, string nick)
        {
            userlist.SetGreeting(nick, args, true);
            userlist.SaveData();
        }

        private void AddQuote(string args, string nick)
        {
            if (userlist.UserIsMuted(nick) || !Settings.Default.quotesEnabled) return;

            string add;

            if (StringLib.Quotes == null)
                StringLib.Quotes = new List<string>();

            if (string.Compare(args.Split(new char[] { ' ' }, 2)[0], "add", true) == 0)
                add = Useful.GetBetween(args, "add ", null);
            else
                add = args;

            if (!string.IsNullOrWhiteSpace(add))
                StringLib.Quotes.Add(add);

            StringLib.SaveLibrary("quotes");
        }

        private void AnimeSearch(string CHANNEL, string nick, string query)
        {
            //TODO: Simplify this anime search

            IrcMessage message;
            MyAnimeList.MalAnimeData malAnime = new MyAnimeList.MalAnimeData();
            Google.GoogleSearch googleObject = new Google.GoogleSearch();
            WebClient webClient = new WebClient()
            {
                Encoding = Encoding.UTF8
            };
            webClient.Headers.Add("User-Agent", Settings.Default.UserAgent);
            string searchResultName = "";
            string id = "";
            string searchResultURL = string.Empty;
            int i_max = 5;

            bool foundGoogle = false;

            if (userlist.UserIsMuted(nick) || Settings.Default.silence || !Settings.Default.aniSearchEnabled || string.IsNullOrWhiteSpace(query))
                return;

            if (query.ToLower().Contains("-u") || query.ToLower().Contains("-user"))
            {
                AnimeUserSearch(CHANNEL, query);
                return;
            }

            webClient.Credentials = new NetworkCredential(Settings.Default.malUser, Settings.Default.malPass);

           
            if ((googleObject = GoogleAnimeSearch(query)) == null)
            {
                SendMessage(new Privmsg(CHANNEL, "[Error] Google Search failed"));
                return;
            }

            if (googleObject.items == null)
            {
                SendMessage(new Privmsg(CHANNEL, "[Error] Google Search had no results, try https://myanimelist.net/anime.php?q=" + query));
                return;
            }

            if (googleObject.items.Length < 5)
                i_max = googleObject.items.Length - 1;

            for(int i = 0; i <= i_max; i++)
            {
                if (googleObject.items[i].link.Contains("myanimelist.net/anime/"))
                {
                    foundGoogle = true;
                    string[] split = googleObject.items[i].link.Split('/');
                    searchResultURL = googleObject.items[i].link;

                    if (split.Length >= 5)
                    {
                        searchResultName = split[5];
                        id = split[4];
                    }
                    else
                        searchResultName = query;

                    break;
                }
            }

            if (foundGoogle)
            {
                string animeName = searchResultName.Replace(" ", "+").Replace("_", "+").Replace("%20", "+");
                string getString = "https://myanimelist.net/api/anime/search.xml?q=" + animeName;

                string xmlAnime = webClient.DownloadString(getString);

                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MyAnimeList.MalAnimeData));
                    using (StringReader reader = new StringReader(xmlAnime))
                    {
                        malAnime = (MyAnimeList.MalAnimeData)(serializer.Deserialize(reader));
                    }
                }
                catch { }

                if (malAnime.entry == null)
                {
                    string score = Useful.GetBetween(xmlAnime, "<score>", "</score>");
                    string episodes = Useful.GetBetween(xmlAnime, "<episodes>", "</episodes>");
                    string title = Useful.GetBetween(xmlAnime, "<title>", "</title>");
                    string status = Useful.GetBetween(xmlAnime, "<status>", "</status>");

                    if (episodes == "0" || episodes == string.Empty)
                        episodes = "?";
                    if (string.IsNullOrWhiteSpace(score))
                        score = "?";
                    if (string.IsNullOrWhiteSpace(title))
                        title = "?";
                    if (string.IsNullOrWhiteSpace(status))
                        status = "?";

                    message = new Privmsg(CHANNEL, "\x02" + title + "\x02 : " + "[" + status + "] " + "[" + episodes + " episode"
                        + (episodes == "1" ? "" : "s") + "] " + "[" + score + " / 10] " + "-> " + searchResultURL);
                }
                else
                {
                    int index = 0;

                    for (int o = 0; o < malAnime.entry.Length; o++)
                    {
                        if (malAnime.entry[o].id.ToString() == id)
                        {
                            index = o;
                            break;
                        }
                    }

                    string score = malAnime.entry[index].score.ToString();
                    string episodes = malAnime.entry[index].episodes.ToString();
                    string title = malAnime.entry[index].title;
                    string status = malAnime.entry[index].status;

                    if (episodes == "0")
                        episodes = "?";

                    message = new Privmsg(CHANNEL, "\x02" + title + "\x02 : " + "[" + status + "] " + "[" + 
                        episodes + " episode" + (episodes == "1" ? "" : "s") + "] " + "[" + score + " / 10] " + "-> " + searchResultURL);
                }
            }
            else
                message = new Privmsg(CHANNEL, "Search Result: " + googleObject.items[0].link);

            SendMessage(message);
            stats.anime();
        }

        private void AnimeUserSearch(string CHANNEL, string query)
        {
            int i = 0;
            int i_max = 5;
            bool found = false;
            IrcMessage message;
            WebClient webClient = new WebClient()
            {
                Encoding = Encoding.UTF8
            };
            webClient.Headers.Add("User-Agent", Settings.Default.UserAgent);

            Google.GoogleSearch g = GoogleAnimeSearch(query);

            if (g == null)
            {
                message = new Privmsg(CHANNEL, "[Error] Google Search failed");
                SendMessage(message);
                return;
            }

            if (g.items == null)
            {
                message = new Privmsg(CHANNEL, "[Error] Google Search had no results, try https://myanimelist.net/anime.php?q=" + query);
                SendMessage(message);
                return;
            }

            if (g.items.Length < 5)
                i_max = g.items.Length - 1;

            while (i <= i_max && found == false)
            {
                if (g.items[i].link.Contains("myanimelist.net/profile/"))
                    found = true;
                else i++;
            }

            if (found)
            {
                string xmlUser = webClient.DownloadString("https://myanimelist.net/malappinfo.php?u=" + query.Replace("%20", string.Empty).Replace("-u", string.Empty)).Trim();

                MyAnimeList.MalUserData u = new MyAnimeList.MalUserData();

                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MyAnimeList.MalUserData));
                    using (StringReader reader = new StringReader(xmlUser))
                    {
                        u = (MyAnimeList.MalUserData)(serializer.Deserialize(reader));
                    }
                }
                catch { }

                if (u == null || u.myinfo == null || xmlUser.Contains("<error>Invalid username</error>"))
                    message = new Privmsg(CHANNEL, "Error fetching user stats");
                else
                    message = new Privmsg(CHANNEL, "[" + u.myinfo.user_name + "] " + "[Completed: " + u.myinfo.user_completed + " | Currently Watching: " + u.myinfo.user_watching + "]" + " -> http://myanimelist.net/profile/" + u.myinfo.user_name);
            }
            else
                message = new Privmsg(CHANNEL, "Search Result: " + g.items[0].link);

            SendMessage(message);
            stats.anime();
        }

        private async void BotThink(string CHANNEL, string line, string nick)
        {
            IrcMessage message;

            if (userlist.UserIsMuted(nick) || !Settings.Default.botThinkEnabled) return;

            string[] split = line.Split(' ');

            string newLine = "";

            for (int i = 0; i < split.Length - 1; i++)
                newLine += split[i] + " ";

            newLine = newLine.Trim();
            newLine = newLine.TrimEnd(',');

            try
            {
                CleverbotResponse answer = await cleverbotSession.GetResponseAsync(newLine);
                message = new Privmsg(CHANNEL, answer.Response);
            }
            catch
            {
                message = new Privmsg(CHANNEL, "Sorry, but i can't think right now");
                cleverbotSession = new CleverbotSession(Settings.Default.cleverbotAPI);
            }

            SendMessage(message);
        }

        private void CheckIfTimeout(object sender, EventArgs e)
        {
            if (waitingForPong)
            {
                waitingForPong = false;
                timeoutTimer.Enabled = false;
                OnTimeout(EventArgs.Empty);
            }
        }

        private void CheckSeen(string CHANNEL, string nick, string arg)
        {
            IrcMessage message;
            DateTime seenTime;
            DateTime now = DateTime.UtcNow;
            TimeSpan diff;

            if (userlist.UserIsMuted(nick)) return;

            seenTime = userlist.GetUserSeenUTC(arg);

            if (seenTime.CompareTo(new DateTime(0)) == 0)
                message = new Privmsg(CHANNEL, "The user has not been seen yet, or an error as occured");
            else
            {
                diff = now.Subtract(seenTime);
                string timeDiff = string.Empty;

                if (diff.Days >= 1)
                    if (diff.Days == 1)
                        timeDiff += diff.Days + " day, ";
                    else
                        timeDiff += diff.Days + " days, ";

                if (diff.Hours >= 1)
                    if (diff.Hours == 1)
                        timeDiff += diff.Hours + " hour ago";
                    else
                        timeDiff += diff.Hours + " hours ago";
                else
                    if (diff.Minutes == 1)
                    timeDiff += diff.Minutes + " minute ago";
                else
                    timeDiff += diff.Minutes + " minutes ago";

                message = new Privmsg(CHANNEL, "The user " + arg + " was last seen " + timeDiff);
            }

            SendMessage(message);
        }

        private void Choose(string CHANNEL, string user, string arg)
        {
            if (userlist.UserIsMuted(user) || !Settings.Default.chooseEnabled) return;

            Random r = new Random();
            string[] choices;
            IrcMessage message;

            arg = arg.Replace("  ", " ");

            if (arg.Contains(','))
                choices = arg.Split(new char[] { ',' });
            else
                choices = arg.Split(new char[] { ' ' });

            if (choices.Length != 0)
            {
                int random = r.Next(choices.Length);
                message = new Privmsg(CHANNEL, user + ": " + choices[random].Trim());
                SendMessage(message);
            }
            stats.choose();
        }

        private void CleanMessages(string args, string user)
        {
            if (!userlist.UserIsOperator(user))
                return;

            string[] split = args.Split(new char[] { ' ' });

            if (split.Length == 1)
            {
                if (userlist.ClearUserMessages(args))
                    SendMessage(new Notice(user, "Success!"));
            }
            else
                if (userlist.ClearUserMessages(split[0], split[1]))
                SendMessage(new Notice(split[0], "Success!"));
        }

        private void CtcpPing(string u, string stamp)
        {
            IrcMessage message = new Notice(u, "\x01" + "PING " + stamp + "\x01");
            SendMessage(message);
        }

        //CTCP replies
        private void CtcpTime(string u)
        {
            DateTime dateValue = new DateTime();
            dateValue = DateTime.Now;
            string week = dateValue.ToString("ddd", new CultureInfo("en-US"));
            string month = dateValue.ToString("MMM", new CultureInfo("en-US"));
            string day = DateTime.Now.ToString("dd");
            string hour = DateTime.Now.ToString("HH:mm:ss");

            string complete = week + " " + month + " " + day + " " + hour;

            IrcMessage message = new Notice(u, "\x01" + "TIME " + complete + "\x01");
            SendMessage(message);
        }

        private void CtcpVersion(string u)
        {
            IrcMessage message = new Notice(u, "\x01" + "VERSION " + "NarutoBot3 by Ricardo1991, compiled on " + GetCompilationDate.RetrieveLinkerTimestamp().ToUniversalTime() + "\x01");
            SendMessage(message);
        }

        private void DoGgEz(string CHANNEL, string nick)
        {
            if (userlist.UserIsMuted(nick)) return;

            Random r = new Random();
            string[] ggez = {   "Great game, everyone!",
                                "It was an honor to play with you all. Thank you.",
                                "Wishing you all the best.",
                                "Good game! Best of luck to you all!",
                                "Gee whiz! That was fun. Good playing!",
                                "Well played. I salute you all.",
                                "I'm wrestling with some insecurity issues in my life but thank you all for playing with me.",
                                "Ah shucks... you guys are the best!",
                                "It's past my bedtime. Please don't tell my mommy.",
                                "I could really use a hug right now.",
                                "I feel very, very small... please hold me...",
                                "I'm trying to be a nicer person. It's hard, but I am trying, guys.",
                                "C'mon, Mom! One more game before you tuck me in. Oops mistell.",
                                "Mommy says people my age shouldn't suck their thumbs.",
                                "For glory and honor! Huzzah comrades!", };

            IrcMessage message = new Privmsg(CHANNEL, (ggez[r.Next(ggez.Length)]));
            SendMessage(message);
        }

        private void FactUser(string CHANNEL, string nick, string args)
        {
            Random r = new Random();
            string target = "";
            string factString;
            int factID;

            List<User> listU = userlist.GetAllOnlineUsers();

            int MAX_FACTS = 300;

            var regex = new Regex(Regex.Escape("<random>"));

            IrcMessage message;

            if (userlist.UserIsMuted(nick) || string.IsNullOrEmpty(nick) || StringLib.Facts.Count < 1 ||
                Settings.Default.silence == true || Settings.Default.factsEnabled == false)

                return;

            

            if (string.IsNullOrWhiteSpace(args) || args.ToLower() == "random")
                target = listU[r.Next((listU.Count))].Nick;
            else
                target = args.Trim();

            if (StringLib.Facts.Count <= MAX_FACTS)
            {
                StringLib.FactsUsed.Clear();
                factID = r.Next(StringLib.Facts.Count);
                StringLib.FactsUsed.Insert(0, factID);
            }
            else
            {
                do factID = r.Next(StringLib.Facts.Count);
                while (StringLib.FactsUsed.Contains(factID));
            }

            if (StringLib.FactsUsed.Count >= MAX_FACTS)
            {
                StringLib.FactsUsed.Remove(StringLib.FactsUsed[StringLib.FactsUsed.Count - 1]);
            }

            StringLib.FactsUsed.Insert(0, factID);

            factString = StringLib.Facts[factID];

            factString = Useful.FillTags(factString, nick.Trim(), target, userlist);

            message = new Privmsg(CHANNEL, factString);

            SendMessage(message);
            stats.fact();
        }

        private void Filmot(string CHANNEL, string args, string user)
        {
            if (userlist.UserIsMuted(user)) return;

            if (string.IsNullOrWhiteSpace(args))
            {
                if (string.IsNullOrWhiteSpace(lastImgurID))
                    SendMessage(new Privmsg(CHANNEL, "No links in memory to convert"));
                SendMessage(new Privmsg(CHANNEL, "http://i.filmot.org/" + lastImgurID));
            }
            else
            {
                string id = Useful.GetBetween(args, "imgur.com/", "");
                SendMessage(new Privmsg(CHANNEL, "http://i.filmot.org/" + id));
            }
        }

        private void GetURLInfo(string CHANNEL, string url)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            string html, title;
            IrcMessage message;

            WebRequest webRequest = HttpWebRequest.Create(url);
            webRequest.Method = "HEAD";
            webRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-gb;q=0.8, en-us;q=0.7, en;q=0.6");

            try
            {
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    foreach (string header in webResponse.Headers)
                        headers.Add(header, webResponse.Headers[header]);
                }
            }
            catch
            {
            }

            if (headers.ContainsKey("Content-Type"))
            {
                if (headers["Content-Type"].Contains("text/html"))
                {
                    WebRequest request = WebRequest.Create(url);
                    request.Proxy = null;
                    request.Timeout = 30000;

                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);

                        html = reader.ReadToEnd();

                        if (!html.Contains("<title")) return;

                        string temp = Useful.GetBetween(html, "<title", "</title>");
                        title = Useful.GetBetween(temp, ">", "</title>");

                        if (!string.IsNullOrWhiteSpace(title))
                        {
                            title = title.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ').Trim();
                            title = HttpUtility.HtmlDecode(title);
                            if (title.ToLower().Contains("gyazo")) return;    //avoid those pages

                            message = new Privmsg(CHANNEL, "[title] " + title);
                            SendMessage(message);
                        }
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        string[] sizes = { "B", "KB", "MB", "GB" };
                        double len = Convert.ToDouble(headers["Content-Length"]);
                        int order = 0;
                        while (len >= 1024 && order + 1 < sizes.Length)
                        {
                            order++;
                            len = len / 1024;
                        }

                        // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
                        // show a single decimal place, and no space.
                        string result = string.Format("{0:0.##} {1}", len, sizes[order]);

                        message = new Privmsg(CHANNEL, "[" + headers["Content-Type"] + "] " + result);
                        SendMessage(message);
                    }
                    catch { }
                }
            }
        }

        private Google.GoogleSearch GoogleAnimeSearch(string query)
        {
            Google.GoogleSearch g = new Google.GoogleSearch();
            WebClient webClient = new WebClient()
            {
                Encoding = Encoding.UTF8
            };
            webClient.Headers.Add("User-Agent", Settings.Default.UserAgent);

            if (string.IsNullOrWhiteSpace(query)) throw new Exception("No Query");

            query = query.ToLower().Replace(" ", "%20").Replace("-user", string.Empty).Replace("-u", string.Empty);

            string getString = "https://www.googleapis.com/customsearch/v1?key=" + Settings.Default.apikey + "&cx=" + Settings.Default.cxKey + "&q=" + query;

            try
            {
                string jsonGoogle = webClient.DownloadString(getString);
                JsonConvert.PopulateObject(jsonGoogle, g);
            }
            catch
            {
                return null;
            }

            return g;
        }

        private void GreetToogle(string nick)
        {
            IrcMessage message;
            string state = "disabled";

            foreach (User u in userlist.Users)
            {
                if (u.Nick == nick)
                {
                    u.GreetingEnabled = !u.GreetingEnabled;

                    if (u.GreetingEnabled) state = "enabled";
                    message = new Notice(nick, "Your Greeting is now " + state);

                    userlist.SaveData();

                    SendMessage(message);
                    return;
                }
            }

            SendMessage(new Notice(nick, "You didn't set a Greeting yet"));
        }

        private void GreetUser(string nick)
        {
            if (userlist.UserIsMuted(nick) || !Settings.Default.greetingsEnabled || Settings.Default.silence) return;

            foreach (User u in userlist.Users)
            {
                if (string.Compare(u.Nick, nick, true) == 0 && u.GreetingEnabled)
                {
                    IrcMessage mensagem = new Privmsg(Client.HOME_CHANNEL, u.Greeting);
                    SendMessage(mensagem);
                    stats.greet();
                    break;
                }
            }
        }

        private void Hello(string CHANNEL, string nick)
        {
            if (!userlist.UserIsMuted(nick) && Settings.Default.hello_Enabled == true && Settings.Default.silence == false)
            {
                IrcMessage message = new Privmsg(CHANNEL, "Hello " + nick + "!");
                SendMessage(message);
            }
        }

        private void Help(string nick)
        {
            IrcMessage message;

            if (!userlist.UserIsMuted(nick) && Settings.Default.help_Enabled == true)
            {
                foreach (string h in StringLib.Help)
                {
                    message = new Notice(nick, h.Replace("\n", "").Replace("\r", ""));
                    SendMessage(message);
                }
                stats.help();
            }
        }

        private void InspectMessages(string args, string user)
        {
            if (userlist.UserIsOperator(user))
            {
                MessageDelivery(args, user);
            }
        }

        private void KillUser(string CHANNEL, string nick, string args)
        {
            Random r = new Random();
            string target = "";
            string killString;
            int killID;

            List<User> listU = userlist.GetAllOnlineUsers();

            int MAX_KILLS = 500;

            if (userlist.UserIsMuted(nick) || string.IsNullOrEmpty(nick) || StringLib.Kill.Count < 1) return;

            var regex = new Regex(Regex.Escape("<random>"));

            if (Settings.Default.silence == false && Settings.Default.killEnabled == true)
            {
                IrcMessage message;
                if (args.ToLower().Trim() == "la kill")
                {
                    message = new Privmsg(CHANNEL, nick + " lost his way");
                }
                else if (args.ToLower() == "me baby".Trim())
                {
                    message = new Privmsg(CHANNEL, "WASSA WASSA https://www.youtube.com/watch?v=dwkClIFBMEE");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(args) || args.ToLower() == "random")
                        target = listU[r.Next(listU.Count)].Nick;
                    else
                        target = args.Trim();

                    if (StringLib.Kill.Count <= MAX_KILLS)
                    {
                        StringLib.KillsUsed.Clear();
                        killID = r.Next(StringLib.Kill.Count);
                        StringLib.KillsUsed.Insert(0, killID);
                    }
                    else
                    {
                        do killID = r.Next(StringLib.Kill.Count);
                        while (StringLib.KillsUsed.Contains(killID));
                    }

                    if (StringLib.KillsUsed.Count >= MAX_KILLS)
                    {
                        StringLib.KillsUsed.Remove(StringLib.KillsUsed[StringLib.KillsUsed.Count - 1]);
                    }

                    StringLib.KillsUsed.Insert(0, killID);

                    killString = StringLib.Kill[killID];

                    killString = Useful.FillTags(killString, nick.Trim(), target, userlist);

                    if (killString.ToLower().Contains("<normal>"))
                    {
                        killString = killString.Replace("<normal>", string.Empty).Replace("<NORMAL>", string.Empty);
                        message = new Privmsg(CHANNEL, killString);
                    }
                    else
                    {
                        message = new ActionMessage(CHANNEL, killString);
                    }
                }

                SendMessage(message);
                stats.kill();
            }
        }

        private void LastKill(string CHANNEL, string nick, string arg)
        {
            if (userlist.UserIsMuted(nick) || string.IsNullOrEmpty(nick) || Settings.Default.silence || !Settings.Default.killEnabled || StringLib.KillsUsed.Count < 1) return;

            IrcMessage message;
            int index = 0;

            if (!string.IsNullOrWhiteSpace(arg))
            {
                try
                {
                    index = Convert.ToInt32(arg) - 1;
                }
                catch
                {
                    message = new Privmsg(CHANNEL, "Invalid Argument");
                    SendMessage(message);
                    return;
                }
            }

            if (StringLib.KillsUsed == null || StringLib.KillsUsed.Count == 0)
                message = new Privmsg(CHANNEL, "No kills since last reset");
            else if (StringLib.KillsUsed.Count < index + 1 || index < 0)
                message = new Privmsg(CHANNEL, "Out of Range");
            else if (index == 0)
                message = new Privmsg(CHANNEL, "[#" + StringLib.KillsUsed[0] + "] " + StringLib.Kill[StringLib.KillsUsed[0]]);
            else
                message = new Privmsg(CHANNEL, "(" + (index + 1) + " kills ago) " + "[#" + StringLib.KillsUsed[index] + "] " + StringLib.Kill[StringLib.KillsUsed[index]]);

            SendMessage(message);
        }

        private void Me(string CHANNEL, string args, string nick)
        {
            if (userlist.UserIsOperator(nick))
                SendMessage(new ActionMessage(CHANNEL, args));
        }

        private void MessageDelivery(string user)
        {
            MessageDelivery(user, user);
        }

        private void MessageDelivery(string user, string destinary)
        {
            int count = userlist.UserMessageCount(user);
            if (count == 0) return;

            IrcMessage message;

            SendMessage(new Privmsg(destinary, user + ", you have " + count + " message(s)"));

            for (int i = 0; i < count; i++)
            {
                System.Threading.Thread.Sleep(250);
                UserMessage m = userlist.GetUserMessage(user, i);
                TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(m.Timestamp);

                string timeDiff = "";

                //TODO: Simplify the if/elses here
                if (diff.Days > 3)
                {
                    timeDiff += diff.Days + " days ago";
                }
                else
                {
                    if (diff.Days >= 1)
                        if (diff.Days == 1)
                            timeDiff += diff.Days + " day, ";
                        else
                            timeDiff += diff.Days + " days, ";

                    if (diff.Hours >= 1)
                        if (diff.Hours == 1)
                            timeDiff += diff.Hours + " hour ago";
                        else
                            timeDiff += diff.Hours + " hours ago";
                    else
                        if (diff.Minutes == 1)
                        timeDiff += diff.Minutes + " minute ago";
                    else
                        timeDiff += diff.Minutes + " minutes ago";
                }

                message = new Privmsg(destinary, "#" + (i + 1) + " Sent " + timeDiff + "\t<" + m.Sender + "> " + m.Message);
                SendMessage(message);
            }

            SendMessage(new Privmsg(destinary, "Remove messages with " + Client.SYMBOL + "acknowledge or " + Client.SYMBOL + "a"));
        }

        private bool MuteUser(string nick, string targetUser)
        {
            IrcMessage message;

            targetUser = targetUser.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();

            if (!userlist.UserIsOperator(nick))
                return false;

            userlist.SetUserMuteStatus(targetUser, true);
            message = new Notice(nick, targetUser + " was muted!");
            SendMessage(message);
            return true;
        }

        private void NickGen(string CHANNEL, string nick, string args)
        {
            if (string.IsNullOrEmpty(nick)) return;
            Random rnd = new Random();

            bool randomnumber = false;
            bool randomUpper = false;
            bool switchLetterNumb = false;
            bool Ique = false;
            bool targeted = false;

            string target = null;
            IrcMessage message;

            if (userlist.UserIsMuted(nick)) return;

            if (StringLib.NickGenStrings.Count < 2)
            {
                message = new Privmsg(CHANNEL, "Nickname generator was not initialized properly");
                SendMessage(message);
                return;
            }

            if (Settings.Default.silence == false && Settings.Default.nickEnabled == true)
            {
                foreach (string s in args.Split(' '))
                {
                    if (s.ToLower() == "random")
                    {
                        switchLetterNumb = rnd.Next(0, 100) <= 30;
                        randomnumber = rnd.Next(0, 100) <= 30;
                        randomUpper = rnd.Next(0, 100) <= 30;
                        Ique = rnd.Next(0, 100) <= 10;
                    }
                    else if (s.ToLower() == "for")
                    {
                        targeted = true;
                        target = Useful.GetBetween(args, "for ", null);
                    }

                    if (s.ToLower() == "sl") switchLetterNumb = true;
                    else if (s.ToLower() == "rn") randomnumber = true;
                    else if (s.ToLower() == "ru") randomUpper = true;
                    else if (s.ToLower() == "iq") Ique = true;
                }

                string nick_ = NarutoBot3.NickGenerator.GenerateNick(StringLib.NickGenStrings, StringLib.NickGenStrings.Count, randomnumber, randomUpper, switchLetterNumb, Ique);

                if (targeted)
                    message = new Privmsg(CHANNEL, nick + " generated a nick for " + target + ": " + nick_);
                else
                    message = new Privmsg(CHANNEL, nick + " generated the nick " + nick_);

                SendMessage(message);
                stats.nick();
            }
        }

        private void OpList(string nick)
        {
            IrcMessage message;

            if (userlist.UserIsOperator(nick))
            {
                message = new Notice(nick, "Bot operators:");
                SendMessage(message);
                foreach (User u in userlist.Users)
                {
                    if (u.IsOperator)
                        SendMessage(new Notice(nick, "     -> " + u.Nick));
                }
            }
        }

        private async void ParseQuestion(string CHANNEL, string user, string arg)
        {
            //This needs to go or be simplified

            if (userlist.UserIsMuted(user)) return;
            if (Settings.Default.silence || !Settings.Default.questionEnabled) return;

            string subjectNPL = qq.getSubject(arg);

            IrcMessage message = null;
            Random r = new Random();

            List<User> listU = userlist.GetAllOnlineUsers();

            string[] howMany = { "I dont know, maybe", "Probably", "More than", "Less than", "I think it was", "I don't know, so i'll give you a random number:", "", "It's" };

            string[] howIs = { "fine", "not fine", "lost", "being retarded again", "not feeling well", "being annoying as always", "probably hungry", "good", "all right", "upset", "bored" };

            string[] because = { "was lost", "is stupid", "asked me to", "was asked to", "has an inferiority complex", "is a terrible person",
                                   "felt like so", "wanted to", "liked it", "already had plans to do it", "wanted it that way"  };

            string[] when = { "maybe next week", "a few days ago", "last year", "yesterday", "tomorrow", "in a few hours",
                                "nobody knows", "next year", "it was yesterday", "I'm not sure", "next week" };

            string[] why = { "I dont know, maybe", "I don't know", "Yeah", "Nope.", "Yes.", "No.", "Probably", "Everything makes me believe so",
                               "Not sure, ask somebody else", "I don't know, im not wikipedia", "Sorry, but i don't know", "Because that was destined to be so" };

            string[] where = { "somewhere in a far away land" , "on the Youtube datacenter", "behind you", "in your house", "in Europe", "near Lygs", "that special place",
                               "in outer space","somewhere i belong", "On the shaddiest subreddit","on tumblr", "in space", "on your computer",
                               "beneath your bed!", "where you didnt expect", "near your house"};

            string[] whoDid = { "Probably", "Maybe it was", "I'm sure it was", "I don't think it was", "I suspect", "Someone told me it was", "I think it was" };
            string[] whoDo = { "Probably", "Maybe it is", "I'm sure it is", "I don't think it was", "I suspect", "Someone told me it is", "I think it is" };

            string[] what = { "Sorry, I have no idea","Can you ask somebody else? I really don't know","No idea, try Google", "I'm not good with questions",
                            "I'm under pressure, i can't answer you that","Stop bullying me!" };

            string[] whyY = { "Im not sure if", "Yeah,", "Yes,", "Correct!", "I think", "I believe that", "" };

            string[] whyN = { "Nope,", "No,", "I think that", "I believe that", "Negative!", "" };

            if (arg[arg.Length - 1] == '?')
            {
                arg = arg.Replace("?", string.Empty).TrimStart(new char[] { ' ' });
                string[] split = arg.Split(new char[] { ' ' });

                if (split.Length >= 1)
                {
                    if (string.Compare(split[0], "how", true) == 0)
                    {
                        if (split.Length >= 2)
                        {
                            if (string.Compare(split[1], "many", true) == 0)
                            {
                                if (string.Compare(arg, "how many killstrings do you have", true) == 0 || string.Compare(arg, "how many kills do you have", true) == 0)
                                    message = new Privmsg(CHANNEL, "I have " + StringLib.Kill.Count + " killstrings loaded in.");
                                else if (arg == "how many fucks do you give")
                                    message = new Privmsg(CHANNEL, "I always give 0 fucks.");
                                else
                                    message = new Privmsg(CHANNEL, (howMany[r.Next(howMany.Length)] + " " + r.Next(21)).Trim());
                            }
                            else if (split.Length >= 2 && string.Compare(split[1], "are", true) == 0)
                            {
                                if (string.Compare(arg, "how are you", true) == 0 || string.Compare(arg, "how are you doing", true) == 0)
                                    message = new Privmsg(CHANNEL, "I'm fine, thanks for asking. And you?");
                                else
                                    message = new Privmsg(CHANNEL, "I dont know yet, ask later");
                            }
                            else if (string.Compare(split[1], "is", true) == 0)
                            {
                                if (split.Length == 3)
                                    message = new Privmsg(CHANNEL, split[2] + " is " + howIs[r.Next(howIs.Length)]);
                            }
                            else if (string.Compare(split[1], "did", true) == 0 && string.Compare(split[split.Length], "die", true) == 0)
                            {
                                KillUser(CHANNEL, user, Useful.GetBetween(arg, "did", "die"));
                                return;
                            }
                            else if (string.Compare(split[1], "old", true) == 0)
                            {
                                if (split.Length >= 4)
                                {
                                    string replaced = QuestionsRegex(subjectNPL);

                                    if (string.Compare(split[2], "is", true) == 0)
                                    {
                                        message = new Privmsg(CHANNEL, replaced + " is " + r.Next(41) + " years old");
                                    }
                                    else if (string.Compare(split[2], "are", true) == 0)
                                    {
                                        if (string.Compare(subjectNPL, "you", true) == 0)
                                            message = new Privmsg(CHANNEL, "I was compiled on " + GetCompilationDate.RetrieveLinkerTimestamp().ToString("R"));
                                        else
                                            message = new Privmsg(CHANNEL, replaced + " are " + r.Next(41) + " years old");
                                    }
                                    else if (string.Compare(split[2], "am", true) == 0 || string.Compare(split[3], "i", true) == 0)
                                        message = new Privmsg(CHANNEL, "You are " + r.Next(41) + " years old");
                                }
                            }
                        }
                        else
                            message = new Privmsg(CHANNEL, user + ", no idea...");
                    }
                    else if (string.Compare(split[0], "how's", true) == 0)
                    {
                        string replaced = QuestionsRegex(subjectNPL);
                        message = new Privmsg(CHANNEL, replaced + " is " + howIs[r.Next(howIs.Length)]);
                    }
                    else if (string.Compare(split[0], "why", true) == 0)
                    {
                        if (split.Length >= 2)
                            message = new Privmsg(CHANNEL, "Because " + listU[r.Next(listU.Count)].Nick + " " + because[r.Next(because.Length)]);
                    }
                    else if (string.Compare(split[0], "is", true) == 0)
                    {
                        bool yes = false;

                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            if (yes)
                                message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject.Replace("your", "my") + " is " + replaced).Trim());
                            else
                                message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject.Replace("your", "my") + " isn't " + replaced).Trim());
                        }
                    }
                    else if (string.Compare(split[0], "was", true) == 0)
                    {
                        bool yes = false;

                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            if (yes)
                                message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject.Replace("your", "my").Replace("Your", "my") + " was " + replaced).Trim());
                            else
                                message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject.Replace("your", "my").Replace("Your", "my") + " wasn't " + replaced).Trim());
                        }
                    }
                    else if (string.Compare(split[0], "when", true) == 0)
                    {
                        message = new Privmsg(CHANNEL, when[r.Next(when.Length)]);
                    }
                    else if (string.Compare(split[0], "are", true) == 0)
                    {
                        if (string.Compare(arg, "are you real", true) == 0)
                            message = new Privmsg(CHANNEL, "Yes, i am real");
                        else if (string.Compare(arg, "are you a real person", true) == 0 || string.Compare(arg, "are you a real human", true) == 0 || string.Compare(arg, "are you human", true) == 0)
                            message = new Privmsg(CHANNEL, "No, i'm a bot");
                        else
                        {
                            if (r.Next(100) < 15)
                                message = new Privmsg(CHANNEL, why[r.Next(why.Length)]);
                            else
                            {
                                bool yes = false;
                                if (r.Next(0, 2) == 1)
                                    yes = true;

                                string subject = subjectNPL;
                                string rest = "";

                                for (int i = 1; i < split.Length; i++)
                                {
                                    rest += split[i] + " ";
                                }
                                rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                                string replaced = QuestionsRegex(rest);

                                if (subject.Trim() == "you")
                                {
                                    if (yes)
                                        message = new Privmsg(CHANNEL, ((whyY[r.Next(whyY.Length)]) + " " + "I'm " + replaced).Trim());
                                    else
                                        message = new Privmsg(CHANNEL, ((whyN[r.Next(whyN.Length)]) + " " + "I'm not " + replaced).Trim());
                                }
                                else
                                {
                                    subject = QuestionsRegex(subject);

                                    if (yes)
                                        message = new Privmsg(CHANNEL, ((whyY[r.Next(whyY.Length)]) + " " + subject + " are " + replaced).Trim());
                                    else
                                        message = new Privmsg(CHANNEL, ((whyN[r.Next(whyN.Length)]) + " " + subject + " aren't " + replaced).Trim());
                                }
                            }
                        }
                    }
                    else if (string.Compare(split[0], "can", true) == 0)
                    {
                        if (string.Compare(arg, "can you give me a nick", true) == 0 || string.Compare(arg, "can you make me a nick", true) == 0 ||
                            string.Compare(arg, "can you generate a nick", true) == 0 || string.Compare(arg, "can you create a nick", true) == 0 ||
                            string.Compare(arg, "can you make me a new nick", true) == 0)
                        {
                            message = new Privmsg(CHANNEL, "Yes, here it is: " + NarutoBot3.NickGenerator.GenerateNick(StringLib.NickGenStrings, StringLib.NickGenStrings.Count, false, false, false, false));
                            stats.nick();
                        }
                        else if (arg.ToLower().Contains("can you kill "))
                        {
                            KillUser(CHANNEL, user, Useful.GetBetween(arg.ToLower(), "can you kill ", ""));
                            return;
                        }
                        else
                            message = new Privmsg(CHANNEL, why[r.Next(why.Length)]);
                    }
                    else if (string.Compare(split[0], "would", true) == 0)
                    {
                        if (string.Compare(arg, "would you make me a nick", true) == 0 || string.Compare(arg, "would you generate a nick", true) == 0 ||
                            string.Compare(arg, "would you create a nick", true) == 0 || string.Compare(arg, "would you make me a new nick", true) == 0)
                            message = new Privmsg(CHANNEL, "Yes, here it is: " + NarutoBot3.NickGenerator.GenerateNick(StringLib.NickGenStrings, StringLib.NickGenStrings.Count, false, false, false, false));
                        else
                            message = new Privmsg(CHANNEL, why[r.Next(why.Length)]);
                    }
                    else if (string.Compare(split[0], "where", true) == 0)
                    {
                        message = new Privmsg(CHANNEL, where[r.Next(where.Length)]);
                    }
                    else if (string.Compare(split[0], "who", true) == 0 || string.Compare(split[0], "who's", true) == 0)
                    {
                        if (string.Compare(arg, "who are you", true) == 0)
                            message = new Privmsg(CHANNEL, "I'm a bot!");
                        else if (split[1] == "do")
                            message = new Privmsg(CHANNEL, whoDid[r.Next(whoDo.Length)] + " " + listU[r.Next(listU.Count)].Nick);
                        else
                            message = new Privmsg(CHANNEL, whoDid[r.Next(whoDid.Length)] + " " + listU[r.Next(listU.Count)].Nick);
                    }
                    else if (string.Compare(split[0], "what", true) == 0 || string.Compare(split[0], "what's", true) == 0)
                    {
                        if (string.Compare(arg, "what are you", true) == 0)
                            message = new Privmsg(CHANNEL, "I'm a bot!");
                        else
                        {
                            message = new Privmsg(CHANNEL, what[r.Next(what.Length)]);
                        }
                    }
                    else if (string.Compare(split[0], "if", true) == 0)
                    {
                        if (Settings.Default.botThinkEnabled)
                        {
                            try
                            {
                                CleverbotResponse answer = await cleverbotSession.GetResponseAsync(arg + "?");
                                message = new Privmsg(CHANNEL, answer.Response);
                            }
                            catch
                            {
                                message = new Privmsg(CHANNEL, "Sorry, but i can't think right now");
                                cleverbotSession = new CleverbotSession(Settings.Default.cleverbotAPI);
                            }
                        }
                    }
                    else if (string.Compare(split[0], "am", true) == 0 && string.Compare(split[1], "i", true) == 0)
                    {
                        bool yes = false;

                        if (r.Next(0, 2) == 1)
                            yes = true;

                        string rest = "";

                        for (int i = 1; i < split.Length; i++)
                        {
                            rest += split[i] + " ";
                        }
                        rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                        string replaced = QuestionsRegex(rest);

                        if (yes)
                            message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " you are " + replaced).Trim());
                        else
                            message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " you aren't " + replaced).Trim());
                    }
                    else if (string.Compare(split[0], "do", true) == 0)
                    {
                        bool yes = false;
                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            if (string.Compare(split[1], "you", true) == 0)
                            {
                                if (yes)
                                    message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + "I " + replaced).Trim());
                                else
                                    message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + "I don't " + replaced).Trim());
                            }
                            else if (string.Compare(split[1], "i", true) == 0)
                            {
                                if (yes)
                                    message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + "you do " + replaced).Trim());
                                else
                                    message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + "you don't " + replaced).Trim());
                            }
                            else
                            {
                                if (yes)
                                    message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject + " do " + replaced).Trim());
                                else
                                    message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject + " doesn't " + replaced).Trim());
                            }
                        }
                    }
                    else if (string.Compare(split[0], "should", true) == 0)
                    {
                        bool yes = false;
                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            if (string.Compare(split[1], "you", true) == 0)
                            {
                                if (yes)
                                    message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + "I should " + replaced).Trim());
                                else
                                    message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + "I shouldn't " + replaced).Trim());
                            }
                            else if (string.Compare(split[1], "i", true) == 0)
                            {
                                if (yes)
                                    message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + "you should " + replaced).Trim());
                                else
                                    message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + "you shouldn't " + replaced).Trim());
                            }
                            else
                            {
                                if (yes)
                                    message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject + " should " + replaced).Trim());
                                else
                                    message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject + " shouldn't " + replaced).Trim());
                            }
                        }
                        else
                        {
                            if (yes)
                                message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)]).Trim());
                            else
                                message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)]).Trim());
                        }
                    }
                    else if (string.Compare(split[0], "did", true) == 0)
                    {
                        bool yes = false;
                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            if (string.Compare(split[1], "you", true) == 0)
                            {
                                if (yes)
                                    message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + "I did " + replaced).Trim());
                                else
                                    message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + "I didn't " + replaced).Trim());
                            }
                            else if (string.Compare(split[1], "i", true) == 0)
                            {
                                if (yes)
                                    message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + "you did " + replaced).Trim());
                                else
                                    message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + "you didn't " + replaced).Trim());
                            }
                            else
                            {
                                if (yes)
                                    message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject + " did " + replaced).Trim());
                                else
                                    message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject + " didn't " + replaced).Trim());
                            }
                        }
                    }
                    else if (string.Compare(split[0], "does", true) == 0)
                    {
                        bool yes = false;
                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            subject = QuestionsRegex(subject);

                            if (yes)
                                message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject + " does " + replaced).Trim());
                            else
                                message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject + " does not " + replaced).Trim());
                        }
                    }
                    else if (string.Compare(split[0], "will", true) == 0)
                    {
                        bool yes = false;

                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            subject = QuestionsRegex(subject);

                            if (yes)
                                message = new Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject + " will " + replaced).Trim());
                            else
                                message = new Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject + " won't " + replaced).Trim());
                        }
                    }
                }
                else
                {
                    if (Settings.Default.botThinkEnabled)
                    {
                        try
                        {
                            CleverbotResponse answer = await cleverbotSession.GetResponseAsync(arg + "?");
                            message = new Privmsg(CHANNEL, answer.Response);
                        }
                        catch
                        {
                            message = new Privmsg(CHANNEL, "Sorry, but i can't think right now");
                            cleverbotSession = new CleverbotSession(Settings.Default.cleverbotAPI);
                        }
                    }
                }
            }
            else
            {
                if (Settings.Default.botThinkEnabled)
                {
                    try
                    {
                        CleverbotResponse answer = await cleverbotSession.GetResponseAsync(arg + "?");
                        message = new Privmsg(CHANNEL, answer.Response);
                    }
                    catch
                    {
                        message = new Privmsg(CHANNEL, "Sorry, but i can't think right now");
                        cleverbotSession = new CleverbotSession(Settings.Default.cleverbotAPI);
                    }
                }
            }

            if (message != null && !string.IsNullOrWhiteSpace(message.body))
            {
                message.body = message.body.Replace("  ", " ");
                SendMessage(message);
                stats.question();
            }
        }

        private void Poke(string CHANNEL, string nick)
        {
            IrcMessage message;
            int userNumber = 0;
            Random rnd = new Random();

            if (userlist.UserIsMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.pokeEnabled == true)
            {
                List<User> list = userlist.GetAllOnlineUsers();

                do
                {
                    userNumber = rnd.Next((list.Count));
                }
                while (list[userNumber].Nick == nick);

                message = new ActionMessage(CHANNEL, "pokes " + list[userNumber].Nick);
                SendMessage(message);
                stats.poke();
            }
        }

        private void PopulateUserList(ParsedMessage messageObject)
        {
            foreach (string user in messageObject.SplitMessage[3].Split(' '))
            {
                userlist.SetUserOnlineStatus(Useful.RemoveUserMode(user), true);

                if (user[0] == '@')
                    userlist.SetUserChannelOP(user, true);
                else if (user[0] == '+')
                    userlist.SetUserChannelVoice(user, true);

                userlist.SetUserMode(user, Useful.GetUserMode(user));
            }
        }

        private void PrintFunk(string CHANNEL, string nick)
        {
            Random r = new Random();
            int i;
            IrcMessage message;

            if (userlist.UserIsMuted(nick) || !Settings.Default.funkEnabled || StringLib.Funk.Count == 0) return;

            i = r.Next(StringLib.Funk.Count);
            message = new Privmsg(CHANNEL, StringLib.Funk[i]);

            SendMessage(message);
            stats.funk();
        }

        private void PrintNames(string nick)
        {
            string list = "[";

            List<User> listU = userlist.GetAllOnlineUsers();

            foreach (User u in listU)
            {
                list += u.Nick + " ";
            }

            list += "]";

            IrcMessage message = new Privmsg(nick, list);
            SendMessage(message);
        }

        private void PrintQuote(string CHANNEL, string args, string nick)
        {
            Random r = new Random();
            IrcMessage message;

            if (userlist.UserIsMuted(nick) || !Settings.Default.quotesEnabled) return;

            if (string.IsNullOrWhiteSpace(args) && StringLib.Quotes.Count > 0) //print random
            {
                PrintRandomQuote(CHANNEL);
                return;
            }
            else if (args[0] == '#')    //Print quote by number
            {
                string split = args.Split(new char[] { ' ' }, 2)[0];
                int number = Convert.ToInt32(split.Replace("#", string.Empty));

                if (number <= StringLib.Quotes.Count)
                    message = new Privmsg(CHANNEL, StringLib.Quotes[number - 1]);
                else
                    message = new Privmsg(CHANNEL, "Quote number " + number + " does not exist");

                SendMessage(message);
                stats.quote();
                return;
            }
            else   //search
            {
                string[] queries = args.Trim().ToLower().Split(' ');
                List<string> restults = SearchQuotes(queries);

                if (restults.Count > 0)
                {
                    if (restults.Count > 1)
                    {
                        message = new Privmsg(CHANNEL, "Found " + restults.Count + " quotes. Showing one of them:");
                        SendMessage(message);
                    }
                    message = new Privmsg(CHANNEL, restults[r.Next(restults.Count)]);
                    SendMessage(message);
                    stats.quote();
                    return;
                }
                else
                {
                    message = new Privmsg(CHANNEL, "No Quotes Found!");

                    SendMessage(message);
                    stats.quote();
                    return;
                }
            }
        }

        private void PrintRandomQuote(string CHANNEL)
        {
            Random r = new Random();
            int i = r.Next(StringLib.Quotes.Count);
            IrcMessage message = new Privmsg(CHANNEL, StringLib.Quotes[i]);

            SendMessage(message);
            stats.quote();
        }

        private void ProcessMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            ParsedMessage messageObject;
            messageObject = new ParsedMessage(message);

            //TODO: Refactor this
            switch (messageObject.Type)
            {
                case ("PING"):

                    SendMessage(new Pong(messageObject.SplitMessage[0]));
                    break;

                case ("001"):
                case ("002"):
                case ("003"):

                    WriteMessage(messageObject.CompleteMessage.Split(new char[] { ' ' }, 2)[1]);
                    break;

                case ("004"): //server used for connection

                    Client.HOST_SERVER = messageObject.SplitMessage[1];
                    break;

                case ("005"):
                case ("250"):
                case ("251"):
                case ("252"):
                case ("254"):
                case ("255"):
                case ("265"):
                case ("266"):
                case ("333"): //Topic author and Time
                case ("366"): //End of /NAMES
                case ("375"): //START OF MOTD
                    WriteMessage(messageObject.SplitMessage[0]);
                    break;

                case ("332"): //TOPIC
                case ("TOPIC"):

                    TopicChanged(messageObject);
                    break;

                case ("353"): //USERLIST

                    PopulateUserList(messageObject);

                    OnCreate(EventArgs.Empty);
                    break;

                case ("372"): //MOTD

                    string motd = messageObject.CompleteMessage.Split(new char[] { ' ' }, 2)[1];
                    WriteMessage(motd, currentColorScheme.Motd);
                    break;

                case ("376"): //END OF MOTD

                    OnConnect(EventArgs.Empty);

                    if (!string.IsNullOrEmpty(Client.HOST_SERVER))
                        OnConnectedWithServer(EventArgs.Empty);

                    break;

                case ("433"): //Nickname is already in use.

                    OnDuplicatedNick(EventArgs.Empty);
                    WriteMessage("* " + messageObject.Type + " " + messageObject.CompleteMessage);
                    break;

                case ("438"): //Nickname change too fast

                    //TODO: revert back to old nick
                    WriteMessage(messageObject.SplitMessage[0]);

                    break;

                case ("PONG"):

                    string[] split = message.Split(':');
                    string pongcmd = split[2];

                    if (waitingForPong)
                    {
                        string currentStamp = Useful.GetTimestamp(DateTime.Now);
                        string format = "mmssffff";

                        DateTime now, then;

                        now = DateTime.ParseExact(currentStamp, format, System.Globalization.CultureInfo.CreateSpecificCulture("en-EN"));
                        then = DateTime.ParseExact(pongcmd, format, System.Globalization.CultureInfo.CreateSpecificCulture("en-EN"));

                        TimeSpan TimeDifference = now.Subtract(then);

                        waitingForPong = false;
                        timeoutTimer.Stop();

                        OnPongReceived(new PongEventArgs(TimeDifference));
                    }

                    break;

                case ("JOIN"):

                    string joinMessage;
                    string userJoin;

                    if (messageObject.Sender.Contains("!"))
                    {
                        userJoin = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!"));
                        joinMessage = messageObject.Sender.Substring(messageObject.Sender.IndexOf("!") + 1);
                    }
                    else
                    {
                        userJoin = messageObject.Sender;
                        joinMessage = "";
                    }

                    UpdateUserListSource(null, null);
                    WriteMessage("** " + userJoin + " (" + joinMessage + ") joined", currentColorScheme.Join);

                    char mode = Useful.GetUserMode(userJoin);
                    userJoin = Useful.RemoveUserMode(userJoin);

                    userlist.SetUserOnlineStatus(userJoin, true);
                    userlist.SetUserMode(userJoin, mode);

                    GreetUser(userJoin);

                    MessageDelivery(userJoin);

                    break;

                case ("PART"):
                case ("QUIT"):

                    UserPart(messageObject);
                    break;

                case ("NICK"):

                    string oldnick = messageObject.Sender;
                    if (messageObject.Sender.Contains("!"))
                        oldnick = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!"));
                    string newnick = messageObject.CompleteMessage;
                    char userMode = userlist.GetUserMode(oldnick);

                    if (userMode != '0')
                        newnick = userMode + newnick;

                    UpdateUserListSource(null, null);
                    WriteMessage("** " + oldnick + " is now known as " + newnick, currentColorScheme.Rename);

                    userlist.SetUserOnlineStatus(oldnick, false);
                    userlist.SetUserOnlineStatus(Useful.RemoveUserMode(newnick), true);

                    MessageDelivery(newnick);

                    break;

                case ("MODE"):

                    string modechange = messageObject.SplitMessage[1];

                    if (messageObject.SplitMessage.Length < 3)
                        break;

                    string affectedUser = messageObject.SplitMessage[2];

                    affectedUser = affectedUser.Split(new char[] { '!' }, 2)[0];

                    if (string.Compare(affectedUser, "*") == 0) return;

                    switch (modechange)
                    {
                        case ("+o"):
                            userlist.SetUserChannelOP(affectedUser, true);

                            userlist.SetUserChannelOP(affectedUser, true);
                            userlist.SetUserMode(affectedUser, '@');
                            break;

                        case ("+v"):
                            if (userlist.UserHasChannelOP(affectedUser))
                                userlist.SetUserMode(affectedUser, '@');
                            else
                                userlist.SetUserMode(affectedUser, '+');

                            userlist.SetUserChannelVoice(affectedUser, true);
                            break;

                        case ("+h"):
                            if (userlist.UserHasChannelOP(affectedUser))
                                userlist.SetUserMode(affectedUser, '@');
                            else
                                userlist.SetUserMode(affectedUser, '%');
                            break;

                        case ("+a"):
                            userlist.SetUserMode(affectedUser, '&');
                            break;

                        case ("+q"):
                            if (userlist.UserHasChannelOP(affectedUser))
                                userlist.SetUserMode(affectedUser, '@');
                            else
                                userlist.SetUserMode(affectedUser, '~');
                            break;

                        case ("-q"):
                            userlist.SetUserMode(affectedUser, '0');
                            break;

                        case ("-o"):
                            userlist.SetUserChannelOP(affectedUser, false);
                            if (userlist.UserHasChannelVoice(affectedUser))
                                userlist.SetUserMode(affectedUser, '+');
                            else
                                userlist.SetUserMode(affectedUser, '0');

                            break;

                        case ("-a"):

                        case ("-h"):
                            if (userlist.UserHasChannelOP(affectedUser))
                                userlist.SetUserMode(affectedUser, '@');
                            else if (userlist.UserHasChannelVoice(affectedUser))
                                userlist.SetUserMode(affectedUser, '+');
                            else
                                userlist.SetUserMode(affectedUser, '0');
                            break;

                        case ("-v"):
                            if (userlist.UserHasChannelOP(affectedUser))
                                userlist.SetUserMode(affectedUser, '@');
                            else
                                userlist.SetUserMode(affectedUser, '0');
                            userlist.SetUserChannelVoice(affectedUser, false);
                            break;

                        case ("+b"):
                        case ("-b"):
                        case ("+m"):
                        case ("-m"):
                            break;

                        default:
                            userlist.SetUserMode(affectedUser, '0');
                            break;
                    }

                    OnModeChange(new ModeChangedEventArgs(affectedUser, modechange));

                    break;

                case ("KICK"):

                    string kickedUser = messageObject.SplitMessage[1];

                    userlist.SetUserOnlineStatus(kickedUser, false);

                    WriteMessage("** " + kickedUser + " was kicked", currentColorScheme.Leave);
                    UpdateUserListSource(null, null);
                    break;

                case ("PRIVMSG"):

                    //TODO: this one really needs to be redone somehow

                    string user;
                    string messageSource = messageObject.Source;            //Who sent is the Source of the Message. (The Channel, or User if private Message)
                    string msg = messageObject.SplitMessage[1].Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
                    string cmd = msg.Split(' ')[0];
                    string arg = "";

                    if (messageObject.Sender.Contains("!"))
                        user = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!")); //Nick of the Sender
                    else
                        user = messageObject.Sender;

                    user = Useful.RemoveUserMode(user);

                    userlist.MarkUserSeen(user);

                    if (string.Compare(messageSource, Client.NICK, true) == 0)
                    {
                        //If its a user sending, check if it has mirror mode

                        if (userlist.UserIsMirrored(user) && (userlist.UserIsOperator(user) || Settings.Default.enforceMirrorOff == false)) //If enforce off is true and user is operator, let it bypass the enforce
                            messageSource = Client.HOME_CHANNEL;
                        else
                            messageSource = user;
                    }

                    if (msg.Length - 1 > cmd.Length)
                        arg = msg.Substring(cmd.Length + 1); //the rest of msg

                    //Write Message on Console
                    if (msg.Contains("\x01" + "ACTION "))
                    {
                        msg = msg.Replace("\x01" + "ACTION ", string.Empty).Replace("\x01", string.Empty);

                        if (string.Compare(messageSource, Client.NICK, true) == 0)
                            WriteMessage("             * : " + user + " " + msg, currentColorScheme.Notice);
                        else if (msg.ToLower().Contains(Client.NICK.ToLower()))
                            WriteMessage("             * : " + user + " " + msg, currentColorScheme.Mention);
                        else
                            WriteMessage("             * : " + user + " " + msg);
                    }
                    else
                    {
                        if (string.Compare(messageSource, Client.NICK, true) == 0)
                        {
                            string alignedNick = user.Truncate(13);
                            int tab = 15 - alignedNick.Length;

                            for (int i = 0; i < tab; i++)
                                alignedNick = alignedNick + " ";
                            WriteMessage(alignedNick + ": " + msg, currentColorScheme.Notice);
                        }
                        else if (msg.ToLower().Contains(Client.NICK.ToLower()))
                        {
                            string alignedNick = user.Truncate(13);
                            int tab = 15 - alignedNick.Length;

                            for (int i = 0; i < tab; i++)
                                alignedNick = alignedNick + " ";
                            WriteMessage(alignedNick + ": " + msg, currentColorScheme.Mention);
                        }
                        else
                        {
                            string alignedNick = user.Truncate(13);
                            int tab = 15 - alignedNick.Length;

                            for (int i = 0; i < tab; i++)
                                alignedNick = alignedNick + " ";
                            WriteMessage(alignedNick + ": " + msg);
                        }
                    }

                    //StartParsing
                    if ((string.Compare(cmd.Replace(",", string.Empty), "hello", true) == 0
                            || string.Compare(cmd.Replace(",", string.Empty), "hi", true) == 0
                            || string.Compare(cmd.Replace(",", string.Empty), "hey", true) == 0)
                            && arg.ToLower().Contains(Client.NICK.ToLower()))
                    {
                        WriteMessage("* Received a hello from " + user, currentColorScheme.BotReport);
                        Hello(messageSource, user);
                    }
                    else if (string.Compare(cmd, Client.NICK + ",", true) == 0 && !string.IsNullOrWhiteSpace(arg))
                    {
                        WriteMessage("* Received a Question from " + user, currentColorScheme.BotReport);
                        ParseQuestion(messageSource, user, arg);
                    }
                    else if ((string.Compare(cmd, "gg", true) == 0 && !string.IsNullOrWhiteSpace(arg) && string.Compare(arg.Split(' ')[0], "ez", true) == 0) || string.Compare(cmd, "ggez", true) == 0)
                    {
                        WriteMessage("* Received a gg ez from " + user, currentColorScheme.BotReport);
                        DoGgEz(messageSource, user);
                    }
                    else if (cmd[0] == Client.SYMBOL)   //Bot Command
                    {
                        cmd = cmd.Substring(1);

                        if (string.Compare(msg, "!anime best anime ever", true) == 0)
                        {
                            SendMessage(new Privmsg(messageSource, "Code Geass: Hangyaku no Lelouch : [Finished Airing] [25 episodes] [8,86 / 10] -> http://myanimelist.net/anime/1575/Code_Geass:_Hangyaku_no_Lelouch"));
                        }
                        else if (string.Compare(cmd, "help", true) == 0)
                        {
                            WriteMessage("* Received a Help request from " + user, currentColorScheme.BotReport);
                            Help(user);
                        }
                        else if (string.Compare(cmd, "mirror", true) == 0)
                        {
                            WriteMessage("* Received a mirror toogle request from " + user, currentColorScheme.BotReport);
                            ToogleMirror(user);
                        }
                        else if (string.Compare(cmd, "seen", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a seen toogle request from " + user, currentColorScheme.BotReport);
                            CheckSeen(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "enforcemirror", true) == 0)
                        {
                            WriteMessage("* Received an enforce mirror off toogle request from " + user, currentColorScheme.BotReport);
                            ToogleEnforceOff(user);
                        }
                        else if (string.Compare(cmd, "rules", true) == 0)
                        {
                            WriteMessage("* Received a Rules request from " + user, currentColorScheme.BotReport);
                            Rules(messageSource, user);
                        }
                        else if (string.Compare(cmd, "quit", true) == 0)
                        {
                            WriteMessage("* Received a quit request from " + user, currentColorScheme.BotReport);
                            QuitIRC(user);
                        }
                        else if (string.Compare(cmd, "oplist", true) == 0)
                        {
                            WriteMessage("* Received a oplist request from " + user, currentColorScheme.BotReport);
                            OpList(user);
                        }
                        else if (string.Compare(cmd, "roll", true) == 0)
                        {
                            WriteMessage("* Received a Roll request from " + user, currentColorScheme.BotReport);
                            Roll(messageSource, user);
                        }
                        else if (string.Compare(cmd, "say", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a say request from " + user, currentColorScheme.BotReport);
                            Say(Client.HOME_CHANNEL, arg, user);
                        }
                        else if (string.Compare(cmd, "greetme", true) == 0)
                        {
                            if (string.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a Greet TOOGLE request from " + user, currentColorScheme.BotReport);
                                GreetToogle(user);
                            }
                            else
                            {
                                WriteMessage("* Received a Greet request from " + user, currentColorScheme.BotReport);
                                AddGreetings(arg, user);
                            }
                        }
                        else if (string.Compare(cmd, "greetmenow", true) == 0)
                        {
                            WriteMessage("* Received a Greet me now request from " + user, currentColorScheme.BotReport);
                            GreetUser(user);
                        }
                        else if (string.Compare(cmd, "me", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a me request from " + user, currentColorScheme.BotReport);
                            Me(Client.HOME_CHANNEL, arg, user);
                        }
                        else if (string.Compare(cmd, "silence", true) == 0)
                        {
                            WriteMessage("* Received a silence request from " + user, currentColorScheme.BotReport);
                            Silence(user);
                        }
                        else if (string.Compare(cmd, "rename", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a Rename request from " + user, currentColorScheme.BotReport);
                            if (userlist.UserIsOperator(user)) ChangeNick(arg);
                        }
                        else if (string.Compare(cmd, "op", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a op request from " + user, currentColorScheme.BotReport);
                            AddBotOP(user, arg);
                        }
                        else if (string.Compare(cmd, "deop", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a deop request from " + user, currentColorScheme.BotReport);
                            RemoveBotOP(user, arg);
                        }
                        else if (string.Compare(cmd, "mute", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a mute request from " + user, currentColorScheme.BotReport);
                            MuteUser(user, arg);
                        }
                        else if (string.Compare(cmd, "unmute", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a unmute request from " + user, currentColorScheme.BotReport);
                            UnmuteUser(user, arg);
                        }
                        else if (string.Compare(cmd, "toF", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a temp. conversion to F request from " + user, currentColorScheme.BotReport);
                            ToFahrenheit(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "toC", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a temp. conversion to C request from " + user, currentColorScheme.BotReport);
                            ToCelcius(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "time", true) == 0)
                        {
                            WriteMessage("* Received a Time request from " + user, currentColorScheme.BotReport);
                            Time(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "wiki", true) == 0)
                        {
                            WriteMessage("* Received a explain request from " + user, currentColorScheme.BotReport);
                            Wiki(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "anime", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a animeSearch request from " + user, currentColorScheme.BotReport);
                            AnimeSearch(messageSource, user, arg);
                        }
                        else if ((string.Compare(cmd, "youtube", true) == 0 || string.Compare(cmd, "yt", true) == 0) && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a youtubeSearch request from " + user, currentColorScheme.BotReport);
                            YoutubeSearch(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "poke", true) == 0)
                        {
                            WriteMessage("* Received a Poke request from " + user, currentColorScheme.BotReport);
                            Poke(messageSource, user);
                        }
                        else if (string.Compare(cmd, "trivia", true) == 0)
                        {
                            WriteMessage("* Received a Trivia request from " + user, currentColorScheme.BotReport);
                            Trivia(messageSource, user);
                        }
                        else if (string.Compare(cmd, "nick", true) == 0)
                        {
                            WriteMessage("* Received a nickname request from " + user, currentColorScheme.BotReport);
                            NickGen(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "kill", true) == 0)
                        {
                            WriteMessage("* Received a Kill request from " + user, currentColorScheme.BotReport);
                            KillUser(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "fact", true) == 0 || string.Compare(cmd, "facts", true) == 0)
                        {
                            WriteMessage("* Received a Fact request from " + user, currentColorScheme.BotReport);
                            FactUser(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "lastkill", true) == 0)
                        {
                            WriteMessage("* Received a lastkill request from " + user, currentColorScheme.BotReport);
                            LastKill(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "rkill", true) == 0)
                        {
                            WriteMessage("* Received a randomkill request from " + user, currentColorScheme.BotReport);
                            RandomKill(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "quote", true) == 0 || string.Compare(cmd, "q", true) == 0)
                        {
                            WriteMessage("* Received a Quote request from " + user, currentColorScheme.BotReport);

                            if (string.Compare(arg.ToLower().Split(new char[] { ' ' }, 2)[0], "add") == 0)  //add
                            {
                                AddQuote(arg, user);
                            }
                            else //lookup or random
                            {
                                PrintQuote(messageSource, arg, user);
                            }
                        }
                        else if ((string.Compare(cmd, "choose", true) == 0 || string.Compare(cmd, "c", true) == 0) && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a Choose request from " + user, currentColorScheme.BotReport);
                            Choose(messageSource, user, arg);
                        }
                        else if ((string.Compare(cmd, "shuffle", true) == 0 || string.Compare(cmd, "s", true) == 0) && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a Shuffle request from " + user, currentColorScheme.BotReport);
                            Shuffle(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "funk", true) == 0 || string.Compare(cmd, "f", true) == 0)
                        {
                            WriteMessage("* Received a Funk request from " + user, currentColorScheme.BotReport);

                            if (string.IsNullOrEmpty(arg)) //lookup or random
                                PrintFunk(messageSource, user);
                            else
                                AddFunk(arg, user);
                        }
                        else if (string.Compare(cmd, "reload", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a reload request from " + user, currentColorScheme.BotReport);
                            ReloadTexts(user, arg);
                        }
                        else if (string.Compare(cmd, "names", true) == 0)
                        {
                            WriteMessage("* Received a names request from " + user, currentColorScheme.BotReport);
                            PrintNames(user);
                        }
                        else if (string.Compare(cmd, "stats", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a stats request from " + user, currentColorScheme.BotReport);
                            StatsPrint(messageSource, user, arg);
                        }
                        else if (string.Compare(cmd, "set", true) == 0 && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a set options request from " + user, currentColorScheme.BotReport);
                            SetSetting(user, arg);
                        }
                        else if (string.Compare(cmd, "acknowledge", true) == 0 || string.Compare(cmd, "a", true) == 0)
                        {
                            WriteMessage("* Received a acknowledge request from " + user, currentColorScheme.BotReport);

                            if (string.IsNullOrEmpty(arg))
                                userlist.ClearUserMessages(user);
                            else
                                userlist.ClearUserMessages(user, arg);
                        }
                        else if ((string.Compare(cmd, "tell", true) == 0 || string.Compare(cmd, "message", true) == 0) && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a Tell request from " + user, currentColorScheme.BotReport);
                            Tell(user, arg);
                        }
                        else if ((string.Compare(cmd, "addcmd", true) == 0) && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received an Add Custom Command request from " + user, currentColorScheme.BotReport);
                            AddCustomCommand(messageSource, arg, user);
                        }
                        else if ((string.Compare(cmd, "removecmd", true) == 0) && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a Remove Custom Command request from " + user, currentColorScheme.BotReport);
                            RemoveCustomCommand(arg, user);
                        }
                        else if (((string.Compare(cmd, "inspectmessages", true) == 0) || (string.Compare(cmd, "viewmessages", true) == 0)) && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received an Inspect Messages request from " + user, currentColorScheme.BotReport);
                            InspectMessages(arg, user);
                        }
                        else if (((string.Compare(cmd, "cleanmessages", true) == 0) || (string.Compare(cmd, "removemessages", true) == 0)) && !string.IsNullOrEmpty(arg))
                        {
                            WriteMessage("* Received a Clean Messages request from " + user, currentColorScheme.BotReport);
                            CleanMessages(arg, user);
                        }
                        else if ((string.Compare(cmd, "filmot", true) == 0))
                        {
                            WriteMessage("* Received a Filmot Convert request from " + user, currentColorScheme.BotReport);
                            Filmot(messageSource, arg, user);
                        }
                        else
                        {
                            //check for custom commands
                            UseCustomCommand(messageSource, cmd, arg, user);
                        }
                    }
                    else if ((msg.ToLower().Contains("youtu.be") && (msg.ToLower().Contains("?v=") == false && msg.ToLower().Contains("&v=") == false))
                        || (msg.ToLower().Contains("youtube") && msg.ToLower().Contains("watch") && (msg.ToLower().Contains("?v=") || msg.ToLower().Contains("&v="))))
                    {
                        WriteMessage("* Detected a Youtube video from " + user, currentColorScheme.BotReport);
                        Youtube(messageSource, user, msg);
                    }
                    else if (msg.Contains("vimeo.com"))
                    {
                        WriteMessage("* Detected an vimeo video from " + user, currentColorScheme.BotReport);
                        Vimeo(messageSource, user, msg);
                    }
                    else if (msg.Contains("reddit.com") && msg.Contains("/r/") && msg.Contains("/comments/"))
                    {
                        WriteMessage("* Detected a reddit link from " + user, currentColorScheme.BotReport);
                        RedditLink(messageSource, user, msg);
                    }
                    else if (msg.Contains("twitter.com") && msg.Contains("/status/"))
                    {
                        WriteMessage("* Detected a twitter link from " + user, currentColorScheme.BotReport);
                        Twitter(messageSource, user, msg);
                    }
                    else if (msg.Contains("http://") || msg.Contains("https://"))
                    {
                        WriteMessage("* Detected an url from " + user, currentColorScheme.BotReport);
                        UrlTitle(messageSource, user, msg);
                    }
                    else if (msg.TrimEnd('?').EndsWith(Client.NICK, true, CultureInfo.CurrentCulture))
                    {
                        WriteMessage("* Detected a think message from " + user, currentColorScheme.BotReport);
                        BotThink(messageSource, msg, user);
                    }
                    else if (message.Contains("\x01"))
                    {
                        if (cmd.Contains("VERSION"))
                        {
                            WriteMessage("* Received a CTCP version request from " + user, currentColorScheme.BotReport);
                            CtcpVersion(user);
                        }
                        else if (cmd.Contains("TIME"))
                        {
                            WriteMessage("* Received a CTCP Time request from " + user, currentColorScheme.BotReport);
                            CtcpTime(user);
                        }
                        else if (cmd.Contains("PING"))
                        {
                            WriteMessage("* Received a CTCP ping request from " + user, currentColorScheme.BotReport);
                            CtcpPing(user, arg);
                        }
                    }
                    if (msg.Contains("imgur.com"))
                    {
                        UpdateLastImgurLink(msg);
                    }

                    break;

                case ("NOTICE"):
                    if (message.Contains("\x01"))
                    {
                        string userN;
                        string argg;
                        string msgN = messageObject.SplitMessage[1].Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
                        string cmdN = msgN.Split(' ')[0];

                        if (messageObject.Sender.Contains('!'))
                            userN = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!"));   //Nick of the Sender
                        else userN = messageObject.Sender;

                        if (msgN.Length - 1 > cmdN.Length)
                            argg = msgN.Substring(cmdN.Length);                 //the rest of msg
                        else argg = "";

                        if (cmdN.Contains("VERSION"))
                        {
                            WriteMessage("* Received a CTCP version request from " + userN, currentColorScheme.BotReport);
                            CtcpVersion(userN);
                        }
                        else if (cmdN.Contains("TIME"))
                        {
                            WriteMessage("* Received a CTCP Time request from " + userN, currentColorScheme.BotReport);
                            CtcpTime(userN);
                        }
                        else if (cmdN.Contains("PING"))
                        {
                            WriteMessage("* Received a CTCP ping request from " + userN, currentColorScheme.BotReport);

                            CtcpPing(userN, argg);
                        }
                    }
                    else
                    {
                        string alignedNick = messageObject.Sender;

                        if (!string.IsNullOrWhiteSpace(alignedNick))
                        {
                            try
                            {
                                if (messageObject.Sender.Contains("!"))
                                    alignedNick = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!"));  //Nick of the Sender
                            }
                            catch { }
                            finally
                            {
                                alignedNick = alignedNick.Truncate(13);
                            }

                            int tab = 15 - alignedNick.Length;

                            for (int i = 0; i < tab; i++)
                                alignedNick = alignedNick + " ";
                            WriteMessage(alignedNick + ": " + messageObject.SplitMessage[1], currentColorScheme.Notice);
                        }
                    }

                    break;

                default:
                    WriteMessage("* " + messageObject.Type + " " + messageObject.CompleteMessage);
                    break;
            }
        }
        private bool QuitIRC(string nick)
        {
            if (userlist.UserIsOperator(nick))
            {
                OnQuit(EventArgs.Empty);

                timeoutTimer.Stop();
                waitingForPong = false;
                return true;
            }
            return false;
        }

        private void RandomKill(string CHANNEL, string nick, string args)
        {
            Random r = new Random();
            string target = "";
            string killString;

            IrcMessage message;

            List<User> listU = userlist.GetAllOnlineUsers();

            if (userlist.UserIsMuted(nick) || string.IsNullOrEmpty(nick)) return;

            var regex = new Regex(Regex.Escape("<random>"));

            if (Settings.Default.silence == false && Settings.Default.killEnabled == true)
            {
                if (string.IsNullOrWhiteSpace(args) || args.ToLower() == "random")
                    target = listU[r.Next((listU.Count))].Nick;
                else
                    target = args.Trim();

                if (!StringLib.Killgen.readyToGenerate())
                {
                    message = new Privmsg(CHANNEL, "Sorry, i can't think of a random kill right now.");
                }
                else
                {
                    killString = StringLib.Killgen.generateSentence();

                    killString = Useful.FillTags(killString, nick.Trim(), target, userlist);

                    if (killString.ToLower().Contains("<normal>"))
                    {
                        killString = killString.Replace("<normal>", string.Empty).Replace("<NORMAL>", string.Empty);
                        message = new Privmsg(CHANNEL, killString);
                    }
                    else
                        message = new ActionMessage(CHANNEL, killString);

                    message.body = message.body.Replace("  ", " ");
                }

                SendMessage(message);
                stats.kill();
            }
        }

        private void RedditInfo(string CHANNEL, string url, string postName)
        {
            IrcMessage message;

            try
            {
                RedditSharp.Things.Post post;
                post = reddit.GetPost(new Uri("https://www.reddit.com/" + postName));

                message = new Privmsg(CHANNEL, "\x02" + "[/r/" + post.SubredditName + "] " + "[" + "↑" + +post.Upvotes + "] " + "\x02" + HttpUtility.HtmlDecode(post.Title) + "\x02" + ", submitted by /u/" + post.Author + "\x02");
                SendMessage(message);

                if (!post.IsSelfPost)
                    SendMessage(new Privmsg(CHANNEL, "\x033" + post.Url + "\x03"));
            }
            catch
            {
                string subreddit = Useful.GetBetween(url, "/r/", "/");
                if (string.IsNullOrWhiteSpace(subreddit)) subreddit = "?";

                message = new Privmsg(CHANNEL, "\x02" + "[/r/" + subreddit.Trim() + "] " + "Failed to get post info" + "\x02");
                SendMessage(message);
            }
        }

        private void RedditInfoWithComment(string CHANNEL, string url, string postName, string commentName)
        {
            RedditSharp.Things.Post post;
            RedditSharp.Things.Comment comment;

            IrcMessage message;

            string postID = postName;
            string commentID = commentName;

            string subreddit = Useful.GetBetween(url, "/r/", "/");

            if (url.Contains('?'))
                url = url.Substring(0, url.IndexOf('?'));

            try
            {
                post = reddit.GetPost(new Uri("https://" + url));

                message = new Privmsg(CHANNEL, "\x02" + "[/r/" + subreddit + "] " + "[" + "↑" + post.Upvotes + "] " + "\x02" + HttpUtility.HtmlDecode(post.Title) + "\x02" + ", submitted by /u/" + post.Author + "\x02");
                SendMessage(message);
            }
            catch
            {
                message = new Privmsg(CHANNEL, "\x02" + "[/r/" + subreddit.Trim() + "] " + "Failed to get post info" + "\x02");
                SendMessage(message);
            }

            try
            {
                comment = reddit.GetComment(new Uri("https://" + url));

                if (comment.Body.ToString().Length > 300)
                    message = new Privmsg(CHANNEL, "\x02" + "Comment by " + comment.AuthorName + " [↑" + comment.Upvotes + "] " + HttpUtility.HtmlDecode(comment.Body.Truncate(300).Replace("\r", " ").Replace("\n", " ") + "(...)" + "\x02"));
                else
                    message = new Privmsg(CHANNEL, "\x02" + "Comment by " + comment.AuthorName + " [↑" + comment.Upvotes + "] " + HttpUtility.HtmlDecode(comment.Body.Replace("\r", " ").Replace("\n", " ") + "\x02"));
                SendMessage(message);
            }
            catch
            {
                message = new Privmsg(CHANNEL, "\x02" + "[/r/" + subreddit.Trim() + "] " + "Failed to get comment info" + "\x02");
                SendMessage(message);
            }
        }

        private void RedditLink(string CHANNEL, string nick, string line)
        {
            if (string.IsNullOrEmpty(line) || string.IsNullOrEmpty(nick)) return;

            string[] temp = line.Split(' ');
            string subreddit;
            string url;

            if (userlist.UserIsMuted(nick)) return;

            foreach (string st in temp)
            {
                if (st.Contains("reddit.com") && st.Contains("/r/") && st.Contains("/comments/"))
                {
                    url = st;
                    url = url.Replace("http://", string.Empty).Replace("https://", string.Empty);

                    subreddit = Useful.GetBetween(url, "/r/", "/");

                    string[] linkParse = url.Replace("\r", string.Empty).Split(new char[] { '/' }, 7);

                    if (linkParse.Length >= 7 && !string.IsNullOrEmpty(linkParse[6]) && !linkParse[6].StartsWith("?"))   //With Comment
                    {
                        if (linkParse[6].Contains('/')) { 
                            linkParse[6] = linkParse[6].Substring(0,linkParse[6].LastIndexOf('/'));
                            url = url.Substring(0, url.LastIndexOf('/'));
                        }

                        RedditInfoWithComment(CHANNEL, url, linkParse[4], linkParse[6]);
                    }
                    else  //No comment link
                    {
                        RedditInfo(CHANNEL, url, linkParse[4]);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a Nick Name to the Bot Operator list
        /// </summary>
        /// <param Name="Nick">the User that called the command</param>
        /// <param Name="targetUser">the User to be made bot operator</param>
        private void ReloadTexts(string user, string arg)
        {
            if (!userlist.UserIsOperator(user)) return;

            string[] options = arg.ToLower().Trim().Split(new char[] { ' ' });

            foreach (string option in options)
            {
                StringLib.ReloadLibrary(option);
            }
        }

        /// <summary>
        /// Removes a Nick Name to the Bot Operator list
        /// </summary>
        /// <param Name="Nick">the User that called the command</param>
        /// <param Name="targetUser">the User to be removed from the bot operator list</param>
        private bool RemoveBotOP(string nick, string targetUser)
        {
            IrcMessage message;

            targetUser = targetUser.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (!userlist.UserIsOperator(nick))
                return false;

            TakeOps(targetUser);

            message = new Notice(nick, targetUser + " was removed as a bot operator!");
            SendMessage(message);
            return true;
        }

        private void RemoveCustomCommand(string args, string nick)
        {
            string[] splits;

            splits = args.Split(new char[] { ' ' }, 2);

            if (userlist.UserIsOperator(nick))
            {
                CustomCommand.RemoveCommandByName(args, StringLib.CustomCommands);
                CustomCommand.saveCustomCommands(StringLib.CustomCommands);
            }
        }

        private void Roll(string CHANNEL, string nick)
        {
            if (userlist.UserIsMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.roll_Enabled == true)
            {
                Random random = new Random();
                int number = random.Next(0, 100);

                nick = nick.Replace("\r", "");
                IrcMessage message = new Privmsg(CHANNEL, nick + " rolled a " + number);
                SendMessage(message);
                stats.roll();
            }
        }

        private void Rules(string CHANNEL, string nick)
        {
            IrcMessage message;
            if (userlist.UserIsMuted(nick)) return;

            if (Settings.Default.silence == true && Settings.Default.rules_Enabled == true)
            {
                if (userlist.UserIsOperator(nick))
                {
                    foreach (string h in StringLib.Rules)
                    {
                        if (!string.IsNullOrWhiteSpace(h))
                        {
                            message = new Privmsg(CHANNEL, h.Replace("\n", "").Replace("\r", ""));
                            SendMessage(message);
                        }
                    }
                    stats.rules();
                    return;
                }
            }
            else if (Settings.Default.rules_Enabled == true)
            {
                foreach (string h in StringLib.Rules)
                {
                    if (!string.IsNullOrWhiteSpace(h))
                    {
                        message = new Privmsg(CHANNEL, h.Replace("\n", "").Replace("\r", ""));
                        SendMessage(message);
                    }
                }
                stats.rules();
                return;
            }
        }

        private void Say(string CHANNEL, string args, string nick)
        {
            if (userlist.UserIsOperator(nick))
            {
                SendMessage(new Privmsg(CHANNEL, args));
            }
        }

        private List<string> SearchQuotes(string[] queries)
        {
            List<string> results = new List<string>();

            foreach (string quote in StringLib.Quotes)
            {
                bool add = true;
                foreach (string query in queries)
                {
                    if (!quote.ToLower().Contains(query))
                    {
                        add = false;
                    }
                }
                if (add)
                    results.Add(quote);
            }

            return results;
        }

        private void SetSetting(string user, string arg)
        {
            if (!userlist.UserIsOperator(user)) return;

            string[] options = arg.ToLower().Trim().Split(new char[] { ' ' });
            bool status = false;

            if (options.Length < 2)
            {
                SendMessage(new Notice(user, "Not enought arguments"));
                return;
            }

            if (options[1] != "off")
                if (options[1] == "on")
                    status = true;
                else { SendMessage(new Notice(user, "Invalid Status")); return; }

            switch (options[0])
            {
                case "rules":
                    Settings.Default.rules_Enabled = status;
                    SendMessage(new Notice(user, "Rules is now " + (status ? "enabled" : "disabled")));
                    break;

                case "help":
                    Settings.Default.help_Enabled = status;
                    SendMessage(new Notice(user, "Help is now " + (status ? "enabled" : "disabled")));
                    break;

                case "time":
                    Settings.Default.timeEnabled = status;
                    SendMessage(new Notice(user, "Time is now " + (status ? "enabled" : "disabled")));
                    break;

                case "conversion":
                case "conversions":
                    Settings.Default.conversionEnabled = status;
                    SendMessage(new Notice(user, "Conversions is now " + (status ? "enabled" : "disabled")));
                    break;

                case "wiki":
                    Settings.Default.wikiEnabled = status;
                    SendMessage(new Notice(user, "Wiki is now " + (status ? "enabled" : "disabled")));
                    break;

                case "anime":
                    Settings.Default.aniSearchEnabled = status;
                    SendMessage(new Notice(user, "Anime Search is now " + (status ? "enabled" : "disabled")));
                    break;

                case "youtube":
                    Settings.Default.youtubeSearchEnabled = status;
                    SendMessage(new Notice(user, "Youtube Search is now " + (status ? "enabled" : "disabled")));
                    break;

                case "choose":
                    Settings.Default.chooseEnabled = status;
                    SendMessage(new Notice(user, "Choose is now " + (status ? "enabled" : "disabled")));
                    break;

                case "shuffle":
                    Settings.Default.shuffleEnabled = status;
                    SendMessage(new Notice(user, "Shuffle is now " + (status ? "enabled" : "disabled")));
                    break;

                case "roll":
                    Settings.Default.roll_Enabled = status;
                    SendMessage(new Notice(user, "Roll is now " + (status ? "enabled" : "disabled")));
                    break;

                case "hello":
                    Settings.Default.hello_Enabled = status;
                    SendMessage(new Notice(user, "Hello is now " + (status ? "enabled" : "disabled")));
                    break;

                case "nick":
                case "nicks":
                    Settings.Default.nickEnabled = status;
                    SendMessage(new Notice(user, "Nickname Generator is now " + (status ? "enabled" : "disabled")));
                    break;

                case "poke":
                    Settings.Default.pokeEnabled = status;
                    SendMessage(new Notice(user, "Poke is now " + (status ? "enabled" : "disabled")));
                    break;

                case "trivia":
                    Settings.Default.triviaEnabled = status;
                    SendMessage(new Notice(user, "Trivia is now " + (status ? "enabled" : "disabled")));
                    break;

                case "kill":
                case "kills":
                    Settings.Default.killEnabled = status;
                    SendMessage(new Notice(user, "Kill is now " + (status ? "enabled" : "disabled")));
                    break;

                case "fact":
                case "facts":
                    Settings.Default.factsEnabled = status;
                    SendMessage(new Notice(user, "Fact is now " + (status ? "enabled" : "disabled")));
                    break;

                case "questions":
                case "question":
                    Settings.Default.questionEnabled = status;
                    SendMessage(new Notice(user, "Questions are now " + (status ? "enabled" : "disabled")));
                    break;

                case "greetings":
                case "greet":
                case "greets":
                    Settings.Default.greetingsEnabled = status;
                    SendMessage(new Notice(user, "Greetings are now " + (status ? "enabled" : "disabled")));
                    break;

                case "quotes":
                case "quote":
                    Settings.Default.quotesEnabled = status;
                    SendMessage(new Notice(user, "Quotes are now " + (status ? "enabled" : "disabled")));
                    break;

                case "funk":
                    Settings.Default.funkEnabled = status;
                    SendMessage(new Notice(user, "Funk is now " + (status ? "enabled" : "disabled")));
                    break;

                case "reddit":
                case "reddittitle":
                    Settings.Default.redditEnabled = status;
                    SendMessage(new Notice(user, "Reddit Titles are now " + (status ? "enabled" : "disabled")));
                    break;

                case "vimeo":
                case "vimeotitle":
                    Settings.Default.vimeoEnabled = status;
                    SendMessage(new Notice(user, "Vimeo Titles are now " + (status ? "enabled" : "disabled")));
                    break;

                case "twitter":
                    Settings.Default.twitterEnabled = status;
                    SendMessage(new Notice(user, "Tweets are now " + (status ? "enabled" : "disabled")));
                    break;

                case "youtubetitle":
                    Settings.Default.youtube_Enabled = status;
                    SendMessage(new Notice(user, "Youtube Info is now " + (status ? "enabled" : "disabled")));
                    break;

                case "url":
                case "urltitle":
                    Settings.Default.urlTitleEnabled = status;
                    SendMessage(new Notice(user, "URL Info is now " + (status ? "enabled" : "disabled")));
                    break;

                case "tell":
                    Settings.Default.tellEnabled = status;
                    SendMessage(new Notice(user, "Tell is now " + (status ? "enabled" : "disabled")));
                    break;

                default: break;
            }
            Settings.Default.Save();
        }

        private void Shuffle(string CHANNEL, string user, string arg)
        {
            if (userlist.UserIsMuted(user) || !Settings.Default.shuffleEnabled) return;

            Random r = new Random();
            string message = "";
            string[] choices;
            List<string> sList = new List<string>();

            arg = arg.Replace("  ", " ");

            if (arg.Contains(','))
                choices = arg.Split(new char[] { ',' });
            else
                choices = arg.Split(new char[] { ' ' });

            foreach (string s in choices)
            {
                sList.Add(s);
            }

            if (sList.Count != 0)
            {
                while (sList.Count > 0)
                {
                    int random = r.Next(sList.Count);
                    message = message + " " + sList[random];
                    sList.Remove(sList[random]);
                }

                SendMessage(new Privmsg(CHANNEL, user + ":" + message));
            }

            stats.shuffle();
        }

        private void Silence(string nick)
        {
            IrcMessage message;
            if (userlist.UserIsOperator(nick))
            {
                if (Settings.Default.silence == true)
                {
                    OnUnsilence(EventArgs.Empty);
                    message = new Notice(nick, "The bot was unmuted");
                }
                else
                {
                    OnSilence(EventArgs.Empty);
                    message = new Notice(nick, "The bot was muted");
                }

                SendMessage(message);
                return;
            }
        }

        ///////////////////////////////////
        //       Commands Functions      //
        ///////////////////////////////////
        private void StatsPrint(string whoSent, string user, string arg)
        {
            if (!userlist.UserIsOperator(user)) return;

            string[] options = arg.ToLower().Trim().Split(new char[] { ' ' });

            foreach (string option in options)
            {
                switch (option)
                {
                    case "roll":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getRoll()[0] + " Lifetime: " + stats.getRoll()[1]));
                        break;

                    case "anime":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getAnime()[0] + " Lifetime: " + stats.getAnime()[1]));
                        break;

                    case "help":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getHelp()[0] + " Lifetime: " + stats.getHelp()[1]));
                        break;

                    case "rules":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getRules()[0] + " Lifetime: " + stats.getRules()[1]));
                        break;

                    case "greet":
                    case "greetings":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getGreet()[0] + " Lifetime: " + stats.getGreet()[1]));
                        break;

                    case "time":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getTime()[0] + " Lifetime: " + stats.getTime()[1]));
                        break;

                    case "temperature":
                    case "temperatures":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getTemperature()[0] + " Lifetime: " + stats.getTemperature()[1]));
                        break;

                    case "wiki":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getWiki()[0] + " Lifetime: " + stats.getWiki()[1]));
                        break;

                    case "poke":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getPoke()[0] + " Lifetime: " + stats.getPoke()[1]));
                        break;

                    case "trivia":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getTrivia()[0] + " Lifetime: " + stats.getTrivia()[1]));
                        break;

                    case "quote":
                    case "quotes":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getQuote()[0] + " Lifetime: " + stats.getQuote()[1]));
                        break;

                    case "choose":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getChoose()[0] + " Lifetime: " + stats.getChoose()[1]));
                        break;

                    case "shuffle":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getShuffle()[0] + " Lifetime: " + stats.getShuffle()[1]));
                        break;

                    case "funk":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getFunk()[0] + " Lifetime: " + stats.getFunk()[1]));
                        break;

                    case "nick":
                    case "nicks":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getNick()[0] + " Lifetime: " + stats.getNick()[1]));
                        break;

                    case "question":
                    case "questions":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getQuestion()[0] + " Lifetime: " + stats.getQuestion()[1]));
                        break;

                    case "youtube":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getYoutube()[0] + " Lifetime: " + stats.getYoutube()[1]));
                        break;

                    case "kill":
                    case "kills":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getKill()[0] + " Lifetime: " + stats.getKill()[1]));
                        break;

                    case "fact":
                    case "facts":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getFact()[0] + " Lifetime: " + stats.getFact()[1]));
                        break;

                    case "tell":
                        SendMessage(new Privmsg(whoSent, "Session: " + stats.getTell()[0] + " Lifetime: " + stats.getTell()[1]));
                        break;

                    default: break;
                }
            }
        }

        private void Tell(string nick, string args)
        {
            if (userlist.UserIsMuted(nick) || !Settings.Default.tellEnabled) return;

            string[] split = args.Split(new char[] { ' ' }, 2);

            if (split.Length < 2) return;
            else
            {
                if (userlist.UserMessageCount(split[0]) > Settings.Default.inboxSize)
                    SendMessage(new Privmsg(nick, split[0] + " has his inbox full! Can't accept more messages."));
                else
                {
                    userlist.AddUserMessage(split[0], nick, split[1]);
                    stats.tell();
                    userlist.SaveData();
                }
            }
        }

        private void Time(string CHANNEL, string nick, string args)
        {
            IrcMessage message;
            string timezoneS;
            string location = "";
            bool wantUTC = false;
            bool invalid = false;
            string requestURL;

            DateTime convertedTime;

            Google.GoogleTimeZone g = new Google.GoogleTimeZone();
            string json;

            if (userlist.UserIsMuted(nick)) return;

            args = args.Replace("\r", string.Empty);
            args = args.Replace("\n", string.Empty);
            args = args.Replace(" ", string.Empty);

            Dictionary<string, string> timezones = new Dictionary<string, string>
            {
                { "IST", "23.7833, 85.9667" },            //Bokaro, india
                { "MSK", "55.74941,37.614441" },          //Moscow
                { "FET", "53.895311,27.563324" },         //Minsk
                { "EET", "44.4476304,26.0860545" },       //bucharest
                { "CET", "48.8588589,2.3470599" },        //paris
                { "WER", "38.7436266,-9.1602038" },       //lisbon
                { "GMT", "51.5232391,-0.1166146" },       //london
                { "BRT", "-23.5778896,-46.6096585" },     //sao paulo
                { "ART", "-34.6158526,-58.4332985" },     //buenos aires
                { "AST", "53.3215407,-60.3542792" },      //Happy+Valley-Goose+Bay
                { "VET", "10.4683917,-66.8903658" },      //caracas
                { "EST", "40.7056308,-73.9780035" },      //NYC
                { "CST", "39.091919,-94.5757195" },       //kansas city
                { "MST", "40.7609881,-111.8936263" },     //salt lake city
                { "PST", "34.0469605,-118.2621293" },     //LA
                { "AKDT", "61.1878492,-149.8158133" }    //Anchorage
            };
            TimeZone localZone = TimeZone.CurrentTimeZone;
            DateTime currentDate = DateTime.Now;

            DateTime currentUTC = localZone.ToUniversalTime(currentDate);

            double timestamp = Useful.ConvertToUnixTimestamp(currentUTC);

            if (Settings.Default.silence == false && Settings.Default.timeEnabled == true)
            {
                switch (args.Replace("\r", string.Empty).ToLower())
                {
                    case "ist":
                        location = timezones["IST"]; timezoneS = "IST";
                        break;

                    case "msk":
                        location = timezones["MSK"]; timezoneS = "MSK";
                        break;

                    case "fet":
                        location = timezones["FET"]; timezoneS = "FET";
                        break;

                    case "eet":
                    case "stillbutterfly":
                    case "romania":
                        location = timezones["EET"]; timezoneS = "EET";
                        break;

                    case "cet":
                        location = timezones["CET"]; timezoneS = "CET";
                        break;

                    case "wer":
                        location = timezones["WER"]; timezoneS = "WER";
                        break;

                    case "gmt":
                    case "masterrace":
                        location = timezones["GMT"]; timezoneS = "GMT";
                        break;

                    case "utc":
                        wantUTC = true; timezoneS = "UTC";
                        break;

                    case "brt":
                        location = timezones["BRT"]; timezoneS = "BRT";
                        break;

                    case "art":
                        location = timezones["ART"]; timezoneS = "ART";
                        break;

                    case "ast":
                        location = timezones["AST"]; timezoneS = "AST";
                        break;

                    case "vet":
                        location = timezones["VET"]; timezoneS = "VET";
                        break;

                    case "est":
                        location = timezones["EST"]; timezoneS = "EST";
                        break;

                    case "cst":
                        location = timezones["CST"]; timezoneS = "CST";
                        break;

                    case "mst":
                    case "jhoudiey":
                        location = timezones["MST"]; timezoneS = "MST";
                        break;

                    case "pst":
                        location = timezones["PST"]; timezoneS = "PST";
                        break;

                    case "akdt":
                        location = timezones["AKDT"]; timezoneS = "AKDT";
                        break;

                    case "":
                        location = timezones["GMT"]; timezoneS = "GMT";
                        break;

                    default:
                        location = timezones["GMT"]; timezoneS = "GMT"; invalid = true;
                        break;
                }

                if (!wantUTC)
                {
                    requestURL = "https://maps.googleapis.com/maps/api/timezone/json?location=" + location + "&timestamp=" + timestamp + "&key=" + Settings.Default.apikey;
                    var webClient = new WebClient()
                    {
                        Encoding = Encoding.UTF8
                    };
                    try
                    {
                        json = webClient.DownloadString(requestURL);
                        JsonConvert.PopulateObject(json, g);
                    }
                    catch { }

                    convertedTime = Useful.ConvertFromUnixTimestamp(timestamp + g.DSTOffset + g.RawOffset);
                }
                else
                    convertedTime = currentUTC;

                if (invalid)
                    if (args.ToLower() == "2blaze" || args.ToLower() == "2blaze1" || args.ToLower() == "toblaze")
                        message = new Privmsg(CHANNEL, "4:20");
                    else if (args.ToLower() == "alan_jackson" || args.ToLower() == "alan" || args.ToLower() == "alanjackson")
                        message = new Privmsg(CHANNEL, "5:00");
                    else
                        message = new Privmsg(CHANNEL, convertedTime.Hour + ":" + convertedTime.Minute.ToString("00") + " " + timezoneS + ". \"" + args + "\" is an invalid argument");
                else
                    message = new Privmsg(CHANNEL, convertedTime.Hour + ":" + convertedTime.Minute.ToString("00") + " " + timezoneS);
                SendMessage(message);
                stats.time();
            }
        }

        private void ToCelcius(string CHANNEL, string nick, string args)
        {
            IrcMessage message;
            double c = 0;
            double f = 0;

            if (userlist.UserIsMuted(nick) || Settings.Default.silence || !Settings.Default.conversionEnabled) return;

            try
            {
                f = Convert.ToDouble(args);
                c = (5.0 / 9.0) * (f - 32);
            }
            catch
            {
                message = new Privmsg(CHANNEL, "Could not parse arguments");
                SendMessage(message);
                return;
            }

            message = new Privmsg(CHANNEL, f + " F is " + Math.Round(c, 2) + " C");
            SendMessage(message);
            stats.temperature();
            return;
        }

        private void ToFahrenheit(string CHANNEL, string nick, string args)
        {
            IrcMessage message;
            double f = 0;
            double c = 0;

            if (userlist.UserIsMuted(nick) || Settings.Default.silence || !Settings.Default.conversionEnabled) return;

            try
            {
                c = Convert.ToDouble(args);
                f = ((9.0 / 5.0) * c) + 32;
            }
            catch
            {
                message = new Privmsg(CHANNEL, "Could not parse arguments");
                SendMessage(message);
                return;
            }

            message = new Privmsg(CHANNEL, c + " C is " + Math.Round(f, 2) + " F");
            SendMessage(message);
            stats.temperature();
            return;
        }

        private void ToogleEnforceOff(string nick)
        {
            IrcMessage message;

            if (!userlist.UserIsOperator(nick)) return;

            Settings.Default.enforceMirrorOff = !Settings.Default.enforceMirrorOff;

            Settings.Default.Save();

            message = new Notice(nick, "Enforce Mirror Mode Off is now " + (Settings.Default.enforceMirrorOff ? "enabled" : "disabled"));
            SendMessage(message);

            OnEnforceMirrorChanged(EventArgs.Empty);
        }

        private void ToogleMirror(string nick)
        {
            IrcMessage message;
            bool mirror = false;

            mirror = userlist.ToogleMirror(nick);

            message = new Notice(nick, "MirrorMode is now " + (mirror ? "enabled" : "disabled"));
            SendMessage(message);
        }

        private void TopicChanged(ParsedMessage messageObject)
        {
            string topic;

            switch (messageObject.Type)
            {
                case ("332"):
                    topic = messageObject.CompleteMessage.Split(new char[] { ' ' }, 3)[2];
                    break;

                default:
                    topic = messageObject.CompleteMessage.Split(new char[] { ' ' }, 2)[1];
                    break;
            }

            OnTopicChange(new TopicChangedEventArgs(topic));
        }

        private void Trivia(string CHANNEL, string nick)
        {
            if (StringLib.Trivia == null || userlist.UserIsMuted(nick) || StringLib.Trivia.Count == 0 || Settings.Default.silence || !Settings.Default.triviaEnabled) return;

            Random rnd = new Random();

            IrcMessage message = new Privmsg(CHANNEL, StringLib.Trivia[rnd.Next(StringLib.Trivia.Count)]);
            SendMessage(message);
            stats.trivia();
            return;
        }

        private void Twitter(string CHANNEL, string nick, string line)
        {
            string author, tweet;

            if (userlist.UserIsMuted(nick)) return;

            if (Settings.Default.silence == true || Settings.Default.twitterEnabled == false) return;
            else
            {
                string ID = Useful.GetBetween(line, "/status/", "?");
                long tweetID = Convert.ToInt64(ID);

                try { 
                    TwitterStatus tweetResult = service.GetTweet(new GetTweetOptions { Id = tweetID,});

                    author = HttpUtility.HtmlDecode(tweetResult.Author.ScreenName);
                    tweet = HttpUtility.HtmlDecode(tweetResult.TextDecoded.Replace("\n", " "));

                    IrcMessage message = new Privmsg(CHANNEL, "Tweet by @" + author + ": " + tweet);

                    SendMessage(message);

                }

                catch
                {
                    SendMessage(new Privmsg(CHANNEL, "Error"));
                }
            }
        }

        private bool UnmuteUser(string nick, string targetUser)
        {
            IrcMessage message;

            targetUser = targetUser.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();

            if (!userlist.UserIsOperator(nick))
                return false;

            userlist.SetUserMuteStatus(targetUser, false);
            message = new Notice(nick, targetUser + " was unmuted!");
            SendMessage(message);
            return true;
        }

        private void UpdateLastImgurLink(string msg)
        {
            lastImgurID = Useful.GetBetween(msg, "imgur.com/", "");
        }

        private void UrlTitle(string CHANNEL, string nick, string line)
        {
            string url;
            string[] split;
            //Dictionary<string, string> headers = new Dictionary<string, string>();

            if (userlist.UserIsMuted(nick)) return;

            if (Settings.Default.silence == true || Settings.Default.urlTitleEnabled == false) return;
            else
            {
                split = line.Split(new char[] { ' ' });

                foreach (string s in split)
                {
                    if (s.Contains("http://") || s.Contains("https://"))
                    {
                        url = s;
                        if (!string.IsNullOrWhiteSpace(url))
                            GetURLInfo(CHANNEL, url);
                    }
                }
            }
        }

        private void UseCustomCommand(string CHANNEL, string cmd, string args, string nick)
        {
            IrcMessage message;
            string response;
            Random r = new Random();
            List<User> listU = userlist.GetAllOnlineUsers();
            CustomCommand customcommand;
            var regex = new Regex(Regex.Escape("<random>"));

            if (CustomCommand.commandExists(cmd, StringLib.CustomCommands) == false)
            {
                //message = new Privmsg(CHANNEL, "Command " + cmd + " doesn't exist.");
                //sendMessage(message);

                return;
            }

            customcommand = CustomCommand.getCustomCommandByName(cmd, StringLib.CustomCommands);

            if (customcommand == null) return;

            response = customcommand.Format;

            response = Useful.FillTags(response, nick.Trim(), args, userlist);

            message = new Privmsg(CHANNEL, response);
            SendMessage(message);
        }

        private string UserPart(ParsedMessage messageObject)
        {
            string quitMessage;
            string whoLeft;

            if (messageObject.Sender.Contains("!"))
                whoLeft = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!"));
            else whoLeft = messageObject.Sender;

            quitMessage = messageObject.CompleteMessage;

            userlist.SetUserOnlineStatus(whoLeft, false);

            UpdateUserListSource(null, null);
            WriteMessage("** " + whoLeft + " parted (" + quitMessage.Trim() + ")", currentColorScheme.Leave);
            return quitMessage;
        }
        private void Vimeo(string CHANNEL, string nick, string line)
        {
            if (userlist.UserIsMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.vimeoEnabled == true)
            {
                string title, duration;
                int hours = 0;
                int minutes = 0;
                int seconds = 0;
                int temp = 0;

                IrcMessage message;
                string ID = Useful.GetBetween(line, "vimeo.com/", "/");
                string URLString = "http://vimeo.com/api/v2/video/" + ID.Replace("\r", "").Replace("\n", "") + ".xml";

                var webClient = new WebClient()
                {
                    Encoding = Encoding.UTF8
                };
                string readHtml = webClient.DownloadString(URLString);

                title = Useful.GetBetween(readHtml, "<title>", "</title>");
                duration = Useful.GetBetween(readHtml, "<duration>", "</duration>");

                temp = Convert.ToInt32(duration);

                while (temp >= 60)
                {
                    minutes++;
                    temp = temp - 60;
                }
                seconds = temp;

                if (minutes >= 60)
                {
                    temp = minutes;
                    while (temp >= 60)
                    {
                        hours++;
                        temp = temp - 60;
                    }
                    minutes = temp;
                    message = new Privmsg(CHANNEL, "\x02" + "Vimeo Video: " + title + " [" + hours + ":" + minutes.ToString("00") + ":" + seconds.ToString("00") + "]\x02");
                    SendMessage(message);
                }
                else
                {
                    message = new Privmsg(CHANNEL, "\x02" + "Vimeo Video: " + title + " [" + minutes + ":" + seconds.ToString("00") + "]\x02");
                    SendMessage(message);
                }
            }
        }

        private void Wiki(string CHANNEL, string nick, string args)
        {
            if (userlist.UserIsMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.wikiEnabled == true)
            {
                IrcMessage message = new Privmsg(CHANNEL, "Here's a Wiki for \"" + args + "\": " + "http://en.wikipedia.org/w/index.php?title=Special:Search&search=" + args.Replace(" ", "%20"));
                SendMessage(message);
                stats.wiki();
            }
        }

        /// <summary>
        /// Writes a Message on the output window
        /// </summary>
        /// <param Name="Message">A sting with the Message to write</param>
        private void WriteMessage(string message) //Writes Message on the TextBox (bot console)
        {
            string timeString = DateTime.Now.ToString("[HH:mm:ss]");

            if (OutputBox != null)
            {
                if (OutputBox.InvokeRequired)
                {
                    try
                    {
                        MethodInvoker invoker = () => WriteMessage(message);
                        OutputBox.Invoke(invoker);
                    }
                    catch { }
                }
                else
                {
                    if (Settings.Default.showTimeStamps)
                        this.OutputBox.AppendText(timeString + " " + message + "\n");
                    else
                        this.OutputBox.AppendText(message + "\n");

                    if (Settings.Default.autoScrollToBottom)
                    {
                        OutputBox.SelectionStart = OutputBox.Text.Length;       //Set the current caret position at the end
                        OutputBox.ScrollToCaret();                          //Now scroll it automatically
                    }
                }

                //also, should make a log
            }
            else
            {
                Console.WriteLine(timeString + " " + message);
            }
        }

        private void WriteMessage(string message, Color color)           //Writes Message on the TextBox (bot console)
        {
            string timeString = DateTime.Now.ToString("[HH:mm:ss]");

            if (OutputBox != null)
            {
                if (OutputBox.InvokeRequired)
                {
                    try
                    {
                        MethodInvoker invoker = () => WriteMessage(message, color);
                        OutputBox.Invoke(invoker);
                    }
                    catch { }
                }
                else
                {
                    if (Settings.Default.showTimeStamps)
                    {
                        this.OutputBox.AppendText(timeString + " ");
                        this.OutputBox.AppendText(message + "\n", color);
                    }
                    else
                        this.OutputBox.AppendText(message + "\n", color);

                    if (Settings.Default.autoScrollToBottom)
                    {
                        OutputBox.SelectionStart = OutputBox.Text.Length;       //Set the current caret position at the end
                        OutputBox.ScrollToCaret();                              //Now scroll it automatically
                    }
                }

                //also, should make a log
            }
            else
            {
                Console.WriteLine(timeString + " " + message);
            }
        }

        private void Youtube(string CHANNEL, string nick, string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            if (userlist.UserIsMuted(nick)) return;

            if (!Settings.Default.silence && Settings.Default.youtube_Enabled)
            {
                string id = YoutubeUseful.getYoutubeIdFromURL(line);

                string result = YoutubeUseful.getYoutubeInfoFromID(id);

                IrcMessage message = new Privmsg(CHANNEL, result);
                SendMessage(message);
            }
        }
        private void YoutubeSearch(string CHANNEL, string nick, string query)
        {
            IrcMessage message;
            string jsonYoutube, title, duration;
            Youtube.YoutubeSearch y = new Youtube.YoutubeSearch();
            Youtube.YoutubeVideoInfo youtubeVideo = new Youtube.YoutubeVideoInfo();

            if (userlist.UserIsMuted(nick)) return;
            if (Settings.Default.silence == true || Settings.Default.youtubeSearchEnabled == false) return;
            if (string.IsNullOrWhiteSpace(query)) return;

            //query = query.Replace(" ", "%20");

            string getString = "https://www.googleapis.com/youtube/v3/search" + "?key=" + Settings.Default.apikey + "&part=id,snippet" + "&q=" +
                HttpUtility.UrlEncode(query) + "&maxresults=10&type=video&safeSearch=none";

            var webClient = new WebClient()
            {
                Encoding = Encoding.UTF8
            };
            webClient.Headers.Add("User-Agent", Settings.Default.UserAgent);

            try
            {
                jsonYoutube = webClient.DownloadString(getString);
                JsonConvert.PopulateObject(jsonYoutube, y);
            }
            catch { }

            foreach (var searchResult in y.items)
            {
                switch (searchResult.id.kind.ToLower())
                {
                    case "youtube#video":

                        getString = "https://www.googleapis.com/youtube/v3/videos/" + "?key=" + Settings.Default.apikey + "&part=snippet,contentDetails,statistics" + "&id=" + searchResult.id.videoId;
                        try
                        {
                            jsonYoutube = webClient.DownloadString(getString);
                            JsonConvert.PopulateObject(jsonYoutube, youtubeVideo);

                            title = WebUtility.HtmlDecode(youtubeVideo.items[0].snippet.title);
                            duration = YoutubeUseful.parseDuration(youtubeVideo.items[0].contentDetails.duration);

                            message = new Privmsg(CHANNEL, "\x02" + "\x031,0You" + "\x030,4Tube" + "\x03 Video: " + title + " [" + duration + "]\x02" + ": https://www.youtube.com/watch?v=" + searchResult.id.videoId);
                            SendMessage(message);
                            return;//Only shows 1 link
                        }
                        catch { }

                        break;
                }
            }
            message = new Privmsg(CHANNEL, "No results found");
            SendMessage(message);
            stats.youtube();
            return;
        }
    }
}