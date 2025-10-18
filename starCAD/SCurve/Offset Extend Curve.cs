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
using sCAD;
using Autodesk.AutoCAD.Geometry;
using starCAD;

namespace sCAD.SCurve
{
    public class Offset_Extend_Curve
    {
        [CommandMethod("p33", CommandFlags.UsePickSet)]


        public void p33()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            if (promptDoubleOptions == null)
            {
                promptDoubleOptions = new PromptDoubleOptions("请输入偏移距离");
            }
            if (promptDoubleOptions1 == null)
            {
                promptDoubleOptions1 = new PromptDoubleOptions("请输入缩短距离");
            }
            PromptDoubleResult promptDoubleResult = ed.GetDouble(promptDoubleOptions);
            if (promptDoubleResult.Status == PromptStatus.Cancel) return;
            if (promptDoubleResult.Status == PromptStatus.None) return;
            if (promptDoubleResult.Status != PromptStatus.OK) return;
            PromptDoubleResult promptDoubleResult1 = ed.GetDouble(promptDoubleOptions1);
            if (promptDoubleResult1.Status == PromptStatus.Cancel) return;
            if (promptDoubleResult1.Status == PromptStatus.None) return;
            if (promptDoubleResult1.Status != PromptStatus.OK) return;
            if (promptDoubleResult.Value != null)
            {
                promptDoubleOptions.DefaultValue = promptDoubleResult.Value;
                OffsetDistance = promptDoubleResult.Value;
            }
            if (promptDoubleResult1.Value != null)
            {
                promptDoubleOptions1.DefaultValue = promptDoubleResult1.Value;
                Domain = promptDoubleResult1.Value;
            }

