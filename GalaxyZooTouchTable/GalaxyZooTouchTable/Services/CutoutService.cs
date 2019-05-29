using System;
using System.Net;
using System.Threading.Tasks;
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
                DECALSIsResponding = false;
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

        public async Task<string> GetSpaceCutout(double ra, double dec, double plateScale)
        {
            string successfulResponse = null;
            if (SDSSIsResponding)
            {
                using (WebResponse response = await FetchSDSSCutout(ra, dec, plateScale))
                    if (response != null)
                        successfulResponse = response.ResponseUri.ToString();
            }
            if (DECALSIsResponding && successfulResponse == null)
            {
                using (WebResponse response = await FetchDECALSCutout(ra, dec, plateScale))
                    if (response != null)
                        successfulResponse = response.ResponseUri.ToString();
            }
            return successfulResponse;
        }
    }
}
