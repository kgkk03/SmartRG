using Newtonsoft.Json;
using SQLite;
using System.Collections.Generic;
using Xamarin.Forms;

namespace smartrg.Models
{
    public class GetLoginData
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public UserProfile Data { get; set; }

        [JsonProperty("team")]
        public List<UserTeam> Team { get; set; }


    }
    public class UserProfile
    {
        [PrimaryKey]
        [JsonProperty("loginname")]
        public string Key { get; set; } = "";

        [JsonProperty("pwd")]
        public string Pwd { get; set; } = "";

        [JsonProperty("empid")]
        public int Empid { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; } = "";

        [JsonProperty("empname")]
        public string Empname { get; set; } = "";

        [JsonProperty("empsurname")]
        public string Empsurname { get; set; } = "";

        [JsonProperty("empcode")]
        public string Empcode { get; set; } = "";

        [JsonProperty("empmobile")]
        public string Phon { get; set; } = "";

        [JsonProperty("empemail")]
        public string Email { get; set; } = "";

        [JsonProperty("empposition")]
        public string Position { get; set; } = "";

        [JsonProperty("teamid")]
        public int Teamid { get; set; } = 1;

        [JsonProperty("teamname")]
        public string TeamName { get; set; } = "";

        [JsonProperty("roleid")]
        public int Role { get; set; } = 11;

        [JsonProperty("rolename")]
        public string Authen { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "logo";

        [JsonProperty("imei")]
        public string Deviceserial { get; set; } = "";

    }
    public class UserRole
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("rolename")]
        public string RoleName { get; set; }
    }
    public class UserTeam
    {
        [PrimaryKey]
        [JsonProperty("teamid")]
        public int ID { get; set; } = 1;

        [JsonProperty("teamname")]
        public string TeamName { get; set; } = "";

        [JsonProperty("roleid")]
        public int Role { get; set; } = 11;

        [JsonProperty("rolename")]
        public string Authen { get; set; } = "";

        [JsonProperty("icon")]
        public string Image { get; set; } = "logo";

    }
    public class UserData
    {
        [PrimaryKey]
        [JsonProperty("loginname")]
        public string Loginname { get; set; } = "";

        [JsonProperty("pwd")]
        public string Pwd { get; set; } = "";

        [JsonProperty("empid")]
        public int Empid { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; } = "";

        [JsonProperty("empmobile")]
        public string Phon { get; set; } = "";

        [JsonProperty("listteam")]
        public List<UserTeam> ListTeam { get; set; } = null;

    }
    public class Userimage
    {
        [PrimaryKey]
        [JsonProperty("empid")]
        public int Empid { get; set; } = 0;

        [JsonProperty("emppic")]
        public string Image64 { get; set; } = "";

        [JsonProperty("employeepic")]
        public ImageSource Image { get; set; } = "avatar";
    }
    public class Empvisit
    {
        [JsonProperty("empid")]
        public int Empid { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; } = "";

        [JsonProperty("empname")]
        public string Empname { get; set; } = "";

        [JsonProperty("empsurname")]
        public string Empsurname { get; set; } = "";

        [JsonProperty("empcode")]
        public string Empcode { get; set; } = "";

        [JsonProperty("empmobile")]
        public string Phon { get; set; } = "";

        [JsonProperty("empemail")]
        public string Email { get; set; } = "";

        [JsonProperty("empposition")]
        public string Position { get; set; } = "";

        [JsonProperty("icon")]
        public ImageSource Icon { get; set; } = "avatar";

        [JsonProperty("img64")]
        public string Img64 { get; set; } = "";


        [JsonProperty("visit")]
        public VisitData Visit { get; set; } = new VisitData();
    }

    public class Empthumbnail
    {
        [JsonProperty("empid")]
        public int Empid { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; } = "";

    }
}