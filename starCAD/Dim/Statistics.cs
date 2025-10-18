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

namespace sCAD.Dim
{
    public class Statistics
    {
        [CommandMethod("TextStatistics")]
        public void TextStatistics()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            TypedValue[] typedValues = new TypedValue[1];

            typedValues[0] = new TypedValue((int)DxfCode.Start, "TEXT,MTEXT");
            SelectionFilter selectionFilter = new SelectionFilter(typedValues);
            PromptSelectionResult psr = ed.GetSelection(selectionFilter);


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
                    DBText dBText = new DBText();
                    MText Mtext = new MText();
                    for (int i = 0; i < dimObjectId.Length; i++)
                    {
                        entities[i] = (Entity)dimObjectId[i].GetObject(OpenMode.ForRead);
                        if (entities[i].GetType() == typeof(DBText))
                        {
                            dBText = entities[i] as DBText;
                            copyDimStr.Add(dBText.TextString);
                        }
                        else
                        {
                            Mtext = entities[i] as MText;
                            copyDimStr.Add(Mtext.Text);
                        }
                    }
                    DistinctCount(copyDimStr);
                }
            }
        }

        public List<string> DistinctCount(List<string> text)
        {
            string[] sts = text.Distinct().ToArray();
            int[] s = text.GroupBy(i => i).Select(i => i.Count()).ToArray();
            List<string> stsCount = new List<string>();
            Array.Sort(sts, s);
            for (int i = 0; i < sts.Length; i++)
            {
                stsCount.Add(string.Format("{0}|{1}", sts[i], s[i]));
            }
            StatisticsWindow mainWindow = new StatisticsWindow(sts, s);
            mainWindow.TopMost = true;
            mainWindow.Show();

            Clipboard.SetDataObject(string.Join("\r\n", stsCount));
            return stsCount;
        }
    }
}