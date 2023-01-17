# Introduction  

ChainFx (品链框架) is a decentralized cloud framework for building blockchain-capable supply chain and digital twins systems

The keyword <i>chain</i> has three implications:
1. A built-in distributed ledger (blockchain)
2. Applications can form federated chains of nodes that are inter-operable with each other
3. Applications typically chain many organizational units and workflows together to conduct industrial business operations 

Characteristics:

* Interoperable through open standards: HTTP/1.1, OAuth 2.1, and JWT
* The built-in federated blockchain engine powers a distributed ledger, suitable for a formal credit solution like Carbon Credits or Time Bank
* A mesh topology of peer network that is free to associate & dessociate and easy to configure, with programmatic node-to-node interoperability
* Support rapid development of workflow-oriented decentralized Web apps 

<pre>
dotnet add package ChainFx --version 4.1.0
</pre>

# Dependencies

The open source software being used: 

| ![kestrel](https://raw.githubusercontent.com/jzgi/chainfx/master/Docs/netcore.jpg) | ![npgsql](https://raw.githubusercontent.com/jzgi/chainfx/master/Docs/postgresql.png) |
| ---- | ----- |
| [kestrel](https://github.com/aspnet/AspNetCore) | [npgsql](http://www.npgsql.org) |
| .NET's built-in web engine | .NET access to PostgreSQL |
