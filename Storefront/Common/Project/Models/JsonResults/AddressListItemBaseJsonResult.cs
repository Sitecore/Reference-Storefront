//-----------------------------------------------------------------------
// <copyright file="AddressListItemBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the AddressListItemBaseJsonResult class.</summary>
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
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;
    using Sitecore.Mvc.Extensions;
    using System.Collections.Generic;

    /// <summary>
    /// Json result for list of parties operations.
    /// </summary>
    public class AddressListItemBaseJsonResult : BaseJsonResult
    {
        private readonly List<AddressItemBaseJsonResult> _addresses = new List<AddressItemBaseJsonResult>();
        private readonly Dictionary<string, string> _countries = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressListItemBaseJsonResult"/> class.
        /// </summary>
        public AddressListItemBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressListItemBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public AddressListItemBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }       

        /// <summary>
        /// Gets the list of addresses.
        /// </summary>
        public List<AddressItemBaseJsonResult> Addresses 
        { 
            get 
            { 
                return this._addresses; 
            } 
        }

        /// <summary>
        /// Gets the available countries.
        /// </summary>
        public Dictionary<string, string> Countries 
        { 
            get 
            {
                return this._countries; 
            } 
        }

        /// <summary>
        /// Initializes the specified addresses.
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        /// <param name="countries">The countries.</param>
        public virtual void Initialize(IEnumerable<Party> addresses, Dictionary<string, string> countries)
        {
            Assert.ArgumentNotNull(addresses, "addresses");            

            if (countries != null && countries.Count > 0)
            {
                this.Countries.AddRange(countries);
            }
        }
    }
}