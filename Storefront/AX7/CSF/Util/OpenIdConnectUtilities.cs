//-----------------------------------------------------------------------
// <copyright file="OpenIdConnectUtilities.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The Open Id Connect Utilities class.</summary>
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
    using IO;
    using Microsoft.IdentityModel.Protocols;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Reference.Storefront.Configuration;
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IdentityModel.Tokens;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Security;
    using System.Security.Claims;
    using System.Text;
    using System.Web;
    using System.Web.Security;
    using System.Xml;

    /// <summary>
    /// Internal class OpenIdConnectUtilities.
    /// </summary>
    internal static class OpenIdConnectUtilities
    {
        /// <summary>
        /// Cookie name used to identify an identity provider which is being used for login purposes.
        /// </summary>
        internal const string CookieCurrentProvider = "oidcp";

        /// <summary>
        /// Cookie name used to identify an identity provider type.
        /// </summary>
        internal const string CookieCurrentProviderType = "oidcpt";

        /// <summary>
        /// Cookie name used for the open id cookie
        /// </summary>
        internal const string OpenIdCookie = "oidc";

        /// <summary>
        /// Cookie name used to identify a state to validate Authorization Code response.
        /// </summary>
        internal const string CookieState = "reqs";

        /// <summary>
        /// Cookie name used to identify a nonce to validate id_token.
        /// </summary>
        internal const string CookieNonce = "reqn";

        /// <summary>
        /// Configuration section name which contains Retail Parameters.
        /// </summary>
        internal const string ConfigurationSectionName = "retailConfiguration";

        /// <summary>
        /// The email.
        /// </summary>
        internal const string Email = "Email";

        /// <summary>
        /// Name for application cookie type of authentication.
        /// </summary>
        /// <remarks>This is the same value as Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie.</remarks>
        internal const string ApplicationCookieAuthenticationType = "ApplicationCookie";

        // Expiration is set to 60 + 1 minute, where 60 is the expiration for id_tokens issues by at least Azure AD and Google
        // So, setting forms auth ticket's (whos UserData is a container for id_token) expiration to be bigger than id_token 
        // to avoid FormsAuthTicket expire earlier than id_token.
        internal const int SignedInCookieDurationInMinutes = 61;

        /// <summary>
        /// Standard OpenID Connect Suffix which is added to an Authority(issuer) to retrieve Identity Provider Discovery Document.
        /// </summary>
        private const string DiscoveryDocumentSuffix = ".well-known/openid-configuration";

        private const string OpenIdConfigurationPathSuffix = @"/.well-known/openid-configuration";
        private const string GenericSecurityError = "The request is invalid.";
        private const string DynamicsConnectorConfigurationFile = "/App_Config/DynamicsRetail.Connectors.config";
        private static System.Configuration.Configuration _dynamicsConnectorConfiguration = null;

        internal static System.Configuration.Configuration DynamicsConnectorConfiguration
        {
            get
            {
                if (_dynamicsConnectorConfiguration == null)
                {
                    _dynamicsConnectorConfiguration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = FileUtil.MapPath(DynamicsConnectorConfigurationFile) }, ConfigurationUserLevel.None);
                }

                return _dynamicsConnectorConfiguration;
            }
        }

        /// <summary>
        /// Executes a request.
        /// </summary>
        /// <param name="req">The request.</param>
        /// <returns>The response.</returns>
        internal static string ExecuteRequest(HttpWebRequest req)
        {
            try
            {
                HttpWebResponse response1 = (HttpWebResponse)req.GetResponse();
                using (StreamReader streamReader = new StreamReader(response1.GetResponseStream()))
                {
                    string content = streamReader.ReadToEnd();
                    if (content != null && content.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        throw new InvalidOperationException(content);
                    }

                    return content;
                }
            }
            catch (WebException ex)
            {
                string error = HandleWebException(ex);
                throw new InvalidOperationException(error, ex);
            }
        }

        /// <summary>
        /// Sends an HttpGet request.
        /// </summary>
        /// <param name="url">URL of the request.</param>
        /// <returns>The response.</returns>
        internal static string HttpGet(Uri url)
        {
            ServicePointManager.CheckCertificateRevocationList = true;
            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            return ExecuteRequest(webRequest);
        }

        /// <summary>
        /// Sends a HTTP POST request and returns its response.
        /// </summary>
        /// <param name="url">URL of the request.</param>
        /// <param name="bodyParameters">Body of the request.</param>
        /// <returns>The response.</returns>
        internal static string HttpPost(Uri url, string bodyParameters)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            Stream stream = req.GetRequestStream();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(bodyParameters);
            stream.Write(bytes, 0, bytes.Length);
            req.ContentType = "application/x-www-form-urlencoded";

            string responseBody = ExecuteRequest(req);
            return responseBody;
        }

        /// <summary>
        /// Deserializes JSON.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="json">JSON string.</param>
        /// <returns>Strongly typed version of the deserialized JSON.</returns>
        internal static T DeserilizeJson<T>(string json)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(json)))
            {
                return (T)jsonSerializer.ReadObject(stream);
            }
        }     

        /// <summary>
        /// Gets Identity Provider used by a user to initiate Login process.
        /// </summary>
        /// <param name="throwExceptionIfAbsent">Throw exception if current provider settings are missing.</param>
        /// <returns>The Identity Provider.</returns>
        internal static IdentityProviderClientConfigurationElement GetCurrentProviderSettings(bool throwExceptionIfAbsent = true)
        {
            HttpCookie currentProviderCookie = HttpContext.Current.Request.Cookies.Get(OpenIdConnectUtilities.CookieCurrentProvider);
            if (currentProviderCookie == null)
            {
                if (throwExceptionIfAbsent)
                {
                    throw new SecurityException(GenericSecurityError);
                }
                else
                {
                    return null;
                }
            }

            return GetIdentityProviderFromConfiguration(currentProviderCookie.Value);
        }

        /// <summary>
        /// Gets the identity provider from configuration.
        /// </summary>
        /// <param name="name">The name of identity provider.</param>
        /// <returns>The identity provider client configuration element.</returns>
        internal static IdentityProviderClientConfigurationElement GetIdentityProviderFromConfiguration(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            RetailConfiguration retailConfiguration = (RetailConfiguration)DynamicsConnectorConfiguration.GetSection(OpenIdConnectUtilities.ConfigurationSectionName);
            foreach (IdentityProviderClientConfigurationElement provider in retailConfiguration.IdentityProviders)
            {
                if (provider.Name == name)
                {
                    return provider;
                }
            }

            throw new InvalidOperationException("Current Identity Provider was not found, this could happen due to timeout, login again.");
        }

        /// <summary>
        /// Validates incoming request and extracts an Authorization Code.
        /// </summary>        
        /// <returns>The Authorization Code.</returns>
        internal static string ValidateRequestAndGetAuthorizationCode()
        {
            string code = HttpContext.Current.Request.Params["code"];

            // string code = this.Request.Params["code"];
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new SecurityException(GenericSecurityError);
            }

            string receivedState = HttpContext.Current.Request.Params["state"];
            HttpCookie stateCookie = HttpContext.Current.Request.Cookies.Get(OpenIdConnectUtilities.CookieState);
            if (stateCookie == null)
            {
                throw new SecurityException(GenericSecurityError);
            }

            if (receivedState != HttpUtility.UrlDecode(stateCookie.Value))
            {
                throw new SecurityException(GenericSecurityError);
            }

            return code;
        }

        /// <summary>
        /// Loads Discovery Document buy using well known provider's URL.
        /// </summary>
        /// <param name="issuer">Uri issuer.</param>        
        /// <returns>The Discovery Document.</returns>
        internal static OpenIdConnectConfiguration GetDiscoveryDocument(Uri issuer)
        {
            string openIdConfigurationBasePath = issuer.ToString().TrimEnd('/');

            string openIdConfigurationFullPath = openIdConfigurationBasePath + OpenIdConfigurationPathSuffix;
            Uri openIdConfigurationUri = new Uri(openIdConfigurationFullPath, UriKind.Absolute);

            string discoveryResponse = HttpGet(openIdConfigurationUri);

            OpenIdConnectConfiguration result = new OpenIdConnectConfiguration(discoveryResponse);
            return result;
        }       

        /// <summary>
        /// Validates raw id_token and returns in strongly typed version.
        /// </summary>
        /// <param name="idTokenRaw">Raw id_token.</param>
        /// <returns>The validated token.</returns>
        internal static JwtSecurityToken GetIdToken(string idTokenRaw)
        {
            JwtSecurityToken token = new JwtSecurityToken(idTokenRaw);

            HttpCookie nonceCookie = HttpContext.Current.Request.Cookies.Get(OpenIdConnectUtilities.CookieNonce);
            if (nonceCookie == null)
            {
                throw new SecurityException(GenericSecurityError);
            }

            Claim nonceClaim = token.Claims.SingleOrDefault(c => c.Type == "nonce");
            if (nonceClaim == null)
            {
                throw new SecurityException(GenericSecurityError);
            }

            if (nonceClaim.Value != nonceCookie.Value)
            {
                throw new SecurityException(GenericSecurityError);
            }

            return token;
        }

        /// <summary>
        /// Gets the ACS token.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <returns>The ACS token.</returns>
        /// <exception cref="SecurityException">Thrown when the token is not found.</exception>
        internal static string GetAcsToken(string queryString)
        {
            NameValueCollection collection = HttpUtility.ParseQueryString(queryString);
            string wresult = collection.Get("wresult");
            TextReader textReader = new StringReader(wresult);

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.XmlResolver = null;

            XmlReader xmlReader = XmlReader.Create(textReader, xmlReaderSettings);
            if (!xmlReader.ReadToFollowing("wsse:BinarySecurityToken"))
            {
                var securityException = new SecurityException("Could not find the token.");
                CommerceLog.Current.Error("Could not find the token.", typeof(OpenIdConnectUtilities), securityException);
                throw securityException;
            }

            string encodedToken = xmlReader.ReadElementContentAsString();
            byte[] bytes = System.Convert.FromBase64String(encodedToken);
            string decodedToken = System.Text.Encoding.UTF8.GetString(bytes);
            return decodedToken;
        }

        /// <summary>
        /// Set the token cookie.
        /// </summary>       
        /// <param name="idToken">The id_token.</param>
        internal static void SetTokenCookie(string idToken)
        {
            HttpCookie ck;
            byte[] encryptedData = MachineKey.Protect(Encoding.ASCII.GetBytes(idToken), "Authentication token");

            ck = new HttpCookie(OpenIdConnectUtilities.OpenIdCookie, Convert.ToBase64String(encryptedData));
            ck.Expires = DateTime.Now.AddMinutes(OpenIdConnectUtilities.SignedInCookieDurationInMinutes);
            ck.Path = FormsAuthentication.FormsCookiePath;
            ck.HttpOnly = true;
            ck.Secure = true;

            HttpContext.Current.Response.Cookies.Add(ck);
        }

        /// <summary>
        /// Sets the cookie.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        internal static void SetCookie(HttpContextBase context, string name, string value)
        {
            HttpCookie cookie = new HttpCookie(name, value);
            cookie.Expires = DateTime.MaxValue;
            cookie.HttpOnly = true;
            cookie.Secure = true;
            context.Response.Cookies.Add(cookie);
        }

        internal static void RemoveCookie(string cookieName)
        {
            HttpCookie expiredCookie = new HttpCookie(cookieName)
            {
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            HttpContext.Current.Response.SetCookie(expiredCookie);
        }

        /// <summary>
        /// Performs clean up operations after any authentication/authorization failure.
        /// </summary>
        /// <param name="httpContextBase">The HTTP context.</param>
        internal static void CleanUpOnSignOutOrAuthFailure(HttpContextBase httpContextBase)
        {
            if (httpContextBase == null)
            {
                throw new ArgumentNullException("httpContextBase");
            }

            var ctx = httpContextBase.Request.GetOwinContext();
            ctx.Authentication.SignOut(ApplicationCookieAuthenticationType);

            RemoveCookie(OpenIdCookie);
        }

        /// <summary>
        /// Handles an exception which could take place while sending a request.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The error extracted from the response.</returns>
        private static string HandleWebException(WebException ex)
        {
            using (WebResponse response = ex.Response)
            {
                using (Stream data = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(data);
                    return reader.ReadToEnd();
                }
            }
        }
    }
}