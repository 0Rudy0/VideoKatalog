using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Video_katalog {
    public class SearchResult {
        public string Name {
            get;
            set;
        }
        public string Link {
            get;
            set;
        }
        public string Description {
            get;
            set;
        }
        public override string ToString () {
            return this.Name;
        }
    }
}
