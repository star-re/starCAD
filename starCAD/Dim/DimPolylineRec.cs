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
    public class DimPolylineRec
    {
        [CommandMethod("DimPolyline", CommandFlags.UsePickSet)]

        public void DimPolylineRectangle()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            bool flag0 = true;

            AlignedDimension alignedDimension1 = new AlignedDimension();
            AlignedDimension alignedDimension2 = new AlignedDimension();
            PromptSelectionResult psr = ed.SelectImplied();
            if (psr.Status != PromptStatus.OK)
            {
                psr = ed.GetSelection();
            }
            if (psr.Status == PromptStatus.Cancel) return;
            if (psr.Status == PromptStatus.None) return;
            if (psr.Status != PromptStatus.OK) return;
            SelectionSet sSet = psr.Value;
            ObjectId[] dimObjectId = sSet.GetObjectIds();
           List< Entity> TEntitys = new List<Entity>();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                for (int j = 0; j < sSet.Count; j++)
                {
                  Entity entity =  (Entity)dimObjectId[j].GetObject(OpenMode.ForWrite);
                    if (entity.GetType() == typeof(Polyline))
                    {
                        TEntitys.Add((Entity)dimObjectId[j].GetObject(OpenMode.ForWrite));
                    }
                }
            }
            for (int j = 0; j < TEntitys.Count; j++)
            {
                Entity TEntity1 = TEntitys[j];
                List<Point3d> MidPoints = new List<Point3d>();
                int PolylineIndexX1 = 0;
                int PolylineIndexX2 = 0;
                int PolylineIndexY1 = 0;
                int PolylineIndexY2 = 0;
                Point3d MidX1 = Point3d.Origin;
                Point3d MidX2 = Point3d.Origin;
                Point3d MidY1 = Point3d.Origin;
                Point3d MidY2 = Point3d.Origin;
                if (flag0)
                {
                    MidPoints.Clear();
                    Polyline polyline = (Polyline)TEntity1;
                    if (polyline.Closed)
                    {
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
                        alignedDimension1.XLine1Point = polyline.GetLineSegmentAt(PolylineIndexX1).StartPoint;
                        alignedDimension1.XLine2Point = polyline.GetLineSegmentAt(PolylineIndexX1).EndPoint;
                        Line l1 = new Line(MidX1, MidPoint3d);
                        alignedDimension1.DimLinePoint = l1.GetPointAtParameter(l1.Length * 0.2);
                        ReBlockTable.ReBlock(db, alignedDimension1);

                        alignedDimension2.XLine1Point = polyline.GetLineSegmentAt(PolylineIndexY1).StartPoint;
                        alignedDimension2.XLine2Point = polyline.GetLineSegmentAt(PolylineIndexY1).EndPoint;
                        l1 = new Line(MidY1, MidPoint3d);
                        alignedDimension2.DimLinePoint = l1.GetPointAtParameter(l1.Length * 0.2);
                        ReBlockTable.ReBlock(db, alignedDimension2);

                        flag0 = false;
                    }
                }
                else
                {
                    MidPoints.Clear();
                    alignedDimension1 = new AlignedDimension();
                    alignedDimension2 = new AlignedDimension();
                    Polyline polyline = (Polyline)TEntity1;
                    if (polyline.Closed)
                    {
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
                        alignedDimension1.XLine1Point = polyline.GetLineSegmentAt(PolylineIndexX1).StartPoint;
                        alignedDimension1.XLine2Point = polyline.GetLineSegmentAt(PolylineIndexX1).EndPoint;
                        Line l1 = new Line(MidX1, MidPoint3d);
                        alignedDimension1.DimLinePoint = l1.GetPointAtParameter(l1.Length * 0.2);
                        ReBlockTable.ReBlock(db, alignedDimension1);

                        alignedDimension2.XLine1Point = polyline.GetLineSegmentAt(PolylineIndexY1).StartPoint;
                        alignedDimension2.XLine2Point = polyline.GetLineSegmentAt(PolylineIndexY1).EndPoint;
                        l1 = new Line(MidY1, MidPoint3d);
                        alignedDimension2.DimLinePoint = l1.GetPointAtParameter(l1.Length * 0.2);
                        ReBlockTable.ReBlock(db, alignedDimension2);
                        //doc = ed.TraceBoundary(ppr.Value, true);
                        //entity = new Entity[] { (Entity)doc[0] };
                        //ReBlockTable.ReBlock(db, entity);
                    }
                }
            }
        }
    }
}
