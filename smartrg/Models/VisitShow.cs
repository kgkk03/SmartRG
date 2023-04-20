using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace smartrg.Models
{
   
    public class ShowVisitBillSale
    {
        [PrimaryKey]
        [JsonProperty("billsalekey")]
        public string Key { get; set; } = "";
        //visitid-productid

        [JsonProperty("visitid")]
        public string Visitid { get; set; } = "";

        [JsonProperty("productid")]
        public string Productid { get; set; } = "";

        [JsonProperty("productcode")]
        public string Productcode { get; set; } = "";

        [JsonProperty("productname")]
        public string Productname { get; set; } = "";

        [JsonProperty("unitname")]
        public string Unitname { get; set; } = "ขวด";

        [JsonProperty("unitprice")]
        public double Price { get; set; } = 0.0;

        [JsonProperty("Total")]
        public int Total { get; set; } = 0;

        [JsonProperty("amount")]
        public double Amount { get; set; } = 0.0;

        [JsonProperty("tabcolor")]
        public Color Cor { get; set; } = Color.LightGreen;

        [JsonProperty("icon")]
        public string Icon { get; set; } = "ic_check";

        [JsonProperty("check")]
        public bool Check { get; set; } = false;

        [JsonProperty("sale")]
        public bool Sale { get; set; } = true;

        [JsonProperty("display")]
        public string Display { get; set; } = "uncheck";

        [JsonProperty("item")]
        public int Piority { get; set; } = 0;
    }

    public class ShowVisitImage
    {
        public int Item { get; set; } = -1;
        public ImageSource Image { get; set; } = "ic_addimage";
        public ImageSource FullImage { get; set; } = "ic_addimage";
        public string Typename { get; set; } = "เพิ่มรูปใหม่";
        public string Display { get; set; } = "เพิ่มรูปใหม่";
        public string Customer { get; set; } = "";
        public string Employee { get; set; } = "";
        public string Location { get; set; } = "";
        public Models.VisitImage Data { get; set; } = new Models.VisitImage();

    }
}
