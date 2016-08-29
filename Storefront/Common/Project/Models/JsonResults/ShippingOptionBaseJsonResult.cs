//-----------------------------------------------------------------------
// <copyright file="ShippingOptionBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Shipping option JSON result.</summary>
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
    /// Defines the ShippingOptionBaseJsonResult class.
    /// </summary>
    public class ShippingOptionBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingOptionBaseJsonResult"/> class.
        /// </summary>
        public ShippingOptionBaseJsonResult()
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
        /// Gets or sets the type of the shipping option.
        /// </summary>
        /// <value>
        /// The type of the shipping option.
        /// </value>
        public Sitecore.Commerce.Entities.Shipping.ShippingOptionType ShippingOptionType { get; set; }

        /// <summary>
        /// Gets or sets the name of the shop.
        /// </summary>
        /// <value>
        /// The name of the shop.
        /// </value>
        public string ShopName { get; set; }

        /// <summary>
        /// Initializes the specified shipping option.
        /// </summary>
        /// <param name="shippingOption">The shipping option.</param>
        public virtual void Initialize(ShippingOption shippingOption)
        {
            if (shippingOption == null)
            {
                return;
            }

            this.ExternalId = shippingOption.ExternalId;
            this.Description = shippingOption.Description;
            this.Name = StorefrontManager.GetShippingName(shippingOption.Name);
            this.Description = shippingOption.Description;
            this.ShippingOptionType = shippingOption.ShippingOptionType;
            this.ShopName = shippingOption.ShopName;
        }
    }
}
