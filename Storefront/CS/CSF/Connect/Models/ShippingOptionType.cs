//-----------------------------------------------------------------------
// <copyright file="ShippingOptionType.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Extesible shipping option type.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Models
{
    using System;

    /// <summary>
    /// Shipping option type enum
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1052:StaticHolderTypesShouldBeSealed", Justification = "Code will be moved to OBEC"), Serializable]
    public class ShippingOptionType : Sitecore.Commerce.Entities.Shipping.ShippingOptionType
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="ShippingOptionType"/> class from being created.
        /// </summary>
        private ShippingOptionType()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingOptionType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code will be moved to OBEC")]
        private ShippingOptionType(int value, string name)
            : base(value, name)
        {
        }

        /// <summary>
        /// Gets the deliver items individually.
        /// </summary>
        /// <value>
        /// The deliver items individually.
        /// </value>
        public static Commerce.Entities.Shipping.ShippingOptionType DeliverItemsIndividually
        {
            get
            {
                return new Commerce.Entities.Shipping.ShippingOptionType(4, "DeliverItemsIndividually");
            }
        }
    }
}