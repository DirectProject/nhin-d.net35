﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using WCF = System.ServiceModel.Channels;

using NHINDirect.Diagnostics;
using NHINDirect.XDS.Common;
using System.IO;

namespace NHINDirect.XDS
{
    public class ExportDocumentSet: IExportDocumentSet
    {
        //# region for diagnostics
        //private ILogger m_Logger;

        //private ILogger Logger
        //{
        //    get { return m_Logger; }
        //}
        //#endregion


        #region IExportDocumentSet Members
        public ProvideAndRegisterResponse ProvideAndRegisterDocumentSet(string xdsMetadata, string xdsDocument, string endpointUrl, string certThumbprint)
        {
            EndpointAddress endpointAddress;
            X509Certificate2 clientCert;
            ProvideAndRegisterRequest pandRXDSBRequest;
            //m_Logger = Log.For(this);
            try
            {
                // create request
                pandRXDSBRequest = getPandRRequest(xdsMetadata, xdsDocument);
                // get an endpoint from the url string
                endpointAddress = new EndpointAddress(endpointUrl);
                // if this is https then get the client cert from the thumbprint
                clientCert = null;
                if ((endpointUrl.StartsWith("https")) && (!string.IsNullOrEmpty(certThumbprint)))
                {
                    clientCert = StaticHelpers.getCertFromThumbprint(certThumbprint);
                }

            }
            catch (Exception ex)
            {
                ProvideAndRegisterResponse pandRResponse = errorResponse(GlobalValues.CONST_ERROR_CODE_XDSRepositoryError, string.Format("error: {0}; stacktrace{1}", ex.Message, ex.StackTrace));
                return pandRResponse;
            }

            return exportXDSB(pandRXDSBRequest, endpointAddress, clientCert);

        }

        public ProvideAndRegisterResponse ProvideAndRegisterDocumentSet(XmlDocument xdsMetadata, XmlDocument xdsDocument, string endpointUrl, string certThumbprint)
        {
            EndpointAddress endpointAddress;
            X509Certificate2 clientCert;
            ProvideAndRegisterRequest pandRXDSBRequest;
            //m_Logger = Log.For(this);
            try
            {
                // create request
                pandRXDSBRequest = getPandRRequest(xdsMetadata, xdsDocument);

                // get an endpoint from the url string
                endpointAddress = new EndpointAddress(endpointUrl);
                // if this is https then get the client cert from the thumbprint
                clientCert = null;
                if ((endpointUrl.StartsWith("https")) && (!string.IsNullOrEmpty(certThumbprint)))
                {
                    clientCert = StaticHelpers.getCertFromThumbprint(certThumbprint);
                }

            }
            catch (Exception ex)
            {
                ProvideAndRegisterResponse pandRResponse = errorResponse(GlobalValues.CONST_ERROR_CODE_XDSRepositoryError, string.Format("error: {0}; stacktrace{1}", ex.Message, ex.StackTrace));
                return pandRResponse;
            }

            return exportXDSB(pandRXDSBRequest, endpointAddress, clientCert);

        }

        public ProvideAndRegisterResponse ProvideAndRegisterDocumentSet(XmlDocument xdsMetadata, XmlDocument xdsDocument, EndpointAddress endpointAddress, string certThumbprint)
        {
            X509Certificate2 clientCert;
            ProvideAndRegisterRequest pandRXDSBRequest;
            //m_Logger = Log.For(this);
            try
            {
                //  create request
                pandRXDSBRequest = getPandRRequest(xdsMetadata, xdsDocument);

                // if this is https then get the client cert from the thumbprint
                clientCert = null;
                if ((endpointAddress.Uri.ToString().StartsWith("https")) && (!string.IsNullOrEmpty(certThumbprint)))
                {
                    clientCert = StaticHelpers.getCertFromThumbprint(certThumbprint);
                }

            }
            catch (Exception ex)
            {
                ProvideAndRegisterResponse pandRResponse = errorResponse(GlobalValues.CONST_ERROR_CODE_XDSRepositoryError, string.Format("error: {0}; stacktrace{1}", ex.Message, ex.StackTrace));
                return pandRResponse;
            }

            return exportXDSB(pandRXDSBRequest, endpointAddress, clientCert);

        }

