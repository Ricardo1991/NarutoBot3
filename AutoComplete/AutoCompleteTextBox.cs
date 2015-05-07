using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AutoComplete
{
    public class AutoCompleteTextBox : TextBox
    {
        private String[] _values;
        private String _formerValue = String.Empty;
        bool autoCompleting = false;

        String[] matches;
        int matchesIndex = 0;

        public AutoCompleteTextBox()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            KeyDown += this_KeyDown;
            KeyUp += this_KeyUp;
        }

        private void this_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateListBox();
        }

        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.Tab:
                    if (autoCompleting)
                    {
                        InsertWord((String)matches[matchesIndex]);
                        _formerValue = Text;

                        if (matchesIndex < matches.Length - 1)
                            matchesIndex++;
                        else
                            matchesIndex = 0;
                    }
                    break;

                default:
                    autoCompleting = false;
                    break;
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Tab:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }

        private void UpdateListBox()
        {
            if (Text == _formerValue) return;
            _formerValue = Text;
            String word = GetWord();

            if (_values != null && word.Length > 0)
            {
                matches = Array.FindAll(_values, x => (x.StartsWith(word, StringComparison.OrdinalIgnoreCase)));
                if (matches.Length > 0)
                {
                    autoCompleting = true;
                    matchesIndex = 0;

                }
            }
        }

        private String GetWord()
        {
            String text = Text;
            int pos = SelectionStart;

            int posStart = text.LastIndexOf(' ', (pos < 1) ? 0 : pos - 1);
            posStart = (posStart == -1) ? 0 : posStart + 1;
            int posEnd = text.IndexOf(' ', pos);
            posEnd = (posEnd == -1) ? text.Length : posEnd;

            int length = ((posEnd - posStart) < 0) ? 0 : posEnd - posStart;

            return text.Substring(posStart, length);
        }

        private void InsertWord(String newTag)
        {
            String text = Text;
            int pos = SelectionStart;

            int posStart = text.LastIndexOf(' ', (pos < 1) ? 0 : pos - 1);
            posStart = (posStart == -1) ? 0 : posStart + 1;
            int posEnd = text.IndexOf(' ', pos);

            String firstPart = text.Substring(0, posStart) + newTag;
            String updatedText = firstPart + ((posEnd == -1) ? "" : text.Substring(posEnd, text.Length - posEnd));


            Text = updatedText;
            SelectionStart = firstPart.Length;
        }

        public String[] Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
            }
        }

        public List<String> SelectedValues
        {
            get
            {
                String[] result = Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return new List<String>(result);
            }
        }

    }
}
