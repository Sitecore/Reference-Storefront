//-----------------------------------------------------------------------
// <copyright file="TranslateEntityToOrderAddress.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline used to a party to a CS order address.</summary>
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
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Models;
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using ConnectOrderModels = Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using RefSFArguments = Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;

    /// <summary>
    /// Defines the TranslateEntityToOrderAddress class.
    /// </summary>
    public class TranslateEntityToOrderAddress : CommerceTranslateProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateEntityToOrderAddress"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public TranslateEntityToOrderAddress([NotNull] IEntityFactory entityFactory)
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
        public override void Process(Sitecore.Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.request");
            Assert.ArgumentNotNull(args.Result, "args.result");

            RefSFArguments.TranslateEntityToOrderAddressRequest request = (RefSFArguments.TranslateEntityToOrderAddressRequest)args.Request;
            Assert.ArgumentNotNull(request.SourceParty, "request.SourceParty");
            Assert.ArgumentNotNull(request.DestinationAddress, "request.DestinationAddress");

            if (request.SourceParty is ConnectOrderModels.CommerceParty)
            {
                this.TranslateCommerceParty(request.SourceParty as ConnectOrderModels.CommerceParty, request.DestinationAddress);
            }
            else if (request.SourceParty is EmailParty)
            {
                this.TranslateEmailParty(request.SourceParty as EmailParty, request.DestinationAddress);
            }
            else
            {
                this.TranslateCustomParty(request.SourceParty, request.DestinationAddress);
            }

            TranslateEntityToOrderAddressResult result = (TranslateEntityToOrderAddressResult)args.Result;

            result.Address = request.DestinationAddress;
        }

        /// <summary>
        /// Translates the specified source party.
        /// </summary>
        /// <param name="sourceParty">The source party.</param>
        /// <param name="destinationAddress">The destination address.</param>
        protected virtual void TranslateCommerceParty([NotNull] ConnectOrderModels.CommerceParty sourceParty, [NotNull] OrderAddress destinationAddress)
        {
            Assert.ArgumentNotNullOrEmpty(sourceParty.Name, "sourceParty.Name");

            destinationAddress.City = sourceParty.City;
            destinationAddress.CountryName = sourceParty.Country;
            destinationAddress.DaytimePhoneNumber = sourceParty.PhoneNumber;
            destinationAddress.Email = sourceParty.Email;
            destinationAddress.FirstName = sourceParty.FirstName;
            destinationAddress.LastName = sourceParty.LastName;
            destinationAddress.Line1 = sourceParty.Address1;
            destinationAddress.Line2 = sourceParty.Address2;
            destinationAddress.PostalCode = sourceParty.ZipPostalCode;
            destinationAddress.State = sourceParty.State;
            destinationAddress.CountryCode = sourceParty.CountryCode;
            destinationAddress.EveningPhoneNumber = sourceParty.EveningPhoneNumber;
            destinationAddress.FaxNumber = sourceParty.FaxNumber;
            destinationAddress.Name = sourceParty.Name;
            destinationAddress.Organization = sourceParty.Company;
            destinationAddress.RegionCode = sourceParty.RegionCode;
            destinationAddress.RegionName = sourceParty.RegionName;

            this.TranslateCommercePartyCustomProperties(sourceParty, destinationAddress);
        }

        /// <summary>
        /// Translates the custom properties.
        /// </summary>
        /// <param name="sourceParty">The source party.</param>
        /// <param name="destinationAddress">The destination address.</param>
        protected virtual void TranslateCommercePartyCustomProperties([NotNull] ConnectOrderModels.CommerceParty sourceParty, [NotNull] OrderAddress destinationAddress)
        {
            destinationAddress[CommerceServerStorefrontConstants.KnowWeaklyTypesProperties.PartyType] = 1;
        }

        /// <summary>
        /// Translates the email party.
        /// </summary>
        /// <param name="sourceParty">The source party.</param>
        /// <param name="destinationAddress">The destination address.</param>
        protected virtual void TranslateEmailParty([NotNull] EmailParty sourceParty, [NotNull] OrderAddress destinationAddress)
        {
            Assert.ArgumentNotNullOrEmpty(sourceParty.Name, "sourceParty.Name");

            destinationAddress.Name = sourceParty.Name;
            destinationAddress.Email = sourceParty.Email;
            destinationAddress.FirstName = sourceParty.FirstName;
            destinationAddress.LastName = sourceParty.LastName;
            destinationAddress.Organization = sourceParty.Company;
            destinationAddress[CommerceServerStorefrontConstants.KnowWeaklyTypesProperties.EmailText] = sourceParty.Text;

            this.TranslateEmailPartyCustomProperties(sourceParty, destinationAddress);
        }

        /// <summary>
        /// Translates the custom properties.
        /// </summary>
        /// <param name="sourceParty">The source party.</param>
        /// <param name="destinationAddress">The destination address.</param>
        protected virtual void TranslateEmailPartyCustomProperties([NotNull] EmailParty sourceParty, [NotNull] OrderAddress destinationAddress)
        {
            destinationAddress[CommerceServerStorefrontConstants.KnowWeaklyTypesProperties.PartyType] = 2;
        }

        /// <summary>
        /// Translates the custom party.
        /// </summary>
        /// <param name="sourceParty">The source party.</param>
        /// <param name="destinationAddress">The destination address.</param>
        protected virtual void TranslateCustomParty([NotNull] Party sourceParty, [NotNull] OrderAddress destinationAddress)
        {
        }
    }
}