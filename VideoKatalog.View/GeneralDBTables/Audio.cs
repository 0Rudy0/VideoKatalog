using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    public class Audio : INotifyPropertyChanged {
        int id;
        string format;
        decimal channels;
        Language language = new Language ();

        public int ID {
            get {
                return this.id;
            }
            set {
                this.id = value;
                NotifyPropertyChanged ("ID");
            }
        }
        public string Format {
            get {
                return this.format;
            }
            set {
                this.format = value;
                NotifyPropertyChanged ("Format");
            }
        }
        public decimal Channels {
            get {
                return this.channels;
            }
            set {
                this.channels = value;
                NotifyPropertyChanged ("Channels");
            }
        }
        public Language Language {
            get {
                return this.language;
            }
            set {
                this.language = value;
                NotifyPropertyChanged ("Language");
            }
        }

        public override string ToString () {
            return string.Format ("{0} - {1} ({2})", this.format, this.channels.ToString ("#.0").Replace (",", "."), this.language.ToString ());
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged (String info) {
            if (PropertyChanged != null) {
                PropertyChanged (this, new PropertyChangedEventArgs (info));
            }
        }

        #endregion
    }

    public class AudioComparerByCharacteristics : IEqualityComparer<Audio> {
        public bool Equals (Audio x, Audio y) {
            if (x.Format == y.Format &&
                x.Channels == y.Channels &&
                y.Language.Name == x.Language.Name)
                return true;
            else
                return false;
        }
        public int GetHashCode (Audio obj) {
            return obj.Format.Length + 7 * (int)(obj.Channels*10) + (System.Convert.ToInt32(obj.Language.ToString().Substring (0,1)));
        }
    }
    public class AudioComparerByID : IEqualityComparer<Audio> {
        public bool Equals (Audio x, Audio y) {
            if (x.ID == y.ID)
                return true;
            else
                return false;
        }
        public int GetHashCode (Audio obj) {
            return obj.Format.Length + 7 * (int) (obj.Channels * 10) + (System.Convert.ToInt32 (obj.Language.ToString ().Substring (0, 1)));
        }
    }
}
