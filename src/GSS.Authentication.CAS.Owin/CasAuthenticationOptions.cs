﻿using System;
using System.Net.Http;
using GSS.Authentication.CAS;
using GSS.Authentication.CAS.Owin;
using GSS.Authentication.CAS.Validation;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Owin
{
    public class CasAuthenticationOptions : AuthenticationOptions, ICasOptions
    {
        public const string Scheme = CasDefaults.AuthenticationType;
        public CasAuthenticationOptions() : base(CasDefaults.AuthenticationType)
        {
            Caption = CasDefaults.AuthenticationType;
            CallbackPath = new PathString("/signin-cas");
            AuthenticationMode = AuthenticationMode.Passive;
            BackchannelTimeout = TimeSpan.FromSeconds(60);
        }

        public TimeSpan BackchannelTimeout { get; set; }

        public HttpMessageHandler BackchannelHttpHandler { get; set; }

        public string Caption
        {
            get => Description.Caption;
            set => Description.Caption = value;
        }

        public PathString CallbackPath { get; set; }

        public string SignInAsAuthenticationType { get; set; }

        /// <summary>
        /// store serviceTicket in AuthenticationProperties for single sign out ?
        /// </summary>
        public bool UseAuthenticationSessionStore { get; set; }

        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }

        #region ICasOptions
        public string CasServerUrlBase { get; set; }
        #endregion

        public IServiceTicketValidator ServiceTicketValidator { get; set; }

        public ICasAuthenticationProvider Provider { get; set; }
    }
}
