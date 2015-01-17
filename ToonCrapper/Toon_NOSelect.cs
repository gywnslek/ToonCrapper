using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ToonCrapper
{
    public partial class Toon_NOSelect : Form
    {
        public string titleid;
        public string titlename;
        public string lastID;

        public delegate void NOSelector(String titleid, String titlename, String startID, String lastToonID);
        public event NOSelector FormSendEvent;

        public Toon_NOSelect()
        {
            InitializeComponent();
            textBox1.KeyPress += textBox1_KeyPress;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int Current = int.Parse(textBox1.Text);
            int Max = int.Parse(lastID);

            if (Current == 0)
            {
                MessageBox.Show("올바르지 않은 값입니다.", "에러");
                return;
            }else if (Current >= Max)
            {
                MessageBox.Show("올바르지 않은 값입니다.", "에러");
                return;
            };

            FormSendEvent(titleid, titlename, textBox1.Text,lastID);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

    }
}
