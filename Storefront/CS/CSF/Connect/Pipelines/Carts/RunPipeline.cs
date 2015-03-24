//-----------------------------------------------------------------------
// <copyright file="RunPipeline.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Override of the CS Integration RunPipeline pipeline processor.  No longer generates a failed
// operation when basket error are detected.  This was required for when we remove valid promo codes from the basket.</summary>
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
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the RunPipeline class.
    /// </summary>
    public class RunPipeline : Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines.RunPipeline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunPipeline"/> class.
        /// </summary>
        /// <param name="pipelineName">Name of the pipeline.</param>
        public RunPipeline(string pipelineName) : base(pipelineName)
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
    }
}