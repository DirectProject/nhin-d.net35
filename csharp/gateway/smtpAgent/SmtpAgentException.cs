﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHINDirect.Collections;
using NHINDirect.Agent;

namespace NHINDirect.SmtpAgent
{
    public enum SmtpAgentError
    {
        Unknown = 0,
        MissingPostmaster,
        MissingLogSettings,
        InvalidEnvelopeFromAgent,
        EmptyResultFromAgent,
        MissingMailPickupFolder,
        MailPickupFolderDoesNotExist,
        MissingBounceTemplateOutgoing,
        MissingBounceTemplateIncoming,
        InvalidBounceMessageTemplate,
        MissingCertResolverClientSettings,
        MissingAnchorResolverClientSettings,
    }
    
    public class SmtpAgentException : NHINDException<SmtpAgentError>
    {
        public SmtpAgentException(SmtpAgentError error)
            : base(error)
        {
        }
    }
}
