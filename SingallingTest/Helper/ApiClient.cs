using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingallingTest
{
    public class ApiClient
    {
        public string GetDeviceInfo()
        {
            var deviceInfo = new DeviceInfo
            {
                AppInfo = new AppInfo
                {
                    Env = new Env
                    {
                        ClientAppId = "www.xbox.com",
                        ClientAppType = "browser",
                        ClientAppVersion = "21.1.98",
                        ClientSdkVersion = "8.5.3",
                        HttpEnvironment = "prod",
                        SdkInstallId = ""
                    }
                },
                Dev = new Dev
                {
                    Hw = new Hw
                    {
                        Make = "Microsoft",
                        Model = "unknown",
                        SdkType = "web"
                    },
                    Os = new Os
                    {
                        Name = this._config.Force1080p ? "windows" : "android",
                        Ver = "22631.2715",
                        Platform = "desktop"
                    },
                    DisplayInfo = new DisplayInfo
                    {
                        Dimensions = new Dimensions
                        {
                            WidthInPixels = 1920,
                            HeightInPixels = 1080
                        },
                        PixelDensity = new PixelDensity
                        {
                            DpiX = 2,
                            DpiY = 2
                        }
                    },
                    Browser = new Browser
                    {
                        BrowserName = "chrome",
                        BrowserVersion = "119.0"
                    }
                }
            };

            return JsonConvert.SerializeObject(deviceInfo);
        }

        private Config _config = new Config { Force1080p = true }; // Example config

        private class Config
        {
            public bool Force1080p { get; set; }
        }

        private class DeviceInfo
        {
            public AppInfo AppInfo { get; set; }
            public Dev Dev { get; set; }
        }

        private class AppInfo
        {
            public Env Env { get; set; }
        }

        private class Env
        {
            public string ClientAppId { get; set; }
            public string ClientAppType { get; set; }
            public string ClientAppVersion { get; set; }
            public string ClientSdkVersion { get; set; }
            public string HttpEnvironment { get; set; }
            public string SdkInstallId { get; set; }
        }

        private class Dev
        {
            public Hw Hw { get; set; }
            public Os Os { get; set; }
            public DisplayInfo DisplayInfo { get; set; }
            public Browser Browser { get; set; }
        }

        private class Hw
        {
            public string Make { get; set; }
            public string Model { get; set; }
            public string SdkType { get; set; }
        }

        private class Os
        {
            public string Name { get; set; }
            public string Ver { get; set; }
            public string Platform { get; set; }
        }

        private class DisplayInfo
        {
            public Dimensions Dimensions { get; set; }
            public PixelDensity PixelDensity { get; set; }
        }

        private class Dimensions
        {
            public int WidthInPixels { get; set; }
            public int HeightInPixels { get; set; }
        }

        private class PixelDensity
        {
            public int DpiX { get; set; }
            public int DpiY { get; set; }
        }

        private class Browser
        {
            public string BrowserName { get; set; }
            public string BrowserVersion { get; set; }
        }
    }
}
