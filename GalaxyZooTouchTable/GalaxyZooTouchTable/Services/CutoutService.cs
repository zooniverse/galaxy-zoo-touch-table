using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.Services
{
    public class CutoutService : ICutoutService
    {
        readonly TimeSpan TimeUntilReset = new TimeSpan(0, 10, 0);
        bool SDSSIsResponding { get; set; } = true;
        bool DECALSIsResponding { get; set; } = true;
        DispatcherTimer SDSSTimer { get; set; } = new DispatcherTimer();
        DispatcherTimer DECALSTimer { get; set; } = new DispatcherTimer();

        public CutoutService()
        {
            SDSSTimer.Tick += new EventHandler(ResetSDSS);
            DECALSTimer.Tick += new EventHandler(ResetDECALS);
            SDSSTimer.Interval = TimeUntilReset;
            DECALSTimer.Interval = TimeUntilReset;
        }

        public void TemporarilyDisableSDSS()
        {
            SDSSIsResponding = false;
            SDSSTimer.Start();
        }

        public void TemporarilyDisableDECALS()
        {
            DECALSIsResponding = false;
            DECALSTimer.Start();
        }

        private void ResetSDSS(object sender, EventArgs e)
        {
            SDSSIsResponding = true;
            SDSSTimer.Stop();
        }

        private void ResetDECALS(object sender, EventArgs e)
        {
            DECALSIsResponding = true;
            DECALSTimer.Stop();
        }
        
        async Task<WebResponse> FetchDECALSCutout(double ra, double dec, double plateScale)
        {
            WebResponse response = null;
            try
            {
                string url = $"http://legacysurvey.org/viewer-dev/jpeg-cutout/?ra={ra}&dec={dec}&pixscale={plateScale}&layer=decals-dr7&size=432";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 1000;
                response = await request.GetResponseAsync();
            }
            catch (WebException e)
            {
                Console.WriteLine($"Unable to Connect to DECALS: {e.Message}");
                TemporarilyDisableDECALS();
            }
            return response;
        }

        async Task<WebResponse> FetchSDSSCutout(double ra, double dec, double plateScale)
        {
            WebResponse response = null;
            try
            {
                string url = $"http://skyserver.sdss.org/dr14/SkyServerWS/ImgCutout/getjpeg?ra={ra}&dec={dec}&width=1248&height=432&scale={plateScale}";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 1000;
                response = await request.GetResponseAsync();
            }
            catch (WebException e)
            {
                Console.WriteLine($"Unable to Connect to SDSS: {e.Message}");
                TemporarilyDisableSDSS();
            }
            return response;
        }

        public async Task<SpaceCutout> GetSpaceCutout(SpaceNavigation location)
        {
            double plateScale = 1.75;
            double ra = Math.Round(location.Center.RightAscension, 3);
            double dec = Math.Round(location.Center.Declination, 3);
            SpaceCutout cutout = new SpaceCutout();

            if (DECALSIsResponding)
            {
                using (WebResponse response = await FetchDECALSCutout(ra, dec, plateScale))
                    if (response != null)
                    {
                        double RaStep = (location.RaRange / 3) + (location.RaRange * 0.049);
                        double leftRA = Math.Round(location.Center.RightAscension + RaStep, 3);
                        double rightRA = Math.Round(location.Center.RightAscension - RaStep, 3);

                        cutout.ImageOne = BitmapFromUrl(response.ResponseUri.ToString());
                        cutout.ImageTwo = BitmapFromUrl(SideDECALImage(plateScale, dec, leftRA));
                        cutout.ImageThree = BitmapFromUrl(SideDECALImage(plateScale, dec, rightRA));
                    }
            }
            if (SDSSIsResponding && cutout.ImageOne == null)
            {
                using (WebResponse response = await FetchSDSSCutout(ra, dec, plateScale))
                    if (response != null)
                        cutout.ImageOne = BitmapFromUrl(response.ResponseUri.ToString());
            }
            return cutout;
        }

        BitmapImage BitmapFromUrl(string url)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(url);
            image.EndInit();
            return image;
        }

        string SideDECALImage(double plateScale, double declination, double ra)
        {
            return $"http://legacysurvey.org/viewer-dev/jpeg-cutout/?ra={ra}&dec={declination}&pixscale={plateScale}&layer=decals-dr7&size=432";
        }
    }
}
