//-----------------------------------------------------------------------
// <copyright file="AXCartBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>AX Reference Storefront specific version/overrides of the common CartBaseJsonResult.</summary>
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
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the AXCartBaseJsonResult class.
    /// </summary>
    public class AXCartBaseJsonResult : CartBaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AXCartBaseJsonResult"/> class.
        /// </summary>
        public AXCartBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AXCartBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public AXCartBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Initializes this object based on the data contained in the provided cart.
        /// </summary>
        /// <param name="cart">The cart used to initialize this object.</param>
        public override void Initialize(Commerce.Entities.Carts.Cart cart)
        {
            base.Initialize(cart);

            if (cart != null)
            {
                foreach (var adjustment in (cart.Adjustments ?? Enumerable.Empty<CartAdjustment>()))
                {
                    this.PromoCodes.Add(adjustment.Description);
                }
            }
        }
    }
}