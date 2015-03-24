//-----------------------------------------------------------------------
// <copyright file="PromoRotator.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the PromoRotator class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.SitecoreItemModels
{
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using Sitecore.Data.Items;
    using Sitecore.Mvc.Presentation;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// PromoRotatorViewModel class
    /// </summary>
    public class PromoRotator : SitecoreItemBase
    {
        private readonly List<CommercePromotion> _promotions = new List<CommercePromotion>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PromoRotator"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public PromoRotator(Item item)
        {
            this.InnerItem = item;
        }

        /// <summary>
        /// Gets the promotions
        /// </summary>
        /// <value>
        /// The promotions.
        /// </value>
        public List<CommercePromotion> Promotions
        {
            get { return _promotions; }
        }

        /// <summary>
        /// Gets the width of the requested Promotion
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width
        {
            get
            {
                //Default
                int defaultWidth = 100;

                NameValueCollection parametersWidth = Web.WebUtil.ParseUrlParameters(RenderingContext.Current.Rendering["Parameters"]);
                if (parametersWidth.AllKeys.Any(p => p == "Width"))
                {
                    if (int.TryParse(parametersWidth.Get("Width"), out defaultWidth))
                    {
                        return defaultWidth;
                    }
                }

                return defaultWidth;
            }
        }

        /// <summary>
        /// Gets the height of the requested Promotion
        /// </summary>
        public int Height
        {
            get
            {
                //Default
                int defaultHeight = 100;

                NameValueCollection parametersHeight = Web.WebUtil.ParseUrlParameters(RenderingContext.Current.Rendering["Parameters"]);

                if (parametersHeight.AllKeys.Any(p => p == "Height"))
                {
                    if (int.TryParse(parametersHeight.Get("Height"), out defaultHeight))
                    {
                        return defaultHeight;
                    }
                }

                return defaultHeight;
            }
        }
    }
}