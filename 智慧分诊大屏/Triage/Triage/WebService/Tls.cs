using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Tls;

namespace WaitingRoomBigScreen.WebService
{
    internal class MyTlsClient : DefaultTlsClient
    {
        public override TlsAuthentication GetAuthentication()
        {
            return new MyTlsAuthentication();
        }
    }

    // Need class to handle certificate auth
    internal class MyTlsAuthentication : TlsAuthentication
    {
        public TlsCredentials GetClientCredentials(CertificateRequest certificateRequest)
        {
            // return client certificate
            return null;
        }

        public void NotifyServerCertificate(Certificate serverCertificate)
        {
            // validate server certificate
        }
    }
}
