//-----------------------------------------------------------------------
// <copyright file="RunTotalPipeline.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Overrides the CS Integration RunTotalPipeline to make sure the pipeline is called 
// even if there is no payment in the basket.</summary>
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
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the RunTotalPipeline class.
    /// </summary>
    public class RunTotalPipeline : Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines.RunTotalPipeline    
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunTotalPipeline"/> class.
        /// </summary>
        /// <param name="pipelineName">Name of the pipeline.</param>
        public RunTotalPipeline(string pipelineName)
            : base(pipelineName)
        {
        }

        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            base.Process(args);

            var cartContext = CartPipelineContext.Get(args.Request.RequestContext);
            Assert.IsNotNullOrEmpty(cartContext.UserId, "cartContext.UserId");
            Assert.IsNotNullOrEmpty(cartContext.ShopName, "cartContext.ShopName");

            if (cartContext.HasBasketErrors && !args.Result.Success)
            {
                args.Result.Success = true;
            }
        }

        /// <summary>
        /// Determines whether this instance [can run pipeline] the specified basket.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <returns>True if the piepline can be executed.  Otherwise false.</returns>
        protected override bool CanRunPipeline(CommerceServer.Core.Runtime.Orders.Basket basket)
        {
            bool canRunPipeline = true;

            foreach (OrderForm orderForm in basket.OrderForms)
            {
                foreach (LineItem lineItem in orderForm.LineItems)
                {
                    if (!this.IsValidShippingMethod(lineItem.ShippingMethodId))
                    {
                        return false;
                    }
                }
            }

            return canRunPipeline;
        }
    }
}