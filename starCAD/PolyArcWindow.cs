using System;
using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.AutoCAD.ApplicationServices;
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

namespace sCAD
{
    public partial class PolyArcWindow : Form
    {
        public PolyArcWindow()
        {
            InitializeComponent();
        }

        private void SureButton_Click(object sender, EventArgs e)
        {
            DrawCircle();
        }

        public void DrawCircle()
        {
            string ss = SizeBox1.Text;
            string[] strings = ss.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<double> SizeList = new List<double>();
            for (int i = 0; i < strings.Length; i++)
            {
                SizeList.Add(CalText(strings[i]));
            }

            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            List<Circle> circles = new List<Circle>();
            for (int i = 0; i < SizeList.Count; i++)
            {
                ShowSizeBox1.Text = SizeList[i].ToString();
                PromptPointResult ppr = ed.GetPoint("\r\n选择点");
                if (ppr.Status == PromptStatus.Cancel) return;
                if (ppr.Status == PromptStatus.None) return;
                if (ppr.Status != PromptStatus.OK) return;
                Circle cc = new Circle(ppr.Value, Vector3d.ZAxis, SizeList[i]);
                DBPoint point3D = new DBPoint(ppr.Value);
                ReBlockTable.ReBlock(cc);
                ReBlockTable.ReBlock(point3D);
            }
        }

        public double CalText(string ss)
        {
            double DouResult = double.NaN;
            SData.DataTable dataTable = new SData.DataTable();
            // SData.DataTable dataTable = new SData.DataTable();
            object result = dataTable.Compute(ss, null);
            DouResult = Convert.ToDouble(result);
            return DouResult;
        }
    }
}
