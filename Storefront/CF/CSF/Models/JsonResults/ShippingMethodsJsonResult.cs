//-----------------------------------------------------------------------
// <copyright file="ShippingMethodsJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the ShippingMethodsJsonResult class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.JsonResults
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Entities.Shipping;
    using Sitecore.Commerce.Services.Shipping;

    /// <summary>
    /// The Json result of a request to retrieve nearby store locations.
    /// </summary>
    /// <seealso cref="Sitecore.Reference.Storefront.Models.JsonResults.ShippingMethodsBaseJsonResult" />
    public class ShippingMethodsJsonResult : ShippingMethodsBaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingMethodsJsonResult"/> class.
        /// </summary>
        public ShippingMethodsJsonResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingMethodsJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public ShippingMethodsJsonResult(GetShippingMethodsResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Gets or sets the available line item shipping methods.
        /// </summary>
        public IEnumerable<ShippingMethodPerItemBaseJsonResult> LineShippingMethods { get; set; }

        /// <summary>
        /// Initializes the specified shipping methods.
        /// </summary>
        /// <param name="shippingMethods">The shipping methods.</param>
        /// <param name="shippingMethodsPerItem">The shipping methods per item.</param>
        public virtual void Initialize(IEnumerable<ShippingMethod> shippingMethods, IEnumerable<ShippingMethodPerItem> shippingMethodsPerItem)
        {
            base.Initialize(shippingMethods);

            if (shippingMethodsPerItem != null && shippingMethodsPerItem.Any())
            {
                var lineShippingMethodList = new List<ShippingMethodPerItemBaseJsonResult>();

                foreach (var shippingMethodPerItem in shippingMethodsPerItem)
                {
                    var jsonResult = CommerceTypeLoader.CreateInstance<ShippingMethodPerItemBaseJsonResult>();

                    jsonResult.Initialize(shippingMethodPerItem);

                    lineShippingMethodList.Add(jsonResult);
                }

                this.LineShippingMethods = lineShippingMethodList;
            }
        }
    }
}