        public ProvideAndRegisterResponse ProvideAndRegisterDocumentSet(XmlDocument xdsMetadata, XmlDocument xdsDocument, EndpointAddress endpointAddress, X509Certificate2 clientCert)
        {
            ProvideAndRegisterRequest pandRXDSBRequest;
            //m_Logger = Log.For(this);
            try
            {
                // create request
                pandRXDSBRequest = getPandRRequest(xdsMetadata, xdsDocument);
            }
            catch (Exception ex)
            {
                ProvideAndRegisterResponse pandRResponse = errorResponse(GlobalValues.CONST_ERROR_CODE_XDSRepositoryError, string.Format("error: {0}; stacktrace{1}", ex.Message, ex.StackTrace));
                return pandRResponse;
            }

            return exportXDSB(pandRXDSBRequest, endpointAddress, clientCert);
        }

        #endregion

        #region private

        private ProvideAndRegisterRequest getPandRRequest(XmlDocument xdsMetadata, XmlDocument xdsDocument)
        {
            // put together a P&R Request
            ProvideAndRegisterRequest pandRRequest = new ProvideAndRegisterRequest();
            SubmissionDocument theSubmissionDocument = new SubmissionDocument();

            // todo - get the id out of the metadata
            XmlNode extrinsicObject = xdsMetadata.DocumentElement.SelectSingleNode("//*[local-name()='ExtrinsicObject']");
            string temp = "";
            XmlNode tempNode = extrinsicObject.Attributes.GetNamedItem("id");
            if (tempNode != null)
            {
                temp = tempNode.InnerXml;
            }
            if (!string.IsNullOrEmpty(temp))
            {
                theSubmissionDocument.documentID = temp;
            }
            else
            {
                XmlNode idAttribute = xdsDocument.CreateNode(XmlNodeType.Attribute, "id", "");
                idAttribute.Value = "theDocument";
                extrinsicObject.Attributes.SetNamedItem(idAttribute);
                theSubmissionDocument.documentID = idAttribute.Value;
            }

            
            theSubmissionDocument.documentText = ASCIIEncoding.UTF8.GetBytes(xdsDocument.DocumentElement.OuterXml);

            pandRRequest.submissionDocuments.Add(theSubmissionDocument);
            pandRRequest.submissionMetadata = xdsMetadata.DocumentElement;

            return pandRRequest;
        }

        private ProvideAndRegisterRequest getPandRRequest(string xdsMetadata, string xdsDocument)
        {
            XmlDocument theMetadada;
            XmlDocument theDocument;
            try
            {
                theMetadada = new XmlDocument();
                theMetadada.LoadXml(xdsMetadata);
                theDocument = new XmlDocument();
                theDocument.LoadXml(xdsDocument);
            }
            catch (Exception)
            {
                throw;
            }
            return getPandRRequest(theMetadada, theDocument);
        }
        private ProvideAndRegisterRequest getPandRRequest(XmlDocument xdsMetadata, string xdsDocument)
        {
            XmlDocument theDocument;
            try
            {
                theDocument = new XmlDocument();
                theDocument.LoadXml(xdsDocument);
            }
            catch (Exception)
            {
                throw;
            }
            return getPandRRequest(xdsMetadata, theDocument);
        }

