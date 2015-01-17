using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Threading;
using System.Diagnostics;

namespace ToonCrapper
{
    public partial class Toon_DownloadProgress : Form
    {
        public int StartID;
        public int LastID;
        public int ComicID;
        public CookieContainer Cookie;
        public string TitleName;
        private string toonURL = "http://comic.naver.com/webtoon/detail.nhn?titleId=";
        private string SelectedDir;
        private string DownloadArg;

        public int i;

        private HttpWebRequest request;
        private HttpWebResponse response;
        private Stream dataStream;
        private HtmlAgilityPack.HtmlDocument doc;

        BackgroundWorker backgroundWorker1;

        public Toon_DownloadProgress()
        {
            InitializeComponent();
        }

        internal void StartDownload()
        {

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();


            if (result == DialogResult.OK)
            {
                SelectedDir = dialog.SelectedPath;
            }
            else
            {
                this.Dispose();
                return;
            }

            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_Completed;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.RunWorkerAsync();
            /**
                        for (int i = StartID; i <= LastID; i++) 
                        {

                            request = (HttpWebRequest)WebRequest.Create(this.toonURL + ComicID + "&no=" + i);
                            request.Proxy = null;
                            request.CookieContainer = Cookie;
                            request.Credentials = CredentialCache.DefaultCredentials;
                            response = (HttpWebResponse)request.GetResponse();
                            dataStream = response.GetResponseStream();

                            doc = new HtmlAgilityPack.HtmlDocument();
                            doc.Load(dataStream, Encoding.UTF8);

                            var ImageInfo = doc.DocumentNode.SelectNodes("/html/body/div/div[5]/div/div[2]/div[3]/div[1]/img");

                            if (ImageInfo == null)
                            {
                                MessageBox.Show("다음화로 넘어갑니다.", "회차 오류");

                            }
                            else
                            {

                                int count = 0;
                                DownloadArg = null;
                                foreach (HtmlNode ImageURL in ImageInfo)
                                {
                                    count++;
                                    String DownloadURL = ImageURL.Attributes["src"].Value;
                                    String SaveDir = SelectedDir + "\\" + TitleName + "\\" + i + "\\" + count + ".jpg";
                                    DownloadArg = DownloadArg + "\"" + DownloadURL + "\" -o \"" + SaveDir + "\" ";
                                }
                                backgroundWorker1.RunWorkerAsync();
                            }
  
                            this.label1.Text = i.ToString();
                            progressBar1.PerformStep();
                        }
             * **/
        }

        private void Toon_DownloadProgress_Load(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = LastID;
            progressBar1.Step = 1;
            progressBar1.Value = StartID;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (i = StartID; i <= LastID; i++)
            {

                request = (HttpWebRequest)WebRequest.Create(this.toonURL + ComicID + "&no=" + i);
                request.Proxy = null;
                request.CookieContainer = Cookie;
                request.Credentials = CredentialCache.DefaultCredentials;
                response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();

                doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(dataStream, Encoding.UTF8);

                var ImageInfo = doc.DocumentNode.SelectNodes("/html/body/div/div[5]/div/div[2]/div[3]/div[1]/img");

                if (ImageInfo == null)
                {

                }
                else
                {

                    int count = 0;
                    DownloadArg = null;
                    foreach (HtmlNode ImageURL in ImageInfo)
                    {
                        count++;
                        String DownloadURL = ImageURL.Attributes["src"].Value;
                        String SaveDir = SelectedDir + "\\" + TitleName + "\\" + i + "\\" + count + ".jpg";
                        DownloadArg = DownloadArg + "\"" + DownloadURL + "\" -o \"" + SaveDir + "\" ";
                    }

                    Process cURL = new Process();

                    cURL.StartInfo.FileName = "curl.exe";
                    cURL.StartInfo.Arguments = "--create-dirs --referer 'http://comic.naver.com' " + DownloadArg;
                    cURL.StartInfo.UseShellExecute = false;
                    cURL.StartInfo.CreateNoWindow = true;
                    cURL.Start();

                    while (!cURL.HasExited)
                    {
                        if (backgroundWorker1.CancellationPending)
                        {
                            cURL.Kill();
                            e.Cancel = true;
                            break;
                        }
                        else
                            Thread.Sleep(100);
                    }

                    if (e.Cancel) { return; };

                }

                backgroundWorker1.ReportProgress(0);

            }
        }

        void backgroundWorker1_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("취소되었습니다.", "에러");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Exception Thrown.", "에러");
            }
            else
            {
                MessageBox.Show(TitleName + "의 다운로드가 완료되었습니다.", "다운로드 완료");
                Process.Start("explorer.exe", SelectedDir);
            }

            this.Dispose();
        }

        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.label1.Text = i.ToString();
            progressBar1.PerformStep();
        }

    }
}

