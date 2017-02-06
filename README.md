# Core

As in 1990's, dBase had let many people become highly productive programmers, by its simple working environment and powerful data operability.

We're aiming similar goals to create a framework for building microservices, that is
* reliable -- based on [.NET Core](https://github.com/dotnet/core) and [PostgresSQL](https://www.postgresql.org/)
* lightweight while battery-included -- live well without thridparties such as Radis or Kafka
* high-performance -- esp on multi-core boxes
* data handling and operation are unified and direct -- no extra layers
* much less coding -- a couple of guys may cope with large applications 
* versatile -- good for from small management systems to large-scale e-commerce.

# Dependencies

The framework specifically uses the following open-source libraries. Many thanks to the great teams respectively!

| ![kestrel](https://dotnetfoundation.org/Themes/DotNetFoundation.Theme/Images/logo-small.png) | ![npgsql](http://www.npgsql.org/img/logo.svg) |
| ---- | ----- |
| [kestrel](https://github.com/aspnet/KestrelHttpServer) | [npgsql](http://www.npgsql.org) |
| ASP.NET Core's built-in web server | .NET Access to PostgreSQL |
