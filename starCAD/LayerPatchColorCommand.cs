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
using AColor = Autodesk.AutoCAD.Colors;

namespace sCAD
{
    public class LayerPatchColorCommand
    {
        [CommandMethod("LayerPatchColor")]

        public void LayerPatchColor()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable layerobjectIds = trans.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
                foreach (var item in layerobjectIds)
                {
                    LayerTableRecord layerTableRecord = item.GetObject(OpenMode.ForWrite) as LayerTableRecord;
                    if (layerTableRecord.Color.ColorNameForDisplay.Length >= 5)
                    {
                        AColor.Color color = new AColor.Color();
                        ColorIndex colorIndex = new ColorIndex();
                        short index = (short)colorIndex.CheckColorindex(layerTableRecord.Color.Red, layerTableRecord.Color.Green, layerTableRecord.Color.Blue);
                        color = AColor.Color.FromColorIndex(AColor.ColorMethod.ByColor, index);
                        layerTableRecord.Color = color;
                    }
                }
                trans.Commit();
            }
        }
    }
}
