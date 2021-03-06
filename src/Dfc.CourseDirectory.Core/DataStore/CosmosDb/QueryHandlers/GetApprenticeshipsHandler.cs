﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfc.CourseDirectory.Core.DataStore.CosmosDb.Models;
using Dfc.CourseDirectory.Core.DataStore.CosmosDb.Queries;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Dfc.CourseDirectory.Core.DataStore.CosmosDb.QueryHandlers
{
    public class GetApprenticeshipsHandler : ICosmosDbQueryHandler<GetApprenticeships, IDictionary<Guid, Apprenticeship>>
    {
        public async Task<IDictionary<Guid, Apprenticeship>> Execute(DocumentClient client, Configuration configuration, GetApprenticeships request)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                 configuration.DatabaseId,
                 configuration.ApprenticeshipCollectionName);

            var query = client.CreateDocumentQuery<Apprenticeship>(collectionUri, new FeedOptions() { EnableCrossPartitionQuery = true })
                .Where(p => p.RecordStatus != 4 && p.RecordStatus != 8);

            if (request.Predicate != null)
            {
                query = query.Where(request.Predicate);
            }

            var results = await query.AsDocumentQuery().FetchAll();
            var resultsDict = results.ToDictionary(r => r.Id, r => r);

            return resultsDict;
        }
    }
}
