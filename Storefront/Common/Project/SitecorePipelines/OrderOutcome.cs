//---------------------------------------------------------------------
// <copyright file="OrderOutcome.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The order outcome regiter</summary>
//---------------------------------------------------------------------
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

namespace Sitecore.Reference.Storefront.SitecorePipelines
{
    using Newtonsoft.Json;
    using Sitecore.Analytics;
    using Sitecore.Analytics.Outcome;
    using Sitecore.Analytics.Outcome.Extensions;
    using Sitecore.Analytics.Outcome.Model;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Entities.Orders;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Orders;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The order outcome regiter
    /// </summary>
    public class OrderOutcome : PipelineProcessor<ServicePipelineArgs>
    {       
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
                if (Tracker.Current != null)
                {
                    SubmitVisitorOrderResult result = args.Result as SubmitVisitorOrderResult;
                    Order orderFromResult = result.Order;
                    Assert.ArgumentNotNull(orderFromResult, "order result");                   

                    var contactId = Guid.Empty;
                    if (Tracker.Current.Contact != null)
                    {
                        contactId = Tracker.Current.Contact.ContactId;
                    }

                    try 
                    {
                        var serializedOrder = SerializeOrder(orderFromResult);                    

                        var outcome = new ContactOutcome(ID.NewID, StorefrontConstants.KnownItemIds.ProductPurchaseOutcome, new ID(contactId));
                        outcome.CustomValues["ShopName"] = orderFromResult.ShopName;
                        outcome.CustomValues["ExternalId"] = orderFromResult.ExternalId;
                        outcome.CustomValues["Order"] = JsonConvert.SerializeObject(orderFromResult);
                        outcome.MonetaryValue = orderFromResult.Total.Amount;                      

                        Tracker.Current.RegisterContactOutcome(outcome);
                    }
                    catch (Exception ex)
                    {
                        CommerceLog.Current.Error("SubmitVisitorOrder, OrderOutcome", this, ex);
                    }           
                }
            }
        }

        /// <summary>
        /// Serializes the order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>Serialized order</returns>
        public virtual string SerializeOrder(Order order)
        {
            string serializedOrder = string.Empty;
            var lines = new List<Dictionary<string, object>>();
            foreach (var line in order.Lines)
            {
                var lineSerialized = new Dictionary<string, object> 
                {
                    { "ExternalCartLineId", line.ExternalCartLineId },
                    { "Quantity", line.Quantity },
                    { "ProductId", line.Product.ProductId },
                    { "StockStatus", line.Product.StockStatus },
                    { "Price", line.Product.Price }                    
                };

                lines.Add(lineSerialized);
            }

            serializedOrder = JsonConvert.SerializeObject(new Dictionary<string, object> 
            {
                { "ExternalId", order.ExternalId },
                { "UserId", order.UserId },
                { "ShopName", order.ShopName },
                { "OrderID", order.OrderID },
                { "Status", order.Status },
                { "Total", order.Total },  
                { "Lines", lines },                                                     
                { "Shipping", order.Shipping },                                   
                { "Parties", order.Parties }
            });

            return serializedOrder;
        }
    }
}
