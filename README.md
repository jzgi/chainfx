ChainFx (品链框架) is a web application framework for building industrial-oriented phygital platform systems.

ChainFx introduces an new incremental engineering process, that allows beginning with the abstraction and evolving into detail realization

The framework has invented types of caches and in-memory representation structures particularly for classic inductiral scenarioes: static datasets, organizational hiararchy, IoT, and so on.

<pre>
dotnet add package ChainFx --version 4.5
</pre>

A ChainFx-based application is made up of one single master-nodal system, perhaps plus a number of sub-nodal systems. 

ChainFx depends on the following open source software products: 

| ![kestrel](https://raw.githubusercontent.com/jzgi/chainfx/master/Docs/dotnet.png) | ![npgsql](https://raw.githubusercontent.com/jzgi/chainfx/master/Docs/postgresql.png) |
|-----------------------------------------------------------------------------------| ----- |
| [kestrel](https://github.com/aspnet/AspNetCore)                                   | [npgsql](http://www.npgsql.org) |
| .NET's built-in web engine                                                        | .NET access to PostgreSQL |
