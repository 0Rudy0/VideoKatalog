using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace VideoKatalog.WebApp.Controllers {
    public class SerieController : Controller {

        public ActionResult List() {
            //VideoDatabaseEntities context = new VideoDatabaseEntities();
            //if (Request.Browser.IsMobileDevice) {
            //    return View("SerieMobileList", context.Serie.OrderBy(s => s.name));
            //}
            //else {
            //    return View("SerieMobileList", context.Serie.OrderBy(s => s.name));
            //}   
            return View("SerieList");
        }

        public JObject allSeriesOrdered(string sortProperty) {
            VideoDatabaseEntities context = new VideoDatabaseEntities();

            System.Linq.IOrderedQueryable<Serie> series;
            List<Serie> serieList = new List<Serie>();
            //System.Linq.IQueryable<Movie> movies;

            switch (sortProperty) {
                case "name":
                    series = context.Serie.OrderBy(m => m.name);
                    break;
                case "rating":
                    series = context.Serie.OrderByDescending(m => m.rating);
                    break;
                default:
                    series = context.Serie.OrderBy(m => m.name);
                    break;
            }
            foreach (Serie tempSer in series) {
                serieList.Add(tempSer);
            }
            return CreateJSONSerie(serieList);
        }

        public JObject getFullSerie(int serieID) {
            VideoDatabaseEntities context = new VideoDatabaseEntities();

            Serie tempSerie = context.Serie.Where(s => s.serieID == serieID).Single();
            return CreateJSONForSingleSerie(tempSerie);
        }

        public JObject serieSeason(int serieID) {
            VideoDatabaseEntities context = new VideoDatabaseEntities();
            System.Linq.IOrderedQueryable<SerieSeason> seasons;
            List<SerieSeason> seasonsList = new List<SerieSeason>();
            if (serieID != 0) {
                seasons = context.SerieSeason.Where(ss => ss.serieID == serieID).OrderBy(ss => ss.name);
                foreach (SerieSeason tempSeas in seasons) {
                    seasonsList.Add(tempSeas);
                }

                return CreateJSONSeason(seasonsList);
            }
            else {
                return null;
            }
        }

        public JObject serieEpisodes(int seasonID) {
            VideoDatabaseEntities context = new VideoDatabaseEntities();
            System.Linq.IOrderedQueryable<SerieEpisode> episodes;
            List<SerieEpisode> episodesList = new List<SerieEpisode>();
            if (seasonID != 0) {
                episodes = context.SerieEpisode.Where(ss => ss.seasonID == seasonID).OrderBy(ss => ss.name);
                foreach (SerieEpisode tempEp in episodes) {
                    episodesList.Add(tempEp);
                }
                return CreateJSONEpisode(episodesList);
            }
            else {
                return null;
            }
        }

        private JObject CreateJSONSerie(List<Serie> serieList) {
            JObject o = new JObject(
                new JProperty("series",
                new JArray(
                   from s in serieList
                   select new JObject(
                       new JProperty("id", s.serieID),
                       new JProperty("name", s.name),
                       new JProperty("origName", s.origName),
                       new JProperty("rating", s.rating),
                       new JProperty("internetLink", s.internetLink),
                       new JProperty("summary", s.summary),
                       new JProperty("trailerLink", s.trailerLink),
                       new JProperty("cast", s.cast),
                       new JProperty("director", s.director),
                       new JProperty("genres", s.genres)
                   )
               )));
            return o;
        }

        private JObject CreateJSONForSingleSerie(Serie serie) {
            JObject serieJobj = new JObject(
                new JProperty("id", serie.serieID),
                new JProperty("name", serie.name),
                new JProperty("origName", serie.origName),
                new JProperty("rating", serie.rating),
                new JProperty("internetLink", serie.internetLink),
                new JProperty("trailerLink", serie.trailerLink),
                new JProperty("cast", serie.cast),
				new JProperty("summary", serie.summary),
				new JProperty("director", serie.director),
                new JProperty("genres", serie.genres),
                new JProperty("seasons",
                    new JArray(
                        from ss in serie.SerieSeason.OrderBy(x => x.name)
                        select new JObject(
                            new JProperty("id", ss.seasonID),
                            new JProperty("serieID", ss.serieID),
                            new JProperty("name", ss.name),
                            new JProperty("internetLink", ss.internetLink),
                            new JProperty("trailerLink", ss.trailerLink),
                            new JProperty("episodes",
                                new JArray(
                                from se in ss.SerieEpisode.OrderBy(y => y.name)
                                select new JObject(
                                    new JProperty("id", se.episodeID),
                                    new JProperty("name", se.name),
                                    new JProperty("origName", se.origName),
                                    new JProperty("seasonID", se.seasonID),
                                    new JProperty("airDate", se.airDate),
                                    new JProperty("size", se.size),
                                    new JProperty("runtime", se.runtime),
                                    new JProperty("hddID", se.hddID),
                                    new JProperty("internetLink", se.internetLink),
                                    new JProperty("hasCroSub", se.hasCroSub),
                                    new JProperty("hasEngSub", se.hasEngSub)
            )))))));                 

            return serieJobj;
        }

        private JObject CreateJSONSeason(List<SerieSeason> seasonList) {
            JObject o = new JObject(
                new JProperty("seasons",
                new JArray(
                   from s in seasonList
                   select new JObject(
                       new JProperty("id", s.seasonID),
                       new JProperty("serieID", s.serieID),
                       new JProperty("name", s.name),
                       new JProperty("internetLink", s.internetLink),
                       new JProperty("trailerLink", s.trailerLink)
                   )
               )));
            return o;
        }

        private JObject CreateJSONEpisode(List<SerieEpisode> episodeList) {
            JObject o = new JObject(
                new JProperty("episodes",
                new JArray(
                   from e in episodeList
                   select new JObject(
                       new JProperty("id", e.episodeID),
                       new JProperty("name", e.name),
                       new JProperty("origName", e.origName),
                       new JProperty("seasonID", e.seasonID),
                       new JProperty("airDate", e.airDate),
                       new JProperty("size", e.size),
                       new JProperty("runtime", e.runtime),
                       new JProperty("hddID", e.hddID),
                       new JProperty("internetLink", e.internetLink),
                       new JProperty("hasCroSub", e.hasCroSub),
                       new JProperty("hasEngSub", e.hasEngSub)
                   )
               )));
            return o;
        }
    }
}
