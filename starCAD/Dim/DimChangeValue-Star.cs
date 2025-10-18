using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AAPP = Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using sCAD;
using starCAD;
using Autodesk.AutoCAD.ApplicationServices;

namespace sCAD.Dim
{

    public class DimChangeValue
    {
        public string thisDimValue = string.Empty;

        [CommandMethod("DimCV")]//"DimCV")]
        public void DimCV()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = AAPP.Application.DocumentManager.MdiActiveDocument.Editor;

            bool flag0 = true;
            string DimValue = string.Empty;
            PromptResult pr = null;
            while (flag0)
            {
                if (thisDimValue == string.Empty)
                {
                    pr = ed.GetString("输入标注值");
                    if (pr.Status == PromptStatus.OK || pr.Status == PromptStatus.Keyword)
                    {
                        DimValue = pr.StringResult;
                        thisDimValue = DimValue;
                        flag0 = false;
                    }
                }
                else
                {
                    string ss = string.Format("输入标注值{0}", thisDimValue);
                    pr = ed.GetString(ss);
                    if (pr.Status == PromptStatus.OK || pr.Status == PromptStatus.Keyword)
                    {
                        if (pr.StringResult == string.Empty)
                        {
                            DimValue = thisDimValue;
                            flag0 = false;
                        }
                        else
                        {
                            DimValue = pr.StringResult;
                            thisDimValue = DimValue;
                            flag0 = false;
                        }

                    }
                }
                if (pr.Status == PromptStatus.Cancel) return;
                if (pr.Status == PromptStatus.None) return;
                if (pr.Status != PromptStatus.OK) return;
            }

            /***********************************************/
            bool flag1 = true;
            while (flag1)
            {
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

        [CommandMethod("DimCVW")]//"DimCV")]
        public void DimCVW()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = AAPP.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionResult psr = ed.GetSelection();
            IDValues.Clear();
            List<string> showtext = new List<string>();
            if (psr.Status == PromptStatus.Cancel) return;
            if (psr.Status == PromptStatus.None) return;
            //PromptSelectionResult psr = ed.GetSelection();
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet sSet = psr.Value;
                ObjectId[] dimObjectId = sSet.GetObjectIds();
                IDValues.AddRange(dimObjectId);
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    Entity[] entities = new Entity[dimObjectId.Length];
                    for (int i = 0; i < dimObjectId.Length; i++)
                    {
                        Entity dimEntity1 = (Entity)dimObjectId[i].GetObject(OpenMode.ForRead);
                        entities[i] = dimEntity1;
                        Dimension dimension1 = (Dimension)dimEntity1;
                        string va = string.Empty;
                        string Men1 = dimension1.Measurement.ToString();
                        string input = dimension1.DimensionText;

                        if (input == string.Empty)
                        {
                            va = Men1;
                        }
                        else
                        {
                            va = input;
                        }
                        showtext.Add(va);
                    }
                    DimChangeValueWindow mainWindow = new DimChangeValueWindow(showtext, entities);
                    mainWindow.TopMost = true;
                    mainWindow.Show();
                    trans.Commit();
                }
            }


        }

        public static List<ObjectId> IDValues = new List<ObjectId>();
        public void ChangeValue()
        {
            DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = AAPP.Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    //Entity[] entities = new Entity[dimObjectId.Length];

                    BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    //打开块表记录
                    BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    for (int i = 0; i < IDValues.Count; i++)
                    {
                        //Dimension dimension1 = IDValues[i].Id.GetObject(OpenMode.ForWrite) as Dimension;
                        Dimension dimension1 = trans.GetObject(IDValues[i], OpenMode.ForWrite) as Dimension;
                        if (sCAD.Dim.DimChangeValueWindow.RangeValue.Count > i)
                        {
                            dimension1.DimensionText = sCAD.Dim.DimChangeValueWindow.RangeValue[i];
                        }
                        else
                        {
                            dimension1.DimensionText = sCAD.Dim.DimChangeValueWindow.showtexts[i];
                        }
                    }
                    trans.Commit();
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    trans.Abort();
                }
            }
            docLock.Dispose();
        }
        //public DimChangeValueWindow myMainWindow
        //{
        //    get
        //    {
        //        DimChangeValueWindow mainWindow = new DimChangeValueWindow();
        //        return mainWindow;
        //    }
        //    set
        //    {
        //    }
        //}
    }
}
