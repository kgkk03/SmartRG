using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Forms;

namespace smartrg.Models
{
    public class logerror
    {
        [PrimaryKey]
        [JsonProperty("logtime")]
        public DateTime Logtime { get; set; }

        [JsonProperty("func")]
        public string Func { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }
    public class Resortdata
    {
        [JsonProperty("resortid")]
        public int Resortid { get; set; } = 0;

        [JsonProperty("resortname")]
        public string Resortname { get; set; } = "My Resort name";

        [JsonProperty("logo")]
        public string Logo { get; set; } = "";

        [JsonProperty("qrcode")]
        public string Qrcode { get; set; } = "";


    }
    public class ResultUpdate
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("servertime")]
        public DateTime Servertime { get; set; }

    }
    public class SelectObj<T>
    {
        public bool Check { get; set; } = false;
        public String Display { get; set; } = "";
        public T Obj { get; set; }
    }
    public class InpuConfig
    {
        public string Feildname { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public int Step { get; set; }
        public int BtnType { get; set; }
        public int KeyboardType { get; set; }
    }
    public class Imagedata
    {
        public string ImageBase64 { get; set; } = "";
        public string Thumbnail { get; set; } = "";
        public string Imagefile { get; set; } = "";
        public ImageSource Image { get; set; } = "ic_addimage";
        public int index { get; set; } = 0;
        public bool ShareFile { get; set; } = false;
        public bool canedit { get; set; } = false;
        public SelectObj<object> Type { get; set; } 
        public string Description { get; set; } = "";
    }
    public class Sync
    {
        [PrimaryKey]
        [JsonProperty("tbname")]
        public string Tbname { get; set; }

        [JsonProperty("lastsync")]
        public DateTime Lastsync { get; set; }
    }
    public class CommandData
    {
        [JsonProperty("command")]
        public string Sql { get; set; }

        [JsonProperty("cmdtype")]
        public int Cmdtype { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }
    }
    public class ShowSelectdate
    {
        public DateTime Selectdate { get; set; }
        public string Showdate { get; set; }
    }
    public class GetSingleData
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public  object Data { get; set; }
    }
    public class GetListData
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public List<object> Data { get; set; }
    }
    public class InsertObj<T>
    {
        public T Obj { get; set; }
    }
    public class BatteryData
    {
        public int Level { get; set; } = 0;
        public string State { get; set; } = "Full";
        public string Source { get; set; } = "Battery";

    }
    public class BillCompany
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public string Key { get; set; } = "1";

        [JsonProperty("billcode")]
        public string Billcode { get; set; } = "Blank";

        [JsonProperty("compname")]
        public string Compname { get; set; } = "";

        [JsonProperty("compaddress")]
        public string Compaddress { get; set; } = "";

        [JsonProperty("taxid")]
        public string Taxid { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "blanklogo.png";

        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = App.Servertime;

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 1;
    }





}