        aaa: while (true)
            {
                PromptEntityResult psr = ed.GetEntity("\n选择偏移对象");
                if (psr.Status == PromptStatus.Cancel) return;
                if (psr.Status == PromptStatus.None) return;
                if (psr.Status != PromptStatus.OK) return;
                List<Entity> entities = new List<Entity>();
                ObjectId dimObjectId = psr.ObjectId;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    Entity TEntity1 = (Entity)dimObjectId.GetObject(OpenMode.ForWrite);
                    Curve offcurvebase = null;
                    if (TEntity1.GetType() == typeof(Line) || TEntity1.GetType() == typeof(Polyline) || TEntity1.GetType() == typeof(Arc))
                    {
                        offcurvebase = TEntity1 as Curve;
                    }
                    offcurvebase = offcurvebase.Clone() as Curve;
                    string pianyitishi = "";
                    if (OffsetDanbian)
                    {
                        pianyitishi = "\n偏移方向_单边缩减[单边(D)/双边(S)]";
                    }
                    else
                    {
                        pianyitishi = " \n偏移方向_双边缩减[单边(D)/ 双边(S)]";
                    }
                    PromptPointOptions ppo = new PromptPointOptions(pianyitishi);
                    ppo.Keywords.Add("D");
                    ppo.Keywords.Add("S");
                    ppo.AppendKeywordsToMessage = false;
                    PromptPointResult ppr = ed.GetPoint(ppo);
                    if (ppr.Status == PromptStatus.Keyword)
                    {
                        switch (ppr.StringResult)
                        {
                            case "D":
                                OffsetDanbian = true;
                                //ppo = new PromptPointOptions("\n偏移方向_单边缩减：[单边(D)/双边(S)]");
                                goto aaa;
                            case "S":
                                OffsetDanbian = false;
                                //ppo = new PromptPointOptions("\n偏移方向_双边缩减：[单边(D)/双边(S)]");
                                goto aaa;
                        }
                    }
                    //DBObjectCollection dBObjectCollectionpanduan1 = offcurvebase.GetOffsetCurves(OffsetDistance);
                    //DBObjectCollection dBObjectCollectionpanduan2 = offcurvebase.GetOffsetCurves(-OffsetDistance);
                    //DBObjectCollection dBObjectCollection = new DBObjectCollection();
                    //for (int ii = 0; ii < dBObjectCollectionpanduan1.Count; ii++)
                    //{
                    //    Curve panduanCurve1 = dBObjectCollectionpanduan1[ii] as Curve;
                    //    Curve panduanCurve2 = dBObjectCollectionpanduan2[ii] as Curve;
                    //    if (ppr.Value.DistanceTo(panduanCurve1.GetClosestPointTo(ppr.Value, true)) < ppr.Value.DistanceTo(panduanCurve2.GetClosestPointTo(ppr.Value, true)))
                    //    {
                    //        dBObjectCollection = dBObjectCollectionpanduan1;
                    //    }
                    //    else
                    //    {
                    //        dBObjectCollection = dBObjectCollectionpanduan2;
                    //    }
                    //}
                    List<Point3d> points = new List<Point3d>();

                    Curve curve = offcurvebase;
                    Point3d point3D = Point3d.Origin;
                    if (OffsetDanbian)
                    {
                        if (ppr.Value.DistanceTo(offcurvebase.StartPoint) < ppr.Value.DistanceTo(offcurvebase.EndPoint))
                        {
                            points.Add(curve.StartPoint);
                        }
                        else
                        {
                            points.Add(curve.EndPoint);
                        }
                    }
                    else
                    {
                        points.Add(curve.StartPoint);
                        points.Add(curve.EndPoint);
                    }
                    for (int i = 0; i < points.Count; i++)
                    {
                        point3D = points[i];
                        Circle IntersectionCircle = new Circle(point3D, Vector3d.ZAxis, Domain);
                        Point3dCollection IntersectionPoint = new Point3dCollection();
                        curve.IntersectWith(IntersectionCircle, Intersect.ExtendArgument, IntersectionPoint, new IntPtr(), new IntPtr());
                        //double IntersectParam = item.GetParameterAtPoint(IntersectionPoint[0]);
                        //IntersectionPoint[0] = item.GetPointAtDist(IntersectParam);
                        DBObjectCollection dBObjectCollection1 = curve.GetSplitCurves(IntersectionPoint);
                        List<Curve> xiaoxianduan = new List<Curve>();
                        Curve OffsetCurve = null;

                        Interval interval = new Interval(Domain - 0.01, Domain + 0.01, 0.01);
                        foreach (Curve item1 in dBObjectCollection1)
                        {
                            if (!interval.Contains(item1.StartPoint.DistanceTo(item1.EndPoint)))
                            {
                                curve = item1;
                                if (i + 1 == points.Count)
                                {
                                    DBObjectCollection dBObjectCollectionpanduan1 = curve.GetOffsetCurves(OffsetDistance);
                                    DBObjectCollection dBObjectCollectionpanduan2 = curve.GetOffsetCurves(-OffsetDistance);
                                    DBObjectCollection dBObjectCollection = new DBObjectCollection();
                                    for (int ii = 0; ii < dBObjectCollectionpanduan1.Count; ii++)
                                    {
                                        Curve panduanCurve1 = dBObjectCollectionpanduan1[ii] as Curve;
                                        Curve panduanCurve2 = dBObjectCollectionpanduan2[ii] as Curve;
                                        if (ppr.Value.DistanceTo(panduanCurve1.GetClosestPointTo(ppr.Value, true)) < ppr.Value.DistanceTo(panduanCurve2.GetClosestPointTo(ppr.Value, true)))
                                        {
                                            dBObjectCollection = dBObjectCollectionpanduan1;
                                        }
                                        else
                                        {
                                            dBObjectCollection = dBObjectCollectionpanduan2;
                                        }
                                    }
                                    foreach (Curve item in dBObjectCollection)
                                    {
                                        Point3d pt1 = offcurvebase.GetClosestPointTo(item.StartPoint, true);
                                        Point3d pt2 = offcurvebase.GetClosestPointTo(item.EndPoint, true);
                                        Line line1 = new Line(pt1, item.StartPoint);
                                        entities.Add(line1);
                                        //curve.Layer = "0";
                                        //curve.ColorIndex = 0;
                                        //entities.Add(curve);
                                        entities.Add(new Line(pt2, item.EndPoint));
                                        entities.Add(item);
                                    }
                                }
                            }
                            else
                            {
                                xiaoxianduan.Add(item1);
                            }
                        }
                        entities.AddRange(xiaoxianduan);
                    }
                }

                ReBlockTable.ReBlock(db, entities.ToArray());
            }
        }
        public static double Domain = 5;
        public static double OffsetDistance = 20;
        public static bool OffsetDanbian = true;
        public static PromptDoubleOptions promptDoubleOptions = new PromptDoubleOptions("请输入偏移距离");
        public static PromptDoubleOptions promptDoubleOptions1 = new PromptDoubleOptions("请输入缩短距离");
    }
}
