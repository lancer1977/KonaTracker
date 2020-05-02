using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using CsvHelper;

namespace KonaAnalyzer.Data
{
    public static class DataExtensions
    {
        public static string GetStringFromUrl(string url)
        {
            var results = string.Empty;
            var req = (HttpWebRequest)WebRequest.Create(url);
            var resp = (HttpWebResponse)req.GetResponse();

            var stream = resp.GetResponseStream();
            if (stream == null) return results;
            var sr = new StreamReader(stream);
            results = sr.ReadToEnd();
            sr.Close();
            return results;
        }

        public static List<T> GetListFromUrl<T>(string url)
        {
            var returnList = new List<T>();
            var text = GetStringFromUrl(url);//.Replace("\n",";");

            // Console.WriteLine(text);
            using (TextReader sr = new StringReader(text))
            {
                var csv = new CsvReader(sr, CultureInfo.CurrentCulture);
                csv.Configuration.Delimiter = ",";
                csv.Configuration.MissingFieldFound = null;
                while (csv.Read())
                {
                    var record = csv.GetRecord<T>();
                    returnList.Add(record);
                }
            }

            return returnList;
        }
    }
}