using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace COVID_19_UK_Bot
{
    public class CovidApi
    {
        public enum Location
        {
            UnitedKingdom,
            England,
            Scotland,
            NorthernIreland,
            Wales
        }

        public static async Task<string> AsyncGetMsgWithLatestDataByNation(Location location = Location.UnitedKingdom)
        {
            var filter = "filters=areaType=nation;areaName=";
            var structure =
                "structure=%7b%22date%22%3a%22date%22%2c%22areaName%22%3a%22areaName%22%2c%22areaCode%22%3a%22areaCode%22%2c%22newCasesByPublishDate%22%3a%22newCasesByPublishDate%22%2c%22cumCasesByPublishDate%22%3a%22cumCasesByPublishDate%22%2c%22newDeaths28DaysByPublishDate%22%3a%22newDeaths28DaysByPublishDate%22%2c%22cumDeaths28DaysByPublishDate%22%3a%22cumDeaths28DaysByPublishDate%22%7d";
            // Change filter
            switch (location)
            {
                case Location.UnitedKingdom:
                    filter = "filters=areaType=overview";
                    break;
                default:
                    filter += nameof(location).Replace(" ", "");
                    break;
            }

            filter += ";date=";
            var tmpFilter = filter + DateTime.UtcNow.ToString("yyyy-MM-dd");
            var response = await AsyncGet(GetUrl(tmpFilter, structure));
            if (string.IsNullOrWhiteSpace(response))
            {
                tmpFilter = filter + DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
                response = await AsyncGet(GetUrl(tmpFilter, structure));
            }

            if (string.IsNullOrWhiteSpace(response))
                return Text.SOMETHING_WRONG;
            var jo = JObject.Parse(response);
            var ja = jo["data"] as JArray;

            try
            {
                return $"{Text.QUEEN_SAYS}\n" +
                       $"Location: {ja[0]["areaName"]}\n" +
                       $"Date: {ja[0]["date"].ToString().ToBritishStyleDate()}\n" +
                       $"New Cases: {ja[0]["newCasesByPublishDate"]}\n" +
                       $"Total Cases: {ja[0]["cumCasesByPublishDate"]}\n" +
                       $"New deaths in 28 days positive: {ja[0]["newDeaths28DaysByPublishDate"]}\n" +
                       $"Total deaths in 28 days positive: {ja[0]["cumDeaths28DaysByPublishDate"]}";
            }
            catch
            {
                return Text.SOMETHING_WRONG;
            }
        }

        private static string GetUrl(string filter, string structure)
            => "https://api.coronavirus.data.gov.uk/v1/data?" + filter + "&" + structure;

        private static async Task<string> AsyncGet(string url)
        {
            try
            {
                var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Accept =
                    "application/json; application/xml; text/csv; application/vnd.PHE-COVID19.v1+json; application/vnd.PHE-COVID19.v1+xml";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                WebResponse responseObject =
                    await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse,
                        request);
                var responseStream = responseObject.GetResponseStream();
                var sr = new StreamReader(responseStream);
                return await sr.ReadToEndAsync();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}