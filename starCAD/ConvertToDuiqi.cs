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

namespace sCAD
{
    public class ConvertToDuiqi
    {
        [CommandMethod("ZDQ", CommandFlags.UsePickSet)]

        public void ZDQ()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            Entity[] result = null;
            Entity[] Refresult = null;
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
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Entity[] entities = new Entity[dimObjectId.Length];
                for (int i = 0; i < dimObjectId.Length; i++)
                {
                    Entity entity = (Entity)trans.GetObject(dimObjectId[i], OpenMode.ForWrite);
                    DBText dBText = entity as DBText;
                    Point3d point3D = dBText.AlignmentPoint;
                    if (dBText.HorizontalMode ==  TextHorizontalMode.TextLeft)
                    {
                        //dBText.Justify = AttachmentPoint.MiddleLeft;
                        dBText.Justify = AttachmentPoint.BaseLeft;
                        dBText.Position = point3D;
                        // dBText.VerticalMode = TextVerticalMode.TextBase;
                    }
                    else if (dBText.HorizontalMode == TextHorizontalMode.TextRight)
                    {
                        dBText.Justify = AttachmentPoint.BaseRight;
                        dBText.Position = point3D;
                        //dBText.AlignmentPoint = point3D;
                        // dBText.VerticalMode = TextVerticalMode.TextBase;
                    }
                }
                trans.Commit();
            }
        }
    }
}
