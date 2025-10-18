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
    public class DimLineArc
    {
        [CommandMethod("DimCurve", CommandFlags.UsePickSet)]

        public void DimCurve()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            TypedValue[] typedValues = new TypedValue[1];
            typedValues[0] = new TypedValue((int)DxfCode.Start, "LINE,Circle,Arc,LWPolyline,Polyline,Spline,Ellipse,Xline");
            SelectionFilter selectionFilter = new SelectionFilter(typedValues);
            
            //ArcDimension arcDimension = new ArcDimension();
            PromptSelectionResult psr = ed.SelectImplied();
            if (psr.Status != PromptStatus.OK)
            {
                psr = ed.GetSelection(selectionFilter);
            }
            if (psr.Status == PromptStatus.Cancel) return;
            if (psr.Status == PromptStatus.None) return;
            if (psr.Status != PromptStatus.OK) return;
            SelectionSet sSet = psr.Value;
            ObjectId[] dimObjectId = sSet.GetObjectIds();
            List<Entity> TEntitys = new List<Entity>();
            Curve 完成面线 = null;
            List<Curve> 分缝线 = new List<Curve>();
            List<Point3d> 分缝点 = new List<Point3d>();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            //using (Transaction trans = db.TransactionManager.TopTransaction)
            {
                for (int j = 0; j < sSet.Count; j++)
                {
                    Entity entity = (Entity)dimObjectId[j].GetObject(OpenMode.ForWrite);
                    TEntitys.Add(entity);
                    //Type entType = entity.GetType();
                    //Curve cc = entity as Curve;
                    //double Len = 0;
                    //if (cc.GetType() == typeof(Line))
                    //{
                    //    Line line = cc as Line;
                    //    Len = line.Length;
                    //}
                    //else if (cc.GetType() == typeof(Polyline))
                    //{
                    //    Polyline polyline = cc as Polyline;
                    //    Len = polyline.Length;
                    //}
                    //if (entity.GetType() == typeof(Polyline))
                    //{
                    //    TEntitys.Add((Entity)dimObjectId[j].GetObject(OpenMode.ForWrite));
                    //}
                }
            }
            DimLineArcClass DLAC = new DimLineArcClass();
            DLAC.GetCurve(TEntitys, out 完成面线, out 分缝线, out 分缝点);
            DLAC.SplitCurve(完成面线, 分缝线, 分缝点);
        }
    }
}
