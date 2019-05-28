using System;
using System.Net;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public class CutoutService : ICutoutService
    {
        bool SDSSIsResponding { get; set; } = true;
        bool DECALSIsResponding { get; set; } = true;

        async Task<WebResponse> FetchDECALSCutout(double ra, double dec, double plateScale)
        {
            WebResponse response;
            try
            {
                string url = $"http://legacysurvey.org/viewer-dev/jpeg-cutout/?ra={ra}&dec={dec}&pixscale={plateScale}&layer=decals-dr7&size=432";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 1000;
                response = await request.GetResponseAsync();
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
                response = (HttpWebResponse)e.Response;
                DECALSIsResponding = false;
            }
            return response;
        }

        async Task<WebResponse> FetchSDSSCutout(double ra, double dec, double plateScale)
        {
            WebResponse response;
            try
            {
                string url = $"http://skyserver.sdss.org/dr14/SkyServerWS/ImgCutout/getjpeg?ra={ra}&dec={dec}&width=1248&height=432&scale={plateScale}";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 1000;
                response = await request.GetResponseAsync();
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
                response = (HttpWebResponse)e.Response;
                SDSSIsResponding = false;
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
