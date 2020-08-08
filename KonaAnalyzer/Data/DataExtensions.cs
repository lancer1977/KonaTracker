using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CsvHelper;

namespace KonaAnalyzer.Data
{
    public static class DataExtensions
    {
        public static async Task<string> GetStringFromUrlAsync(string url)
        {
            try
            {
                var results = string.Empty;
                var req = WebRequest.Create(url);
                var resp = await req.GetResponseAsync();

                var stream = resp.GetResponseStream();
                if (stream == null) return results;
                using (var sr = new StreamReader(stream))
                {
                    results = await sr.ReadToEndAsync();
                }
                return results;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }

        }

        public static async Task<List<T>> GetListFromUrlAsync<T>(string url)
        {
            int count = 0;
            var returnList = new List<T>();
            var text = await GetStringFromUrlAsync(url);//.Replace("\n",";");
            if (string.IsNullOrEmpty(text))
            {
                Debug.WriteLine("Text was empty");
                return returnList;
            }
            // Console.WriteLine(text);
            try
            {
                using (TextReader sr = new StringReader(text))
                {
                    var csv = new CsvReader(sr, CultureInfo.CurrentCulture);
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.MissingFieldFound = null;
                    await Task.Run(() =>
                    {
                        while (csv.Read())
                        {
                            count++;
                            if (count % 1000 == 0)
                                Debug.WriteLine(count);
                            try
                            {
                                var record = csv.GetRecord<T>();
                                returnList.Add(record);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }


                        }
                    });
            
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


            return returnList;
        }
    }
}