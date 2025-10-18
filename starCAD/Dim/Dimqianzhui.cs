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
    public class DimqianzhuiZhi
    {
        public static string AA = "@";
        public static string NN = "内";
        public static string HF = "⌒";
        public static string FF = "";
        public static string HH = FF;
    }

    public class Dimqianzhui
    {
        public string thisDimValue = string.Empty;
        public bool qhflag = true;//前后缀，默认前缀
        public string qianhouzhui = "前缀(P)";
        [CommandMethod("DimPost")]
        public void DimPost()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            string promptstr = "\n需要的前缀&后缀 [@(A)/内(N)/弧(H)/自定义文本(T)/"+ qianhouzhui+"]:";
            //PromptSelectionOptions pso = new PromptSelectionOptions();
            string Post = DimqianzhuiZhi.HH;
            bool isC = true;

            while (isC)
            {
                PromptResult pr = ed.GetKeywords(ed.GetSelectOp2(promptstr, new string[] { "A", "N", "H", "T", "P" }));
                if (pr.Status == PromptStatus.Cancel) { isC = false; }
                if (pr.Status == PromptStatus.None) { isC = false; }
                switch (pr.StringResult)
                {
                    case "A":
                        Post = DimqianzhuiZhi.AA;
                        isC = false;
                        break;
                    case "N":
                        Post = DimqianzhuiZhi.NN;
                        isC = false;
                        break;
                    case "H":
                        Post = DimqianzhuiZhi.HF;
                        isC = false;
                        break;
                    case "T":
                        PromptResult Fenge;
                        if (thisDimValue != string.Empty)
                        {
                            Fenge = ed.GetString("\n 请输入前缀&后缀 | " + thisDimValue);
                        }
                        else
                        {
                            Fenge = ed.GetString("\n 请输入前缀&后缀");
                        }
                        if (Fenge.StringResult == string.Empty)
                        {
                            DimqianzhuiZhi.FF = thisDimValue;
                            DimqianzhuiZhi.HH = DimqianzhuiZhi.FF;
                        }
                        else
                        {
                            Post = Fenge.StringResult;
                            DimqianzhuiZhi.FF = Post;
                            DimqianzhuiZhi.HH = DimqianzhuiZhi.FF;
                            thisDimValue = Post;
                        }
                        isC = false;
                        break;
                    case "P":
                        string 前缀后缀 = "\n [前缀(Q)/后缀(H)]:";
                        bool qianzhouzhuiflag = true;
                        while (qianzhouzhuiflag)
                        {
                            PromptResult prqianzhui = ed.GetKeywords(ed.GetSelectOp2(前缀后缀, new string[] { "Q", "H" }));
                            if (prqianzhui.Status == PromptStatus.Cancel) { qianzhouzhuiflag = false; }
                            if (prqianzhui.Status == PromptStatus.None) { qianzhouzhuiflag = false; }
                            switch (prqianzhui.StringResult)
                            {
                                case "Q":
                                    qhflag = true;
                                    qianzhouzhuiflag = false;
                                    qianhouzhui = "前缀(P)";
                                    break;
                                case "H":
                                    qhflag = false;
                                    qianzhouzhuiflag = false;
                                    qianhouzhui = "后缀(P)";
                                    break;
                            }
                        }
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
                    string postEmpty = Post;
                    if (qhflag)
                    {
                        Post = string.Concat(Post, "<>");
                    }
                    else
                    {
                        Post = string.Concat("<>", Post);
                    }
                    for (int i = 0; i < dimObjectId.Length; i++)
                    {
                        entities[i] = (Entity)dimObjectId[i].GetObject(OpenMode.ForWrite);
                        Dimension dimension1 = (Dimension)entities[i];
                        if (dimension1.DimensionText != string.Empty)
                        {
                            if (qhflag)
                            {
                                dimension1.DimensionText = string.Concat(postEmpty, dimension1.DimensionText);
                            }
                            else
                            {
                                dimension1.DimensionText = string.Concat(dimension1.DimensionText, postEmpty);

                            }
                        }
                        else
                        {
                            dimension1.Dimpost = Post;
                        }
                    }
                    trans.Commit();
                }
            }
        }
    }
}
