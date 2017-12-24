using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaInfoLib;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

namespace Video_katalog {
    public abstract class Video : INotifyPropertyChanged {        
        private int id;
        private string name = "";
        private string origName = "";
        private int height;
        private int width;
        private int runtime;
        private long size;
        private int hddNum;
        private HDD hdd;
        private int bitrate;
        private string aspectRatio = "";
        private DateTime addTime = new DateTime (1900, 1, 1);
        private string summary = "";
        private string internetLink = "";
        private string trailerLink = "";
        private bool isViewed = false;
        private bool selected = false;
        private int version;
        private bool show = true;

        public int ID {
            get {
                return this.id;
            }
            set {
                if (value != this.id) {
                    this.id = value;
                    NotifyPropertyChanged ("ID");
                }
            }
        }
        public string Name {
            get {
                return this.name;
            }
            set {
                this.name = value;               
                NotifyPropertyChanged ("Name");                
            }
        }
        public string OrigName {
            get {
                return this.origName;
            }
            set {
                this.origName = value;
                NotifyPropertyChanged ("OrigName");
            }
        }
        public int Height{
            get {
                return this.height;
            }
            set {
                this.height = value;
                NotifyPropertyChanged ("Height");
            }
        }
        public int Width {
            get {
                return this.width;
            }
            set {
                this.width = value;
                NotifyPropertyChanged ("Width");
            }
        }
        public int Runtime {
            get {
                return this.runtime;
            }
            set {
                this.runtime = value;
                NotifyPropertyChanged ("Runtime");
            }
        }
        public long Size {
            get {
                return this.size;
            }
            set {
                this.size = value;
                NotifyPropertyChanged ("Size");
            }
        }
        public int HddNum {
            get {
                return this.hddNum;
            }
            set {
                this.hddNum = value;
                NotifyPropertyChanged ("HddNum");
            }
        }
        public HDD Hdd {
            get {
                return this.hdd;
            }
            set {
                this.hdd = value;
                NotifyPropertyChanged ("Hdd");
            }
        }
        public int Bitrate {
            get {
                return this.bitrate;
            }
            set {
                this.bitrate = value;
                NotifyPropertyChanged ("Bitrate");
            }
        }
        public string AspectRatio {
            get {
                return this.aspectRatio;
            }
            set {
                this.aspectRatio = value;
                NotifyPropertyChanged ("AspectRatio");
            }
        }
        public DateTime AddTime {
            get {
                return this.addTime;
            }
            set {
                this.addTime = value;
                NotifyPropertyChanged ("AddTime");
            }
        }
        public string Summary {
            get {
                return this.summary;
            }
            set {
                this.summary = value;
                NotifyPropertyChanged ("Summary");
            }
        }
        public string InternetLink {
            get {
                return this.internetLink;
            }
            set {
                this.internetLink = value;
                NotifyPropertyChanged ("InternetLink");
            }
        }
        public string TrailerLink {
            get {
                return this.trailerLink;
            }
            set {
                this.trailerLink = value;
                NotifyPropertyChanged ("TrailerLink");
            }
        }
        public bool IsViewed {
            get {
                return this.isViewed;
            }
            set {
                this.isViewed = value;
                NotifyPropertyChanged ("IsViewed");
            }
        }
        public bool Selected {
            get {
                return this.selected;
            }
            set {
                this.selected = value;
                NotifyPropertyChanged ("Selected");
            }
        }
        public int Version {
            get {
                return this.version;
            }
            set {
                this.version = value;
                NotifyPropertyChanged ("Version");
            }
        }
        public bool Show {
            get {
                return this.show;
            }
            set {
                this.show = value;
                NotifyPropertyChanged ("Show");
            }
        }
        
        public ObservableCollection<Language> SubtitleLanguageList {
            get;
            set;
        }
        public ObservableCollection<Audio> AudioList {
            get;
            set;
        }

        public Video () {
            AudioList = new ObservableCollection<Audio> ();
        }

