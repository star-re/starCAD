using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace starCAD
{
    public class DimSwapValue
    {
        [CommandMethod("DimSwapV")]

        public void DimSwapV()
        {
            
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionResult psr = ed.GetSelection();
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet sSet = psr.Value;
                ObjectId[] dimObjectId = sSet.GetObjectIds();

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    Entity dimEntity1 = (Entity)dimObjectId[0].GetObject(OpenMode.ForWrite);
                    Entity dimEntity2 = (Entity)dimObjectId[1].GetObject(OpenMode.ForWrite);

                    Dimension dimension1 = (Dimension)dimEntity1;
                    Dimension dimension2 = (Dimension)dimEntity2;
                    string Men1 = Math.Round(dimension1.Measurement,dimension1.Dimdec).ToString();
                    string Men2 = Math.Round(dimension2.Measurement,dimension2.Dimdec).ToString();
                    string DimT1 = dimension1.DimensionText;
                    string DimT2 = dimension2.DimensionText;

                    if (DimT1 == string.Empty || DimT2 == string.Empty)
                    {
                        if (DimT1 == string.Empty) { dimension2.DimensionText = Men1; }
                        else { dimension2.DimensionText = DimT1; }

                        if (DimT2 == string.Empty) { dimension1.DimensionText = Men2; }
                        else { dimension1.DimensionText = DimT2; }
                    }
                    else
                    {
                        dimension1.DimensionText = DimT2;
                        dimension2.DimensionText = DimT1;
                    }
                    
                    trans.Commit();
                }
            }
        }
    }
}
