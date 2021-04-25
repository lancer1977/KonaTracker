using System;
using System.Diagnostics;
using PolyhydraGames.Core.Interfaces;

namespace KonaAnalyzer.Cli.Setup
{
    public class StorageFolder : IStorageFolder
    {
        public string Get()
        {
            //var folder = Environment.CurrentDirectory;
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Debug.WriteLine(folder);
            return folder; // Documents folder
        }
    }
}