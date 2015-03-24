//-----------------------------------------------------------------------
// <copyright file="TranslateOrderAddressToEntity.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline used to translate a CS order address to a Party entity.</summary>
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
    using Sitecore.Commerce.Connect.CommerceServer.Pipelines;
    using Sitecore.Commerce.Entities;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using ConnectOrderModels = Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using ConnectPipelines = Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using RefSFArguments = Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;

    /// <summary>
    /// Defines the TranslateOrderAddressToEntity class.
    /// </summary>
    public class TranslateOrderAddressToEntity : CommerceTranslateProcessor
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Sitecore.Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.request");
            Assert.ArgumentNotNull(args.Result, "args.result");

            RefSFArguments.TranslateOrderAddressToEntityRequest request = (RefSFArguments.TranslateOrderAddressToEntityRequest)args.Request;
            Assert.ArgumentNotNull(request.SourceAddress, "request.SourceAddress");
            Assert.ArgumentNotNull(request.DestinationParty, "request.DestinationParty");

            if (request.DestinationParty is ConnectOrderModels.CommerceParty)
            {
                this.TranslateToCommerceParty(request.SourceAddress, request.DestinationParty as ConnectOrderModels.CommerceParty);
            }
            else if (request.DestinationParty is EmailParty)
            {
                this.TranslateToEmailParty(request.SourceAddress, request.DestinationParty as EmailParty);
            }
            else
            {
                this.TranslateToCustomParty(request.SourceAddress, request.DestinationParty);
            }
        }

        /// <summary>
        /// Translates to commerce party.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="destinationParty">The destination party.</param>
        protected virtual void TranslateToCommerceParty(CommerceServer.Core.Runtime.Orders.OrderAddress sourceAddress, ConnectOrderModels.CommerceParty destinationParty)
        {
            destinationParty.ExternalId = sourceAddress.OrderAddressId;
            destinationParty.City = sourceAddress.City;
            destinationParty.Country = sourceAddress.CountryName;
            destinationParty.CountryCode = sourceAddress.CountryCode;
            destinationParty.PhoneNumber = sourceAddress.DaytimePhoneNumber;
            destinationParty.Email = sourceAddress.Email;
            destinationParty.FirstName = sourceAddress.FirstName;
            destinationParty.LastName = sourceAddress.LastName;
            destinationParty.Address1 = sourceAddress.Line1;
            destinationParty.Address2 = sourceAddress.Line2;
            destinationParty.ZipPostalCode = sourceAddress.PostalCode;
            destinationParty.State = sourceAddress.State;
            destinationParty.EveningPhoneNumber = sourceAddress.EveningPhoneNumber;
            destinationParty.FaxNumber = sourceAddress.FaxNumber;
            destinationParty.Name = sourceAddress.Name;
            destinationParty.Company = sourceAddress.Organization;
            destinationParty.RegionCode = sourceAddress.RegionCode;
            destinationParty.RegionName = sourceAddress.RegionName;

            this.TranslateToCustomAddressProperties(sourceAddress, destinationParty);
        }

        /// <summary>
        /// Translates to custom address properties.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="destinationParty">The destination party.</param>
        protected virtual void TranslateToCustomAddressProperties([NotNull] CommerceServer.Core.Runtime.Orders.OrderAddress sourceAddress, [NotNull] ConnectOrderModels.CommerceParty destinationParty)
        {
        }

        /// <summary>
        /// Translates to email party.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="destinationParty">The destination party.</param>
        protected virtual void TranslateToEmailParty(CommerceServer.Core.Runtime.Orders.OrderAddress sourceAddress, EmailParty destinationParty)
        {
            destinationParty.ExternalId = sourceAddress.OrderAddressId;
            destinationParty.Name = sourceAddress.Name;
            destinationParty.Email = sourceAddress.Email;
            destinationParty.FirstName = sourceAddress.FirstName;
            destinationParty.LastName = sourceAddress.LastName;
            destinationParty.Company = sourceAddress.Organization;
            destinationParty.Text = sourceAddress[CommerceServerStorefrontConstants.KnowWeaklyTypesProperties.EmailText] as string;

            this.TranslateToCustomAddressProperties(sourceAddress, destinationParty);
        }

        /// <summary>
        /// Translates to custom address properties.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="destinationParty">The destination party.</param>
        protected virtual void TranslateToCustomAddressProperties([NotNull] CommerceServer.Core.Runtime.Orders.OrderAddress sourceAddress, [NotNull] EmailParty destinationParty)
        {
        }

        /// <summary>
        /// Translates to custom party.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="destinationParty">The destination party.</param>
        protected virtual void TranslateToCustomParty(CommerceServer.Core.Runtime.Orders.OrderAddress sourceAddress, Party destinationParty)
        {
        }
    }
}