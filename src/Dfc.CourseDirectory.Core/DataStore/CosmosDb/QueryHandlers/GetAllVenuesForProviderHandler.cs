﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfc.CourseDirectory.Core.DataStore.CosmosDb.Models;
using Dfc.CourseDirectory.Core.DataStore.CosmosDb.Queries;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Dfc.CourseDirectory.Core.DataStore.CosmosDb.QueryHandlers
{
    public class GetAllVenuesForProviderHandler :
        ICosmosDbQueryHandler<GetAllVenuesForProvider, IReadOnlyCollection<Venue>>
    {
        public Task<IReadOnlyCollection<Venue>> Execute(
            DocumentClient client,
            Configuration configuration,
            GetAllVenuesForProvider request)
        {
            var collectionLink = UriFactory.CreateDocumentCollectionUri(
                configuration.DatabaseId,
                configuration.VenuesCollectionName);

            var feedOptions = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            return client.CreateDocumentQuery<Venue>(collectionLink, feedOptions)
                .Where(f => f.Ukprn == request.ProviderUkprn && f.Status == 1)
                .AsDocumentQuery()
                .FetchAll();
        }
    }
}
