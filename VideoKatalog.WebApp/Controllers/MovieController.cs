using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using VideoKatalog.WebApp.Models;
using System.Xml;
using VideoKatalog.WebApp.ExtraClasses;
using Newtonsoft.Json.Linq;

namespace VideoKatalog.WebApp.Controllers
{
	public class MovieController : Controller
	{
		public ActionResult List()
		{
			return View("MovieList");
		}

		public ActionResult MovieList()
		{
			return View("MovieList");
		}

		public ActionResult Test()
		{
			return View("Test");
		}

		public JObject moviesAfterDate(int year, int month, int day, int hours, int minutes, int seconds)
		{
			DateTime passedDate = new DateTime(year, month, day, hours, minutes, seconds);
			VideoDatabaseEntities context = new VideoDatabaseEntities();

			System.Linq.IOrderedQueryable<Movie> movies = context.Movie.Where(m => m.addDate >= passedDate).OrderBy(m => m.customName);
			return CreateJSON(movies.ToList());
		}

		public JObject allMoviesOrdered(string sortProperty)
		{
			try
			{
				VideoDatabaseEntities context = new VideoDatabaseEntities();

				System.Linq.IOrderedQueryable<Movie> movies;
				//System.Linq.IQueryable<Movie> movies;

				switch (sortProperty)
				{
				case "name":
					movies = GetCachedMovies().OrderBy(m => m.customName);
					break;
				case "rating":
					movies = GetCachedMovies().OrderByDescending(m => m.rating);
					break;
				case "runtime":
					movies = GetCachedMovies().OrderByDescending(m => m.runtime);
					break;
				case "year":
					movies = GetCachedMovies().OrderByDescending(m => m.year);
					break;
				case "addDate":
					movies = GetCachedMovies().OrderByDescending(m => m.addDate);
					break;
				default:
					movies = GetCachedMovies().OrderBy(m => m.customName);
					break;
				}

				List<Movie> movieList = new List<Movie>();
				foreach (Movie tempMovie in movies)
				{
					movieList.Add(tempMovie);
				}
				return CreateJSON(movieList);
			}
			catch (Exception e)
			{
				return CreateJSON(new List<Movie>());
			}
		}

		public JObject fullMovieInfo(int movieID)
		{
			VideoDatabaseEntities context = new VideoDatabaseEntities();
			Movie movie = context.Movie.Where(m => m.movieID == movieID).Single();
			List<Movie> movieList = new List<Movie>();
			movieList.Add(movie);
			return CreateJSON(movieList);
		}

		public JObject allMovies()
		{
			VideoDatabaseEntities context = new VideoDatabaseEntities();

			try
			{
				IQueryable<Movie> movies = context.Movie.OrderBy(m => m.customName).Take(50);
				return CreateJSON(movies.ToList());
			}
			catch
			{
				return CreateJSON(new List<Movie>());
			}
		}

		public ActionResult Details(int id)
		{
			VideoDatabaseEntities context = new VideoDatabaseEntities();
			return View(context.Movie.Where(m => m.movieID == id).Single());
		}

		public ActionResult SelectedList()
		{
			return View("SelectedMovieMobileList");
		}

		public ActionResult Filter()
		{
			return View("FilterMovieMobileList");
		}

		private JObject CreateJSON(List<Movie> movieList)
		{
			JObject o = new JObject(
				new JProperty("movies",
				new JArray(
				   from m in movieList
				   select new JObject(
					   new JProperty("id", m.movieID),
					   new JProperty("name", m.customName),
					   new JProperty("origName", m.originalName),
					   new JProperty("size", m.size),
					   new JProperty("rating", m.rating),
					   new JProperty("plot", m.plot),
					   new JProperty("year", m.year),
					   new JProperty("runtime", m.runtime),
					   new JProperty("dateAdded", m.addDate),
					   new JProperty("imdbLink", m.internetLink),
					   new JProperty("trailerLink", m.trailerLink),
					   new JProperty("hddNum", m.hddNum),
					   new JProperty("hasCroSub", m.hasCroSub),
					   new JProperty("hasEngSub", m.hasEngSub),
					   new JProperty("cast", m.cast),
					   new JProperty("director", m.director),
					   new JProperty("genres", m.genres)
				   )
			   )));
			return o;
		}

		private System.Data.Entity.DbSet<Movie> GetCachedMovies()
		{
			System.Data.Entity.DbSet<Movie> cachedMovies =  (System.Data.Entity.DbSet<Movie>)System.Web.HttpRuntime.Cache.Get("movies");

			if (cachedMovies == null)
			{
				cachedMovies = (new VideoDatabaseEntities()).Movie;
				System.Web.HttpRuntime.Cache.Insert("movies", cachedMovies);
			}

			return cachedMovies;
		}

		public int CheckForNewMovies()
		{
			System.Data.Entity.DbSet<Movie> cachedMovies = (System.Data.Entity.DbSet<Movie>)System.Web.HttpRuntime.Cache.Get("movies");

			if (cachedMovies == null)
			{				
				return 0;
			}
			else
			{
				System.Data.Entity.DbSet<Movie> databaseMovies = (new VideoDatabaseEntities()).Movie;
				if (databaseMovies.Count() != cachedMovies.Count())
				{
					cachedMovies = databaseMovies;
					System.Web.HttpRuntime.Cache.Insert("movies", cachedMovies);
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}
	}
}
