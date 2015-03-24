//-----------------------------------------------------------------------
// <copyright file="OrderHeaderItemBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the OrderHeaderItemBaseJsonResult class.</summary>
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
    using Sitecore.Commerce.Entities.Orders;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;

    /// <summary>
    /// Json result for order header operations.
    /// </summary>
    public class OrderHeaderItemBaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderHeaderItemBaseJsonResult"/> class.
        /// </summary>
        /// <param name="header">The order header.</param>
        public OrderHeaderItemBaseJsonResult(OrderHeader header)
        {
            this.ExternalId = header.ExternalId;
            this.Status = header.Status;
            this.LastModified = ((CommerceOrderHeader)header).LastModified.ToShortDateString();
            this.DetailsUrl = string.Concat(StorefrontManager.StorefrontUri("/accountmanagement/myorder"), "?id=", header.ExternalId);
            this.OrderId = header.ExternalId;
        }

        /// <summary>
        /// Gets or sets the external ID of the order header.
        /// </summary>
        public string ExternalId { get; protected set; }

        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public string OrderId { get; protected set; }

        /// <summary>
        /// Gets or sets the status of the order header.
        /// </summary>
        public string Status { get; protected set; }

        /// <summary>
        /// Gets or sets the last modified date of the order header
        /// </summary>
        public string LastModified { get; protected set; }

        /// <summary>
        /// Gets or sets the url for the order details
        /// </summary>
        public string DetailsUrl { get; protected set; }
    }
}