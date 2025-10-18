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
using starCAD;

namespace sCAD.Dim
{
    public class ReverseDim
    {
        [CommandMethod("DimReverse")]

        public void DimReverse()
        {

            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionResult psr = ed.GetSelection();
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet sSet = psr.Value;
                ObjectId[] dimObjectId = sSet.GetObjectIds();
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    for (int i = 0; i < dimObjectId.Length; i++)
                    {
                        Entity dimEntity1 = (Entity)dimObjectId[i].GetObject(OpenMode.ForWrite);

                        //Dimension dimension1 = (Dimension)dimEntity1;
                        if (dimEntity1.GetType() == typeof(RotatedDimension))
                        {
                            RotatedDimension lineDim = (RotatedDimension)dimEntity1;

                            Point3d dimTPoint = new Point3d(lineDim.TextPosition.X, lineDim.TextPosition.Y, lineDim.TextPosition.Z);
                            Plane plane = lineDim.GetPlane();

                            Line line = new Line(lineDim.XLine1Point, lineDim.XLine2Point);
                            Point3d point3D = line.GetClosestPointTo(dimTPoint, true);
                            Vector3d vector3D = dimTPoint - point3D;
                            lineDim.TransformBy(Matrix3d.Rotation(Math.PI * 1, vector3D, dimTPoint));
                            //entities[0] = line as Entity;
                            //entities[1] = new Line(dimTPoint, point3D);
                            //plane.RotateBy(Math.PI*2)
                            //Point3d dimLPoint = new Point3d(lineDim.DimLinePoint.X, lineDim.DimLinePoint.Y, lineDim.DimLinePoint.Z);
                            //MText mText = new MText();
                            //mText.Rotation = mText.Rotation - 180;
                            //Point3d p31 = lineDim.XLine1Point;
                            //Point3d p32 = lineDim.XLine2Point;
                            //Vector3d v3 = lineDim.Normal;
                            //lineDim.
                            //lineDim.Normal = -v3;
                            //lineDim.XLine2Point = p31;
                            //lineDim.XLine1Point = p32;
                            //lineDim.TextPosition = dimTPoint;
                            //lineDim.DimLinePoint = dimLPoint;

                        }
                        if (dimEntity1.GetType() == typeof(AlignedDimension))
                        {
                            AlignedDimension lineDim = (AlignedDimension)dimEntity1;

                            Point3d dimTPoint = new Point3d(lineDim.TextPosition.X, lineDim.TextPosition.Y, lineDim.TextPosition.Z);
                            Plane plane = lineDim.GetPlane();

                            Line line = new Line(lineDim.XLine1Point, lineDim.XLine2Point);
                            Point3d point3D = line.GetClosestPointTo(dimTPoint, true);
                            Vector3d vector3D = dimTPoint - point3D;
                            lineDim.TransformBy(Matrix3d.Rotation(Math.PI * 1, vector3D, dimTPoint));
                        }
                    }
                   
                    trans.Commit();
                }
                //ReBlockTable.ReBlock(entities);
            }
        }
    }
}