        private ProvideAndRegisterResponse exportXDSB(ProvideAndRegisterRequest pandRXDSBRequest, System.ServiceModel.EndpointAddress endpointAddress, X509Certificate2 clientCert)
        {
            ProvideAndRegisterResponse pandRResponse = null;
            // setup a default, we blew it, error response
            pandRResponse = errorResponse(GlobalValues.CONST_ERROR_CODE_XDSRepositoryError, "");
            XDSRepositoryClient xdsRepClient = null;
            try
            {
                // four basic steps
                // 1) build the message
                // 2) create the client proxy (using our own binding (instead of depending on a web.config))
                // 3) send the message and get back the response
                // 4) interrogate the repository/xdr recipient response and create a response object

                //Logger.Debug("begin exportXDSB");
               
                // 1) build the message
                // setup the WCF in and output messages
                WCF.Message wcfInput, wcfOutput;

                wcfInput = WCF.Message.CreateMessage(WCF.MessageVersion.Soap12WSAddressing10
                    , StaticHelpers.XDS_PANDR_ACTION    // the action
                    , pandRXDSBRequest);      // the body

                wcfOutput = WCF.Message.CreateMessage(WCF.MessageVersion.Soap12WSAddressing10, "");

                // 2) create the client proxy (using our own binding (instead of depending on a web.config))
                
                WSHttpBinding myBinding = new WSHttpBinding();
                // some basic binding properties, regardless of transport
                myBinding.MaxBufferPoolSize = 524288; 
                myBinding.MaxReceivedMessageSize = 67108864;
                myBinding.MessageEncoding = WSMessageEncoding.Mtom;
                myBinding.ReaderQuotas.MaxArrayLength = 16384;
                myBinding.ReaderQuotas.MaxBytesPerRead = 8192;
                myBinding.ReaderQuotas.MaxDepth = 32;
                myBinding.ReaderQuotas.MaxNameTableCharCount = 46384;
                myBinding.ReaderQuotas.MaxStringContentLength = 20000;
                
                // tls specifics
                if (clientCert != null)
                {
                    myBinding.Security.Mode = SecurityMode.Transport;
                    myBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }
                else
                {
                    myBinding.Security.Mode = SecurityMode.None;
                    myBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                }

                xdsRepClient = new XDSRepositoryClient(myBinding, endpointAddress);

                // tls certificate and callback
                if (clientCert != null)
                {
                    xdsRepClient.ClientCredentials.ClientCertificate.Certificate = clientCert;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(StaticHelpers.RemoteCertificateValidation);
                }

                // 3) send the message and get back the response
                // 
                //Logger.Debug("sending message");
                //using (StreamWriter sw = new StreamWriter("tempDubugging"))
                //{
                //    sw.Write(string.Format("message to send: {0}", wcfInput.ToString()));
                //}
                wcfOutput = xdsRepClient.ProvideAndRegisterDocumentSet(wcfInput);

                // 4) interrogate the repository/xdr recipient response and create a response object
                pandRResponse = interrogateWCFResponse(wcfOutput);
            }
            catch (Exception ex)
            {
                // good point for debug logging
                //Logger.Error(string.Format("exportXDSB catches error: {0}; stack: {1}", ex.Message, ex.StackTrace));

                pandRResponse = errorResponse(GlobalValues.CONST_ERROR_CODE_XDSRepositoryError, string.Format("error: {0}; stacktrace{1}", ex.Message, ex.StackTrace));
                //throw;
            }
            if (xdsRepClient.State == CommunicationState.Opened)
            {
                xdsRepClient.Close();
            }
            //Logger.Debug("end exportXDSB");
            return pandRResponse;
        }

        private ProvideAndRegisterResponse errorResponse(string errorCode, string contextText)
        {
            ProvideAndRegisterResponse pandRResponse = new ProvideAndRegisterResponse();
            pandRResponse.Status = GlobalValues.CONST_RESPONSE_STATUS_TYPE_FAILURE;  // unless proven otherwise
            pandRResponse.RegistryErrorList = new RegistryErrorList();
            pandRResponse.RegistryErrorList.HighestSeverity = GlobalValues.CONST_SEVERITY_TYPE_ERROR;
            pandRResponse.RegistryErrorList.RegistryErrors = new List<RegistryError>(1);
            pandRResponse.RegistryErrorList.RegistryErrors.Add(new RegistryError());
            pandRResponse.RegistryErrorList.RegistryErrors[0].Severity = GlobalValues.CONST_SEVERITY_TYPE_ERROR;
            pandRResponse.RegistryErrorList.RegistryErrors[0].ErrorCode = errorCode;
            pandRResponse.RegistryErrorList.RegistryErrors[0].CodeContext = contextText;
            return pandRResponse;
        }

