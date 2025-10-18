using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using sCAD;
using SWF = System.Windows.Forms;

namespace sCAD
{
    public class TextLengthD
    {

        [CommandMethod("TextLength")]
        public void DimCV()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            bool flag0 = true;
            string ss = string.Empty;
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
                    ObjectId[] textObjectId = sSet.GetObjectIds();

                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        Entity entity = (Entity)textObjectId[0].GetObject(OpenMode.ForRead);
                        if (entity.GetType() == typeof(DBText))
                        {
                            DBText dB = entity as DBText;
                            ss = dB.TextString.Length.ToString();
                        }
                        else if (entity.GetType() == typeof(MText))
                        {
                            MText dB = entity as MText;
                            ss = dB.Text.Length.ToString();
                        }
                        ed.WriteMessage(ss);
                    }

                }
            }
        }
    }
}
