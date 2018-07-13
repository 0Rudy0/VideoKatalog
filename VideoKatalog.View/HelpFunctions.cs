using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Windows.Media.Imaging;

namespace Video_katalog {
    static class HelpFunctions {

        public static string TryToFetchResult (string source, string searchString) {
            try {
                string returnString = source.Substring (source.IndexOf (searchString));
                return returnString.Substring (1);
            }
            catch {
                return null;
            }
        }
        public static string ReverseString (string s) {
            char[] arr = s.ToCharArray ();
            Array.Reverse (arr);
            return new string (arr);
        }
        public static string ReturnWebPageSource (string link) {
            StringBuilder sb = new StringBuilder ();
            byte[] buf = new byte[8192];
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create (link);
            HttpWebResponse response = (HttpWebResponse) request.GetResponse ();
            Stream resStream = response.GetResponseStream ();
            string tempString = null;
            int count = 0;
            do {
                // fill the buffer with data
                count = resStream.Read (buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0) {
                    // translate from bytes to ASCII text
                    tempString = Encoding.ASCII.GetString (buf, 0, count);

                    // continue building the string
                    sb.Append (tempString);
                }
            }
            while (count > 0); // any more data to read
            return sb.ToString ();
        }
        public static string GetStringBetweenStrings (string sourceStr, string startStr, string endStr) {
            int startIndex = sourceStr.IndexOf (startStr) + startStr.Length;
            if (startIndex == (startStr.Length - 1)) {
                return "";
                //throw new Exception ("Ne postoji trazeni string u source-u");
            }
            string returnString = sourceStr.Substring (startIndex);
            int endIndex = returnString.IndexOf (endStr);
            returnString = returnString.Substring (0, endIndex);
            return returnString;
        }
        public static string GetStringFromStringToEnd (string sourceStr, string startStr) {
            int startIndex = sourceStr.IndexOf (startStr) + startStr.Length;
            if (startIndex == (startStr.Length - 1)) {
                return "";
                //throw new Exception ("Ne postoji trazeni string u source-u");
            }
            return sourceStr.Substring (startIndex);
        }
        public static string GetStringFromStartToString (string sourceStr, string endStr) {
            int endIndex = sourceStr.IndexOf (endStr);
            if (endIndex < 0) {
                return "";
                //throw new Exception ("Trazeni string nije nadjen u source-u.");
            }
            return sourceStr.Substring (0, endIndex);
        }
        public static string FormatStringForUrl (string source) {
            source = source.Replace (":", "%3A");
            source = source.Replace ("&", "%26");
            return source;
        }
        public static List<string> SplitByString (string source, string splitString) {
            List<string> listToReturn = new List<string> ();
            int indexOf = source.IndexOf (splitString);
            while (indexOf > -1) {
                string temp = source.Substring (0, indexOf);
                source = source.Substring (indexOf + 1);
                listToReturn.Add (temp);
                indexOf = source.IndexOf (splitString);
            }
            listToReturn.Add (source);
            return listToReturn;
        }
        public static DateTime DateFromString (string source) {
            DateTime date = new DateTime ();
            //moguca su slijedeca 2 formata datuma
            try {
                //2001
                date = new DateTime (int.Parse (source), 12, 31);
                date.AddYears (int.Parse (source));
            }
            catch {
                try {
                    //20 April 2006
                    string[] formatDate = source.Split (' ');
                    switch (formatDate[1]) {
                        case "January":
                            date = new DateTime (int.Parse (formatDate[2]), 1, int.Parse (formatDate[0]));
                            break;
                        case "February":
                            date = new DateTime (int.Parse (formatDate[2]), 2, int.Parse (formatDate[0]));
                            break;
                        case "March":
                            date = new DateTime (int.Parse (formatDate[2]), 3, int.Parse (formatDate[0]));
                            break;
                        case "April":
                            date = new DateTime (int.Parse (formatDate[2]), 4, int.Parse (formatDate[0]));
                            break;
                        case "May":
                            date = new DateTime (int.Parse (formatDate[2]), 5, int.Parse (formatDate[0]));
                            break;
                        case "June":
                            date = new DateTime (int.Parse (formatDate[2]), 6, int.Parse (formatDate[0]));
                            break;
                        case "July":
                            date = new DateTime (int.Parse (formatDate[2]), 7, int.Parse (formatDate[0]));
                            break;
                        case "August":
                            date = new DateTime (int.Parse (formatDate[2]), 8, int.Parse (formatDate[0]));
                            break;
                        case "September":
                            date = new DateTime (int.Parse (formatDate[2]), 9, int.Parse (formatDate[0]));
                            break;
                        case "October":
                            date = new DateTime (int.Parse (formatDate[2]), 10, int.Parse (formatDate[0]));
                            break;
                        case "November":
                            date = new DateTime (int.Parse (formatDate[2]), 11, int.Parse (formatDate[0]));
                            break;
                        case "December":
                            date = new DateTime (int.Parse (formatDate[2]), 12, int.Parse (formatDate[0]));
                            break;
                    }
                }
                catch {
                    date = new DateTime (2200, 1, 1);
                }
            }
            return date;
        }        
        public static string RemoveQuotes (string source) {
            //ako su navodnici na pocetku i/ili na kraju, makni ih
            if (source[0] == '"')
                source = source.Substring (1);
            int length = source.Length;
            if (source[length - 1] == '"')
                source = source.Substring (0, length - 1);
            return source;
        }
        public static void UploadPhotoFTP(string path, GlobalCategory category, string fileName) {
            FtpWebRequest request;
            //fileName = ReplaceIllegalChars (fileName);
            //fileName = fileName.Replace ("'", "%27");
            //fileName = fileName.Replace (" ", "%20");

            if (category == GlobalCategory.movie) {
                request = (FtpWebRequest)FtpWebRequest.Create(HardcodedStrings.ftpAdress + "movie/" + fileName);
                
            }
            else {
                request = (FtpWebRequest) FtpWebRequest.Create (HardcodedStrings.ftpAdress + "serie/" + fileName);
            }
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential (HardcodedStrings.ftpUsername, HardcodedStrings.ftpPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            FileStream stream = File.OpenRead(path);
            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            Stream reqStream = request.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();
        }
        public static void UploadNewVersionFTP () {
            FtpWebRequest request;
            request = (FtpWebRequest) FtpWebRequest.Create (HardcodedStrings.ftpAppUpdatesPath);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential (HardcodedStrings.ftpUsername, HardcodedStrings.ftpPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.ReadWriteTimeout = 30000000;
            request.Proxy = null;
            request.Timeout = 30000000;

            FileStream stream = File.OpenRead (HardcodedStrings.appExecutablePath.Replace("Video Katalog.exe", "Video Katalog - SelWin.exe"));
            byte[] buffer = new byte[stream.Length];

            stream.Read (buffer, 0, buffer.Length);
            stream.Close ();

            Stream reqStream = request.GetRequestStream ();
            reqStream.Write (buffer, 0, buffer.Length);
            reqStream.Close ();
        }
        public static void DownloadPhotoFTP (string posterName, GlobalCategory category) {
            // Get the object used to communicate with the server.
            FtpWebRequest request;

            //posterName = posterName.Replace ("'", "%27");
            //posterName = posterName.Replace (" ", "%20");
            //posterName = posterName.Replace (":", "");

            if (category == GlobalCategory.movie) {
                request = (FtpWebRequest) FtpWebRequest.Create (HardcodedStrings.ftpAdress + "movie/" + posterName + ".jpg");

            }
            else if (category == GlobalCategory.serie) {
                request = (FtpWebRequest) FtpWebRequest.Create (HardcodedStrings.ftpAdress + "serie/" + posterName + ".jpg");
            }
            else {
                request = (FtpWebRequest) FtpWebRequest.Create (HardcodedStrings.ftpAdress + "default.png");
            }
            
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.ReadWriteTimeout = 30000000;
            request.Proxy = null;
            request.Timeout = 30000000;

            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential (HardcodedStrings.ftpUsername, HardcodedStrings.ftpPassword);

            using (FtpWebResponse response = (FtpWebResponse) request.GetResponse ()) {
                using (Stream responseStream = response.GetResponseStream ()) {
                    string pathToSave = "";
                    if (category == GlobalCategory.movie) {
                        pathToSave = HardcodedStrings.moviePosterFolder + posterName + ".jpg";
                    }
                    else {
                        pathToSave = HardcodedStrings.seriePosterFolder + posterName + ".jpg";
                    }
                    using (FileStream fs = new FileStream (pathToSave, FileMode.Create)) {
                        byte[] buffer = new byte[102400];
                        int read = 0;
                        do {
                            read = responseStream.Read (buffer, 0, buffer.Length);
                            fs.Write (buffer, 0, read);
                            fs.Flush ();
                        } while (!(read == 0));
                        fs.Flush ();
                        fs.Close ();

                        response.Close ();
                        responseStream.Close ();
                    }
                }
            }
        }
        public static void DownloadNewVersionFTP () {
            // Get the object used to communicate with the server.
            FtpWebRequest request;
            request = (FtpWebRequest) FtpWebRequest.Create (HardcodedStrings.ftpAppUpdatesPath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential (HardcodedStrings.ftpUsername, HardcodedStrings.ftpPassword);

            using (FtpWebResponse response = (FtpWebResponse) request.GetResponse ()) {
                using (Stream responseStream = response.GetResponseStream ()) {
                    string pathToSave = HardcodedStrings.appExecutablePath.Replace ("Video Katalog.exe", "Video Katalog nova verzija.exe");

                    try {
                        //System.IO.File.Delete (pathToSave);

                        using (FileStream fs = new FileStream (pathToSave, FileMode.Create)) {
                            byte[] buffer = new byte[102400];
                            int read = 0;
                            do {
                                read = responseStream.Read (buffer, 0, buffer.Length);
                                fs.Write (buffer, 0, read);
                                fs.Flush ();
                            } while (!(read == 0));
                            fs.Flush ();
                            fs.Close ();

                            response.Close ();
                            responseStream.Close ();
                        }
                    }
                    catch (Exception e) {
                        throw e;
                    }
                }
            }
        }
        public static void DeletePhotoFTP (string posterName, GlobalCategory category) {
            string fileName = ReplaceIllegalChars (posterName);
            fileName = fileName.Replace ("'", "%27");
            fileName = fileName.Replace (" ", "%20");

            FtpWebRequest requestFileDelete;
            if (category == GlobalCategory.movie)
                requestFileDelete = (FtpWebRequest) FtpWebRequest.Create (HardcodedStrings.ftpAdress + "movie/" + fileName);
            else
                requestFileDelete = (FtpWebRequest) FtpWebRequest.Create (HardcodedStrings.ftpAdress + "serie/" + fileName);

            requestFileDelete.Credentials = new NetworkCredential (HardcodedStrings.ftpUsername, HardcodedStrings.ftpPassword);
            requestFileDelete.UsePassive = true;
            requestFileDelete.UseBinary = true;
            requestFileDelete.KeepAlive = false;
            requestFileDelete.Method = WebRequestMethods.Ftp.DeleteFile;

            FtpWebResponse responseFileDelete = (FtpWebResponse) requestFileDelete.GetResponse ();
        }
        public static void DeleteCurrentVersionFTP () {
            FtpWebRequest requestFileDelete;
            requestFileDelete = (FtpWebRequest) FtpWebRequest.Create (HardcodedStrings.ftpAppUpdatesPath);
            requestFileDelete.Credentials = new NetworkCredential (HardcodedStrings.ftpUsername, HardcodedStrings.ftpPassword);
            requestFileDelete.UsePassive = true;
            requestFileDelete.UseBinary = true;
            requestFileDelete.KeepAlive = false;
            requestFileDelete.Method = WebRequestMethods.Ftp.DeleteFile;

            FtpWebResponse responseFileDelete = (FtpWebResponse) requestFileDelete.GetResponse ();
        }
        public static bool IsVideoFile (FileInfo file) {
            foreach (VideoFileExtensions ext in Enum.GetValues (typeof (VideoFileExtensions))) {
                //makni tocku sa pocetka extenzije video filea, te prva 3 znaka svake extenzije  ("EXT")
                if (file.Extension.Remove (0, 1).ToLower () == ext.ToString ().Remove (0, 3))
                    return true;
            }
            return false;
        }
        public static string ReplaceIllegalChars (string orginal) {
            return orginal.Replace ("\\", "").Replace ("/", "").Replace (":", " -").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "");
        }
        public static string GetPosterImageUrl(string imdbLink, string title, string type) {
            string ImdbID = "";
            foreach (char c in imdbLink) {
                if (Char.IsDigit(c)) {
                    ImdbID += c.ToString();
                }
            }
            ImdbID = ImdbID.Replace("tt", "");
            ImdbID = "i" + ImdbID;
            bool replaced = true;
            while (replaced) {
                replaced = false;
                if (title.Contains(' ')) {
                    replaced = true;
                    title = title.Replace(' ', '-');
                }
            }

            try
            {
                string posterPageSource = ReturnWebPageSource("https://www.cinematerial.com/" + type + "/" + title + "-" + ImdbID);
                string imageSourceLink = GetStringBetweenStrins(posterPageSource, "class=\"postercollection title\"", "</div>");
                imageSourceLink = GetStringBetweenStrins(imageSourceLink, "src=\"", "\"");
                imageSourceLink = "https://www.cinematerial.com" + imageSourceLink;// +imageSourceLink.Replace("/sm/", "/md/");
                return imageSourceLink;
            }
            catch (Exception ex)
            {
                return "http://www.chabotcollege.edu/Library/subjectindex/film.jpg";
            }
        }
        public static string GetStringBetweenStrins(string sourceStr, string startStr, string endStr) {
            int startIndex = sourceStr.IndexOf(startStr) + startStr.Length;
            if (startIndex == (startStr.Length - 1)) {
                throw new Exception("Ne postoji trazeni string u source-u");
            }
            string returnString = sourceStr.Substring(startIndex);
            int endIndex = returnString.IndexOf(endStr);
            returnString = returnString.Substring(0, endIndex);
            return returnString;
        }

    }
}
