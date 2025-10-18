using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using starCAD;

namespace sCAD
{
    public class Der
    {
        public static string SS = "\r\n";
        public static int CC = 100;
        public static string FF = "|";
        public static string HH = FF;
    }

    public class RectangeWei
    {
        [CommandMethod("DeconstuctRectangle", CommandFlags.UsePickSet)]

        public void DeconstuctRectangle()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            bool flag0 = true;
            bool flag1 = true;
            int TextHeight = Der.CC;
            DBText dBText = new DBText();
            //dBText.Layer = "矩形框文字";
            List<Entity> TextEntitys = new List<Entity>();
            PromptSelectionResult psr = ed.SelectImplied();
            PromptSelectionOptions promptSelectionOptions = new PromptSelectionOptions();
            promptSelectionOptions.MessageForAdding = "请拾取矩形框：";
            string promptstr = "请拾取矩形框 [字高(H)]";

            List<ObjectId> RectangleID = new List<ObjectId>();
            //if (psr.Status != PromptStatus.OK)
            //{
            //PromptEntityResult promptEntityOptions = ed.GetEntity(ed.GetSelectEntityOp2(promptstr, new string[] { "H", "S", "C", "F" }));
            while (flag1)
            {
                PromptEntityResult per = ed.GetEntity(ed.GetSelectEntityOp2(promptstr, new string[] { "H" }));
                string ss = per.StringResult;
                switch (per.StringResult)
                {
                    case "H":
                        PromptResult Countstr = ed.GetString("\n 字高：");
                        TextHeight = int.Parse(Countstr.StringResult);
                        Der.CC = TextHeight;
                        break;
                }
                if (per.Status == PromptStatus.OK)
                {
                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                       Entity entity =(Entity)per.ObjectId.GetObject(OpenMode.ForWrite);
                        entity.Highlight();
                        trans.Commit();
                    }
                    RectangleID.Add(per.ObjectId);
                }
                if (per.Status == PromptStatus.Cancel) flag1 = false;
                if (per.Status == PromptStatus.None) flag1 = false;
                //if (per.Status != PromptStatus.OK) flag1 = false;
            }

