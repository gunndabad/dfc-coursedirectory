﻿using System.Threading.Tasks;
using Dfc.CourseDirectory.Core.DataStore.Sql;
using Dfc.CourseDirectory.Core.DataStore.Sql.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Dfc.CourseDirectory.WebV2.SharedViews.Components
{
    public class AdminProviderContextNavViewComponent : ViewComponent
    {
        private readonly ISqlQueryDispatcher _sqlQueryDispatcher;

        public AdminProviderContextNavViewComponent(ISqlQueryDispatcher sqlQueryDispatcher)
        {
            _sqlQueryDispatcher = sqlQueryDispatcher;
        }

        public async Task<IViewComponentResult> InvokeAsync(ProviderInfo providerInfo)
        {
            var qaStatus = await _sqlQueryDispatcher.ExecuteQuery(
                new GetProviderApprenticeshipQAStatus()
                {
                    ProviderId = providerInfo.ProviderId
                });

            var vm = ProviderNavViewModel.Create(providerInfo, qaStatus);

            return View("~/SharedViews/Components/AdminProviderContextNav.cshtml", vm);
        }
    }
}
