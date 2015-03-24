//-----------------------------------------------------------------------
// <copyright file="RemoveParties.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline processor used to remove parties (addresses) from CS user profiles.</summary>
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
    using Sitecore.Commerce.Connect.CommerceServer.Profiles.Models;
    using Sitecore.Commerce.Services.Customers;
    using Sitecore.Diagnostics;
    using System;
    using System.Linq;
    using RefSFModels = Sitecore.Reference.Storefront.Connect.Models;

    /// <summary>
    /// Defines the RemoveParties class.
    /// </summary>
    public class RemoveParties : CustomerPipelineProcessor
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentCondition(args.Request is RemovePartiesRequest, "args.Request", "args.Request is RemovePartiesRequest");
            Assert.ArgumentCondition(args.Result is CustomerResult, "args.Result", "args.Result is CustomerResult");

            var request = (RemovePartiesRequest)args.Request;
            var result = (CustomerResult)args.Result;

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
                var addressList = new ProfilePropertyListCollection<string>(e);

                foreach (var partyToRemove in request.Parties)
                {
                    var foundId = addressList.Where(x => x.Equals(partyToRemove.ExternalId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (foundId != null)
                    {
                        response = this.DeleteAddressCommerceProfile(foundId);
                        if (!response.Success)
                        {
                            result.Success = false;
                            response.SystemMessages.ToList().ForEach(m => result.SystemMessages.Add(m));
                            return;
                        }

                        addressList.Remove(foundId);

                        if (addressList.Count() == 0)
                        {
                            customerProfile["GeneralInfo.address_list"].Value = System.DBNull.Value;
                        }
                        else
                        {
                            customerProfile["GeneralInfo.address_list"].Value = addressList.Cast<object>().ToArray();
                        }

                        // Preffered address check. If the address being deleted was the preferred address we must clear it from the customer profile.
                        if (!string.IsNullOrWhiteSpace(preferredAddress) && preferredAddress.Equals(partyToRemove.ExternalId, StringComparison.OrdinalIgnoreCase))
                        {
                            customerProfile["GeneralInfo.preferred_address"].Value = System.DBNull.Value;
                        }

                        customerProfile.Update();
                    }
                }
            }
        }
    }
}