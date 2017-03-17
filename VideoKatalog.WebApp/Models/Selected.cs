using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoKatalog.WebApp.Models
{
	public class Selected
	{

		public List<int> selectedMovies { get; set; }
		public List<Serie> selectedSeries { get; set; }
	}

	public class SelectedSerie
	{
		public int serieID { get; set; }
		//public List< { get; set; }
	}
}