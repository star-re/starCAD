using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using AAG = Autodesk.AutoCAD.GraphicsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace sCAD
{
    public class JigJig : DrawJig
    {
        private Point3d _point;
        private List<Entity> _ents;
        private List<Entity> _ents1;
        private Action<Point3d, List<Entity>, List<Entity>> _func;
        private string _message;
        private JigPromptPointOptions _jppo;
        private bool _add;
        private bool _stop;

        public JigJig(Point3d point, Action<Point3d, List<Entity>, List<Entity>> func,
            JigPromptPointOptions jppo = null, string message = "")
        {
           _point = point;
            _ents = new List<Entity>();
            _ents1 = new List<Entity>();
            _func = func;
            _message = message;
            if (jppo == null)
            {
                _jppo = new JigPromptPointOptions(message);
                _add = true;
            }
            else
            {
                _jppo = jppo;
                _add = false;
            }
            if (_add && Application.DocumentManager.MdiActiveDocument.Editor.Drag(this).Status == PromptStatus.OK)
            {
                starCAD.ReBlockTable.ReBlock(_ents);
                _stop = false;
            }
            else
            {
                _stop = true;
            }
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            PromptPointResult psr = prompts.AcquirePoint(_jppo);
            if (psr.Status == PromptStatus.OK)
            {
                Point3d poNow = psr.Value;
                if (poNow != _point)
                {
                    _ents.Clear();
                    _ents1.Clear();
                    _func(poNow, _ents, _ents1);
                    return SamplerStatus.OK;
                }
                else
                {
                    return SamplerStatus.NoChange;
                }
            }
            return SamplerStatus.Cancel;
        }

        protected override bool WorldDraw(AAG.WorldDraw draw)
        {
            _ents.ForEach(i => draw.Geometry.Draw(i));
            _ents1.ForEach(i => draw.Geometry.Draw(i));
            return true;
        }
    }

    public class OleRecDraw : DrawJig
    {
        public Point3d baseDimPt;
        List<Curve> showCrv = new List<Curve>();
        Rectangle3d Crvc;
        Point3d psr;
        public Matrix3d raMo = new Matrix3d();
        public Matrix3d raSC = new Matrix3d();
        public static double radi = 2;
        public OleRecDraw(Rectangle3d Wid, Point3d ps)
        {
            Crvc = Wid;
            psr = ps;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            JigPromptPointOptions jpo = new JigPromptPointOptions("\n第二个点");
            PromptPointResult ppr = prompts.AcquirePoint(jpo);
            if (ppr.Status == PromptStatus.OK)
            {
                Point3d pttemp = ppr.Value;
                if (pttemp != baseDimPt)
                {
                    showCrv.Clear();
                }
                if (pttemp != baseDimPt)
                {
                    Point3d p31 = psr;
                    Point3d p32 = ppr.Value;
                    double minX = Math.Min(p31.X, p32.X);
                    double maxX = Math.Max(p31.X, p32.X);
                    double minY = Math.Min(p31.Y, p32.Y);
                    double maxY = Math.Max(p31.Y, p32.Y);
                    Polyline rect = new Polyline();
                    rect.AddVertexAt(0, new Point2d(minX, minY), 0, 0, 0);
                    rect.AddVertexAt(1, new Point2d(maxX, minY), 0, 0, 0);
                    rect.AddVertexAt(2, new Point2d(maxX, maxY), 0, 0, 0);
                    rect.AddVertexAt(3, new Point2d(minX, maxY), 0, 0, 0);
                    rect.Closed = true;
                    showCrv.Add(rect);

                    double rec1Wid = Crvc.LowerLeft.DistanceTo(Crvc.LowerRight);
                    double rec1Hei = Crvc.LowerLeft.DistanceTo(Crvc.UpperLeft);
                    double rec2Wid = Math.Abs(p31.X - p32.X);
                    double rec2Hei = Math.Abs(p31.Y - p32.Y);
                    double scalebili = 0;
                    double format1 = rec2Wid / rec1Wid * rec1Hei;
                    if (format1 > rec2Hei)
                    {
                        //scalebili = rec2Wid / rec1Wid;
                        scalebili = rec2Hei / rec1Hei;
                    }
                    else
                    {
                        scalebili = rec2Wid / rec1Wid;
                        //scalebili = rec2Hei / rec1Hei;
                    }
                    if (double.IsInfinity(scalebili))
                    {
                        scalebili = 0;
                    }
                    Polyline rect1 = new Polyline();
                    Point2d p21 = Crvc.LowerLeft.Convert2d(new Plane());
                    Point2d p22 = Crvc.UpperLeft.Convert2d(new Plane());
                    Point2d p23 = Crvc.UpperRight.Convert2d(new Plane());
                    Point2d p24 = Crvc.LowerRight.Convert2d(new Plane());
                    rect1.AddVertexAt(0, p21, 0, 0, 0);
                    rect1.AddVertexAt(1, p22, 0, 0, 0);
                    rect1.AddVertexAt(2, p23, 0, 0, 0);
                    rect1.AddVertexAt(3, p24, 0, 0, 0);
                    rect1.Closed = true;
                    raMo = Matrix3d.Displacement(rect.StartPoint - rect1.StartPoint);
                    rect1.TransformBy(raMo);
                    raSC = Matrix3d.Scaling(scalebili, rect.StartPoint);
                    rect1.TransformBy(raSC);
                    rect1.ColorIndex = 1;
                    showCrv.Add(rect1);
                    baseDimPt = pttemp;
                    return SamplerStatus.OK;
                }
                else return SamplerStatus.NoChange;
            }
            else return SamplerStatus.Cancel;
        }

        protected override bool WorldDraw(AAG.WorldDraw draw)
        {
            for (int i = 0; i < showCrv.Count; i++)
            {
                draw.Geometry.Draw(showCrv[i]);
            }
            return true;
        }
    }

    public class HanJieDraw : DrawJig
    {
        public Point3d baseDimPt;
        Curve showCrv;
        Curve Crvc;
        public static double radi = 2;
        public HanJieDraw(Curve Crv)
        {
            Crvc = Crv;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
        hanjie: Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ViewTableRecord view = ed.GetCurrentView();
            Vector3d viewDir = view.ViewDirection;//当前视图方向
            Plane viewPlane = new Plane(Point3d.Origin, viewDir);
            JigPromptPointOptions jpo = new JigPromptPointOptions("\n定位点：[半径(R)]", "R");
            PromptPointResult ppr = prompts.AcquirePoint(jpo);
            if (ppr.StringResult == "R")
            {
                PromptDoubleResult pdr = ed.GetDouble("\n需求半径");
                if (pdr.Status == PromptStatus.OK)
                {
                    radi = pdr.Value;
                    goto hanjie;
                }
            }

            if (ppr.Status == PromptStatus.OK)
            {
                Point3d pttemp = ppr.Value;
                if (pttemp != baseDimPt)
                {
                    showCrv = null;
                }
                if (pttemp != baseDimPt)
                {
                    Point3d p3 = Crvc.GetClosestPointTo(ppr.Value, false);
                    Circle cc = new Circle(p3, viewDir, radi);
                    Point3dCollection points = new Point3dCollection();
                    Crvc.IntersectWith((Entity)cc, Intersect.OnBothOperands, viewPlane, points, IntPtr.Zero, IntPtr.Zero);
                    List<Point3d> zhongzhuan = new List<Point3d>();
                    foreach (Point3d item in points)
                    {
                        zhongzhuan.Add(item);
                    }
                    zhongzhuan = zhongzhuan.Select(m => cc.GetClosestPointTo(m, viewDir, false)).ToList();
                    double[] paramsArr = zhongzhuan.Select(k => cc.GetParameterAtPoint(k)).ToArray();
                    Array.Sort(paramsArr);
                    DoubleCollection doubleCollection = new DoubleCollection(paramsArr);
                    DBObjectCollection doc = cc.GetSplitCurves(doubleCollection);

                    Curve cc1 = doc[0] as Curve;
                    Point3d closestp31 = cc1.GetClosestPointTo(ppr.Value, false);
                    Curve cc2 = doc[1] as Curve;
                    Point3d closestp32 = cc2.GetClosestPointTo(ppr.Value, false);

                    int Insertindex = 0;
                    if (Crvc.Closed) Insertindex = 0;
                    else Insertindex = 1;


                    if (closestp32.DistanceTo(ppr.Value) > closestp31.DistanceTo(ppr.Value))
                    {
                        showCrv = doc[0] as Curve;
                    }
                    else
                    {
                        showCrv = doc[1] as Curve;
                    }
                    baseDimPt = pttemp;
                    return SamplerStatus.OK;
                }
                else return SamplerStatus.NoChange;
            }
            else return SamplerStatus.Cancel;
        }

        protected override bool WorldDraw(AAG.WorldDraw draw)
        {
            draw.Geometry.Draw(showCrv);
            return true;
        }
    }

    public class PtDimLine : DrawJig
    {
        public Point3d baseDimPt;
        List<Line> lines = new List<Line>();
        List<Point3d> point3Dss;
        public PtDimLine(List<Point3d> point3Ds)
        {
            point3Dss = point3Ds;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            PromptPointResult ppr = prompts.AcquirePoint("选择标注点");
            double an = 0;
            double linelength = 0;
            Vector3d vector3D = Vector3d.XAxis;
            if (ppr.Status == PromptStatus.OK)
            {
                Point3d pttemp = ppr.Value;
                if (pttemp != baseDimPt)
                {
                    lines.Clear();
                }
                if (pttemp != baseDimPt)
                {
                    point3DsSort(point3Dss, pttemp, out an);
                    for (int i = 0; i < point3Dss.Count; i++)
                    {
                        if (an != 0)
                        {
                            linelength = pttemp.X - point3Dss[i].X;
                            vector3D = new Vector3d(linelength, 0, 0);
                        }
                        else
                        {
                            linelength = pttemp.Y - point3Dss[i].Y;
                            vector3D = new Vector3d(0, linelength, 0);

                        }
                        Line line = new Line(point3Dss[i], point3Dss[i].TransformBy(Matrix3d.Displacement(vector3D)));
                        lines.Add(line);
                    }
                    baseDimPt = pttemp;
                    return SamplerStatus.OK;
                }
                else return SamplerStatus.NoChange;
            }
            else return SamplerStatus.Cancel;
        }

        protected override bool WorldDraw(AAG.WorldDraw draw)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                draw.Geometry.Draw(lines[i]);
            }
            return true;
        }

        public List<Point3d> point3DsSort(List<Point3d> point3Ds, Point3d baseDimPoint, out double Angle)
        {
            double[] Ys = point3Ds.Select(i => i.Y).ToArray();
            Array.Sort(Ys);
            double angleresult = 0;
            List<Point3d> result = new List<Point3d>();
            if (Ys[0] < baseDimPoint.Y && Ys[Ys.Length - 1] > baseDimPoint.Y)
            {
                angleresult = 0;
            }
            else
            {
                angleresult = 90;
            }

            if (angleresult == 0)
            {
                double[] SortYs = point3Ds.Select(i => i.Y).ToArray();
                Point3d[] pointlinshi = point3Ds.ToArray();
                Array.Sort(SortYs, pointlinshi);
                result = pointlinshi.ToList();
                angleresult = starMathdy.Radians(90);
            }
            else
            {
                double[] SortXs = point3Ds.Select(i => i.X).ToArray();
                Point3d[] pointlinshi = point3Ds.ToArray();
                Array.Sort(SortXs, pointlinshi);
                result = pointlinshi.ToList();
                angleresult = starMathdy.Radians(0);
            }
            Angle = angleresult;
            return result;
        }
    }

    public class sCADDrawJig : DrawJig
    {
        private List<Entity> jEntList { get; set; }
        private Point3d jPointBase { get; set; }
        private Point3d jPointBaseNoChange { get; set; }
        private Point3d PointPre { get; set; }
        private string jMessage;
        private Point3d p31 { get; set; }
        public Point3d p32 { get; set; }
        public Point3d MovePoint { get; set; }

        public Matrix3d m3d = Matrix3d.Displacement(new Vector3d(0, 0, 0));

        public sCADDrawJig(List<Entity> entities, Point3d point, Point3d point1, Point3d point2, string message)
        {
            jEntList = entities;
            jPointBase = point;
            jMessage = message;
            PointPre = point;
            p31 = point1;
            p32 = point2;
            jPointBaseNoChange = point;
        }

        //重绘图形
        protected override bool WorldDraw(AAG.WorldDraw draw)
        {
            for (int i = 0; i < jEntList.Count; i++)
            {
                draw.Geometry.Draw(jEntList[i]);
            }
            return true;
        }

        //获取鼠标在屏幕的运动，需要更新图形对象的属性
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //声明一个提示类
            JigPromptPointOptions jppo = new JigPromptPointOptions(jMessage);
            jppo.Cursor = CursorType.RubberBand;
            jppo.BasePoint = jPointBase;
            jppo.UseBasePoint = true;
            jppo.UserInputControls = UserInputControls.Accept3dCoordinates;
            PromptPointResult ppr = prompts.AcquirePoint(jppo);
            Point3d curPoint = ppr.Value;
            if (curPoint != PointPre /*&& curPoint != jPointBaseNoChange*/)
            {
                OrientDrawJig(jEntList, p31, p32, jPointBaseNoChange, curPoint);
                p31 = jPointBase;
                p32 = curPoint;
                //    Vector3d vector3D1 = p32 - p31;
                //vector3D1 = starMathdy.Unitize(vector3D1);
                //Vector3d angleY3d1 = vector3D1.RotateBy(Math.PI * 0.5, Vector3d.ZAxis);
                //Vector3d vector3D2 = MovePoint - jPointBase;
                //vector3D2 = starMathdy.Unitize(vector3D2);
                //Vector3d angleY3d2 = vector3D2.RotateBy(Math.PI * 0.5, Vector3d.ZAxis);
                //Matrix3d matrix3D = Matrix3d.AlignCoordinateSystem(p31, vector3D1, angleY3d1, Vector3d.ZAxis, jPointBase, vector3D2, angleY3d2, Vector3d.ZAxis);
                //using (Transaction trans = db.TransactionManager.StartTransaction())
                //{
                //    for (int i = 0; i < jEntList.Count; i++)
                //    {
                //        Entity TEntity1 = (Entity)jEntList[i].ObjectId.GetObject(OpenMode.ForWrite);
                //        TEntity1.TransformBy(matrix3D);
                //    }
                //    trans.Commit();
                //}

            }
            PointPre = curPoint;
            return SamplerStatus.NoChange;
        }




        public void OrientDrawJig(List<Entity> Entitys, Point3d p3a1, Point3d p3a2, Point3d p3b1, Point3d p3b2)
        {
            Database db = HostApplicationServices.WorkingDatabase;

            Vector3d vector3D1 = p3a2 - p3a1;
            vector3D1 = starMathdy.Unitize(vector3D1);
            Vector3d angleY3d1 = vector3D1.RotateBy(Math.PI * 0.5, Vector3d.ZAxis);
            Vector3d vector3D2 = p3b2 - p3b1;
            vector3D2 = starMathdy.Unitize(vector3D2);
            Vector3d angleY3d2 = vector3D2.RotateBy(Math.PI * 0.5, Vector3d.ZAxis);
            m3d = Matrix3d.AlignCoordinateSystem(p3a1, vector3D1, angleY3d1, Vector3d.ZAxis, p3b1, vector3D2, angleY3d2, Vector3d.ZAxis);
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                if (double.IsNaN(vector3D2.Length) || double.IsNaN(vector3D1.Length))
                {
                    return;
                }
                for (int i = 0; i < Entitys.Count; i++)
                {
                    Entity TEntity1 = (Entity)Entitys[i].ObjectId.GetObject(OpenMode.ForWrite);
                    TEntity1.TransformBy(m3d);
                }
                trans.Commit();
            }
        }
    }
}


