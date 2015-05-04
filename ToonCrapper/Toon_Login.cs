using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using WinHttp;
using System.Net;

namespace ToonCrapper
{
    public partial class Toon_Login : Form
    {
        private string LoginURL = "https://nid.naver.com/nidlogin.login";

        public delegate void Login(CookieContainer cookie);
        public event Login FormSendEvent;

        public Toon_Login()
        {
            InitializeComponent();
            webBrowser1.Navigate(LoginURL);
        }

        
/** 이 코드 사용시 CAPTCHA 나옴.
        bool NaverLogin(String ID, String PW)
        {
            WinHttpRequest Winhttp = new WinHttpRequest();

            Winhttp.Open("POST", "https://nid.naver.com/nidlogin.login");
            Winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            Winhttp.SetRequestHeader("Referer", "https://nid.naver.com/nidlogin.login");
            Winhttp.Send("enctp=2&url=http://www.naver.com&enc_url=http://www.naver.com&postDataKey=&saveID=0&nvme=0&smart_level=1&id=" + ID + "&pw=" + PW);
            Winhttp.WaitForResponse();

            if (Winhttp.ResponseText.IndexOf("https://nid.naver.com/login/sso/finalize.nhn?url=http%3A%2F%2Fwww.naver.com&sid=") == -1)
            {
                return false;
            }
            else
            {
                String Cookie = Winhttp.GetResponseHeader("Set-Cookie");
                MessageBox.Show(Cookie);
                return true;
            }
        }
**/

        private void button1_Click(object sender, EventArgs e)
        {
            FormSendEvent(GetCookieContainer());
        }

        public CookieContainer GetCookieContainer()
        {
            CookieContainer container = new CookieContainer();

            foreach (string cookie in webBrowser1.Document.Cookie.Split(';'))
            {
                string name = cookie.Split('=')[0];
                string value = cookie.Substring(name.Length + 1);
                string path = "/";
                string domain = ".naver.com"; //change to your domain name
                container.Add(new Cookie(name.Trim(), value.Trim(), path, domain));
            }

            return container;
        }

    }
}
