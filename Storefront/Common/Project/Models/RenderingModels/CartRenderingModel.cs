//-----------------------------------------------------------------------
// <copyright file="CartRenderingModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Sitecore redering class for the CommerceCart.</summary>
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

namespace Sitecore.Reference.Storefront.Models.RenderingModels
{
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Mvc.Presentation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the CartRenderingModel class.
    /// </summary>
    public class CartRenderingModel : Sitecore.Mvc.Presentation.RenderingModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartRenderingModel"/> class.
        /// </summary>
        /// <param name="cart">The cart.</param>
        public CartRenderingModel(CommerceCart cart)
        {
            this.Cart = cart;
        }

        /// <summary>
        /// Gets or sets the cart.
        /// </summary>
        /// <value>
        /// The cart.
        /// </value>
        public CommerceCart Cart { get; set; }

        /// <summary>
        /// Gets the specified cart.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <param name="rendering">The rendering.</param>
        /// <returns>Gets an instance of the CartRenderingModel with initialized Sitecore rendering.</returns>
        public static CartRenderingModel Get(CommerceCart cart, Rendering rendering)
        {
            CartRenderingModel model = new CartRenderingModel(cart);

            model.Initialize(rendering);

            return model;
        }
    }
}