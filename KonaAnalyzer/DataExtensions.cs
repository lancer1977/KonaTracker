using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CsvHelper;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Services;
using Newtonsoft.Json;

namespace KonaAnalyzer.Data
{
    public static class MathExtensions
    {
        public static int Round(this int i, int nearest)
        {
            if (nearest <= 0 || nearest % 10 != 0)
                throw new ArgumentOutOfRangeException("nearest", "Must round to a positive multiple of 10");

            return (i + 5 * nearest / 10) / nearest * nearest;
        }
    }
    public static class DataExtensions
    {
     
        public static async Task<string> GetStringFromUrlAsync_old(string url)
        {
            try
            {
                var results = string.Empty;
                var req = WebRequest.Create(url);
                Debug.WriteLine("WebReqeist Create Passed");
                var resp = await req.GetResponseAsync();
                Debug.WriteLine("GetResponseAsync  Passed");
                var stream = resp.GetResponseStream();
                Debug.WriteLine("GetResponseStream  Passed");
                if (stream == null) return results;
                using (var sr = new StreamReader(stream))
                {
                    results = await sr.ReadToEndAsync();
                }
                Debug.WriteLine("StreamReader  Passed");
                return results;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }

        }

        public static async Task<string> GetStringFromUrlAsync(string url)
        {
            if (HttpService.GetHandler == null)
            {
                return await GetStringFromUrlAsync_old(url);
            }
            else
            {
                return await GetStringFromUrlAsync_new(url);
            }
        }

        public static async Task<string> GetStringFromUrlAsync_new(string url)
        {
            try
            {
                var http = HttpService.Instance.CreateHttp();
                var response = await http.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }

        }
        public static async Task<List<T>> GetListFromUrlAsync<T>(string url, Serialize type = Serialize.CSV)
        {
            switch (type)
            {
                case Serialize.Json:return await GetListFromJsonUrlAsync<T>(url);
                case Serialize.XML:
                    break;
                case Serialize.CSV: return await GetListFromUrlAsyncCSV<T>(url);
            }
            return null;
        }
        private static async Task<List<T>> GetListFromUrlAsyncCSV<T>(string url )
        { 
            Debug.WriteLine("Parsing html at " + url);
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
                    
                    //csv.Configuration.MissingFieldFound = (x, y, z) => { Debug.WriteLine(x.Aggregate((a,b)=>a + b)); };
                    while (await csv.ReadAsync())
                        { 
                            try
                            {
                                var record = csv.GetRecord<T>();
                                returnList.Add(record);
                            }
                            catch (Exception ex)
                            {
                                return returnList;
                                Debug.WriteLine(ex.Message);
                            }


                        } 

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


            return returnList;
        }

        private static async Task<List<T>> GetListFromJsonUrlAsync<T>(string url)
        { 
            var returnList = new List<T>();
            var text = await GetStringFromUrlAsync(url);//.Replace("\n",";");
            if (string.IsNullOrEmpty(text))
            {
                Debug.WriteLine("Text was empty");
                return returnList;
            }
            var items = JsonConvert.DeserializeObject<List<T>>(text);
            return items; 
        }
    }
}