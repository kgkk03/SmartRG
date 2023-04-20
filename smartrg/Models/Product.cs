using Newtonsoft.Json;
using SQLite;
using System.Collections.Generic;

namespace smartrg.Models
{
    public class ProductData
    {
        [PrimaryKey]
        [JsonProperty("productid")]
        public int Productid { get; set; }

        [JsonProperty("productcode")]
        public string Productcode { get; set; }
       
        [JsonProperty("productname")]
        public string Productname { get; set; }

        [JsonProperty("unitname")]
        public string Unitname { get; set; } = "";

        [JsonProperty("sizename")]
        public string Size { get; set; } = "";

        [JsonProperty("price")]
        public double Price { get; set; } = 0.0;

        [JsonProperty("lastprice")]
        public double Lastprice { get; set; } = 0.0;

        [JsonProperty("brandid")]
        public int Brandid { get; set; } = 0;

        [JsonProperty("brandname")]
        public string Brandname { get; set; } = "";

        [JsonProperty("company")]
        public string Company { get; set; } = "";

        [JsonProperty("companyname")]
        public string Companyname { get; set; } = "";

        [JsonProperty("iscompet")]
        public bool Iscompet { get; set; } = false;

        [JsonProperty("remark")]
        public string Remark { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "ic_shop";

    }
    public class ProductType
    {

        [JsonProperty("id")]
        public int ID { get; set; } = 0;

        [JsonProperty("producttype")]
        public string Typename { get; set; } = "beer";

        [JsonProperty("typeimage")]
        public string Image { get; set; } = "ic_beer";

    }

    public class ProductInCustgroup
    {
        [PrimaryKey]
        [JsonProperty("key")]
        public string Key { get; set; }
        //groupid-productid

        [JsonProperty("prodid")]
        public int ProductID { get; set; }

        [JsonProperty("grouppriceid")]
        public int PriceID { get; set; }

        [JsonProperty("productcustcode")]
        public string Productcustcode { get; set; }

        [JsonProperty("productname")]
        public string Productname { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; } = 0.0;

        [JsonProperty("producttype")]
        public string ProductType { get; set; } = "";

        [JsonProperty("typeimage")]
        public string Typeimage { get; set; } = "logo";
    }
    public class ProductInStock
    {
        public int Productid { get; set; }
        public int Balance { get; set; } = 0;
        public int LastStock { get; set; } = 0;
        public int Sale { get; set; } = 0;
        public int Totalagent { get; set; } = 0;
        public int Maxstock { get; set; } = 0;
        public List<PickingLineData> Stockdata { get; set; } = new List<PickingLineData>();
        public List<CashSalelineData> Saledata { get; set; } = new List<CashSalelineData>();
        public ProductData Product { get; set; } = new ProductData();

    }

}
