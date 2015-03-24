//-----------------------------------------------------------------------
// <copyright file="StringUtility.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>General string utility class.</summary>
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

namespace Sitecore.Reference.Storefront.Util
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines the StringUtility class.
    /// </summary>
    public static class StringUtility
    {
        /// <summary>
        /// Removes the curly brackets.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The string without the curly brackets.</returns>
        public static string RemoveCurlyBrackets(string value)
        {
            return Regex.Replace(value, "[{}]", string.Empty);
        }
    }
}