/*******************************************************************/
//    #region DrawJig动态拖动多个实体
//    public class CADDrawJig : DrawJig
//    {
//        //变量
//        Point3d oldPt;//实体移动之前的位置
//        Point3d newPt;//实体移动之后的位置
//        Vector3d v;//实体移动的向量

//        List<Entity> ents = new List<Entity>();
//        List<Point3d> oldPts = new List<Point3d>();
//        public CADDrawJig(List<Entity> ents)
//        {
//            oldPt = Point3d.Origin;
//            newPt = Point3d.Origin;
//            v = new Vector3d();
//            this.ents = ents;

//            Autodesk.AutoCAD.DatabaseServices.Entity ent = ents[0];
//            if (ent is BlockReference)
//            {
//                BlockReference br = ent as BlockReference;
//                oldPt = br.Position;
//            }

//            for (int i = 1; i < ents.Count; i++)
//            {
//                if (ents[i] is BlockReference)
//                {
//                    BlockReference br = ents[i] as BlockReference;
//                    oldPts.Add(br.Position);
//                }
//            }
//        }

//        //更新
//        protected override bool WorldDraw(Autodesk.AutoCAD.GraphicsInterface.WorldDraw draw)
//        {
//            Entity ent = ents[0];
//            ent.UpgradeOpen();
//            if (ent is BlockReference)
//            {
//                BlockReference br = ent as BlockReference;
//                br.Position = newPt;
//                draw.Geometry.Draw(br);
//            }

