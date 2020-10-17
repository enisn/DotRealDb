using DotRealDb.Client;
using DotRealDb.Client.Configurations;
using Sample.DotRealDb.Web.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Sample.DotRealDb.Mobile
{
    public class MainPageViewModel : BindableObject
    {
        private readonly DotRealChangeHandler changeHandler;
        public MainPageViewModel()
        {
            changeHandler = new DotRealChangeHandler(new DotRealDbClientOptions
            {
                ServerBaseUrl = "http://10.0.2.2:5000"
            });
            FetchData();
        }

        public IList<WeatherForecast> Items { get; set; } = new ObservableCollection<WeatherForecast>();

        private async void FetchData()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync("http://10.0.2.2:5000/WeatherForecast");

                var list = JsonSerializer.Deserialize<IList<WeatherForecast>>(response);
                list.ForEach(this.Items.Add);

                await changeHandler.StartTrackingAsync(Items, "SampleDbContext");
            }
        }
    }
}
