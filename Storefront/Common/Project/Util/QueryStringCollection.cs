//-----------------------------------------------------------------------
// <copyright file="QueryStringCollection.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>QueryStringCollection helper class</summary>
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
    using System;
    using System.Text;
    using System.Web;
    using System.Collections.Specialized;
    using Microsoft.Security.Application;
    using System.Runtime.Serialization;
    using Sitecore.Commerce.Connect.CommerceServer;

    /// <summary>
    /// Used for managing querystrings
    /// </summary>
    [Serializable]
    public class QueryStringCollection : NameValueCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryStringCollection"/> class.
        /// </summary>
        public QueryStringCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryStringCollection"/> class.
        /// </summary>
        /// <param name="url">The URL to get the query string from.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "By design.")]
        public QueryStringCollection(string url)
        {
            Uri baseUri;
            var isValid = Uri.TryCreate(url, UriKind.Absolute, out baseUri);

            if (!isValid)
            {
                throw new UriFormatException("Invalid url format");
            }

            if (url.IndexOf("?", StringComparison.OrdinalIgnoreCase) <= -1)
            {
                return;
            }

            foreach (string keyValuePair in this.ExtractQueryString(url).Split('&'))
            {
                if (string.IsNullOrEmpty(keyValuePair))
                {
                    continue;
                }

                var firstEquals = keyValuePair.IndexOf("=", StringComparison.OrdinalIgnoreCase);
                var key = keyValuePair.Substring(0, firstEquals);
                var value = keyValuePair.Substring(firstEquals + 1);
                this.Add(key, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryStringCollection"/> class.
        /// </summary>
        /// <param name="info">A System.Runtime.Serialization.SerializationInfo object that contains the information required to serialize the new System.Collections.Specialized.NameValueCollection instance.</param>
        /// <param name="context">A System.Runtime.Serialization.StreamingContext object that contains the source and destination of the serialized stream associated with the new System.Collections.Specialized.NameValueCollection instance.</param>
        protected QueryStringCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the querystring instance based on the current context url.
        /// </summary>
        /// <value>The current.</value>
        public static QueryStringCollection Current
        {
            get
            {
                return new QueryStringCollection().FromCurrent();
            }
        }

        /// <summary>
        /// Trys to parse a query string and add its values to the current collection.
        /// </summary>
        /// <param name="queryString">The query string part of a url</param>
        /// <value>The current.</value>
        public void Parse(string queryString)
        {
            foreach (string keyValuePair in queryString.Split('&'))
            {
                if (string.IsNullOrEmpty(keyValuePair))
                {
                    continue;
                }

                var firstEquals = keyValuePair.IndexOf("=", StringComparison.OrdinalIgnoreCase);
                var key = keyValuePair.Substring(0, firstEquals);
                var value = keyValuePair.Substring(firstEquals + 1);
                this.Add(key, value);
            }
        }

        /// <summary>        
        /// checks if a name already exists within the query string collection        
        /// </summary>        
        /// <param name="name">the name to check</param>        
        /// <returns>a boolean if the name exists</returns>        
        public bool Contains(string name)
        {
            string existingValue = base[name];
            return !string.IsNullOrEmpty(existingValue);
        }

        /// <summary>        
        /// outputs the querystring object to a string        
        /// </summary>        
        /// <returns>the encoded querystring as it would appear in a browser</returns>        
        public override string ToString()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < this.Keys.Count; i++)
            {
                var keyValue = this.Keys[i];
                if (!string.IsNullOrEmpty(keyValue) && !String.IsNullOrEmpty(base[keyValue]))
                {
                    foreach (string val in base[keyValue].Split(','))
                    {
                        builder.Append(builder.Length == 0 ? "?" : "&");
                        builder.Append(Microsoft.Security.Application.Encoder.UrlEncode(keyValue));
                        builder.Append("=");
                        builder.Append(Microsoft.Security.Application.Encoder.UrlEncode(val));
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>        
        /// extracts a querystring from a full URL        
        /// </summary>        
        /// <param name="fullUrl">the string to extract the querystring from</param>        
        /// <returns>a string representing only the querystring</returns>        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "By design.")]
        public string ExtractQueryString(string fullUrl)
        {
            if (!string.IsNullOrEmpty(fullUrl))
            {
                if (fullUrl.Contains("?"))
                {
                    return fullUrl.Substring(fullUrl.IndexOf("?", StringComparison.OrdinalIgnoreCase) + 1);
                }
            }

            return fullUrl;
        }

        /// <summary>        
        /// returns a QueryStringCollection object based on the current querystring of the request        
        /// </summary>        
        /// <returns>the QueryStringCollection object </returns>        
        public QueryStringCollection FromCurrent()
        {
            var siteContext = CommerceTypeLoader.CreateInstance<ISiteContext>();
            return new QueryStringCollection(siteContext.CurrentContext.Request.Url.ToString());
        }

        /// <summary>        
        /// add a name value pair to the collection        
        /// </summary>        
        /// <param name="name">the name of the parameter</param>        
        /// <param name="value">the value associated to the name</param>        
        /// <returns>the QueryStringCollection object </returns>        
        public new QueryStringCollection Add(string name, string value)
        {
            return this.Add(name, value, false);
        }

        /// <summary>
        /// Adds the or set a property.
        /// </summary>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="value">The value of the property to add.</param>
        public void AddOrSet(string name, string value)
        {
            if (!this.Contains(name))
            {
                this.Add(name, value);
            }
            else
            {
                this.Set(name, value);
            }
        }

        /// <summary>        
        /// adds a name value pair to the collection        
        /// </summary>        
        /// <param name="name">the name of the parameter</param>        
        /// <param name="value">the value associated to the name</param>        
        /// <param name="isUnique">true if the name is unique within the querystring. This allows us to override existing values</param>        
        /// <returns>the QueryStringCollection object </returns>        
        public QueryStringCollection Add(string name, string value, bool isUnique)
        {
            string existingValue = base[name];
            if (string.IsNullOrEmpty(existingValue))
            {
                base.Add(name, value);
            }
            else if (isUnique)
            {
                base[name] = value;
            }
            else
            {
                base[name] += "," + value;
            }

            return this;
        }

        /// <summary>        
        /// removes a name value pair from the querystring collection        
        /// </summary>        
        /// <param name="name">name of the querystring value to remove</param>        
        /// <returns>the QueryStringCollection object</returns>        
        public new QueryStringCollection Remove(string name)
        {
            base.Remove(name);
            return this;
        }

        /// <summary>        
        /// clears the collection        
        /// </summary>        
        /// <returns>the QueryStringCollection object </returns>        
        public QueryStringCollection Reset()
        {
            this.Clear();
            return this;
        }
    }
}