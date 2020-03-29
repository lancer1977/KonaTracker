using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using CsvHelper;

namespace KonaAnalyzer.Data
{
    public static class DataExtensions
    {
        public static string GetCSV(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }

        public static List<T> GetListFromUrl<T>(string url)
        {
            var returnList = new List<T>();
            var text = GetCSV(url);//.Replace("\n",";");

           // Console.WriteLine(text);
            using (TextReader sr = new StringReader(text))
            {
                CsvReader csv = new CsvReader(sr, CultureInfo.CurrentCulture);
                csv.Configuration.Delimiter = ",";
                csv.Configuration.MissingFieldFound = null;
                while (csv.Read())
                {
                    T Record = csv.GetRecord<T>();
                    returnList.Add(Record);
                }
            }

            return returnList;
        }
    }
}