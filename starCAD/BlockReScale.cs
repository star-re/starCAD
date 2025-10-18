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
using Autodesk.AutoCAD.Geometry;

namespace sCAD
{
    public class BlockReScaleA
    {
        [CommandMethod("BlockReScale")]//"DimCV")]
        public void BlockReScale()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

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
                        if (dimEntity1.GetType() == typeof(BlockReference))
                        {
                            BlockReference blockReference = (BlockReference)dimEntity1;
                            AttributeCollection ac = blockReference.AttributeCollection;
                            //blockReference.Bounds.Value.MinPoint
                            blockReference.ScaleFactors = new Scale3d(1);
                        }
                    }
                    trans.Commit();
                }
            }
        }
    }
}
