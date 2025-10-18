using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace starCAD
{
    public class TweenCurve
    {
        [CommandMethod("TweenCrv")]

        public void TweenCrv()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionResult psr = ed.SelectAll();
            SelectionSet sSet = psr.Value;
            ObjectId[] dimObjectId = sSet.GetObjectIds();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Entity[] entities = new Entity[dimObjectId.Length];
                for (int i = 0; i < dimObjectId.Length; i++)
                {
                    Entity entity = (Entity)trans.GetObject(dimObjectId[i], OpenMode.ForWrite);
                    SubentityId subentityId = new SubentityId();
                    FullSubentityPath fullSubentityPath = new FullSubentityPath(dimObjectId,subentityId);
                    entity.Highlight(fullSubentityPath,false);
                }
                trans.Commit();
            }
        }
    }
}
