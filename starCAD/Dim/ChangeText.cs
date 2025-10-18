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
    public class ChangeZengLiang
    {
        public static double CC = 0;
    }
    public class ChangeText
    {
        [CommandMethod("ChangeText", CommandFlags.UsePickSet)]

        public void ChangeTexts()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            bool flag0 = true;
            PromptSelectionResult Dpsr = ed.SelectImplied();
            PromptSelectionOptions DpromptSelectionOptions = new PromptSelectionOptions();
            DpromptSelectionOptions.MessageForAdding = "请拾取标注：";
            string promptstr = "请拾取标注 [增量(D)]";
            //PromptSelectionResult Dpsr = ed.SelectImplied();
            //PromptSelectionOptions DpromptSelectionOptions = new PromptSelectionOptions();
            //DpromptSelectionOptions.MessageForAdding = "请拾取标注";
            //if (Dpsr.Status != PromptStatus.OK)
            //{
            //    goto SelectImpliedJump;
            //    //Dpsr = ed.GetSelection(DpromptSelectionOptions);
            //}
            PromptEntityResult per = ed.GetEntity(ed.GetSelectEntityOp2(promptstr, new string[] { "D" }));
            ObjectId objectId = ObjectId.Null;
            if (per.ObjectId != null)
            {
                objectId = per.ObjectId;
            }
            while (flag0)
            {
                per = ed.GetEntity(ed.GetSelectEntityOp2(promptstr, new string[] { "D" }));
                string ss = per.StringResult;
                switch (per.StringResult)
                {
                    case "D":
                        PromptDoubleResult Countstr = ed.GetDouble("增量：");
                        ChangeZengLiang.CC = Countstr.Value;
                        break;
                }
                if (!per.ObjectId.IsNull)
                {
                    objectId = per.ObjectId;
                }
                if (per.Status == PromptStatus.Cancel) flag0 = false;
                if (per.Status == PromptStatus.None) flag0 = false;
            }


      //  SelectImpliedJump:
            //SelectionSet DsSet = Dpsr.Value;
            ObjectId[] DdimObjectId = new ObjectId[] { objectId };
            /*************************************************************/
            List<Entity> TextEntitys = new List<Entity>();
            PromptSelectionResult psr = ed.SelectImplied();
            PromptSelectionOptions promptSelectionOptions = new PromptSelectionOptions();
            promptSelectionOptions.MessageForAdding = "请拾取文字";
            if (psr.Status != PromptStatus.OK)
            {
                psr = ed.GetSelection(promptSelectionOptions);
            }
            if (psr.Status == PromptStatus.Cancel) return;
            if (psr.Status == PromptStatus.None) return;
            SelectionSet sSet = psr.Value;
            ObjectId[] dimObjectId = sSet.GetObjectIds();
            Entity[] entities = new Entity[dimObjectId.Length];

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Entity Dentity = (Entity)DdimObjectId[0].GetObject(OpenMode.ForRead);
                Dimension dimension1 = (Dimension)Dentity;

                DBText dBText = new DBText();
                string dengyu = "=";
                for (int i = 0; i < sSet.Count; i++)
                {
                    Entity TEntity = (Entity)dimObjectId[i].GetObject(OpenMode.ForWrite);
                    dBText = (DBText)TEntity;

                    dBText.TextString = string.Concat(dBText.TextString, dengyu, Math.Round(dimension1.Measurement, 0) + ChangeZengLiang.CC);
                    // entities[i] = TEntity1;
                }
                trans.Commit();
            }
        }
    }
}
