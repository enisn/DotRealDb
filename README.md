# DotRealDb

A real-time database for ASpNetCore Backend which powered by SignalR.

> STILL UNDER-DEVELOPMENT

# Summary
*to be added...*

# Getting Started

## Server-Side

- Install NuGet package

- Add following code to your Startup:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    //...

    services.AddDotRealDb(); // <-- Add this one.
}
```

- Go your DbContext which you want to make real-time and inherit from **DotRealDbContext**. If you're using IdentityDbContext or something else and not able to change your inheritance, you just go ---alternative way--- step to make manual implementation.

```csharp
public class SampleDbContext : DotRealDbContext // <- Change inheritance
{
    protected SampleDbContext() : base()
    {
    }

    // inject IDotRealChangeTracker and sent it to base.
    public SampleDbContext(DbContextOptions options, IDotRealChangeTracker tracker) : base(options, tracker)
    {
    }

    /* Anything else...*/
}
```

--- Alternative Way ---

If you're not able to change your DbContext inehritance, just use this way.

 1) Inject **IDotREalChangeTracker** into your DbContext:
 2) Override **SaveChanges** methods, and call **TrackAndPublishAsync()** method over tracker

```csharp
public class SampleDbContext : IdentityDbContext  // Can't change this inheritance
{

    private readonly IDotRealChangeTracker tracker;
    protected SampleDbContext() : base()
    {
    }

    // (1) inject IDotRealChangeTracker and set it into a field.
    public SampleDbContext(DbContextOptions options, IDotRealChangeTracker tracker) : base(options, tracker)
    {
        this.tracker = tracker;
    }

    /* Anything else...*/

    // (2) Override SaveChanges method and call TrackAndPublish method.
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        tracker.TrackAndPublish(this); // <-- Here
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        await tracker.TrackAndPublishAsync(this); // <-- And here
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
```

- That's it! Your server-side implementation is ready for your DbContext.

***

## Client-Side

### Dependency Injection Supported Apps (Blazor, ConsoleApp etc.)

- Go your Configure services or Program.cs for Blazor:

For regular Service Collections:

```csharp
services.AddDotRealDbClient(opts => opts.ServerBaseUrl = "https://localhost:5001");
```


For Blazor:
```csharp
public static async Task Main(string[] args)
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("#app");

    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

    // Add here for Blazor:
    builder.Services.AddDotRealDbClient(opts => opts.ServerBaseUrl = "https://localhost:5001");

    await builder.Build().RunAsync();
}
```

- Inject **IDotRealChangeHandler** into your object or get it from DI Container.

- Create a list with type of your model.

- Start tracking over IDotRealChangeHandler interface.

Plain C#:
```csharp
public class MyViewModel
{
    private readonly IDotRealChangeHandler changeHandler;

    public MyViewModel(IDotRealChangeHandler changeHandler)
    {
        this.changeHandler = changeHandler;
        StartTracking();
    }

    /* You can use any type of collection but I suggest to use ObservableCollection in here. 
     * Because tracker will add, remove, or update elements in your given list.
     * So you can handle changes manually with CollectionChanged event.
     */
    public ObservableCollection<WeatherForecast> Items { get; set; } = new ObservableCollection<WeatherForecast>();
    private async void StartTracking()
    {
        await changeHandler.StartTrackingAsync(this.Items, "SampleDbContext");
    }
}
```


For blazor:

```html
@using DotRealDb.Client
@using System.Collections.ObjectModel;
@inject HttpClient Http
@inject IDotRealChangeHandler ChangeHandler

<h1>Weather forecast</h1>

<p>This component demonstrates fetching data from the server.</p>

<hr />

@if (forecasts == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var forecast in forecasts)
            {
                <tr>
                    <td>@forecast.Date.ToShortDateString()</td>
                    <td>@forecast.TemperatureC</td>
                    <td>@forecast.TemperatureF</td>
                    <td>@forecast.Summary</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private ObservableCollection<WeatherForecast> forecasts;

    protected override async Task OnInitializedAsync()
    {
        forecasts = await Http.GetFromJsonAsync<ObservableCollection<WeatherForecast>>("https://localhost:5001" + "/WeatherForecast");
        forecasts.CollectionChanged += (_, __) => StateHasChanged();

        await ChangeHandler.StartTrackingAsync(forecasts, "SampleDbContext");
    }
}

```

***

### Dependency Injection Not-Supported Built-in Apps(Xamarin Forms)

- Add `DotRealDb.Client` nuget package into your portable layer.

- Go your ViewModel and Create an instance of **DotRealChangeHandler**.

- Create a list and start to track it.

````csharp
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
    /* I suggest to use ObservableCollection in here.
     * UI will be automatically updated in Xamarin Forms.
     */
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
```