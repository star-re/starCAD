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

namespace starCAD
{
    public class DimCommonstring
    {
        public static string AA = "弧长A1,半径A2,弦长A3,弦高A4";
        public static string BB = "弧长B1,半径B2,弦长B3,弦高B4";
        public static string CC = "变量";
        public static string DD = "弧长D1,半径D2,弦长D3,弦高D4";
        public static string HF = "⌒";
        public static string FF = "";
        public static string HH = FF;
    }

    public class DimCommonlyCommon
    {
        public string thisDimValue = string.Empty;

        [CommandMethod("DimCommonly", CommandFlags.UsePickSet)]//"DimCV")]
        public void DimCommonly()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            bool flag0 = true;
           // string DimValue = string.Empty;
            PromptResult pr = null;


            string promptstr = "\n需要常用标注 [弧(A)/弧B(B)/变量(C)/弧D(D)/弧(H)/自定义文本(T)/前缀&后缀(P)]:";
            //PromptSelectionOptions pso = new PromptSelectionOptions();
            string DimValue = DimCommonstring.AA;   
            bool isC = true;
            while (isC)
            {
                PromptResult ppr = ed.GetKeywords(ed.GetSelectOp2(promptstr, new string[] { "A", "B", "C", "D", "H", "T", "P" }));
                if (ppr.Status == PromptStatus.Cancel) { isC = false; }
                if (ppr.Status == PromptStatus.None) { isC = false; }
                switch (ppr.StringResult)
                {
                    case "A":
                        DimValue = DimCommonstring.AA;
                        isC = false;
                        break;
                    case "B":
                        DimValue = DimCommonstring.BB;
                        isC = false;
                        break;
                    case "C":
                        DimValue = DimCommonstring.CC;
                        isC = false;
                        break;
                    case "D":
                        DimValue = DimCommonstring.DD;
                        isC = false;
                        break;
                    case "H":
                        DimValue = DimCommonstring.HF;
                        isC = false;
                        break;
                    case "T":
                        PromptResult Fenge = ed.GetString("\n 请输入前缀&后缀");
                        DimValue = Fenge.StringResult;
                        DimCommonstring.FF = DimValue;
                        DimCommonstring.HH = DimCommonstring.FF;
                        isC = false;
                        break;
                    case "P":
                        //PromptResult Fenge = ed.GetString("\n 请输入分隔符：");
                        //fenge = Fenge.StringResult;
                        //DimqianzhuiZhi.FF = fenge;
                        //DimqianzhuiZhi.HH = DimqianzhuiZhi.FF;
                        break;
                }


                /***********************************************/
                bool flag1 = true;
                while (flag1)
                {
                    PromptSelectionResult psr = ed.SelectImplied();
                    if (psr.Status != PromptStatus.OK)
                    {
                        psr = ed.GetSelection();
                    }
                    if (psr.Status == PromptStatus.Cancel) return;
                    if (psr.Status == PromptStatus.None) return;


                    //PromptSelectionResult psr = ed.GetSelection();
                    if (psr.Status == PromptStatus.OK)
                    {
                        SelectionSet sSet = psr.Value;
                        ObjectId[] dimObjectId = sSet.GetObjectIds();

                        using (Transaction trans = db.TransactionManager.StartTransaction())
                        {
                            Entity[] entities = new Entity[dimObjectId.Length];
                            for (int i = 0; i < dimObjectId.Length; i++)
                            {
                                Entity dimEntity1 = (Entity)dimObjectId[i].GetObject(OpenMode.ForWrite);
                                Dimension dimension1 = (Dimension)dimEntity1;
                                dimension1.DimensionText = DimValue;
                            }
                            trans.Commit();
                        }
                    }
                }
            }
        }
    }
}
