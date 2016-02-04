//-----------------------------------------------------------------------
// <copyright file="GetProductPrices.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Pipeline is used to get product prices</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Prices
{
    using CommerceServer.Core.Catalog;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog.Pipelines;
    using Sitecore.Commerce.Entities.Prices;
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Models;
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sitecore.Commerce.Entities;

    /// <summary>
    /// Pipeline is used to get product prices
    /// </summary>
    public class GetProductPrices : PricePipelineProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetProductPrices"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public GetProductPrices([NotNull] IEntityFactory entityFactory)
        {
            Assert.ArgumentNotNull(entityFactory, "entityFactory");

            this.EntityFactory = entityFactory;
        }

        /// <summary>
        /// Gets or sets the entity factory.
        /// </summary>
        /// <value>
        /// The entity factory.
        /// </value>
        public IEntityFactory EntityFactory { get; set; }

        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.request");
            Assert.ArgumentCondition(args.Request is GetProductPricesRequest, "args.Request", "args.Request is GetProductPricesRequest");
            Assert.ArgumentCondition(args.Result is Sitecore.Commerce.Services.Prices.GetProductPricesResult, "args.Result", "args.Result is GetProductPricesResult");

            GetProductPricesRequest request = (GetProductPricesRequest)args.Request;
            Sitecore.Commerce.Services.Prices.GetProductPricesResult result = (Sitecore.Commerce.Services.Prices.GetProductPricesResult)args.Result;

            Assert.ArgumentNotNull(request.ProductCatalogName, "request.ProductCatalogName");
            Assert.ArgumentNotNull(request.ProductId, "request.ProductId");
            Assert.ArgumentNotNull(request.PriceTypeIds, "request.PriceTypeIds");

            ICatalogRepository catalogRepository = CommerceTypeLoader.CreateInstance<ICatalogRepository>();
            bool isList = request.PriceTypeIds.FirstOrDefault(x => x.Equals(PriceTypes.List, StringComparison.OrdinalIgnoreCase)) != null;
            bool isAdjusted = request.PriceTypeIds.FirstOrDefault(x => x.Equals(PriceTypes.Adjusted, StringComparison.OrdinalIgnoreCase)) != null;

            try
            {
                var product = catalogRepository.GetProductReadOnly(request.ProductCatalogName, request.ProductId);

                ExtendedCommercePrice extendedPrice = this.EntityFactory.Create<ExtendedCommercePrice>("Price");

                // BasePrice is a List price and ListPrice is Adjusted price 
                if (isList)
                {
                    if (product.HasProperty("BasePrice") && product["BasePrice"] != null)
                    {
                        extendedPrice.Amount = (product["BasePrice"] as decimal?).Value;
                    }
                    else
                    {
                        // No base price is defined, the List price is set to the actual ListPrice define in the catalog
                        extendedPrice.Amount = product.ListPrice;
                    }
                }               

                if (isAdjusted && !product.IsListPriceNull())
                {
                    extendedPrice.ListPrice = product.ListPrice;
                }

                result.Prices.Add(request.ProductId, extendedPrice);

                if (request.IncludeVariantPrices && product is ProductFamily)
                {
                    foreach (Variant variant in ((ProductFamily)product).Variants)
                    {
                        ExtendedCommercePrice variantExtendedPrice = this.EntityFactory.Create<ExtendedCommercePrice>("Price");

                        bool hasBasePrice = product.HasProperty("BasePrice");

                        if (hasBasePrice && variant["BasePriceVariant"] != null)
                        {
                            variantExtendedPrice.Amount = (variant["BasePriceVariant"] as decimal?).Value;
                        }                      

                        if (!variant.IsListPriceNull())
                        {
                            variantExtendedPrice.ListPrice = variant.ListPrice;

                            if (!hasBasePrice)
                            {
                                variantExtendedPrice.Amount = variant.ListPrice;
                            }
                        }

                        result.Prices.Add(variant.VariantId.Trim(), variantExtendedPrice);
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