using Newtonsoft.Json;
using SQLite;
using System;
using Xamarin.Forms;

namespace smartrg.Models
{
    public class VisitPage
    {
        [PrimaryKey]
        [JsonProperty("tbkey")]
        public string Key { get; set; } = "";

        [JsonProperty("custtype")]
        public int Custtype { get; set; } = 0;

        [JsonProperty("id")]
        public int Pageid { get; set; } = 0;

        [JsonProperty("item")]
        public int Piority { get; set; } = 0;

        [JsonProperty("fieldname")]
        public string Fieldname { get; set; } = "";

        [JsonProperty("title")]
        public string Title { get; set; } = "";

        [JsonProperty("pathname")]
        public string Pathname { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "";

        [JsonProperty("reqpage")]
        public bool Reqpage { get; set; } = false;
        // บังคับว่าต้องคีย์ข้อมูลให้หมด

        [JsonProperty("reqsend")]
        public bool Reqsend { get; set; } = false;
        // บังคับว่าถ้าไม่ส่งข้อมูล ไม่ให้ save

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;

        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = new DateTime();
    }
    public class MenuList
    {
        [PrimaryKey]
        [JsonProperty("tbkey")]
        public string Key { get; set; } = "";

        [JsonProperty("teamid")]
        public int Teamid { get; set; } = 0;

        [JsonProperty("roleid")]
        public int roleid { get; set; } = 0;

        [JsonProperty("id")]
        public int Id { get; set; } = 0;

        [JsonProperty("title")]
        public string Title { get; set; } = "ข้อมูลผู้ใช้งาน";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "mnu_profile";

        [JsonProperty("fieldname")]
        public string Fieldname { get; set; } = "Profile";

        [JsonProperty("pathname")]
        public string Pathname { get; set; } = "Profile.ProfilePage";

        [JsonProperty("item")]
        public int Piority { get; set; } = 0;

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;

        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = new DateTime();
    }
    public class Admindata
    {

        [JsonProperty("admincode")]
        public string AdminCode { get; set; } = "000000";

        [JsonProperty("adminname")]
        public string AdminName { get; set; } = "ไม่ระบุ";

        [JsonProperty("adminename")]
        public string AdminEname { get; set; } = "Unknown";
    }
    public class CustomerPage
    {
        [PrimaryKey]
        [JsonProperty("tbkey")]
        public string Key { get; set; } = "";

        [JsonProperty("custtype")]
        public int Custtype { get; set; } = 0;

        [JsonProperty("id")]
        public int Pageid { get; set; } = 0;

        [JsonProperty("item")]
        public int Piority { get; set; } = 0;

        [JsonProperty("fieldname")]
        public string Fieldname { get; set; } = "";

        [JsonProperty("title")]
        public string Title { get; set; } = "";

        [JsonProperty("pathname")]
        public string Pathname { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "";

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;

        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = new DateTime();
    }

}
