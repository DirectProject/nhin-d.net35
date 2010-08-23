﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using NHINDirect.Certificates;
using NHINDirect.Agent.Config;
using NHINDirect.Config.Client;

namespace NHINDirect.SmtpAgent
{
    [XmlType("ServiceResolver")]
    public class CertServiceResolverSettings : CertResolverSettings
    {
        public CertServiceResolverSettings()
        {
        }
        
        [XmlElement]        
        public ClientSettings ClientSettings
        {
            get;
            set;
        }
        
        public override ICertificateResolver CreateResolver()
        {
            return new ConfigCertificateResolver(this.ClientSettings);            
        }

        public override void Validate()
        {
            if (this.ClientSettings == null)
            {
                throw new SmtpAgentException(SmtpAgentError.MissingCertResolverClientSettings);
            }
            this.ClientSettings.Validate();
        }
    }
    
    public class AnchorServiceResolverSettings : TrustAnchorResolverSettings
    {
        public AnchorServiceResolverSettings()
        {
        }
        
        [XmlElement]
        public ClientSettings ClientSettings
        {
            get;
            set;
        }
    
        public override ITrustAnchorResolver CreateResolver()
        {
            return new ConfigAnchorResolver(this.ClientSettings);
        }

        public override void Validate()
        {
            if (this.ClientSettings == null)
            {
                throw new SmtpAgentException(SmtpAgentError.MissingAnchorResolverClientSettings);
            }
            this.ClientSettings.Validate();
        }
    }    
}