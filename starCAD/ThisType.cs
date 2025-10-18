using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;
using aai = Autodesk.AutoCAD.Interop;
using aaic = Autodesk.AutoCAD.Interop.Common;

namespace sCAD
{
    public class ThisType
    {
        [CommandMethod("TType")]

        public void TType()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //PromptSelectionResult psr = ed.GetSelection();
            PromptEntityResult psr = ed.GetEntity("选择Entity");

            ObjectId oldlast = Utils.EntLast();
            //aai.AcadDocument acadDocument = (aai.AcadDocument)HostApplicationServices.WorkingDatabase.AcadDatabase;
           
           // aaic.AcadObject acadObject = acadDocument.ActiveLayout.ModelSpace.AddOLEControlObject(excelApp.OLEObjects(1));
           //acadObject.add

            if (psr.Status == PromptStatus.OK)
            {
                ObjectId dimObjectId = psr.ObjectId;

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    Entity dimEntity1 = (Entity)dimObjectId.GetObject(OpenMode.ForWrite);
                    //if (dimEntity1.GetType() == typeof(Ole2Frame))
                    //{
                    //    Ole2Frame o2f = dimEntity1 as Ole2Frame;
                    //    Ole2Frame ole2Frame = new Ole2Frame();
                    //}
                    BlockReference rd = dimEntity1 as BlockReference;
                    RotatedDimension rd1 = dimEntity1 as RotatedDimension;
                    string ObjectType = dimEntity1.GetType().ToString();
                    ed.WriteMessage(ObjectType);
                    trans.Commit();
                }
            }
        }
    }
}
