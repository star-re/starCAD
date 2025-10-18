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
    public class Orient_Rhino
    {

        [CommandMethod("Orient", CommandFlags.UsePickSet)]
        public void Orient()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            List<Point3d> Vector3d1 = new List<Point3d>();
            List<Point3d> Vector3d2 = new List<Point3d>();
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
            Entity[] entities = new Entity[dimObjectId.Length];
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                for (int i = 0; i < dimObjectId.Length; i++)
                {
                    entities[i] = (Entity)dimObjectId[i].GetObject(OpenMode.ForWrite);
                }
            }
            PromptPointOptions ppo = new PromptPointOptions("参考点1");
            PromptPointResult ppr = ed.GetPoint("参考点1");
            bool flag0 = true;
            bool flag1 = true;
            bool flag2 = true;
            bool flag3 = true;

            while (flag0)
            {
                if (ppr.Status == PromptStatus.OK)
                {
                    Vector3d1.Add(ppr.Value);
                    flag0 = false;
                }
                if (ppr.Status == PromptStatus.Cancel) return;
                if (ppr.Status == PromptStatus.None) return;
                if (ppr.Status != PromptStatus.OK) return;
            }
            while (flag1)
            {
                ppr = ed.GetPoint("参考点2");
                if (ppr.Status == PromptStatus.OK)
                {
                    Vector3d1.Add(ppr.Value);
                    flag1 = false;
                }
                if (ppr.Status == PromptStatus.Cancel) return;
                if (ppr.Status == PromptStatus.None) return;
                if (ppr.Status != PromptStatus.OK) return;
            }
            while (flag2)
            {
                ppr = ed.GetPoint("目标点1<参考点1>");
                if (ppr.Status == PromptStatus.OK)
                {
                    Vector3d2.Add(ppr.Value);
                    flag2 = false;
                }
                if (ppr.Status == PromptStatus.Cancel) return;
                if (ppr.Status == PromptStatus.None) return;
                if (ppr.Status != PromptStatus.OK) return;
            }
            //while (flag3)
            //{
            //    ppr = ed.GetPoint("目标点2<参考点2>");
            //    if (ppr.Status == PromptStatus.OK)
            //    {
            //        Vector3d2.Add(ppr.Value);
            //        flag3 = false;
            //    }
            //    if (ppr.Status == PromptStatus.Cancel) return;
            //    if (ppr.Status == PromptStatus.None) return;
            //    if (ppr.Status != PromptStatus.OK) return;
            //}


            Point3d P3a1 = new Point3d(Vector3d1[0].X, Vector3d1[0].Y, Vector3d1[0].Z);
            Point3d P3a2 = new Point3d(Vector3d1[1].X, Vector3d1[1].Y, Vector3d1[1].Z);
            Point3d P3b1 = new Point3d(Vector3d2[0].X, Vector3d2[0].Y, Vector3d2[0].Z);
            //Point3d P3b2 = new Point3d(Vector3d2[1].X, Vector3d2[1].Y, Vector3d2[1].Z);
            sCADDrawJig scadrawJig = new sCADDrawJig(entities.ToList(), P3b1, P3a1, P3a2, "目标点2<参考点2>");
            PromptResult promptResult = ed.Drag(scadrawJig);
            //using (Transaction trans = db.TransactionManager.StartTransaction())
            //{

            //    //JigPrompts jigPrompts = 
            //    Point3d P3a1 = new Point3d(Vector3d1[0].X, Vector3d1[0].Y, Vector3d1[0].Z);
            //    Point3d P3a2 = new Point3d(Vector3d1[1].X, Vector3d1[1].Y, Vector3d1[1].Z);
            //    Point3d P3b1 = new Point3d(Vector3d2[0].X, Vector3d2[0].Y, Vector3d2[0].Z);
            //    Point3d P3b2 = new Point3d(Vector3d2[1].X, Vector3d2[1].Y, Vector3d2[1].Z);
            //    sCADDrawJig scadrawJig = new sCADDrawJig(entities.ToList(), P3b1, P3a1, P3a2, "目标点2<参考点2>");
            //    PromptResult promptResult = ed.Drag(scadrawJig);

                //Vector3d vector3D1 = P3a2 - P3a1;
                //vector3D1 = starMathdy.Unitize(vector3D1);
                //Vector3d angleY3d1 = vector3D1.RotateBy(Math.PI * 0.5, Vector3d.ZAxis);
                //Vector3d vector3D2 = P3b2 - P3b1;
                //vector3D2 = starMathdy.Unitize(vector3D2);
                //Vector3d angleY3d2 = vector3D2.RotateBy(Math.PI * 0.5, Vector3d.ZAxis);
                //Matrix3d matrix3D = Matrix3d.AlignCoordinateSystem(P3a1, vector3D1, angleY3d1, Vector3d.ZAxis, P3b1, vector3D2, angleY3d2, Vector3d.ZAxis);



                //for (int i = 0; i < sSet.Count; i++)
                //{
                //    Entity TEntity1 = (Entity)dimObjectId[i].GetObject(OpenMode.ForWrite);
                //    TEntity1.TransformBy(matrix3D);
                //    entities[i] = TEntity1;
                //}
                //trans.Commit();
            }
        }
    }
