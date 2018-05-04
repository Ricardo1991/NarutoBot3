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
            LoadThemes();
        }

        public bool ThemeExists(string name)
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

        public bool SelectTheme(int index)
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

        /// <summary>
        /// Get a theme by name, returns the default theme if the specified theme was not found
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ColorScheme GetThemeByName(string name)
        {
            ColorScheme color = new ColorScheme();

            foreach (ColorScheme c in ThemeColection)
            {
                if (string.Compare(c.Name, name, true) == 0)
                    return c;
            }

            return color;
        }

        public bool SelectTheme(ColorScheme theme)
        {
            CurrentColorScheme = theme;

            if (!ThemeExists(theme.Name))
                themeColection.Add(theme);

            return true;
        }

        private void LoadThemes()
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
                if (!ThemeExists(tmpScheme.Name))
                    ThemeColection.Add(tmpScheme);
            }
        }
    }
}