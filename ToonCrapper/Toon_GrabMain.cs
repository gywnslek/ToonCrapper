using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace ToonCrapper
{
    public partial class Toon_GrabMain : Form
    {
        private WebRequest request;
        private HttpWebResponse response;
        private Stream dataStream;
        private HtmlAgilityPack.HtmlDocument doc;

        private String SearchURL = "http://comic.naver.com/search.nhn?m=webtoon&type=title&page=1&keyword=";
        private String titleURL = "http://comic.naver.com/webtoon/list.nhn?titleId=";
        Toon_TitleSelect TitleSelector;
        Toon_DownloadProgress Downloader;
        Toon_Login Login;
        Toon_NOSelect NOSelector;

        private CookieContainer LoginInfo;

        public Toon_GrabMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (toonName.Text == "")
            {
                MessageBox.Show("제목을 입력 해주세요", "오류");
                return;
            }
            request = WebRequest.Create(this.SearchURL + toonName.Text);
            request.Proxy = null;
            request.Credentials = CredentialCache.DefaultCredentials;
            response = (HttpWebResponse)request.GetResponse();
            dataStream = response.GetResponseStream();

            doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(dataStream, Encoding.UTF8);

            HtmlAgilityPack.HtmlNode pages = doc.DocumentNode.SelectSingleNode("//*[@id='content']/div[3]/a[1]");

            if (pages != null)
            {
                MessageBox.Show("다른 검색어로 시도해 주십시오.", "작품 수 초과");
                return;
            }

            var nodes = doc.DocumentNode.SelectNodes("//*[@id='content']/div[2]/ul[2]/li/h5");
            var artists = doc.DocumentNode.SelectNodes("//*[@id='content']/div[2]/ul[2]/li/ul/li[1]/em/a");

            if (nodes != null)
            {
                TitleSelector = new Toon_TitleSelect();
                TitleSelector.FormSendEvent += new Toon_TitleSelect.ToonSelector(ToonSelected);
                TitleSelector.listView1.BeginUpdate();
                foreach (HtmlNode title in nodes)
                {

                    String titleid = title.SelectSingleNode(".//a").Attributes["href"].Value.Replace("/webtoon/list.nhn?titleId=", "");
                    String titleName = Regex.Replace(title.SelectSingleNode(".//a").OuterHtml, "<[^>]*>", "");
                    
                    ListViewItem lvi = new ListViewItem(titleid);
                    lvi.SubItems.Add(titleName);
                    lvi.SubItems.Add("N/A");
                    HtmlNode isRestricted = title.SelectSingleNode(".//span[contains(@class, 'mark_adult')]");
                    if (isRestricted != null)
                    {
                        lvi.SubItems.Add("Y");
                    }else{
                        lvi.SubItems.Add("N");
                    }

                    TitleSelector.listView1.Items.Add(lvi);
                }

                int count = 0;
                foreach (HtmlNode artist in artists) //작가 수정
                {
                    String ArtistName = Regex.Replace(artist.OuterHtml, "<[^>]*>", "").Trim() ;
                    TitleSelector.listView1.Items[count].SubItems[2].Text = ArtistName;
                    count++;
                }

                TitleSelector.listView1.EndUpdate();
                TitleSelector.ShowDialog();
            }
            else
            {
                MessageBox.Show("다른 검색어로 시도해 주십시오.", "검색결과 없음");
                return;
            }

        }

        public void ToonSelected(String titleid, String titlename, bool Restricted)
        {
            TitleSelector.Dispose();
            
            request = WebRequest.Create(this.titleURL + titleid);
            request.Proxy = null;
            request.Credentials = CredentialCache.DefaultCredentials;
            response = (HttpWebResponse)request.GetResponse();
            dataStream = response.GetResponseStream();

            doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(dataStream, Encoding.UTF8);

            HtmlAgilityPack.HtmlNode lastToon = doc.DocumentNode.SelectSingleNode("//*[@id='content']/table/tr[2]/td[2]/a");
            if (lastToon == null) { lastToon = doc.DocumentNode.SelectSingleNode("//*[@id='content']/table/tr[3]/td[2]/a"); };
            String lastToonID = HttpUtility.ParseQueryString(lastToon.Attributes["href"].Value).Get("no");

            if (Restricted)
            {                
                if (LoginInfo == null)
                {
                    Login = new Toon_Login();
                    Login.FormSendEvent += new Toon_Login.Login(SetCookie);
                    Login.ShowDialog();

                    if (LoginInfo == null)
                    {
                        return;
                    }
                }
            }

            SelectNO(titleid, titlename, lastToonID);
            //DownloadToon(titleid, titlename, "1", lastToonID, LoginInfo);

        }

        private void SetCookie(CookieContainer cookie)
        {
            Login.Dispose();
            LoginInfo = cookie;
        }

        private void DownloadToon(string titleid, string titlename, string startID, string lastID, CookieContainer LoginInfo)
        {
            Downloader = new Toon_DownloadProgress();
            Downloader.label1.Text = "0";
            Downloader.label3.Text = lastID;
            Downloader.StartID = int.Parse(startID);
            Downloader.LastID = int.Parse(lastID);
            Downloader.ComicID = int.Parse(titleid);
            Downloader.TitleName = titlename;
            Downloader.Cookie = LoginInfo;
            Downloader.StartDownload();
            Downloader.ShowDialog();

        }

        private void SelectNO(String titleid, String titlename, string lastID)
        {
            NOSelector = new Toon_NOSelect();
            NOSelector.FormSendEvent += new Toon_NOSelect.NOSelector(NOSelected);
            NOSelector.titleid = titleid;
            NOSelector.titlename = titlename;
            NOSelector.lastID = lastID;
            NOSelector.textBox3.Text = lastID;
            NOSelector.textBox1.Text = "1";
            NOSelector.ShowDialog();

        }

        private void NOSelected(String titleid, String titlename, String startID, String lastToonID)
        {
            NOSelector.Dispose();
            DownloadToon(titleid, titlename, startID, lastToonID, LoginInfo);
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutBox About = new AboutBox();
            About.ShowDialog();
        }
    }
}
