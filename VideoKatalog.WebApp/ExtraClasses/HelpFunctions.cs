using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;

namespace VideoKatalog.WebApp.ExtraClasses {
    public static class HelpFunctions {

        public static string TryToFetchResult(string source, string searchString) {
            try {
                string returnString = source.Substring(source.IndexOf(searchString));
                return returnString.Substring(1);
            }
            catch {
                return null;
            }
        }

        public static string ReturnWebPageSource(string link) {
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                string tempString = null;
                int count = 0;
                do {
                    // fill the buffer with data
                    count = resStream.Read(buf, 0, buf.Length);

                    // make sure we read some data
                    if (count != 0) {
                        // translate from bytes to ASCII text
                        tempString = Encoding.ASCII.GetString(buf, 0, count);

                        // continue building the string
                        sb.Append(tempString);
                    }
                }
                while (count > 0); // any more data to read
                return sb.ToString();
            }
            catch {
                return null;
            }
        }

        public static string GetStringBetweenStrings(string sourceStr, string startStr, string endStr) {
            int startIndex = sourceStr.IndexOf(startStr) + startStr.Length;
            if (startIndex == (startStr.Length - 1)) {
                return "";
                //throw new Exception ("Ne postoji trazeni string u source-u");
            }
            string returnString = sourceStr.Substring(startIndex);
            int endIndex = returnString.IndexOf(endStr);
            returnString = returnString.Substring(0, endIndex);
            return returnString;
        }

        public static string GetStringFromStringToEnd(string sourceStr, string startStr) {
            int startIndex = sourceStr.IndexOf(startStr) + startStr.Length;
            if (startIndex == (startStr.Length - 1)) {
                return "";
                //throw new Exception ("Ne postoji trazeni string u source-u");
            }
            return sourceStr.Substring(startIndex);
        }

        public static string GetStringFromStartToString(string sourceStr, string endStr) {
            int endIndex = sourceStr.IndexOf(endStr);
            if (endIndex < 0) {
                return "";
                //throw new Exception ("Trazeni string nije nadjen u source-u.");
            }
            return sourceStr.Substring(0, endIndex);
        }

        public static List<string> SplitByString(string source, string splitString) {
            List<string> listToReturn = new List<string>();
            int indexOf = source.IndexOf(splitString);
            while (indexOf > -1) {
                string temp = source.Substring(0, indexOf);
                source = source.Substring(indexOf + 1);
                listToReturn.Add(temp);
                indexOf = source.IndexOf(splitString);
            }
            listToReturn.Add(source);
            return listToReturn;
        }

        public static string FormatStringForUrl(string source) {
            source = source.Replace(":", "%3A");
            source = source.Replace("&", "%26");
            return source;
        }

        public static string RemoveQuotes(string source) {
            //ako su navodnici na pocetku i/ili na kraju, makni ih
            if (source[0] == '"')
                source = source.Substring(1);
            int length = source.Length;
            if (source[length - 1] == '"')
                source = source.Substring(0, length - 1);
            return source;
        }

        public static List<string> GetEngGenres() {
            List<string> genresEng = new List<string> ();
            genresEng.Add ("Action");
            genresEng.Add ("Adventure");
            genresEng.Add ("Animation");
            genresEng.Add ("Biography");
            genresEng.Add ("Comedy");
            genresEng.Add ("Crime");
            genresEng.Add ("Documentary");
            genresEng.Add ("Drama");
            genresEng.Add ("Family");
            genresEng.Add ("Fantasy");
            genresEng.Add ("Film-Noir");
            genresEng.Add ("History");
            genresEng.Add ("Horror");
            genresEng.Add ("Music");
            genresEng.Add ("Musical");
            genresEng.Add ("Mystery");
            genresEng.Add ("Romance");
            genresEng.Add ("Sci-Fi");
            genresEng.Add ("Sport");
            genresEng.Add ("Thriller");
            genresEng.Add ("War");
            genresEng.Add ("Western");
            return genresEng;
        }

        public static List<string> GetCroGenres() {
            List<string> genresCro = new List<string>();
            genresCro.Add("Akcija");
            genresCro.Add("Avantura");
            genresCro.Add("Animacija");
            genresCro.Add("Biografija");
            genresCro.Add("Komedija");
            genresCro.Add("Krimi");
            genresCro.Add("Dokumentarni");
            genresCro.Add("Drama");
            genresCro.Add("Obiteljski");
            genresCro.Add("Fantastika");
            genresCro.Add("Crno-bijeli");
            genresCro.Add("Povijesni");
            genresCro.Add("Horor");
            genresCro.Add("Muzički");
            genresCro.Add("Pjevan");
            genresCro.Add("Misterija");
            genresCro.Add("Romantika");
            genresCro.Add("Znanstvena fantastika");
            genresCro.Add("Sportski");
            genresCro.Add("Triler");
            genresCro.Add("Ratni");
            genresCro.Add("Vestern");
            return genresCro;
        }

        /// <summary>
        /// Uploads movie or serie poster via FTP to hddpunjenje.herobo.com
        /// </summary>
        /// <param name="localPath">Local path to saved image to upload</param>
        /// <param name="category">Is it poster for "movie" or "serie"</param>
        /// <param name="fileName">File name of an image when it is saved after uploading</param>
        public static void UploadPhotoFTP(string localPath, string category, string fileName) {
            FtpWebRequest request;

            if (category == "movie") {
                request = (FtpWebRequest)FtpWebRequest.Create("ftp://files.000webhost.com/public_html/posters/movie/" + fileName);

            }
            else {
                request = (FtpWebRequest)FtpWebRequest.Create("ftp://files.000webhost.com/public_html/posters/serie/" + fileName);
            }
            request.Method = WebRequestMethods.Ftp.UploadFile;
            string ftpUsername = "a7194937";
            string ftpPassword = "rudXYZ1%";
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            FileStream stream = File.OpenRead(localPath);
            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            Stream reqStream = request.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();
        }

        public static void DownloadRemoteImageFile(string uri, string fileName) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)) {

                // if the remote file was found, download oit
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(fileName)) {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }
            }
        }

        //public static bool IsAuthorised(Controllers.MovieController controller) {
        //    try {
        //        UserProfile user = controller.Session["CurrentUser"] as UserProfile;
        //        if (user.UserRole.roleName.Trim() == "Admin") {
        //            return true;
        //        }
        //        else
        //            return false;
        //    }
        //    catch {
        //        return false;
        //    }
        //}

        public static string EncodePassword(string rawPassword) {
            SHA3.SHA3Managed man = new SHA3.SHA3Managed(224);
            return rawPassword;
        }

    }
}