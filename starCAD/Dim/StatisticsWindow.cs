using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sCAD.Dim
{
    public partial class StatisticsWindow : Form
    {
        public StatisticsWindow(IEnumerable<string> Showtexts, IEnumerable<int> ShowtextCount)
        {
            showtexts.Clear();
            showtextcount.Clear();
            InitializeComponent();
            showtexts = Showtexts.ToList();
            showtextcount = ShowtextCount.ToList();
            SendDataToListView();
        }

        public static List<string> showtexts = new List<string>();
        public static List<int> showtextcount = new List<int>();

        public void SendDataToListView()
        {
            for (int i = 0; i < showtexts.Count; i++)
            {
                ListViewItem listViewItem = new ListViewItem(new string[] { showtexts[i], showtextcount[i].ToString() });
                // listViewItem.Checked = true;
                listView1.Items.Add(listViewItem);
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //private void trackBar1_Scroll(object sender, EventArgs e)
        //{
          
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            StatisticsWindow2 statisticsWindow2 = new StatisticsWindow2(showtexts, showtextcount);
            statisticsWindow2.TopMost = true;
            statisticsWindow2.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> stsCount = new List<string>();
            for (int i = 0; i < showtexts.Count; i++)
            {
                stsCount.Add(string.Format("{0}|{1}", showtexts[i], showtextcount[i]));
            }
            Clipboard.SetDataObject(string.Join("\r\n", stsCount));
        }
    }
}
