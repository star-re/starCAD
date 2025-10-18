using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using starCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sCAD
{

    public class setObjectDrawLine
    {
        [CommandMethod("SLL", CommandFlags.UsePickSet)]



        public void SLL()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            //Entity[] entities = null;
            List<Point3d> p3s = new List<Point3d>();
            List<Point3d> p3R = new List<Point3d>();
            List<Line> ls = new List<Line>();
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
                    p3s.Clear();
                    Entity TEntity1 = (Entity)dimObjectId[i].GetObject(OpenMode.ForWrite);
                    Plane pl = TEntity1.GetPlane();
                    Extents3d e = (Extents3d)TEntity1.Bounds;
                    p3s.Add(e.MaxPoint);
                    p3s.Add(e.MinPoint);
                    p3R.Add(starMathdy.Pointaverage(p3s));
                }
            }
            JigJig jig = new JigJig(new Point3d(), (p, e1, e2) =>
            {
                //entities = new Entity[p3R.Count];
                List<Entity> entities = new List<Entity>();
                for (int i = 0; i < p3R.Count; i++)
                {
                    e1.Add(new Line(p3R[i], p));
                }
                //e1.Add(entities[0]);
            }, null, "\r\n需放置的点");
        }
    }
}
