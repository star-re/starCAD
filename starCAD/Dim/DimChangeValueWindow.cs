using System;
using System.Collections.Generic;
using System.ComponentModel;
using AAPP = Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using starCAD;
using System.Windows.Forms;
using SData = System.Data;

namespace sCAD.Dim
{
    public partial class DimChangeValueWindow : Form
    {
        public DimChangeValueWindow(List<string> Showtexts, Entity[] objectIds)
        {
            InitializeComponent();
            RangeValue.Clear();
            showtexts = Showtexts;
            // IDValues = objectIds;
            SendDataToListView();
        }

        public void SendDataToListView()
        {
            if (listView1.Items.Count != showtexts.Count)
            {
                for (int i = 0; i < showtexts.Count; i++)
                {
                    ListViewItem listViewItem = new ListViewItem(new string[] { showtexts[i] });
                    // listViewItem.Checked = true;
                    listView1.Items.Add(listViewItem);
                }
            }
            else
            {
                for (int i = 0; i < showtexts.Count; i++)
                {
                    if (RangeValue.Count > i)
                    {
                        listView1.Items[i] = new ListViewItem(new string[] { showtexts[i], RangeValue[i] });
                        // listViewItem.Checked = true;
                        //listView1.Items.Add(listViewItem);
                    }
                    else
                    {
                        listView1.Items[i] = new ListViewItem(new string[] { showtexts[i] });
                        // listViewItem.Checked = true;
                        // listView1.Items.Add(listViewItem);
                    }
                }
            }
        }

        public void SendDataToListView(string Sendtext)
        {
            RangeValue.Add(Sendtext);
            for (int i = 0; i < showtexts.Count; i++)
            {
                if (RangeValue.Count > i)
                {
                    listView1.Items[i] = new ListViewItem(new string[] { showtexts[i], RangeValue[i] });
                    // listViewItem.Checked = true;
                    //listView1.Items.Add(listViewItem);
                }
                else
                {
                    listView1.Items[i] = new ListViewItem(new string[] { showtexts[i] });
                    // listViewItem.Checked = true;
                    // listView1.Items.Add(listViewItem);
                }
            }
        }

        public void SendDatasToListView(string Sendtexts)
        {
            string[] Sendtext = Sendtexts.Split(new string[] { "\n" }, StringSplitOptions.None);
            RangeValue.AddRange(Sendtext);
            for (int i = 0; i < showtexts.Count; i++)
            {
                if (RangeValue.Count > i)
                {
                    listView1.Items[i] = new ListViewItem(new string[] { showtexts[i], RangeValue[i] });
                    // listViewItem.Checked = true;
                    //listView1.Items.Add(listViewItem);
                }
                else
                {
                    listView1.Items[i] = new ListViewItem(new string[] { showtexts[i] });
                    // listViewItem.Checked = true;
                    // listView1.Items.Add(listViewItem);
                }
            }
        }

        public static List<string> showtexts = new List<string>();
        public static List<string> RangeValue = new List<string>();
        public static Entity[] IDValues = null;

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            RangeValue.RemoveAt(RangeValue.Count - 1);
            SendDataToListView();
        }

        private void buttonSure_Click_1(object sender, EventArgs e)
        {
            sCAD.Dim.DimChangeValue dimChange = new DimChangeValue();
            dimChange.ChangeValue();
            string copy = string.Join("|", RangeValue.ToArray());
            Clipboard.SetText(copy);
            //Database db = HostApplicationServices.WorkingDatabase;
            //Editor ed = AAPP.Application.DocumentManager.MdiActiveDocument.Editor;
            //using (Transaction trans = db.TransactionManager.TopTransaction)
            //{
            //    //Entity[] entities = new Entity[dimObjectId.Length];
            //    for (int i = 0; i < IDValues.Length; i++)
            //    {
            //        Dimension dimension1 = IDValues[i].Id.GetObject(OpenMode.ForWrite) as Dimension;
            //        if (RangeValue.Count > i)
            //        {
            //            dimension1.DimensionText = RangeValue[i];
            //        }
            //        else
            //        {
            //            dimension1.DimensionText = showtexts[i];
            //        }
            //    }
            //    trans.Commit();
            //}
        }

