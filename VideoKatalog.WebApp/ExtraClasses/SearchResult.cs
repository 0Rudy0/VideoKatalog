﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoKatalog.WebApp.ExtraClasses {
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
        public override string ToString() {
            return this.Name;
        }
    }
}