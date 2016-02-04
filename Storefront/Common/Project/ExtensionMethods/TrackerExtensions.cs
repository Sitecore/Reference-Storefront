//-----------------------------------------------------------------------
// <copyright file="TrackerExtensions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Extension to the Sitecore Tracker object.</summary>
//-----------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
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
    using Sitecore.Analytics;
    using Sitecore.Reference.Storefront.Exceptions;
    using Sitecore.Reference.Storefront.Managers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the TrackerExtensions class.
    /// </summary>
    public static class TrackerExtensions
    {
        /// <summary>
        /// Checks if the Tracker.Current object os null.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        /// <returns>In the context of the Runtime site, will trow a TrackingNotEnabledException; Otherwise null is returned
        /// in the context of the page editor.</returns>
        /// <exception cref="TrackingNotEnabledException">Tracker is null we are in the context of the runtime site.</exception>
        public static ITracker CheckForNull(this ITracker tracker)
        {
            if (tracker == null && !Sitecore.Context.PageMode.IsExperienceEditor)
            {
                throw new TrackingNotEnabledException(StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.TrackingNotEnabled));
            }

            return tracker;
        }
    }
}