        public virtual void GetTechInfo (string videoFilePath) {           
            MediaInfo MI = new MediaInfo ();
            MI.Open (videoFilePath);            
            this.AudioList.Clear ();
            this.SubtitleLanguageList.Clear ();
            this.Size = Convert.ToInt64 (MI.Get (0, 0, "FileSize"));
            this.Runtime = Convert.ToInt32 (MI.Get (0, 0, "Duration"));
            this.Height = Convert.ToInt32 (MI.Get (StreamKind.Video, 0, "Height"));
            this.Width = Convert.ToInt32 (MI.Get (StreamKind.Video, 0, "Width"));
            this.Bitrate = Convert.ToInt32 (MI.Get (0, 0, "OverallBitRate"));
            this.AspectRatio = MI.Get (StreamKind.Video, 0, "DisplayAspectRatio/String");
            this.AddTime = DateTime.Now;
            FillAudioList (MI);                      
            FillSubtitleList (MI);
            MI.Close ();            
        }
        void FillSubtitleList (MediaInfo MI) {            
            try {
                string allInfo = MI.Inform ();
                allInfo = allInfo.Substring (allInfo.IndexOf ("\r\nText"));
                foreach (string currLangString in allInfo.Split (new string[] { "\r\nText" }, System.StringSplitOptions.RemoveEmptyEntries)) {
                    Language newSubLang = new Language ();
                    try {
                        string langString = currLangString.Substring (currLangString.IndexOf ("\r\nLanguage"));
                        langString = langString.Substring (langString.IndexOf (":") + 1);
                        langString = langString.Substring (0, langString.IndexOf ("\r\n"));
                        foreach (string lang in langString.Split ('/')) {
                            if (string.IsNullOrWhiteSpace (lang))
                                continue;
                            newSubLang.Name = lang.Trim ();                            
                        }
                    }
                    catch {
                        newSubLang.Name = "Unknown";
                    }
                    if (this.SubtitleLanguageList.Contains (newSubLang, new LanguageComparerByName() ) == false)
                        this.SubtitleLanguageList.Add (newSubLang);
                }
            }
            catch {
                this.SubtitleLanguageList = new ObservableCollection<Language> ();
            }
        }
        void FillAudioList (MediaInfo MI) {
            try {
                string allInfo = MI.Inform ();
                allInfo = allInfo.Substring (allInfo.IndexOf ("\r\nAudio"));
                int endIndex = allInfo.IndexOf ("\r\nText");
                if (endIndex > 0)
                    allInfo = allInfo.Substring (0, endIndex);

                foreach (string audioString in allInfo.Split (new string[] { "\r\nAudio" }, System.StringSplitOptions.RemoveEmptyEntries)) {
                    Audio newAudio = new Audio ();
                    newAudio.Format = GetAudioFormat (audioString);
                    newAudio.Channels = GetAudioChannels (audioString);
                    newAudio.Language = GetAudioLanguage (audioString);
                    if (this.AudioList.Contains (newAudio, new AudioComparerByCharacteristics ()) == false)
                        this.AudioList.Add (newAudio);
                }               
            }
            catch {
                this.AudioList = new ObservableCollection<Audio> ();
            }            
        }
        string GetAudioFormat (string source) {            
            //OBLIK: "\r\nFormat                           : AC-3\r\n"
            string audioFormat = source.Substring (source.IndexOf ("\nFormat"));
            audioFormat = audioFormat.Substring (audioFormat.IndexOf (":") + 1);
            audioFormat = audioFormat.Substring (0, audioFormat.IndexOf ("\r\n"));
            audioFormat = audioFormat.Trim ();
            return audioFormat;            
        }
        decimal GetAudioChannels (string source) {
            //OBLIK: "\r\nChannel count                       : 6 channels\r\n"            
            string channelsString = source.Substring(source.IndexOf("\nChannel(s)"));
            decimal channels;
            channelsString = channelsString.Substring (channelsString.IndexOf (":") + 1);
            channelsString = channelsString.Substring (0, channelsString.IndexOf ("channel"));
            channelsString = channelsString.Trim ();
            int intChannels = int.Parse (channelsString);

            //NEMA UVIJEK: "\r\nChannel positions                : Front: L C R, Surround: L R, LFE\r\n";
            try {
                string channelsPos = source.Substring (source.IndexOf ("\nChannel positions"));
                channelsPos = channelsPos.Substring (channelsPos.IndexOf (":") + 1, channelsPos.IndexOf ("\r\n"));
                //LFE označava da ima subwoofer channel
                if (channelsPos.Contains ("LFE")) {
                    channels = Convert.ToDecimal (string.Format ("{0},1", intChannels - 1));
                }
                else {
                    channels = Convert.ToDecimal (string.Format ("{0},0", intChannels.ToString ()));
                }
            }
            catch {
                channels = Convert.ToDecimal (string.Format ("{0},0", intChannels.ToString ()));
            }
            return channels;
        }
        Language GetAudioLanguage (string source) {
            Language newLang = new Language ();
            try {
                string language = source.Substring (source.IndexOf ("\r\nLanguage"));
                language = language.Substring (language.IndexOf (":") + 1);
                language = language.Substring (0, language.IndexOf ("\r\n"));
                foreach (string lang in language.Split ('/')) {
                    if (string.IsNullOrWhiteSpace (lang))
                        continue;
                    newLang.Name = lang.Trim ();                    
                }
                return newLang;
            }
            catch {                
                //newLang.Name = "Unknown";
                newLang.Name = "English";
                return newLang;
            }
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
}
