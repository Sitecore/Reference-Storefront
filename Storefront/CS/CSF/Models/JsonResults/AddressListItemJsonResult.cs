//-----------------------------------------------------------------------
// <copyright file="AddressListItemJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the AddressListJsonResult class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.JsonResults
{
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Services;
    using System.Collections.Generic;

    /// <summary>
    /// Json result for list of parties operations.
    /// </summary>
    public class AddressListItemJsonResult : AddressListItemBaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressListItemJsonResult"/> class.
        /// </summary>
        public AddressListItemJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressListItemJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public AddressListItemJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }         

        /// <summary>
        /// Initializes the specified addresses.
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        /// <param name="countries">The countries.</param>
        public override void Initialize(IEnumerable<Party> addresses, Dictionary<string, string> countries)
        {
            base.Initialize(addresses, countries);

            foreach (var address in addresses)
            {
                var result = new AddressItemJsonResult();
                result.Initialize(address as CommerceParty);
                this.Addresses.Add(result);
            }           
        }
    }
}