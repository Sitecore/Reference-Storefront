//-----------------------------------------------------------------------
// <copyright file="GetShippingOptions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline responsible for returning the shipping options.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Shipping
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Connect.CommerceServer.Pipelines;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Shipping;
    using Sitecore.Commerce.Services.Shipping;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Models;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CSCatalog = CommerceServer.Core.Catalog;

    /// <summary>
    /// Defines the GetShippingOptions class.
    /// </summary>
    public class GetShippingOptions : CommercePipelineProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetShippingOptions"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public GetShippingOptions([NotNull] IEntityFactory entityFactory)
        {
            Assert.IsNotNull(entityFactory, "entityFactory");

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
            Assert.ArgumentCondition(args.Request is GetShippingOptionsRequest, "args.Request", "args.Request is GetShippingOptionsRequest");
            Assert.ArgumentCondition(args.Result is GetShippingOptionsResult, "args.Result", "args.Result is GetShippingOptionsResult");

            var request = (GetShippingOptionsRequest)args.Request;
            var result = (GetShippingOptionsResult)args.Result;

            List<ShippingOption> availableShippingOptionList = new List<ShippingOption>();
            List<ShippingOption> allShippingOptionList = new List<ShippingOption>();

            foreach (Item shippingOptionItem in this.GetShippingOptionsItem().GetChildren())
            {
                ShippingOption option = this.EntityFactory.Create<ShippingOption>("ShippingOption");

                this.TranslateToShippingOption(shippingOptionItem, option);

                bool add = option.ShippingOptionType == Models.ShippingOptionType.ElectronicDelivery && !CartCanBeEmailed(request.Cart as CommerceCart) ? false :
                           option.ShippingOptionType == Models.ShippingOptionType.DeliverItemsIndividually && request.Cart.Lines.Count <= 1 ? false : true;

                if (add)
                {
                    availableShippingOptionList.Add(option);
                }

                allShippingOptionList.Add(option);
            }

            result.ShippingOptions = new System.Collections.ObjectModel.ReadOnlyCollection<ShippingOption>(availableShippingOptionList);
            result.LineShippingPreferences = this.GetLineShippingOptions(request.Cart as CommerceCart, allShippingOptionList).AsReadOnly();
        }

        /// <summary>
        /// Translates to shipping option.
        /// </summary>
        /// <param name="shippingOptionItem">The shipping option item.</param>
        /// <param name="shippingOption">The shipping option.</param>
        protected virtual void TranslateToShippingOption(Item shippingOptionItem, ShippingOption shippingOption)
        {
            shippingOption.ExternalId = shippingOptionItem.ID.Guid.ToString();
            shippingOption.Name = shippingOptionItem.DisplayName;
            shippingOption.ShopName = this.GetShopName();
            shippingOption.ShippingOptionType = MainUtil.GetInt(shippingOptionItem[CommerceServerStorefrontConstants.KnownFieldNames.ShippingOptionValue], 0);
        }

        /// <summary>
        /// Gets the shipping option item.
        /// </summary>
        /// <returns>The shipping options item from Sitecore.</returns>
        protected virtual Item GetShippingOptionsItem()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/Commerce/Shipping Options");
        }

        /// <summary>
        /// Gets the name of the shop.
        /// </summary>
        /// <returns>The shopname.</returns>
        protected virtual string GetShopName()
        {
            // TODO: Shopname missing from the request.  Maybe OBEC should define it.
            return ((CommerceServerStorefront)StorefrontManager.CurrentStorefront).ShopName;
        }

        /// <summary>
        /// Gets the line shipping options.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <param name="allShippingOptions">All shipping options.</param>
        /// <returns>The list of line shipping options.</returns>
        protected virtual List<LineShippingOption> GetLineShippingOptions(CommerceCart cart, List<ShippingOption> allShippingOptions)
        {
            List<LineShippingOption> lineShippingOptions = new List<LineShippingOption>();

            if (cart != null && cart.Lines != null && cart.Lines.Any())
            {
                foreach (CommerceCartLine lineItem in cart.Lines)
                {
                    List<ShippingOption> shippingOptionsForLine = new List<ShippingOption>();

                    foreach (var shippingOption in allShippingOptions)
                    {
                        if (shippingOption.ShippingOptionType == Sitecore.Reference.Storefront.Connect.Models.ShippingOptionType.ShipToAddress || shippingOption.ShippingOptionType == Sitecore.Reference.Storefront.Connect.Models.ShippingOptionType.ElectronicDelivery)
                        {
                            bool add = shippingOption.ShippingOptionType == Sitecore.Reference.Storefront.Connect.Models.ShippingOptionType.ElectronicDelivery && !this.CanLineItemBeEmailed(lineItem) ? false : true;

                            if (add)
                            {
                                shippingOptionsForLine.Add(shippingOption);
                            }
                        }
                    }

                    LineShippingOption lineShippingOption = new LineShippingOption();
                    lineShippingOption.LineId = Guid.Parse(lineItem.ExternalCartLineId).ToString();
                    lineShippingOption.ShippingOptions = shippingOptionsForLine.AsReadOnly();

                    lineShippingOptions.Add(lineShippingOption);
                }
            }

            return lineShippingOptions;
        }

        /// <summary>
        /// Carts the can be emailed.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <returns>True if the full cart can be emailed, otherwise false.</returns>
        protected virtual bool CartCanBeEmailed(CommerceCart cart)
        {
            bool canBeEmailed = true;

            if (cart.Lines != null && cart.Lines.Any())
            {
                foreach (CommerceCartLine lineItem in cart.Lines)
                {
                    if (!this.CanLineItemBeEmailed(lineItem))
                    {
                        canBeEmailed = false;
                    }
                }
            }

            return canBeEmailed;
        }

        /// <summary>
        /// Determines whether this instance [can line item be emailed] the specified line item.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <returns>True if the line item can be emailed; otherwise false.</returns>
        protected virtual bool CanLineItemBeEmailed(CommerceCartLine lineItem)
        {
            bool lineItemCanBeEmailed = true;

            var product = (CommerceCartProduct)lineItem.Product;
            var repository = CommerceTypeLoader.CreateInstance<ICatalogRepository>();
            CSCatalog.Product commerceProduct = repository.GetProduct(product.ProductCatalog, product.ProductId);
            if (commerceProduct != null)
            {
                if (!commerceProduct.DefinitionName.Equals("GiftCard", StringComparison.OrdinalIgnoreCase))
                {
                    lineItemCanBeEmailed = false;
                }
            }

            return lineItemCanBeEmailed;
        }
    }
}