﻿using System.Collections.Generic;
using Dfc.CourseDirectory.Core.DataStore.CosmosDb.Models;

namespace Dfc.CourseDirectory.Core.DataStore.CosmosDb.Queries
{
    public class GetAllFrameworks : ICosmosDbQuery<IReadOnlyCollection<Framework>>
    {
    }
}
