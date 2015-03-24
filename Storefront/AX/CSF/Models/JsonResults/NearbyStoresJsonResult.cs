//-----------------------------------------------------------------------
// <copyright file="NearbyStoresJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the NearbyStoresJsonResult class.</summary>
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
    using System.Collections.Generic;
    using Sitecore.Commerce.Connect.DynamicsRetail.Entities.Stores;
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;

    /// <summary>
    /// The Json result of a request to retrieve nearby store locations.
    /// </summary>
    public class NearbyStoresJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NearbyStoresJsonResult"/> class.
        /// </summary>
        public NearbyStoresJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NearbyStoresJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public NearbyStoresJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Gets or sets the nearby stores.
        /// </summary>
        public IEnumerable<Store> Stores { get; set; }

        /// <summary>
        /// Initializes the specified stores.
        /// </summary>
        /// <param name="stores">The stores.</param>
        public virtual void Initialize(IEnumerable<Store> stores)
        {
            Assert.ArgumentNotNull(stores, "stores");

            this.Stores = stores;
        }
    }
}