//-----------------------------------------------------------------------
// <copyright file="StockInfoListBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the StockInfoListBaseJsonResult class.</summary>
//-----------------------------------------------------------------------
// Copyright 2015 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

namespace Sitecore.Reference.Storefront.Models.JsonResults
{
    using System.Collections.Generic;
    using System.Linq;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;

    /// <summary>
    /// The Json result of a request to retrieve product stock information.
    /// </summary>
    public class StockInfoListBaseJsonResult : BaseJsonResult
    {
        private readonly List<StockInfoBaseJsonResult> _stockInformations = new List<StockInfoBaseJsonResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StockInfoListBaseJsonResult"/> class.
        /// </summary>
        public StockInfoListBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StockInfoListBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public StockInfoListBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Gets the stock informations.
        /// </summary>
        /// <value>
        /// The stock informations.
        /// </value>
        public List<StockInfoBaseJsonResult> StockInformations
        {
            get { return this._stockInformations; }
        }

        /// <summary>
        /// Initializes the specified stock infos.
        /// </summary>
        /// <param name="stockInformations">The stock informations.</param>
        public virtual void Initialize(IEnumerable<StockInformation> stockInformations)
        {
            Assert.ArgumentNotNull(stockInformations, "stockInformations");

            var stockInfos = stockInformations as IList<StockInformation> ?? stockInformations.ToList();
            if (!stockInfos.Any())
            {
                return;
            }

            foreach (var info in stockInfos)
            {
                var stockInfo = new StockInfoBaseJsonResult();
                stockInfo.Initialize(info);
                this._stockInformations.Add(stockInfo);
            }
        }
    }
}