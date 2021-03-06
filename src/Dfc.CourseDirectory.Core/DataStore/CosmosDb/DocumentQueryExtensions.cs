﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Linq;

namespace Dfc.CourseDirectory.Core.DataStore.CosmosDb
{
    public static class DocumentQueryExtensions
    {
        public static async Task<IReadOnlyCollection<T>> FetchAll<T>(this IDocumentQuery<T> query)
        {
            var results = new List<T>();

            while (query.HasMoreResults)
            {
                var response = await query.ExecuteNextAsync<T>();

                results.AddRange(response);
            }

            return results;
        }

        public static async Task ProcessAll<T>(this IDocumentQuery<T> query, Func<IReadOnlyCollection<T>, Task> process)
        {
            while (query.HasMoreResults)
            {
                var response = await query.ExecuteNextAsync<T>();

                await process(response.ToList());
            }
        }
    }
}
