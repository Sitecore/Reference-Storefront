//-----------------------------------------------------------------------
// <copyright file="GetProductPrices.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline is used to get product prices</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Prices
{
    using CommerceServer.Core.Catalog;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog.Pipelines;
    using Sitecore.Commerce.Entities.Prices;
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Pipeline is used to get product prices
    /// </summary>
    public class GetProductPrices : PricePipelineProcessor
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.request");
            Assert.ArgumentCondition(args.Request is GetProductPricesRequest, "args.Request", "args.Request is GetProductPricesRequest");
            Assert.ArgumentCondition(args.Result is GetProductPricesResult, "args.Result", "args.Result is GetProductPricesResult");

            GetProductPricesRequest request = (GetProductPricesRequest)args.Request;
            GetProductPricesResult result = (GetProductPricesResult)args.Result;

            Assert.ArgumentNotNull(request.ProductCatalogName, "request.ProductCatalogName");
            Assert.ArgumentNotNull(request.ProductId, "request.ProductId");
            Assert.ArgumentNotNull(request.PriceTypeIds, "request.PriceTypeIds");

            ICatalogRepository catalogRepository = CommerceTypeLoader.CreateInstance<ICatalogRepository>();
            bool isList = Array.FindIndex(request.PriceTypeIds as string[], t => t.IndexOf("List", StringComparison.OrdinalIgnoreCase) >= 0) > -1;
            bool isAdjusted = Array.FindIndex(request.PriceTypeIds as string[], t => t.IndexOf("Adjusted", StringComparison.OrdinalIgnoreCase) >= 0) > -1;

            try
            {
                var product = catalogRepository.GetProductReadOnly(request.ProductCatalogName, request.ProductId);
                Dictionary<string, Price> prices = new Dictionary<string, Price>();

                // BasePrice is a List price and ListPrice is Adjusted price 
                if (isList)
                {
                    if (product.HasProperty("BasePrice") && product["BasePrice"] != null)
                    {
                        prices.Add("List", new Price { PriceType = "List", Amount = (product["BasePrice"] as decimal?).Value });
                    }
                    else
                    {
                        // No base price is defined, the List price is set to the actual ListPrice define in the catalog
                        prices.Add("List", new Price { PriceType = "List", Amount = product.ListPrice });
                    }
                }               

                if (isAdjusted && !product.IsListPriceNull())
                {
                    prices.Add("Adjusted", new Price { PriceType = "Adjusted", Amount = product.ListPrice });
                } 

                result.Prices.Add(request.ProductId, prices);
                if (request.IncludeVariantPrices && product is ProductFamily)
                {
                    foreach (Variant variant in ((ProductFamily)product).Variants)
                    {
                        Dictionary<string, Price> variantPrices = new Dictionary<string, Price>();
                        if (isList && product.HasProperty("BasePrice") && variant["BasePriceVariant"] != null)
                        {
                            variantPrices.Add("List", new Price { PriceType = "List", Amount = (variant["BasePriceVariant"] as decimal?).Value });
                        }                      

                        if (isAdjusted && !variant.IsListPriceNull())
                        {
                            variantPrices.Add("Adjusted", new Price { PriceType = "Adjusted", Amount = variant.ListPrice });
                        }

                        result.Prices.Add(variant.VariantId, variantPrices);
                    }
                }
            }
            catch (CommerceServer.Core.EntityDoesNotExistException e)
            {
                result.Success = false;
                result.SystemMessages.Add(new SystemMessage { Message = e.Message });
            }
        }
    }
}