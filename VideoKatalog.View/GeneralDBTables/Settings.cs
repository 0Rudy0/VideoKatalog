using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Video_katalog {
    public class Settings {
        int id;
        string trailerProvider;
        string posterProvider;
        bool useOnlyOriginalName;
        string imageEditorPath;
        int imageQualityLevel;

        public int ImageQualityLevel {
            get {
                return this.imageQualityLevel;
            }
            set {
                this.imageQualityLevel = value;
            }
        }

        public string TrailerProvider {
            get {
                return this.trailerProvider;
            }
            set {
                this.trailerProvider = value;
            }
        }
        public int ID {
            get {
                return this.id;
            }
            set {
                this.id = value;
            }
        }
        public string PosterProvider {
            get {
                return this.posterProvider;
            }
            set {
                this.posterProvider = value;
            }
        }
        public string ImageEditorPath {
            get {
                return this.imageEditorPath;
            }
            set {
                this.imageEditorPath = value;
            }
        }
        public bool UseOnlyOriginalName {
            get {
                return this.useOnlyOriginalName;
            }
            set {
                this.useOnlyOriginalName = value;
            }
        }
    }
}
