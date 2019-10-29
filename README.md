# The Greatbone Framework  

Initially developed in Sowmob Ltd (江西雄猫), the Greatbone framework has been addressing enterprise complexity and powering many Web applications in a compact approach.

The framwork introduces a new server-side architectural model called contextual Web programming.

* Lightweight -- About 250K in size 
* High performance -- Exploit async I/O, built-in optimized caching. 
* Concise data API -- Same or similiar APIs for operating difference data formats
* Rapid workflow implementation -- The code structure already implies workflow logics, UI can be derived accordingly 

<pre>
dotnet add package Greatbone --version 3.5.0
</pre>

# Dependencies

The framework specifically uses the following open-source libraries. Many thanks to the great teams respectively!

| ![kestrel](https://dotnet.github.io/images/Logo_DotNet.png) | ![npgsql](http://www.npgsql.org/img/logo.svg) |
| ---- | ----- |
| [kestrel](https://github.com/aspnet/AspNetCore) | [npgsql](http://www.npgsql.org) |
| ASP.NET Core's built-in Web server | .NET Access to PostgreSQL |
