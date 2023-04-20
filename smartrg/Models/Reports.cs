using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace smartrg.Models
{
    public class ReportPlanVisitSumary
    {

        [JsonProperty("workday")]
        public DateTime Workday { get; set; } = App.Servertime;

        [JsonProperty("showday")]
        public string Showday { get; set; } = "01022022";

        [JsonProperty("totalvisit")]
        public int TotalVisit { get; set; } = 0;

        [JsonProperty("onplan")]
        public int Onplan { get; set; } = 0;

        [JsonProperty("miss")]
        public int Miss { get; set; } = 0;

        [JsonProperty("delay")]
        public int Delay { get; set; } = 0;

        [JsonProperty("unplan")]
        public int Unplan { get; set; } = 0;


    }
    public class ReportVisitsumary
    {
        [JsonProperty("workday")]
        public DateTime Workday { get; set; } = App.Servertime;

        [JsonProperty("showday")]
        public string Showday { get; set; } = "1 มีนาคม 2022";

        [JsonProperty("totalvisit")]
        public int TotalVisit { get; set; } = 0;

        [JsonProperty("error")]
        public int Error { get; set; } = 0;

        [JsonProperty("listvisit")]
        public List<ReportVisit> Listvisit { get; set; } = null;
    }
    public class ReportVisit
    {
        [JsonProperty("visitid")]
        public string Visitid { get; set; } = "01022022-001";

        [JsonProperty("workday")]
        public DateTime Workday { get; set; } = App.Servertime;

        [JsonProperty("showtime")]
        public string ShowTime { get; set; } = "00:00";

        [JsonProperty("custname")]
        public string Custname { get; set; } = "";

        [JsonProperty("custicon")]
        public string Custicon { get; set; } = "logo";

        [JsonProperty("custlat")]
        public double Custlat { get; set; } = 13.678626;

        [JsonProperty("custlng")]
        public double Custlng { get; set; } = 100.636245;

        [JsonProperty("visitlat")]
        public double Visitlat { get; set; } = 13.678626;

        [JsonProperty("visitlng")]
        public double Visitlng { get; set; } = 100.636245;

        [JsonProperty("statusicon")]
        public string Statusicon { get; set; } = "ic_onplan";

        [JsonProperty("visittype")]
        public string Visittype { get; set; } = "เข้าพบลูกค้า";

    }
    public class ReportSOsumary
    {
        [JsonProperty("workday")]
        public DateTime Workday { get; set; } = App.Servertime;

        [JsonProperty("showday")]
        public string Showday { get; set; } = "1 มีนาคม 2022";

        [JsonProperty("totalorder")]
        public int TotalSO { get; set; } = 0;

        [JsonProperty("amount")]
        public int Amount { get; set; } = 0;


        [JsonProperty("listso")]
        public List<ReportSO> ListSO { get; set; } = null;
    }
    public class ReportSO
    {
        [JsonProperty("soid")]
        public string Soid { get; set; } = "01022022-001";

        [JsonProperty("workday")]
        public DateTime Workday { get; set; } = App.Servertime;

        [JsonProperty("showtime")]
        public string ShowTime { get; set; } = "00:00";

        [JsonProperty("custname")]
        public string Custname { get; set; } = "";

        [JsonProperty("custicon")]
        public string Custicon { get; set; } = "logo";

        [JsonProperty("custlat")]
        public double Custlat { get; set; } = 13.678626;

        [JsonProperty("custlng")]
        public double Custlng { get; set; } = 100.636245;

        [JsonProperty("totalline")]
        public int TotalLine { get; set; } = 0;

        [JsonProperty("amount")]
        public double Amount { get; set; } = 0;

    }

}
