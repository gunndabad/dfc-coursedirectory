﻿using System;
using Dfc.CourseDirectory.Core.Models;

namespace Dfc.CourseDirectory.Core.DataStore.Sql.Models
{
    public class Provider
    {
        public Guid ProviderId { get; set; }
        public int Ukprn { get; set; }
        public string ProviderName { get; set; }
        public string Alias { get; set; }
        public ProviderDisplayNameSource DisplayNameSource { get; set; }

        public string DisplayName => DisplayNameSource switch
        {
            ProviderDisplayNameSource.TradingName when HaveAlias => Alias,
            ProviderDisplayNameSource.ProviderName => ProviderName,
            _ => throw new NotSupportedException($"Unknown {nameof(DisplayNameSource)}: '{DisplayNameSource}'.")
        };

        public bool HaveAlias => !string.IsNullOrEmpty(Alias);
    }
}
