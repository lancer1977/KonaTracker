using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using KonaAnalyzer.Dapper.Annotations;
using KonaAnalyzer.Data.Interface;
using PolyhydraGames.Core.Interfaces;

namespace KonaAnalyzer.Dapper
{
    public abstract class DapperSource<T> : INotifyPropertyChanged
    {
        public abstract Task<List<T>> GetWebItems();
        protected string TableName { get; }
        protected readonly IDBConnectionFactory Factory;
        protected DapperSource(IDBConnectionFactory factory)
        {
            Factory = factory;
            TableName = typeof(T).Name;
        }

        public LoadedState LoadState { get; }


        public async Task Reload()
        {
            await LoadAsync();
        }
        protected IEnumerable<T> GetAll
        {
            get
            {
                using var con = Factory.GetConnection();
                return con.Query<T>($"SELECT * FROM {TableName}");
            }
        }

        public virtual async Task LoadAsync()
        {
            if (GetAll.Any()) return;
            try
            {
                var locations = await GetWebItems();

                using var con = Factory.GetConnection();
                con.Insert(locations);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            OnPropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}