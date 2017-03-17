using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Video_katalog.Converters {
    class SerieLanguagesConverterToString : IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            Dictionary<string, int> languageDictionary = new Dictionary<string, int> ();
            int countAll = 0;            

            try {
                Serie serie = (Serie) value;
                foreach (SerieSeason tempSeason in serie.Seasons) {
                    FillLanguageDictionary (ref languageDictionary, tempSeason, parameter, ref countAll);
                }
                return ReturnLanguagesInString (languageDictionary, countAll);
            }
            catch {
            }
            try {
                SerieSeason season = (SerieSeason) value;
                FillLanguageDictionary (ref languageDictionary, season, parameter, ref countAll);
                return ReturnLanguagesInString (languageDictionary, countAll);
            }
            catch {
            }
            return "nije prosao converter";
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion

        private Dictionary<string, int> FillLanguageDictionary (ref Dictionary<string, int> languageDictionary, SerieSeason season, object parameter, ref int countAll) {
            foreach (SerieEpisode tempEpisode in season.Episodes) {
                countAll++;
                if (parameter.ToString () == "Subtitle") {                    
                    foreach (Language tempSubtitle in tempEpisode.SubtitleLanguageList) {                        
                        if (languageDictionary.ContainsKey (tempSubtitle.Name)) {
                            languageDictionary[tempSubtitle.Name]++;
                        }
                        else {
                            Language newLang = new Language ();
                            newLang.Name = tempSubtitle.Name;
                            newLang.ID = tempSubtitle.ID;
                            languageDictionary.Add (tempSubtitle.Name, 1);
                        }
                    }
                }
            }
            return languageDictionary;
        }

        private string ReturnLanguagesInString (Dictionary<string, int> languageDic, int countAll) {
            string toReturn = "";
            foreach (KeyValuePair<string, int> languagePair in languageDic) {
                toReturn += string.Format ("{0} ({1}/{2}), ", languagePair.Key, languagePair.Value, countAll);
            }
            if (toReturn.Length == 0)
                return toReturn;
            //makni ", " na kraju
            toReturn = toReturn.Substring (0, toReturn.Length - 2);
            //System.Windows.MessageBox.Show (toReturn);
            return toReturn;
        }
    }
}
