//-----------------------------------------------------------------------
// <copyright file="GetSupportedCurrencies.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Processor used to return the supported catalog currency.</summary>
//-----------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Prices
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog.Pipelines;
    using Sitecore.Commerce.Services.Prices;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using RefSFArgs = Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;

    /// <summary>
    /// Defines the GetSupportedCurrencies class.
    /// </summary>
    public class GetSupportedCurrencies : PricePipelineProcessor
    {
        private string _currencyToInjextString;
        private List<string> _currenciesToInject = new List<string>();

        /// <summary>
        /// Gets or sets the currencies to inject in the list returned. Use primarily as a debuging tool.
        /// </summary>
        /// <value>
        /// The inject currencies.
        /// </value>
        public string InjectCurrencies
        {
            get
            {
                return this._currencyToInjextString;
            }

            set
            {
                value.Split(',').ToList().ForEach(x => this._currenciesToInject.Add(x.Trim()));
                this._currencyToInjextString = value;
            }
        }

        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.request");
            Assert.ArgumentCondition(args.Request is RefSFArgs.GetSupportedCurrenciesRequest, "args.Request", "args.Request is RefSFArgs.GetSupportedCurrenciesRequest");
            Assert.ArgumentCondition(args.Result is GetSupportedCurrenciesResult, "args.Result", "args.Result is GetSupportedCurrenciesResult");

            var request = (RefSFArgs.GetSupportedCurrenciesRequest)args.Request;
            var result = (GetSupportedCurrenciesResult)args.Result;

            Assert.ArgumentNotNullOrEmpty(request.CatalogName, "request.CatalogName");

            ICatalogRepository catalogRepository = CommerceTypeLoader.CreateInstance<ICatalogRepository>();

            var catalog = catalogRepository.GetCatalogReadOnly(request.CatalogName);

            List<string> currencyList = new List<string>();

            currencyList.Add(catalog["Currency"].ToString());

            if (this._currenciesToInject.Count > 0)
            {
                currencyList.AddRange(this._currenciesToInject);
            }

            result.Currencies = new System.Collections.ObjectModel.ReadOnlyCollection<string>(currencyList);
        }
    }
}