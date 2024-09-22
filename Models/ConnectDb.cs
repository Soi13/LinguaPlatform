using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace LinguaPlatform.Models
{
    public class ConnectDb
    {
       string connectionString = "Data Source=OLEG_LAPTOP;Initial Catalog=LinguaPlatform;User ID=sa;Password=";
       
        public SqlConnection connection;

        public ConnectDb()
        {
            connection = new SqlConnection(connectionString);
        }



        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        //////////////////////////////////////////////////

        ///////////////////////////Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        /////////////////////////////////////////////


        /// <summary>
        /// Return Count of rows in a specified table
        /// </summary>
        /// <param name="tbl">It's a name of table which you want to get count of rows</param>
        /// <returns>COUNT(*)</returns>
        public int GetCountRows(string tbl)
        {
            string query = string.Format("select count(*) from {0}", tbl);
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            da.Fill(ds, tbl);

            this.CloseConnection();
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }
        ///////////////////////////////////////////////////////////////////


        /// <summary>
        /// Check existence of row in a specified table
        /// </summary>
        /// <param name="tbl">It's a name of table which you want to check existence of row</param>
        /// <param name="row">It's a name of row which you want to check existence of.</param>
        /// <returns>True/False</returns>
        public bool CheckExsistRow(string row, string tbl, string val)
        {
            string query = string.Format("select {0} from {1} where {2} like '{3}'", row, tbl, row, val);
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            da.Fill(ds, tbl);

            this.CloseConnection();

            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        ///////////////////////////////////////////////////////////////////


        ///////////////////////Get LevelKnowledges
        public Dictionary<string, string>[] LoadLevelKnowledges(string sort, string order)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select Id, Name, Blocked from LevelKnowledge order by " + sort + " " + order;
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "LevelKnowledge");

                int cnt_rows = ds.Tables[0].Rows.Count;
                Dictionary<string, string>[] payment = new Dictionary<string, string>[cnt_rows];

                //We need to instantiate the dictionaries inside the array before we can use them.
                for (int k = 0; k <= cnt_rows - 1; k++)
                {
                    payment[k] = new Dictionary<string, string>();
                }

                //Filling array of dictionaries
                for (int j = 0; j <= cnt_rows - 1; j++)
                {
                    for (int i = 0; i <= ds.Tables[0].Columns.Count - 1; i++)
                    {
                        payment[j].Add(ds.Tables[0].Columns[i].ColumnName, ds.Tables[0].Rows[j][i].ToString());
                    }
                }

                this.CloseConnection();
                return payment;
            }
            else
            {
                return null;
            }
        }
        /////////////////////////////////////////////////////////////////////////

        //////////////////////Add level of knowledge
        public void AddLevelKnowledge(LevelKnowledge levelknowledge)
        {
            if (this.OpenConnection() == true)
            {
                string query = "insert into LevelKnowledge (Name) values (@name)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", levelknowledge.Name);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        ///////////////////////////Edit level of knowledge
        public void EditLevelKnowledge(LevelKnowledge levelknowledge)
        {
            if (this.OpenConnection() == true)
            {
                string query = "update Levelknowledge set Name=@name where Id like @id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", levelknowledge.Name);
                cmd.Parameters.AddWithValue("@id", levelknowledge.Id);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        /////////////////////////////////////////////////////////////////////////


        ///////////////////////Get kind of languages
        public Dictionary<string, string>[] LoadKindLanguages(string sort, string order)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select Id, Name, Blocked from KindLanguages order by " + sort + " " + order;
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "KindLanguages");

                int cnt_rows = ds.Tables[0].Rows.Count;
                Dictionary<string, string>[] payment = new Dictionary<string, string>[cnt_rows];

                //We need to instantiate the dictionaries inside the array before we can use them.
                for (int k = 0; k <= cnt_rows - 1; k++)
                {
                    payment[k] = new Dictionary<string, string>();
                }

                //Filling array of dictionaries
                for (int j = 0; j <= cnt_rows - 1; j++)
                {
                    for (int i = 0; i <= ds.Tables[0].Columns.Count - 1; i++)
                    {
                        payment[j].Add(ds.Tables[0].Columns[i].ColumnName, ds.Tables[0].Rows[j][i].ToString());
                    }
                }

                this.CloseConnection();
                return payment;
            }
            else
            {
                return null;
            }
        }
        /////////////////////////////////////////////////////////////////////////

        //////////////////////Add language
        public void AddKindLanguage(KindLanguages kindlanguages)
        {
            if (this.OpenConnection() == true)
            {
                string query = "insert into KindLanguages (Name) values (@name)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", kindlanguages.Name);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        ///////////////////////////Edit level of knowledge
        public void EditKindLanguage(KindLanguages kindlanguages)
        {
            if (this.OpenConnection() == true)
            {
                string query = "update KindLanguages set Name=@name where Id like @id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", kindlanguages.Name);
                cmd.Parameters.AddWithValue("@id", kindlanguages.Id);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        /////////////////////////////////////////////////////////////////////////


        ///////////////////////Get languages
        public string GetLanguages()
        {
            if (this.OpenConnection() == true)
            {
                string query = "select ID as id, NAME as text from KindLanguages for json path";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "KindLanguages");

                this.CloseConnection();
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        ///////////////////////////////////////////////////////////////


        ///////////////////////Get levels of knowledge
        public string GetLevels()
        {
            if (this.OpenConnection() == true)
            {
                string query = "select ID as id, NAME as text from LevelKnowledge for json path";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "LevelKnowledge");

                this.CloseConnection();
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        ///////////////////////////////////////////////////////////////


        ///////////////////////Get teachers
        public string GetTeachers()
        {
            if (this.OpenConnection() == true)
            {
                string query = "select AspNetUsers.Id as id, AspNetUsers.Email as text from AspNetUsers, AspNetUserRoles, AspNetRoles where AspNetUsers.Id=AspNetUserRoles.UserId and AspNetUserRoles.RoleId=AspNetRoles.Id and AspNetRoles.Name='Teacher' for json path";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "AspNetUsers");

                this.CloseConnection();
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        ///////////////////////////////////////////////////////////////


        //////////////////////Add group
        public void AddGroup(Groups groups, string us_id, string id_session)
        {
            if (this.OpenConnection() == true)
            {
                string query = "insert into Groups (GroupName, IdKindLanguages, IdLevelKnowledge, MaxCountStudents, Notes, IdSession, UserId, DateTimeCreate, TeacherIsConnected) values (@gr_name, @lang, @level, @max_cnt, @notes, @idsession, @us_id, GETDATE(), 0)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@gr_name", groups.GroupName);
                cmd.Parameters.AddWithValue("@lang", groups.IdKindLanguages);
                cmd.Parameters.AddWithValue("@level", groups.IdLevelKnowledge);
                cmd.Parameters.AddWithValue("@max_cnt", groups.MaxCountStudents);
                cmd.Parameters.AddWithValue("@notes", groups.Notes);
                cmd.Parameters.AddWithValue("@idsession", id_session);
                cmd.Parameters.AddWithValue("@us_id", us_id);       
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        //////////////////////Edit group
        public void EditGroup(Groups groups, string us_id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "update Groups set GroupName=@gr_name, IdKindLanguages=@lang, IdLevelKnowledge=@level, MaxCountStudents=@max_cnt, Notes=@notes, UserId=@us_id where Id=@Id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@gr_name", groups.GroupName);
                cmd.Parameters.AddWithValue("@lang", groups.IdKindLanguages);
                cmd.Parameters.AddWithValue("@level", groups.IdLevelKnowledge);
                cmd.Parameters.AddWithValue("@max_cnt", groups.MaxCountStudents);
                cmd.Parameters.AddWithValue("@notes", groups.Notes);
                cmd.Parameters.AddWithValue("@us_id", us_id);
                cmd.Parameters.AddWithValue("@Id", groups.Id);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        ///////////////////////Load groups
        public Dictionary<string, string>[] LoadGroups(string sort, string order)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select Groups.Id, Groups.GroupName, KindLanguages.Name as IdKindLanguages, LevelKnowledge.Name as IdLevelKnowledge, Groups.MaxCountStudents, Groups.Notes from Groups, KindLanguages, LevelKnowledge where Groups.IdKindLanguages=KindLanguages.Id and Groups.IdLevelKnowledge=LevelKnowledge.Id order by " + sort + " " + order;
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Groups");

                int cnt_rows = ds.Tables[0].Rows.Count;
                Dictionary<string, string>[] gr = new Dictionary<string, string>[cnt_rows];

                //We need to instantiate the dictionaries inside the array before we can use them.
                for (int k = 0; k <= cnt_rows - 1; k++)
                {
                    gr[k] = new Dictionary<string, string>();
                }

                //Filling array of dictionaries
                for (int j = 0; j <= cnt_rows - 1; j++)
                {
                    for (int i = 0; i <= ds.Tables[0].Columns.Count - 1; i++)
                    {
                        gr[j].Add(ds.Tables[0].Columns[i].ColumnName, ds.Tables[0].Rows[j][i].ToString());
                    }
                }

                this.CloseConnection();
                return gr;
            }
            else
            {
                return null;
            }
        }
        /////////////////////////////////////////////////////////////////////////


        ///////////////////////Get group for edit
        public Dictionary<string, string> Group4Edit(int id_gr)
        {
            if (this.OpenConnection() == true)
            {
                Dictionary<string, string> gr_data = new Dictionary<string, string>();

                string query = "select Id, GroupName, IdKindLanguages, IdLevelKnowledge, MaxCountStudents, Notes from Groups where Id=@id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id_gr);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Groups");
                this.CloseConnection();
                string[] data_gr = new string[ds.Tables[0].Columns.Count];

                for (int i = 0; i <= ds.Tables[0].Columns.Count - 1; i++)
                {
                    gr_data.Add(ds.Tables[0].Columns[i].ColumnName, ds.Tables[0].Rows[0][i].ToString());
                }
                
                return gr_data;
            }
            else
            {
                return null;
            }
        }
        ///////////////////////////////////////////////////////////////



        ///////////////////////Load students for including to group
        public Dictionary<string, string>[] GetStudents(string sort, string order)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select AspNetUsers.Id, AspNetUsers.UserName, AspNetUsers.Email from AspNetUsers, AspNetUserRoles, AspNetRoles where AspNetUsers.Id=AspNetUserRoles.UserId and AspNetUserRoles.RoleId=AspNetRoles.Id and AspNetRoles.Name='User' order by " + sort + " " + order;
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "AspNetUsers");

                int cnt_rows = ds.Tables[0].Rows.Count;
                Dictionary<string, string>[] gr = new Dictionary<string, string>[cnt_rows];

                //We need to instantiate the dictionaries inside the array before we can use them.
                for (int k = 0; k <= cnt_rows - 1; k++)
                {
                    gr[k] = new Dictionary<string, string>();
                }

                //Filling array of dictionaries
                for (int j = 0; j <= cnt_rows - 1; j++)
                {
                    for (int i = 0; i <= ds.Tables[0].Columns.Count - 1; i++)
                    {
                        gr[j].Add(ds.Tables[0].Columns[i].ColumnName, ds.Tables[0].Rows[j][i].ToString());
                    }
                }

                this.CloseConnection();
                return gr;
            }
            else
            {
                return null;
            }
        }
        /////////////////////////////////////////////////////////////////////////


        //////////////////////Add student to group
        public void AddStud2Group(GroupsStudents groupsstudents, string us_id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "insert into GroupsStudents (IdGroups, IdStudent, UserId, DateTimeCreate) values (@id_gr, @id_stud, @us_id, GETDATE())";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id_gr", groupsstudents.IdGroups);
                cmd.Parameters.AddWithValue("@id_stud", groupsstudents.IdStudent);
                cmd.Parameters.AddWithValue("@us_id", us_id);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        //////////////////////Delete student from group
        public void DeleteStudFromGroup(DelStudentFromGroup groupsstudents)
        {
            if (this.OpenConnection() == true)
            {
                string query = "delete from GroupsStudents where IdGroups like @id_gr and IdStudent like @id_stud";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id_gr", groupsstudents.IdGroups);
                cmd.Parameters.AddWithValue("@id_stud", groupsstudents.IdStudent);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        //////////////////////Add teacher to group
        public void AddTeachToGroup(GroupsStudents groupsstudents)
        {
            if (this.OpenConnection() == true)
            {
                string query = "update GroupsStudents set IdTeacher=@id_teach where IdGroups=@id_gr";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id_gr", groupsstudents.IdGroups);
                cmd.Parameters.AddWithValue("@id_teach", groupsstudents.IdTeacher);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        //////////////////////Delete teacher from group
        public void DeleteTeachFromGroup(DelTeacherFromGroup groupsstudents)
        {
            if (this.OpenConnection() == true)
            {
                string query = "update GroupsStudents set IdTeacher='NULL' where IdGroups=@id_gr and IdTeacher=@id_teach";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id_gr", groupsstudents.IdGroups);
                cmd.Parameters.AddWithValue("@id_teach", groupsstudents.IdTeacher);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        //////////////////////Check if group have a teacher
        public bool CheckTeacherInGroup(GroupsStudents groupsstudents)
        {
            if (this.OpenConnection() == true)
            {
                //Check if groups have students
                string query = "select Id from GroupsStudents where IdGroups like @id_gr";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id_gr", groupsstudents.IdGroups);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "GroupsStudents");

                if (ds.Tables[0].Rows.Count > 0)
                {                  
                    this.CloseConnection();
                    return true;
                }
                else
                {
                    this.CloseConnection();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        //////////////////////////////////////////////
        

        ////////////////////Get LimnuUserId of teacher
        public string GetTeacherLimnuUserId (GroupsStudents groupsstudents)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select LimnuUserId from AspNetUsers where Id like @us_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@us_id", groupsstudents.IdTeacher);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "AspNetUsers");

                this.CloseConnection();
                return ds.Tables[0].Rows[0][0].ToString(); 

            }
            else
            {
                return null;
            }
        }
        ////////////////////////////////////////////////////////////


        ////////////////////Get name of group
        public string GetGroupName(GroupsStudents groupsstudents)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select GroupName from Groups where Id like @gr_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@gr_id", groupsstudents.IdGroups);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Groups");

                this.CloseConnection();
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        ////////////////////////////////////////////////////////////


        //////////////////////Add whiteboard to group
        public void AddWhiteBoard2Group(string whiteboard_url, string whiteboard_id, int group_id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "update Groups set WhiteBoardUrl=@wb_url, WhiteBoardId=@wb_id where Id like @gr_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@wb_url", whiteboard_url);
                cmd.Parameters.AddWithValue("@wb_id", whiteboard_id);
                cmd.Parameters.AddWithValue("@gr_id", group_id);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////



        ///////////////////////Get students from  group
        public Dictionary<string, string>[] GetStudentsFromGroup(int id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select GroupsStudents.IdStudent, AspNetUsers.UserName from GroupsStudents, AspNetUsers where AspNetUsers.Id=GroupsStudents.IdStudent and GroupsStudents.IdGroups=@id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "AspNetUsers");

                int cnt_rows = ds.Tables[0].Rows.Count;
                Dictionary<string, string>[] gr = new Dictionary<string, string>[cnt_rows];

                //We need to instantiate the dictionaries inside the array before we can use them.
                for (int k = 0; k <= cnt_rows - 1; k++)
                {
                    gr[k] = new Dictionary<string, string>();
                }

                //Filling array of dictionaries
                for (int j = 0; j <= cnt_rows - 1; j++)
                {
                    for (int i = 0; i <= ds.Tables[0].Columns.Count - 1; i++)
                    {
                        gr[j].Add(ds.Tables[0].Columns[i].ColumnName, ds.Tables[0].Rows[j][i].ToString());
                    }
                }

                this.CloseConnection();
                return gr;
            }
            else
            {
                return null;
            }
        }
        /////////////////////////////////////////////////////////////////////////


        ///////////////////////Get teacher from  group
        public Dictionary<string, string>[] GetTeacherFromGroup(int id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select distinct GroupsStudents.IdTeacher, AspNetUsers.UserName from GroupsStudents, AspNetUsers where AspNetUsers.Id=GroupsStudents.IdTeacher and GroupsStudents.IdGroups=@id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "AspNetUsers");

                int cnt_rows = ds.Tables[0].Rows.Count;
                Dictionary<string, string>[] gr = new Dictionary<string, string>[cnt_rows];

                //We need to instantiate the dictionaries inside the array before we can use them.
                for (int k = 0; k <= cnt_rows - 1; k++)
                {
                    gr[k] = new Dictionary<string, string>();
                }

                //Filling array of dictionaries
                for (int j = 0; j <= cnt_rows - 1; j++)
                {
                    for (int i = 0; i <= ds.Tables[0].Columns.Count - 1; i++)
                    {
                        gr[j].Add(ds.Tables[0].Columns[i].ColumnName, ds.Tables[0].Rows[j][i].ToString());
                    }
                }

                this.CloseConnection();
                return gr;
            }
            else
            {
                return null;
            }
        }
        /////////////////////////////////////////////////////////////////////////


        ///////////////////////Get TeacherId from group
        public string GetTeacherIdFromGroup(string id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select distinct GroupsStudents.IdTeacher from GroupsStudents where GroupsStudents.IdStudent=@us_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@us_id", id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "GroupsStudents");

                this.CloseConnection();
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        ///////////////////////////////////////////////////////////////



        ///////////////////////Get SessionId from group according with a student
        public string GetLessonSessionIdStudent(string us_id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select Groups.IdSession from Groups, GroupsStudents, AspNetUsers where Groups.Id=GroupsStudents.IdGroups and GroupsStudents.IdStudent=AspNetUsers.Id and AspNetUsers.Id like @us_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@us_id", us_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Groups");

                this.CloseConnection();
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        ///////////////////////////////////////////////////////////////


        ///////////////////////Get SessionId from group according with a teacher
        public string GetLessonSessionIdTeacher(GroupLesson gr, string us_id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select distinct Groups.IdSession from Groups, GroupsStudents, AspNetUsers where Groups.Id=GroupsStudents.IdGroups and GroupsStudents.IdTeacher=AspNetUsers.Id and Groups.Id like @gr_id and AspNetUsers.Id like @us_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@us_id", us_id);
                cmd.Parameters.AddWithValue("@gr_id", gr.Id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Groups");

                this.CloseConnection();
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        ///////////////////////////////////////////////////////////////


        ///////////////////////Get WhiteBoardUrl from group according with a student
        public string[] GetWhiteBoardUrlStudent(string us_id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select Groups.WhiteBoardUrl, AspNetUsers.LimnuToken from Groups, GroupsStudents, AspNetUsers where Groups.Id=GroupsStudents.IdGroups and GroupsStudents.IdStudent=AspNetUsers.Id and AspNetUsers.Id like @us_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@us_id", us_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Groups");

                this.CloseConnection();

                string[] data = new string[2];
                data[0] = ds.Tables[0].Rows[0][0].ToString();
                data[1] = ds.Tables[0].Rows[0][1].ToString();
                return data;
            }
            else
            {
                return null;
            }
        }
        ///////////////////////////////////////////////////////////////


        ///////////////////////Get WhiteBoardUrl from group according with a teacher
        public string[] GetWhiteBoardUrlTeacher(string us_id, int gr_id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select distinct Groups.WhiteBoardUrl, AspNetUsers.LimnuToken from Groups, GroupsStudents, AspNetUsers where Groups.Id=GroupsStudents.IdGroups and GroupsStudents.IdTeacher=AspNetUsers.Id and GroupsStudents.IdGroups like @gr_id and AspNetUsers.Id like @us_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@us_id", us_id);
                cmd.Parameters.AddWithValue("@gr_id", gr_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Groups");

                this.CloseConnection();

                string[] data = new string[2];
                data[0] = ds.Tables[0].Rows[0][0].ToString();
                data[1] = ds.Tables[0].Rows[0][1].ToString();
                return data;
            }
            else
            {
                return null;
            }
        }
        ///////////////////////////////////////////////////////////////


        ///////////////////////Load groups for lesson for teacher
        public string GetGroups4Lesson(string us_id)
        {
            if (this.OpenConnection() == true) 
            {
                string query = "select distinct Groups.Id as id, Groups.GroupName as text from Groups, GroupsStudents where Groups.Id=GroupsStudents.IdGroups and GroupsStudents.IdTeacher like @user_id for json path";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@user_id", us_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Groups");

                this.CloseConnection();
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        ///////////////////////////////////////////////////////////////


        //////////////////////Set status to true in group that teacher is connected
        public void SetTeacherConnectedTrue(GroupLesson gr_les)
        {
            if (this.OpenConnection() == true)
            {
                string query = "update Groups set TeacherIsConnected=1 where Id=@Id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", gr_les.Id);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////
       

        //////////////////////Set status to false in group that teacher is disconnected
        public void SetTeacherConnectedFalse(int gr_les_id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "update Groups set TeacherIsConnected=0 where Id=@Id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", gr_les_id);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        ///////////////////////Check if teacher connected to group
        public bool StatusTeacherInGroupTrue(string us_id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select Groups.TeacherIsConnected from Groups, GroupsStudents where GroupsStudents.IdGroups=Groups.Id and GroupsStudents.IdStudent like @user_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@user_id", us_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Groups");

                this.CloseConnection();
                
                if (Convert.ToInt32(ds.Tables[0].Rows[0][0]) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }                
                
            }
            else
            {
                return false;
            }
        }
        ///////////////////////////////////////////////////////////////


        //////////////////////Set status to false in group that teacher is disconnected abnormally. This signal receive from student whhen techer lose connection for example
        public void SetTeacherConnectedFalseAbnormally(string us_id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "update Groups set TeacherIsConnected=0 where Groups.Id=(select IdGroups from GroupsStudents where IdStudent like @user_id)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@user_id", us_id);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        ///////////////////////Check user's status. If account is blocked, user can't login to system.
        public bool CheckUserStatus(string em)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select IsActive from AspNetUsers where UserName like @email";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@email", em);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "AspNetUsers" );

                this.CloseConnection();

                if (Convert.ToInt32(ds.Tables[0].Rows[0][0]) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        ///////////////////////////////////////////////////////////////


        /////////////////////////Get user data
        public Dictionary<string, string> GetUserData(string us_id)
        {
            if (this.OpenConnection() == true)
            {
                Dictionary<string, string> user_data = new Dictionary<string, string>();

                string query = "select * from PersonalUserData where IdUser like @user_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@user_id", us_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "PersonalUserData");


                //If query doesn't return data then return dictionary with blank data 
                if (ds.Tables[0].Rows.Count == 0)
                {
                    for (int i = 0; i <= ds.Tables[0].Columns.Count - 1; i++)
                    {
                        user_data.Add(ds.Tables[0].Columns[i].ColumnName, "");
                    }
                }
                else //If query return data then put data in dictionary
                {
                    for (int i = 0; i <= ds.Tables[0].Columns.Count - 1; i++)
                    {
                        user_data.Add(ds.Tables[0].Columns[i].ColumnName, ds.Tables[0].Rows[0][i].ToString());
                    }
                }

                this.CloseConnection();
                return user_data;
            }
            else
            {
                return null;
            }
        }
        //////////////////////////////////////////////////////////////


        //////////////////////Save user's personal data to database
        public void SaveUserData(PersonalUserData us_data, string us_id)
        {
            if (this.OpenConnection() == true)
            {
                //Check existense of personal user's data. If data are present, then update it using IdUser field, if data are absent, then just insert new data. 
                string query = "select Id from PersonalUserData where IdUser like @user_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@user_id", us_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Groups");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    string query0 = "update PersonalUserData set Name=@name, Surname=@surname, DateOfBirth=@dob, Gender=@gender, City=@city where IdUser like @user_id";
                    SqlCommand cmd0 = new SqlCommand(query0, connection);
                    cmd0.Parameters.AddWithValue("@user_id", us_id);
                    cmd0.Parameters.AddWithValue("@name", us_data.Name);
                    cmd0.Parameters.AddWithValue("@surname", us_data.Surname);
                    cmd0.Parameters.AddWithValue("@dob", us_data.DateOfBirth);
                    cmd0.Parameters.AddWithValue("@gender", us_data.Gender);
                    cmd0.Parameters.AddWithValue("@city", us_data.City);
                    cmd0.ExecuteNonQuery();
                    this.CloseConnection();
                }
                else
                {
                    string query1 = "insert into PersonalUserData (IdUser, Name, Surname, DateOfBirth, Gender, City, DateTimeCreate) values (@user_id, @name, @surname, @dob, @gender, @city, GETDATE())";
                    SqlCommand cmd1 = new SqlCommand(query1, connection);
                    cmd1.Parameters.AddWithValue("@user_id", us_id);
                    cmd1.Parameters.AddWithValue("@name", us_data.Name);
                    cmd1.Parameters.AddWithValue("@surname", us_data.Surname);
                    cmd1.Parameters.AddWithValue("@dob", us_data.DateOfBirth);
                    cmd1.Parameters.AddWithValue("@gender", us_data.Gender);
                    cmd1.Parameters.AddWithValue("@city", us_data.City);
                    cmd1.ExecuteNonQuery();
                    this.CloseConnection();
                }
            }
        }
        //////////////////////////////////////////////


        //////////////////////Delete group
        public bool DeleteGroup(DelGroup gr)
        {
            if (this.OpenConnection() == true)
            {
                //Check existense of students in group. If students are present we can't delete group. 
                string query = "select COUNT(*) as Cnt from GroupsStudents where IdGroups like @gr_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@gr_id", gr.Id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "GroupsStudents");

                if (Convert.ToInt32(ds.Tables[0].Rows[0][0]) > 0)
                {                    
                    this.CloseConnection();
                    return false; 
                }
                else
                {
                    string query1 = "delete from Groups where Id like @gr_id";
                    SqlCommand cmd1 = new SqlCommand(query1, connection);
                    cmd1.Parameters.AddWithValue("@gr_id", gr.Id);
                    cmd1.ExecuteNonQuery();
                    this.CloseConnection();
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        //////////////////////////////////////////////


        //////////////////////Add geo data of user
        public void InsertGeoData(string us_id, string continent, string continent_code, string country, string country_code, string region, string region_name, string city, string district, string zip, float lat, float lon, string timezone, int offset, string currency, string isp, string org, string asname, bool mobile, bool proxy, bool hosting, string ip_query)
        {
            if (this.OpenConnection() == true)
            {
                string query = "insert into UsersJournal (UserId, Continent, ContinentCode, Country, CountryCode, Region, RegionName, City, District, Zip, Lat, Lon, Timezone, Offset, Currency, ISP, Org, Asname, Mobile, Proxy, Hosting, IpQuery, DateTimeCreate) values (@user_id, @continent_, @continent_code_, @country_, @country_code_, @region_, @region_name_, @city_, @district_, @zip_, @lat_, @lon_, @timezone_, @offset_, @currency_, @isp_, @org_, @asname_, @mobile_, @proxy_, @hosting_, @ip_query_, GETDATE())";
        
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@user_id", us_id);
                cmd.Parameters.AddWithValue("@continent_", continent);
                cmd.Parameters.AddWithValue("@continent_code_", continent_code);
                cmd.Parameters.AddWithValue("@country_", country);
                cmd.Parameters.AddWithValue("@country_code_", country_code);
                cmd.Parameters.AddWithValue("@region_", region);
                cmd.Parameters.AddWithValue("@region_name_", region_name);
                cmd.Parameters.AddWithValue("@city_", city);
                cmd.Parameters.AddWithValue("@district_", district);
                cmd.Parameters.AddWithValue("@zip_", zip);
                cmd.Parameters.AddWithValue("@lat_", lat);
                cmd.Parameters.AddWithValue("@lon_", lon);
                cmd.Parameters.AddWithValue("@timezone_", timezone);
                cmd.Parameters.AddWithValue("@offset_", offset);
                cmd.Parameters.AddWithValue("@currency_", currency);
                cmd.Parameters.AddWithValue("@isp_", isp);
                cmd.Parameters.AddWithValue("@org_", org);               
                cmd.Parameters.AddWithValue("@asname_", asname);
                cmd.Parameters.AddWithValue("@mobile_", mobile);
                cmd.Parameters.AddWithValue("@proxy_", proxy);
                cmd.Parameters.AddWithValue("@hosting_", hosting);
                cmd.Parameters.AddWithValue("@ip_query_", ip_query);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        //////////////////////Add errors to journal
        public void InsertError(string us_id, string err)
        {
            this.CloseConnection();
            if (this.OpenConnection() == true)
            {
                string query = "insert into ErrorJournal (UserId, Error, DateTimeCreate) values (@user_id, @error, GETDATE())";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@user_id", us_id);
                cmd.Parameters.AddWithValue("@error", err);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        //////////////////////Add information about begining of lessons of teacher into journal
        public void InsertInfoStartLessonTeacher(GroupLesson gr_id, string us_id)
        {
            this.CloseConnection();
            if (this.OpenConnection() == true)
            {
                string query = "insert into LessonsTeacherJournal (IdTeacher, IdGroup, StartLesson, DateTimeCreate) values (@user_id, @group_id, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@user_id", us_id);
                cmd.Parameters.AddWithValue("@group_id", gr_id.Id);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        //////////////////////Add information about finishing of lessons of teacher into journal
        public void InsertInfoFinishLessonTeacher(int gr_id, string us_id)
        {
            this.CloseConnection();
            if (this.OpenConnection() == true)
            {
                string query = "insert into LessonsTeacherJournal (IdTeacher, IdGroup, StopLesson, DateTimeCreate) values (@user_id, @group_id, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@user_id", us_id);
                cmd.Parameters.AddWithValue("@group_id", gr_id);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        ///////////////////////Get group Id of student's group
        public int GetGroupId4Student(string us_id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select Groups.Id from Groups, GroupsStudents where Groups.Id=GroupsStudents.IdGroups and GroupsStudents.IdStudent like @us_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@us_id", us_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Groups");

                this.CloseConnection();
                
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            else
            {
                return 0;
            }
        }
        ///////////////////////////////////////////////////////////////


        //////////////////////Add information about begining of lessons of student into journal
        public void InsertInfoStartLessonStudent(int gr_id, string us_id)
        {
            this.CloseConnection();
            if (this.OpenConnection() == true)
            {
                string query = "insert into LessonsStudentJournal (IdStudent, IdGroup, StartLesson, DateTimeCreate) values (@user_id, @group_id, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@user_id", us_id);
                cmd.Parameters.AddWithValue("@group_id", gr_id);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        //////////////////////Add information about finishing of lessons of student into journal
        public void InsertInfoFinishLessonStudent(int gr_id, string us_id)
        {
            this.CloseConnection();
            if (this.OpenConnection() == true)
            {
                string query = "insert into LessonsStudentJournal (IdStudent, IdGroup, StopLesson, DateTimeCreate) values (@user_id, @group_id, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@user_id", us_id);
                cmd.Parameters.AddWithValue("@group_id", gr_id);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        ///////////////////////Get all students Id in a group
        public List<string> GetStudentsIdInGroup(int gr_id, string us_id)
        {
            if (this.OpenConnection() == true)
            {
                List<string> lst_id = new List<string>();
                string query = "select IdStudent from GroupsStudents where IdGroups like @gr_id and IdTeacher like @us_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@us_id", us_id);
                cmd.Parameters.AddWithValue("@gr_id", gr_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "GroupsStudents");

                this.CloseConnection();

                for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                {
                    lst_id.Add(ds.Tables[0].Rows[i][0].ToString());
                }

                return lst_id;
            }
            else
            {
                return null;
            }
        }
        ///////////////////////////////////////////////////////////////


        //////////////////////Add data about video archive
        public void AddVideoArchData(Arch archive)
        {
            if (this.OpenConnection() == true)
            {
                DateTimeOffset dateTimeOffset_create = DateTimeOffset.FromUnixTimeMilliseconds(archive.createdAt);
                DateTimeOffset dateTimeOffset_update = DateTimeOffset.FromUnixTimeMilliseconds(archive.updatedAt);
                DateTime dateTime_create = dateTimeOffset_create.DateTime;
                DateTime dateTime_update = dateTimeOffset_update.DateTime;

                string query = "insert into VideoArch (IdArch, Status, Name, Reason, SessionId, ProjectId, CreateAt, Size, Duration, OutputMode, HasAudio, HasVideo, Certificate, Sha256sum, Password, UpdatedAt, Width, Height, Resolution, PartnerId, DatetimeCreate) values (@idarch, @status, @name, @reason, @sessionid, @projectid, @createat, @size, @duration, @outputmode, @hasaudio, @hasvideo, @certificate, @sha256sum, @password, @updatedat, @width, @height, @resolution, @partnerid, SYSDATETIMEOFFSET())";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@idarch", archive.id);
                cmd.Parameters.AddWithValue("@status", archive.status);
                cmd.Parameters.AddWithValue("@name", archive.name);
                cmd.Parameters.AddWithValue("@reason", archive.reason);
                cmd.Parameters.AddWithValue("@sessionid", archive.sessionId);
                cmd.Parameters.AddWithValue("@projectid", archive.projectId);
                cmd.Parameters.AddWithValue("@createat", dateTime_create);
                cmd.Parameters.AddWithValue("@size", archive.size);
                cmd.Parameters.AddWithValue("@duration", archive.duration);
                cmd.Parameters.AddWithValue("@outputmode", archive.outputMode);
                cmd.Parameters.AddWithValue("@hasaudio", archive.hasAudio);
                cmd.Parameters.AddWithValue("@hasvideo", archive.hasVideo);
                cmd.Parameters.AddWithValue("@certificate", archive.certificate);
                cmd.Parameters.AddWithValue("@sha256sum", archive.sha256sum);
                cmd.Parameters.AddWithValue("@password", archive.password);
                cmd.Parameters.AddWithValue("@updatedat", dateTimeOffset_update);
                cmd.Parameters.AddWithValue("@width", archive.width);
                cmd.Parameters.AddWithValue("@height", archive.height);
                cmd.Parameters.AddWithValue("@resolution", archive.resolution);
                cmd.Parameters.AddWithValue("@partnerid", archive.partnerId);
              
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }
        //////////////////////////////////////////////


        ///////////////////////Load video archives
        public Dictionary<string, string>[] LoadArchs(string sort, string order, string us_id)
        {
            if (this.OpenConnection() == true)
            {
                string query = "select distinct VideoArch.Id, VideoArch.IdArch, VideoArch.SessionId, Groups.GroupName, VideoArch.DatetimeCreate from Groups, GroupsStudents, VideoArch where Groups.Id=GroupsStudents.IdGroups and GroupsStudents.IdTeacher like @us_id and Groups.IdSession=VideoArch.SessionId order by " + sort + " " + order;
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@us_id", us_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Groups");

                int cnt_rows = ds.Tables[0].Rows.Count;
                Dictionary<string, string>[] gr = new Dictionary<string, string>[cnt_rows];

                //We need to instantiate the dictionaries inside the array before we can use them.
                for (int k = 0; k <= cnt_rows - 1; k++)
                {
                    gr[k] = new Dictionary<string, string>();
                }

                //Filling array of dictionaries
                for (int j = 0; j <= cnt_rows - 1; j++)
                {
                    for (int i = 0; i <= ds.Tables[0].Columns.Count - 1; i++)
                    {
                        gr[j].Add(ds.Tables[0].Columns[i].ColumnName, ds.Tables[0].Rows[j][i].ToString());
                    }
                }

                this.CloseConnection();
                return gr;
            }
            else
            {
                return null;
            }
        }
        /////////////////////////////////////////////////////////////////////////


        /// Check count of records in video archive table
        public int GetCountRowsVideoArchive(string us_id)
        {
            string query = ("select count(distinct VideoArch.Id) from Groups, GroupsStudents, VideoArch where Groups.Id=GroupsStudents.IdGroups and GroupsStudents.IdTeacher like @us_id and Groups.IdSession=VideoArch.SessionId");
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@us_id", us_id);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            da.Fill(ds, "VideoArch");

            this.CloseConnection();
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }
        ///////////////////////////////////////////////////////////////////


        ///////////////////////Get Name and Surname of user during login
        public string GetUserName(string us_id)
        {
            if (this.OpenConnection() == true)
            {
                string full_name = "";
                string query = "select PersonalUserData.Name, PersonalUserData.Surname from PersonalUserData where PersonalUserData.IdUser like @us_id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@us_id", us_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "PersonalUserData");

                this.CloseConnection();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    full_name = ds.Tables[0].Rows[0][0] + " " + ds.Tables[0].Rows[0][1];
                }
                
                return full_name;
            }
            else
            {
                return null;
            }
        }
        ///////////////////////////////////////////////////////////////

    }
}
