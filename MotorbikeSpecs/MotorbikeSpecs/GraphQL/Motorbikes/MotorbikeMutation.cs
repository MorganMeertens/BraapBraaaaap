using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.Extensions;
using HotChocolate;
using MotorbikeSpecs.Model;
using System.Threading;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using RestSharp;

namespace MotorbikeSpecs.GraphQL.Motorbikes
{
    [ExtendObjectType(name: "Mutation")]
    public class MotorbikeMutations
    {
        [UseBraapDbContext]
        public async Task<Motorbike> AddMotorbikeAsync(AddMotorbikeInput input,
        [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
        {
            //Call Motorbike API for specs (Careful there are only 100 calls/month
            string noSpaceModel = checkForSpaces(input.Model);

            string apiCallURL = "https://motorcycle-specs-database.p.rapidapi.com/article/" + input.Year + "/" + input.Make + "/" + input.Make + "%20" + noSpaceModel;

             string API_KEY = GetBraapAPIKey();
             var client = new HttpClient();
             var request = new HttpRequestMessage
             {
                 Method = HttpMethod.Get,
                 RequestUri = new Uri(apiCallURL),
                 Headers =
                                 {
                                     { "x-rapidapi-host", "motorcycle-specs-database.p.rapidapi.com" },
                                     { "x-rapidapi-key", API_KEY },
                                 },
             };
             var response = await client.SendAsync(request);
             response.EnsureSuccessStatusCode();
             var motorbikeInfoJSON = await response.Content.ReadAsStringAsync();

            // Test string to determine what to access in debug
            //String motorbikeInfoJSON = "{\"articleCompleteInfo\":{\"makeName\":\"Kawasaki\",\"modelName\":\"Kawasaki Ninja ZX-6R\",\"categoryName\":\"Sport\",\"yearName\":2021},\"articleImage\":{\"imageName\":\"Kawasaki Ninja ZX-6R 2021.jpg\",\"link\":\"http:\\/\\/api-motorcycle.makingdatameaningful.com\\/files\\/Kawasaki\\/2021\\/Kawasaki Ninja ZX-6R\\/Kawasaki_2021_Kawasaki Ninja ZX-6R.jpg\"},\"engineAndTransmission\":{\"id\":35155,\"displacementName\":\"636.0 ccm (38.81 cubic inches)\",\"engineTypeName\":\"In-line four, four-stroke\",\"powerName\":\"126.2 HP (92.1  kW)) @ 13500 RPM\",\"torqueName\":\"70.8 Nm (7.2 kgf-m or 52.2 ft.lbs) @ 11000 RPM\",\"compressionName\":\"12.9:1\",\"boreXStrokeName\":\"67.0 x 45.1 mm (2.6 x 1.8 inches)\",\"valvesPerCylinderName\":\"4\",\"fuelSystemName\":\"Injection. DFI\\u00ae with 38mm Keihin throttle bodies (4) and oval sub-throttles\",\"ignitionName\":\"TCBI with Digital Advance\",\"lubricationSystemName\":\"Forced lubrication, wet sump with oil cooler\",\"coolingSystemName\":\"Liquid\",\"gearboxName\":\"6-speed\",\"transmissionTypeFinalDriveName\":\"Chain\",\"clutchName\":\"Wet multi-disc, manual\",\"drivelineName\":\"Sealed Chain\"},\"chassisSuspensionBrakesAndWheels\":{\"id\":35155,\"frameTypeName\":\"Tubular, diamond frame\",\"frontBrakesName\":\"180\\/55-ZR17 \",\"frontBrakesDiameterName\":\"Double disc. Floating discs. Optional ABS.\",\"frontSuspensionName\":\"102 mm (4.0 inches)\",\"frontTyreName\":\"134 mm (5.3 inches)\",\"frontWheelTravelName\":\"41mm inverted Showa SFF-BP fork with top-out springs, stepless compression and rebound damping, adjustable spring preload\",\"rakeName\":\"23.5\\u0026deg\",\"rearBrakesDiameterName\":\"Single disc. Optional ABS.\",\"rearSuspensionName\":\"119 mm (4.7 inches)\",\"rearTyreName\":\"120\\/70-ZR17 \",\"rearWheelTravelName\":\"Bottom-Link Uni-Trak with gas-charged shock, top-out spring and pillow ball upper mount. Compression damping\"},\"physicalMeasuresAndCapacities\":{\"id\":35155,\"alternateSeatHeightName\":\"830 mm (32.7 inches) If adjustable, lowest setting.\",\"fuelCapacityName\":\"1400 mm (55.1 inches)\",\"groundClearanceName\":\"710 mm (28.0 inches)\",\"overallLengthName\":\"1100 mm (43.3 inches)\",\"overallWidthName\":\"2025 mm (79.7 inches)\",\"powerWeightRatioName\":\"195.0 kg (430.0 pounds)\",\"reserveFuelCapacityName\":\"17.00 litres (4.49 gallons)\"},\"otherSpecifications\":{\"id\":35155,\"commentsName\":\"12 Month Limited Warranty\",\"instrumentsName\":\"Electric\",\"modificationsComparedToPreviousModelName\":\"Small windscreen. Kawasaki Traction Control (KTRC), Power Mode, Optional Kawasaki Intelligent anti-lock Brake System (KIBS).\",\"starterName\":\"Pearl Nightshade Teal\\/Metallic Spark Black, Pearl Crystal White\\/Pearl Storm Gray\\/Ebony\"}}";
            JObject json = JObject.Parse(motorbikeInfoJSON);


            var motorbike = new Motorbike
            {

                Make = input.Make,
                Model = input.Model,
                Year = input.Year,
                YouTubeReviewLink = input.YouTubeReviewLink,
                YouTubeThumbnailURL = GetVideoInfo(input.YouTubeReviewLink),
                Category = json["articleCompleteInfo"]["categoryName"].ToString(),
                ImageURL = checkForSpaces(json["articleImage"]["link"].ToString()),


                //check specs
                EngineType = checkIfEmpty(json, "engineTypeName"),
                Power = checkIfEmpty(json, "powerName"),
                Torque = checkIfEmpty(json, "torqueName"),
                Displacement = checkIfEmpty(json, "displacementName"),
                Compression = checkIfEmpty(json, "compressionName"),
                BoreXStroke = checkIfEmpty(json, "boreXStrokeName"),
                FuelConsumption = checkIfEmpty(json, "fuelConsumptionName"),


                CompanyId = int.Parse(input.CompanyId),
                Modified = DateTime.Now,
                Created = DateTime.Now,

            };

            context.Motorbikes.Add(motorbike);
            await context.SaveChangesAsync(cancellationToken);

            return motorbike;
        }

        [UseBraapDbContext]
        public async Task<Motorbike> EditMotorbikeAsync(EditMotorbikeInput input,
                [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
        {

            var motorbike = await context.Motorbikes.FindAsync(int.Parse(input.MotorbikeId));

            string noSpaceModel = checkForSpaces(input.Model);

            string apiCallURL = "https://motorcycle-specs-database.p.rapidapi.com/article/" + input.Year + "/" + input.Make + "/" + input.Make + "%20" + noSpaceModel;

            string API_KEY = GetBraapAPIKey();

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(apiCallURL),
                Headers =
                                {
                                    { "x-rapidapi-host", "motorcycle-specs-database.p.rapidapi.com" },
                                    { "x-rapidapi-key", API_KEY },
                                },
            };
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var motorbikeInfoJSON = await response.Content.ReadAsStringAsync();

            
            JObject json = JObject.Parse(motorbikeInfoJSON);


            motorbike.Make = input.Make;
            motorbike.Model = input.Model;
            motorbike.Year = input.Year;
            motorbike.YouTubeReviewLink = input.YouTubeReviewLink ?? motorbike.YouTubeReviewLink;
            motorbike.YouTubeThumbnailURL = GetVideoInfo(input.YouTubeReviewLink) ?? motorbike.YouTubeThumbnailURL;
            motorbike.Category = json["articleCompleteInfo"]["categoryName"].ToString();
            motorbike.ImageURL = checkForSpaces(json["articleImage"]["link"].ToString());

            //check specs
            motorbike.EngineType = checkIfEmpty(json, "engineTypeName") ?? motorbike.EngineType;
            motorbike.Power = checkIfEmpty(json, "powerName") ?? motorbike.Power;
            motorbike.Torque = checkIfEmpty(json, "torqueName") ?? motorbike.Torque;
            motorbike.Displacement = checkIfEmpty(json, "displacementName") ?? motorbike.Displacement;
            motorbike.Compression = checkIfEmpty(json, "compressionName") ?? motorbike.Compression;
            motorbike.BoreXStroke = checkIfEmpty(json, "boreXStrokeName") ?? motorbike.BoreXStroke;
            motorbike.FuelConsumption = checkIfEmpty(json, "fuelConsumptionName") ?? motorbike.FuelConsumption;
            motorbike.Modified = DateTime.Now;


            await context.SaveChangesAsync(cancellationToken);

            return motorbike;
        }




        //Check the model name for spaces
        public static string checkForSpaces(string Name)
        {

            string newString = "";  // MUST set the Regex result to a variable for it to take effect
            newString = Regex.Replace(Name, @"\s+", "%20"); //Replaces all(+) space characters (\s) with empty("")

            return newString;

        }


        public static string checkIfEmpty(JObject motorbikeObject, string engineAttribute)
        {

            if ((motorbikeObject["engineAndTransmission"]?[engineAttribute] != null) && (motorbikeObject["engineAndTransmission"][engineAttribute].ToString() != null))
            {
                return motorbikeObject["engineAndTransmission"][engineAttribute].ToString();
            }
            else
            {
                return " "; //If spec is not in database (not expected) then fill spec with a space
            }

        }





        public static String GetVideoInfo(String videoURL)
        {
            if (string.IsNullOrEmpty(videoURL))
            {
                return null;
            }
            else
            {

                // Extract the string after the '=' sign
                // e.g. https://www.youtube.com/watch?v=ehvz3iN8pp4 becomes ehvz3iN8pp4 
                int indexOfFirstId = videoURL.IndexOf("=") + 1;
                String videoId = videoURL.Substring(indexOfFirstId);

                String APIKey = GetYoutubeAPIKey();
                String YouTubeAPIURL = "https://www.googleapis.com/youtube/v3/videos?id=" + videoId + "&key=" + APIKey + "&part=snippet,contentDetails";

                // Use an http client to grab the JSON string from the web.
                String videoInfoJSON = new WebClient().DownloadString(YouTubeAPIURL);

                // Using dynamic object helps us to more effciently extract infomation from a large JSON String.
                dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(videoInfoJSON);

                String thumbnailURL = jsonObj["items"][0]["snippet"]["thumbnails"]["medium"]["url"];

                return thumbnailURL;
            }
        }

        public static String GetBraapAPIKey()
        {
            var keyVaultUrl = "https://braap-apikey.vault.azure.net/";
            var credential = new DefaultAzureCredential();
            var client = new SecretClient(vaultUri: new Uri(keyVaultUrl), credential);
            KeyVaultSecret secret = client.GetSecret("BraapAPIKey");

            return secret.Value;
        }

        public static String GetYoutubeAPIKey()
        {
            var keyVaultUrl = "https://braap-apikey.vault.azure.net/";
            var credential = new DefaultAzureCredential();
            var client = new SecretClient(vaultUri: new Uri(keyVaultUrl), credential);
            KeyVaultSecret secret = client.GetSecret("YoutubeAPIKey");

            return secret.Value;
        }

    }
}
