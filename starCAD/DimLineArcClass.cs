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
    public class DimLineArcClass
    {
        public void GetCurve(List<Entity> entities, out Curve curve, out List<Curve> curves, out List<Point3d> points)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            
            List<Entity> TEntitys = entities;
            Curve finishCurve = null;
            List<Curve> SplitCurves = new List<Curve>();
            List<double> AllSize = new List<double>();
            List<Point3d> SplitPoint = new List<Point3d>();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                for (int j = 0; j < entities.Count; j++)
                {
                    Type entType = entities[j].GetType();
                    if (entities[j].GetType() == typeof(DBPoint))
                    {
                        DBPoint splitPoint = entities[j] as DBPoint;
                        SplitPoint.Add(splitPoint.Position);
                    }
                    else
                    {
                        Curve cc = entities[j] as Curve;
                        SplitCurves.Add(cc);
                        //double Len = 0;
                        if (cc.GetType() == typeof(Line))
                        {
                            Line line = cc as Line;
                            AllSize.Add(line.Length);
                        }
                        else if (cc.GetType() == typeof(Polyline))
                        {
                            Polyline polyline = cc as Polyline;
                            AllSize.Add(polyline.Length);
                        }
                        else if (cc.GetType() == typeof(Arc))
                        {
                            Arc arc = cc as Arc;
                            AllSize.Add(arc.Length);
                        }
                        else if (cc.GetType() == typeof(Polyline2d))
                        {
                            Polyline2d P2d = cc as Polyline2d;
                            //Polyline polyline = new Polyline();
                            //polyline.ConvertFrom(P2d, true);
                            //SplitCurves.RemoveAt(SplitCurves.Count - 1);
                            //SplitCurves.Add(polyline);
                            AllSize.Add(P2d.Length);
                        }
                        else if (cc.GetType() == typeof(Polyline3d))
                        {
                            Polyline3d P3d = cc as Polyline3d;
                            AllSize.Add(P3d.Length);
                        }
                        else if (cc.GetType() == typeof(Spline))
                        {
                            Spline spline = cc as Spline;
                            double length = spline.GetGeCurve().GetLength(spline.StartParam, spline.EndParam, 0.001);
                            AllSize.Add(length);
                        }
                    }
                }
            }
            finishCurve = SplitCurves[AllSize.IndexOf(AllSize.Max())];
            SplitCurves.RemoveAt(AllSize.IndexOf(AllSize.Max()));
            curve = finishCurve;
            curves = SplitCurves;
            points = SplitPoint;
        }

        public Point3dCollection PLpt(Polyline3d polyline3D)
        {
            List<Point3d> p3s = new List<Point3d>();
            Point3dCollection pts = new Point3dCollection();
            polyline3D.GetStretchPoints(pts);
            //p3s = pts.Cast<Point3d>().ToList();
            return pts;
        }

        public Point3dCollection PLpt(Polyline2d polyline2D)
        {
            List<Point3d> p3s = new List<Point3d>();
            Point3dCollection pts = new Point3dCollection();
            polyline2D.GetStretchPoints(pts);
            //p3s = pts.Cast<Point3d>().ToList();
            return pts;
        }

        public void SplitCurve(Curve curve, List<Curve> curves, List<Point3d> points)
        {
            Point3dCollection SplitBasePoint = GetMinPoints(curve, curves, points);
            // curve.GetSplitCurves()
            DBObjectCollection dbc = curve.GetSplitCurves(SplitBasePoint);
            List<Curve> DimCurve = new List<Curve>();
            for (int i = 0; i < dbc.Count; i++)
            {
                DimCurve.Add((Curve)dbc[i]);
                //ReBlockTable.ReBlock((Entity)dbc[i]);
            }
            ShengChengDim(curve, DimCurve);
        }

        public void SplitCurve(Curve curve, List<Point3d> points)
        {
            Point3dCollection SplitBasePoint = GetPoints(curve, points);
            // curve.GetSplitCurves()
            DBObjectCollection dbc = curve.GetSplitCurves(SplitBasePoint);
            List<Curve> DimCurve = new List<Curve>();
            for (int i = 0; i < dbc.Count; i++)
            {
                DimCurve.Add((Curve)dbc[i]);
                //ReBlockTable.ReBlock((Entity)dbc[i]);
            }
            ShengChengDim(curve, DimCurve);
        }


        public Point3dCollection GetMinPoints(Curve curve, List<Curve> curves, List<Point3d> points)
        {
            List<Point3d> result = new List<Point3d>();
            Point3d pt1, pt2;
            double d1, d2;
            for (int i = 0; i < curves.Count; i++)
            {
                pt1 = curve.GetClosestPointTo(curves[i].StartPoint, false);
                pt2 = curve.GetClosestPointTo(curves[i].EndPoint, false);
                d1 = pt1.DistanceTo(curves[i].StartPoint);
                d2 = pt1.DistanceTo(curves[i].EndPoint);
                if (d1 > d2)
                {
                    result.Add(pt2);
                }
                else
                {
                    result.Add(pt1);
                }
            }
            List<Point3d> result1 = points.Select(i => curve.GetClosestPointTo(i, false)).ToList();
            result.AddRange(result1);
            double[] TArray = result.Select(i => curve.GetParameterAtPoint(i)).ToArray();
            Point3d[] PArray = result.ToArray();
            Array.Sort(TArray, PArray);
            Point3dCollection point3DCollection = new Point3dCollection(PArray);
            return point3DCollection;
        }

        public Point3dCollection GetPoints(Curve curve, List<Point3d> points)
        {
            List<Point3d> result = new List<Point3d>();
            Point3d pt1;
            for (int i = 0; i < points.Count; i++)
            {
                pt1 = curve.GetClosestPointTo(points[i], false);
                result.Add(pt1);

            }
            double[] TArray = result.Select(i => curve.GetParameterAtPoint(i)).ToArray();
            Point3d[] PArray = result.ToArray();
            Array.Sort(TArray, PArray);
            Point3dCollection point3DCollection = new Point3dCollection(PArray);
            return point3DCollection;
        }

        public void ShengChengDim(Curve basecurve, List<Curve> DimCurve)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptPointOptions ppo = new PromptPointOptions("\n偏移距离[(O)]", "选择标注方向");
            PromptPointResult psr = ed.GetPoint(ppo);
            PromptPointResult ppr = null;
            // PromptPointResult psr = null;
            bool flag = true;
            double offLength = 100;
            //while (flag)
            //{
            //    ppr = ed.GetPoint(ppo);
            //    if (ppr.StringResult == "O")
            //    {
            //        PromptResult Fenge = ed.GetString("\n 请输入偏移距离");
            //        offLength = double.Parse(Fenge.StringResult);
            //        psr = ppr;
            //        flag = false;
            //    }
            //    if (ppr.Status == PromptStatus.OK)
            //    {
            //        flag = false;
            //        psr = ppr;
            //    }
            //    if (ppr.Status == PromptStatus.Cancel) return;
            //    if (ppr.Status == PromptStatus.None) return;
            //    //if (ppr.Status != PromptStatus.OK) return;
            //}



            Point3d sSet = psr.Value;

            List<Line> lines = new List<Line>();
            List<Arc> arcs = new List<Arc>();
            List<Line> Savelines = new List<Line>();
            List<Arc> SavearcList = new List<Arc>();
            List<Spline> splines = new List<Spline>();

            List<Point3d> linedimpointlist = new List<Point3d>();
            List<Point3d> arcdimpointlist = new List<Point3d>();
            ExplodeCurve(DimCurve, out lines, out arcs, out splines);
            if (lines.Count != 0)
            {
                linedimpointlist = NorPoint(lines.Select(i => (Curve)i).ToList(), basecurve, sSet, offLength);
                LineDim(lines, linedimpointlist);
            }
            if (arcs.Count != 0)
            {
                arcdimpointlist = NorPoint(arcs.Select(i => (Curve)i).ToList(), basecurve, sSet, offLength);
                ArcDim(arcs, arcdimpointlist);
            }
            if (splines.Count != 0)
            {
                arcdimpointlist = NorPoint(splines.Select(i => (Curve)i).ToList(), basecurve, sSet, offLength);
                splinesDim(splines, arcdimpointlist);
            }
            //for (int i = 0; i < length; i++)
            //{

            //}
            //for (int i = 0; i < DimCurve.Count; i++)
            //{
            //    Point3d locationPoint = MiddlePoint(DimCurve[i]);
            //    Vector3d vector3D = basecurve.GetFirstDerivative(locationPoint);
            //    Vector3d Nor = vector3D.CrossProduct(Vector3d.ZAxis);
            //    Nor = starMathdy.Unitize(Nor);
            //    Nor = Nor.MultiplyBy(100);
            //    Matrix3d matrix3D = Matrix3d.Displacement(Nor);
            //    lines.Add(new Line(locationPoint, locationPoint.TransformBy(matrix3D)));
            //}
            //ReBlockTable.ReBlock(lines);
        }

        public List<Point3d> NorPoint(List<Curve> curves, Curve basecurve, Point3d point3D, double offLen)//jisuan 
        {
            Point3d dispoint1 = basecurve.GetClosestPointTo(point3D, false);
            Vector3d offsetV = point3D - dispoint1;
            Curve offsetcurve = null;
            List<Point3d> result = new List<Point3d>();
            //Curve offsetcurve = (Curve)basecurve.GetOffsetCurvesGivenPlaneNormal(offsetV, 100)[0];
            if (basecurve.GetType() == typeof(Spline))
            {
                // DBObjectCollection dboc = basecurve.GetOffsetCurves(100);
                Spline spline = basecurve as Spline;
                Point3d closestpoint = spline.GetClosestPointTo(point3D, true);
                Vector3d splineV3Nor = spline.GetFirstDerivative(closestpoint);
                splineV3Nor = splineV3Nor.CrossProduct(Vector3d.ZAxis);
                double angle = splineV3Nor.GetAngleTo(offsetV);

                Point3d locationPoint = new Point3d();
                Vector3d vector3D = new Vector3d();
                double t = 0;
                for (int i = 0; i < curves.Count; i++)
                {
                    locationPoint = MiddlePoint(curves[i]);
                    t = basecurve.GetParameterAtPoint(locationPoint);
                    vector3D = basecurve.GetFirstDerivative(t);
                    vector3D = vector3D.CrossProduct(Vector3d.ZAxis);
                    vector3D = starMathdy.Unitize(vector3D);
                    if (angle > Math.PI * 0.51)
                    {
                        vector3D = vector3D.Negate();
                    }
                    vector3D = vector3D.MultiplyBy(offLen);
                    Matrix3d matrix3D = Matrix3d.Displacement(vector3D);
                    result.Add(locationPoint.TransformBy(matrix3D));
                }
                //DBObjectCollection dboc = spline.GetOffsetCurvesGivenPlaneNormal(offsetV, 100);
                //basecurve = basecurve.GetOffsetCurvesGivenPlaneNormal(offsetV, 100)[0] as Curve;
            }
            else
            {
                offsetcurve = (Curve)basecurve.GetOffsetCurves(100)[0];
                Point3d dispoint2 = offsetcurve.GetClosestPointTo(point3D, false);
                //Point3d dispoint2 = new Point3d();
                Point3d locationPoint = new Point3d();
                Vector3d vector3D = new Vector3d();
                //Vector3d Nor = new Vector3d();
                double t = 0;
                for (int i = 0; i < curves.Count; i++)
                {
                    locationPoint = MiddlePoint(curves[i]);
                    //locationPoint = offsetcurve.GetClosestPointTo(locationPoint, false);
                    //t = basecurve.GetParameterAtPoint(locationPoint);
                    t = CurveT(basecurve, locationPoint, curves[i]);
                    //dispoint2 = offsetcurve.GetClosestPointTo(locationPoint, false);
                    vector3D = basecurve.GetFirstDerivative(t);
                    Vector3d Nor = dispoint2 - basecurve.GetClosestPointTo(locationPoint, false);
                    Nor = vector3D.CrossProduct(Vector3d.ZAxis);
                    Nor = starMathdy.Unitize(Nor);
                    Nor = Nor.MultiplyBy(offLen);
                    Matrix3d matrix3D = Matrix3d.Displacement(Nor);
                    //dispoint2 = locationPoint.TransformBy(matrix3D);
                    if (dispoint1.DistanceTo(point3D) < dispoint2.DistanceTo(point3D))
                    {
                        Nor = Nor.Negate();
                        matrix3D = Matrix3d.Displacement(Nor);
                    }
                    if (basecurve.GetType() == typeof(Line))
                    {
                        Nor = starMathdy.Unitize(offsetV);
                        Nor = Nor.MultiplyBy(offLen);
                        matrix3D = Matrix3d.Displacement(Nor);
                    }
                    if (basecurve.GetType() == typeof(Arc))
                    {
                        Nor = starMathdy.Unitize(offsetV);
                        Nor = Nor.MultiplyBy(offLen);
                        matrix3D = Matrix3d.Displacement(Nor);
                    }
                    result.Add(locationPoint.TransformBy(matrix3D));
                    //lines.Add(new Line(locationPoint, locationPoint.TransformBy(matrix3D)));
                }
            }
            return result;
        }

        public double LineT = 0;
        public double CurveT(Curve curve, Point3d point3D, Curve cc)
        {
            double result = 0;
            if (curve.GetType() == typeof(Line))
            {
                double aa = cc.StartParam;
                double bb = cc.EndParam;
                LineT = LineT + (aa + bb) / 2;
                result = LineT;
            }
            else
            {
                result = curve.GetParameterAtPoint(point3D);
            }
            return result;
        }

        public Point3d MiddlePoint(Curve curve)
        {
            double aa = curve.StartParam;
            double bb = curve.EndParam;
            return curve.GetPointAtParameter((aa + bb) / 2);
        }

        public void ExplodeCurve(List<Curve> curves, out List<Line> Dimlines, out List<Arc> DimArcs, out List<Spline> DimSplines)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            List<Line> lines = new List<Line>();
            List<Arc> arcList = new List<Arc>();
            List<Spline> splineList = new List<Spline>();

            List<Line> Savelines = new List<Line>();
            List<Arc> SavearcList = new List<Arc>();
            List<Spline> SavesplineList = new List<Spline>();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DBObjectCollection dBObjectCollection = new DBObjectCollection();
                for (int i = 0; i < curves.Count; i++)
                {
                    dBObjectCollection.Clear();
                    //curves[i].Explode(dBObjectCollection);
                    if (curves[i].GetType() == typeof(Polyline))
                    {
                        curves[i].Explode(dBObjectCollection);
                    }
                    else if (curves[i].GetType() == typeof(Polyline2d))
                    {
                        curves[i].Explode(dBObjectCollection);
                    }
                    else if (curves[i].GetType() == typeof(Polyline3d))
                    {
                        curves[i].Explode(dBObjectCollection);
                    }
                    dBObjectCollection.Add(curves[i]);
                    ArcLineType(dBObjectCollection, out lines, out arcList, out splineList);
                    Savelines.AddRange(lines);
                    SavearcList.AddRange(arcList);
                    SavesplineList.AddRange(splineList);
                }
            }
            Dimlines = Savelines;
            DimArcs = SavearcList;
            DimSplines = SavesplineList;
        }

        public void ArcLineType(DBObjectCollection dBObjectCollection, out List<Line> line, out List<Arc> arc, out List<Spline> spline)
        {
            List<Line> lines = new List<Line>();
            List<Arc> arcList = new List<Arc>();
            List<Spline> splinelist = new List<Spline>();
            for (int i = 0; i < dBObjectCollection.Count; i++)
            {
                if (dBObjectCollection[i].GetType() == typeof(Line))
                {
                    lines.Add((Line)dBObjectCollection[i]);
                }
                else if (dBObjectCollection[i].GetType() == typeof(Arc))
                {
                    arcList.Add((Arc)dBObjectCollection[i]);
                }
                else if (dBObjectCollection[i].GetType() == typeof(Spline))
                {
                    splinelist.Add((Spline)dBObjectCollection[i]);
                }
            }
            line = lines;
            arc = arcList;
            spline = splinelist;
        }

        public void LineDim(List<Line> lines, List<Point3d> dimPoints)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            List<AlignedDimension> dimensions = new List<AlignedDimension>();
            for (int i = 0; i < lines.Count; i++)
            {
                string dimensionText = "";
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                ObjectId dimensionStyle = doc.Database.Dimstyle;
                dimensions.Add(new AlignedDimension(lines[i].StartPoint, lines[i].EndPoint, dimPoints[i], dimensionText, dimensionStyle));
            }
            ReBlockTable.ReBlock(dimensions.Select(i => (Entity)i).ToList());
        }

        public void ArcDim(List<Arc> arcs, List<Point3d> dimPoints)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            List<ArcDimension> arcDimension = new List<ArcDimension>();
            for (int i = 0; i < arcs.Count; i++)
            {
                string dimensionText = "";
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                ObjectId dimensionStyle = doc.Database.Dimstyle;
                arcDimension.Add(new ArcDimension(arcs[i].Center, arcs[i].StartPoint, arcs[i].EndPoint, dimPoints[i], dimensionText, dimensionStyle));
            }
            ReBlockTable.ReBlock(arcDimension.Select(i => (Entity)i).ToList());
        }

        public void splinesDim(List<Spline> arcs, List<Point3d> dimPoints)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            List<AlignedDimension> dimensions = new List<AlignedDimension>();
            for (int i = 0; i < arcs.Count; i++)
            {
                string dimensionText = Math.Round(arcs[i].GetGeCurve().GetLength(arcs[i].StartParam, arcs[i].EndParam, 0.001), 2).ToString();
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                ObjectId dimensionStyle = doc.Database.Dimstyle;
                Point3d p31 = arcs[i].StartPoint;
                //Vector3d v31 = arcs[i].StartFitTangent;
                //v31 = v31.CrossProduct(Vector3d.ZAxis);
                //v31 = starMathdy.Unitize(v31);
                //v31 = v31.MultiplyBy(100);
                //Matrix3d matrix3D1 = Matrix3d.Displacement(v31);
                //Point3d p311 = p31.TransformBy(matrix3D1);

                Point3d p32 = arcs[i].EndPoint;
                //Vector3d v32 = arcs[i].EndFitTangent;
                //v32 = v32.CrossProduct(Vector3d.ZAxis);
                //v32 = starMathdy.Unitize(v32);
                //v32 = v32.MultiplyBy(100);
                //Matrix3d matrix3D2 = Matrix3d.Displacement(v32);
                //Point3d p322 = p32.TransformBy(matrix3D2);


                dimensions.Add(new AlignedDimension(p31, p32, dimPoints[i], dimensionText, dimensionStyle));

            }
            ReBlockTable.ReBlock(dimensions.Select(i => (Entity)i).ToList());
        }
    }
}
