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

