//---------------------------------------------------------------------
// <copyright file="SitecoreExtensions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Extensions for dealing with Commerce Server entities.</summary>
//---------------------------------------------------------------------
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
    using Sitecore.Data.Items;
    using System;
    using System.Web.Mvc;
    using Sitecore.Resources.Media;

    /// <summary>
    /// Extensions for working with Sitecore objects in the MVC Site
    /// </summary>
    public static class SitecoreExtensions
    {
        /// <summary>
        /// Gets the full size image paths based on the BaseUrl in the app settings and the short path in the delimited property Image_Filename in the Product.
        /// </summary>
        /// <param name="item">The item on which the image is contained.</param>
        /// <param name="width">The width of the image to draw.</param>
        /// <param name="height">The height of the image to draw.</param>
        /// <returns>
        /// The full size image paths.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "By design")]
        public static string GetImageUrl(this MediaItem item, int width, int height)
        {
            if (item == null)
            {
                //This happens if there is a trailing pipe '|' at the end of an image list
                return string.Empty;
            }

            var options = new Sitecore.Resources.Media.MediaUrlOptions() { Height = height, Width = width };
            var url = Sitecore.Resources.Media.MediaManager.GetMediaUrl(item, options);
            var cleanUrl = Sitecore.StringUtil.EnsurePrefix('/', url);
            var hashedUrl = HashingUtils.ProtectAssetUrl(cleanUrl);

            return hashedUrl;
        }

        /// <summary>
        /// Creates a Sitecore Editframe for an item
        /// </summary>
        /// <param name="html">The html helper managing the current request</param>
        /// <param name="dataSource">A path to the Sitecore item that the frame is for</param>
        /// <param name="buttons">A path to the edit frame buttons in the core database that need to be shown</param>
        /// <returns>The editor for disposal</returns>
        public static IDisposable EditFrame(this HtmlHelper html, string dataSource = null, string buttons = null)
        {
            return new FrameEditor(html, dataSource, buttons);
        }
    }
}
