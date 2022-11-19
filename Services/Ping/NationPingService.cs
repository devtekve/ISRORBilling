using ISRORBilling.Models.Ping;

using Microsoft.Extensions.Options;

using System.Buffers;
using System.Net;
using System.Net.Sockets;

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

            return ProcessAccept(cancellationToken);
        }

        private async Task ProcessAccept(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var socket = await _tcpListener.AcceptSocketAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogDebug("[{clientEndPoint}]: connected.", socket.RemoteEndPoint);

                _ = ProcessSocket(socket, cancellationToken);
            }
        }

        private static async Task ProcessSocket(Socket socket, CancellationToken cancellationToken)
        {
            await using var stream = new NetworkStream(socket, true);

            var buffer = ArrayPool<byte>.Shared.Rent(14);
            var memory = new Memory<byte>(buffer)[..14];
            try
            {
                await stream.ReadExactlyAsync(memory, cancellationToken).ConfigureAwait(false);
                if (buffer[2] != (byte)'R' || buffer[3] != (byte)'E' || buffer[4] != (byte)'Q' || buffer[5] != (byte)'\0')
                    return;

                buffer[2] = (byte)'A';
                buffer[3] = (byte)'C';
                buffer[4] = (byte)'K';
                buffer[5] = (byte)'\0';

                await stream.WriteAsync(memory, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}