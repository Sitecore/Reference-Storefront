//-----------------------------------------------------------------------
// <copyright file="RemovePartiesFromCart.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline used to remove parties to an existing cart.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Carts
{
    using CommerceServer.Core.Runtime.Orders;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using Sitecore.Commerce.Connect.CommerceServer.Pipelines;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the RemovePartiesFromCart class.
    /// </summary>
    public class RemovePartiesFromCart : CommerceCartPipelineProcessor
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Sitecore.Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.request");

            RemovePartiesRequest request = (RemovePartiesRequest)args.Request;
            RemovePartiesResult result = (RemovePartiesResult)args.Result;

            var cartContext = CartPipelineContext.Get(request.RequestContext);
            Assert.IsNotNull(cartContext, "cartContext");

            List<Party> partiesRemoved = new List<Party>();

            if (cartContext.Basket != null)
            {
                foreach (Party party in request.Parties)
                {
                    if (party != null)
                    {
                        Assert.ArgumentNotNullOrEmpty(party.ExternalId, "party.ExternalId");

                        OrderAddress orderAddress = cartContext.Basket.Addresses[party.ExternalId];
                        if (orderAddress != null)
                        {
                            cartContext.Basket.Addresses.Remove(orderAddress);

                            partiesRemoved.Add(party);
                        }
                    }
                }
            }

            result.Parties = partiesRemoved;

            // Needed by the RunSaveCart CommerceConnect pipeline.
            var translateCartRequest = new TranslateOrderGroupToEntityRequest(cartContext.UserId, cartContext.ShopName, cartContext.Basket);
            var translateCartResult = PipelineUtility.RunCommerceConnectPipeline<TranslateOrderGroupToEntityRequest, TranslateOrderGroupToEntityResult>(PipelineNames.TranslateOrderGroupToEntity, translateCartRequest);

            result.Cart = translateCartResult.Cart;
        }
    }
}