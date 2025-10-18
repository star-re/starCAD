//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Autodesk.AutoCAD.ApplicationServices;
//using Autodesk.AutoCAD.DatabaseServices;
//using Autodesk.AutoCAD.EditorInput;
//using Autodesk.AutoCAD.Geometry;
//using Autodesk.AutoCAD.Internal;
//using Autodesk.AutoCAD.Runtime;
//using starCAD;
//using aai = Autodesk.AutoCAD.Interop;
//using aaic = Autodesk.AutoCAD.Interop.Common;

//namespace sCAD
//{
//    public class AddJiaoMa
//    {
//        [CommandMethod("JiaoMa")]


//        public void JiaoMa()
//        {
//            Document doc = Application.DocumentManager.MdiActiveDocument;
//            Database db = doc.Database;
//            Editor ed = doc.Editor;

//            try
//            {
//                // 1. 选择基线
//                PromptSelectionResult psr = ed.SelectImplied();
//                if (psr.Status != PromptStatus.OK)
//                {
//                    psr = ed.GetSelection();
//                }
//                if (psr.Status == PromptStatus.Cancel) return;
//                if (psr.Status == PromptStatus.None) return;
//                if (psr.Status != PromptStatus.OK) return;
//                SelectionSet sSet = psr.Value;
//                ObjectId[] dimObjectId = sSet.GetObjectIds();
//                using (Transaction trans = db.TransactionManager.StartTransaction())
//                {
//                    Curve baseCurve = (Curve)dimObjectId[i].GetObject(OpenMode.ForWrite);
//                    //Curve baseCurve = SelectBaseCurve(ed);
//                    if (baseCurve == null) return;

//                    // 2. 获取用户输入
//                    double startOffset = ed.GetDouble("请输入起点偏移量：").Value;
//                    double endOffset = ed.GetDouble("请输入终点偏移量：").Value;
//                    double sphereRadius = ed.GetDouble("请输入球体半径：").Value;

//                    // 3. 计算有效段
//                    Curve3d c3 = baseCurve.GetGeCurve();
//                    double totalLength = c3.GetLength(c3.GetParameterOf(c3.StartPoint), c3.GetParameterOf(c3.EndPoint), 0.001);
//                    if (!ValidateOffsets(startOffset, endOffset, totalLength))
//                    {
//                        ed.WriteMessage("\n错误：偏移量总和超过曲线长度！");
//                        return;
//                    }
//                    // 4. 生成球体
//                    List<Point3d> positions = CalculatePositions(baseCurve, startOffset, endOffset);
//                    CreateSpheres(db,trans, positions, sphereRadius);
//                    trans.Commit();
//                }

//            }
//            catch (System.Exception ex)
//            {
//                ed.WriteMessage($"\n错误：{ex.Message}");
//            }
//        }

//        private static bool ValidateOffsets(double start, double end, double totalLength)
//        {
//            return (start + end) < totalLength;
//        }

//        private static List<Point3d> CalculatePositions(Curve curve, double startOffset, double endOffset)
//        {
//            List<Point3d> points = new List<Point3d>();
//            Curve3d c3 = curve.GetGeCurve();
//            double totalLength = curve.GetLength();
//            double effectiveLength = totalLength - startOffset - endOffset;

//            // 计算三个均分位置（包含起点/终点偏移位置）
//            double[] percentages = { 0.0, 0.5, 1.0 };
//            foreach (double pct in percentages)
//            {
//                double position = startOffset + (effectiveLength * pct);
//                points.Add(curve.GetPointAtDist(position));
//            }

//            return points;
//        }

//        private static void CreateSpheres(Database db,Transaction tr, List<Point3d> centers, double radius)
//        {
//            BlockTableRecord modelSpace = tr.GetObject(
//                SymbolUtilityServices.GetBlockModelSpaceId(db),
//                OpenMode.ForWrite) as BlockTableRecord;

//            foreach (Point3d center in centers)
//            {
//                using (Solid3d sphere = new Solid3d())
//                {
//                    sphere.CreateSphere(radius);
//                    sphere.TransformBy(Matrix3d.Displacement(center.GetAsVector()));
//                    modelSpace.AppendEntity(sphere);
//                    tr.AddNewlyCreatedDBObject(sphere, true);
//                }
//            }
//        }
//    }
//}
