//-----------------------------------------------------------------------
// <copyright file="SearchExtensions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the SearchExtensions class.</summary>
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

namespace Sitecore.Reference.Storefront.Extensions
{
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using Sitecore.ContentSearch.Linq;
    using System.Linq;

    /// <summary>
    /// Extension methods to help with some search functionality
    /// </summary>
    public static class SearchExtensions
    {
        /// <summary>
        /// Removes invalid facet values from a facet
        /// </summary>
        /// <param name="facet">The facet to clean</param>
        public static void Clean(this CommerceQueryFacet facet)
        {
            var items = facet.FoundValues.Where(v => string.IsNullOrEmpty(v.Name) || v.AggregateCount == 0);
            items.ToList().ForEach(v => facet.FoundValues.Remove(v));
        }
        
        /// <summary>
        /// Checks to make sure a facet is valid for use
        /// </summary>
        /// <param name="facet">The facet to check</param>
        /// <returns>Returns True if valid and False otherwise</returns>
        public static bool IsValid(this CommerceQueryFacet facet)
        {
            facet.Clean();

            if (facet.FoundValues.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks to make sure a facet value is valid for use
        /// </summary>
        /// <param name="value">The facet value to check</param>
        /// <returns>Returns True if valid and False otherwise</returns>
        public static bool IsValid(this FacetValue value)
        {
            if (value.AggregateCount > 0)
            {
                return true;
            }

            return false;
        }
    }
}