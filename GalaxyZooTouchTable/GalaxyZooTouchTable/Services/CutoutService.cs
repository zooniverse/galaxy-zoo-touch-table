using System;
using System.Net;

namespace GalaxyZooTouchTable.Services
{
    public class CutoutService : ICutoutService
    {
        bool FetchDECALSCutout(double ra, double dec, double plateScale)
        {
            bool isSuccessfulResponse = false;

            try
            {
                string url = $"http://legacysurvey.org/viewer-dev/jpeg-cutout/?ra={ra}&dec={dec}&pixscale={plateScale}&layer=decals-dr7&size=432";
                var request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                isSuccessfulResponse = response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return isSuccessfulResponse;
        }

        bool FetchSDSSCutout(double ra, double dec, double plateScale)
        {
            bool isSuccessfulResponse = false;
            try
            {
                string url = $"http://skyserver.sdss.org/dr14/SkyServerWS/ImgCutout/getjpeg?ra={ra}&dec={dec}&width=1248&height=432&scale={plateScale}";
                var request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                isSuccessfulResponse = response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return isSuccessfulResponse;
        }

        public bool GetSpaceCutout(double ra, double dec, double plateScale)
        {
            return FetchSDSSCutout(ra, dec, plateScale);
        }
    }
}
