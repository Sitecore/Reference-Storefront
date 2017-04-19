//-----------------------------------------------------------------------
// <copyright file="ShippingMethodJsonResult.cs" company="Sitecore Corporation">
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

    /// <summary>
    ///  Defines the ShippingMethodJsonResult class.
    /// </summary>
    /// <seealso cref="Sitecore.Reference.Storefront.Models.JsonResults.ShippingMethodBaseJsonResult" />
    public class ShippingMethodJsonResult : ShippingMethodBaseJsonResult
    {
        /// <summary>
        /// Initializes the specified shipping method.
        /// </summary>
        /// <param name="shippingMethod">The shipping method.</param>
        public override void Initialize(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
            {
                return;
            }

            this.ExternalId = shippingMethod.ExternalId;
            this.Description = shippingMethod.Description;
            this.Name = shippingMethod.Name;
            this.ShippingOptionId = shippingMethod.ShippingOptionId;
            this.ShopName = shippingMethod.ShopName;
        }
    }
}
