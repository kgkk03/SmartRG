using Newtonsoft.Json;
using SQLite;
using System;

namespace smartrg.Models
{
    public class CustomerType
    {
        [PrimaryKey]
        [JsonProperty("tbkey")]
        public string Key { get; set; } = "";

        [JsonProperty("id")]
        public int PriceID { get; set; } = 0;

        [JsonProperty("groupname")]
        public string Groupname { get; set; } = "";

        [JsonProperty("typeid")]
        public int TypeID { get; set; } = 0;

        [JsonProperty("typename")]
        public string typename { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "";

        [JsonProperty("priority")]
        public int Priority { get; set; } = 0;

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;

        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = App.Servertime;

    }
    public class CustomerFillter
    {

        [PrimaryKey]
        [JsonProperty("fillterid")]
        public string Key { get; set; } = "";
        // Team - ID

        [JsonProperty("teamid")]
        public int Team { get; set; } = 0;

        [JsonProperty("id")]
        public int ID { get; set; } = 0;

        [JsonProperty("typeid")]
        public int Type { get; set; } = 0;

        [JsonProperty("isstore")]
        public bool Isstore { get; set; } = false;

        [JsonProperty("filltername")]
        public string Display { get; set; } = "ไม่ระบุ";

        [JsonProperty("piority")]
        public int Piority { get; set; } = 0;

        [JsonProperty("icon")]
        public string Icon { get; set; } = "ic_shop";

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;

        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = App.Servertime;
    }
    public class CustomerData
    {
        [PrimaryKey]
        [JsonProperty("custid")]
        public string Key { get; set; } = "";

        [JsonProperty("custcode")]
        public string Custcode { get; set; } = "";

        [JsonProperty("custname")]
        public string Custname { get; set; } = "";

        [JsonProperty("contractname")]
        public string Contractname { get; set; } = "";

        [JsonProperty("contractmobile")]
        public string Contractmobile { get; set; } = "";

        [JsonProperty("typeid")]
        public int Typeid { get; set; } = 0;

        [JsonProperty("typename")]
        public string Typename { get; set; } = "";

        [JsonProperty("typeicon")]
        public string Icon { get; set; } = "ic_shop";

        [JsonProperty("custaddress")]
        public string Custaddress { get; set; } = "";

        [JsonProperty("taxid")]
        public string TaxID { get; set; } = "";

        [JsonProperty("custphone")]
        public string Custphone { get; set; } = "";

        [JsonProperty("empid")]
        public int Empid { get; set; } = 0;

        [JsonProperty("custgroupid")]
        public int Custgroupid { get; set; } = 0;

        [JsonProperty("groupname")]
        public string Custgroupname { get; set; } = "";

        [JsonProperty("remark")]
        public string Remark { get; set; } = "";

        [JsonProperty("lat")]
        public double Lat { get; set; } = 0;

        [JsonProperty("lng")]
        public double Lng { get; set; } = 0;

        [JsonProperty("admincode")]
        public string Admincode { get; set; } = "";

        [JsonProperty("adminname")]
        public string Adminname { get; set; } = "";

        [JsonProperty("createdate")]
        public DateTime Createdate { get; set; } = App.Servertime;

        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = App.Servertime;

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;
    }
    public class CustinlistData
    {
        [PrimaryKey]
        [JsonProperty("custid")]
        public string Custid { get; set; } = "";

        [JsonProperty("custname")]
        public string Custname { get; set; } = "";

        [JsonProperty("custcode")]
        public string Custcode { get; set; } = "";

        [JsonProperty("custaddress")]
        public string Custaddress { get; set; } = "";

        [JsonProperty("custphone")]
        public string Custphone { get; set; } = "";

        [JsonProperty("lat")]
        public double Lat { get; set; } = 0;

        [JsonProperty("lng")]
        public double Lng { get; set; } = 0;

        [JsonProperty("candidate")]
        public bool Iscandidate { get; set; } = false;

        [JsonProperty("groupname")]
        public string Custgroupname { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "ic_shop";

        [JsonProperty("empfullname")]
        public string Empfullname { get; set; } = "";
    }
    public class CustImage
    {
        [PrimaryKey]
        [JsonProperty("custid")]
        public string Key { get; set; } = "";

        [JsonProperty("image")]
        public string Image { get; set; } = "";

        [JsonProperty("thumbnail")]
        public string Icon { get; set; } = "";

    }

}
