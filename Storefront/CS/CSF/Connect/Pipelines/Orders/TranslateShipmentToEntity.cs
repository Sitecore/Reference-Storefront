//-----------------------------------------------------------------------
// <copyright file="TranslateShipmentToEntity.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The translate shipment to entity pipeline processor.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Orders
{
    using CommerceServer.Core.Runtime.Orders;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using Sitecore.Commerce.Entities.Carts;

    /// <summary>
    /// TranslateShipmentToEntity pipeline processor
    /// </summary>
    public class TranslateShipmentToEntity : Commerce.Connect.CommerceServer.Orders.Pipelines.TranslateShipmentToEntity
    {
        /// <summary>
        /// Translates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="sourceShipment">The source shipment.</param>
        /// <param name="destinationShippingInfo">The destination shipping information.</param>
        protected override void Translate(TranslateShipmentToEntityRequest request, Shipment sourceShipment, ShippingInfo destinationShippingInfo)
        {
            base.Translate(request, sourceShipment, destinationShippingInfo);
            destinationShippingInfo.Properties.Add("ShippingMethodName", sourceShipment.ShippingMethodName);
        }
    }
}