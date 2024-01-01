using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using Elasticsearch.Net;
using Nest;

namespace OneSolution_API.Models.Utils
{
    public class ElasticSearchConnect
    {
       
        public ElasticClient client;


        public ElasticSearchConnect()
        {
            Uri  node = new Uri(Utils.elastic_url);

            var settings = new ConnectionSettings(
                node
            );

            client = new ElasticClient(settings);
        }

        

    }
}