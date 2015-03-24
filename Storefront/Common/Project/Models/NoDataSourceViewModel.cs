//-----------------------------------------------------------------------
// <copyright file="NoDataSourceViewModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the NoDataSourceViewModel class.</summary>
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
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;

    /// <summary>
    /// Used to represent a no datasource view
    /// </summary>
    public class NoDataSourceViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoDataSourceViewModel" /> class
        /// </summary>
        /// <param name="noDataSourceItem">The no data source item.</param>
        public NoDataSourceViewModel(Item noDataSourceItem)
        {
            Assert.ArgumentNotNull(noDataSourceItem, "noDatasourceItem");
            this.Message = noDataSourceItem["Text"];
        }

        /// <summary>
        /// Gets the error message to display when there is no datasource
        /// </summary>
        public string Message { get; private set; }
    }
}