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
    public class TextChangeValue
    {
        public string thisTextValue = string.Empty;

        [CommandMethod("TextCV")]
        public void DimCV()
        {



            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            bool flag0 = true;
            string TextValue = SWF.Clipboard.GetText();

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
                        Entity[] entities = new Entity[textObjectId.Length];
                        for (int i = 0; i < textObjectId.Length; i++)
                        {
                            Entity textEntity1 = (Entity)textObjectId[i].GetObject(OpenMode.ForWrite);
                            DBText textension1 = (DBText)textEntity1;
                            textension1.TextString = TextValue;
                        }
                        trans.Commit();
                    }
                }
            }

        }
    }
}
