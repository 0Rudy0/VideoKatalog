using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoKatalog.WebApp.ExtraClasses;

namespace VideoKatalog.WebApp.Controllers
{
	public class HomeController : Controller
	{
		//
		// GET: /Home/

		public ActionResult Index()
		{
			return RedirectToAction("List", "Movie");
		}

		[HttpGet]
		public ActionResult Login()
		{
			return View();
		}

		public ActionResult Logout()
		{
			Session["CurrentUser"] = null;
			return RedirectToAction("List", "Movie");
		}

		[HttpPost]
		public ActionResult Upload(HttpPostedFileBase file)
		{

			if (file != null && file.ContentLength > 0)
			{
				var fileName = Path.GetFileName(file.FileName);
				var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
				file.SaveAs(path);

				StreamReader reader = new StreamReader(path);
				string wholeString = reader.ReadToEnd();
				reader.Close();
				Session["selected"] = wholeString;
				System.IO.File.Delete(path);
				Session["loadSelected"] = true;
			}
			return RedirectToAction("List", "Movie");
		}

		public ActionResult Kontakt()
		{
			return View("Kontakt");
		}

		public string GetSelected()
		{
			string selected = Session["selected"].ToString();
			List<int> selectedMovies = new List<int>();
			List<Serie> selectedSeries = new List<Serie>();
			WebApp.Models.Selected selectedMS = new Models.Selected();
			selectedMS.selectedMovies = selectedMovies;
			selectedMS.selectedSeries = selectedSeries;

			if (selected != null)
			{

				int movieStartsIndex = selected.IndexOf("F") + 1;
				int movieEndIndex = selected.IndexOf("!S");
				if (movieStartsIndex < movieEndIndex)
				{
					string movies = selected.Substring(movieStartsIndex, movieEndIndex - movieStartsIndex);
					foreach (string movieID in movies.Split('%'))
					{
						if (!string.IsNullOrEmpty(movieID.Trim()))
						{
							selectedMovies.Add(int.Parse(movieID));
						}
					}
				}

				int serieStartsIndex = selected.IndexOf("!S") + 2;
				int serieSEndIndex = Math.Max(selected.IndexOf("\r\n"), selected.Length);
				if (serieStartsIndex < selected.Length)
				{					
					string serieSelectedString = selected.Substring(serieStartsIndex, serieSEndIndex - serieStartsIndex);
					char[] serieDelimiter = new char[] { '#'};
					foreach (string serieString in serieSelectedString.Split(serieDelimiter, StringSplitOptions.RemoveEmptyEntries))
					{
						Serie tempSerie = new Serie();
						tempSerie.serieID = int.Parse(serieString.Substring(0, serieString.IndexOf('$')));
						tempSerie.SerieSeason = new List<SerieSeason>();
						string serieRestString = serieString.Substring(serieString.IndexOf('$') + 1);

						char[] seasonDelimiter = new char[] { '$' };
						foreach (string seasonString in serieRestString.Split(seasonDelimiter, StringSplitOptions.RemoveEmptyEntries))
						{
							SerieSeason tempSeason = new SerieSeason();
							tempSeason.seasonID = int.Parse(seasonString.Substring(0, seasonString.IndexOf('&')));
							tempSeason.SerieEpisode = new List<SerieEpisode>();
							string seasonRestString = seasonString.Substring(seasonString.IndexOf('&') + 1);

							char[] episodeDelimiter = new char[] { '&' };
							foreach (string episodeString in seasonRestString.Split(episodeDelimiter, StringSplitOptions.RemoveEmptyEntries))
							{
								SerieEpisode tempEpisode = new SerieEpisode();
								tempEpisode.episodeID = int.Parse(episodeString);
								tempSeason.SerieEpisode.Add(tempEpisode);

							}

							tempSerie.SerieSeason.Add(tempSeason);
                        }

						selectedSeries.Add(tempSerie);
                    }
										
                }
			}
			return (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(selectedMS);
		}

		//[HttpPost]
		//public ActionResult Login(UserProfile enteredInfo) {
		//    //VideoDatabaseEntities context = new VideoDatabaseEntities();
		//    //try {
		//    //    UserProfile fetchedUser = context.UserProfile.Where(up => up.userName == enteredInfo.userName).Single();
		//    //    if (!HelpFunctions.EncodePassword(fetchedUser.userPassword.Trim()).Equals(enteredInfo.userPassword.Trim())) {
		//    //        ViewBag.ErrorMessage = "Lozinka nije ispravna";
		//    //        enteredInfo.userPassword = "";
		//    //        return View(enteredInfo);
		//    //    }
		//    //    Session["CurrentUser"] = fetchedUser;
		//    //}
		//    //catch (InvalidOperationException ex) {
		//    //    ViewBag.ErrorMessage = "Ne postoji korisnik s tim korisničkim imenom";
		//    //    enteredInfo.userName = "";
		//    //    enteredInfo.userPassword = "";
		//    //    return View(enteredInfo);
		//    //    //user doesn't exist
		//    //}
		//    return RedirectToAction("List", "Movie");
		//}

		//[HttpGet]
		//public ActionResult Register() {
		//    return View();
		//}

		//[HttpPost]
		//public ActionResult Register(UserProfile newUser, string userPasswordConfirm) {

		//    return RedirectToAction("List", "Movie");

		//    VideoDatabaseEntities context = new VideoDatabaseEntities();
		//    if ((context.UserProfile.Where(up => up.userName == newUser.userName).Count()) > 0) {
		//        ViewBag.ErrorMessage = "Korisničko ime već postoji";
		//        return View(newUser);
		//    }
		//    else {
		//        if (newUser.userPassword != userPasswordConfirm) {
		//            ViewBag.ErrorMessage = "Lozinke nisu iste";
		//            newUser.userPassword = "";
		//            return View(newUser);
		//        }
		//        newUser.UserRole = context.UserRole.Where(ur => ur.roleName == "User").Single();
		//        newUser.lastLogin = DateTime.Now;
		//        newUser.userPassword = HelpFunctions.EncodePassword(newUser.userPassword);
		//        context.UserProfile.Add(newUser);
		//        context.SaveChanges();
		//        Session["CurrentUser"] = newUser;
		//    }
		//    return RedirectToAction("List", "Movie");
		//}
	}
}
