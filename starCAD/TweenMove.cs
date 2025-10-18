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
    public class TweenMove
    {
        [CommandMethod("TweenMove", CommandFlags.UsePickSet)]

        public void TweenM()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            List<Point3d> p3s = new List<Point3d>();
            List<double> p3Len = new List<double>();
            PromptSelectionResult psr = ed.SelectImplied();
            if (psr.Status != PromptStatus.OK)
            {
                psr = ed.GetSelection();
            }
            if (psr.Status == PromptStatus.Cancel) return;
            if (psr.Status == PromptStatus.None) return;
            if (psr.Status != PromptStatus.OK) return;
            PromptDoubleResult pdr = ed.GetDouble("输入间距");
            if (pdr.Status == PromptStatus.Cancel) return;
            if (pdr.Status == PromptStatus.None) return;
            double inputmove = pdr.Value;
            SelectionSet sSet = psr.Value;
            ObjectId[] dimObjectId = sSet.GetObjectIds();
            Entity[] entities = new Entity[dimObjectId.Length];
            Point3d[] p3R = new Point3d[dimObjectId.Length];
            Extents3d[] rectangleMinAndMax = new Extents3d[dimObjectId.Length];

            PromptSelectionResult psrAll = ed.SelectAll();
            List<Point3d> p3sAll = new List<Point3d>();
            List<double> p3LenAll = new List<double>();
            SelectionSet sSetAll = psrAll.Value;
            ObjectId[] dimObjectIdAll = sSetAll.GetObjectIds();
            Entity[] entitiesAll = new Entity[dimObjectIdAll.Length];
            Point3d[] p3RAll = new Point3d[dimObjectIdAll.Length];
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                for (int i = 0; i < sSet.Count; i++)
                {
                    p3s.Clear();
                    Entity TEntity1 = (Entity)dimObjectId[i].GetObject(OpenMode.ForWrite);
                    entities[i] = TEntity1;
                    Extents3d e = (Extents3d)TEntity1.Bounds;
                    p3s.Add(e.MaxPoint);
                    p3s.Add(e.MinPoint);
                    p3Len.Add(Math.Abs(e.MaxPoint.X - e.MinPoint.X));
                    p3R[i] = starMathdy.Pointaverage(p3s);
                    Point3d[] p3szhuan = new Point3d[2];
                    p3s.CopyTo(p3szhuan);
                    rectangleMinAndMax[i] = e;
                }
                for (int i = 0; i < sSetAll.Count; i++)
                {
                    p3sAll.Clear();
                    Entity TEntity1 = (Entity)dimObjectIdAll[i].GetObject(OpenMode.ForWrite,true,true);
                    LayerTable lt = (LayerTable)trans.GetObject(dimObjectIdAll[i].Database.LayerTableId, OpenMode.ForRead);

                    entitiesAll[i] = TEntity1;
                    Extents3d e = new Extents3d();
                    if (TEntity1.Bounds != null)
                    {
                        e = (Extents3d)TEntity1.Bounds;
                    }
                    p3sAll.Add(e.MaxPoint);
                    p3sAll.Add(e.MinPoint);
                    p3RAll[i] = starMathdy.Pointaverage(p3sAll);
                }

                double movelen = 0;
                double[] xarr = p3R.Select(i => i.X).ToArray();
                p3R = starMathdy.pointsXSort(xarr, p3R);
                entities = starMathdy.EntitySort(xarr, entities);
                rectangleMinAndMax = starMathdy.ExtentSort(xarr, rectangleMinAndMax);


                for (int i = 1; i < p3R.Length; i++)
                {
                    movelen = movelen + inputmove;
                    Vector3d v3 = new Vector3d(movelen, 0, 0);
                    Matrix3d matrix2D = Matrix3d.Displacement(v3);
                    Extents3d e = rectangleMinAndMax[i];
                    for (int j = 0; j < p3RAll.Length; j++)
                    {
                        if (e.MaxPoint.X > p3RAll[j].X &&
                            e.MaxPoint.Y > p3RAll[j].Y &&
                            e.MinPoint.X < p3RAll[j].X &&
                            e.MinPoint.Y < p3RAll[j].Y)
                        {
                            entitiesAll[j].TransformBy(matrix2D);
                        }
                    }
                }
                trans.Commit();
            }
            //ReBlockTable.ReBlock(db,entities);
        }
    }
}
