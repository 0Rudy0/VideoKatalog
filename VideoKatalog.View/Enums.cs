using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Video_katalog {

    public enum GlobalCategory {
        movie,
        serie,        
        homeVideo
    }

    public enum WishOrRegular {
        regular,
        wish
    }

    public enum SerieType {
        regular,
        wish
    }

    public enum SerieCategory {
        serie,
        season,
        episode,
        none
    }

    public enum VideoFileExtensions {
        EXTavi,
        EXTmpg,
        EXTmpeg,
        EXTvob,
        EXTmp4,
        EXTmov,
        EXTmkv,
        EXT3gp,
        EXTwmv,
        EXTmpgv,
        EXTmpv,
        EXTm1v,
        EXTm2v,
        EXTasf,
        EXTrmvb
    }

    public enum AddOrEdit {
        add,
        edit
    }

    public enum PersonTypes {
        zaKucniVideo,
        zaFilmoveSerije
    }
}