//            v = newPt.GetVectorTo(oldPt);

//            for (int i = 1; i < ents.Count; i++)
//            {
//                Entity entity = ents[i];
//                entity.UpgradeOpen();
//                if (entity is BlockReference)
//                {
//                    BlockReference br = entity as BlockReference;
//                    br.Position = oldPts[i - 1] + v;
//                    draw.Geometry.Draw(entity);
//                }
//            }

//            return true;
//        }
//        //取样函数
//        protected override SamplerStatus Sampler(JigPrompts prompts)
//        {
//            JigPromptPointOptions opt = new JigPromptPointOptions();
//            opt.UserInputControls = UserInputControls.Accept3dCoordinates | UserInputControls.NoNegativeResponseAccepted | UserInputControls.NullResponseAccepted;
//            opt.UseBasePoint = true;
//            opt.Cursor = CursorType.RubberBand;
//            opt.BasePoint = oldPt;
//            opt.Message = "选择移动到的终点";

//            PromptPointResult res = prompts.AcquirePoint(opt);
//            if (PromptStatus.OK == res.Status)
//            {
//                if (res.Value == newPt)
//                {
//                    return SamplerStatus.NoChange;
//                }
//                newPt = res.Value;
//            }
//            else
//            {
//                return SamplerStatus.Cancel;
//            }
//            return SamplerStatus.OK;
//        }
//    }
//    #endregion

