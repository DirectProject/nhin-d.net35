﻿/* 
 Copyright (c) 2010, NHIN Direct Project
 All rights reserved.

 Authors:
    Umesh Madan     umeshma@microsoft.com
 
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of the The NHIN Direct Project (nhindirect.org). nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using DnsResolver;

namespace DnsResponder
{
    public class DnsServer
    {
        DnsServerSettings m_settings;
        WorkThrottle m_activeRequestThrottle;
        IDnsStore m_store;
        
        TcpServer m_tcpServer;
        
        DnsResponderTCP m_tcpResponder;
        DnsResponderUDP m_udpResponder;
        
        public DnsServer(IDnsStore store, DnsServerSettings settings)
        {
            if (store == null || settings == null)
            {
                throw new ArgumentNullException();
            }

            m_settings = settings;
            m_activeRequestThrottle = new WorkThrottle(m_settings.ServerSettings.MaxActiveRequests);
            m_store = store;

            m_tcpResponder = new DnsResponderTCP(this);
            m_udpResponder = new DnsResponderUDP(this);

            m_tcpServer = new TcpServer(m_settings.Endpoint, m_settings.ServerSettings, m_tcpResponder);            
        }
                
        public DnsServerSettings Settings
        {
            get
            {
                return m_settings;
            }
        }
        
        public TcpServer TCPServer
        {
            get
            {
                return m_tcpServer;
            }
        }
        
        public IDnsStore Store
        {
            get
            {
                return m_store;
            }
        }
        
        public DnsResponderTCP TCPResponder
        {
            get
            {
                return m_tcpResponder;
            }
        }
        
        public DnsResponderUDP UDPResponsder
        {
            get
            {
                return m_udpResponder;
            }
        }
        
        public void Start()
        {
            m_tcpServer.Start();
        }
        
        public void Stop()
        {
            m_tcpServer.Stop();
        }
    }
}
