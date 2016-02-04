//-----------------------------------------------------------------------
// <copyright file="DynamicsStorefront.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the DynamicsStorefront class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.SitecoreItemModels
{
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the DynamicsStorefront class.
    /// </summary>
    public class DynamicsStorefront : CommerceStorefront
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicsStorefront"/> class.
        /// </summary>
        public DynamicsStorefront()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicsStorefront"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public DynamicsStorefront(Item item)
            : base(item)
        {
        }

        /// <summary>
        /// Gets the maximum number of loyalty programs a user can join.
        /// </summary>
        /// <value>
        /// The maximum number of loyalty programs a user can join.
        /// </value>
        public int MaxNumberOfLoyaltyProgramsToJoin
        {
            get
            {
                return MainUtil.GetInt(this.HomeItem[DynamicsStorefrontConstants.KnownFieldNames.MaxNumberOfLoyaltyProgramsToJoin], 1);
            }
        }
        
        /// <summary>
        /// Gets the channel identifier.
        /// </summary>
        /// <value>
        /// The channel identifier.
        /// </value>
        public virtual string ChannelId
        {
            get
            {
                if (this.InnerItem == null)
                {
                    return string.Empty;
                }

                return this.InnerItem[DynamicsStorefrontConstants.KnownFieldNames.ChannelId];
            }
        }
    }
}