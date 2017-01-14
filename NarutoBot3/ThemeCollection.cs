using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace NarutoBot3
{
    public class ThemeCollection
    {
        private List<ColorScheme> themeColection = new List<ColorScheme>();
        private ColorScheme currentColorScheme = new ColorScheme();

        public List<ColorScheme> ThemeColection
        {
            get
            {
                return themeColection;
            }

            set
            {
                themeColection = value;
            }
        }

        public ColorScheme CurrentColorScheme
        {
            get
            {
                return currentColorScheme;
            }

            set
            {
                currentColorScheme = value;
            }
        }

        public ThemeCollection()
        {
            loadThemes();
        }

        public bool themeExists(string name)
        {
            foreach (ColorScheme c in ThemeColection)
            {
                if (string.Compare(c.Name, name, true) == 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public bool selectTheme(int index)
        {
            try
            {
                CurrentColorScheme = themeColection[index];
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool selectTheme(ColorScheme theme)
        {
            CurrentColorScheme = theme;

            if (!themeExists(theme.Name))
                themeColection.Add(theme);

            return true;
        }

        private void loadThemes()
        {
            ThemeColection.Clear();
            ThemeColection.Add(CurrentColorScheme);

            ColorScheme tmpScheme = new ColorScheme();

            string[] dirs = Directory.GetFiles(@"Theme", "*.json");

            foreach (string dir in dirs)
            {
                tmpScheme = new ColorScheme();

                TextReader stream = new StreamReader(dir);
                string json = stream.ReadToEnd();
                JsonConvert.PopulateObject(json, tmpScheme);

                stream.Close();
                if (!themeExists(tmpScheme.Name))
                    ThemeColection.Add(tmpScheme);
            }
        }
    }
}