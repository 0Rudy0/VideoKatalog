using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Video_katalog {
    public class Selection {
        int id;
        string name;
        DateTime dateOfLastChange;
        long size;

        public int ID {
            get {
                return this.id;
            }
            set {
                this.id = value;
            }
        }
        public string Name {
            get {
                return this.name;
            }
            set {
                this.name = value;
            }
        }
        public DateTime DateOfLastChange {
            get {
                return this.dateOfLastChange;
            }
            set {
                this.dateOfLastChange = value;
            }
        }
        public long Size {
            get {
                return this.size;
            }
            set {
                this.size = value;
            }
        }

        public ObservableCollection<Movie> SelectedMovies = new ObservableCollection<Movie> ();
        public ObservableCollection<SerieEpisode> selectedEpisodes = new ObservableCollection<SerieEpisode> ();

        public override string ToString () {
            return this.name;
        }
    }
}
