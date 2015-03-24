//-----------------------------------------------------------------------
// <copyright file="StoreManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The manager class responsible for encapsulating the store business logic for the site.</summary>
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

namespace Sitecore.Reference.Storefront.Managers
{
    using Sitecore.Commerce.Connect.DynamicsRetail.Entities.Stores;
    using Sitecore.Commerce.Connect.DynamicsRetail.Services.Stores;
    using Sitecore.Reference.Storefront.Models.InputModels;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using Sitecore.Diagnostics;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Defines the StoreManager class.
    /// </summary>
    public class StoreManager : BaseManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreManager"/> class.
        /// </summary>
        /// <param name="storeServiceProvider">The store service provider.</param>
        public StoreManager([NotNull] StoreServiceProvider storeServiceProvider)
        {
            Assert.ArgumentNotNull(storeServiceProvider, "storeServiceProvider");

            this.StoreServiceProvider = storeServiceProvider;
        }

        /// <summary>
        /// Gets or sets the store service provider.
        /// </summary>
        /// <value>
        /// The store service provider.
        /// </value>
        public StoreServiceProvider StoreServiceProvider { get; protected set; }

        /// <summary>
        /// Gets the near by stores.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// The manager response where the stores are returned in the Result.
        /// </returns>
        public ManagerResponse<GetNearbyStoresResult, IReadOnlyCollection<Store>> GetNearbyStores([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] GetNearbyStoresInputModel inputModel)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(inputModel, "inputModel");
            Assert.ArgumentNotNullOrEmpty(inputModel.Latitude, "inputModel.Latitude");
            Assert.ArgumentNotNullOrEmpty(inputModel.Longitude, "inputModel.Longitude");

            var request = new GetNearbyStoresRequest(System.Convert.ToDecimal(inputModel.Latitude, CultureInfo.InvariantCulture), System.Convert.ToDecimal(inputModel.Longitude, CultureInfo.InvariantCulture), 10);
            var result = this.StoreServiceProvider.GetNearbyStores(request);

            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<GetNearbyStoresResult, IReadOnlyCollection<Store>>(result, result.Stores);
        }
    }
}