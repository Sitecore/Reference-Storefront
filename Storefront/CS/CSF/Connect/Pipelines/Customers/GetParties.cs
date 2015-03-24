//-----------------------------------------------------------------------
// <copyright file="GetParties.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline processor used to retrieve parties (addresses) from CS user profiles.</summary>
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
    /// Defines the GetParties class.
    /// </summary>
    public class GetParties : CustomerPipelineProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetParties"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public GetParties([NotNull] IEntityFactory entityFactory)
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
            Assert.ArgumentCondition(args.Request is GetPartiesRequest, "args.Request", "args.Request is GetPartiesRequest");
            Assert.ArgumentCondition(args.Result is GetPartiesResult, "args.Result", "args.Result is GetPartiesResult");

            var request = (GetPartiesRequest)args.Request;
            var result = (GetPartiesResult)args.Result;
            Assert.ArgumentNotNull(request.CommerceCustomer, "request.CommerceCustomer");

            List<Party> partyList = new List<Party>();

            Profile customerProfile = null;
            var response = this.GetCommerceUserProfile(request.CommerceCustomer.ExternalId, ref customerProfile);
            if (!response.Success)
            {
                result.Success = false;
                response.SystemMessages.ToList().ForEach(m => result.SystemMessages.Add(m));
                return;
            }

            string preferredAddress = customerProfile["GeneralInfo.preferred_address"].Value as string;

            var profileValue = customerProfile["GeneralInfo.address_list"].Value as object[];
            if (profileValue != null)
            {
                var e = profileValue.Select(i => i.ToString());
                ProfilePropertyListCollection<string> addresIdsList = new ProfilePropertyListCollection<string>(e);
                if (addresIdsList != null)
                {
                    foreach (string addressId in addresIdsList)
                    {
                        Profile commerceAddress = null;
                        response = this.GetCommerceAddressProfile(addressId, ref commerceAddress);
                        if (!response.Success)
                        {
                            result.Success = false;
                            response.SystemMessages.ToList().ForEach(m => result.SystemMessages.Add(m));
                            return;
                        }

                        var newParty = this.EntityFactory.Create<RefSFModels.CommerceParty>("Party");
                        var requestTorequestToEntity = new TranslateCommerceAddressProfileToEntityRequest(commerceAddress, newParty);
                        PipelineUtility.RunCommerceConnectPipeline<TranslateCommerceAddressProfileToEntityRequest, CommerceResult>(CommerceServerStorefrontConstants.PipelineNames.TranslateCommerceAddressProfileToEntity, requestTorequestToEntity);

                        if (!string.IsNullOrWhiteSpace(preferredAddress) && preferredAddress.Equals(newParty.ExternalId, System.StringComparison.OrdinalIgnoreCase))
                        {
                            newParty.IsPrimary = true;
                        }

                        var address = requestTorequestToEntity.DestinationParty;

                        partyList.Add(address);
                    }
                }
            }

            result.Parties = partyList.AsReadOnly();
        }
    }
}