using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace smartrg.Models
{
    #region Visit
    public class VisitShowpageData
    {
        public VisitDetailData Detail { get; set; } = new VisitDetailData();
        public CustomerData Customer { get; set; } = new CustomerData();
        public VisitData Visitdata { get; set; } = new VisitData();

        //1	เริ่มงาน
        //2	ส่งงานเช็คสต็อก
        //3	ส่งแบบสอบถาม
        //4	ส่งรายการขายเงินสด
        //5	ส่งใบสั่งขาย
        //6	ส่งใบเบิกสินค้า
        //7	ส่งใบคืนสินค้า
        //8	ส่งรายการยอดขาย
        //10 เลิกงาน

        public List<VisitPage> VisitPage { get; set; } = null;
        public ReportVisitsumary Visitsumary { get; set; } = new ReportVisitsumary();
        public List<VisitImage> VisitImage { get; set; } = null;
        public List<Questionnaire> Questionnaire { get; set; } = null;
        public List<Answer> Answer { get; set; } = null;
        public List<VisitStockData> Stock { get; set; } = null;
        public List<ShowVisitBillSale> BillSale { get; set; } = null;
        public CashSaleData CashSale { get; set; } = null;
        public List<CashSalelineData> CashSaleLine { get; set; } = null;
        public List<PickingLineData> Picking { get; set; } = null;
    }
    public class VisitData
    {
        [PrimaryKey]
        [JsonProperty("visitid")]
        public string Key { get; set; } = "";

        [JsonProperty("visittime")]
        public DateTime Visittime { get; set; } = App.Servertime;

        [JsonProperty("visitstart")]
        public DateTime Visitstart { get; set; } = App.Servertime;

        [JsonProperty("visitfinish")]
        public DateTime Visitfinish { get; set; } = App.Servertime;

        [JsonProperty("battery")]
        public int Battery { get; set; } = 0;

        [JsonProperty("visitlat")]
        public double Lat { get; set; } = 0;

        [JsonProperty("visitlon")]
        public double Lng { get; set; } = 0;

        [JsonProperty("imei")]
        public string Imei { get; set; } = "";

        [JsonProperty("empid")]
        public int Empid { get; set; } = 0;

        [JsonProperty("empcode")]
        public string Empcode { get; set; } = "";

        [JsonProperty("empfullname")]
        public string Empfullname { get; set; } = "";

        [JsonProperty("custid")]
        public string Custid { get; set; } = "";

        [JsonProperty("custcode")]
        public string Custcode { get; set; } = "";

        [JsonProperty("custname")]
        public string Custname { get; set; } = "-";

        [JsonProperty("custlat")]
        public double Custlat { get; set; } = 0;

        [JsonProperty("custlon")]
        public double Custlng { get; set; } =0;

        [JsonProperty("custtype")]
        public int Typeid { get; set; } = 0;

        [JsonProperty("custtypename")]
        public string Typename { get; set; } = "";

        [JsonProperty("groupid")]
        public int Groupid { get; set; } = 0;

        [JsonProperty("groupname")]
        public string Groupname { get; set; } = "";

        [JsonProperty("custicon")]
        public string Icon { get; set; } = "";

        [JsonProperty("diffdistance")]
        public double Diffdistance { get; set; } = 0;

        [JsonProperty("errortype")]
        public int Errortype { get; set; } = 0;

        [JsonProperty("errortext")]
        public string Errortext { get; set; } = "";

        [JsonProperty("planid")]
        public string Planid { get; set; } = "";

        [JsonProperty("planstatus")]
        public int Planstatus { get; set; } = 12;

        [JsonProperty("details")]
        public string Details { get; set; } = "";

        [JsonProperty("remark")]
        public string Remark { get; set; } = "";

        [JsonProperty("admincode")]
        public string Admincode { get; set; } = "";

        [JsonProperty("adminname")]
        public string Adminname { get; set; } = "-";

        [JsonProperty("location")]
        public string Location { get; set; } = "-";

        [JsonProperty("modifiedid")]
        public int Modifiedid { get; set; } = 0;

        [JsonProperty("modifieddate")]
        public DateTime Modifieddate { get; set; } = App.Servertime;

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 1;

        [JsonProperty("showtime")]
        public string Showtime { get; set; } = "-";

        

    }
    public class VisitDetailData
    {
        [PrimaryKey]
        //[JsonProperty("visitid")]
        public string Key { get; set; } = "";
        public bool Newvisit { get; set; } = true;
        public bool VisitStockSuccess { get; set; } = false;
        public string Stockicon { get; set; } = "ic_uncheck";
        public string StockHeader { get; set; } = "สรุปการเช็คส็อก";
        public string StockCount { get; set; } = "0/0";
        public bool QuestionSuccess { get; set; } = false;
        public string Questionicon { get; set; } = "ic_uncheck";
        public string QuestionHeader { get; set; } = "สรุปการตอบคำถาม";
        public string QuestionCount { get; set; } = "0/0";
        public bool ImageSuccess { get; set; } = false;
        public string Imageicon { get; set; } = "ic_uncheck";
        public string ImageHeader { get; set; } = "สรุปการถ่ายภาพประกอบงาน";
        public string ImageCount { get; set; } = "0/0";
        public bool BillSaleSuccess { get; set; } = false;
        public string BillSaleicon { get; set; } = "ic_uncheck";
        public string BillSaleHeader { get; set; } = "สรุปการเช็คยอดขาย";
        public string BillSaleCount { get; set; } = "0/0";
        public bool CashSaleSuccess { get; set; } = false;
        public string CashSaleicon { get; set; } = "ic_uncheck";
        public string CashSaleHeader { get; set; } = "สรุปการขายเงินสด";
        public string CashSaleCount { get; set; } = "0/0";
        public bool Canedit { get; set; } = false;
    }

    #endregion

    #region Questionnaire
    public class Questionnaire
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public string Key { get; set; } = "";
        // visitid +_+ Question ID

        [JsonProperty("visitid")]
        public string Visitid { get; set; } = "";

        [JsonProperty("item")]
        public int Piority { get; set; } = 0;

        [JsonProperty("qid")]
        public int QuestionID { get; set; } = 0;

        [JsonProperty("question")]
        public string Question { get; set; } = "";

        [JsonProperty("qtype")]
        public int Qtype { get; set; } = 0;
        // 0 = SingleAnswer 1=MultiAnswer  2=TimeAnswer  3=PeriodAnswer

        [JsonProperty("answer")]
        public int AnswerID { get; set; } = 0;

        [JsonProperty("descript")]
        public string Answer { get; set; } = "";

        [JsonProperty("answerlist")]
        public string AnswerAll { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "ic_uncheck";

        [JsonProperty("check")]
        public bool Check { get; set; } = false;

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;

        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = App.Servertime;
    }
    public class Answer
    {
        [PrimaryKey]
        [JsonProperty("tbkey")]
        public string Key { get; set; } = "";
        //id + item
        [JsonProperty("id")]
        public int ID { get; set; } = 0;

        [JsonProperty("item")]
        public int Item { get; set; } = 0;

        [JsonProperty("descript")]
        public string Description { get; set; }
    }

    #endregion

    #region Cash Sale
    public class CashSaleData
    {
        [PrimaryKey]
        [JsonProperty("visitid")]
        public string Key { get; set; } = "";

        [JsonProperty("saledate")]
        public DateTime Saledate { get; set; } = App.Servertime;

        [JsonProperty("custid")]
        public string Custid { get; set; } = "";

        [JsonProperty("custcode")]
        public string Custcode { get; set; } = "";

        [JsonProperty("custname")]
        public string Custname { get; set; } = "";

        [JsonProperty("custaddress")]
        public string Custaddress { get; set; } = "";

        [JsonProperty("custax")]
        public string Custax { get; set; } = "";

        [JsonProperty("totalline")]
        public int Totalline { get; set; } = 0;

        [JsonProperty("amount")]
        public double Amount { get; set; } = 0.0;

        [JsonProperty("discount")]
        public double Discount { get; set; } = 0.0;

        [JsonProperty("vat")]
        public double Vat { get; set; } = 0.0;

        [JsonProperty("total")]
        public double Total { get; set; } = 0.0;

        [JsonProperty("empid")]
        public int Empid { get; set; } = 0;

        [JsonProperty("salename")]
        public string Salename { get; set; } = "";

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;

        [JsonProperty("modified")]
        public DateTime Modified { get; set; } = App.Servertime;

        [JsonProperty("showtime")]
        public string Showtime { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "ic_shop";

    }
    public class CashSalelineData
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public string Key { get; set; } = "";
        //visitid - item

        [JsonProperty("visitid")]
        public string Visitid { get; set; } = "";

        [JsonProperty("item")]
        public int Item { get; set; } = 0;

        [JsonProperty("productid")]
        public int Productid { get; set; } = -1;

        [JsonProperty("productcode")]
        public string Productcode { get; set; } = "";

        [JsonProperty("productname")]
        public string Productname { get; set; } = "";

        [JsonProperty("qtyperpack")]
        public int Qtyperpack { get; set; } = 1;

        [JsonProperty("qty")]
        public int Qty { get; set; } = 0;

        [JsonProperty("unitid")]
        public int Unitid { get; set; } = 0;

        [JsonProperty("unitname")]
        public string Unitname { get; set; } = "ขวด";

        [JsonProperty("price")]
        public double Price { get; set; } = 0.0;

        [JsonProperty("discount")]
        public double Discount { get; set; } = 0.0;

        [JsonProperty("amount")]
        public double Amount { get; set; } = 0.0;

        [JsonProperty("agentid")]
        public string Agentid { get; set; } = "";

        [JsonProperty("agentname")]
        public string Agentname { get; set; } = "";

        [JsonProperty("agentstock")]
        public int Agentstock { get; set; } = 0;


        [JsonProperty("custid")]
        public string Custid { get; set; } = "";

        [JsonProperty("custname")]
        public string Custname { get; set; } = "";

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;

        [JsonProperty("modified")]
        public DateTime Modified { get; set; } = App.Servertime;

    }
    public class PaymentData
    {
        [PrimaryKey]
        [JsonProperty("paymentid")]
        public string Paymentid { get; set; } = "";
        // Visitid + item

        [JsonProperty("visitid")]
        public string Visitid { get; set; } = "";

        [JsonProperty("item")]
        public int Item { get; set; } = 0;

        [JsonProperty("paytypeid")]
        public int paytypeid { get; set; } = 0;

        [JsonProperty("paytypename")]
        public string Paytypename { get; set; } = "เงินสด";

        [JsonProperty("total")]
        public double Total { get; set; } = 0;

        [JsonProperty("bank")]
        public string Bank { get; set; } = "";

        [JsonProperty("ref1")]
        public string Ref1 { get; set; } = "";

        [JsonProperty("ref2")]
        public string Ref2 { get; set; } = "";

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = -1;
        // -1 = รอบันทึก 0 =บันทึกข้อมูลแล้ว

        [JsonProperty("modified")]
        public DateTime Modified { get; set; } = App.Servertime;
    }
    public class PaymentBank
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public string Key { get; set; }

        [JsonProperty("bankname")]
        public string Bankname { get; set; }

        [JsonProperty("bankcode")]
        public string Bankcode { get; set; }

        [JsonProperty("swiftcode")]
        public string Swiftcode { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }

    }
    public class PickingData
    {
        [PrimaryKey]
        [JsonProperty("pickingid")]
        public string Key { get; set; } = "";
        // visitid + PickingType(0=คืน 1=ยืม/เบิก 2=ขาย)

        [JsonProperty("visitid")]
        public string Visitid { get; set; } = "";

        [JsonProperty("typename")]
        public string Pickingtype { get; set; } = "";
        // PickingType(เบิก, ขาย, คืน)

        [JsonProperty("typeid")]
        public int Typeid { get; set; } = 0;
        // PickingType(-1=คืน/ขาย 1=ยืม/เบิก )

        [JsonProperty("pickingdate")]
        public DateTime Pickingdate { get; set; } = App.Servertime;

        [JsonProperty("agentid")]
        public string Agentid { get; set; } = "";

        [JsonProperty("agenticon")]
        public string Icon { get; set; } = "";

        [JsonProperty("agentcode")]
        public string Agentcode { get; set; } = "";

        [JsonProperty("agentname")]
        public string Agentname { get; set; } = "";

        [JsonProperty("totalline")]
        public int Totalline { get; set; } = 0;

        [JsonProperty("totalunit")]
        public int Totalunit { get; set; } = 0;

        [JsonProperty("empid")]
        public int Empid { get; set; } = 0;

        [JsonProperty("empname")]
        public string Empfullname { get; set; } = "";

        [JsonProperty("remark")]
        public string Remark { get; set; } = "";

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = -1;
        //-1 กำลังสร้าง 0 = บันทึกแล้ว -3 ยกเลิก 
        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = App.Servertime;

        [JsonProperty("showtime")]
        public string Showtime { get; set; } = "";
    }
    public class PickingLineData
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public string Key { get; set; } = "";
        // visitid + PickingType(0=คืน 1=ยืม/เบิก 2=ขาย) - item

        [JsonProperty("pickingid")]
        public string Pickingid { get; set; } = "";
        // visitid + PickingType(0=คืน 1=ยืม/เบิก 2=ขาย)

        [JsonProperty("typename")]
        public string Pickingtype { get; set; } = "";
        // PickingType(เบิก, ขาย, คืน)

        [JsonProperty("typeid")]
        public int Typeid { get; set; } = 0;
        // PickingType(-1=คืน/ขาย 1=ยืม/เบิก )

        [JsonProperty("item")]
        public int Item { get; set; } = 0;

        [JsonProperty("pickingdate")]
        public DateTime Pickingdate { get; set; } = App.Servertime;

        [JsonProperty("productid")]
        public int Productid { get; set; } = 0;

        [JsonProperty("productcode")]
        public string Productcode { get; set; } = "";

        [JsonProperty("productname")]
        public string Productname { get; set; } = "";

        [JsonProperty("unitid")]
        public int Unitid { get; set; } = 0;

        [JsonProperty("unitname")]
        public string Unitname { get; set; } = "";

        [JsonProperty("sizename")]
        public string Sizename { get; set; } = "";

        [JsonProperty("qtyperpack")]
        public int Qtyperpack { get; set; } = 1;

        [JsonProperty("price")]
        public double Price { get; set; } = 0;

        [JsonProperty("amount")]
        public double Amount { get; set; } = 0;

        [JsonProperty("stock")]
        public int Stock { get; set; } = 0;

        [JsonProperty("qty")]
        public int Qty { get; set; } = 0;
        // จำนวนที่เบิกหรือคืน 

        [JsonProperty("total")]
        public int Total { get; set; } = 0;
        // จำนวนนับสต็อก (Pickingtype * Qty)

        [JsonProperty("balance")]
        public int Balance { get; set; } = 0;

        [JsonProperty("agentid")]
        public string Agentid { get; set; } = "";

        [JsonProperty("agentname")]
        public string Agentname { get; set; } = "";

        [JsonProperty("custid")]
        public string Custid { get; set; } = "";

        [JsonProperty("custname")]
        public string Custname { get; set; } = "";

        [JsonProperty("remark")]
        public string Remark { get; set; } = "";

        [JsonProperty("refid")]
        public string Refid { get; set; } = "";

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = -1;
        //-1 = ยังไม่บันทึก 0 = บันทึกแล้ว (stock ล่าสุด) 1=Logdata 2 Clear ค่าใช้จ่ายแล้ว

        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = App.Servertime;
    }

    #endregion

    #region Visit Stock
    public class VisitStockData
    {
        [PrimaryKey]
        [JsonProperty("stockkey")]
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
        public string Unitname { get; set; } = "";

        [JsonProperty("price")]
        public double Price { get; set; } = 0.0;

        [JsonProperty("lastprice")]
        public double LastPrice { get; set; } = 0.0;

        [JsonProperty("issale")]
        public bool Sale { get; set; } = true;

        [JsonProperty("isstock")]
        public bool Stock { get; set; } = true;

        [JsonProperty("qty")]
        public int Qty { get; set; } = 0;

        [JsonProperty("facing")]
        public int Facing { get; set; } = 0;

        [JsonProperty("tier")]
        public int Tier { get; set; } = 0;

        [JsonProperty("total")]
        public int Total { get; set; } = 0;

        [JsonProperty("amount")]
        public double Amount { get; set; } = 0;

        [JsonProperty("barcode")]
        public string Barcode { get; set; } = "";

        [JsonProperty("item")]
        public int Piority { get; set; } = 0;

        [JsonProperty("custid")]
        public string Custid { get; set; } = "";

        [JsonProperty("custtype")]
        public int Custtype { get; set; } = 0;

        [JsonProperty("groupid")]
        public int Grouptype { get; set; } = 0;

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 1;

        [JsonProperty("ischeck")]
        public bool Check { get; set; } = false;

        [JsonProperty("icon")]
        public string Icon { get; set; } = "ic_uncheck";

        [JsonProperty("display")]
        public string Display { get; set; } = "uncheck";

        [JsonProperty("canedit")]
        public bool CanEdit { get; set; } = true;

    }

    public class VisitStockShop
    {
        public ImageSource Icon { get; set; } = "ic_shop";
        public bool Stock { get; set; } = false;
        public bool Lost { get; set; } = false;
        public bool Unsale { get; set; } = false;
        public string Productname { get; set; } = "เพิ่มรูปใหม่";
        public double Price { get; set; } = 0.0;
        public bool Canedit { get; set; } = false;
        public Models.VisitStockData Data { get; set; } = new Models.VisitStockData();

    }

    #endregion

    #region Image
    public class VisitImage
    {
        [PrimaryKey]
        [JsonProperty("tbkey")]
        public string Key { get; set; } = "";
        //visitid-item

        [JsonProperty("visitid")]
        public string Visitid { get; set; } = "";

        [JsonProperty("item")]
        public int Item { get; set; } = -1;

        [JsonProperty("typeid")]
        public int Typeid { get; set; } = -1;

        [JsonProperty("typename")]
        public string Typename { get; set; } = "เพิ่มรูปใหม่";

        [JsonProperty("pic")]
        public string ImgBase64 { get; set; } = "";

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; } = "";

        [JsonProperty("lat")]
        public double Lat { get; set; } = 0;

        [JsonProperty("lng")]
        public double Lng { get; set; } = 0;

        [JsonProperty("custname")]
        public string Custname { get; set; } = "";

        [JsonProperty("empfullname")]
        public string Empfullname { get; set; } = "";

        [JsonProperty("location")]
        public string Location { get; set; } = "";

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;

        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = App.Servertime;

    }
    public class VisitImageType
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public int Key { get; set; } = 0;

        [JsonProperty("typename")]
        public string Typename { get; set; } = "";

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 1;

        [JsonProperty("modifieddate")]
        public DateTime Modified { get; set; } = App.Servertime;
    }

    #endregion

    #region Sale Order
    public class SaleorderData
    {
        [PrimaryKey]
        [JsonProperty("soid")]
        public string Key { get; set; } = "";

        [JsonProperty("custicon")]
        public string Icon { get; set; } = "";

        [JsonProperty("custid")]
        public string Custid { get; set; } = "";

        [JsonProperty("custcode")]
        public string CustCode { get; set; } = "";

        [JsonProperty("custname")]
        public string Custname { get; set; } = "";

        [JsonProperty("custaddress")]
        public string CustAddress { get; set; } = "";

        [JsonProperty("totalline")]
        public int Totalline { get; set; } = 0;

        [JsonProperty("amount")]
        public double Amount { get; set; } = 0;

        [JsonProperty("discount")]
        public double Discount { get; set; } = 0;

        [JsonProperty("vat")]
        public double Vat { get; set; } = 0;

        [JsonProperty("total")]
        public double Total { get; set; } = 0;

        [JsonProperty("requestdate")]
        public DateTime Requestdate { get; set; } = App.Servertime.AddDays(2);

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 0;

        [JsonProperty("modified")]
        public DateTime Modified { get; set; } = App.Servertime;

        [JsonProperty("showtime")]
        public string Showtime { get; set; } = "";
    }
    public class SOlineData
    {
        [PrimaryKey]
        [JsonProperty("salelineid")]
        public string Key { get; set; } = "";
        // Soid + item
        [JsonProperty("soid")]
        public string Soid { get; set; } = "";

        [JsonProperty("item")]
        public int Item { get; set; } = 0;

        [JsonProperty("productid")]
        public int Productid { get; set; } = 0;

        [JsonProperty("productcode")]
        public string Productcode { get; set; } = "";

        [JsonProperty("productname")]
        public string Productname { get; set; } = "";

        [JsonProperty("qtyperpack")]
        public int Qtyperpack { get; set; } = 1;

        [JsonProperty("qty")]
        public int Qty { get; set; } = 0;

        [JsonProperty("unitid")]
        public int UnitID { get; set; } = 0;

        [JsonProperty("unitname")]
        public string UnitName { get; set; } = "ขวด";

        [JsonProperty("price")]
        public double Price { get; set; } = 0;

        [JsonProperty("discount")]
        public double Discount { get; set; } = 0;

        [JsonProperty("amount")]
        public double Amount { get; set; } = 0;

        [JsonProperty("custid")]
        public string Custid { get; set; } = "";

        [JsonProperty("transtatus")]
        public int Transtatus { get; set; } = 1;

        [JsonProperty("modified")]
        public DateTime Modified { get; set; } = App.Servertime;
    }

    #endregion












    public class VisitQuestion
    {
        [JsonProperty("id")]
        public int ID { get; set; } = 0;

        [JsonProperty("item")]
        public int Piority { get; set; } = 0;

        [JsonProperty("question")]
        public string Question { get; set; } = "";

        [JsonProperty("qtype")]
        public int Qtype { get; set; } = 0;
        // 0=single select 1=multi select 2=number(single select number)

        [JsonProperty("listanswer")]
        public List<VisitAnswer> Listanswer { get; set; } = new List<VisitAnswer>();

        [JsonProperty("answerid")]
        public int AnswerID { get; set; } = -1;

        [JsonProperty("anstext")]
        public string AnsText { get; set; } = "";

        [JsonProperty("ansshow")]
        public string Display { get; set; } = "";

        [JsonProperty("tabcolor")]
        public Color Cor { get; set; } = Color.Wheat;

        [JsonProperty("icon")]
        public ImageSource Icon { get; set; } = "ic_uncheck"; 

    }
    public class VisitAnswer
    {
        [JsonProperty("id")]
        public int ID { get; set; } = 0;

        [JsonProperty("check")]
        public bool Check { get; set; } = false;

        [JsonProperty("item")]
        public int Item { get; set; } = 0;

        [JsonProperty("ansnumber")]
        public double AnsNumber { get; set; } = 0;

        [JsonProperty("anstext")]
        public string AnsText { get; set; } = "";
    }
    public class VisitBillSale
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

        [JsonProperty("check")]
        public bool Check { get; set; } = false;

        [JsonProperty("sale")]
        public bool Sale { get; set; } = true;

        [JsonProperty("display")]
        public string Display { get; set; } = "uncheck";

        [JsonProperty("item")]
        public int Piority { get; set; } = 0;
    }
    public class Product
    {
        [PrimaryKey]
        [JsonProperty("productid")]
        public string Productid { get; set; }

        [JsonProperty("productcode")]
        public string Productcode { get; set; }

        [JsonProperty("productname")]
        public string Productname { get; set; }

        [JsonProperty("brandid")]
        public string Brandid { get; set; }

        [JsonProperty("brandname")]
        public string Brandname { get; set; }

        [JsonProperty("modelid")]
        public string Modelid { get; set; }

        [JsonProperty("modelname")]
        public string Modelname { get; set; }

        [JsonProperty("unitid")]
        public int Unitid { get; set; }

        [JsonProperty("unitname")]
        public string Unitname { get; set; }

        [JsonProperty("sizename")]
        public string Sizename { get; set; }

        [JsonProperty("qtyperpack")]
        public int Qtyperpack { get; set; }

        [JsonProperty("productvolume")]
        public string Productvolume { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("barcodepack")]
        public string Barcodepack { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("merprice")]
        public double Merprice { get; set; } = 0;

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("iscompet")]
        public bool Iscompet { get; set; }

        [JsonProperty("issale")]
        public bool Issale { get; set; }

        [JsonProperty("isstock")]
        public bool Isstock { get; set; }

        [JsonProperty("istester")]
        public int Istester { get; set; }

        [JsonProperty("pict")]
        public string pict { get; set; }

        [JsonProperty("transtatus")]
        public string Transtatus { get; set; }

        [JsonProperty("modifieddate")]
        public DateTime Modifieddate { get; set; }
    }

   



   




}
