using System;
using MediaBrowser.Model.Plugins;

namespace Emby.WakeOnLan.Plugin
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string HostMacAddress { get; set; }

        public int WakeOnLanPort { get; set; }
    }
}
