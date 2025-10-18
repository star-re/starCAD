using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace starCAD
{
    public class Class1
    {
        [CommandMethod("GetType")]

        public void GetType()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult psr = ed.SelectImplied();
            if (psr.Status != PromptStatus.OK)
            {
                psr = ed.GetSelection();
            }
            if (psr.Status == PromptStatus.Cancel) return;
            if (psr.Status == PromptStatus.None) return;
            if (psr.Status != PromptStatus.OK) return;
            SelectionSet sSet = psr.Value;
            ObjectId[] dimObjectId = sSet.GetObjectIds();
            List<string> strList = new List<string>();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Entity[] entities = new Entity[dimObjectId.Length];
                for (int i = 0; i < dimObjectId.Length; i++)
                {
                    Entity entity = (Entity)trans.GetObject(dimObjectId[i], OpenMode.ForWrite);
                    strList.Add(entity.GetType().ToString());
                }
            }
            
            ed.WriteMessage(string.Join("\r\n", strList));
            //Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            ////向命令行发送命令
            //doc.SendStringToExecute(FunctionName.FirstOrDefault() + " ", true, false, false);
        }
    }
}
