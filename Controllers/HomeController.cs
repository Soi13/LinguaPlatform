using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LinguaPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using OpenTokSDK;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.SignalR;


namespace LinguaPlatform.Controllers
{
    public class HomeController : Controller
    {        
        private readonly ILogger<HomeController> _logger;
        ConnectDb cdb = new ConnectDb();
        IHubContext<Signal> hubContext;

        public HomeController(ILogger<HomeController> logger, IHubContext<Signal> hubContext)
        {
            _logger = logger;
            this.hubContext = hubContext;
        }

        //Function for getting response form Html link like a JSON object
        public string GetResponseText(string url, string postData)
        {
            string responseText = String.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = postData.Length;
            using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
            {
                sw.Write(postData);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }
            return responseText;
        }

       

        [Authorize]
        public IActionResult Index()
        {
            HttpContext.Session.SetString("UserId", User.FindFirst(ClaimTypes.NameIdentifier).Value); //Put UserId in session
            HttpContext.Session.SetString("UserFullName", cdb.GetUserName(User.FindFirst(ClaimTypes.NameIdentifier).Value)); //Put User name in session
            try
            {
                string user_ip = HttpContext.Connection.RemoteIpAddress.ToString(); //Getting user's IP

                dynamic GeoRequest = JObject.Parse(GetResponseText("http://ip-api.com/json/"+user_ip+"?fields=continent,continentCode,country,countryCode,region,regionName,city,district,zip,lat,lon,timezone,offset,currency,isp,org,asname,mobile,proxy,hosting,query", ""));
                string continent = GeoRequest.continent;
                string continentCode = GeoRequest.continentCode;
                string country = GeoRequest.country;
                string countryCode = GeoRequest.countryCode;
                string region = GeoRequest.region;
                string regionName = GeoRequest.regionName;
                string city = GeoRequest.city;
                string district = GeoRequest.district;
                string zip = GeoRequest.zip;
                float lat = GeoRequest.lat;
                float lon = GeoRequest.lon;
                string timezone = GeoRequest.timezone;
                int offset = GeoRequest.offset;
                string currency = GeoRequest.currency;
                string isp = GeoRequest.isp;
                string org = GeoRequest.org;
                string asname = GeoRequest.asname;
                bool mobile = GeoRequest.mobile;
                bool proxy = GeoRequest.proxy;
                bool hosting = GeoRequest.hosting;
                string query = GeoRequest.query;
                                
                cdb.InsertGeoData(User.FindFirst(ClaimTypes.NameIdentifier).Value, continent, continentCode, country, countryCode, region, regionName, city, district, zip, lat, lon, timezone, offset, currency, isp, org, asname, mobile, proxy, hosting, query);

            }
            catch (Exception ex)
            {
                cdb.InsertError(User.FindFirst(ClaimTypes.NameIdentifier).Value, ex.ToString());
            }


            return View();
        }

        [Authorize]
        public IActionResult Lessons()
        {            
            return View();
        }

        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public IActionResult Tests()
        {
            return View();
        }

        [Authorize]
        public IActionResult Materials()
        {          
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Connect2Session()
        {      
            //Get User Id from session
            string user_id = HttpContext.Session.GetString("UserId"); 

            //Get full user name from session
            string full_user_name = HttpContext.Session.GetString("UserFullName"); 


            if (!cdb.StatusTeacherInGroupTrue(user_id))
            {
                return new JsonResult(new { result = false });
            }
            else
            {
                //Get Session id of lesson
                string session_id = cdb.GetLessonSessionIdStudent(user_id);


                //Set the following constants with the API key and API secret that you receive when you sign up to use the OpenTok API:
                OpenTok opentok = new OpenTok(00000000, "00000000000000000000000000000000000");

                //Replace with meaningful metadata for the connection.
                string us_name = User.Identity.Name;
                string connectionMetadata = string.Format("username={0}, status=student", us_name);

                // Set live time of token
                double inTwoHours = (DateTime.UtcNow.Add(TimeSpan.FromHours(2)).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                // Generate a token. Use the Role value appropriate for the user.
                string token_ = opentok.GenerateToken(session_id, Role.PUBLISHER, expireTime: inTwoHours, connectionMetadata);

                int gr_id = cdb.GetGroupId4Student(user_id);
                string teacher_id = cdb.GetTeacherIdFromGroup(user_id);

                //Safe in session group Id during connecting to lesson. This is necessary for detecting Id group during disconnecting from session
                HttpContext.Session.SetInt32("GroupId", gr_id); //Put GroupId in session

                cdb.InsertInfoStartLessonStudent(gr_id, user_id);

                string[] data = new string[] { "check_st_wb", user_id };
                hubContext.Clients.User(teacher_id).SendAsync("Status_chk", data);

                string[] data1 = new string[] { "StreamId", user_id };
                hubContext.Clients.User(teacher_id).SendAsync("GetStreamId", data1);

                return new JsonResult(new { result = true, apiKey = 00000000, sessionId = session_id, token = token_, fullUserName = full_user_name });               
            }
        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DisconnectFromSession()
        {
            //Get User Id from database for getting SessionId of lesson from group
            string user_id = HttpContext.Session.GetString("UserId"); //Get User Id from session

            //Get Group Id from database for change status of teacher in a group
            int group_id = (int)HttpContext.Session.GetInt32("GroupId"); //Get Group Id from session

            //Delete key from session
            HttpContext.Session.Remove("GroupId");

           cdb.InsertInfoFinishLessonStudent(group_id, user_id);

            return new JsonResult(new { result = true });
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult WhiteBoard()
        {
            //Get User Id from database for getting whiteboardUrl from group
            string user_id = HttpContext.Session.GetString("UserId"); //Get User Id from session

            //Get whiteboard url and user's token for accessing to whiteboard
            string[] white_board_data = cdb.GetWhiteBoardUrlStudent(user_id);
            string full_whiteboard_url = white_board_data[0] + "t=" + white_board_data[1]+ "&chat=0&pins=0&people=0&video=0";
                        
            return new JsonResult(new { wbu = full_whiteboard_url });
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DisconnetTeacherAbnormally()
        {
            //Get User Id from database for getting whiteboardUrl from group
            string user_id = HttpContext.Session.GetString("UserId"); //Get User Id from session                       

            cdb.SetTeacherConnectedFalseAbnormally(user_id);

            return new JsonResult(new { result = true });
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetUserData()
        {
            Dictionary<string, string> usr_data = new Dictionary<string, string>();

            //Get User Id from database for getting personal data of user
            string user_id = HttpContext.Session.GetString("UserId"); //Get User Id from session  

            usr_data = cdb.GetUserData(user_id);

            return new JsonResult(new { name = usr_data ["Name"], surname = usr_data["Surname"], dob = usr_data["DateOfBirth"], gender = usr_data["Gender"], city = usr_data["City"] });
                   
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveUserData([FromBody] PersonalUserData user_data)
        {
            if (ModelState.IsValid)
            {
                //Get User Id from database for saving personal data of user
                string user_id = HttpContext.Session.GetString("UserId"); //Get User Id from session  

                cdb.SaveUserData(user_data, user_id);
                return new JsonResult(new { text = "Personal information updated successfully!" });
            }
            else
            {
                string error_messages = string.Join("<br /><br />", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                return new JsonResult(new { text = error_messages });
            }

        }


   
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


