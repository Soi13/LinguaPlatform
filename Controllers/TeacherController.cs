using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using LinguaPlatform.Models;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using OpenTokSDK;
using Microsoft.AspNetCore.SignalR;
using Azure.Storage.Blobs;
using System.IO;
using Azure;

namespace LinguaPlatform.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ILogger<TeacherController> _logger;
        ConnectDb cdb = new ConnectDb();
        IHubContext<Signal> hubContext;

        public TeacherController(ILogger<TeacherController> logger, IHubContext<Signal> hubContext)
        {
            _logger = logger;
            this.hubContext = hubContext;
        }

     
        [Authorize]
        public IActionResult Lessons()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Connect2Session([FromBody] GroupLesson gr_les)
        {
            if (ModelState.IsValid)
            {   
                //Get User Id from database for getting SessionId of lesson from group
                string user_id = HttpContext.Session.GetString("UserId"); //Get User Id from session

                //Get Session id of lesson
                string session_id = cdb.GetLessonSessionIdTeacher(gr_les, user_id);

                //Set the following constants with the API key and API secret that you receive when you sign up to use the OpenTok API:
                OpenTok opentok = new OpenTok(00000000, "00000000000000000000000000000000000000");

                //Replace with meaningful metadata for the connection.
                string us_name = User.Identity.Name;
                string connectionMetadata = string.Format("username={0}, status=teacher", us_name);

                // Set live time of token
                double inTwoHours = (DateTime.UtcNow.Add(TimeSpan.FromHours(2)).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                // Generate a token. Use the Role value appropriate for the user.
                string token_ = opentok.GenerateToken(session_id, Role.MODERATOR, expireTime: inTwoHours, connectionMetadata);
                                
                //Safe in session group Id during connecting to lesson. This is necessary for detecting Id group during disconnecting from session
                HttpContext.Session.SetInt32("GroupId", gr_les.Id); //Put GroupId in session

                cdb.SetTeacherConnectedTrue(gr_les);
                cdb.InsertInfoStartLessonTeacher(gr_les, user_id);

                return new JsonResult(new { apiKey = 00000000, sessionId = session_id, token = token_ });
                
            }
            else
            {
                return new JsonResult(new { err = true });
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

            cdb.SetTeacherConnectedFalse(group_id);
            cdb.InsertInfoFinishLessonTeacher(group_id, user_id);

            return new JsonResult(new { result = true });
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult WhiteBoard()
        {
            List<string> lst_id = new List<string>();
            
            //Get User Id from database for getting whiteboardUrl from group
            string user_id = HttpContext.Session.GetString("UserId"); //Get User Id from session

            //Get Group Id from database for sending signal to students
            int group_id = (int)HttpContext.Session.GetInt32("GroupId"); //Get Group Id from session

            //Get whiteboard url and user's token for accessing to whiteboard
            string[] white_board_data = cdb.GetWhiteBoardUrlTeacher(user_id, group_id);
            string full_whiteboard_url = white_board_data[0] + "t=" + white_board_data[1];

            lst_id = cdb.GetStudentsIdInGroup(group_id, user_id);

            string status_whiteboard = HttpContext.Session.GetString("WbStatus"); 

            if (status_whiteboard == "WbOpened")
            {
                HttpContext.Session.Remove("WbStatus");
                hubContext.Clients.Users(lst_id).SendAsync("Receive", "c_wb");
            }
            else
            {
                HttpContext.Session.SetString("WbStatus", "WbOpened");
                hubContext.Clients.Users(lst_id).SendAsync("Receive", "o_wb");
            }

            //hubContext.Clients.Users(lst_id).SendAsync("Receive", "o_wb");
            
            return new JsonResult(new { wbu = full_whiteboard_url });
        }

      

        [Authorize]
        public IActionResult Training()
        {
            return View();
        }

        [Authorize]
        public IActionResult VideoArchive()
        {
            return View();
        }

        [Authorize]
        public string LoadGroups4Lesson()
        {
            //Get User Id from database for getting SessionId of lesson from group
            string user_id = HttpContext.Session.GetString("UserId"); //Get User Id from session

            string tp = cdb.GetGroups4Lesson(user_id);
            return tp;
        }



        [HttpPost]
        public JsonResult MakeArch([FromBody] Arch archive)
        {
            if (ModelState.IsValid)
            {
                if (archive.status == "uploaded")
                {                    
                    cdb.AddVideoArchData(archive);

                    return new JsonResult(new { Result = true });
                }
                else
                {
                    return new JsonResult(new { Result = "Empty" });
                }

            }
            else
            {
                //string error_messages = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                //return new JsonResult(new { text = error_messages });   

                return new JsonResult(new { err = true });
            }
        }

        
        [Authorize]      
        public JsonResult GetArchs(string sort, string order)
        {
            //Get User Id from database for getting SessionId of lesson from group
            string user_id = HttpContext.Session.GetString("UserId"); //Get User Id from session

            int cnt_table = 0;
            Dictionary<string, string>[] archives;

            archives = cdb.LoadArchs(sort, order, user_id);
            cnt_table = cdb.GetCountRowsVideoArchive(user_id);

            return new JsonResult(new { total = cnt_table, rows = archives });
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DownLoadArchive([FromBody] Arch ar)
        {
            // Get a connection string to our Azure Storage account.
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=linguastorage;AccountKey=000000000000000000000000000000000000;EndpointSuffix=core.windows.net";

            try
            {    
                BlobClient bl = new BlobClient(connectionString, "fileslingua/000000000", ar.id+"/archive.mp4"); 
                                
                //bl.DownloadTo(downloadPath);

                return new JsonResult(new { result = "<a href=\"" + bl.Uri.AbsoluteUri +"\">Download archive</a>" });
            }
            catch (RequestFailedException ex)
            {
                return new JsonResult(new { result = ex});
            }


        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SendStatusWB([FromBody] SendStatusWB st)
        {
            //if (ModelState.IsValid)
            //{
                hubContext.Clients.User(st.Status_wb[0]).SendAsync("ResultStatusWB", st.Status_wb[1]);
                return new JsonResult(new { result = true });
            //}
            //else
            //{
            //    return new JsonResult(new { Result = false });
            //}
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SendTeacherStreamId([FromBody] TeacherStream ts)
        {
            if (ModelState.IsValid)
            {
               hubContext.Clients.User(ts.StreamId[0]).SendAsync("GetTeacherId", ts.StreamId[1]);
               return new JsonResult(new { result = true });
            }
            else
            {
                return new JsonResult(new { Result = false });
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