            // psr = ed.GetSelection(promptSelectionOptions);
            //}
            //if (psr.Status == PromptStatus.Cancel) return;
            //if (psr.Status == PromptStatus.None) return;
            //if (psr.Status != PromptStatus.OK) return;
            //SelectionSet sSet = psr.Value;
            ObjectId[] dimObjectId = RectangleID.ToArray();
            //Entity[] entities = new Entity[dimObjectId.Length];
            List<Polyline> pllist = new List<Polyline>();
            List<Point3d> MidPoints = new List<Point3d>();
            int PolylineIndexX1 = 0;
            int PolylineIndexX2 = 0;
            int PolylineIndexY1 = 0;
            int PolylineIndexY2 = 0;
            Point3d MidX1 = Point3d.Origin;
            Point3d MidX2 = Point3d.Origin;
            Point3d MidY1 = Point3d.Origin;
            Point3d MidY2 = Point3d.Origin;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                for (int k = 0; k < dimObjectId.Length; k++)
                {
                    pllist.Add((Polyline)dimObjectId[k].GetObject(OpenMode.ForWrite));
                }
            }
            if (flag0)
            {
                for (int k = 0; k < pllist.Count; k++)
                {
                    PolylineIndexX1 = 0;
                     PolylineIndexX2 = 0;
                     PolylineIndexY1 = 0;
                     PolylineIndexY2 = 0;
                     MidX1 = Point3d.Origin;
                     MidX2 = Point3d.Origin;
                     MidY1 = Point3d.Origin;
                     MidY2 = Point3d.Origin;
                    MidPoints.Clear();
                    Polyline polyline = pllist[k];
                    MidX1 = polyline.GetLineSegmentAt(0).MidPoint;
                    MidX2 = polyline.GetLineSegmentAt(0).MidPoint;
                    MidY1 = polyline.GetLineSegmentAt(0).MidPoint;
                    MidY2 = polyline.GetLineSegmentAt(0).MidPoint;
                    for (int i = 0; i < polyline.NumberOfVertices; i++)
                    {
                        MidPoints.Add(polyline.GetLineSegmentAt(i).MidPoint);
                        if (polyline.GetLineSegmentAt(i).MidPoint.X < MidX1.X)
                        {
                            MidX1 = polyline.GetLineSegmentAt(i).MidPoint;
                            PolylineIndexX1 = i;
                        }
                        if (polyline.GetLineSegmentAt(i).MidPoint.X > MidX2.X)
                        {
                            MidX2 = polyline.GetLineSegmentAt(i).MidPoint;
                            PolylineIndexX2 = i;
                        }
                        if (polyline.GetLineSegmentAt(i).MidPoint.Y > MidY1.Y)
                        {
                            MidY1 = polyline.GetLineSegmentAt(i).MidPoint;
                            PolylineIndexY1 = i;
                        }
                        if (polyline.GetLineSegmentAt(i).MidPoint.Y < MidY2.Y)
                        {
                            MidY2 = polyline.GetLineSegmentAt(i).MidPoint;
                            PolylineIndexY2 = i;
                        }
                    }
                    Point3d MidPoint3d = starMathdy.Pointaverage(MidPoints);
                    dBText = new DBText();
                    dBText.Position = MidPoint3d;
                    dBText.TextString = string.Concat(Math.Round(polyline.GetLineSegmentAt(PolylineIndexY1).Length, 0), "*", Math.Round(polyline.GetLineSegmentAt(PolylineIndexX1).Length, 0));
                    dBText.HorizontalMode = TextHorizontalMode.TextMid;
                    dBText.AlignmentPoint = dBText.Position;
                    dBText.Height = TextHeight;
                    ReBlockTable.ReBlock(db, dBText);
                }
            }
            else
            {
                MidPoints.Clear();
                for (int k = 0; k < pllist.Count; k++)
                {
                    Polyline polyline = pllist[k];
                    MidX1 = polyline.GetLineSegmentAt(0).MidPoint;
                    MidX2 = polyline.GetLineSegmentAt(0).MidPoint;
                    MidY1 = polyline.GetLineSegmentAt(0).MidPoint;
                    MidY2 = polyline.GetLineSegmentAt(0).MidPoint;
                    for (int i = 0; i < polyline.NumberOfVertices; i++)
                    {
                        MidPoints.Add(polyline.GetLineSegmentAt(i).MidPoint);
                        if (polyline.GetLineSegmentAt(i).MidPoint.X < MidX1.X)
                        {
                            MidX1 = polyline.GetLineSegmentAt(i).MidPoint;
                            PolylineIndexX1 = i;
                        }
                        if (polyline.GetLineSegmentAt(i).MidPoint.X > MidX2.X)
                        {
                            MidX2 = polyline.GetLineSegmentAt(i).MidPoint;
                            PolylineIndexX2 = i;
                        }
                        if (polyline.GetLineSegmentAt(i).MidPoint.Y > MidY1.Y)
                        {
                            MidY1 = polyline.GetLineSegmentAt(i).MidPoint;
                            PolylineIndexY1 = i;
                        }
                        if (polyline.GetLineSegmentAt(i).MidPoint.Y < MidY2.Y)
                        {
                            MidY2 = polyline.GetLineSegmentAt(i).MidPoint;
                            PolylineIndexY2 = i;
                        }
                    }
                    Point3d MidPoint3d = starMathdy.Pointaverage(MidPoints);
                    dBText = new DBText();
                    dBText.Position = MidPoint3d;
                    dBText.TextString = string.Concat(Math.Round(polyline.GetLineSegmentAt(PolylineIndexY1).Length, 0), "*", Math.Round(polyline.GetLineSegmentAt(PolylineIndexX1).Length, 0));
                    dBText.HorizontalMode = TextHorizontalMode.TextMid;
                    dBText.Height = TextHeight;
                    ReBlockTable.ReBlock(db, dBText);

                }
            }
        }
    }
}
