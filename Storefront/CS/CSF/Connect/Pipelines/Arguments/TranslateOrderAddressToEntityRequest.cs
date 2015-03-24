//-----------------------------------------------------------------------
// <copyright file="TranslateOrderAddressToEntityRequest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Request object to translate CS Order Address to a Party.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Arguments
{
    using CommerceServer.Core.Runtime.Orders;
    using Sitecore.Commerce.Connect.CommerceServer.Pipelines;
    using Sitecore.Commerce.Entities;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the TranslateOrderAddressToEntityRequest class.
    /// </summary>
    public class TranslateOrderAddressToEntityRequest : CommerceRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateOrderAddressToEntityRequest"/> class.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="destinationParty">The destination party.</param>
        public TranslateOrderAddressToEntityRequest([NotNull] OrderAddress sourceAddress, [NotNull] Party destinationParty)
        {
            Assert.ArgumentNotNull(sourceAddress, "sourceAddress");
            Assert.ArgumentNotNull(destinationParty, "destinationParty");

            this.SourceAddress = sourceAddress;
            this.DestinationParty = destinationParty;
        }

        /// <summary>
        /// Gets or sets the destination party.
        /// </summary>
        /// <value>
        /// The destination party.
        /// </value>
        public Party DestinationParty { get; set; }

        /// <summary>
        /// Gets or sets the source address.
        /// </summary>
        /// <value>
        /// The source address.
        /// </value>
        public OrderAddress SourceAddress { get; set; }
    }
}