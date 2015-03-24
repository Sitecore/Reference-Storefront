//-----------------------------------------------------------------------
// <copyright file="TranslateOrderGroupToEntity.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>CS Pipeline override to handle the EmailParty entity.</summary>
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
    using Sitecore.Commerce.Connect.CommerceServer.Pipelines;
    using Sitecore.Commerce.Entities;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using ConnectOrdersPipelines = Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using RefSFArguments = Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using RefSFModels = Sitecore.Reference.Storefront.Connect.Models;

    /// <summary>
    /// Defines the TranslateOrderGroupToEntity class.
    /// </summary>
    public class TranslateOrderGroupToEntity : ConnectOrdersPipelines.TranslateOrderGroupToEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateOrderGroupToEntity"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public TranslateOrderGroupToEntity([NotNull]IEntityFactory entityFactory)
            : base(entityFactory)
        {
        }

        /// <summary>
        /// Translates the addresses.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="cart">The cart.</param>
        protected override void TranslateAddresses(CommerceServer.Core.Runtime.Orders.OrderAddressCollection collection, Commerce.Entities.Carts.Cart cart)
        {
            List<Party> partyList = new List<Party>();

            foreach (OrderAddress commerceAddress in collection)
            {
                int partyType = (commerceAddress[CommerceServerStorefrontConstants.KnowWeaklyTypesProperties.PartyType] == null) ? 1 : Convert.ToInt32(commerceAddress[CommerceServerStorefrontConstants.KnowWeaklyTypesProperties.PartyType], CultureInfo.InvariantCulture);

                Party party = null;

                switch (partyType)
                {
                    default:
                    case 1:
                        party = this.EntityFactory.Create<Commerce.Connect.CommerceServer.Orders.Models.CommerceParty>("Party");
                        this.TranslateAddress(commerceAddress, party as Sitecore.Commerce.Connect.CommerceServer.Orders.Models.CommerceParty);
                        break;

                    case 2:
                        party = this.EntityFactory.Create<RefSFModels.EmailParty>("EmailParty");
                        this.TranslateAddress(commerceAddress, party as RefSFModels.EmailParty);
                        break;
                }

                partyList.Add(party);
            }

            cart.Parties = partyList.AsReadOnly();
        }

        /// <summary>
        /// Translates the address.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="destinationParty">The destination party.</param>
        protected override void TranslateAddress(CommerceServer.Core.Runtime.Orders.OrderAddress sourceAddress, Commerce.Connect.CommerceServer.Orders.Models.CommerceParty destinationParty)
        {
            RefSFArguments.TranslateOrderAddressToEntityRequest request = new RefSFArguments.TranslateOrderAddressToEntityRequest(sourceAddress, destinationParty);
            PipelineUtility.RunCommerceConnectPipeline<RefSFArguments.TranslateOrderAddressToEntityRequest, CommerceResult>(PipelineNames.TranslateOrderAddressToEntity, request);
        }

        /// <summary>
        /// Translates the address.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="destinationParty">The destination party.</param>
        protected virtual void TranslateAddress(CommerceServer.Core.Runtime.Orders.OrderAddress sourceAddress, RefSFModels.EmailParty destinationParty)
        {
            RefSFArguments.TranslateOrderAddressToEntityRequest request = new RefSFArguments.TranslateOrderAddressToEntityRequest(sourceAddress, destinationParty);
            PipelineUtility.RunCommerceConnectPipeline<RefSFArguments.TranslateOrderAddressToEntityRequest, CommerceResult>(PipelineNames.TranslateOrderAddressToEntity, request);
        }
    }
}