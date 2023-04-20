using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace smartrg.Models
{
    public class PlanCalendar
    {
        public int ActiveId { get; set; }
        public List<CalendarData> Data { get; set; }
    }
    public class CalendarData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("calendardate")]
        public DateTime Calendardate { get; set; }

        [JsonProperty("calendartext")]
        public string Calendartext { get; set; }

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; }

        [JsonProperty("fontcolor")]
        public string Fontcolor { get; set; }

        [JsonProperty("backgrouncolor")]
        public string Backgrouncolor { get; set; }
    }
    public class PlanData
    {
        [PrimaryKey]
        [JsonProperty("planid")]
        public string Key { get; set; } = "";
        // yyMMdd-custid-empid หรือ yyMMddHH-empid-agencyid

        [JsonProperty("plancode")]
        public string Plancode { get; set; } = "";
        // yyMMdd ใช้ในการเรียกดูแผนงานในแต่ละวัน

        [JsonProperty("empid")]
        public int Empid { get; set; } = 0;

        [JsonProperty("startplan")]
        public DateTime Startplan { get; set; } = App.Servertime;

        [JsonProperty("endplan")]
        public DateTime Endplan { get; set; } = App.Servertime;

        [JsonProperty("custid")]
        public string Custid { get; set; } = "";

        [JsonProperty("custcode")]
        public string Custcode { get; set; } = "";

        [JsonProperty("custname")]
        public string Custname { get; set; } = "";

        [JsonProperty("custlat")]
        public double Custlat { get; set; } = 0;

        [JsonProperty("custlon")]
        public double Custlon { get; set; } = 0;

        [JsonProperty("custaddress")]
        public string Custaddress { get; set; } = "";

        [JsonProperty("admincode")]
        public string Admincode { get; set; } = "";

        [JsonProperty("adminname")]
        public string Adminname { get; set; } = "";

        [JsonProperty("visitid")]
        public string Visitid { get; set; } = "";

        [JsonProperty("visitdate")]
        public DateTime Visitdate { get; set; } = App.Servertime;

        [JsonProperty("location")]
        public string Location { get; set; } = "";

        [JsonProperty("remark")]
        public string Remark { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "";

        [JsonProperty("color")]
        public string Color { get; set; } = "";

        [JsonProperty("planstatus")]
        public int Planstatus { get; set; } = 0;

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;

        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = App.Servertime;


    }

}
