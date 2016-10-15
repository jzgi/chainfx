using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Greatbone.Core

{
    public class WebCall
    {

        // request
        HttpRequestMessage request;

        HttpContent reqcon;

        Uri uri;

        HttpMethod method;


        // Get Post,  uri,  


        // response
        HttpResponseMessage response;

        HttpContent con;

        HttpResponseHeaders readers;

    }
}