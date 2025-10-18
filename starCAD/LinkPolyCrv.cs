using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using sCAD;
using starCAD;

namespace sCAD
{


    public class LinkPolyCrv
    {
        public List<Point3d> ARCpt(Arc arc)
        {
            List<Point3d> p3s = new List<Point3d>();
            p3s.Add(arc.StartPoint);
            p3s.Add(arc.EndPoint);
            return p3s;
        }

        public List<Point3d> Lpt(Line line)
        {
            List<Point3d> p3s = new List<Point3d>();
            p3s.Add(line.StartPoint);
            p3s.Add(line.EndPoint);
            return p3s;
        }

        public List<Point3d> PLpt(Polyline polyline)
        {
            List<Point3d> p3s = new List<Point3d>();
            int PLnum = polyline.NumberOfVertices;
            for (int j = 0; j < PLnum; j++)
            {
                p3s.Add(polyline.GetPoint3dAt(j));
            }
            return p3s;
        }

        public List<Point3d> PLpt(Polyline3d polyline3D)
        {
            List<Point3d> p3s = new List<Point3d>();
            Point3dCollection pts = new Point3dCollection();
            polyline3D.GetStretchPoints(pts);
            p3s = pts.Cast<Point3d>().ToList();
            return p3s;
        }

        public List<Point3d> PLpt(Polyline2d polyline2D)
        {
            List<Point3d> p3s = new List<Point3d>();
            Point3dCollection pts = new Point3dCollection();
            polyline2D.GetStretchPoints(pts);
            p3s = pts.Cast<Point3d>().ToList();
            return p3s;
        }

        [CommandMethod("LianJiePL")]
        public void LianJiePL()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Entity[] entities = null;
            List<List<Point3d>> p3s = new List<List<Point3d>>();
            List<Point3d> p3R = new List<Point3d>();
            List<Polyline> PL = new List<Polyline>();
            List<Entity> ls = new List<Entity>();
            ed.WriteMessage("按顺序选择多段线：");
            //SelectionFilter sf = new SelectionFilter(new TypedValue()
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
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {

                for (int i = 0; i < sSet.Count; i++)
                {
                    Entity TEntity1 = (Entity)dimObjectId[i].GetObject(OpenMode.ForWrite);
                    if (TEntity1.GetType() == typeof(Polyline3d))
                    {
                        Polyline3d polyline3D = (Polyline3d)TEntity1;
                        p3s.Add(PLpt(polyline3D));
                    }
                    if (TEntity1.GetType() == typeof(Polyline2d))
                    {
                        Polyline2d polyline3D = (Polyline2d)TEntity1;
                        p3s.Add(PLpt(polyline3D));
                    }
                    if (TEntity1.GetType() == typeof(Polyline))
                    {
                        Polyline polyline = (Polyline)TEntity1;
                        p3s.Add(PLpt(polyline));
                    }
                    if (TEntity1.GetType() == typeof(Arc))
                    {
                        Arc arc = (Arc)TEntity1;
                        p3s.Add(ARCpt(arc));
                    }
                    if (TEntity1.GetType() == typeof(Line))
                    {
                        Line line = (Line)TEntity1;
                        p3s.Add(Lpt(line));
                    }
                    //int PLnum = polyline.NumberOfVertices;
                    //for (int j = 0; j < PLnum; j++)
                    //{
                    //    p3s.Add(polyline.GetPoint3dAt(j));
                    //}
                }

                //entities = new Entity[p3s[0].Count];
                for (int i = 1; i < p3s.Count; i++)
                {
                    for (int j = 0; j < p3s[i].Count; j++)
                    {
                        ls.Add(new Line(p3s[i - 1][j], p3s[i][j]));
                    }
                }
            }
            ReBlockTable.ReBlock(db, ls.ToArray());
        }
    }
}
