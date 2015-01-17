using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace ToonCrapper
{
    public partial class Toon_TitleSelect : Form
    {
        public Toon_TitleSelect()
        {
            InitializeComponent();
        }

        public delegate void ToonSelector(String titleno, String title, bool Restricted);
        public event ToonSelector FormSendEvent;

        public CookieContainer Cookie;

        bool isAgeRestrict = false;

        private void button1_Click(object sender, EventArgs e)
        {
            int indexnum;
            indexnum = listView1.FocusedItem.Index;
            String titleno = listView1.FocusedItem.Text;
            String title = listView1.FocusedItem.SubItems[1].Text;
            String restricted = listView1.FocusedItem.SubItems[3].Text;

            if (restricted == "Y")
            {
                isAgeRestrict = true;
            }

            FormSendEvent(titleno, title, isAgeRestrict);
        }

    }
}