        private ProvideAndRegisterResponse interrogateWCFResponse(Message wcfOutput)
        {
            // 
            //Logger.Debug("begin interrogateWCFResponse");
            ProvideAndRegisterResponse pandRResponse = new ProvideAndRegisterResponse();
            try
            {
                XmlDictionaryReader dictReader = wcfOutput.GetReaderAtBodyContents();
                XmlDocument resultsDOM = new XmlDocument();
                resultsDOM.Load(dictReader);

                // handle the response status
                pandRResponse.Status = resultsDOM.DocumentElement.Attributes.GetNamedItem("status").InnerXml;

                // add any registry errors
                XmlNode registryErrorList = resultsDOM.DocumentElement.SelectSingleNode("//*[local-name()='RegistryErrorList']");
                if (registryErrorList == null)
                {
                    pandRResponse.RegistryErrorList = null;
                }
                else  // process all of the registry errors
                {
                    XmlNodeList registryErrors = registryErrorList.SelectNodes("//*[local-name()='RegistryError']");
                    pandRResponse.RegistryErrorList = new RegistryErrorList();
                    pandRResponse.RegistryErrorList.RegistryErrors = new List<RegistryError>();
                    RegistryError theRegistryError = null;
                    XmlNode temp;

                    temp = registryErrorList.Attributes.GetNamedItem("highestSeverity");
                    if (temp != null)
                    {
                        pandRResponse.RegistryErrorList.HighestSeverity = temp.InnerXml;
                    }
                    else  // shouldn't happen....highest should be present
                    {
                        pandRResponse.RegistryErrorList.HighestSeverity = "";
                    } // fi highestSeverity attribute exists

                    foreach (XmlNode aRegError in registryErrors)
                    {
                        theRegistryError = new RegistryError();
                        temp = aRegError.Attributes.GetNamedItem("errorCode");
                        if (temp != null)
                        {
                            theRegistryError.ErrorCode = temp.InnerXml;
                        }
                        else  // should not happen....error code should be present
                        {
                            theRegistryError.ErrorCode = "";
                        } // fi

                        temp = aRegError.Attributes.GetNamedItem("codeContext");
                        if (temp != null)
                        {
                            theRegistryError.CodeContext = temp.InnerXml;
                        }
                        else
                        {
                            theRegistryError.CodeContext = "";  
                        } // fi

                        temp = aRegError.Attributes.GetNamedItem("location");
                        if (temp != null)
                        {
                            theRegistryError.Location = temp.InnerXml;
                        }
                        else
                        {
                            theRegistryError.Location = "";
                        } // fi

                        temp = aRegError.Attributes.GetNamedItem("severity");
                        if (temp != null)
                        {
                            theRegistryError.Severity = temp.InnerXml;
                        }
                        else
                        {
                            theRegistryError.Severity = "";
                        } // fi
                        
                        // highest severity
                        if (string.IsNullOrEmpty(pandRResponse.RegistryErrorList.HighestSeverity))
                        {
                            pandRResponse.RegistryErrorList.HighestSeverity = theRegistryError.Severity;
                        }
                        else
                        {
                            //
                            if (pandRResponse.RegistryErrorList.HighestSeverity != GlobalValues.CONST_SEVERITY_TYPE_ERROR)
                            {
                                pandRResponse.RegistryErrorList.HighestSeverity = theRegistryError.Severity;
                            }
                        }

                        // add to the registry error list
                        pandRResponse.RegistryErrorList.RegistryErrors.Add(theRegistryError);
                    }

                }  // fi some registry errors
            }
            catch (Exception ex)
            {
                //Logger.Error(string.Format("interrogateWCFResponse catches error: {0}; stack: {1}", ex.Message, ex.StackTrace));
                pandRResponse = errorResponse(GlobalValues.CONST_ERROR_CODE_XDSRepositoryError, string.Format("error: {0}; stacktrace{1}", ex.Message, ex.StackTrace));
                //throw;
            }
            //Logger.Debug("end interrogateWCFResponse");
            return pandRResponse;
        }

        #endregion
    }
}