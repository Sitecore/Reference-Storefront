//-----------------------------------------------------------------------
// <copyright file="SubmitVisitorOrder.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The submit visitor order pipeline processor.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Orders
{
    using System;
    using System.Linq;
    using CommerceServer.Core.Runtime.Orders;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using Sitecore.Commerce.Connect.CommerceServer.Pipelines;
    using Sitecore.Commerce.Entities.Orders;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Orders;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Models;
    using System.Text;
    using Sitecore.Data.Items;
    using System.Collections.Generic;

    /// <summary>
    /// SubmitVisitorOrder pipeline processor
    /// </summary>
    public class SubmitVisitorOrder : CommerceOrderPipelineProcessor
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.Request");
            Assert.ArgumentNotNull(args.Result, "args.Result");
            Assert.IsTrue((bool)(args.Request is SubmitVisitorOrderRequest), "args.Request is SubmitVisitorOrderRequest");
            Assert.IsTrue((bool)(args.Result is SubmitVisitorOrderResult), "args.Result is SubmitVisitorOrderResult");
            
            SubmitVisitorOrderRequest request = (SubmitVisitorOrderRequest)args.Request;
            SubmitVisitorOrderResult result = (SubmitVisitorOrderResult)args.Result;
            CartPipelineContext context = CartPipelineContext.Get(request.RequestContext);
            
            Assert.IsNotNull(context, "cartContext");

            if (((context.Basket != null) && !context.HasBasketErrors) && result.Success)
            {
                foreach (OrderForm orderForm in context.Basket.OrderForms)
                {
                    foreach (LineItem lineItem in orderForm.LineItems)
                    {
                        var cartLine = request.Cart.Lines.FirstOrDefault(l => l.ExternalCartLineId.Equals(lineItem.LineItemId.ToString("B"), StringComparison.OrdinalIgnoreCase));
                        if (cartLine == null)
                        {
                            continue;
                        }

                        // Store the image as a string since a dictionary is not serializable and causes problems in C&OM.
                        StringBuilder imageList = new StringBuilder();
                        foreach (MediaItem image in ((CustomCommerceCartLine)cartLine).Images)
                        {
                            if (image != null)
                            {
                                if (imageList.Length > 0)
                                {
                                    imageList.Append("|");
                                }

                                imageList.Append(image.ID.ToString());
                                imageList.Append(",");
                                imageList.Append(image.MediaPath);
                            }
                        }

                        lineItem["Images"] = imageList.ToString();
                    }
                }

                PurchaseOrder orderGroup = context.Basket.SaveAsOrder();
                TranslateOrderGroupToEntityRequest request2 = new TranslateOrderGroupToEntityRequest(context.UserId, context.ShopName, orderGroup);
                TranslateOrderGroupToEntityResult result2 = PipelineUtility.RunCommerceConnectPipeline<TranslateOrderGroupToEntityRequest, TranslateOrderGroupToEntityResult>("translate.orderGroupToEntity", request2);
                result.Order = result2.Cart as Order;
            }
        }
    }
}