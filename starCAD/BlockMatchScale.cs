using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;

namespace sCAD
{
    public class BlockMatchScale
    {
        [CommandMethod("BlockMatchSc")]

        public void BlockMatchSC()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionResult psr = ed.GetSelection();
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet sSet = psr.Value;
                ObjectId[] dimObjectId = sSet.GetObjectIds();
                PromptSelectionResult psr1 = ed.GetSelection();
                if (psr1.Status == PromptStatus.OK)
                {
                    SelectionSet sSet1 = psr1.Value;
                    ObjectId[] dimObjectId1 = sSet1.GetObjectIds();

                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        BlockReference br = (BlockReference)dimObjectId[0].GetObject(OpenMode.ForWrite);
                        for (int i = 0; i < dimObjectId1.Length; i++)
                        {
                            BlockReference br1 = (BlockReference)dimObjectId1[i].GetObject(OpenMode.ForWrite);
                            br1.ScaleFactors = br.ScaleFactors;
                        }
                        trans.Commit();
                    }
                }
            }
        }
    }
}
