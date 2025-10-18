using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Windows.Forms;
using sCAD;
using Autodesk.AutoCAD.Geometry;

namespace sCAD
{
    public class OlePatch
    {
        [CommandMethod("OlePatchObject")]
        public void OlePatchObject()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult psr = ed.GetSelection();


            if (psr.Status == PromptStatus.Cancel) return;
            if (psr.Status == PromptStatus.None) return;
            //PromptSelectionResult psr = ed.GetSelection();
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet sSet = psr.Value;
                ObjectId[] dimObjectId = sSet.GetObjectIds();
                Ole2Frame ole2Frame = new Ole2Frame();
                
                Entity[] entitys = new Entity[2];
                Entity entity;
                int index = 0;
                if (dimObjectId.Length != 2) return;

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    for (int i = 0; i <= 1; i++)
                    {
                        entitys[i] = (Entity)dimObjectId[i].GetObject(OpenMode.ForWrite);
                        if (entitys[i].GetType() == typeof(Ole2Frame))
                        {
                            ole2Frame = entitys[i] as Ole2Frame;
                            index = i;
                        }
                    }
                    if (index == 0)
                    {
                        entity = entitys[1];
                    }
                    else
                    {
                        entity = entitys[0];
                    }

                    double olewid = ole2Frame.WcsWidth;
                    Extents3d e = (Extents3d)entity.Bounds;
                    double objectwid = e.MaxPoint.X - e.MinPoint.X;
                    ole2Frame.ScaleWidth = objectwid / olewid * ole2Frame.ScaleWidth;

                    Extents3d oe = (Extents3d)ole2Frame.Bounds;
                    Point3d oeP1 = new Point3d(oe.MinPoint.X, oe.MaxPoint.Y, oe.MinPoint.Z);
                    Point3d eP1 = e.MinPoint;
                    Vector3d vector3D =  eP1- oeP1;
                    Matrix3d matrix3D = Matrix3d.Displacement(vector3D);
                    ole2Frame.TransformBy(matrix3D);

                    trans.Commit();
                }


            }
        }
    }
}
