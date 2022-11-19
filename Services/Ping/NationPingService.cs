using System.Net;
using System.Net.Sockets;
using ISRORBilling.Models.Ping;
using Microsoft.Extensions.Options;

namespace ISRORBilling.Services.Ping
{
    public class NationPingService : BackgroundService
    {
        private readonly ILogger<NationPingService> _logger;
        readonly NationPingServiceOptions _options;

        private readonly TcpListener _tcpListener;

        public NationPingService(ILogger<NationPingService> logger, IOptions<NationPingServiceOptions> options)
        {
            _logger = logger;
            _options = options.Value;

            if (!IPAddress.TryParse(_options.ListenAddress, out var address))
                address = Dns.GetHostEntry(_options.ListenAddress).AddressList.FirstOrDefault() ?? IPAddress.Loopback;

            _tcpListener = new TcpListener(address, _options.ListenPort);
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _tcpListener.Start();
            _logger.LogInformation(
                "Ping Service listening on [{ServiceOptionsListenAddress}:{ServiceOptionsListenPort}]",
                _options.ListenAddress, _options.ListenPort);
            return RequestHandler(cancellationToken);
        }

        private async Task RequestHandler(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var client = await _tcpListener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
                var innerCancellationToken = new CancellationTokenSource();
                var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, innerCancellationToken.Token);
                try
                {
                    await DoRequest(client, linkedCancellationToken.Token);
                }
                catch
                {
                    innerCancellationToken.Cancel();
                }
            }
        }

        private async Task DoRequest(TcpClient client, CancellationToken cancellationToken)
        {
            _logger.LogDebug("[{@ClientRemoteEndPoint}]: connected", client.Client.RemoteEndPoint);
            var stream = client.GetStream();
            while (!cancellationToken.IsCancellationRequested && client.Connected)
            {
                var received = new byte[14];
                await stream.ReadExactlyAsync(received, cancellationToken);

                received[2] = (byte)'A';
                received[3] = (byte)'C';
                received[4] = (byte)'K';

                await stream.WriteAsync(received, cancellationToken);
                client.Close();
            }

            stream.Close(); //If the GW wants to keep the session alive this should be after the while loop
        }
    }
}