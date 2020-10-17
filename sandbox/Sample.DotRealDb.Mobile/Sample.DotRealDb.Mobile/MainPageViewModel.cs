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
        private IList<WeatherForecast> items;

        public MainPageViewModel()
        {
            changeHandler = new DotRealChangeHandler(new DotRealDbClientOptions
            {
                ServerBaseUrl = "http://10.0.2.2:5000"
            });
            FetchData();
        }

        public IList<WeatherForecast> Items { get => items; set { items = value; OnPropertyChanged(); } }

        private async void FetchData()
        {
            this.Items = await changeHandler.ConnectAndTrackAsync<WeatherForecast>("SampleDbContext");
        }
    }
}
