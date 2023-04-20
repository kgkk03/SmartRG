using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;

namespace smartrg.Helpers
{
    public class Ws
    {
        public string Url { get; set; }
        IMqttClient client;

        public Ws()
        {
            Url = "http://35.240.136.227:9014";
            //Url = "http://192.168.1.38:9014";
        }

        public async Task<Models.GetLoginData> GetLogin(string login,string pwd)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "login",
                    loginname = login,
                    password = pwd,
                    devicetime = App.Servertime,
                    deviceserial = App.Imei,
                    version = Xamarin.Essentials.AppInfo.VersionString,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getlogin", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetLoginData>();
                    return await Task.FromResult(result); 
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("WS GetLogin", ex.Message);
            }
            return null;
        }
        public async Task<DateTime> GetServertime()
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "getservertime",
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    devicetime = App.Servertime,
                    deviceserial = App.Imei,
                    version = Xamarin.Essentials.AppInfo.VersionString,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return await Task.FromResult(result.Servertime);
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("WS GetServertime", ex.Message);
            }
            return DateTime.Now;
        }

        #region Syncdata
        public async Task<List<object>> GetSyncData(string tbname)
        {
            try
            {
                string tb = tbname;
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    deviceid = App.UserProfile.Deviceserial,
                    teamid = App.UserProfile.Teamid,
                    team = App.UserProfile.TeamName,
                    role=App.UserProfile.Role,
                    modified = App.dbmng.Getsynctime(tb)
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if(result.Data!=null && result.Data.Count > 0)
                        {
                            List<object> resultdata = new List<object>();

                            foreach (var dr in result.Data)
                            {
                                resultdata.Add(dr);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("Get GetSyncData from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.CommandData>> GetCommandData()
        {
            try
            {
                string tb = "devicecmd";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    deviceid = App.UserProfile.Deviceserial,
                    modified = App.dbmng.Getsynctime(tb)
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        List<object> resultdata = result.Data;
                        var data = new List<Models.CommandData>();
                        if ((resultdata != null) && resultdata.Count > 0)
                        {
                            foreach (var dr in resultdata)
                            {
                                Models.CommandData temp = JObject.Parse(dr.ToString()).ToObject<Models.CommandData>();
                                data.Add(temp);
                            }
                        }
                        return await Task.FromResult(data);
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("Get CommandData from Server", ex.Message);
            }
            return null;
        }
        public async Task<Models.Admindata> GetAdminData(double latitude, double longitude)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    lat = latitude,
                    lng = longitude
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getadmin", contents).ConfigureAwait(false);

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetSingleData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null)
                        {
                            Models.Admindata data = JObject.Parse(result.Data.ToString()).ToObject<Models.Admindata>();
                            return await Task.FromResult(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("Get StatusText from Server", ex.Message);
            }
            return await Task.FromResult(new Models.Admindata()) ;
        }
        public async Task<int> SendataInTable(string tbname)
        {
            try
            {
                var client = new HttpClient();
                List<object> Data = await App.dbmng.GetdataInTable(tbname);
                var jsontxt = new
                {
                    tbname = tbname,
                    uid = App.UserProfile.Empid,
                    data = Data,
                    teamid = App.UserProfile.Teamid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedataintable", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return ((result.Code == "000") ? Data.Count : 0);
                }

            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SendataInTable", ex.Message);
                return 0;
            }
            return 0;
        }

        #endregion

        #region GetData
        public async Task<List<Models.CustinlistData>> GetListCustomer(int Filterid, string Keyword,double Lat, double Lng, int startlimit )
        {
            try
            {
                string tb = "custlist";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    teamid = App.UserProfile.Teamid,
                    role = App.UserProfile.Role,
                    uid = App.UserProfile.Empid,
                    filterid = Filterid,
                    keyword = Keyword,
                    lat = Lat,
                    lng = Lng,
                    limit = startlimit.ToString() + ",20",
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.CustinlistData> resultdata = new List<Models.CustinlistData>();

                            foreach (var dr in result.Data)
                            {
                                var temp = JObject.Parse(dr.ToString()).ToObject<Models.CustinlistData>();
                                resultdata.Add(temp);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("Get GetListCustomer from Server", ex.Message);
            }
            return null;
        }
        public async Task<Models.CustomerData> GetCustomer(string CustID)
        {
            try
            {
                string tb = "customer";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = true,
                    teamid = App.UserProfile.Teamid,
                    role = App.UserProfile.Role,
                    uid = App.UserProfile.Empid,
                    custid = CustID,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetSingleData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null )
                        {
                            Models.CustomerData resultdata = JObject.Parse(result.Data.ToString()).ToObject<Models.CustomerData>();
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("Get GetCustomer from Server", ex.Message);
            }
            return null;
        }
        public async Task<Models.CustImage> GetCustomerImage(string CustID)
        {
            try
            {
                string tb = "custpic";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = true,
                    teamid = App.UserProfile.Teamid,
                    role = App.UserProfile.Role,
                    uid = App.UserProfile.Empid,
                    custid = CustID,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetSingleData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null)
                        {
                            Models.CustImage resultdata = JObject.Parse(result.Data.ToString()).ToObject<Models.CustImage>();
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("Get GetCustomer from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.Empvisit>> GetEmpByAudit()
        {
            try
            {
                string tb = "empaudit";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.Empvisit> resultdata = new List<Models.Empvisit>();
                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.Empvisit>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GET GetEmpByAudit from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.VisitData>> GetVisitList(DateTime start, DateTime finish)
        {
            try
            {
                string tb = "visitlist";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    starttime = start,
                    finishtime = finish,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.VisitData> resultdata = new List<Models.VisitData>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.VisitData>();
                                data.Showtime = Helpers.Controls.Date2String(data.Visittime, "HH:mm");
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GET VisitInlistData from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.VisitData>> GetVisitByAudit()
        {
            try
            {
                string tb = "visitaudit";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.VisitData> resultdata = new List<Models.VisitData>();
                            DateTime today = Helpers.Controls.GetToday();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.VisitData>();
                                if(data.Modifieddate> today) { data.Showtime = Helpers.Controls.Date2String(data.Modifieddate, "HH:mm"); }
                                else { data.Showtime = Helpers.Controls.Date2ThaiString(data.Modifieddate, "dd-MMM-yy"); }
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GET VisitInlistData from Server", ex.Message);
            }
            return null;
        }
        public async Task<Xamarin.Forms.ImageSource> GetEmpThumbnail(int Empid)
        {
            try
            {
                string tb = "empthumbnail";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = true,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    empid = Empid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetSingleData>();
                    if (result.Code.Equals("000"))
                    {
                        var data = JObject.Parse(result.Data.ToString()).ToObject<Models.Empthumbnail>();
                        return ImageConvert.ImageFB64(data.Thumbnail);
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GET GetEmpByAudit from Server", ex.Message);
            }
            return null;
        }
        public async Task<Models.VisitData> GetVisitData(string id)
        {
            try
            {
                string tb = "visit";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = true,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    visitid = id,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetSingleData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null )
                        {
                            Models.VisitData resultdata = JObject.Parse(result.Data.ToString()).ToObject<Models.VisitData>();
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GET GetVisitData from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.VisitQuestion>> GetVisitQuestionByCust(int custgroup)
        {
            try
            {
                string tb = "visitquestioncust";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    goupid = custgroup,

                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.VisitQuestion> resultdata = new List<Models.VisitQuestion>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.VisitQuestion>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetVisitQuestion from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.Questionnaire>> GetVisitQuestionLog(int Grouppriceid, string Visitid)
        {
            try
            {
                string tb = "visitquestionlog";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    groupid = Grouppriceid,
                    teamid = App.UserProfile.Teamid,
                    visitid = Visitid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.Questionnaire> resultdata = new List<Models.Questionnaire>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.Questionnaire>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetVisitQuestion from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.Answer>> GetVisitAnswer(string Questionlist)
        {
            try
            {
                string tb = "visitanswer";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    questionlist = Questionlist
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.Answer> resultdata = new List<Models.Answer>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.Answer>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetVisitQuestion from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.VisitStockData>> GetVisitStock(int Grouppriceid,string Custid)
        {
            try
            {
                string tb = "productvisitstock";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    groupid = Grouppriceid,
                    custid = Custid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.VisitStockData> resultdata = new List<Models.VisitStockData>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.VisitStockData>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetVisitQuestion from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.VisitStockData>> GetVisitStocklog(int Grouppriceid, string Visitid)
        {
            try
            {
                string tb = "visitstocklog";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    groupid = Grouppriceid,
                    visitid = Visitid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.VisitStockData> resultdata = new List<Models.VisitStockData>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.VisitStockData>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetVisitQuestion from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.VisitImage>> GetVisitImage(string Visitid)
        {
            try
            {
                string tb = "visitimage";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    visitid = Visitid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getimage", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.VisitImage> resultdata = new List<Models.VisitImage>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.VisitImage>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetVisitQuestion from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.VisitImage>> GetVisitShowImage(int Type, string Value,int Startlimit=0, int Limit = 6)
        {
            try
            {
                string tb = "visitshowimage";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    type = Type,
                    value = Value,
                    start = Startlimit,
                    limit=Limit,
                };
                // type 1 = custid, 2= empid, 3= visitid
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.VisitImage> resultdata = new List<Models.VisitImage>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.VisitImage>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetVisitQuestion from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.ProductData>> GetProductList(string Keyword, string Productid="")
        {
            try
            {
                string tb = "listproduct";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    keyword = Keyword,
                    productid = Productid,

                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.ProductData> resultdata = new List<Models.ProductData>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.ProductData>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetProductList from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.ProductData>> GetSaleProduct(int Grouppriceid, string Custid)
        {
            try
            {
                string tb = "saleproduct";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    groupid = Grouppriceid,
                    custid = Custid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.ProductData> resultdata = new List<Models.ProductData>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.ProductData>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetVisitQuestion from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.SaleorderData>> GetSOList(DateTime start, DateTime finish)
        {
            try
            {
                string tb = "saleorder";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    starttime = start,
                    finishtime = finish,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.SaleorderData> resultdata = new List<Models.SaleorderData>();
                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.SaleorderData>();
                                data.Showtime = Helpers.Controls.Date2String(data.Modified, "HH:mm");
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GET VisitInlistData from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.SOlineData>> GetSOline(string Soid)
        {
            try
            {
                string tb = "soline";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    soid = Soid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.SOlineData> resultdata = new List<Models.SOlineData>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.SOlineData>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetSOline from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.PickingLineData>> GetReturnStock(string AgentID = "")
        {
            try
            {
                string tb = "returnstock";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    agenid = AgentID,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.PickingLineData> resultdata = new List<Models.PickingLineData>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.PickingLineData>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetReturnStock from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.PickingLineData>> GetPickingStock(string AgentID = "")
        {
            try
            {
                string tb = "pickingstock";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    agenid = AgentID,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.PickingLineData> resultdata = new List<Models.PickingLineData>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.PickingLineData>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetProductList from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.PickingLineData>> GetPickingDetail(string PickingID = "")
        {
            try
            {
                string tb = "pickingdetails";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    pickingid = PickingID,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.PickingLineData> resultdata = new List<Models.PickingLineData>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.PickingLineData>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetProductList from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.PickingData>> GetPickingList(DateTime start, DateTime finish,string Agenid)
        {
            try
            {
                string tb = "pickinglist";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    starttime = start,
                    finishtime = finish,
                    agentid = Agenid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.PickingData> resultdata = new List<Models.PickingData>();
                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.PickingData>();
                                data.Showtime = Helpers.Controls.Date2String(data.Pickingdate, "HH:mm");
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GET VisitInlistData from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.CustomerData>> GetAgentPicking(bool onstock=false)
        {
            try
            {
                string tb = "pickingagent";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    stock = onstock,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.CustomerData> resultdata = new List<Models.CustomerData>();
                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.CustomerData>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GET VisitInlistData from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.CashSaleData>> GetCashsaleList(DateTime start, DateTime finish)
        {
            try
            {
                string tb = "cashsalelist";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    starttime = start,
                    finishtime = finish,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.CashSaleData> resultdata = new List<Models.CashSaleData>();
                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.CashSaleData>();
                                data.Showtime = Helpers.Controls.Date2String(data.Saledate, "HH:mm");
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GET VisitInlistData from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.CashSalelineData>> GetCashsaleLine(string CashSaleID = "")
        {
            try
            {
                string tb = "cashsaleline";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    cashsaleid = CashSaleID,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.CashSalelineData> resultdata = new List<Models.CashSalelineData>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.CashSalelineData>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetProductList from Server", ex.Message);
            }
            return null;
        }
        public async Task<List<Models.PaymentData>> GetCashsalePayment(string CashSaleID = "")
        {
            try
            {
                string tb = "cashsalepayment";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    cashsaleid = CashSaleID,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.PaymentData> resultdata = new List<Models.PaymentData>();

                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.PaymentData>();
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GetProductList from Server", ex.Message);
            }
            return null;
        }
        public async Task<Models.Userimage> GetUserimage(int id,int type = 0)
        {
            Models.Userimage resulterr = new Models.Userimage() { Empid = id, Image = "avatar", Image64 = "" };
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "userimage",
                    empid = id,
                    item = type,
                    isone = true,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetSingleData>();
                    if (result.Code.Equals("000"))
                    {
                        Models.Userimage data = JObject.Parse(result.Data.ToString()).ToObject<Models.Userimage>();
                        if (data == null) { data = resulterr;  }
                        else { data.Image = ImageConvert.ImageFB64(data.Image64); }
                        return await Task.FromResult(data);
                    }
                    else { return resulterr; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("WS GetUserimage", ex.Message);
            }
            return resulterr;
        }
        public async Task<List<Models.PlanData>> GetPlanList(DateTime start, DateTime finish)
        {
            try
            {
                string tb = "plandetails";
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = tb,
                    isone = false,
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    starttime = start,
                    finishtime = finish,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/getdata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.GetListData>();
                    if (result.Code.Equals("000"))
                    {
                        if (result.Data != null && result.Data.Count > 0)
                        {
                            List<Models.PlanData> resultdata = new List<Models.PlanData>();
                            foreach (var dr in result.Data)
                            {
                                var data = JObject.Parse(dr.ToString()).ToObject<Models.PlanData>();
                                //data.Showtime = Helpers.Controls.Date2String(data.Pickingdate, "HH:mm");
                                resultdata.Add(data);
                            }
                            return await Task.FromResult(resultdata);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("GET VisitInlistData from Server", ex.Message);
            }
            return null;
        }
        #endregion

        #region Save & Update
        public async Task<string> SaveVisit(Models.VisitData Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "visit",
                    uid = App.UserProfile.Empid,
                    data = Data,
                    teamid = App.UserProfile.Teamid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    string visitdata = JsonConvert.SerializeObject(Data);
                    SendMqtt(visitdata, "smartrg-" + Data.Empid.ToString());
                    return ((result.Code == "000") ? "" : result.Message);
                }



            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SaveVisit", ex.Message);
                return ex.Message;
            }
            return "SaveVisit Main function Error";
        }
        public async Task<Models.ResultUpdate> SaveCandidate(Models.CustomerData Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "candidate",
                    uid = App.UserProfile.Empid,
                    data = Data,
                    teamid = App.UserProfile.Teamid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url  + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    Models.ResultUpdate result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return result;
                }

            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SaveCandidate", ex.Message);
                return null;
            }
            return null;
        }
        public async Task<bool> SaveCuctImage(Models.CustImage Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "custpic",
                    uid = App.UserProfile.Empid,
                    data = Data,
                    teamid = App.UserProfile.Teamid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return ((result.Code == "000") ? true : false);
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SaveCandidate", ex.Message);
                return false;
            }
            return false;
        }
        public async Task<bool> SaveUserImage(Models.Imagedata Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "emppic",
                    uid = App.UserProfile.Empid,
                    data = new { emppic = Data.ImageBase64, thumbnail = Data.Thumbnail },
                    teamid = App.UserProfile.Teamid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updateimage", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                
                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return ((result.Code == "000") ? true : false);
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SaveCandidate", ex.Message);
                return false;
            }
            return false;
        }
        public async Task<bool> SaveUserPassword(string md5pwd)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "emppwd",
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    pwd = md5pwd,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return ((result.Code == "000") ? true : false);
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SaveCandidate", ex.Message);
                return false;
            }
            return false;
        }
        public async Task<string> SaveVisitStock(Models.VisitStockData Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "visitstock",
                    uid = App.UserProfile.Empid,
                    data = Data,
                    teamid = App.UserProfile.Teamid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return ((result.Code == "000") ? "" : result.Message);
                }

            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SaveVisitStock", ex.Message);
                return ex.Message;
            }
            return "SaveVisitStock Main function Error";
        }
        public async Task<string> SaveQuestionnaire(Models.Questionnaire Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "visitquestion",
                    uid = App.UserProfile.Empid,
                    data = Data,
                    teamid = App.UserProfile.Teamid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return ((result.Code == "000") ? "" : result.Message);
                }

            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SaveQuestionnaire", ex.Message);
                return ex.Message;
            }
            return "SaveQuestionnaire Main function Error";
        }
        public async Task<string> SaveSOline(Models.SOlineData Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "soline",
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    data = Data,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return (result.Code == "000"? "" : result.Message);
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SaveVisit", ex.Message);
                return ex.Message;
            }
            return "SaveVisit Main function Error";
        }
        public async Task<string> SaveSO(Models.SaleorderData Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "saleorder",
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    data = Data,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return (result.Code == "000" ? "" : result.Message);
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SaveVisit", ex.Message);
                return ex.Message;
            }
            return "SaveVisit Main function Error";
        }
        public async Task<string> SavePickingline(Models.PickingLineData Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "pickingline",
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    data = Data,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return (result.Code == "000" ? "" : result.Message);
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SavePickingline", ex.Message);
                return ex.Message;
            }
            return "SavePickingline Main function Error";
        }
        public async Task<string> SaveClearstock(Models.PickingLineData Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "pickingline",
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    data = Data,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/clearstock", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return (result.Code == "000" ? "" : result.Message);
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SavePickingline", ex.Message);
                return ex.Message;
            }
            return "SavePickingline Main function Error";
        }

        public async Task<string> SavePicking(Models.PickingData Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "picking",
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    data = Data,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return (result.Code == "000" ? "" : result.Message);
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SavePicking", ex.Message);
                return ex.Message;
            }
            return "SavePicking Main function Error";
        }
        public async Task<string> SaveCashSale(Models.CashSaleData Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "cashsale",
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    data = Data,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return (result.Code == "000" ? "" : result.Message);
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SaveCashSale", ex.Message);
                return ex.Message;
            }
            return "SaveCashSale Main function Error";
        }
        public async Task<string> SaveCashSaleline(Models.CashSalelineData Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "cashsaleline",
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    data = Data,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return (result.Code == "000" ? "" : result.Message);
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SaveCashSaleline", ex.Message);
                return ex.Message;
            }
            return "SaveCashSaleline Main function Error";
        }
        public async Task<string> SaveCashSalePayment(Models.PaymentData Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "cashsalepayment",
                    uid = App.UserProfile.Empid,
                    teamid = App.UserProfile.Teamid,
                    data = Data,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updatedata", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return (result.Code == "000" ? "" : result.Message);
                }
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SavePayment", ex.Message);
                return ex.Message;
            }
            return "SavePayment Main function Error";
        }
        public async Task<string> SaveVisitImage(Models.VisitImage Data)
        {
            try
            {
                var client = new HttpClient();
                var jsontxt = new
                {
                    tbname = "visitimage",
                    uid = App.UserProfile.Empid,
                    data = Data,
                    teamid = App.UserProfile.Teamid,
                };
                var json = JsonConvert.SerializeObject(jsontxt);
                var contents = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url + "/updateimage", contents).ConfigureAwait(false);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);

                if (jObject != null)
                {
                    var result = JObject.Parse(jObject.ToString()).ToObject<Models.ResultUpdate>();
                    return ((result.Code == "000") ? "" : result.Message);
                }

            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SaveVisitImage", ex.Message);
                return ex.Message;
            }
            return "SaveVisitImage Main function Error";
        }

        #endregion

        #region Mqtt
        async Task<bool> ConnectMqtt()
        {
            try
            {
                client = await MqttClient.CreateAsync(App.MqttIP, App.MqttPort);
                await client.ConnectAsync();
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }
        async void SendMqtt(string data, string toppic)
        {
            try
            {
                if (client == null) { await ConnectMqtt(); }
                if (!client.IsConnected) { await client.ConnectAsync(); }
                var playload = Encoding.UTF8.GetBytes(data);
                var msg = new MqttApplicationMessage(toppic, playload);
                if (client.IsConnected) { await client.PublishAsync(msg, MqttQualityOfService.AtLeastOnce, true); }
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
        }


        #endregion

    }
}
