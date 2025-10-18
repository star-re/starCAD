using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using starCAD;

namespace sCAD.SCurve
{
    public class hanjiekongfangfa
    {
        [CommandMethod("HanJieKong", CommandFlags.UsePickSet)]
        public void HanJieKong()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptEntityOptions peo = new PromptEntityOptions("\n选择曲线");

            PromptEntityResult psr = ed.GetEntity("\n选择曲线");
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
             XUANZE:   Entity entity = (Entity)trans.GetObject(psr.ObjectId, OpenMode.ForWrite);
                Type ty = entity.GetType();
                if (ty != typeof(Line) &&
                    ty != typeof(Arc) &&
                    ty != typeof(Circle) &&
                    ty != typeof(Polyline) &&
                    ty != typeof(Polyline2d) &&
                    ty != typeof(Polyline3d) &&
                    ty != typeof(Spline) &&
                    ty != typeof(PolyArc)&&
                     ty != typeof(Rectangle3d))
                {
                    psr = ed.GetEntity("\n选择曲线");
                    goto XUANZE;
                }
                else
                {
                    goto NEXT;
                }
            }

            NEXT: ViewTableRecord view = ed.GetCurrentView();
            Vector3d viewDir = view.ViewDirection;//当前视图方向
            Plane viewPlane = new Plane(Point3d.Origin, viewDir);
            if (psr.Status != PromptStatus.OK)
            {
                //psr = ed.GetSelection();
            }
            if (psr.Status == PromptStatus.Cancel) return;
            if (psr.Status == PromptStatus.None) return;
            if (psr.Status == PromptStatus.OK)
            {
                //SelectionSet sSet = psr.Value;
                // ObjectId[] dimObjectId = sSet.GetObjectIds();
                ObjectId[] dimObjectId = new ObjectId[1];
                dimObjectId[0] = psr.ObjectId;
                //PromptPointResult ppr = ed.GetPoint("定位点");
                DBObjectCollection dbc = new DBObjectCollection();
                List<Entity> entities = new List<Entity>();
                DoubleCollection doubleCollection = new DoubleCollection();
                //PromptPointResult ppr1 = ed.GetPoint("定位点");
                Polyline3d curveResult = new Polyline3d();
                Entity entity1 = null;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    for (int i = 0; i < dimObjectId.Length; i++)
                    {
                        Entity entity = (Entity)trans.GetObject(dimObjectId[i], OpenMode.ForWrite);
                        entity1 = entity;
                        Curve curve = (Curve)entity;
                        HanJieDraw hanJieDraw = new HanJieDraw(curve);
                        PromptResult result = Application.DocumentManager.MdiActiveDocument.Editor.Drag(hanJieDraw);
                        Point3d point3D = new Point3d();
                        if (result.Status == PromptStatus.OK)
                        {
                            point3D = hanJieDraw.baseDimPt;
                        }
                        Point3d p3 = curve.GetClosestPointTo(point3D, false);
                        Circle cc = new Circle(p3, viewDir, HanJieDraw.radi);
                        //Solid3d solid3D = CreateSphere(2);
                        //Move(solid3D, p3);
                        Point3dCollection points = new Point3dCollection();
                        entity.IntersectWith((Entity)cc, Intersect.OnBothOperands, viewPlane, points, IntPtr.Zero, IntPtr.Zero);
                        List<Point3d> zhongzhuan = new List<Point3d>();
                        foreach (Point3d item in points)
                        {
                            zhongzhuan.Add(item);
                        }
                        zhongzhuan = zhongzhuan.Select(m => curve.GetClosestPointTo(m, viewDir, false)).ToList();
                        double[] paramsArr = zhongzhuan.Select(k => curve.GetParameterAtPoint(k)).ToArray();
                        Array.Sort(paramsArr);
                        doubleCollection = new DoubleCollection(paramsArr);
                        dbc = curve.GetSplitCurves(doubleCollection);
                        //entities.Add(cc);
                        List<double> ds = new List<double>();
                        foreach (Curve item in dbc)
                        {

                            ds.Add(point3D.DistanceTo(item.GetPointAtDist(item.GetDistanceAtParameter(item.EndParam) / 2)));
                        }
                        dbc.RemoveAt(ds.IndexOf(ds.Min()));

                        zhongzhuan = zhongzhuan.Select(m => cc.GetClosestPointTo(m, viewDir, false)).ToList();
                        paramsArr = zhongzhuan.Select(k => cc.GetParameterAtPoint(k)).ToArray();
                        Array.Sort(paramsArr);
                        doubleCollection = new DoubleCollection(paramsArr);
                        DBObjectCollection doc = cc.GetSplitCurves(doubleCollection);

                        Curve cc1 = doc[0] as Curve;
                        Point3d closestp31 = cc1.GetClosestPointTo(point3D, false);
                        Curve cc2 = doc[1] as Curve;
                        Point3d closestp32 = cc2.GetClosestPointTo(point3D, false);

                        int Insertindex = 0;
                        if (curve.Closed) Insertindex = 0;
                        else Insertindex = 1;


                        if (closestp32.DistanceTo(point3D) > closestp31.DistanceTo(point3D))
                        {
                            dbc.Insert(Insertindex, doc[0]);
                        }
                        else
                        {
                            dbc.Insert(Insertindex, doc[1]);
                        }
                        entity.Erase();
                    }
                    trans.Commit();
                }
                foreach (Entity item in dbc)
                {
                    entities.Add(item);
                }
                if (entity1.GetType() == typeof(Spline))
                {
                    var cur1 = (Spline)entities.First();
                    entities.RemoveAt(0);
                    cur1.JoinEntities(entities.ToArray());
                    ReBlockTable.ReBlock(db, cur1);
                }
                else
                {
                    var cur1 = CurveToPolyline((Curve)entities.First());
                    entities.RemoveAt(0);
                    cur1.JoinEntities(entities.ToArray());
                    ReBlockTable.ReBlock(db, cur1);
                }
                //curveResult.JoinEntities(entities.ToArray());
            }
        }

        private Solid3d CreateSphere(double radius)
        {
            Solid3d solid3D = new Solid3d();
            solid3D.CreateSphere(radius);
            return solid3D;
        }

        private void Move(Entity entity, Point3d point)
        {
            Matrix3d matrix = Matrix3d.Displacement(point - new Point3d());
            entity.TransformBy(matrix);
        }

        public static Polyline CurveToPolyline(Curve crv)
        {
            if (crv is Polyline pl1) return pl1;
            else if (crv is Line line)
            {
                Polyline pl = new Polyline();
                pl.AddVertexAt(0, line.StartPoint.Convert2d(new Plane(Point3d.Origin, Vector3d.ZAxis)), 0, 0, 0);
                pl.AddVertexAt(1, line.EndPoint.Convert2d(new Plane(Point3d.Origin, Vector3d.ZAxis)), 0, 0, 0);
                return pl;
            }
            else if (crv is Arc arc)
            {
                var bug = Math.Tan(arc.TotalAngle / 4);
                Polyline pl = new Polyline();
                pl.AddVertexAt(0, arc.StartPoint.Convert2d(new Plane(Point3d.Origin, Vector3d.ZAxis)), bug, 0, 0);
                pl.AddVertexAt(1, arc.EndPoint.Convert2d(new Plane(Point3d.Origin, Vector3d.ZAxis)), 0, 0, 0);
                return pl;
            }
            else if (crv is Spline sl)
            {
                return sl.ToPolyline() as Polyline;
            }
            else if (crv is Ellipse el)
            {
                return el.Spline.ToPolyline() as Polyline;
            }
            else if (crv is Circle cir)
            {
                var p1 = cir.StartPoint;
                var p2 = cir.GetPointAtParameter(Math.PI);
                var bug = Math.Tan(Math.PI / 4);
                Polyline pl = new Polyline();
                pl.AddVertexAt(0, p1.Convert2d(new Plane(Point3d.Origin, Vector3d.ZAxis)), bug, 0, 0);
                pl.AddVertexAt(1, p2.Convert2d(new Plane(Point3d.Origin, Vector3d.ZAxis)), bug, 0, 0);
                return pl;
            }
            else throw new System.Exception();
        }
    }
}
