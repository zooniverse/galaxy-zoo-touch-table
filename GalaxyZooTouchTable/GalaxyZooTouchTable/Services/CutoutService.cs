using GalaxyZooTouchTable.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.Services
{
    public class CutoutService
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

        public async Task<BitmapImage> GetSpaceCutout(double ra, double dec)
        {
            double plateScale = 1.75;
            BitmapImage image = null;
            //if (SDSSIsResponding)
            //{
            //    using (WebResponse response = await FetchSDSSCutout(ra, dec, plateScale))
            //        if (response != null)
            //            successfulResponse = response.ResponseUri.ToString();
            //}
            if (DECALSIsResponding && image == null)
            {
                using (WebResponse response = await FetchDECALSCutout(ra, dec, plateScale))
                    if (response != null)
                    {
                        string url = response.ResponseUri.ToString();
                        image = BitmapFromUrl(url);
                    }
            }
            //return CreateImage(successfulResponse);
            return image;
        }

        BitmapImage BitmapFromUrl(string url)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(url);
            image.EndInit();
            return image;
        }

        //BitmapImage StitchImagesTogether(SpaceNavigation currentLocation, double plateScale)
        //{
        //    double RaStep = (currentLocation.RaRange / 3);
        //    double RightImageCenterRa = currentLocation.Center.RightAscension - RaStep;
        //    double LeftImageCenterRa = currentLocation.Center.RightAscension + RaStep;
        //    double[] imageRAs = { LeftImageCenterRa, currentLocation.Center.RightAscension, RightImageCenterRa };
        //    List<string> urls = new List<string>();

        //    foreach (double ra in imageRAs)
        //    {
        //        string url = $"http://legacysurvey.org/viewer-dev/jpeg-cutout/?ra={ra}&dec={currentLocation.Center.Declination}&pixscale={plateScale}&layer=decals-dr7&size=432";
        //        urls.Add(url);
        //    }
        //    List<Bitmap> Bitmaps = ConvertUrlsToBitmaps(urls);

        //    var finalBitmap = new Bitmap(1248, 432);
        //    using (var g = Graphics.FromImage(finalBitmap))
        //    {
        //        int index = 0;
        //        foreach (var image in Bitmaps)
        //        {
        //            g.DrawImage(image, 0, 0);
        //        }
        //    }
        //    return ToBitmapImage(finalBitmap);
        //}

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

        Bitmap ConvertUrlToBitmap(string url)
        {
            WebClient wc = new WebClient();
            byte[] bytes = wc.DownloadData(url);
            using (var ms = new MemoryStream(bytes))
            {
                return new Bitmap(ms);
            }
        }

        private static List<Bitmap> ConvertUrlsToBitmaps(List<string> imageUrls, WebProxy proxy = null)
        {
            List<Bitmap> bitmapList = new List<Bitmap>();
            foreach (string imgUrl in imageUrls)
            {
                try
                {
                    WebClient wc = new WebClient();
                    // If proxy setting then set 
                    if (proxy != null)
                        wc.Proxy = proxy;
                    // Download image 
                    byte[] bytes = wc.DownloadData(imgUrl);
                    MemoryStream ms = new MemoryStream(bytes);
                    Image img = Image.FromStream(ms);
                    bitmapList.Add((Bitmap)img);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
            return bitmapList;
        }
    }
}
