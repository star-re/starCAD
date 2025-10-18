using System;
using System.IO;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

using Excel = Microsoft.Office.Interop.Excel;
using AutoCAD;

namespace starCAD
{
    public class ReplaceOleValue
    {
        [CommandMethod("ReplaceOLEValue")]

        public void ReplaceOLEValue()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            try
            {
                // 选择OLE对象
                PromptEntityResult per = ed.GetEntity("\n选择Excel OLE对象: ");
                if (per.Status != PromptStatus.OK) return;

                // 获取替换参数
                PromptResult prSearch = ed.GetString("\n输入要查找的文本: ");
                PromptResult prReplace = ed.GetString("\n输入替换后的文本: ");

                // 通过COM直接操作
                ModifyOleContentDirectly(per.ObjectId, prSearch.StringResult, prReplace.StringResult);

                ed.WriteMessage("\nOLE内容修改完成！");
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage($"\n错误: {ex.Message}");
            }
        }

        private dynamic _acadApp;
        private void ModifyOleContentDirectly(ObjectId oleId, string searchText, string replaceText)
        {
            // 获取AutoCAD COM接口
            //dynamic acadApp = Marshal.GetActiveObject("AutoCAD.AcadApplication");
            try
            {
                _acadApp = Marshal.GetActiveObject("AutoCAD.Application");
            }
            catch
            {
                //_acadApp = new AcadApplicationClass();
                _acadApp.Visible = true;
            }
            dynamic acadDoc = _acadApp.ActiveDocument;
            dynamic acadOle = acadDoc.HandleToObject(oleId.Handle.ToString());

            // 通过COM激活OLE对象
            acadOle.Activate();

            // 获取Excel实例
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;

            try
            {
                excelApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
                workbook = excelApp.ActiveWorkbook;
                Excel.Worksheet worksheet = workbook.Sheets[1];

                // 执行替换操作
                Excel.Range usedRange = worksheet.UsedRange;
                object[,] values = (object[,])usedRange.Value2;

                for (int i = 1; i <= values.GetLength(0); i++)
                {
                    for (int j = 1; j <= values.GetLength(1); j++)
                    {
                        if (values[i, j] != null &&
                            values[i, j].ToString().Contains(searchText))
                        {
                            values[i, j] = values[i, j].ToString().Replace(searchText, replaceText);
                        }
                    }
                }

                // 批量写回修改
                usedRange.Value2 = values;

                // 更新OLE显示
                acadOle.Update();
                workbook.Save();
            }
            finally
            {
                // 释放资源
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);
                Marshal.ReleaseComObject(acadOle);
                Marshal.ReleaseComObject(acadDoc);
                Marshal.ReleaseComObject(_acadApp);
            }
        }
    }
}