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
    public partial class StatisticsWindow2 : Form
    {
        public StatisticsWindow2(List<string> Showtexts, List<int> ShowtextCount)
        {
            showtexts.Clear();
            showtextcount.Clear();
            InitializeComponent();
            showtexts = Showtexts.ToList();
            showtextcount = ShowtextCount.ToList();
        }

        public static List<string> showtexts = new List<string>();
        public static List<int> showtextcount = new List<int>();
        public static List<string> copytextcount = new List<string>();
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            showsplitCount.Text = trackBar1.Value.ToString();
            string ss = richTextBox1.Text;
            string[] Sendtext = ss.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            SendDataToListView(Sendtext);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            string ss = richTextBox1.Text;
            string[] Sendtext = ss.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            SendDataToListView(Sendtext);
        }

        public void SendDataToListView(string[] sendtext)
        {
            listView1.Items.Clear();
            copytextcount.Clear();
            for (int i = 0; i < sendtext.Length; i++)
            {
                bool sw = true;
                if (trackBar1.Value >= 0)
                {
                    sw = true;
                }
                else
                {
                    sw = false;
                }
                string spT = TextSplits(sendtext[i], trackBar1.Value, sw);
                int ti = showtexts.IndexOf(spT);
                if (ti != -1)
                {
                    ListViewItem listViewItem = new ListViewItem(new string[] { spT, showtextcount[ti].ToString() });
                    // listViewItem.Checked = true;
                    listView1.Items.Add(listViewItem);
                    copytextcount.Add(showtextcount[ti].ToString());
                }
                else
                {
                    ListViewItem listViewItem = new ListViewItem(new string[] { spT, "0" });
                    // listViewItem.Checked = true;
                    listView1.Items.Add(listViewItem);
                    copytextcount.Add("0");
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(string.Join("\r\n", copytextcount));
        }



        //               public static string(string s,int TextCount ,bool Sw)
        //            {
        //        //    string s = string.Empty;
        //        //int TextCount = 0;
        //        //bool Sw = true;



        ////            TextSplits(s, Math.Abs(TextCount), Sw);
        ////DA.SetData(0, Result1);
        ////            DA.SetData(1, Result2);
        ////        }

        public static string Result1 { get; set; }
        public static string Result2 { get; set; }

        public static string TextSplits(string text, int Count, bool flag)
        {

            Count = Math.Abs(Count);
            text = text.Replace("\t", "");
            string result = text;
            if (flag)
            {
                if (text.Length < Count)
                {
                    Result1 = text;
                    Result2 = string.Empty;
                }
                else
                {
                    Result1 = result.Remove(Count, text.Length - Count);
                    Result2 = result.Remove(0, Count);
                }
            }
            else
            {
                if (text.Length < Count)
                {
                    Result2 = string.Empty;
                    Result1 = text;
                }
                else
                {
                    Result2 = result.Remove(0, text.Length - Count);
                    Result1 = result.Remove(text.Length - Count, Count);
                }
            }
            if (flag)
            {
                return Result2;
            }
            else
            {
                return Result1;
            }
        }
    }
}
