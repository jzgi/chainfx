# The Greatbone Framework  

Initially developed in Sowmob Ltd (江西雄猫), the Greatbone framework has been addressed enterprise complexicity and powered many Web applications in a concise approach.

The framwork introduces a new architectural model called contextual Web programming.

* Reliable -- based on [.NET Core](https://github.com/dotnet/core) and [PostgresSQL](https://www.postgresql.org/)
* lightweight -- 
* High performance -- Exploit async I/O, optimized caching. 
* Unified data API -- Same or similiar APIs for operating difference data formats
* Concise workflow implementation -- a couple of guys may cope with large projects 

<pre>
dotnet add package Greatbone --version 3.5.0
</pre>

# Dependencies

The framework specifically uses the following open-source libraries. Many thanks to the great teams respectively!

| ![kestrel](https://dotnet.github.io/images/Logo_DotNet.png) | ![npgsql](http://www.npgsql.org/img/logo.svg) |
| ---- | ----- |
| [kestrel](https://github.com/aspnet/KestrelHttpServer) | [npgsql](http://www.npgsql.org) |
| ASP.NET Core's built-in web server | .NET Access to PostgreSQL |
