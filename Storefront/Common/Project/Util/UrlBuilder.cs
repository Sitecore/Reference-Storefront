//-----------------------------------------------------------------------
// <copyright file="UrlBuilder.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>UrlBuilder helper class</summary>
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
    using Sitecore.Commerce.Connect.CommerceServer;
    using System;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// Used to help in the building and modification of urls
    /// </summary>
    public class UrlBuilder : UriBuilder
    {
        private QueryStringCollection _query = new QueryStringCollection();

        #region Constructor overloads

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlBuilder"/> class.
        /// </summary>
        public UrlBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlBuilder"/> class.
        /// </summary>
        /// <param name="uri">The URI to work with.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads", Justification = "By design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "By design.")]
        public UrlBuilder(string uri)
            : base(uri)
        {
            this.PopulateQueryString(uri);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlBuilder"/> class.
        /// </summary>
        /// <param name="uri">The URI to work with.</param>
        public UrlBuilder(Uri uri)
            : base(uri)
        {
            this.PopulateQueryString(uri.ToString());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlBuilder"/> class.
        /// </summary>
        /// <param name="schemeName">Name of the scheme.</param>
        /// <param name="hostName">Name of the host.</param>
        public UrlBuilder(string schemeName, string hostName)
            : base(schemeName, hostName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlBuilder"/> class.
        /// </summary>
        /// <param name="scheme">The scheme of the url.</param>
        /// <param name="host">The host of the url.</param>
        /// <param name="portNumber">The port number.</param>
        public UrlBuilder(string scheme, string host, int portNumber)
            : base(scheme, host, portNumber)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlBuilder"/> class.
        /// </summary>
        /// <param name="scheme">The scheme of the url.</param>
        /// <param name="host">The host of the url.</param>
        /// <param name="port">The port of the url.</param>
        /// <param name="pathValue">The path value.</param>
        public UrlBuilder(string scheme, string host, int port, string pathValue)
            : base(scheme, host, port, pathValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlBuilder"/> class.
        /// </summary>
        /// <param name="scheme">An Internet access protocol.</param>
        /// <param name="host">A DNS-style domain name or IP address.</param>
        /// <param name="port">An IP port number for the service.</param>
        /// <param name="path">The path to the Internet resource.</param>
        /// <param name="extraValue">A query string or fragment identifier.</param>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="extraValue"/> is neither null nor <see cref="F:System.String.Empty"/>, nor does a valid fragment identifier begin with a number sign (#), nor a valid query string begin with a question mark (?).
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="port"/> is less than 0.
        /// </exception>
        public UrlBuilder(string scheme, string host, int port, string path, string extraValue)
            : base(scheme, host, port, path, extraValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlBuilder"/> class.
        /// </summary>
        /// <param name="request">The web request.</param>
        protected UrlBuilder(System.Web.HttpRequest request) :
            base(request.Url.Scheme, request.Url.Host, request.Url.Port, request.Url.LocalPath)
        {
            this.PopulateQueryString(request);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a url build for the request url in the current context.
        /// </summary>
        /// <value>The current URL.</value>
        public static UrlBuilder CurrentUrl
        {
            get
            {
                var siteContext = CommerceTypeLoader.CreateInstance<ISiteContext>();
                return new UrlBuilder(siteContext.CurrentContext.Request.Url);
            }
        }

        /// <summary>
        /// Gets the querystring list.
        /// </summary>
        /// <value>The querystring list.</value>
        public QueryStringCollection QueryList
        {
            get
            {
                return this._query;
            }
        }

        /// <summary>
        /// Gets or sets the name of the page.
        /// </summary>
        /// <value>The name of the page.</value>
        public string PageName
        {
            get
            {
                string path = this.Path;
                return path.Substring(path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1);
            }

            set
            {
                string path = this.Path;
                path = path.Substring(0, path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase));
                this.Path = string.Concat(path, "/", value);
            }
        }

        /// <summary>
        /// Gets the domain of the url e.g. http://www.abc.com:81.
        /// </summary>
        /// <value>The domain.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design.")]
        public string UrlDomain
        {
            get
            {
                var urlDomain = string.Concat(this.Scheme + "://" + this.Uri.Host);
                if (!(this.Port == 80 && this.Scheme == "http") || !(this.Port == 443 && this.Scheme == "https"))
                {
                    urlDomain = string.Concat(urlDomain, ":", this.Port.ToString(CultureInfo.InvariantCulture));
                }

                return urlDomain;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// </PermissionSet>
        public new string ToString()
        {
            this.SetQuery();
            return this.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <param name="relative">True returns an absolute url, Fale returns a relative one</param>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// </PermissionSet>
        public string ToString(bool relative)
        {
            if (relative)
            {
                this.SetQuery();
                return this.Uri.PathAndQuery; 
            }
            else
            {
                return this.ToString();
            }
        }

        /// <summary>
        /// Navigates this instance.
        /// </summary>
        public void Navigate()
        {
            this._Navigate(true);
        }

        /// <summary>
        /// Navigates the specified end response.
        /// </summary>
        /// <param name="endResponse">if set to <c>true</c> [end response].</param>
        public void Navigate(bool endResponse)
        {
            this._Navigate(endResponse);
        }

        /// <summary>
        /// Toggles the scheme.
        /// </summary>
        /// <param name="toHttps">if set to <c>true</c> [to HTTPS].</param>
        public void ToggleScheme(bool toHttps)
        {
            if (toHttps)
            {
                this.Scheme = "https";
                this.Port = 443;
            }
            else
            {
                this.Scheme = "http";
                this.Port = 80;
            }
        }

        #endregion

        #region Private methods

        private void PopulateQueryString(System.Web.HttpRequest request)
        {
            this._query = new QueryStringCollection();

            foreach (string key in request.QueryString.AllKeys)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    this._query.Add(key, request.QueryString[key]);
                }
            }

            this.SetQuery();
        }

        /// <summary>
        /// Populates the query string.
        /// </summary>
        /// <param name="url">The URL to get the query string from.</param>
        private void PopulateQueryString(string url)
        {
            this._query = new QueryStringCollection(url);
            this.SetQuery();
        }

        /// <summary>
        /// Gets the query string.
        /// </summary>
        private void SetQuery()
        {
            this.Query = this._query.ToString().TrimStart(new[] { '?' });
        }

        /// <summary>
        /// _s the navigate.
        /// </summary>
        /// <param name="endResponse">if set to <c>true</c> [end response].</param>
        private void _Navigate(bool endResponse)
        {
            string uri = this.ToString();
            var siteContext = CommerceTypeLoader.CreateInstance<ISiteContext>();
            siteContext.CurrentContext.Response.Redirect(uri, endResponse);
        }

        #endregion
    }
}