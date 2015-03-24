//-----------------------------------------------------------------------
// <copyright file="GetShippingMethodsResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Extended GetShippingMethodsResult class which allows to return the valid shipping 
// methods on a per line item basis.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Services.Orders
{
    using System.Collections.ObjectModel;
    using Sitecore.Reference.Storefront.Connect.Models;

    /// <summary>
    /// Defines the GetShippingMethodsResult class.
    /// </summary>
    public class GetShippingMethodsResult : Sitecore.Commerce.Services.Shipping.GetShippingMethodsResult
    {
        /// <summary>
        /// Gets or sets the shipping methods per items.
        /// </summary>
        /// <value>
        /// The shipping methods per items.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ReadOnlyCollection<ShippingMethodPerItem> ShippingMethodsPerItems { get; set; }
    }
}