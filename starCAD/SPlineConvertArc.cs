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
    public class SPlineConvertArc
    {
        [CommandMethod("YZH", CommandFlags.UsePickSet)]

        public void YZH()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            Entity[] result = null;
            Entity[] Refresult = null;
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
                Entity[] entities = new Entity[dimObjectId.Length];
                for (int i = 0; i < dimObjectId.Length; i++)
                {
                    Entity entity = (Entity)trans.GetObject(dimObjectId[i], OpenMode.ForWrite);
                    if (entity.GetType() == typeof(Spline))
                    {
                        Spline sp = (Spline)entity;
                        CircularArc3d circularArc3D = new CircularArc3d(sp.StartPoint, sp.GetPointAtParameter(0.5), sp.EndPoint);
                        Arc arc = new Arc(circularArc3D.Center, circularArc3D.Radius, circularArc3D.StartAngle, circularArc3D.EndAngle);
                        Point3d ArcCenter = arc.Center;
                        Arc ArcresultA = new Arc(circularArc3D.Center, circularArc3D.Radius, ActiveAngle(circularArc3D.StartPoint, arc.Center), ActiveAngle(circularArc3D.EndPoint, arc.Center));
                        Arc ArcresultB = new Arc(circularArc3D.Center, circularArc3D.Radius, ActiveAngle(circularArc3D.EndPoint, arc.Center), ActiveAngle(circularArc3D.StartPoint, arc.Center));
                        Point3d p3a = ArcresultA.GetPointAtParameter((ArcresultA.StartParam+ArcresultA.EndParam) / 2);
                        Point3d p3b = ArcresultB.GetPointAtParameter((ArcresultB.StartParam + ArcresultB.EndParam) / 2);
                        Point3d spMid = sp.GetPointAtParameter((sp.StartParam + sp.EndParam) / 2);
                        if (p3a.DistanceTo(spMid) > p3b.DistanceTo(spMid))
                        {
                            result = new Entity[] { ArcresultB };
                           Refresult = new Entity[] { entity };
                        }
                        else
                        {
                            result = new Entity[] { ArcresultA };
                            Refresult = new Entity[] { entity };
                        }
                    }
                }
            }
            ReBlockTable.ReBlock(result, Refresult);
        }

        private static double ActiveAngle(Point3d p3a, Point3d p3b)
        {
            double angle = double.NaN;
            Vector3d vector3D = p3a - p3b;
            angle = Vector3d.XAxis.GetAngleTo(vector3D, Vector3d.XAxis);
            return angle;
        }
    }
}
