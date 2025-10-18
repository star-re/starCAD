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


namespace sCAD.SCurve
{
    public class kaicaoClass
    {
        [CommandMethod("KaiCao", CommandFlags.UsePickSet)]

        public void KaiCao()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult psr = ed.SelectImplied();
            if (psr.Status != PromptStatus.OK)
            {
                psr = ed.GetSelection();
            }
            if (psr.Status == PromptStatus.Cancel) return;
            if (psr.Status == PromptStatus.None) return;
            if (psr.Status != PromptStatus.OK) return;
            SelectionSet sSet = psr.Value;
            ObjectId[] crvObjectId = sSet.GetObjectIds();
            List<DBPoint> p3s = new List<DBPoint>();
            Solid3d solid3Ds = new Solid3d();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {

                for (int i = 0; i < sSet.Count; i++)
                {
                    Entity TEntity1 = (Entity)crvObjectId[i].GetObject(OpenMode.ForWrite);
                    if (TEntity1.GetType() == typeof(Curve))
                    {
                        Curve curve1 = TEntity1 as Curve;
                        Solid3d solid3D = CreateSphere(40);
                        Move(solid3D, new Point3d(0, 0, 0));
                        Point3dCollection points = new Point3dCollection();
                        DBPoint point3D = new DBPoint(curve1.GetPointAtDist(0));

                        p3s.Add(point3D);
                        solid3Ds = solid3D;
                        // dBObject = sphere.IntersectWith()
                        //Entity entity = sphere;
                        //curve1.IntersectWith(sphere, Intersect.ExtendBoth, points, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
            //ReBlockTable.ReBlock(p3s[0]);
            ReBlockTable.ReBlock(solid3Ds);
        }

        private Solid3d CreateSphere(double radius)
        {
            Solid3d solid3D = new Solid3d();
            solid3D.CreateSphere(radius);
            return solid3D;
        }

        private void Move(Entity entity, Point3d point)
        {
            Matrix3d matrix = Matrix3d.Displacement(point - new Point3d());
            entity.TransformBy(matrix);
        }
    }
}
