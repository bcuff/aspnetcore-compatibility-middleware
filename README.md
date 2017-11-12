# ASP .NET Core Compatibility Middleware
Provides compatibility for accessing HttpContext in a way similar to the old ASP .NET

### Install
Package is hosted on nuget [here](https://www.nuget.org/packages/AspNetCoreCompatibility).
```
dotnet add package AspNetCoreCompatibility
```

### Setup
Add the following to your Startup.cs file.
```csharp
using AspNetCoreCompatibility;

// In Startup.cs

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        // Enables the compatibility middleware.
        // Middleware registered before this will not run in compatibility mode
        // and won't have access to HttpContextAccessor.Current.
        app.UseAspNetCompatibility();
        // ...
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        // Optional
        // Allows access to HttpContext via IHttpContextAccessor
        services.AddSingleton(new HttpContextAccessor());
    }
}
```

### Usage
Old code using this:
```csharp
using System.Web;

void ExampleMethod()
{
    var context = HttpContext.Current;
    // ...
}
```

Can now access http context info using this:
```csharp
using AspNetCoreCompatibility;

void ExampleMethod()
{
    var context = HttpContextAccessor.Current;
    // ...
}
```

In new code you can inject an IHttpContextAccessor if you register one. AspNetCoreCompatibility provides an implementation that you can use when you have a mix of old and new code.

### Threading Model

The new ASP .NET Core doesn't set up a custom
[SynchronizationContext](https://msdn.microsoft.com/en-us/library/system.threading.synchronizationcontext(v=vs.110).aspx)
whereas the old ASP .NET did. The old context prevented more than one thread
from executing at a time. This package re-enables that behavior and also
makes it possible to access the HttpContext via thread local storage.

Beware that in the Old ASP .NET this sort of thing would cause a deadlock:
```csharp
public class SampleController : Controller
{
    public ActionResult Index()
    {
        var task = DoSomethingAsync();
        return View(task.Result); // deadlock!
    }

    private async Task<SomeModel> DoSomethingAsync()
    {
        return new SomeModel
        {
            Value = await GetSomethingAsync()
        };
    }
}
```
This deadlocks because the synchronization context won't let multiple
threads run code for this request at the same time. This is important
to guarantee that things like HttpContext.Items (which is not a
thread-safe structure) aren't corrupted.