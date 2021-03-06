﻿#pragma warning disable IDE1006 // Naming Styles

namespace MyAnimeList
{
    /// <remarks/>
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
    public partial class MalAnimeData
    {
        private animeEntry[] entryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElement("entry")]
        public animeEntry[] entry
        {
            get
            {
                return this.entryField;
            }
            set
            {
                this.entryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlType(AnonymousType = true)]
        public partial class animeEntry
        {
            private int idField;

            private string titleField;

            private string englishField;

            private string synonymsField;

            private int episodesField;

            private decimal scoreField;

            private string typeField;

            private string statusField;

            private string start_dateField;

            private string end_dateField;

            private string synopsisField;

            private string imageField;

            /// <remarks/>
            public int id
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }

            /// <remarks/>
            public string title
            {
                get
                {
                    return this.titleField;
                }
                set
                {
                    this.titleField = value;
                }
            }

            /// <remarks/>
            public string english
            {
                get
                {
                    return this.englishField;
                }
                set
                {
                    this.englishField = value;
                }
            }

            /// <remarks/>
            public string synonyms
            {
                get
                {
                    return this.synonymsField;
                }
                set
                {
                    this.synonymsField = value;
                }
            }

            /// <remarks/>
            public int episodes
            {
                get
                {
                    return this.episodesField;
                }
                set
                {
                    this.episodesField = value;
                }
            }

            /// <remarks/>
            public decimal score
            {
                get
                {
                    return this.scoreField;
                }
                set
                {
                    this.scoreField = value;
                }
            }

            /// <remarks/>
            public string type
            {
                get
                {
                    return this.typeField;
                }
                set
                {
                    this.typeField = value;
                }
            }

            /// <remarks/>
            public string status
            {
                get
                {
                    return this.statusField;
                }
                set
                {
                    this.statusField = value;
                }
            }

            /// <remarks/>
            public string start_date
            {
                get
                {
                    return this.start_dateField;
                }
                set
                {
                    this.start_dateField = value;
                }
            }

            /// <remarks/>
            public string end_date
            {
                get
                {
                    return this.end_dateField;
                }
                set
                {
                    this.end_dateField = value;
                }
            }

            /// <remarks/>
            public string synopsis
            {
                get
                {
                    return this.synopsisField;
                }
                set
                {
                    this.synopsisField = value;
                }
            }

            /// <remarks/>
            public string image
            {
                get
                {
                    return this.imageField;
                }
                set
                {
                    this.imageField = value;
                }
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
    public partial class MalUserData
    {
        private myanimelistMyinfo myinfoField;

        private myanimelistAnime[] animeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElement("myinfo")]
        public myanimelistMyinfo myinfo
        {
            get
            {
                return this.myinfoField;
            }
            set
            {
                this.myinfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement("anime")]
        public myanimelistAnime[] anime
        {
            get
            {
                return this.animeField;
            }
            set
            {
                this.animeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlType(AnonymousType = true)]
        public partial class myanimelistMyinfo
        {
            private uint user_idField;

            private string user_nameField;

            private int user_watchingField;

            private int user_completedField;

            private int user_onholdField;

            private int user_droppedField;

            private int user_plantowatchField;

            private float user_days_spent_watchingField;

            /// <remarks/>
            public uint user_id
            {
                get
                {
                    return this.user_idField;
                }
                set
                {
                    this.user_idField = value;
                }
            }

            /// <remarks/>
            public string user_name
            {
                get
                {
                    return this.user_nameField;
                }
                set
                {
                    this.user_nameField = value;
                }
            }

            /// <remarks/>
            public int user_watching
            {
                get
                {
                    return this.user_watchingField;
                }
                set
                {
                    this.user_watchingField = value;
                }
            }

            /// <remarks/>
            public int user_completed
            {
                get
                {
                    return this.user_completedField;
                }
                set
                {
                    this.user_completedField = value;
                }
            }

            /// <remarks/>
            public int user_onhold
            {
                get
                {
                    return this.user_onholdField;
                }
                set
                {
                    this.user_onholdField = value;
                }
            }

            /// <remarks/>
            public int user_dropped
            {
                get
                {
                    return this.user_droppedField;
                }
                set
                {
                    this.user_droppedField = value;
                }
            }

            /// <remarks/>
            public int user_plantowatch
            {
                get
                {
                    return this.user_plantowatchField;
                }
                set
                {
                    this.user_plantowatchField = value;
                }
            }

            /// <remarks/>
            public float user_days_spent_watching
            {
                get
                {
                    return this.user_days_spent_watchingField;
                }
                set
                {
                    this.user_days_spent_watchingField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlType(AnonymousType = true)]
        public partial class myanimelistAnime
        {
            private uint series_animedb_idField;

            private string series_titleField;

            private string series_synonymsField;

            private int series_typeField;

            private int series_episodesField;

            private int series_statusField;

            private string series_startField;

            private string series_endField;

            private string series_imageField;

            private int my_idField;

            private int my_watched_episodesField;

            private string my_start_dateField;

            private string my_finish_dateField;

            private int my_scoreField;

            private int my_statusField;

            private string my_rewatchingField;

            private int my_rewatching_epField;

            private uint my_last_updatedField;

            private string my_tagsField;

            /// <remarks/>
            public uint series_animedb_id
            {
                get
                {
                    return this.series_animedb_idField;
                }
                set
                {
                    this.series_animedb_idField = value;
                }
            }

            /// <remarks/>
            public string series_title
            {
                get
                {
                    return this.series_titleField;
                }
                set
                {
                    this.series_titleField = value;
                }
            }

            /// <remarks/>
            public string series_synonyms
            {
                get
                {
                    return this.series_synonymsField;
                }
                set
                {
                    this.series_synonymsField = value;
                }
            }

            /// <remarks/>
            public int series_type
            {
                get
                {
                    return this.series_typeField;
                }
                set
                {
                    this.series_typeField = value;
                }
            }

            /// <remarks/>
            public int series_episodes
            {
                get
                {
                    return this.series_episodesField;
                }
                set
                {
                    this.series_episodesField = value;
                }
            }

            /// <remarks/>
            public int series_status
            {
                get
                {
                    return this.series_statusField;
                }
                set
                {
                    this.series_statusField = value;
                }
            }

            /// <remarks/>
            public string series_start
            {
                get
                {
                    return this.series_startField;
                }
                set
                {
                    this.series_startField = value;
                }
            }

            /// <remarks/>
            public string series_end
            {
                get
                {
                    return this.series_endField;
                }
                set
                {
                    this.series_endField = value;
                }
            }

            /// <remarks/>
            public string series_image
            {
                get
                {
                    return this.series_imageField;
                }
                set
                {
                    this.series_imageField = value;
                }
            }

            /// <remarks/>
            public int my_id
            {
                get
                {
                    return this.my_idField;
                }
                set
                {
                    this.my_idField = value;
                }
            }

            /// <remarks/>
            public int my_watched_episodes
            {
                get
                {
                    return this.my_watched_episodesField;
                }
                set
                {
                    this.my_watched_episodesField = value;
                }
            }

            /// <remarks/>
            public string my_start_date
            {
                get
                {
                    return this.my_start_dateField;
                }
                set
                {
                    this.my_start_dateField = value;
                }
            }

            /// <remarks/>
            public string my_finish_date
            {
                get
                {
                    return this.my_finish_dateField;
                }
                set
                {
                    this.my_finish_dateField = value;
                }
            }

            /// <remarks/>
            public int my_score
            {
                get
                {
                    return this.my_scoreField;
                }
                set
                {
                    this.my_scoreField = value;
                }
            }

            /// <remarks/>
            public int my_status
            {
                get
                {
                    return this.my_statusField;
                }
                set
                {
                    this.my_statusField = value;
                }
            }

            /// <remarks/>
            public string my_rewatching
            {
                get
                {
                    return this.my_rewatchingField;
                }
                set
                {
                    this.my_rewatchingField = value;
                }
            }

            /// <remarks/>
            public int my_rewatching_ep
            {
                get
                {
                    return this.my_rewatching_epField;
                }
                set
                {
                    this.my_rewatching_epField = value;
                }
            }

            /// <remarks/>
            public uint my_last_updated
            {
                get
                {
                    return this.my_last_updatedField;
                }
                set
                {
                    this.my_last_updatedField = value;
                }
            }

            /// <remarks/>
            public string my_tags
            {
                get
                {
                    return this.my_tagsField;
                }
                set
                {
                    this.my_tagsField = value;
                }
            }
        }
    }
}