        private void buttonL_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonL.Text);
        }

        private void buttonL1_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonL1.Text);
        }

        private void buttonL2_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonL2.Text);
        }

        private void buttonL3_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonL3.Text);
        }

        private void buttonL4_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonL4.Text);
        }

        private void buttonL5_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonL5.Text);
        }

        private void buttonL6_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonL6.Text);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void buttonW_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonW.Text);
        }

        private void buttonW1_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonW1.Text);
        }

        private void buttonW2_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonW2.Text);

        }

        private void buttonW3_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonW3.Text);

        }

        private void buttonW4_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonW4.Text);

        }

        private void buttonW5_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonW5.Text);

        }

        private void buttonW6_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonW6.Text);

        }

        private void buttonS1_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonS1.Text);

        }

        private void buttonS2_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonS2.Text);

        }

        private void buttonS3_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonS3.Text);

        }

        private void buttonS4_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonS4.Text);

        }

        private void buttonS5_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonS5.Text);

        }

        private void buttonS6_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonS6.Text);

        }

        private void buttonX_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonX.Text);

        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonX1.Text);

        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonX2.Text);

        }

        private void buttonY_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonY.Text);

        }

        private void buttonY1_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonY1.Text);

        }

        private void buttonY2_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonY2.Text);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!HutoggleButton1.Checked)
            {
                SendDataToListView("弧长A1,半径A2,弦长A3,弦高A4");
            }
            else
            {
                SendDataToListView("弧长A1,半径A2");
            }
        }

        private void buttonHX2_Click(object sender, EventArgs e)
        {
            if (!HutoggleButton1.Checked)
            {
                SendDataToListView("弧长B1,半径B2,弦长B3,弦高B4");
            }
            else
            {
                SendDataToListView("弧长B1,半径B2");
            }
        }

        private void buttonHX3_Click(object sender, EventArgs e)
        {
            if (!HutoggleButton1.Checked)
            {
                SendDataToListView("弧长C1,半径C2,弦长C3,弦高C4");
            }
            else
            {
                SendDataToListView("弧长C1,半径C2");
            }
        }

        private void buttonHX4_Click(object sender, EventArgs e)
        {
            if (!HutoggleButton1.Checked)
            {
                SendDataToListView("弧长D1,半径D2,弦长D3,弦高D4");
            }
            else
            {
                SendDataToListView("弧长D1,半径D2");
            }
        }

        private void buttonHX5_Click(object sender, EventArgs e)
        {
            if (!HutoggleButton1.Checked)
            {
                SendDataToListView("弧长E1,半径E2,弦长E3,弦高E4");
            }
            else
            {
                SendDataToListView("弧长E1,半径E2");
            }
        }

        private void buttonHX6_Click(object sender, EventArgs e)
        {
            if (!HutoggleButton1.Checked)
            {
                SendDataToListView("弧长F1,半径F2,弦长F3,弦高F4");
            }
            else
            {
                SendDataToListView("弧长F1,半径F2");
            }
        }

        private void buttonJ1_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonJ1.Text);
        }

        private void buttonJ2_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonJ2.Text);
        }

        private void buttonJ3_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonJ3.Text);
        }

        private void buttonJ4_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonJ4.Text);

        }

        private void buttonJ5_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonJ5.Text);

        }

        private void buttonJ6_Click(object sender, EventArgs e)
        {
            SendDataToListView(buttonJ6.Text);

        }

        private void returnText_Click(object sender, EventArgs e)
        {
            string ss = richTextBox1.Text;
            SendDatasToListView(ss);
        }
    }
}
