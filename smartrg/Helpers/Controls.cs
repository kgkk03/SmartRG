using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace smartrg.Helpers
{
    public class Controls
    {
        public static string GetMD5string(string data)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return GetMd5Hash(md5Hash, data);
            }
        }
        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        // Verify a hash against a string.
        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static public string GetMd5Password(String source)
        {
            MD5 md = MD5CryptoServiceProvider.Create();
            byte[] hash;

            //Create a new instance of ASCIIEncoding to 
            //convert the string into an array of Unicode bytes.
            UTF8Encoding enc = new UTF8Encoding();
            //            ASCIIEncoding enc = new ASCIIEncoding();

            //Convert the string into an array of bytes.
            byte[] buffer = enc.GetBytes(source);

            //Create the hash value from the array of bytes.
            hash = md.ComputeHash(buffer);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
        public static DateTime String2Date(string date, string textformat)
        {
            DateTime result = App.Servertime;
            if (date.Equals("")) { return result; }
            try
            {
                switch (textformat)
                {
                    case "yyyy-MM-dd":
                        var result1 = date.Split('-');
                        if (result1.Length > 2)
                        {
                            result = new DateTime(int.Parse(result1[0]), int.Parse(result1[1]), int.Parse(result1[2]));
                        }
                        break;
                    case "yyyy-MM-dd HH:mm:ss":
                        var result2 = date.Split(' ');
                        if (result2.Length > 1)
                        {
                            string resultDate = result2[0];
                            string resultTime = result2[1];
                            var arrdate = resultDate.Split('-');
                            if (arrdate.Length > 2)
                            {
                                result = new DateTime(int.Parse(arrdate[0]), int.Parse(arrdate[1]), int.Parse(arrdate[2]), 0, 0, 0);
                                var arrtime = resultTime.Split(':');
                                if (arrtime.Length > 2)
                                {
                                    result.AddHours(int.Parse(arrtime[0])).AddMinutes(int.Parse(arrtime[1])).AddSeconds(int.Parse(arrtime[2]));
                                }
                            }
                        }
                        break;



                }
            }
            catch { }

            return result;
        }
        public static string GetID(bool onlydate = false)
        {
            DateTime d = App.Servertime;
            System.Globalization.CultureInfo arCI = new System.Globalization.CultureInfo("en-US");
            string hijriDate = d.ToString("yyyyMMddHHmmss", arCI);
            if (onlydate) { hijriDate = d.ToString("yyyyMMdd", arCI); }
            return hijriDate;
        }
        public static string Date2String(DateTime d, string textformat = "yyyy-MM-dd HH:mm:ss")
        {
            System.Globalization.CultureInfo arCI = new System.Globalization.CultureInfo("en-US");
            //System.Globalization.CultureInfo arCI = new System.Globalization.CultureInfo("th-TH");
            string hijriDate = d.ToString(textformat, arCI);
            return hijriDate;
        }
        public static string Date2ThaiString(DateTime d, string textformat = "dd-MMM-yyyy HH:mm:ss")
        {
            string resultday = "";
            string resulthour = "";
            var strarray = textformat.Split(' ');
            if (strarray.Length == 1)
            {
                //มีแต่วันเดือนปี หรือ ชั่วโมงนาที
                var dayarray = strarray[0].Split('-');
                if (dayarray.Length > 1)
                {
                    //มีแต่วันเดือนปี
                    foreach (var st in dayarray)
                    {
                        resultday += ((resultday.Equals("") ? "" : " ") + GetThaiDate(st, d));
                    }
                }
                else
                {
                    //มีแต่ชั่วโมงนาที
                    var hourarray = strarray[0].Split(':');
                    if (dayarray.Length > 1)
                    {
                        //มีแต่วันเดือนปี
                        foreach (var st in hourarray)
                        {
                            resulthour += ((resulthour.Equals("") ? "" : ":") + GetThaiDate(st, d));
                        }
                    }
                }
            }
            else
            {
                //มีทั้งวันเดือนปี และ ชั่วโมงนาที
                var dayarray = strarray[0].Split('-');
                if (dayarray.Length > 1)
                {
                    //มีแต่วันเดือนปี
                    foreach (var st in dayarray)
                    {
                        resultday += ((resultday.Equals("") ? "" : " ") + GetThaiDate(st, d));
                    }
                }
                //มีแต่ชั่วโมงนาที
                var hourarray = strarray[1].Split(':');
                if (dayarray.Length > 1)
                {
                    //มีแต่วันเดือนปี
                    foreach (var st in hourarray)
                    {
                        resulthour += ((resulthour.Equals("") ? "" : ":") + GetThaiDate(st, d));
                    }
                }
            }

            return (resultday + " " + resulthour).Trim();
        }
        public static string GetThaiDate(string format, DateTime value)
        {
            if (format.Equals("d")) { return value.Day.ToString(); }
            else if (format.Equals("dd")) { return value.Day.ToString().PadLeft(2, '0'); }
            else if (format.Equals("M")) { return value.Month.ToString(); }
            else if (format.Equals("MM")) { return value.Month.ToString().PadLeft(2, '0'); }
            else if (format.Equals("MMM")) { return GetThaiMonth(value.Month); }
            else if (format.Equals("MMMM")) { return GetThaiMonth(value.Month, true); }
            else if (format.Equals("yy")) { return (value.Year + 543).ToString().Substring(2, 2); }
            else if (format.Equals("yyyy")) { return (value.Year + 543).ToString(); }
            else if (format.Equals("H")) { return value.Hour.ToString(); }
            else if (format.Equals("HH")) { return value.Hour.ToString().PadLeft(2, '0'); }
            else if (format.Equals("m")) { return value.Minute.ToString(); }
            else if (format.Equals("mm")) { return value.Minute.ToString().PadLeft(2, '0'); }
            else if (format.Equals("s")) { return value.Second.ToString(); }
            else if (format.Equals("ss")) { return value.Second.ToString().PadLeft(2, '0'); }
            else return "";
        }
        public static string GetThaiMonth(int month, bool full = false)
        {

            if (month == 1) { return full ? "มกราคม" : "ม.ค."; }
            else if (month == 2) { return full ? "กุมภาพันธ์" : "ก.พ."; }
            else if (month == 3) { return full ? "มีนาคม" : "มี.ค."; }
            else if (month == 4) { return full ? "เมษายน" : "เม.ย."; }
            else if (month == 5) { return full ? "พฤษภาคม" : "พ.ค."; }
            else if (month == 6) { return full ? "มิถุนายน" : "มิ.ย."; }
            else if (month == 7) { return full ? "กรกฎาคม" : "ก.ค."; }
            else if (month == 8) { return full ? "สิงหาคม" : "ส.ค."; }
            else if (month == 9) { return full ? "กันยายน" : "ก.ย."; }
            else if (month == 10) { return full ? "ตุลาคม" : "ต.ค."; }
            else if (month == 11) { return full ? "พฤศจิกายน" : "พ.ย."; }
            else if (month == 12) { return full ? "ธันวาคม" : "ธ.ค."; }
            else { return month.ToString().PadLeft(2, '0'); }
        }
        public static DateTime GetToday(DateTime Today = new DateTime())
        {
            if (Today == new DateTime()) { Today = App.Servertime; }
            return new DateTime(Today.Year, Today.Month, Today.Day, 0, 0, 0, 0);
        }
        public static string MinutestoString(double MyMinutes)
        {
            string result = "0 Min";
            try
            {
                double dd = 0;
                double HH = Math.Floor(MyMinutes / 60);
                int mm = (int)(MyMinutes % 60);
                if (HH > 24)
                {
                    dd = Math.Floor(HH / 24);
                    HH = (HH % 24);
                }
                if (dd == 0)
                {
                    if (HH < 1)
                    {
                        result = mm.ToString("0") + " Min";
                    }
                    else
                    {
                        result = ((int)(HH)).ToString() + ":" + mm.ToString("00") + " Hrs.";
                    }
                }
                else { result = ((int)(dd)).ToString() + " Day " + ((int)(HH)).ToString("0") + ":" + mm.ToString("00") + " Hrs."; }
                return result;

            }
            catch { }
            return result;
        }
        public static string BathText(double number)
        {
            try
            {
                string bathresult = "";
                string stangresult = "";
                string[] unitname = { "", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน", "ล้าน", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน", "ล้าน", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน", "ล้าน" };
                string[] numbername = { "ศูนย์", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า" };
                var strnumber = number.ToString().Split('.');
                string strbath = strnumber[0];
                string strstang = strnumber.Length > 1 ? strnumber[1] : "0";
                if (double.Parse(strbath) == 0) { bathresult = "ศูนย์บาท"; }
                else
                {
                    for (int i = 0; i < strbath.Length; i++)
                    {
                        int position = strbath.Length - i - 1;
                        int value = int.Parse(strbath[position].ToString());
                        string unit = unitname[i];
                        if (unit.Equals("สิบ") && value == 2) { bathresult = "ยี่สิบ" + bathresult; }
                        else
                        {
                            if (value > 0)
                            {
                                bathresult = numbername[value] + unit + bathresult;
                            }
                            else { if (unit.Equals("ล้าน")) bathresult = unit + bathresult; }
                        }
                    }
                    bathresult += "บาท";
                }
                if (double.Parse(strstang) == 0) { stangresult += "ถ้วน"; }
                else
                {
                    for (int i = 0; i < strstang.Length; i++)
                    {
                        int position = strstang.Length - i - 1;
                        int value = int.Parse(strstang[position].ToString());
                        string unit = unitname[i];
                        if (unit.Equals("สิบ") && value == 2) { stangresult = "ยี่สิบ" + stangresult; }
                        else
                        {
                            if (value > 0) { stangresult = numbername[value] + unit + stangresult; }
                            else { if (unit.Equals("ล้าน")) stangresult = unit + stangresult; }
                        }
                    }
                    stangresult += "สตางค์";
                }
                return bathresult + stangresult;
            }
            catch { return "ตัวเลขสูงเกินกว่าจะคำนวณได้"; }
        }
        public static async Task<string> GetLocationname(double lat, double lng)
        {
            try
            {
                string result = "";
                var placemarks = await Geocoding.GetPlacemarksAsync(lat, lng);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    var daf = placemark.SubThoroughfare;
                    result =
                        ((placemark.SubThoroughfare == null) ? "" : (placemark.SubThoroughfare + " ")) +
                        ((placemark.Thoroughfare == null) ? "" : (placemark.Thoroughfare + " ")) +
                        ((placemark.Locality == null) ? "" : (placemark.Locality + " ")) +
                        ((placemark.SubAdminArea == null) ? "" : (placemark.SubAdminArea + " ")) +
                        ((placemark.AdminArea == null) ? "" : (placemark.AdminArea + " ")) +
                        ((placemark.PostalCode == null) ? "" : (placemark.PostalCode + " "));
                }
                return await Task.FromResult(result);
            }
            catch
            {
                return await Task.FromResult("");
            }
        }
        public static async Task<string> GetAddressName(double lat, double lng)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(lat, lng);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    var geocodeAddress =
                        $"AdminArea:       {placemark.AdminArea}\n" +
                        $"CountryCode:     {placemark.CountryCode}\n" +
                        $"CountryName:     {placemark.CountryName}\n" +
                        $"FeatureName:     {placemark.FeatureName}\n" +
                        $"Locality:        {placemark.Locality}\n" +
                        $"PostalCode:      {placemark.PostalCode}\n" +
                        $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                        $"SubLocality:     {placemark.SubLocality}\n" +
                        $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                        $"Thoroughfare:    {placemark.Thoroughfare}\n";

                    return await Task.FromResult("geocodeAddress");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Handle exception that may have occurred in geocoding
            }
            return await Task.FromResult("un know location");
        }
        public static async void GotoNavigator(Location location)
        {
            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Default };
            try
            {
                await Map.OpenAsync(location, options);
            }
            catch { }
        }
        public static async Task<bool> ShareVisitImage(string Customer, string Visitdate, string Empname, string location, string ImgtypeName, int Imgtype)
        {
            try
            {
                string filename = Imgtype.ToString() + ".png";
                filename = Path.Combine(App.Imagepath, filename);
                filename = DependencyService.Get<Helpers.ICallService>().GetPath(filename);
                Visitdate = Visitdate + " ( " + ImgtypeName + " )";
                string imgfile = DependencyService.Get<Helpers.ICallService>().DrawImage(filename, Imgtype, Customer, Visitdate, Empname, location);
                if (!imgfile.Equals(""))
                {
                    await Share.RequestAsync(new ShareFileRequest
                    {
                        Title = ImgtypeName,
                        File = new ShareFile(imgfile)
                    });
                }
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("Share DrawImage Error", ex.Message);
                return await Task.FromResult(false);
            }

        }

        // คำนวนระยะทาง
        public static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = deg2rad(lat2 - lat1);  // deg2rad below
            var dLon = deg2rad(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in km
            return d * 1000; // Distance in m.
        }
        static double deg2rad(double deg)
        {
            return deg * (Math.PI / 180);
        }
        public static Page GetSubPage(int ID)
        {
            //VisitSumaryPage
            if (ID == 1100) { return new Visit.VisitSumaryPage(); }
            //QuestionPage
            else if (ID == 1101) { return new Visit.QuestionPage(); }
            //CVSStockPage
            else if (ID == 1102) { return new VisitStock.CVSStockPage(); }
            //HyperStockPage
            else if (ID == 1103) { return new VisitStock.HyperStockPage(); }
            //AgentStockPage
            else if (ID == 1104) { return new VisitStock.ShopStockPage(); }
            //ShopStockPage
            else if (ID == 1105) { return new VisitStock.ShopStockPage(); }
            //AgentPickingPage
            else if (ID == 1106) { return new Visit.VisitHeaderPage(); }
            //AgentReturnPage
            else if (ID == 1107) { return new Visit.VisitHeaderPage(); }
            //CashSalePage
            else if (ID == 1108) { return new Visit.VisitHeaderPage(); }
            //SOPage
            else if (ID == 1109) { return new Visit.VisitHeaderPage(); }
            //BillsalePage
            else if (ID == 1110) { return new Visit.BillsalePage(); }
            // ListVisitPage
            else if (ID == 1201) { return new Customer.ListVisitPage(); }
            // ListBilsalePage
            else if (ID == 1202) { return new Customer.ListBilsalePage(); }
            // ListCashsalePage
            else if (ID == 1203) { return new Customer.ListCashsalePage(); }
            // ListPickingPage
            else if (ID == 1204) { return new Customer.ListPickingPage(); }
            // ListSOPage
            else if (ID == 1205) { return new Customer.ListSOPage(); }
            // LastStockPage
            else if (ID == 1206) { return new Customer.LastStockPage(); }
            // ListImagePage
            else if (ID == 1207) { return new Customer.ListImagePage(); }

            else return new Tools.BackPage();
        }
        public static String GetVisitDetails(Models.VisitShowpageData visit)
        {
            string result = "";
            foreach (var dr in visit.VisitPage)
            {
                if (dr.Pageid == 1101 && visit.Detail.QuestionSuccess) { result += (", " + Helpers.Controls.GetDetailsVisit(dr.Pageid)); }
                else if (dr.Pageid == 1102 && visit.Detail.VisitStockSuccess) { result += (", " + Helpers.Controls.GetDetailsVisit(dr.Pageid)); }
                else if (dr.Pageid == 1103 && visit.Detail.VisitStockSuccess) { result += (", " + Helpers.Controls.GetDetailsVisit(dr.Pageid)); }
                else if (dr.Pageid == 1110 && visit.Detail.BillSaleSuccess) { result += (", " + Helpers.Controls.GetDetailsVisit(dr.Pageid)); }
            }
            return result;
        }
        public static String GetDetailsVisit(int ID)
        {
            //VisitSumaryPage
            if (ID == 1100) { return "เข้าพบ"; }
            //QuestionPage
            else if (ID == 1101) { return "แบบสอบถาม"; }
            //CVSStockPage
            else if (ID == 1102) { return "เช็คสต็อก"; }
            //HyperStockPage
            else if (ID == 1103) { return "เช็คสต็อก"; }
            //AgentStockPage
            else if (ID == 1104) { return "เช็คสต็อก"; }
            //ShopStockPage
            else if (ID == 1105) { return "เช็คสต็อก"; }
            //AgentPickingPage
            else if (ID == 1106) { return "เบิกสินค้า"; }
            //AgentReturnPage
            else if (ID == 1107) { return "คืนสินค้า"; }
            //CashSalePage
            else if (ID == 1108) { return "ขายเงินสด"; }
            //SOPage
            else if (ID == 1109) { return "สั่งขายสินค้า"; }
            //BillsalePage
            else if (ID == 1110) { return "เช็คยอดขาย"; }

            else return "";
        }
        public static Page GetMenuPage(int ID)
        {
            if (ID == 1000) { return new Visit.TodayPage(); }
            else if (ID == 1001) { return new Plan.TodayPage(); }
            else if (ID == 1002) { return new Customer.CustomerListPage(); }
            else if (ID == 1003) { return new Product.ProductListPage(); }
            else if (ID == 1004) { return new Profile.EmployeeToday(); }
            else if (ID == 1005) { return new Plan.CreatePlanPage(); }
            else if (ID == 1006) { return new Picking.ListPickingPage(); }
            else if (ID == 1007) { return new CashSale.ListCashsalePage(); }
            else if (ID == 1008) { return new SaleOrder.TodayPage(); }
            else if (ID == 1009) { return new Visit.TodayPage(); }
            else if (ID == 1010) { return new Reports.ReportVisitPage(); }
            else if (ID == 1011) { return new Reports.ReportSaleOrderPage(); }
            else if (ID == 1012) { return new Reports.ReportVisitPage(); }
            else if (ID == 1013) { return new Profile.ProfilePage(); }
            else { return new Menu.NoPage(); }

        }

        #region ======= Visit ========
        public static async Task<Models.VisitShowpageData> CheckLastVisit()
        {
            Models.VisitData lastvisit = App.dbmng.sqlite.Table<Models.VisitData>().FirstOrDefault();
            if (lastvisit != null)
            {
                Models.VisitShowpageData data = await GetVisitShowpage(lastvisit);
                return await Task.FromResult(data);
            }

            return null;
        }
        public static async Task<Models.VisitShowpageData> GetVisitShowpage(Models.CustomerData cust, Models.PlanData plan = null, string visitid = "")
        {
            Models.VisitData visitdata = null;
            //if (plan == null) { plan = new Models.PlanData(); }
            if (visitid.Equals(""))
            {
                // New Visit
                visitdata = await GetVisitdata(cust, plan);
            }
            else
            {
                visitdata = await App.Ws.GetVisitData(visitid);
            }
            if (visitdata == null) { return null; }
            List<Models.VisitPage> listpage = await GetVisitpage(cust.Typeid);
            Models.VisitShowpageData result = new Models.VisitShowpageData()
            {
                Customer = cust,
                Visitdata = visitdata,
                VisitPage = listpage,
            };
            result.Detail.Key = visitdata.Key;
            result.Detail.Newvisit = visitid.Equals("");
            return result;
        }
        public static async Task<List<Models.VisitStockData>> GetVisitstokLog(string visitid, int custgroupid)
        {
            try
            {
                List<Models.VisitStockData> result = await App.Ws.GetVisitStocklog(custgroupid, visitid);
                if (result != null && result.Count > 0)
                {
                    foreach (var dr in result)
                    {
                        if (!dr.Sale) { dr.Stock = false; dr.Display = "ไม่มีจำหน่าย"; }
                        else { dr.Display += dr.Stock ? "มีจำหน่าย, มีสินค้า" : "มีจำหน่าย, สินค้าหมด"; }
                        dr.Check = true;
                        dr.Icon = "ic_check";
                        dr.CanEdit = false;
                    }
                }
                return await Task.FromResult(result);
            }
            catch { }
            return null;
        }
        public static async Task<List<Models.VisitStockData>> GetNewVisitstok(string visitid, Models.CustomerData customer)
        {
            try
            {
                List<Models.VisitStockData> result = App.dbmng.sqlite.Table<Models.VisitStockData>()
                                        .Where(x => x.Visitid.Equals(visitid))
                                        .OrderBy(x => x.Piority).ThenBy(x => x.Productname).ToList();
                if (result == null || result.Count == 0)
                {
                    result = await App.Ws.GetVisitStock(customer.Custgroupid, customer.Key);
                    if (result != null && result.Count > 0)
                    {
                        foreach (var dr in result)
                        {
                            if (!dr.Sale) { dr.Stock = false; }
                            dr.Visitid = visitid;
                            dr.Custid = customer.Key;
                            dr.Custtype = customer.Typeid;
                            dr.Grouptype = customer.Custgroupid;
                            dr.Key = visitid + "-" + dr.Productid;
                            dr.Check = true;
                            App.dbmng.InsetData(dr);
                        }

                    }

                }
                return await Task.FromResult(result);
            }
            catch { }
            return null;
        }
        public static async Task<Models.VisitShowpageData> GetVisitShowpage(Models.VisitData visitdata)
        {
            Models.VisitShowpageData result = new Models.VisitShowpageData();
            try
            {
                result.Visitdata = visitdata;
                result.Customer = App.dbmng.sqlite.Table<Models.CustomerData>().Where(x => x.Key.Equals(visitdata.Custid)).FirstOrDefault();
                if (result.Customer == null) { result.Customer = await App.Ws.GetCustomer(visitdata.Custid); }
                result.VisitPage = await GetVisitpage(result.Customer.Typeid);

                result.Detail = App.dbmng.sqlite.Table<Models.VisitDetailData>().Where(x => x.Key.Equals(visitdata.Key)).FirstOrDefault();
                if (result.Detail == null) { result.Detail = new Models.VisitDetailData(); }

                result.Stock = App.dbmng.sqlite.Table<Models.VisitStockData>().Where(x => x.Visitid.Equals(visitdata.Key)).OrderBy(x => x.Piority).ThenBy(x => x.Productname).ToList();
                if (result.Stock != null && result.Stock.Count == 0) { result.Stock = null; }

                //result.VisitQuestionnaire = App.dbmng.sqlite.Table<Models.Questionnaire>().Where(x => x.Visitid.Equals(visitdata.Key)).OrderBy(x => x.Piority).ThenBy(x => x.QuestionID).ToList();
                result.VisitImage = App.dbmng.sqlite.Table<Models.VisitImage>().Where(x => x.Visitid.Equals(visitdata.Key)).OrderBy(x => x.Item).ToList();
                if (result.VisitImage != null && result.VisitImage.Count == 0) { result.VisitImage = null; }

                //result.VisitBillSale = App.dbmng.sqlite.Table<Models.VisitBillSale>().Where(x => x.Visitid.Equals(visitdata.Key)).OrderBy(x => x. Piority).ThenBy(x => x.Productname).ToList();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
            return await Task.FromResult(result);
        }
        public static async Task<bool> SaveVisit(Models.VisitShowpageData data)
        {
            try
            {
                if (data.Detail.Newvisit)
                {
                    App.dbmng.InsetData(data.Visitdata);
                    App.dbmng.InsetData(data.Customer);
                    App.dbmng.InsetData(data.Detail);
                }
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);

        }
        public static async Task<bool> ClearLastvisit(string visitid)
        {
            try
            {
                App.dbmng.sqlite.Table<Models.VisitData>().Delete(x => x.Key.Equals(visitid));
                App.dbmng.sqlite.Table<Models.VisitDetailData>().Delete(x => x.Key.Equals(visitid));
                App.dbmng.sqlite.Table<Models.VisitStockData>().Delete(x => x.Visitid.Equals(visitid));

                App.dbmng.sqlite.Table<Models.VisitImage>().Delete(x => x.Key.Equals(visitid));
                App.dbmng.sqlite.Table<Models.VisitBillSale>().Delete(x => x.Visitid.Equals(visitid));

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
            return await Task.FromResult(false);

        }
        public static async Task<List<Models.VisitPage>> GetVisitpage(int Typeid)
        {
            List<Models.VisitPage> res = App.dbmng.sqlite.Table<Models.VisitPage>().OrderBy(x => x.Piority).ToList();
            List<Models.VisitPage> result = App.dbmng.sqlite.Table<Models.VisitPage>().Where(x => x.Custtype == Typeid).OrderBy(x => x.Piority).ToList();
            if (result == null || result.Count == 0) { result = new List<Models.VisitPage>(); }
            return await Task.FromResult(result);
        }
        public static async Task<Models.VisitData> GetVisitdata(Models.CustomerData cust, Models.PlanData plan)
        {
            try
            {
                if (cust != null)
                {
                    if (App.Checkinlocation == null)
                    {
                        var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5));
                        App.Checkinlocation = await Geolocation.GetLocationAsync(request);
                    }
                    string address = await GetLocationname(App.Checkinlocation.Latitude, App.Checkinlocation.Longitude);
                    Models.Admindata admindata = await App.Ws.GetAdminData(App.Checkinlocation.Latitude, App.Checkinlocation.Longitude);
                    Models.BatteryData battery = GetBattery();
                    string visitid = GetID() + App.UserProfile.Empid.ToString().PadLeft(3, '0');
                    Models.VisitData result = new Models.VisitData()
                    {
                        Key = visitid,
                        Lat = App.Checkinlocation.Latitude,
                        Lng = App.Checkinlocation.Longitude,
                        Imei = App.UserProfile.Deviceserial,
                        Empid = App.UserProfile.Empid,
                        Empcode = App.UserProfile.Empcode,
                        Empfullname = App.UserProfile.Fullname,
                        Custid = cust.Key,
                        Custcode = cust.Custcode,
                        Custname = cust.Custname,
                        Custlat = cust.Lat,
                        Custlng = cust.Lng,
                        Typeid = cust.Typeid,
                        Typename = cust.Typename,
                        Groupid = cust.Custgroupid,
                        Groupname = cust.Custgroupname,
                        Icon = cust.Icon,
                        Planid = plan == null ? "" : plan.Key,
                        Planstatus = plan == null ? 12 : plan.Planstatus,
                        Admincode = admindata.AdminCode,
                        Adminname = admindata.AdminName,
                        Location = address,
                        Battery = battery.Level,
                        Remark = "เข้าพบ",
                    };
                    if (cust.Lat == 0 || cust.Lng == 0) { result.Diffdistance = 0; }
                    else
                    {
                        Location Custlocation = new Location(cust.Lat, cust.Lng);
                        int Mile = Convert.ToInt32(Location.CalculateDistance(Custlocation, App.Checkinlocation, DistanceUnits.Kilometers) * 1000);
                        result.Diffdistance = (Mile / 1000);
                    }
                    if (result.Diffdistance > App.CheckLocation)
                    {
                        result.Errortext = "ระยะห่างเกินกำหนด " + result.Diffdistance.ToString("0.000") + " กม.";
                        result.Errortype = -1;
                    }
                    return await Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
            return null;
        }
        public static async Task<List<Models.VisitStockShop>> GetStockList(List<Models.VisitStockData> datas, bool canedit)
        {
            List<Models.VisitStockShop> result = new List<Models.VisitStockShop>();
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    foreach (var dr in datas)
                    {
                        Models.VisitStockShop data = new Models.VisitStockShop()
                        {
                            Icon = dr.Icon,
                            Productname = dr.Productname,
                            Price = dr.Price,
                            Data = dr,
                            Canedit = canedit,
                        };
                        if (!dr.Sale) { data.Unsale = true; }
                        else
                        {
                            if (dr.Stock) { data.Stock = true; }
                            else { data.Lost = true; }
                        }
                        result.Add(data);
                    }
                }
            }
            catch { }
            return await Task.FromResult(result);
        }

        #endregion

        #region ======= Visit Questionnaire=
        public static async Task<List<Models.Answer>> GetListofAnswerTime()
        {
            List<Models.Answer> result = new List<Models.Answer>();
            int period = Helpers.Settings.AnswerTime;
            if (period <= 0) { period = 60; }
            int total = (1440 / period);
            DateTime today = Helpers.Controls.GetToday();
            try
            {
                Random ran = new Random();
                for (int i = 0; i < total; i++)
                {
                    Models.Answer temp = new Models.Answer()
                    {
                        Item = i,
                        Description = Date2String(today.AddMinutes(i * period), "HH:mm"),
                    };
                    result.Add(temp);
                }
            }
            catch { }
            return await Task.FromResult(result);
        }
        public static async Task<List<Models.Answer>> GetListofAnswerPeriod()
        {
            List<Models.Answer> result = new List<Models.Answer>();
            int period = Helpers.Settings.AnswerPeriod;
            if (period <= 0) { period = 60; }
            int total = (1440 / period);
            DateTime today = Helpers.Controls.GetToday();
            try
            {
                Random ran = new Random();
                for (int i = 0; i < total; i++)
                {
                    Models.Answer temp = new Models.Answer()
                    {
                        Item = i,
                        Description = Helpers.Controls.Date2String(today.AddMinutes(i * period), "HH:mm"),
                    };
                    result.Add(temp);
                }
            }
            catch { }
            return await Task.FromResult(result);
        }
        #endregion

        public static List<Models.CustomerFillter> GetCustFillter(int team)
        {
            return App.dbmng.sqlite.Table<Models.CustomerFillter>().Where(x => x.Team == team).OrderBy(x => x.Piority).ThenBy(x => x.Display).ToList();
        }
        static Models.BatteryData GetBattery()
        {
            Models.BatteryData result = new Models.BatteryData();
            try
            {
                result.Level = Convert.ToInt32(Battery.ChargeLevel * 100);
                result.Source = Battery.PowerSource.ToString();
                result.State = Battery.State.ToString();
            }
            catch { }
            return result;
        }
        public static async Task<ImageSource> GetProfileImage()
        {
            ImageSource result = "avartar";
            try
            {
                string imagefile = Settings.ProfileImage;
                if (imagefile.Equals(""))
                {
                    // ไปหามา จาก WS
                    Models.Userimage uimg = await App.Ws.GetUserimage(App.UserProfile.Empid);
                    if (uimg.Image64.Equals("no_image") || uimg.Image64.Equals(""))
                    {
                        Settings.ProfileImage = "avatar";
                        result = "avatar";
                    }
                    else
                    {
                        imagefile = DependencyService.Get<Helpers.ICallService>().GetPath("UserProfile.jpg");
                        if (ImageConvert.Image2File(uimg.Image64, imagefile))
                        {
                            Settings.ProfileImage = imagefile;
                            result = ImageSource.FromFile(imagefile);
                        }
                    }

                }
                else
                {
                    result = ImageSource.FromFile(imagefile);
                }

            }
            catch { }
            return await Task.FromResult(result);
        }
        public static async Task<ImageSource> GetQRPayment()
        {
            ImageSource result = "no_image";
            try
            {
                string imagefile = Settings.QRPayment;
                if (imagefile.Equals(""))
                {
                    // ไปหามา จาก WS
                    Models.Userimage uimg = await App.Ws.GetUserimage(App.UserProfile.Empid, 10);
                    if (uimg.Image64.Equals("no_image") || uimg.Image64.Equals(""))
                    {
                        Settings.QRPayment = "no_image";
                        result = "no_image";
                    }
                    else
                    {
                        imagefile = DependencyService.Get<Helpers.ICallService>().GetPath("QRPayment.jpg");
                        if (ImageConvert.Image2File(uimg.Image64, imagefile))
                        {
                            Settings.QRPayment = imagefile;
                            result = ImageSource.FromFile(imagefile);
                        }
                    }

                }
                else
                {
                    result = ImageSource.FromFile(imagefile);
                }

            }
            catch { }
            return await Task.FromResult(result);
        }

        public static async Task<List<Models.MenuList>> GetListMenu(int roleid, int teamid)
        {
            List<Models.MenuList> result = App.dbmng.sqlite.Table<Models.MenuList>().Where(x => x.roleid == roleid && x.Teamid == teamid).OrderBy(x => x.Piority).ToList();
            result.Add(new Models.MenuList() { Id = 999, Title = "ออกจากระบบ", Icon = "mnu_logout", Piority = 999 });
            return await Task.FromResult(result);
        }

        public static async Task<bool> ShareVisitImageFromDb(bool isone, Models.VisitShowpageData VisitData, string imgKey)
        {
            try
            {
                MakeFileLocation(isone, VisitData.Visitdata, imgKey);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("Share DrawImage Error", ex.Message);
                return await Task.FromResult(false);
            }
        }

        private static async void MakeFileLocation(bool isone, Models.VisitData vid, string imgKey)
        {
            try
            {
                string visitdate = Date2String(vid.Visittime, "dd-MM-yyyy HH:mm");

                if (isone)
                {
                    Models.VisitImage ListImg = new Models.VisitImage();
                    ListImg = App.dbmng.sqlite.Table<Models.VisitImage>().Where(x => x.Key.Equals(imgKey)).FirstOrDefault();

                    //สร้างไฟล์ไปเก็บที่อยู่เครื่อง (root)
                    string paths = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string filename = vid.Typeid.ToString() + ".jpg";
                    string pathname = Path.Combine(paths, filename);
                    pathname = DependencyService.Get<Helpers.ICallService>().GetPath(pathname);

                    //สร้างไฟล์ไปเก็บที่อยู่เครื่อง
                    ImageConvert.Image2File(ListImg.ImgBase64, pathname);

                    //copy file root>media
                    string fn = Path.Combine(App.Imagepath, pathname);
                    fn = DependencyService.Get<Helpers.ICallService>().GetPath(pathname);
                    DependencyService.Get<Helpers.ICallService>().ReplaceFile(fn, vid.Typeid.ToString());
                    DependencyService.Get<Helpers.ICallService>().DeleteFile(fn);
                    DependencyService.Get<Helpers.ICallService>().DeleteFile(pathname);

                    await ShareVisitImage(vid.Custname, visitdate, App.UserProfile.Fullname, vid.Location, vid.Typename, vid.Typeid);
                }
                else
                {
                    List<Models.VisitImage> ListImg = new List<Models.VisitImage>();
                    ListImg = App.dbmng.sqlite.Table<Models.VisitImage>().Where(x => x.Visitid.Equals(vid.Key)).ToList();
                    var sharename = "";
                    foreach (var dr in ListImg)
                    {
                        //สร้างไฟล์ไปเก็บที่อยู่เครื่อง (root)
                        string paths = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        string filename = vid.Typeid.ToString() + dr.Item.ToString() + ".png";
                        string pathname = Path.Combine(paths, filename);
                        pathname = DependencyService.Get<Helpers.ICallService>().GetPath(pathname);

                        //สร้างไฟล์ไปเก็บที่อยู่เครื่อง
                        ImageConvert.Image2File(dr.ImgBase64, pathname);

                        //copy file root-->media
                        string fn2 = DependencyService.Get<Helpers.ICallService>().GetPath(pathname);
                        int Typeid = int.Parse(vid.Typeid.ToString() + dr.Item.ToString());
                        DependencyService.Get<Helpers.ICallService>().ReplaceFile(fn2, Typeid.ToString());

                        string imgfile = DependencyService.Get<Helpers.ICallService>().DrawImage(pathname, Typeid, vid.Custname, visitdate, App.UserProfile.Fullname, vid.Location);

                        //ต่อชื่อไฟล์
                        if (sharename.Equals("")) { sharename = imgfile; }
                        else { sharename += "," + imgfile; }

                        DependencyService.Get<Helpers.ICallService>().DeleteFile(fn2);
                        DependencyService.Get<Helpers.ICallService>().DeleteFile(pathname);

                    }
                    await DependencyService.Get<Helpers.ICallService>().Share(sharename);
                }
            }
            catch (Exception ex)
            {
                var exc = ex.Message;
            }


        }
    }
}
