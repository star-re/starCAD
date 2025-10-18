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

namespace starCAD
{
    //此处需public，否则CAD命令行找不到此命令
    public class LineDe
    {
        [CommandMethod("ConvertToLine",CommandFlags.UsePickSet)]

        public void ConvertToLine()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionResult psr = ed.GetSelection();
            List<Line> lines = new List<Line>();
            if (psr.Status != PromptStatus.OK)
            {
                psr = ed.GetSelection();
            }
            if (psr.Status == PromptStatus.Cancel) return;
            if (psr.Status == PromptStatus.None) return;
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet sSet = psr.Value;
                ObjectId[] dimObjectId = sSet.GetObjectIds();
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    for (int i = 0; i < dimObjectId.Length; i++)
                    {
                        Line line = new Line();
                        Entity entity = (Entity)trans.GetObject(dimObjectId[i], OpenMode.ForWrite);

                        if (typeof(Polyline3d) == entity.GetType())
                        {
                            Polyline3d p3 = (Polyline3d)dimObjectId[i].GetObject(OpenMode.ForWrite);
                            line = new Line(p3.StartPoint, p3.EndPoint);
                        }
                        else if (typeof(Polyline) == entity.GetType())
                        {
                            Polyline p3 = (Polyline)dimObjectId[i].GetObject(OpenMode.ForWrite);
                            line = new Line(p3.StartPoint, p3.EndPoint);
                        }
                        else if (typeof(Polyline2d) == entity.GetType())
                        {
                            Polyline2d p3 = (Polyline2d)dimObjectId[i].GetObject(OpenMode.ForWrite);
                            line = new Line(p3.StartPoint, p3.EndPoint);
                        }
                        else if (typeof(Spline) == entity.GetType())
                        {
                            Spline p3 = (Spline)dimObjectId[i].GetObject(OpenMode.ForWrite);
                            line = new Line(p3.StartPoint, p3.EndPoint);
                        }
                        else if (typeof(Arc) == entity.GetType())
                        {
                            Arc p3 = (Arc)dimObjectId[i].GetObject(OpenMode.ForWrite);
                            line = new Line(p3.StartPoint, p3.EndPoint);
                        }
                        else
                        {
                            return;
                        }
                        line.SetPropertiesFrom(entity);
                        lines.Add(line);
                        // ReBlockTable.ReBlock(db, lines.ToArray());
                        entity.Erase();
                    }
                    trans.Commit();
                }
                ReBlockTable.ReBlock(db, lines.ToArray());
            }
        }
    }
}
