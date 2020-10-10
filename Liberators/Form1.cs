using System;
using System.Collections;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;

namespace Liberators
{
    public partial class Liberators : Form
    {
        static string vs = "1.0.1";
        static string gameUrl = "https://liberators.mutantbox.com?micend=1";
        static string uplaodUrl = "https://upload.mutantbox.com";

        //static string gameUrl = "https://testliberators.movemama.com?micend=1";
        //static string uplaodUrl = "https://testupload.movemama.com";
        //static string gameUrl = "https://weili.ooopic.com/weili_12358894.html?_Track=1d02405e43410555a1ecadc3e738a3df";

        //static string gameUrl = "http://test.test.com/22.html";
        //static string uplaodUrl = "https://testupload.movemama.com";

        ChromiumWebBrowser webview;
        private Serializer serializer;
        static string uid = "";
        private bool send = false;
        private static tool tool = null;

        static bool devTools = false;
        static string env = "prod";

        public Liberators(string[] args)
        {
            try {
                InitializeComponent();
                if (args.Length == 4 && args[0]=="ljq0930") {
                    gameUrl = args[1];
                    env = args[2];
                    if(env == "test") {
                        uplaodUrl = "https://testupload.movemama.com";
                    }
                    devTools = Convert.ToBoolean(args[3]);
                }

                int width, height;
                Rectangle screenArea = Screen.GetWorkingArea(this);
                width = screenArea.Width;
                height = screenArea.Height;

                Size areaInfo = new Size((int)(width * 0.95), (int)(height * 0.95));
                this.Size = areaInfo;
                this.Location = (Point)new Size((int)(width * 0.05) / 2, (int)(height * 0.05) / 2);
                this.BackColor = Color.FromArgb(255, 14, 5, 22);

                if (tool == null) tool = new tool();

                ThreadStart childref = new ThreadStart(CallToChildThread);
                Thread childThread = new Thread(childref);
                childThread.Start();
                CheckForIllegalCrossThreadCalls = false;
            }
            catch (Exception e1) {
                string msg1 = "e1" + ":" + e1.Message + "^" + e1.StackTrace + "^" + e1.Source;
                log("error", msg1);
                throw;
            }
        }

        private void Liberators_Load(object sender, EventArgs e)
        {
            try {
                LoadFlash();
                webview = new ChromiumWebBrowser(gameUrl);
                webview.IsBrowserInitializedChanged += (s, args) =>
                {
                    if (devTools) webview.ShowDevTools();//调试模式
                    if (webview.IsBrowserInitialized) {
                        
                        Cef.UIThreadTaskFactory.StartNew(() => {
                            string error = "";
                            var requestContext = webview.GetBrowser().GetHost().RequestContext;
                            requestContext.SetPreference("profile.default_content_setting_values.plugins", 1, out error);
                        });
                    }                    
                };
                webview.FrameLoadStart += Webview_FrameLoadStart;
                webview.FrameLoadEnd += Webview_FrameLoadEnd;

                webview.Dock = DockStyle.Fill;
                this.Controls.Add(webview);                
            }
            catch (Exception e2) {
                string msg2 = "e2" + ":" + e2.Message + "^" + e2.StackTrace + "^" + e2.Source;
                log("error", msg2);
                throw;
            }           
        }


        private void Webview_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            button1.BackgroundImage = global::Liberators.Properties.Resources.refresh1;

            var cookieManager = CefSharp.Cef.GetGlobalCookieManager();

            CookieVisitor visitor = new CookieVisitor();
            visitor.SendCookie += visitor_SendCookie;
            cookieManager.VisitAllCookies(visitor);
        }

        private void visitor_SendCookie(CefSharp.Cookie obj)
        {
            try {
                if ((string.Compare(obj.Domain, ".movemama.com") == 0 || string.Compare(obj.Domain, ".mutantbox.com") == 0) && string.Compare(obj.Name, "user_auth") == 0) {
                    string stri = HttpUtility.UrlDecode(obj.Value);
                    string str = stri.Substring(stri.IndexOf(":") - 1);
                    serializer = new Serializer();
                    ArrayList userAuth = (ArrayList)serializer.Deserialize(str);
                    Hashtable info = (Hashtable)userAuth[1];
                    uid = info["uid"].ToString();
                    if (send == false) {
                        var regValue = tool.GetRegValue("uid");
                        string regNewValue = uid + "^" + vs;
                        if (regValue == null || string.Compare(regValue.ToString(), regNewValue) != 0) {
                            tool.SetRegValue("uid", uid + "^" + vs);
                            log("micend", "", 0);
                        }
                        
                        log("micend", "", 2);
                        send = true;
                    }
                }
            }
            catch (Exception) {

                throw;
            }            
        }

