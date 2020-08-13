using System;
using Newtonsoft.Json;

namespace KonaAnalyzer.Services
{
    public class SettingsService : ISettings
    {
        private DateTime _lastDate;

        public SettingsService()
        {
            try
            {

                _lastDate = JsonConvert.DeserializeObject<DateTime>(Xamarin.Essentials.SecureStorage
                    .GetAsync("LastDate").GetAwaiter().GetResult());
            }
            catch (Exception ex)
            {

            }
        }

        public DateTime LastDate
        {
            get { return _lastDate; }
            set
            {
                if (_lastDate == value) return;
                _lastDate = value;
                Xamarin.Essentials.SecureStorage.SetAsync("LastDate", JsonConvert.SerializeObject(value));

            }
        }
    }
}