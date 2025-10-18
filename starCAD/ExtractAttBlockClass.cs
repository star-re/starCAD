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
    public class ExtractAttBlockClass
    {
        [CommandMethod("ExtractAttBlock")]

        public void ExtractAttBlock()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionResult psr = ed.GetSelection();
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet sSet = psr.Value;
                ObjectId[] dimObjectId = sSet.GetObjectIds();
                BlockReference[] blockReference = new BlockReference[1];
                Point3d basepoint = Point3d.Origin;
                double Scale3d = 1;
                DBObjectCollection dBObjectCollection = new DBObjectCollection();
                string bName = "";
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    Entity blockEntity1 = (Entity)dimObjectId[0].GetObject(OpenMode.ForWrite);
                    BlockReference rd = blockEntity1 as BlockReference;
                    if (rd.IsDynamicBlock)
                    {
                        Scale3d = rd.ScaleFactors.X;
                        rd.ScaleFactors = new Scale3d(1);
                        basepoint = rd.Position;
                        foreach (DynamicBlockReferenceProperty item in rd.DynamicBlockReferencePropertyCollection)
                        {
                            rd.Explode(dBObjectCollection);
                            bName = item.Value.ToString();
                            //  blockReference[0] = new BlockReference(Point3d.Origin, item.BlockId);
                        }
                    }
                    rd.ScaleFactors = new Scale3d(Scale3d);
                    blockEntity1.TransformBy(Matrix3d.Displacement(new Vector3d(0, 150, 0)));
                    trans.Commit();
                }
                List<Entity> entities = new List<Entity>();
                for (int i = 0; i < dBObjectCollection.Count; i++)
                {
                    Entity entity = dBObjectCollection[i] as Entity;
                    if (entity.Visible)
                    {
                        entities.Add(entity);
                    }
                }
                //starCAD.ReBlockTable.ReBlock((Entity)blockReference[0]);
                starCAD.ReBlockTool.AddBlock(db, bName, entities, basepoint);
            }
        }
    }
}
