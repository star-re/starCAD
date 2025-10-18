using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Windows.Forms;
using Autodesk.AutoCAD.Geometry;

namespace starCAD
{
    public class dimhuchang2
    {
        public class Zhong
        {
            public static bool zhongwen = true;
            public static string FF = "|";
            public static string HH = FF;
        }
        [CommandMethod("DimHu")]

        public void DimHu()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            string promptstr = "\n [显示文字(H)/分隔符(F)]:";
            string fenge = Zhong.HH;
            bool isC = true;
            while (isC)
            {
                PromptResult pr = ed.GetKeywords(ed.GetSelectOp2(promptstr, new string[] { "H", "F" }));
                if (pr.Status == PromptStatus.Cancel) { isC = false; }
                if (pr.Status == PromptStatus.None) { isC = false; }
                switch (pr.StringResult)
                {
                    case "H":
                        string shifou = "\n [是(Y)/否(N)]";
                        PromptResult shifoupr = ed.GetKeywords(ed.GetSelectOp2(shifou, new string[] { "Y", "N" }));
                        switch (shifoupr.StringResult)
                        {
                            case "Y":
                                Zhong.zhongwen = true;
                                break;
                            case "N":
                                Zhong.zhongwen = false;
                                break;
                        }
                        fenge = Zhong.HH;
                        isC = false;
                        break;
                    case "S":
                        fenge = Zi.SS;
                        isC = false;
                        break;
                    case "C":
                        //PromptResult Countstr = ed.GetString("\n 请输入需求的一组数量：");
                        //ArrayCount = int.Parse(Countstr.StringResult);
                        //Zi.CC = ArrayCount.ToString();
                        break;
                    case "F":
                        PromptResult Fenge = ed.GetString("\n 请输入分隔符：");
                        fenge = Fenge.StringResult;
                        Zhong.FF = fenge;
                        Zhong.HH = Zhong.FF;
                        isC = false;
                        break;
                }
                PromptSelectionResult psr = ed.GetSelection();
                if (psr.Status == PromptStatus.Cancel) return;
                if (psr.Status == PromptStatus.None) return;

                if (psr.Status == PromptStatus.OK)
                {
                    SelectionSet sSet = psr.Value;
                    ObjectId[] dimObjectId = sSet.GetObjectIds();

                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        string format = string.Empty;
                        for (int i = 0; i < sSet.Count; i++)
                        {
                            Entity dimEntity1 = (Entity)dimObjectId[i].GetObject(OpenMode.ForWrite);
                            Arc x = (Arc)dimEntity1;
                            double length = x.Length;
                            double ar = x.Radius;
                            Point3d start = x.StartPoint;
                            Point3d end = x.EndPoint;
                            List<Point3d> gg = new List<Point3d>();
                            gg.Add(start);
                            gg.Add(end);
                            double xianchang = start.DistanceTo(x.EndPoint);
                            double xiangao = x.GetPointAtDist(length / 2).DistanceTo(Pointaverage(gg));
                            if (Zhong.zhongwen)
                            {
                                if (format == string.Empty)
                                {
                                    format = string.Format("弧长{0},半径{1},弦长{2},弦高{3}", Math.Round(length, 1), Math.Round(ar, 1), Math.Round(xianchang, 1), Math.Round(xiangao, 1));
                                }
                                else
                                {
                                    format = string.Format("{0}\r\n弧长{1},半径{2},弦长{3},弦高{4}", format, Math.Round(length, 1), Math.Round(ar, 1), Math.Round(xianchang, 1), Math.Round(xiangao, 1));
                                }
                            }
                            else
                            {
                                if (format == string.Empty)
                                {
                                    format = string.Format("{0}{1}{2}{3}{4}{5}{6}", Math.Round(length, 1),fenge, Math.Round(ar, 1),fenge, Math.Round(xianchang, 1), fenge,Math.Round(xiangao, 1));
                                }
                                else
                                {
                                    format = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", format, "\r\n", Math.Round(length, 1), fenge, Math.Round(ar, 1), fenge, Math.Round(xianchang, 1), fenge, Math.Round(xiangao, 1));
                                }
                            }

                        }
                        Clipboard.SetDataObject(format);
                    }
                }
            }
        }

        public Point3d Pointaverage(List<Point3d> points)
        {
            List<double> xa = new List<double>();
            List<double> ya = new List<double>();
            List<double> za = new List<double>();
            for (int i = 0; i < points.Count; i++)
            {
                xa.Add(points[i].X);
                ya.Add(points[i].Y);
                za.Add(points[i].Z);
            }
            double xx = xa.Average();
            double yy = ya.Average();
            double zz = za.Average();
            return new Point3d(xx, yy, zz);
        }
    }
}
