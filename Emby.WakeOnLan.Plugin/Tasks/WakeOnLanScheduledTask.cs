using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emby.WakeOnLan.Plugin.Common;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Tasks;

namespace Emby.WakeOnLan.Plugin.Tasks
{
    public class WakeOnLanScheduledTask : IScheduledTask
    {
        private readonly ILogger logger;

        public WakeOnLanScheduledTask(ILogger logger)
        {
            this.logger = logger;
        }

        public string Category
        {
            get { return "Wake My Server"; }
        }

        public string Key
        {
            get { return "WakeOnLan"; }
        }

        public string Description
        {
            get { return "Wake On Lan"; }
        }

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            // Parse string to byte array
            int port = Plugin.Instance.Configuration.WakeOnLanPort;
            byte[] mac = null;
            string[] macDigits = null;
            string macAddress = Plugin.Instance.Configuration.HostMacAddress;

            if (macAddress.Contains("-"))
            {
                macDigits = macAddress.Split('-');
            }
            else
            {
                macDigits = macAddress.Split(':');
            }

            if (macDigits.Length != 6)
            {
                this.logger.Warn("Incorrect MAC address");
                return;
            }
            else
            {
                mac = macDigits.Select(s => Convert.ToByte(s, 16)).ToArray();
            }


            this.logger.Info($"Waking on MAC: {BitConverter.ToString(mac)}");

            int counter = 0;

            byte[] bytes = new byte[6 * 17];
            for (var i = 0; i < 6; i++)
            {
                bytes[counter++] = 0xFF;
            }

            //16x MAC
            for (var i = 0; i < 16; i++)
            {
                mac.CopyTo(bytes, 6 + i * 6);
            }

            progress.Report(5);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var client = new UdpClient())
                {
                    client.EnableBroadcast = true;
                    await client.SendAsync(bytes, bytes.Length, new IPEndPoint(new IPAddress(0xffffffff), port))
                        .WithCancellation(cancellationToken)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.logger.ErrorException($"Exception during wake on lan request.", ex);
            }
            
            progress.Report(100);
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
                {
                    new TaskTriggerInfo
                    {
                        Type = TaskTriggerInfo.TriggerDaily,
                        TimeOfDayTicks = TimeSpan.FromHours(12).Ticks,
                        MaxRuntimeTicks = TimeSpan.FromMinutes(5).Ticks
                    }
                };
        }

        public string Name
        {
            get { return "Wake On Lan"; }
        }
    }
}