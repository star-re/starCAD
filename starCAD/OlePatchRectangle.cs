using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
//using System.Windows.Forms;
using sCAD;
using Autodesk.AutoCAD.Geometry;

namespace sCAD
{
    public class OlePatchRectangle
    {
        [CommandMethod("OlePatchRec", CommandFlags.UsePickSet)]

        public void OlePatchRec()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult psr = ed.SelectImplied();
            if (psr.Status != PromptStatus.OK)
            {
                psr = ed.GetSelection();
            }
            if (psr.Status == PromptStatus.Cancel) return;
            if (psr.Status == PromptStatus.None) return;
            //PromptSelectionResult psr = ed.GetSelection();
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet sSet = psr.Value;
                ObjectId[] dimObjectId = sSet.GetObjectIds();
                Ole2Frame ole2Frame = new Ole2Frame();
                

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    Entity entity = (Entity)dimObjectId[0].GetObject(OpenMode.ForWrite);
                    if (entity.GetType() != typeof(Ole2Frame)) return;
                    ole2Frame = entity as Ole2Frame;
                    double olewid = ole2Frame.WcsWidth;
                    double olehei = ole2Frame.WcsHeight;
                    PromptPointResult ppr = ed.GetPoint("第一个点");
                    OleRecDraw oleRecDraw = new OleRecDraw(ole2Frame.Position3d,ppr.Value);
                    PromptResult result = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oleRecDraw);
                    ole2Frame.TransformBy(oleRecDraw.raMo);
                    ole2Frame.TransformBy(oleRecDraw.raSC);
                    trans.Commit();
                }
            }
        }
    }
}
