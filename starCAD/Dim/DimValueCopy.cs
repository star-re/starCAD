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
using System.Text.RegularExpressions;

namespace starCAD
{
    public class Zi
    {
        public static string SS = "\r\n";
        public static string CC = "2";
        public static string FF = "|";
        public static string HH = FF;

        public static string GetString(Entity entity)
        {
            string ss = string.Empty;
            if (entity.GetType() == typeof(DBText))
            {
                DBText dB = entity as DBText;
                ss = dB.TextString;
            }
            else if (entity.GetType() == typeof(MText))
            {
                MText dB = entity as MText;
                ss = dB.Text;
            }
            else
            {
                Dimension dimension1 = entity as Dimension;
                string Men1 = dimension1.Measurement.ToString();
                string input = dimension1.DimensionText;

                string pattern = @"{\\f\w+(\|[^;]*)*;([^}]*)}([^{,]+)";
                string DimT1 = Regex.Replace(input, pattern, "$2$3");

                if (DimT1 == string.Empty)
                {
                    if (entity.Drawable.GetType() == typeof(LineAngularDimension2))
                    {
                        ss = starMathdy.Dreeges(dimension1.Measurement).ToString();
                    }
                    else
                    {
                        ss = Men1;
                    }
                }
                else
                {
                    ss = DimT1;
                }
            }

            return ss;
        }
    }

    public class DimValueCopy
    {

        [CommandMethod("DimVCo")]
        public void DimVCo()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            string promptstr = "\n [横(H)/竖(S)/组数量(C)/分隔符(F)]:";
            //PromptSelectionOptions pso = new PromptSelectionOptions();
            string fenge = Zi.SS;
            int ArrayCount = int.Parse(Zi.CC);
            bool isC = true;
            while (isC)
            {
                //PromptEntityResult promptEntityOptions = ed.GetEntity(ed.GetSelectEntityOp2(promptstr, new string[] { "H", "S", "C", "F" }));
                PromptResult pr = ed.GetKeywords(ed.GetSelectOp2(promptstr, new string[] { "H", "S", "C", "F" }));
                if (pr.Status == PromptStatus.Cancel) { isC = false; }
                if (pr.Status == PromptStatus.None) { isC = false; }
                switch (pr.StringResult)
                {
                    case "H":
                        fenge = Zi.HH;
                        isC = false;
                        break;
                    case "S":
                        fenge = Zi.SS;
                        isC = false;
                        break;
                    case "C":
                        PromptResult Countstr = ed.GetString("\n 请输入需求的一组数量：");
                        ArrayCount = int.Parse(Countstr.StringResult);
                        Zi.CC = ArrayCount.ToString();
                        break;
                    case "F":
                        PromptResult Fenge = ed.GetString("\n 请输入分隔符：");
                        fenge = Fenge.StringResult;
                        Zi.FF = fenge;
                        Zi.HH = Zi.FF;
                        break;
                }

                // pr = ed.GetString(ed.GetSelectOp2("\n 横[(H)]", new string[] { "H" }));
            }
            PromptSelectionResult psr = ed.GetSelection();

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
                    List<string> copyDimStr = new List<string>();
                    string copyDim = string.Empty;
                    for (int i = 0; i < dimObjectId.Length; i++)
                    {
                        entities[i] = (Entity)dimObjectId[i].GetObject(OpenMode.ForRead);
                        copyDimStr.Add(Zi.GetString(entities[i]));
                        //Dimension dimension1 = (Dimension)entities[i];


                        //string Men1 = dimension1.Measurement.ToString();
                        //string DimT1 = dimension1.DimensionText;

                        //if (DimT1 == string.Empty)
                        //{
                        //    if (entities[i].Drawable.GetType() == typeof(LineAngularDimension2))
                        //    {
                        //        copyDimStr.Add(starMathdy.Dreeges(dimension1.Measurement).ToString());
                        //    }
                        //    else
                        //    {
                        //        copyDimStr.Add(Men1);
                        //    }
                        //}
                        //else
                        //{
                        //    copyDimStr.Add(DimT1);
                        //}
                    }


                    //...............................
                    int fengeflag = 1;
                    List<string> fengestrArray = new List<string>();
                    List<string> copyDimResult = new List<string>();
                    if (fenge != "\r\n")
                    {

                        for (int i = 0; i < copyDimStr.Count - 1; i++)
                        {
                            if (fengeflag != ArrayCount)
                            {
                                fengeflag++;
                                fengestrArray.Add(fenge);
                            }
                            else
                            {
                                fengeflag = 1;
                                fengestrArray.Add(Zi.SS);
                            }
                        }


                        for (int i = 0; i < copyDimStr.Count; i++)
                        {
                            copyDimResult.Add(copyDimStr[i]);
                            if (i != fengestrArray.Count)
                            {
                                copyDimResult.Add(fengestrArray[i]);
                            }
                        }
                        copyDim = String.Concat(copyDimResult);
                    }
                    else
                    {
                        copyDim = String.Join(fenge, copyDimStr);
                    }
                    Clipboard.SetDataObject(copyDim);
                }
            }
        }
    }
}
