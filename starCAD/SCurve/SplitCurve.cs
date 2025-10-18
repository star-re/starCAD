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
using starCAD;

namespace sCAD
{
    public class SplitCurve
    {
        [CommandMethod("SplitCrv", CommandFlags.UsePickSet)]

        public void SplitCrv()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionResult psr1 = ed.SelectImplied();
            if (psr1.Status != PromptStatus.OK)
            {
                psr1 = ed.GetSelection();
            }
            if (psr1.Status == PromptStatus.Cancel) return;
            if (psr1.Status == PromptStatus.None) return;
            if (psr1.Status != PromptStatus.OK) return;
            SelectionSet sSet1 = psr1.Value;
            ObjectId[] dimObjectId1 = sSet1.GetObjectIds();

            ed.SetImpliedSelection(new ObjectId[0]);//清除预选择列表
            PromptSelectionResult psr2 = ed.GetSelection();
            if (psr2.Status == PromptStatus.Cancel) return;
            if (psr2.Status == PromptStatus.None) return;
            if (psr2.Status != PromptStatus.OK) return;
            SelectionSet sSet2 = psr2.Value;
            ObjectId[] dimObjectId2 = sSet2.GetObjectIds();

            Point3dCollection p3dresult = new Point3dCollection();
            Point3dCollection p3d = new Point3dCollection();
            List<Point3d> zhongzhuan = new List<Point3d>();
            DBObjectCollection dBObjectCollection = new DBObjectCollection();
            List<Entity> dbo = new List<Entity>();
            DoubleCollection doubleCollection = new DoubleCollection();
            ViewTableRecord view = ed.GetCurrentView();
            Vector3d viewDir = view.ViewDirection;//当前视图方向
            Plane viewPlane = new Plane(Point3d.Origin, viewDir);
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                for (int i = 0; i < sSet1.Count; i++)
                {
                    Entity TEntity1 = (Entity)dimObjectId1[i].GetObject(OpenMode.ForWrite);
                    for (int j = 0; j < sSet2.Count; j++)
                    {
                        p3d.Clear();
                        Entity TEntity2 = (Entity)dimObjectId2[j].GetObject(OpenMode.ForWrite);
                        TEntity1.IntersectWith(TEntity2, Intersect.OnBothOperands, viewPlane, p3d, IntPtr.Zero, IntPtr.Zero);
                        foreach (Point3d item in p3d)
                        {
                            zhongzhuan.Add(item);
                        }
                    }
                    p3dresult = new Point3dCollection(zhongzhuan.ToArray());
                    Curve curve = (Curve)TEntity1;
                    zhongzhuan = zhongzhuan.Select(m => curve.GetClosestPointTo(m, viewDir, false)).ToList();
                    double[] paramsArr = zhongzhuan.Select(k => curve.GetParameterAtPoint(k)).ToArray();
                    Array.Sort(paramsArr);
                    doubleCollection = new DoubleCollection(paramsArr);
                    dBObjectCollection = curve.GetSplitCurves(doubleCollection);

                    foreach (Curve item in dBObjectCollection)
                    {
                        item.SetPropertiesFrom(TEntity1);
                        dbo.Add(item);
                    }
                    TEntity1.Erase();
                    zhongzhuan.Clear();
                }
                trans.Commit();
            }
            ReBlockTable.ReBlock(db, dbo.ToArray());
        }
    }
}
