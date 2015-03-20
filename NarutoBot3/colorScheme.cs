using System.Drawing;

namespace NarutoBot3
{
    public class ColorScheme
    {
        private string name = "Cool";

        private Color join = ColorTranslator.FromHtml("#8C8A8A");
        private Color leave = ColorTranslator.FromHtml("#D9A641");
        private Color rename = ColorTranslator.FromHtml("#4545E6");
        private Color statusChanged = ColorTranslator.FromHtml("#185555");
        private Color notice = ColorTranslator.FromHtml("#1E90FF");
        private Color mention = ColorTranslator.FromHtml("#2A8C2A");
        private Color botReport = ColorTranslator.FromHtml("#FFC0CB");
        private Color motd = ColorTranslator.FromHtml("#B955D3");
        private Color ownMessage = ColorTranslator.FromHtml("#FFFFFF");



        private Color mainWindowBG = ColorTranslator.FromHtml("#333333");
        private Color mainWindowText = ColorTranslator.FromHtml("#C7C7C7");
        private Color userListBG = ColorTranslator.FromHtml("#333333");
        private Color userListText = ColorTranslator.FromHtml("#C7C7C7");
        private Color inputBG = ColorTranslator.FromHtml("#333333");
        private Color inputText = ColorTranslator.FromHtml("#C7C7C7");
        private Color topicBG = ColorTranslator.FromHtml("#333333");
        private Color topicText = ColorTranslator.FromHtml("#C7C7C7");


        public Color OwnMessage
        {
            get { return ownMessage; }
            set { ownMessage = value; }
        }
        public Color Leave
        {
            get { return leave;  }
            set { leave = value; }
        }
        public Color Rename
        {
            get { return rename; }
            set { rename = value; }
        }
        public Color StatusChanged
        {
            get { return statusChanged; }
            set { statusChanged = value; }
        }
        public Color Notice
        {
            get { return notice; }
            set { notice = value; }
        }
        public Color Mention
        {
            get { return mention; }
            set { mention = value; }
        }
        public Color BotReport
        {
            get { return botReport; }
            set { botReport = value; }
        }
        public Color Motd
        {
            get { return motd; }
            set { motd = value; }
        }
        public Color MainWindowBG
        {
            get { return mainWindowBG; }
            set { mainWindowBG = value; }
        }
        public Color MainWindowText
        {
            get { return mainWindowText; }
            set { mainWindowText = value; }
        }
        public Color UserListBG
        {
            get { return userListBG; }
            set { userListBG = value; }
        }
        public Color UserListText
        {
            get { return userListText; }
            set { userListText = value; }
        }
        public Color InputBG
        {
            get { return inputBG; }
            set { inputBG = value; }
        }
        public Color InputText
        {
            get { return inputText; }
            set { inputText = value; }
        }
        public Color Join
        {
            get { return join; }
            set { join = value; }
        }
        public Color TopicText
        {
            get { return topicText; }
            set { topicText = value; }
        }
        public Color TopicBG
        {
            get { return topicBG; }
            set { topicBG = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
