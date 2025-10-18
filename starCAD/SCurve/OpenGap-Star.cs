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
using starCAD;

namespace sCAD.SCurve
{
    public class OpenGap
    {
        [CommandMethod("OpGap")]

        public void OpGap()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            ViewTableRecord vtr = ed.GetCurrentView();
            Plane plane = new Plane(Point3d.Origin, vtr.ViewDirection);

            PromptEntityResult psr1 = ed.GetEntity("选择曲线1");
            PromptEntityResult psr2 = ed.GetEntity("选择曲线2");
            //SelectionSet sSet = 
            ObjectId dimObjectId1 = psr1.ObjectId;
            ObjectId dimObjectId2 = psr2.ObjectId;
            Circle sphere = new Circle();
            Curve[] result = new Curve[3];
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {

                Entity entity1 = (Entity)trans.GetObject(dimObjectId1, OpenMode.ForWrite);
                Entity entity2 = (Entity)trans.GetObject(dimObjectId2, OpenMode.ForWrite);
                Curve curve1 = entity1 as Curve;
                Curve curve2 = entity2 as Curve;
                Point3d psr1p31 = plane.ClosestPointTo(psr1.PickedPoint);
                Point3d crvstartPoint = plane.ClosestPointTo(curve1.StartPoint);
                Point3d crvendPoint = plane.ClosestPointTo(curve1.EndPoint);
                if (psr1p31.DistanceTo(crvstartPoint) > psr1p31.DistanceTo(crvendPoint))
                {
                    curve1.ReverseCurve();
                }
                //Point3d point3D1 = curve1.GetPointAtDist(Width);
                //DBObjectCollection curve1s = curve1.GetSplitCurves(new Point3dCollection(new Point3d[] { point3D1 }));
                Point3d point3D1 = curve1.GetPointAtDist(Width);
                double point3D1P = curve1.GetParameterAtPoint(point3D1);
                DBObjectCollection curve1s = curve1.GetSplitCurves(new DoubleCollection(new double[] { point3D1P }));

                Point3d psr1p32 = psr2.PickedPoint;
                if (curve1.StartPoint.DistanceTo(curve2.StartPoint) > curve1.StartPoint.DistanceTo(curve2.EndPoint))
                {
                    curve2.ReverseCurve();
                }

                Point3d point3D2 = curve2.GetPointAtDist(Depth);
                double point3D2P = curve2.GetParameterAtPoint(point3D2);
                DBObjectCollection curve2s = curve2.GetSplitCurves(new DoubleCollection(new double[] { point3D2P }));// curve2.GetSplitCurves(new Point3dCollection(new Point3d[] { point3D2 }));
                Curve cc10 = (Curve)curve1s[0];
                Curve cc11 = (Curve)curve1s[1];
                Curve cc20 = (Curve)curve2s[0];
                Curve cc21 = (Curve)curve2s[1];
                Matrix3d matrix3D1 = Matrix3d.Displacement(cc20.EndPoint - cc20.StartPoint);
                cc10.TransformBy(matrix3D1);

                Matrix3d matrix3D2 = Matrix3d.Displacement(cc10.EndPoint - cc10.StartPoint);
                cc20.TransformBy(matrix3D2);
                result[0] = cc10;
                result[1] = cc11;
                result[2] = cc20;
                entity1.Erase(true);
                trans.Commit();
            }

            ReBlockTable.ReBlock(sphere);
            ReBlockTable.ReBlock(result);
        }

        public static double Width = 40;
        public static double Depth = 20;

        [CommandMethod("OpGapOption")]

        public void OpGapOption()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptDoubleOptions promptDoubleOptions = new PromptDoubleOptions("开缺宽度");
            promptDoubleOptions.DefaultValue = Width;
            PromptDoubleResult psr1 = ed.GetDouble(promptDoubleOptions);
            PromptDoubleResult psr2 = ed.GetDouble("开缺深度");
            Width = psr1.Value;
            Depth = psr2.Value;
        }
    }
}
