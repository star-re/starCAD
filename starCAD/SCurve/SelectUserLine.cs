using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;

namespace SLineCommand
{
    public class SLineCommand
    {
        [CommandMethod("SLine", CommandFlags.UsePickSet)]
        public void SelectParallelLines()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            try
            {
                // 步骤1：获取选择集（支持预选对象）
                SelectionSet selSet = GetSelection(ed);
                if (selSet == null) return;

                // 步骤2：选择参考轴（X/Y）
                PromptKeywordOptions axisOpts = new PromptKeywordOptions("\n选择参考轴 [X轴/Y轴]: ")
                {
                    AllowNone = false,
                    Keywords = { "X轴", "Y轴" },
                    AppendKeywordsToMessage = true
                };
                PromptResult axisRes = ed.GetKeywords(axisOpts);
                if (axisRes.Status != PromptStatus.OK) return;

                Vector3d refAxis = axisRes.StringResult == "X轴" ?
                    Vector3d.XAxis : Vector3d.YAxis;

                // 步骤3：输入角度容差
                PromptDoubleOptions angleOpts = new PromptDoubleOptions("\n输入角度容差(度): ")
                {
                    AllowNegative = false,
                    AllowZero = true,
                    DefaultValue = 5.0,
                    UseDefaultValue = true
                };
                PromptDoubleResult angleRes = ed.GetDouble(angleOpts);
                if (angleRes.Status != PromptStatus.OK) return;

                double tolerance = angleRes.Value;
                double radTolerance = tolerance * Math.PI / 180.0;

                // 步骤4-5：检查平行性并收集符合条件的直线
                ObjectIdCollection parallelLines = new ObjectIdCollection();
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (SelectedObject selObj in selSet)
                    {
                        if (selObj != null)
                        {
                            Entity ent = tr.GetObject(selObj.ObjectId, OpenMode.ForRead) as Entity;
                            // 仅处理直线对象
                            if (ent is Line line)
                            {
                                Vector3d lineDir = line.EndPoint - line.StartPoint;
                                if (!lineDir.IsZeroLength())
                                {
                                    lineDir = lineDir.GetNormal();

                                    // 计算直线与参考轴的夹角
                                    double angle = lineDir.GetAngleTo(refAxis);
                                    angle = Math.Min(angle, Math.PI - angle); // 考虑反向平行

                                    if (angle <= radTolerance)
                                    {
                                        parallelLines.Add(line.ObjectId);
                                    }
                                }
                            }
                        }
                    }
                    tr.Commit();
                }

                // 步骤6：设置符合条件的直线为选择状态
                if (parallelLines.Count > 0)
                {
                    ObjectId[] parallelLineArray = new ObjectId[parallelLines.Count];
                    parallelLines.CopyTo(parallelLineArray, 0);

                    // 使用SetImpliedSelection更新选择集
                    ed.SetImpliedSelection(parallelLineArray);
                    ed.WriteMessage($"\n已选中 {parallelLines.Count} 条符合要求的直线");
                }
                else
                {
                    ed.WriteMessage("\n未找到符合条件的直线");
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\n错误: {ex.Message}");
            }
        }

        private SelectionSet GetSelection(Editor ed)
        {
            // 检查预选集
            PromptSelectionResult impliedSel = ed.SelectImplied();
            if (impliedSel.Status == PromptStatus.OK)
                return impliedSel.Value;

            // 无预选集时主动提示选择
            PromptSelectionOptions opts = new PromptSelectionOptions
            {
                MessageForAdding = "\n选择多段直线对象: ",
                AllowDuplicates = false
            };

            TypedValue[] filterList = { new TypedValue(0, "LINE") };
            SelectionFilter filter = new SelectionFilter(filterList);

            PromptSelectionResult res = ed.GetSelection(opts, filter);
            return (res.Status == PromptStatus.OK) ? res.Value : null;
        }
    }
}