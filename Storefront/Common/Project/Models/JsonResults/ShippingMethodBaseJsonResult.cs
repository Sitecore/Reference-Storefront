//-----------------------------------------------------------------------
// <copyright file="ShippingMethodBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Shipping method JSON result.</summary>
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
    using Sitecore.Commerce.Entities.Shipping;
    using Sitecore.Reference.Storefront.Managers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the ShippingMethodBaseJsonResult class.
    /// </summary>
    /// <seealso cref="Sitecore.Reference.Storefront.Models.JsonResults.BaseJsonResult" />
    public class ShippingMethodBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingMethodBaseJsonResult"/> class.
        /// </summary>
        public ShippingMethodBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>
        /// The external identifier.
        /// </value>
        public string ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the shipping option identifier.
        /// </summary>
        /// <value>
        /// The shipping option identifier.
        /// </value>
        public string ShippingOptionId { get; set; }

        /// <summary>
        /// Gets or sets the name of the shop.
        /// </summary>
        /// <value>
        /// The name of the shop.
        /// </value>
        public string ShopName { get; set; }

        /// <summary>
        /// Initializes the specified shipping method.
        /// </summary>
        /// <param name="shippingMethod">The shipping method.</param>
        public virtual void Initialize(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
            {
                return;
            }

            this.ExternalId = shippingMethod.ExternalId;
            this.Description = StorefrontManager.GetShippingName(shippingMethod.Description);
            this.Name = shippingMethod.Name;
            this.ShippingOptionId = shippingMethod.ShippingOptionId;
            this.ShopName = shippingMethod.ShopName;
        }
    }
}
