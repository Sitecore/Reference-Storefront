//-----------------------------------------------------------------------
// <copyright file="AddParties.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline processor responsible for adding parties (addresses) to a CS user profile.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Customers
{
    using CommerceServer.Core.Runtime.Profiles;
    using Sitecore.Commerce.Connect.CommerceServer.Pipelines;
    using Sitecore.Commerce.Connect.CommerceServer.Profiles.Models;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Services.Customers;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using System.Collections.Generic;
    using System.Linq;
    using RefSFModels = Sitecore.Reference.Storefront.Connect.Models;

    /// <summary>
    /// Defines the AddParties class.
    /// </summary>
    public class AddParties : CustomerPipelineProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddParties"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public AddParties([NotNull] IEntityFactory entityFactory)
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
            Assert.ArgumentCondition(args.Request is AddPartiesRequest, "args.Request", "args.Request is AddPartiesRequest");
            Assert.ArgumentCondition(args.Result is AddPartiesResult, "args.Result", "args.Result is AddPartiesResult");

            var request = (AddPartiesRequest)args.Request;
            var result = (AddPartiesResult)args.Result;
            Assert.ArgumentNotNull(request.Parties, "request.Parties");
            Assert.ArgumentNotNull(request.CommerceCustomer, "request.CommerceCustomer");

            Profile customerProfile = null;
            var response = this.GetCommerceUserProfile(request.CommerceCustomer.ExternalId, ref customerProfile);
            if (!response.Success)
            {
                result.Success = false;
                response.SystemMessages.ToList().ForEach(m => result.SystemMessages.Add(m));
                return;
            }

            List<Party> partiesAdded = new List<Party>();

            foreach (Party party in request.Parties)
            {
                if (party == null)
                {
                    continue;
                }

                Party newParty = null;

                if (party is RefSFModels.CommerceParty)
                {
                    newParty = this.ProcessCommerceParty(result, customerProfile, party as RefSFModels.CommerceParty);
                }
                else
                {
                    newParty = this.ProcessCustomParty(result, customerProfile, party);
                }

                if (newParty != null)
                {
                    partiesAdded.Add(newParty);
                }
            }
        }

        /// <summary>
        /// Processes the commerce party.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="customerProfile">The customer profile.</param>
        /// <param name="partyToAdd">The party to add.</param>
        /// <returns>Newly created party.</returns>
        protected virtual Party ProcessCommerceParty(AddPartiesResult result, Profile customerProfile, RefSFModels.CommerceParty partyToAdd)
        {
            Assert.ArgumentNotNull(partyToAdd.Name, "partyToAdd.Name");
            Assert.ArgumentNotNull(partyToAdd.ExternalId, "partyToAdd.ExternalId");

            Profile addressProfile = null;
            var response = this.CreateAddressProfile(partyToAdd.ExternalId, ref addressProfile);
            if (!response.Success)
            {
                result.Success = false;
                response.SystemMessages.ToList().ForEach(m => result.SystemMessages.Add(m));
                return null;
            }

            var requestToCommerceProfile = new TranslateEntityToCommerceAddressProfileRequest(partyToAdd, addressProfile);
            PipelineUtility.RunCommerceConnectPipeline<TranslateEntityToCommerceAddressProfileRequest, CommerceResult>(CommerceServerStorefrontConstants.PipelineNames.TranslateEntityToCommerceAddressProfile, requestToCommerceProfile);

            addressProfile.Update();

            ProfilePropertyListCollection<string> addressList;
            var profileValue = customerProfile["GeneralInfo.address_list"].Value as object[];
            if (profileValue != null)
            {
                var e = profileValue.Select(i => i.ToString());
                addressList = new ProfilePropertyListCollection<string>(e);
            }
            else
            {
                addressList = new ProfilePropertyListCollection<string>();
            }

            addressList.Add(partyToAdd.ExternalId);
            customerProfile["GeneralInfo.address_list"].Value = addressList.Cast<object>().ToArray();

            if (partyToAdd.IsPrimary)
            {
                customerProfile["GeneralInfo.preferred_address"].Value = partyToAdd.ExternalId;
            }

            customerProfile.Update();

            var newParty = this.EntityFactory.Create<RefSFModels.CommerceParty>("Party");
            TranslateCommerceAddressProfileToEntityRequest requestToEntity = new TranslateCommerceAddressProfileToEntityRequest(addressProfile, newParty);
            PipelineUtility.RunCommerceConnectPipeline<TranslateCommerceAddressProfileToEntityRequest, CommerceResult>(CommerceServerStorefrontConstants.PipelineNames.TranslateCommerceAddressProfileToEntity, requestToEntity);

            return requestToEntity.DestinationParty;
        }

        /// <summary>
        /// Processes the custom party.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="customerProfile">The customer profile.</param>
        /// <param name="party">The party.</param>
        /// <returns>Newly created party.</returns>
        private Party ProcessCustomParty(AddPartiesResult result, Profile customerProfile, Party party)
        {
            return null;
        }
    }
}