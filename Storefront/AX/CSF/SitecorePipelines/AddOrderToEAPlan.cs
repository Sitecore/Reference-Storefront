//-----------------------------------------------------------------------
// <copyright file="AddOrderToEAPlan.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the AddOrderToEAPlan class.</summary>
//---------------------------------------------------------------------
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

namespace Sitecore.Reference.Storefront.SitecorePipelines
{
    using Sitecore.Analytics;
    using Sitecore.Commerce.Automation.MarketingAutomation;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Orders;
    using Sitecore.Commerce.Multishop;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Orders;
    using Sitecore.Data;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Adding order to EA plan
    /// </summary>
    public class AddOrderToEAPlan : PipelineProcessor<ServicePipelineArgs>
    {
        #region Variables

        private const string DataKey = "commerce.orders";
        private readonly IEaPlanProvider _engagementPlanProvider;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddOrderToEAPlan"/> class.
        /// </summary>
        /// <param name="engagementPlanProvider">The engagement plan provider.</param>
        /// <param name="entityFactory">The entity factory.</param>
        public AddOrderToEAPlan(IEaPlanProvider engagementPlanProvider, IEntityFactory entityFactory)
        {
            Assert.ArgumentNotNull(engagementPlanProvider, "engagementPlanProvider");
            Assert.ArgumentNotNull(entityFactory, "entityFactory");
            this._engagementPlanProvider = engagementPlanProvider;
            this.EntityFactory = entityFactory;
        }

        #endregion

        #region Properties (public)
        /// <summary>
        /// Gets or sets the name of the engagement plan.
        /// </summary>       
        public string EngagementPlanName { get; set; }

        /// <summary>
        /// Gets or sets the entity factory.
        /// </summary>       
        public IEntityFactory EntityFactory { get; set; }

        /// <summary>
        /// Gets or sets the initial name of the state.
        /// </summary>       
        public string InitialStateName { get; set; }

        #endregion

        #region Methods (public. virtual)

        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.Request");
            Assert.ArgumentNotNull(args.Result, "args.Result");
            Assert.ArgumentCondition(args.Request is SubmitVisitorOrderRequest, "args.Request", "args.Request is SubmitVisitorOrderRequest");
            Assert.ArgumentCondition(args.Result is SubmitVisitorOrderResult, "args.Result", "args.Result is SubmitVisitorOrderResult");
            if (args.Result.Success)
            {
                SubmitVisitorOrderRequest request = (SubmitVisitorOrderRequest)args.Request;
                SubmitVisitorOrderResult result = (SubmitVisitorOrderResult)args.Result;
                Assert.ArgumentNotNull(result.Order, "result.Order");
                Assert.IsNotNullOrEmpty(result.Order.ShopName, "result.Order.ShopName");
                Assert.IsNotNullOrEmpty(result.Order.ExternalId, "result.Order.ExternalId");
                string shopName = request.Cart.ShopName;
                var userId = Tracker.Current.Contact.Identifiers.Identifier;
                string engagementPlanName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[] { shopName, this.EngagementPlanName });
                Tuple<ID, ID> engagementPlan = this._engagementPlanProvider.GetEaPlanId(shopName, engagementPlanName, this.InitialStateName);
                this.AddContactToPlan(userId, result.Order, engagementPlan.Item1, engagementPlan.Item2);
            }
        }

        #endregion

        #region Methods (protected)

        /// <summary>
        /// Adds the contact to plan.
        /// </summary>
        /// <param name="contactIdentifier">The contact identifier.</param>
        /// <param name="order">The order.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="initialStateId">The initial state identifier.</param>
        protected virtual void AddContactToPlan(string contactIdentifier, Order order, ID planId, ID initialStateId)
        {
            Dictionary<string, object> customDictionary = new Dictionary<string, object>();
            customDictionary.Add(order.ExternalId, order);           
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData.Add("commerce.orders", customDictionary);
            CommerceAutomationHelper.AddContactToState(contactIdentifier, planId, initialStateId, customData);
        }       

        #endregion
    }
}