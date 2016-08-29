//-----------------------------------------------------------------------
// <copyright file="ShippingMethodPerItemBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Shipping method per item JSON result.</summary>
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
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Entities.Shipping;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the ShippingMethodPerItemBaseJsonResult class.
    /// </summary>
    /// <seealso cref="Sitecore.Reference.Storefront.Models.JsonResults.BaseJsonResult" />
    public class ShippingMethodPerItemBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingMethodPerItemBaseJsonResult"/> class.
        /// </summary>
        public ShippingMethodPerItemBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the line identifier.
        /// </summary>
        /// <value>
        /// The line identifier.
        /// </value>
        public string LineId { get; set; }

        /// <summary>
        /// Gets or sets the shipping methods.
        /// </summary>
        /// <value>
        /// The shipping methods.
        /// </value>
        public IEnumerable<ShippingMethodBaseJsonResult> ShippingMethods { get; set; }

        /// <summary>
        /// Initilaizes the specified shipping method per item.
        /// </summary>
        /// <param name="shippingMethodPerItem">The shipping method per item.</param>
        public virtual void Initialize(ShippingMethodPerItem shippingMethodPerItem)
        {
            if (shippingMethodPerItem == null)
            {
                return;
            }

            this.LineId = shippingMethodPerItem.LineId;

            if (shippingMethodPerItem.ShippingMethods != null && shippingMethodPerItem.ShippingMethods.Any())
            {
                var shippingMethodList = new List<ShippingMethodBaseJsonResult>();

                foreach (var shippingMethod in shippingMethodPerItem.ShippingMethods)
                {
                    var jsonResult = CommerceTypeLoader.CreateInstance<ShippingMethodBaseJsonResult>();

                    jsonResult.Initialize(shippingMethod);
                    shippingMethodList.Add(jsonResult);
                }

                this.ShippingMethods = shippingMethodList;
            }
        }
    }
}
