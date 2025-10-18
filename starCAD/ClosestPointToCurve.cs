using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using sCAD;
using starCAD;

namespace sCAD
{
    public class ClosestPointToCurve
    {
        [CommandMethod("ZJD", CommandFlags.UsePickSet)]//"DimCV")]

        public void ZJD()
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
            SelectionSet sSet = psr.Value;
            ObjectId[] dimObjectId = sSet.GetObjectIds();
            //Entity[] entities = new Entity[dimObjectId.Length];
            //bool flag = true;
            Entity TEntity1 = null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                TEntity1 = (Entity)dimObjectId[0].GetObject(OpenMode.ForWrite);
            }
            Line result = new Line();
            PromptPointOptions ppr = new PromptPointOptions("\r\n选择最近点");
            while (true)
            {
                PromptPointResult promptPointResult = ed.GetPoint(ppr);
                if (promptPointResult.Status == PromptStatus.Cancel) return;
                if (promptPointResult.Status == PromptStatus.None) return;
                Point3d closestpoint = promptPointResult.Value;
                Point3d zuijindian = new Point3d();
                if (TEntity1.GetType() == typeof(Line))
                {
                    Line line = (Line)TEntity1;
                    zuijindian = line.GetClosestPointTo(closestpoint, false);
                    result = new Line(closestpoint, zuijindian);
                    ed.WriteMessage(result.Length.ToString());
                    ReBlockTable.ReBlock(result);
                }
                else if (TEntity1.GetType() == typeof(Arc))
                {
                    Arc arc = (Arc)TEntity1;
                    zuijindian = arc.GetClosestPointTo(closestpoint, false);
                    result = new Line(closestpoint, zuijindian);
                   ed.WriteMessage(result.Length.ToString());
                    ReBlockTable.ReBlock(result);
                }
                else if (TEntity1.GetType() == typeof(Circle))
                {
                    Circle circle = (Circle)TEntity1;
                    zuijindian = circle.GetClosestPointTo(closestpoint, false);
                    result = new Line(closestpoint, zuijindian);
                    ed.WriteMessage(result.Length.ToString());
                    ReBlockTable.ReBlock(result);
                }
                else if (TEntity1.GetType() == typeof(Polyline))
                {
                    Polyline polyline = (Polyline)TEntity1;
                    zuijindian = polyline.GetClosestPointTo(closestpoint, false);
                    result = new Line(closestpoint, zuijindian);
                   ed.WriteMessage(result.Length.ToString());
                    ReBlockTable.ReBlock(result);
                }
                else if (TEntity1.GetType() == typeof(Polyline2d))
                {
                    Polyline2d polyline2d = (Polyline2d)TEntity1;
                    zuijindian = polyline2d.GetClosestPointTo(closestpoint, false);
                    result = new Line(closestpoint, zuijindian);
                    ed.WriteMessage(result.Length.ToString());
                    ReBlockTable.ReBlock(result);
                }
                else if (TEntity1.GetType() == typeof(Polyline3d))
                {
                    Polyline3d polyline3d = (Polyline3d)TEntity1;
                    zuijindian = polyline3d.GetClosestPointTo(closestpoint, false);
                    result = new Line(closestpoint, zuijindian);
                    ed.WriteMessage(result.Length.ToString());
                    ReBlockTable.ReBlock(result);
                }
                else if (TEntity1.GetType() == typeof(Spline))
                {
                    Spline spline = (Spline)TEntity1;
                    zuijindian = spline.GetClosestPointTo(closestpoint, false);
                    result = new Line(closestpoint, zuijindian);
                    ed.WriteMessage(result.Length.ToString());
                    ReBlockTable.ReBlock(result);
                }
            }
        }
    }
}
