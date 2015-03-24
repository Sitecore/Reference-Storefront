//-----------------------------------------------------------------------
// <copyright file="WishListLineInputModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Controller parameters required to update/delete a wish list line.</summary>
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
    /// Defines the WishListLineInputModel class.
    /// </summary>
    public class WishListLineInputModel : BaseInputModel
    {
        /// <summary>
        /// Gets or sets the wish list identifier.
        /// </summary>
        /// <value>The wish list identifier.</value>
        [Required]
        public string WishListId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        /// <value>The product identifier.</value>
        [Required]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the variant identifier.
        /// </summary>
        /// <value>The variant identifier.</value>
        public string VariantId { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        [Required]
        public uint Quantity { get; set; }

        /// <summary>
        /// Gets or sets the external line identifier.
        /// </summary>
        /// <value>The external line identifier.</value>
        public string ExternalLineId { get; set; }
    }
}