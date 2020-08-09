﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CsvHelper;
using KonaAnalyzer.Services;

namespace KonaAnalyzer.Data
{
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
        //public static async IAsyncEnumerable<T> GetListFromUrlAsyncEnumerable<T>(string url)
        //{
        //    int count = 0;
        //    var returnList = new List<T>();
        //    var text = await GetStringFromUrlAsync(url);//.Replace("\n",";");
        //    if (string.IsNullOrEmpty(text))
        //    {
        //        Debug.WriteLine("Text was empty");

        //    }
        //    else
        //    {


        //        // Console.WriteLine(text);
        //        try
        //        {
        //            using (TextReader sr = new StringReader(text))
        //            {
        //                var csv = new CsvReader(sr, CultureInfo.CurrentCulture);
        //                csv.Configuration.Delimiter = ",";
        //                csv.Configuration.MissingFieldFound = null;

        //                while (await csv.ReadAsync())
        //                {
        //                    count++;
        //                    if (count % 1000 == 0)
        //                        Debug.WriteLine(count);

        //                    var record = csv.GetRecord<T>();
        //                    yield return record;


        //                }

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine(ex.Message);
        //        }
        //    }


        //}
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