        private void Webview_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            loadimg.Visible = false;
            button1.Visible = true;
        }

        private void LoadFlash()
        {
            try {
                var settings = new CefSettings();
                settings.CefCommandLineArgs.Add("enable-npapi", "1");
                settings.CefCommandLineArgs.Add("enable-system-flash", "1"); //启用flash
                settings.CefCommandLineArgs.Add("ppapi-flash-version", "99.0.0.999"); //设置flash插件版本
                settings.CefCommandLineArgs.Add("ppapi-flash-path", AppDomain.CurrentDomain.BaseDirectory + "\\plugins\\pepflashplayer18.dll");
                settings.CefCommandLineArgs.Add("enable-media-stream", "1");                

                //var cachePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Liberators";
                var cachePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Liberators";

                //settings.RootCachePath = cachePath;
                settings.CachePath = cachePath;

                settings.PersistSessionCookies = true;
                //settings.LogSeverity = LogSeverity.Disable;
                settings.BackgroundColor = ColorToUInt(System.Drawing.Color.FromArgb(255, 14, 5, 22));

                Cef.Initialize(settings);
            }
            catch (Exception e3) {
                string msg3 = "e3" + ":" + e3.Message + "^" + e3.StackTrace + "^" + e3.Source;
                log("error", msg3);
                throw;
            }
            
        }

        public static uint ColorToUInt(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0));
        }

        /// <summary>
        /// 创建子进程发送日志数据
        /// </summary>
        public static void CallToChildThread()
        {
            try {
                log("micend");
                checkoutVersion();
            }
            catch (Exception) {
                //MessageBox.Show("e4" + ":" + e4.Message + "^" + e4.StackTrace + "^" + e4.Source);
                throw;
            }
        }

        private static void log(string type = "micend", string res = "", int step=1)
        {
            try {
                var regUid = tool.GetRegValue("uid");
                uid = regUid == null ? "" : regUid.ToString();

                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                string tmp = Convert.ToInt64(ts.TotalSeconds).ToString();
                string mac = tool.getMac();
                string json = "{\"tag\":\"" + vs + "\",\"typeid\":1,\"type\":\"" + type + "\",\"appid\":4,\"action\":\"start\",\"tmp\":" + tmp + ",\"data\":\"" + res + "\",\"uid\":\"" + uid + "^" + mac + "\",\"step\":" + step + "}";

                var domain = uplaodUrl + "/push_platform.php?data=" + json;
                var url = new Uri(domain);
                var httpClient = new HttpClient();
                //for (int i = 0; i < 50; i++) {
                    var response = httpClient.GetAsync(url).Result;
                    var data = response.Content.ReadAsStringAsync().Result;
                //}                
            }
            catch (Exception) {
                throw;
            }
        }

        /// <summary>
        /// 检查版本号
        /// </summary>
        public static void checkoutVersion()
        {
            var result = tool.Http(uplaodUrl+"/version.php");
            if(result != null) {
                vinfo rt = JsonConvert.DeserializeObject<vinfo>(result);

                var compare = string.Compare(rt.Vs, vs);
                bool e = true;
                string day = DateTime.Now.ToString("yyyyMMdd");
                var OldDay = tool.GetRegValue("day");
                if (OldDay == null) {
                    tool.SetRegValue("day", day);
                    e = false;
                }
                else {
                    e = day.Equals(tool.GetRegValue("day"));
                    if (!e) tool.SetRegValue("day", day);
                }

                if (compare == 1 && !e) {
                    string msg = "Please update the game client(" + rt.Vs + ").";
                    string lastUrl = rt.Url;
                    var yesorno = MessageBox.Show(msg, "Tips", MessageBoxButtons.YesNo);
                    if (yesorno == DialogResult.Yes) {
                        System.Diagnostics.Process.Start(rt.Url);
                    }
                }
            }            
        }

        private void refresh(object sender, EventArgs e)
        {
            //webview.GetBrowser().Reload();
            webview.Reload(true);
            button1.BackgroundImage = global::Liberators.Properties.Resources.refresh2;
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.FlatAppearance.BorderSize = 1;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.FlatAppearance.BorderSize = 0;
        }

    }
}