using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Net.Http.Client;
using Renci.SshNet;

namespace Docker.DotNet.SSH
{
    public class SSHCredentials : Credentials
    {
        public override HttpMessageHandler GetHandler(HttpMessageHandler innerHandler)
        {
            return innerHandler;
        }

        public override bool IsSshCredentials()
        {
            return true;
        }

        public override bool IsTlsCredentials()
        {
            return false;
        }

        public override ManagedHandler.StreamOpener GetStreamOpener()
        {
            // TODO
            return async (string host, int port, CancellationToken cancellationToken) => {
                // TODO username
                var authMethod = new PrivateKeyAuthenticationMethod("ubuntu", new PrivateKeyFile("/home/marek/test_rsa"));
                var connectionInfo = new ConnectionInfo(host, port, "ubuntu", authMethod);
                var client = new SshClient(connectionInfo);
                client.Connect();

                var shellStream = client.CreateShellStream("dumb-terminal", 1, 1, 1, 1, 50);
                shellStream.WriteLine("docker system dial-stdio");

                await Task.Delay(100);
                return shellStream;
            };
        }
    }
}