//    public class JigMoveMultipleEntity : DrawJig
//    {
//        private Point3d mBase;
//        private Point3d mLocation;
//        List<Entity> mEntities;

//        public JigMoveMultipleEntity(Point3d basePt)
//        {
//            mBase = basePt.TransformBy(UCS);
//            mEntities = new List<Entity>();
//        }

//        public Point3d Base
//        {
//            get { return mLocation; }
//            set { mLocation = value; }
//        }

//        public Point3d Location
//        {
//            get { return mLocation; }
//            set { mLocation = value; }
//        }

//        public Matrix3d UCS
//        {
//            get
//            {
//                return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.CurrentUserCoordinateSystem;
//            }
//        }

//        public void AddEntity(Entity ent)
//        {
//            mEntities.Add(ent);
//        }

//        public void TransformEntities()
//        {
//            Matrix3d mat = Matrix3d.Displacement(mBase.GetVectorTo(mLocation));

//            foreach (Entity ent in mEntities)
//            {
//                ent.TransformBy(mat);
//            }
//        }

//        protected override SamplerStatus Sampler(JigPrompts prompts)
//        {
//            JigPromptPointOptions prOptions1 = new JigPromptPointOptions("\n新的位置:");
//            prOptions1.UseBasePoint = false;

//            PromptPointResult prResult1 = prompts.AcquirePoint(prOptions1);
//            if (prResult1.Status == PromptStatus.Cancel || prResult1.Status == PromptStatus.Error)
//                return SamplerStatus.Cancel;

//            if (!mLocation.IsEqualTo(prResult1.Value, new Tolerance(10e-10, 10e-10)))
//            {
//                mLocation = prResult1.Value;
//                return SamplerStatus.OK;
//            }
//            else
//                return SamplerStatus.NoChange;
//        }
//        protected override bool WorldDraw(Autodesk.AutoCAD.GraphicsInterface.WorldDraw draw)
//        {
//            Matrix3d mat = Matrix3d.Displacement(mBase.GetVectorTo(mLocation));

//            WorldGeometry geo = draw.Geometry;
//            if (geo != null)
//            {
//                geo.PushModelTransform(mat);

//                foreach (Entity ent in mEntities)
//                {
//                    geo.Draw(ent);
//                }

//                geo.PopModelTransform();
//            }

//            return true;
//        }
//    }
//}
