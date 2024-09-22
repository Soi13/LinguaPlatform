using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using LinguaPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OpenTokSDK;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace LinguaPlatform.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        ConnectDb cdb = new ConnectDb();
        
        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
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


        public IActionResult Index()
        {            
            return View();
        }

        [Authorize]
        public IActionResult LevelKnowledge()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddLevelKnowledge([FromBody] LevelKnowledge levelknowledge)
        {
            if (ModelState.IsValid)
            {
                if (cdb.CheckExsistRow("Name", "LevelKnowledge", levelknowledge.Name))
                {
                    return new JsonResult(new { text = string.Format("Value {0} has been already added!", levelknowledge.Name) });
                }

                cdb.AddLevelKnowledge(levelknowledge);

                return new JsonResult(new { text = "Level of knowledge added successfully!" });
            }
            else
            {
                return new JsonResult(new { text = "Not all data valid!" });
            }
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditLevelKnowledge([FromBody] LevelKnowledge levelknowledge)
        {
            if (ModelState.IsValid)
            {
                if (cdb.CheckExsistRow("Name", "LevelKnowledge", levelknowledge.Name))
                {
                    return new JsonResult(new { text = string.Format("Value {0} has been already added!", levelknowledge.Name) });
                }

                cdb.EditLevelKnowledge(levelknowledge);

                return new JsonResult(new { text = "Level of knowledge edited successfully!" });
            }
            else
            {
                return new JsonResult(new { text = "Not all data valid!" });
            }
        }


        [Authorize]
        public JsonResult GetLevelKnowledge(string sort, string order)
        {
            int cnt_table = 0;
            Dictionary<string, string>[] payments;

            payments = cdb.LoadLevelKnowledges(sort, order);
            cnt_table = cdb.GetCountRows("LevelKnowledge");

            return new JsonResult(new { total = cnt_table, rows = payments });
        }


        [Authorize]
        public IActionResult KindLanguages()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddKindLanguage([FromBody] KindLanguages kindlanguages)
        {
            if (ModelState.IsValid)
            {
                if (cdb.CheckExsistRow("Name", "KindLanguages", kindlanguages.Name))
                {
                    return new JsonResult(new { text = string.Format("Value {0} has been already added!", kindlanguages.Name) });
                }

                cdb.AddKindLanguage(kindlanguages);

                return new JsonResult(new { text = "Language added successfully!" });
            }
            else
            {
                return new JsonResult(new { text = "Not all data valid!" });
            }
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditKindLanguage([FromBody] KindLanguages kindlanguages)
        {
            if (ModelState.IsValid)
            {
                if (cdb.CheckExsistRow("Name", "KindLanguages", kindlanguages.Name))
                {
                    return new JsonResult(new { text = string.Format("Value {0} has been already added!", kindlanguages.Name) });
                }

                cdb.EditKindLanguage(kindlanguages);

                return new JsonResult(new { text = "Language edited successfully!" });
            }
            else
            {
                return new JsonResult(new { text = "Not all data valid!" });
            }
        }


        [Authorize]
        public JsonResult GetKindLanguages(string sort, string order)
        {
            int cnt_table = 0;
            Dictionary<string, string>[] payments;

            payments = cdb.LoadKindLanguages(sort, order);
            cnt_table = cdb.GetCountRows("KindLanguages");

            return new JsonResult(new { total = cnt_table, rows = payments });
        }


        [Authorize]
        public IActionResult Groups()
        {
            return View();
        }


        [Authorize]
        public string LoadLanguages()
        {
            string tp = cdb.GetLanguages();
            return tp;
        }


        [Authorize]
        public string LoadLevels()
        {
            string tp = cdb.GetLevels();
            return tp;
        }


        [Authorize]
        public string LoadTeachers()
        {
            string tp = cdb.GetTeachers();
            return tp;
        }

        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddGroup([FromBody] Groups groups)
        {
            if (ModelState.IsValid)
            {
                //string error_messages = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                //return new JsonResult(new { text = error_messages });   
              
                if (groups.GroupName.Length == 0)
                {
                    return new JsonResult(new { text = "You must input name of group!" });
                }

                if (groups.IdKindLanguages == null)
                {
                    return new JsonResult(new { text = "You must select language!" });
                }

                if (groups.IdLevelKnowledge == null)
                {
                    return new JsonResult(new { text = "You must select level of knowledge!" });
                }

                if (groups.MaxCountStudents > 255 )
                {
                    return new JsonResult(new { text = "Count of students in a group can't be more than 10 persons!" });
                }

                if (cdb.CheckExsistRow("GroupName", "Groups", groups.GroupName))
                {
                    return new JsonResult(new { text = "Group with a such name already exist." });
                }

                
                //Make OpenTok session identificator for connecting students and teacher in concrete group
                int ApiKey = 000000; // YOUR API KEY
                string ApiSecret = "00000000000000000000000000000000";
                var OpenTok = new OpenTok(ApiKey, ApiSecret);

                var session = OpenTok.CreateSession(mediaMode: MediaMode.ROUTED, archiveMode: ArchiveMode.MANUAL);
               
                // Store this sessionId in the database for later use:
                string sessionId = session.Id;
                ////////////////////////////////////////////////////////////////

                string user_id = HttpContext.Session.GetString("UserId"); //Get User Id from session

                cdb.AddGroup(groups, user_id, sessionId);
                return new JsonResult(new { text = "Group added successfully!" });
            }
            else
            {
                return new JsonResult(new { text = "Not all data valid!" });
            }
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditGroup([FromBody] Groups groups)
        {
            if (ModelState.IsValid)
            {
                //string error_messages = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                //return new JsonResult(new { text = error_messages });   

                if (groups.GroupName.Length == 0)
                {
                    return new JsonResult(new { text = "You must input name of group!" });
                }

                if (groups.IdKindLanguages == null)
                {
                    return new JsonResult(new { text = "You must select language!" });
                }

                if (groups.IdLevelKnowledge == null)
                {
                    return new JsonResult(new { text = "You must select level of knowledge!" });
                }

                if (groups.MaxCountStudents > 255)
                {
                    return new JsonResult(new { text = "Count of students in a group can't be more than 10 persons!" });
                }

               
                string user_id = HttpContext.Session.GetString("UserId"); //Get User Id from session

                cdb.EditGroup(groups, user_id);
                return new JsonResult(new { text = "Group edited successfully!" });
            }
            else
            {
                return new JsonResult(new { text = "Not all data valid!" });
            }
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DelGroup([FromBody] DelGroup gr)
        {
            if (ModelState.IsValid)
            {
                //string error_messages = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                //return new JsonResult(new { text = error_messages });   

                if (cdb.DeleteGroup(gr))
                {
                    return new JsonResult(new { text = "Group deleted successfully!" });
                }
                else
                {
                    return new JsonResult(new { text = "Group has a students. You can't delete group with a students!" });
                }
            }
            else
            {
                return new JsonResult(new { text = "Not all data valid!" });
            }
        }


        [Authorize]
        public JsonResult GetGroups(string sort, string order)
        {
            int cnt_table = 0;
            Dictionary<string, string>[] groups;

            groups = cdb.LoadGroups(sort, order);
            cnt_table = cdb.GetCountRows("Groups");

            return new JsonResult(new { total = cnt_table, rows = groups });
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetGroup4Edit([FromBody] GetData4GroupEdit group)
        {
            if (ModelState.IsValid)
            {
                //string error_messages = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                //return new JsonResult(new { text = error_messages });   

                Dictionary<string, string> gr_data = new Dictionary<string, string>();               
                gr_data = cdb.Group4Edit(group.Id);
                return new JsonResult(new { groupName = gr_data["GroupName"], idKindLanguages = gr_data["IdKindLanguages"], idLevelKnowledge = gr_data["IdLevelKnowledge"], maxCountStudents = gr_data["MaxCountStudents"], notes = gr_data["Notes"] });
            }
            else
            {
                return new JsonResult(new { text = "Not all data valid!" });
            }
        }


        [Authorize]
        public JsonResult LoadStudents(string sort, string order)
        {
            int cnt_table = 0;
            Dictionary<string, string>[] groups;

            groups = cdb.GetStudents(sort, order);
            cnt_table = cdb.GetCountRows("AspNetUsers");

            return new JsonResult(new { total = cnt_table, rows = groups });
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddStudent2Group([FromBody] GroupsStudents groupsstudents)
        {
            if (ModelState.IsValid)
            {
                //string error_messages = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                //return new JsonResult(new { text = error_messages });   

                if (groupsstudents.IdStudent == null)
                {
                    return new JsonResult(new { text = "You must select student!" });
                }

                
                if (cdb.CheckExsistRow("IdStudent", "GroupsStudents", groupsstudents.IdStudent))
                {
                    return new JsonResult(new { text = "Student has already group. You can't assign to student more than one group." });
                }

                string user_id = HttpContext.Session.GetString("UserId"); //Get User Id from session

                cdb.AddStud2Group(groupsstudents, user_id);
                return new JsonResult(new { text = "Student added successfully!" });
            }
            else
            {
                return new JsonResult(new { text = "Not all data valid!" });
            }
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DelStudentFromGroup([FromBody] DelStudentFromGroup groupsstudents)
        {
            if (ModelState.IsValid)
            {
                //string error_messages = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                //return new JsonResult(new { text = error_messages });  
                                
                cdb.DeleteStudFromGroup(groupsstudents);
                return new JsonResult(new { text = "Student excluded successfully!" });
            }
            else
            {
                return new JsonResult(new { text = "Not all data valid!" });
            }
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DelTeacherFromGroup([FromBody] DelTeacherFromGroup groupsstudents)
        {
            if (ModelState.IsValid)
            {
                //string error_messages = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                //return new JsonResult(new { text = error_messages });  

                cdb.DeleteTeachFromGroup(groupsstudents);
                return new JsonResult(new { text = "Teacher excluded successfully!" });
            }
            else
            {
                return new JsonResult(new { text = "Not all data valid!" });
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddTeacher2Group([FromBody] GroupsStudents groupsstudents)
        {
            if (ModelState.IsValid)
            {
                //string error_messages = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                //return new JsonResult(new { text = error_messages });   

                if (groupsstudents.IdTeacher == "")
                {
                    return new JsonResult(new { text = "You must select teacher!" });
                }

                
                if (cdb.CheckTeacherInGroup(groupsstudents))
                {
                    string LimnuUsId = cdb.GetTeacherLimnuUserId(groupsstudents);
                    string GrName = cdb.GetGroupName(groupsstudents);

                    //Creating whiteboard and getting URL of board. It will be necessary for getting access to whiteboard of student of group
                    dynamic LimnuWhiteBoard = JObject.Parse(GetResponseText("https://api.limnu.com/v1/boardCreate", "{\"apiKey\":\"0000000000000000000000000\", \"disallowSharing\":\"true\", \"boardName\":\""+ GrName + " - whiteboard\", \"creatorId\":\"" + LimnuUsId + "\", \"isLeading\":\"true\", \"soloDraw\":\"false\"   }"));
                 
                    string LimnuWhiteBoardUrl = LimnuWhiteBoard.boardUrl;
                    string LimnuWhiteBoardId = LimnuWhiteBoard.boardId;

                    cdb.AddWhiteBoard2Group(LimnuWhiteBoardUrl, LimnuWhiteBoardId, groupsstudents.IdGroups);

                    cdb.AddTeachToGroup(groupsstudents);

                    return new JsonResult(new { text = "Teacher added successfully!" });                    
                }
                else
                {
                    return new JsonResult(new { text = "Group doesn't contain students. Before adding teacher you have to add students." });
                }
            }
            else
            {
                return new JsonResult(new { text = "Not all data valid!" });
            }
        }


        [Authorize]
        public JsonResult GetStudentsInGroup(int Id)
        {
            int cnt_table = 0;
            Dictionary<string, string>[] groups;

            groups = cdb.GetStudentsFromGroup(Id);
            cnt_table = cdb.GetCountRows("AspNetUsers");

            return new JsonResult(new { total = cnt_table, rows = groups });
        }


        [Authorize]
        public JsonResult GetTeacherInGroup(int Id)
        {
            int cnt_table = 0;
            Dictionary<string, string>[] groups;

            groups = cdb.GetTeacherFromGroup(Id);
            cnt_table = cdb.GetCountRows("AspNetUsers");

            return new JsonResult(new { total = cnt_table, rows = groups });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
