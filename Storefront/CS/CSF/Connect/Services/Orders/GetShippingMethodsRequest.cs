//-----------------------------------------------------------------------
// <copyright file="GetShippingMethodsRequest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Extended GetShippingMethodsRequest class which allows to pass in Cart and Line 
// information which is used to determine what are the valid shipping methods.</summary>
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
    using System.Collections.Generic;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Entities.Shipping;

    /// <summary>
    /// Defines the GetShippingMethodsRequest class.
    /// </summary>
    public class GetShippingMethodsRequest : CommerceGetShippingMethodsRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetShippingMethodsRequest"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
        public GetShippingMethodsRequest(string language) : this(language, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetShippingMethodsRequest"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="shippingOption">The shipping option.</param>
        /// <param name="party">The party.</param>
        public GetShippingMethodsRequest(string language, ShippingOption shippingOption, Party party = null)
            : base(language, shippingOption, party)
        {
        }

        /// <summary>
        /// Gets or sets the cart.
        /// </summary>
        /// <value>
        /// The cart.
        /// </value>
        public Cart Cart { get; set; }

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>
        /// The lines.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<CartLine> Lines { get; set; }
    }
}