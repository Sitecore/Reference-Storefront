//-----------------------------------------------------------------------
// <copyright file="PaginationModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
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

namespace Sitecore.Reference.Storefront.Models
{
    /// <summary>
    /// Used to represent pagination details
    /// </summary>
    public class PaginationModel
    {
        /// <summary>
        /// Gets or sets the current page number
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of pages in the result set
        /// </summary>
        public int NumberOfPages { get; set; }
        
        /// <summary>
        /// Gets or sets the number of items on this page
        /// </summary>
        public int PageResultCount { get; set; }

        /// <summary>
        /// Gets or sets the index of the first item in this result set
        /// </summary>
        public int StartResultIndex { get; set; }

        /// <summary>
        /// Gets or sets the index ofthe last item in this result set
        /// </summary>
        public int EndResultIndex { get; set; }

        /// <summary>
        /// Gets or sets the overall total count of items
        /// </summary>
        public int TotalResultCount { get; set; }
    }
}