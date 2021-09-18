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
           
            // Test
            // String motorbikeInfoJSON = "{\"articleCompleteInfo\":{\"makeName\":\"Yamaha\",\"modelName\":\"Yamaha YZF-R1\",\"categoryName\":\"Sport\",\"yearName\":2021},\"articleImage\":{\"imageName\":\"Yamaha YZF-R1 2021.jpg\",\"link\":\"http:\\/\\/api-motorcycle.makingdatameaningful.com\\/files\\/Yamaha\\/2021\\/Yamaha YZF-R1\\/Yamaha_2021_Yamaha YZF-R1.jpg\"},\"engineAndTransmission\":{\"id\":35301,\"displacementName\":\"998.0 ccm (60.90 cubic inches)\",\"engineTypeName\":\"In-line four, four-stroke\",\"engineDetailsName\":\"Crossplane crankshaft technology. Titanium intake valves.\",\"powerName\":\"200.0 HP (146.0  kW)) @ 13500 RPM\",\"torqueName\":\"112.4 Nm (11.5 kgf-m or 82.9 ft.lbs) @ 11500 RPM\",\"compressionName\":\"13.0:1\",\"boreXStrokeName\":\"79.0 x 50.9 mm (3.1 x 2.0 inches)\",\"valvesPerCylinderName\":\"4\",\"fuelSystemName\":\"Injection. Fuel Injection with YCC-T and YCC-I\",\"ignitionName\":\"TCI: Transistor Controlled Ignition\",\"coolingSystemName\":\"Liquid\",\"gearboxName\":\"6-speed\",\"transmissionTypeFinalDriveName\":\"Chain\",\"clutchName\":\"Multiplate assist and slipper clutch\",\"fuelConsumptionName\":\"7.13 litres\\/100 km (14.0 km\\/l or 32.99 mpg)\",\"greenhouseGasesName\":\"165.4 CO2 g\\/km. (CO2 - Carbon dioxide emission) \",\"exhaustSystemName\":\"Titanium Exhaust\"},\"chassisSuspensionBrakesAndWheels\":{\"id\":35301,\"frameTypeName\":\"Aluminum Deltabox\",\"frontBrakesName\":\"120\\/70-ZR17 \",\"frontBrakesDiameterName\":\"190\\/55-ZR17 \",\"frontSuspensionName\":\"102 mm (4.0 inches)\",\"frontTyreName\":\"KYB\\u00ae piggyback shock, 4-way adjustable\",\"frontWheelTravelName\":\"43mm KYB\\u00ae inverted fork\",\"rakeName\":\"24.0\\u0026deg\",\"rearBrakesName\":\"Double disc. ABS. Hydraulic. Four-piston calipers. \",\"rearSuspensionName\":\" fully adjustable\",\"rearTyreName\":\"119 mm (4.7 inches)\",\"rearWheelTravelName\":\"119 mm (4.7 inches)\",\"wheelsName\":\"Single disc. ABS.\"},\"physicalMeasuresAndCapacities\":{\"id\":35301,\"dryWeightName\":\"Bridgestone RS11 Tires\",\"frontPercentageOfWeightName\":\"856 mm (33.7 inches) If adjustable, lowest setting.\",\"groundClearanceName\":\"2055 mm (80.9 inches)\",\"oilCapacityName\":\"17.03 litres (4.50 gallons)\",\"overallWidthName\":\"1151 mm (45.3 inches)\",\"reserveFuelCapacityName\":\"1405 mm (55.3 inches)\",\"seatHeightName\":\"203.2 kg (448.0 pounds)\"},\"otherSpecifications\":{\"id\":35301,\"electricalName\":\"Electric\",\"factoryWarrantyName\":\"LED headlights\",\"instrumentsName\":\"Team Yamaha Blue, Raven\",\"lightName\":\"Colour TFT.\",\"modificationsComparedToPreviousModelName\":\"1 Year Limited Factory Warranty\",\"starterName\":\"3.90 litres (0.26 quarts)\"}}";
            
            
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
                EngineType = json["engineAndTransmission"]["engineTypeName"].ToString(),
                Power = json["engineAndTransmission"]["powerName"].ToString(),
                Torque = json["engineAndTransmission"]["torqueName"].ToString(),
                Displacement = json["engineAndTransmission"]["displacementName"].ToString(),
                Compression = json["engineAndTransmission"]["compressionName"].ToString(),
                BoreXStroke = json["engineAndTransmission"]["boreXStrokeName"].ToString(),
                FuelConsumption = json["engineAndTransmission"]["fuelConsumptionName"].ToString(),
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

            // Test String
            // String motorbikeInfoJSON = "{\"articleCompleteInfo\":{\"makeName\":\"Yamaha\",\"modelName\":\"Yamaha YZF-R1\",\"categoryName\":\"Sport\",\"yearName\":2021},\"articleImage\":{\"imageName\":\"Yamaha YZF-R1 2021.jpg\",\"link\":\"http:\\/\\/api-motorcycle.makingdatameaningful.com\\/files\\/Yamaha\\/2021\\/Yamaha YZF-R1\\/Yamaha_2021_Yamaha YZF-R1.jpg\"},\"engineAndTransmission\":{\"id\":35301,\"displacementName\":\"998.0 ccm (60.90 cubic inches)\",\"engineTypeName\":\"In-line four, four-stroke\",\"engineDetailsName\":\"Crossplane crankshaft technology. Titanium intake valves.\",\"powerName\":\"200.0 HP (146.0  kW)) @ 13500 RPM\",\"torqueName\":\"112.4 Nm (11.5 kgf-m or 82.9 ft.lbs) @ 11500 RPM\",\"compressionName\":\"13.0:1\",\"boreXStrokeName\":\"79.0 x 50.9 mm (3.1 x 2.0 inches)\",\"valvesPerCylinderName\":\"4\",\"fuelSystemName\":\"Injection. Fuel Injection with YCC-T and YCC-I\",\"ignitionName\":\"TCI: Transistor Controlled Ignition\",\"coolingSystemName\":\"Liquid\",\"gearboxName\":\"6-speed\",\"transmissionTypeFinalDriveName\":\"Chain\",\"clutchName\":\"Multiplate assist and slipper clutch\",\"fuelConsumptionName\":\"7.13 litres\\/100 km (14.0 km\\/l or 32.99 mpg)\",\"greenhouseGasesName\":\"165.4 CO2 g\\/km. (CO2 - Carbon dioxide emission) \",\"exhaustSystemName\":\"Titanium Exhaust\"},\"chassisSuspensionBrakesAndWheels\":{\"id\":35301,\"frameTypeName\":\"Aluminum Deltabox\",\"frontBrakesName\":\"120\\/70-ZR17 \",\"frontBrakesDiameterName\":\"190\\/55-ZR17 \",\"frontSuspensionName\":\"102 mm (4.0 inches)\",\"frontTyreName\":\"KYB\\u00ae piggyback shock, 4-way adjustable\",\"frontWheelTravelName\":\"43mm KYB\\u00ae inverted fork\",\"rakeName\":\"24.0\\u0026deg\",\"rearBrakesName\":\"Double disc. ABS. Hydraulic. Four-piston calipers. \",\"rearSuspensionName\":\" fully adjustable\",\"rearTyreName\":\"119 mm (4.7 inches)\",\"rearWheelTravelName\":\"119 mm (4.7 inches)\",\"wheelsName\":\"Single disc. ABS.\"},\"physicalMeasuresAndCapacities\":{\"id\":35301,\"dryWeightName\":\"Bridgestone RS11 Tires\",\"frontPercentageOfWeightName\":\"856 mm (33.7 inches) If adjustable, lowest setting.\",\"groundClearanceName\":\"2055 mm (80.9 inches)\",\"oilCapacityName\":\"17.03 litres (4.50 gallons)\",\"overallWidthName\":\"1151 mm (45.3 inches)\",\"reserveFuelCapacityName\":\"1405 mm (55.3 inches)\",\"seatHeightName\":\"203.2 kg (448.0 pounds)\"},\"otherSpecifications\":{\"id\":35301,\"electricalName\":\"Electric\",\"factoryWarrantyName\":\"LED headlights\",\"instrumentsName\":\"Team Yamaha Blue, Raven\",\"lightName\":\"Colour TFT.\",\"modificationsComparedToPreviousModelName\":\"1 Year Limited Factory Warranty\",\"starterName\":\"3.90 litres (0.26 quarts)\"}}";

            JObject json = JObject.Parse(motorbikeInfoJSON);


            motorbike.Make = input.Make;
            motorbike.Model = input.Model;
            motorbike.Year = input.Year;
            motorbike.YouTubeReviewLink = input.YouTubeReviewLink ?? motorbike.YouTubeReviewLink;
            motorbike.YouTubeThumbnailURL = GetVideoInfo(input.YouTubeReviewLink) ??  motorbike.YouTubeThumbnailURL;
            motorbike.Category = json["articleCompleteInfo"]["categoryName"].ToString();
            motorbike.ImageURL = checkForSpaces(json["articleImage"]["link"].ToString());
            motorbike.EngineType = json["engineAndTransmission"]["engineTypeName"].ToString();
            motorbike.Power = json["engineAndTransmission"]["powerName"].ToString();
            motorbike.Torque = json["engineAndTransmission"]["torqueName"].ToString();
            motorbike.Displacement = json["engineAndTransmission"]["displacementName"].ToString();
            motorbike.Compression = json["engineAndTransmission"]["compressionName"].ToString();
            motorbike.BoreXStroke = json["engineAndTransmission"]["boreXStrokeName"].ToString();
            motorbike.FuelConsumption = json["engineAndTransmission"]["fuelConsumptionName"].ToString();
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
