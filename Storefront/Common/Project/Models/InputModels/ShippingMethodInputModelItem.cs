//-----------------------------------------------------------------------
// <copyright file="ShippingMethodInputModelItem.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>InputModel item parameter accepting shipping method information.</summary>
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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines gthe ShippingMethodInputModelItem class.
    /// </summary>
    public class ShippingMethodInputModelItem
    {
        /// <summary>
        /// Gets or sets the shipping method identifier.
        /// </summary>
        /// <value>
        /// The shipping method identifier.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        [Required]
        public string ShippingMethodID { get; set; }

        /// <summary>
        /// Gets or sets the name of the shipping method.
        /// </summary>
        /// <value>
        /// The name of the shipping method.
        /// </value>
        [Required]
        public string ShippingMethodName { get; set; }

        /// <summary>
        /// Gets or sets the type of the shipping preference.
        /// </summary>
        /// <value>
        /// The type of the shipping preference.
        /// </value>
        [Required]
        public string ShippingPreferenceType { get; set; }

        /// <summary>
        /// Gets or sets the party identifier.
        /// </summary>
        /// <value>
        /// The party identifier.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public string PartyID { get; set; }

        /// <summary>
        /// Gets or sets the electronic delivery email.
        /// </summary>
        /// <value>
        /// The electronic delivery email.
        /// </value>
        [EmailAddress]
        public string ElectronicDeliveryEmail { get; set; }

        /// <summary>
        /// Gets or sets the content of the electronic delivery email.
        /// </summary>
        /// <value>
        /// The content of the electronic delivery email.
        /// </value>
        public string ElectronicDeliveryEmailContent { get; set; }

        /// <summary>
        /// Gets or sets the line ids.
        /// </summary>
        /// <value>
        /// The line ids.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IDs"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<string> LineIDs { get; set; }
    }
}