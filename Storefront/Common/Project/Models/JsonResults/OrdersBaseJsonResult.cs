//-----------------------------------------------------------------------
// <copyright file="OrdersBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the OrdersBaseJsonResult class.</summary>
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
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Entities.Orders;
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;
    using System.Collections.Generic;

    /// <summary>
    /// Json result for orders operations.
    /// </summary>
    public class OrdersBaseJsonResult : BaseJsonResult
    {
        private readonly List<OrderHeaderItemBaseJsonResult> _orders = new List<OrderHeaderItemBaseJsonResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersBaseJsonResult"/> class.
        /// </summary>
        public OrdersBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public OrdersBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }        

        /// <summary>
        /// Gets the orders.
        /// </summary>
        public List<OrderHeaderItemBaseJsonResult> Orders 
        { 
            get 
            { 
                return this._orders; 
            } 
        }

        /// <summary>
        /// Initializes the specified order headers.
        /// </summary>
        /// <param name="orderHeaders">The order headers.</param>
        public virtual void Initialize(IEnumerable<OrderHeader> orderHeaders)
        {
            Assert.ArgumentNotNull(orderHeaders, "orderHeaders");

            foreach (var orderHeader in orderHeaders)
            {
                var headerItem = CommerceTypeLoader.CreateInstance<OrderHeaderItemBaseJsonResult>(orderHeader);
                this._orders.Add(headerItem);
            }
        }
    }
}