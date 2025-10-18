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

namespace sCAD.Dim
{
    public class _3DDynamicDimClass
    {
        [CommandMethod("DynamicDimLinear")]

        public void DynamicDimLinear()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            ObjectId CameraId = db.ViewportTableId;
            ViewTableRecord viewTableRecord = ed.GetCurrentView();
            Point3d centerPoint = viewTableRecord.Target;
            //Circle circle = new Circle(centerPoint, viewTableRecord.ViewDirection, 50);
            //ReBlockTable.ReBlock(circle);
            //Plane plane = new Plane(db.Chamfera, db.Chamferb, db.Chamferc, db.Chamferd);
            PromptPointResult ppr1 = ed.GetPoint("选取第一点");
            if (ppr1.Status == PromptStatus.Cancel) return;
            if (ppr1.Status == PromptStatus.None) return;
            if (ppr1.Status != PromptStatus.OK) return;
            PromptPointResult ppr2 = ed.GetPoint("选取第二点");
            if (ppr2.Status == PromptStatus.Cancel) return;
            if (ppr2.Status == PromptStatus.None) return;
            if (ppr2.Status != PromptStatus.OK) return;

            Point3d point3D1 = Point3d.Origin;
            Point3d point3D2 = Point3d.Origin;
            if (ed.CurrentUserCoordinateSystem == Matrix3d.Identity)
            {
                point3D1 = ppr1.Value;
                point3D2 = ppr2.Value;
            }
            else
            {
                point3D1 = ppr1.Value.TransformBy(ed.CurrentUserCoordinateSystem);
                point3D2 = ppr2.Value.TransformBy(ed.CurrentUserCoordinateSystem);
            }

            Point3d TransPoint = new Point3d(100000, 100000, 100000);
            Vector3d Trans =   TransPoint - point3D1;
            point3D1 = point3D1.TransformBy(Matrix3d.Displacement(Trans));
            point3D2 = point3D2.TransformBy(Matrix3d.Displacement(Trans));

            Vector3d Vx = point3D2 - point3D1;
            Vector3d Vz = viewTableRecord.ViewDirection;
            Vector3d Vy = Vz.CrossProduct(Vx);
            Vy = Vx.CrossProduct(Vy);
            Vy = sCAD.starMathdy.Unitize(Vy);
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            ObjectId dimensionStyle = doc.Database.Dimstyle;
            AlignedDimension dal = new AlignedDimension(point3D2, point3D1, point3D1, "", dimensionStyle);
            //dal.XLine1Point = ppr2.Value;
            //dal.XLine2Point = ppr1.Value;
            dal.Normal = Vy;
            dal.DimLinePoint = point3D1;
            dal.DimensionStyle = dimensionStyle;
            Plane pl = dal.GetPlane();
            double len1 = pl.DistanceTo(point3D1);
            dal.Elevation = -len1;
            dal.TransformBy(Matrix3d.Displacement(-Trans));
            //RotatedDimension dli = new RotatedDimension(0, ppr1.Value, ppr2.Value, ppr1.Value, "", dimensionStyle);

            //dal.DimLinePoint = sCAD.starMathdy.Pointaverage(new List<Point3d>() { ppr1.Value, ppr2.Value });
            ReBlockTable.ReBlock(dal);
        }

    }
}
