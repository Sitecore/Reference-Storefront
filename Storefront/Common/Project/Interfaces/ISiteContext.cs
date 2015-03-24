//-----------------------------------------------------------------------
// <copyright file="ISiteContext.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the ISiteContext class.</summary>
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

namespace Sitecore.Reference.Storefront
{
    using Sitecore.Data.Items;
    using System;

    /// <summary>
    /// Interface that represents the current site context.
    /// </summary>
    public interface ISiteContext
    {
        /// <summary>
        /// Gets the current HTTP context.
        /// </summary>
        System.Web.HttpContext CurrentContext { get; }

        /// <summary>
        /// Gets the current HTTP context items collection.
        /// </summary>
        System.Collections.IDictionary Items { get; }

        /// <summary>
        /// Gets or sets the current catalog item.
        /// </summary>
        /// <value>
        /// The current catalog item.
        /// </value>
        Item CurrentCatalogItem { get; set; }

        /// <summary>
        /// Gets a value indicating whether the instance of the CurrentCatalogItem is a category.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is category; otherwise, <c>false</c>.
        /// </value>
        bool IsCategory { get; }

        /// <summary>
        /// Gets a value indicating whether the instance of the CurrentCatalogItem is product.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is product; otherwise, <c>false</c>.
        /// </value>
        bool IsProduct { get; }
    }
}
