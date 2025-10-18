//using Autodesk.AutoCAD.Runtime;
//using Autodesk.AutoCAD.DatabaseServices;
//using Autodesk.AutoCAD.Geometry;
//using Autodesk.AutoCAD.EditorInput;
//using System.Collections.Generic;
//using Autodesk.AutoCAD.ApplicationServices;
//using starCAD;
//using sCAD;
//using System.Linq;
//using System;

//public class QDIMClone
//{
//    [CommandMethod("MYQDIM")]
//    public void Run()
//    {
//        Document doc = Application.DocumentManager.MdiActiveDocument;
//        Database db = doc.Database;
//        Editor ed = doc.Editor;

//        // 1. 选择标注模式
//        //PromptKeywordOptions modeOpt = new PromptKeywordOptions("\n选择标注模式 [连续(C)/基线(B)/坐标(O)]: ");
//        //modeOpt.Keywords.Add("Continuous", "C", "连续");
//        //modeOpt.Keywords.Add("Baseline", "B", "基线");
//        //modeOpt.Keywords.Add("Ordinate", "O", "坐标");
//        //modeOpt.AllowNone = true;
//        //modeOpt.AppendKeywordsToMessage = true;
//        //PromptResult modeRes = ed.GetKeywords(modeOpt);

//        // 2. 选择点集
//        PromptSelectionOptions selOpt = new PromptSelectionOptions();
//        selOpt.MessageForAdding = "\n选择要标注的点、直线或块参照: ";
//        PromptSelectionResult selRes = ed.GetSelection(selOpt);
//        if (selRes.Status != PromptStatus.OK) return;

//        using (Transaction tr = db.TransactionManager.StartTransaction())
//        {
//            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(
//                db.CurrentSpaceId, OpenMode.ForWrite);

//            // 3. 提取点坐标
//            List<Point3d> points = ExtractPoints(tr, selRes.Value);
//            PtDimLine pdl = new PtDimLine(points);
//            PromptResult result = Application.DocumentManager.MdiActiveDocument.Editor.Drag(pdl);
//            Point3d point3D = new Point3d();
//            if (result.Status == PromptStatus.OK)
//            {
//                point3D = pdl.baseDimPt;
//            }

//            // 4. 执行标注
//            //switch (modeRes.StringResult)
//            //{
//            //    case "Continuous":
//            //        CreateContinuousDims(btr, points);
//            //        break;
//            //    case "Baseline":
//            //        CreateBaselineDims(btr, points);
//            //        break;
//            //    //case "Ordinate":
//            //    //    CreateOrdinateDims(btr, points);
//            //    //    break;
//            //    default:
//            //        CreateContinuousDims(btr, points);
//            //        break;
//            //}

//            //PromptPointResult pointResult = ed.GetPoint("选择标注点");
//            CreateContinuousDims(btr, points, point3D);
//            tr.Commit();
//        }
//    }

//    private List<Point3d> ExtractPoints(Transaction tr, SelectionSet selection)
//    {
//        List<Point3d> points = new List<Point3d>();
//        foreach (SelectedObject obj in selection)
//        {
//            Entity ent = tr.GetObject(obj.ObjectId, OpenMode.ForRead) as Entity;
//            if (ent is DBPoint) points.Add(((DBPoint)ent).Position);
//            else if (ent is Line)
//            {
//                points.Add(((Line)ent).StartPoint);
//                points.Add(((Line)ent).EndPoint);
//            }
//            else if (ent is Circle)
//            {
//                points.Add(((Circle)ent).Center);
//            }
//            else if (ent is BlockReference)
//            {
//                points.Add(((BlockReference)ent).Position);
//            }
//            // 可扩展其他实体类型...
//        }
//        return points;
//    }

//    private void CreateContinuousDims(BlockTableRecord btr, List<Point3d> points, Point3d baseDimPoint)
//    {
//        double angle = 0;
//        points = point3DsSort(points, baseDimPoint, out angle);
//        for (int i = 0; i < points.Count - 1; i++)
//        {
//            RotatedDimension dim = new RotatedDimension();
//            dim.XLine1Point = points[i];
//            dim.XLine2Point = points[i + 1];
//            dim.DimLinePoint = baseDimPoint;
//            dim.Rotation = angle;
//            //dim.Rotation = GetRotationAngle(points[i], points[i + 1]);
//            ReBlockTable.ReBlock(dim);
//        }
//    }

//    public List<Point3d> point3DsSort(List<Point3d> point3Ds, Point3d baseDimPoint, out double Angle)
//    {
//        double[] Ys = point3Ds.Select(i => i.Y).ToArray();
//        Array.Sort(Ys);
//        double angleresult = 0;
//        List<Point3d> result = new List<Point3d>();
//        if (Ys[0] < baseDimPoint.Y && Ys[Ys.Length - 1] > baseDimPoint.Y)
//        {
//            angleresult = 0;
//        }
//        else
//        {
//            angleresult = 90;
//        }

//        if (angleresult == 0)
//        {
//            double[] SortYs = point3Ds.Select(i => i.Y).ToArray();
//            Point3d[] pointlinshi = point3Ds.ToArray();
//            Array.Sort(SortYs, pointlinshi);
//            result = pointlinshi.ToList();
//            angleresult = starMathdy.Radians(90);
//        }
//        else
//        {
//            double[] SortXs = point3Ds.Select(i => i.X).ToArray();
//            Point3d[] pointlinshi = point3Ds.ToArray();
//            Array.Sort(SortXs, pointlinshi);
//            result = pointlinshi.ToList();
//            angleresult = starMathdy.Radians(0);
//        }
//        Angle = angleresult;
//        return result;
//    }

//    private void CreateContinuousDims(BlockTableRecord btr, List<Point3d> points)
//    {

//        for (int i = 0; i < points.Count - 1; i++)
//        {
//            RotatedDimension dim = new RotatedDimension();
//            dim.XLine1Point = points[i];
//            dim.XLine2Point = points[i + 1];
//            dim.DimLinePoint = GetOffsetPoint(points[i], points[i + 1], 10);
//            dim.Rotation = 0;
//            //dim.Rotation = GetRotationAngle(points[i], points[i + 1]);
//            ReBlockTable.ReBlock(dim);
//        }
//    }

//    private void CreateBaselineDims(BlockTableRecord btr, List<Point3d> points)
//    {
//        if (points.Count < 2) return;
//        double offset = 10;
//        for (int i = 1; i < points.Count; i++)
//        {
//            RotatedDimension dim = new RotatedDimension();
//            dim.XLine1Point = points[0];
//            dim.XLine2Point = points[i];

//            dim.DimLinePoint = GetOffsetPoint(points[0], points[i], offset * i);
//            dim.Rotation = 0;
//            ReBlockTable.ReBlock(dim);
//        }
//    }

//    private Point3d GetOffsetPoint(Point3d p1, Point3d p2, double offset)
//    {
//        Vector3d dir = (p2 - p1).GetNormal();
//        Vector3d perp = new Vector3d(-dir.Y, dir.X, 0);
//        return p1 + (p2 - p1) * 0.5 + perp * offset;
//    }

//    private double GetRotationAngle(Point3d p1, Point3d p2)
//    {
//        return (p2 - p1).GetAngleTo(Vector3d.XAxis);
//    }
//}
