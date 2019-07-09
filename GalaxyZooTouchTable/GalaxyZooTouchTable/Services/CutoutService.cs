using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
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
            SDSSIsResponding = false;
            SDSSTimer.Start();
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
                        cutout.ImageOne = BitmapFromUrl(response.ResponseUri.ToString());
                        cutout.ImageTwo = BitmapFromUrl(LeftDECALImage(location, plateScale));
                        cutout.ImageThree = BitmapFromUrl(RightDECALImage(location, plateScale));
                        cutout.IsDECALS = true;
                    }
            }
            if (SDSSIsResponding && cutout.ImageOne == null)
            {
                using (WebResponse response = await FetchSDSSCutout(ra, dec, plateScale))
                    if (response != null)
                    {
                        cutout.ImageOne = BitmapFromUrl(response.ResponseUri.ToString());
                        cutout.IsSDSS = true;
                    }
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

        string LeftDECALImage(SpaceNavigation currentLocation, double plateScale)
        {
            double RaStep = (currentLocation.RaRange / 3) + (currentLocation.RaRange * 0.049);
            double ra = Math.Round(currentLocation.Center.RightAscension + RaStep, 3);
            double declination = Math.Round(currentLocation.Center.Declination, 3);
            return $"http://legacysurvey.org/viewer-dev/jpeg-cutout/?ra={ra}&dec={declination}&pixscale={plateScale}&layer=decals-dr7&size=432";
        }

        string RightDECALImage(SpaceNavigation currentLocation, double plateScale)
        {
            double RaStep = (currentLocation.RaRange / 3) + (currentLocation.RaRange * 0.049);
            double ra = Math.Round(currentLocation.Center.RightAscension - RaStep, 3);
            double declination = Math.Round(currentLocation.Center.Declination, 3);
            return $"http://legacysurvey.org/viewer-dev/jpeg-cutout/?ra={ra}&dec={declination}&pixscale={plateScale}&layer=decals-dr7&size=432";
        }

        async Task<BitmapImage> StitchImagesTogether(SpaceNavigation currentLocation, double plateScale, double declination)
        {
            //TODO: Remove 0.049 constant below when finding cutouts directly aside one another
            double RaStep = (currentLocation.RaRange / 3) + (currentLocation.RaRange * 0.049);
            double RightImageCenterRa = Math.Round(currentLocation.Center.RightAscension - RaStep, 3);
            double LeftImageCenterRa = Math.Round(currentLocation.Center.RightAscension + RaStep, 3);
            double[] imageRAs = { LeftImageCenterRa, Math.Round(currentLocation.Center.RightAscension, 3), RightImageCenterRa };
            List<Bitmap> bitmaps = new List<Bitmap>();

            foreach (double ra in imageRAs)
            {
                string url = $"http://legacysurvey.org/viewer-dev/jpeg-cutout/?ra={ra}&dec={declination}&pixscale={plateScale}&layer=decals-dr7&size=432";
                bitmaps.Add(await ReturnBitmap(url));
            }

            Bitmap finalBitmap = new Bitmap(1248, 432);
            using (Graphics g = Graphics.FromImage(finalBitmap))
            {
                int index = 0;
                foreach (var image in bitmaps)
                {
                    if (index == 0)
                        g.DrawImage(image, 0, 0);
                    if (index == 1)
                        g.DrawImage(image, 408, 0);
                    if (index == 2)
                        g.DrawImage(image, 816, 0);
                    index++;
                }
            }
            return ToBitmapImage(finalBitmap);
        }

        BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        async Task<Bitmap> ReturnBitmap(string url)
        {
            Bitmap bitmap = null;
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var memStream = new MemoryStream();
                        await stream.CopyToAsync(memStream);
                        bitmap = new Bitmap(memStream);
                    }
                }
            }
            return bitmap;
        }
    }
}
