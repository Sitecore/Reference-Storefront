//-----------------------------------------------------------------------
// <copyright file="OrderConfirmationViewModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
//-----------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

namespace Sitecore.Reference.Storefront.Models
{
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Mvc.Presentation;
    using Sitecore.Reference.Storefront.Managers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the OrderConfirmationViewModel class.
    /// </summary>
    public class OrderConfirmationViewModel : RenderingModel
    {
        /// <summary>
        /// Gets or sets the confirmation identifier.
        /// </summary>
        /// <value>
        /// The confirmation identifier.
        /// </value>
        public string ConfirmationId { get; set; }

        /// <summary>
        /// Gets or sets the order status.
        /// </summary>
        /// <value>
        /// The order status.
        /// </value>
        public string OrderStatus { get; set; }

        /// <summary>
        /// Initializes the specified renderings.
        /// </summary>
        /// <param name="renderings">The renderings.</param>
        /// <param name="confirmationId">The confirmation identifier.</param>
        /// <param name="order">The order.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This could get extended.")]
        public void Initialize(Rendering renderings, string confirmationId, CommerceOrder order)
        {
            base.Initialize(renderings);

            this.ConfirmationId = confirmationId;
            this.OrderStatus = StorefrontManager.GetOrderStatusName(order.Status);
        }
    }
}
