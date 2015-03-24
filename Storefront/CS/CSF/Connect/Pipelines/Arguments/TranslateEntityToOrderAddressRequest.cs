//-----------------------------------------------------------------------
// <copyright file="TranslateEntityToOrderAddressRequest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Request object to translate a Party entity to a CS Order Address.</summary>
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
    using Sitecore.Reference.Storefront.Connect.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the TranslateEntityToOrderAddressRequest class.
    /// </summary>
    public class TranslateEntityToOrderAddressRequest : CommerceRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateEntityToOrderAddressRequest"/> class.
        /// </summary>
        /// <param name="sourceParty">The source party.</param>
        /// <param name="destinationAddress">The destination address.</param>
        public TranslateEntityToOrderAddressRequest([NotNull] Party sourceParty, [NotNull] OrderAddress destinationAddress)
        {
            Assert.ArgumentNotNull(sourceParty, "sourceParty");
            Assert.ArgumentNotNull(destinationAddress, "destinationAddress");

            this.SourceParty = sourceParty;
            this.DestinationAddress = destinationAddress;
        }

        /// <summary>
        /// Gets or sets the source party.
        /// </summary>
        /// <value>
        /// The source party.
        /// </value>
        public Party SourceParty { get; set; }

        /// <summary>
        /// Gets or sets the destination address.
        /// </summary>
        /// <value>
        /// The destination address.
        /// </value>
        public OrderAddress DestinationAddress { get; set; }
    }
}