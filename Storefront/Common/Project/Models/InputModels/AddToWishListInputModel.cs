//-----------------------------------------------------------------------
// <copyright file="AddToWishListInputModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Controller parameters required to add a wish list line request.</summary>
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

namespace Sitecore.Reference.Storefront.Models.InputModels
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the AddToWishListInputModel class.
    /// </summary>
    public class AddToWishListInputModel : BaseInputModel
    {
        /// <summary>
        /// Gets or sets the Product Id of the current LineItem.
        /// </summary>
        [Required]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the VariantId of the current LineItem.
        /// </summary>
        public string VariantId { get; set; }

        /// <summary>
        /// Gets or sets the ProductCatalog of the current LineItem.
        /// </summary>
        [Required]
        public string ProductCatalog { get; set; }

        /// <summary>
        /// Gets or sets the Quantity of the current LineItem.
        /// </summary>
        [Required]
        public decimal? Quantity { get; set; }

        /// <summary>
        /// Gets or sets the wish list identifier.
        /// </summary>       
        public string WishListId { get; set; }

        /// <summary>
        /// Gets or sets the name of the wish list.
        /// </summary>        
        public string WishListName { get; set; }

        /// <summary>
        /// Gets or sets the gift card amount.
        /// </summary>       
        public decimal? GiftCardAmount { get; set; }
    }
}
