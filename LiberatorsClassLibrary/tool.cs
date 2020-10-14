using System;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Win32;

namespace LiberatorsClassLibrary
{
    class tool
    {
        public string Http(string url)
        {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (Exception) {

                //throw;
            }
            return null;
        }


        /// <summary>
        /// 日期对比
        /// </summary>
        /// <param name="dateStr1"></param>
        /// <param name="dateStr2"></param>
        /// <param name="msg"></param>
        public bool CompanyDate(string dateStr1, string dateStr2)
        {
            //将日期字符串转换为日期对象
            DateTime t1 = Convert.ToDateTime(dateStr1);
            DateTime t2 = Convert.ToDateTime(dateStr2);
            //通过DateTIme.Compare()进行比较（）
            int compNum = DateTime.Compare(t1, t2);
            if (compNum > 0) {
                return false;
            }
            else {
                return true;
            }
        }

        public object GetRegValue(string key)
        {
            object v = null ;
            try {
                RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Liberators");
                v = regkey.GetValue(key);
            }
            catch (NullReferenceException) {
                RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Liberators");
                v = regkey.GetValue(key);
            }
            catch (Exception) {
                //throw;
            }
            return v == null ? null : v;
        }

        public void SetRegValue(string key, string value)
        {
            try {
                RegistryKey regkeySetKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Liberators", true);
                regkeySetKey.SetValue(key, value);
            }
            catch (NullReferenceException) {
                RegistryKey regkeySetKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Liberators", true);
                regkeySetKey.SetValue(key, value);
            }
            catch (Exception) {
                throw;
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getMac()
        {
            string madAddr = null;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc2 = mc.GetInstances();
            foreach (ManagementObject mo in moc2) {
                if (Convert.ToBoolean(mo["IPEnabled"]) == true) {
                    madAddr = mo["MacAddress"].ToString();
                    madAddr = madAddr.Replace(':', '-');
                }
                mo.Dispose();
            }
            return madAddr;
        }

        public void log(string type = "micend", string res = "", int step = 1)
        {
            try
            {
                string uplaodUrl = "https://upload.mutantbox.com";
                var regUid = GetRegValue("uid");
                string uid = regUid == null ? "" : regUid.ToString();

                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                string tmp = Convert.ToInt64(ts.TotalSeconds).ToString();
                string mac = getMac();
                string json = "{\"tag\":\"" + "1.0.2" + "\",\"typeid\":1,\"type\":\"" + type + "\",\"appid\":4,\"action\":\"start\",\"tmp\":" + tmp + ",\"data\":\"" + res + "\",\"uid\":\"" + uid + "^" + mac + "\",\"step\":" + step + "}";

                var domain = uplaodUrl + "/push_platform.php?data=" + json;
                var url = new Uri(domain);
                var httpClient = new HttpClient();
                //for (int i = 0; i < 50; i++) {
                var response = httpClient.GetAsync(url).Result;
                var data = response.Content.ReadAsStringAsync().Result;
                //}                
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}