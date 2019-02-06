using System;
using System.Collections.Generic;
using System.IO;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Emby.WakeOnLan.Plugin
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages, IHasThumbImage
    {
        private static readonly Guid PluginId = new Guid("2d674448-e7ce-49da-b900-2f5956f4aad0");

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public static Plugin Instance
        {
            get; private set;
        }

        public override Guid Id
        {
            get { return PluginId; }
        }

        public override string Name
        {
            get { return "Wake-On-Lan Plugin"; }
        }

        public override string Description
        {
            get { return "Schedule / trigger wake-on-lan commands for servers"; }
        }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "Wake On Lan",
                    EmbeddedResourcePath = this.GetType().Namespace + ".UI.configuration.html"
                }
            };
        }

        public Stream GetThumbImage()
        {
            var type = this.GetType();
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".UI.wake_on_lan.png");
        }

        public ImageFormat ThumbImageFormat
        {
            get
            {
                return ImageFormat.Png;
            }
        }
    }
}