//-----------------------------------------------------------------------
// <copyright file="CSOrderHeaderItemBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CSOrderHeaderItemBaseJsonResult class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.JsonResults
{
    using Sitecore.Commerce.Entities.Orders;
    using Sitecore.Reference.Storefront.Managers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the CSOrderHeaderItemBaseJsonResult class.
    /// </summary>
    public class CSOrderHeaderItemBaseJsonResult : OrderHeaderItemBaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSOrderHeaderItemBaseJsonResult"/> class.
        /// </summary>
        /// <param name="header">The order header.</param>
        public CSOrderHeaderItemBaseJsonResult(OrderHeader header) : base(header)
        {
            this.DetailsUrl = string.Concat(StorefrontManager.StorefrontUri("/accountmanagement/myorder"), "?id=", header.OrderID);
            this.OrderId = header.OrderID;
        }
    }
}