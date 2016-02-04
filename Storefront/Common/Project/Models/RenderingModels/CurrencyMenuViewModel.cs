//---------------------------------------------------------------------
// <copyright file="CurrencyMenuViewModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Model used for currency information.</summary>
//---------------------------------------------------------------------
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

namespace Sitecore.Reference.Storefront.Models.RenderingModels
{
    using Sitecore.Commerce.Services.Prices;
    using Sitecore.Mvc.Presentation;
    using Sitecore.Reference.Storefront.Managers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the CurrencyMenuViewModel class.
    /// </summary>
    public class CurrencyMenuViewModel : RenderingModel
    {
        private List<CurrencyInformationModel> _currencyList = new List<CurrencyInformationModel>();

        /// <summary>
        /// Gets the currency list.
        /// </summary>
        /// <value>
        /// The currency list.
        /// </value>
        public List<CurrencyInformationModel> CurrencyList
        {
            get
            {
                return _currencyList;
            }
        }

        /// <summary>
        /// Initializes the specified rendering.
        /// </summary>
        /// <param name="rendering">The rendering.</param>
        /// <param name="result">The result.</param>
        public void Initialize(Rendering rendering, GetSupportedCurrenciesResult result)
        {
            if (!result.Success || result.Currencies == null)
            {
                return;
            }

            List<string> supportedCurrencies = StorefrontManager.CurrentStorefront.SupportedCurrencies;

            var currencies = supportedCurrencies.Intersect(result.Currencies);

            foreach (string currency in currencies)
            {
                var currencyInfoModel = StorefrontManager.GetCurrencyInformation(currency);
                if (currencyInfoModel != null)
                {
                    this._currencyList.Add(currencyInfoModel);
                }
            }
        }
    }
}
