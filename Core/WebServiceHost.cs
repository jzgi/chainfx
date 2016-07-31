using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Greatbone.Core
{
	public class WebServiceHost : WebHost
	{
		private List<WebService> _services;

		public WebServiceHost(IServiceCollection services, IServiceProvider provider, WebHostOptions options,
			IConfiguration config) : base(services, provider, options, config)
		{
		}

		public TService AddService<TService>(string key, Checker checker) where TService : WebService, new()
		{
			TService service = new TService()
			{
//				Key = key,
				Checker = checker
			};

			_services.Add(service);

			return service;
		}

		internal void Dispatch(HttpContent ctx)
		{
		}
	}
}