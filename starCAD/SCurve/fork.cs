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

namespace sCAD.SCurve
{
    public class forkCommands
    {
        public static Point3d GetMidPoint(List<Point3d> points)
        {
            List<double> xa = new List<double>();
            List<double> ya = new List<double>();
            List<double> za = new List<double>();
            for (int i = 0; i < points.Count; i++)
            {
                xa.Add(points[i].X);
                ya.Add(points[i].Y);
                za.Add(points[i].Z);
            }
            double xx = xa.Average();
            double yy = ya.Average();
            double zz = za.Average();
            return new Point3d(xx, yy, zz);
        }
    }

    public class forkCommand
    {
        [CommandMethod("fork", CommandFlags.UsePickSet)]

        public void fork()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            Point3d p31 = new Point3d();
            Point3d p32 = new Point3d();
            PromptPointResult ppr1 = ed.GetPoint("\r\n需放置的点");
            if (ppr1.Status == PromptStatus.OK)
            {
                p31 = ppr1.Value;
            }
            if (ppr1.Status == PromptStatus.Cancel) {return; }
            if (ppr1.Status == PromptStatus.None) { return; }

            JigJig jig = new JigJig(new Point3d(), (p, e1, e2) =>
             {
                 p32 = p;
                 List<Point3d> p3s = new List<Point3d>() { p31, p32 };
                 Point3d Midpo1 = forkCommands.GetMidPoint(p3s);
                 Point3d Midpo2 = new Point3d(Midpo1.X, Midpo1.Y + 100, Midpo1.Z);
                 Line line = new Line(p31, p32);
                 e1.Add(line);

                 Line line2 = new Line(p31, p32);
                 Line3d MidLine = new Line3d(Midpo1, Midpo2);
                 line2.TransformBy(Matrix3d.Mirroring(MidLine));
                 e1.Add(line2);
             },null, "\r\n需放置的点");
            //PromptPointResult ppr2 = ed.GetPoint("\r\n需放置的点");
            //if (ppr2.Status == PromptStatus.OK)
            //{
            //    p32 = ppr2.Value;
            //}
            //if (ppr2.Status == PromptStatus.Cancel) { return; }
            //if (ppr2.Status == PromptStatus.None) { return; }
            //List<Point3d> p3s = new List<Point3d>() { p31, p32 };
            //Point3d Midpo1 = forkCommands.GetMidPoint(p3s);
            //Point3d Midpo2 = new Point3d(Midpo1.X, Midpo1.Y + 100, Midpo1.Z);
            //Line line = new Line(p31, p32);
            //ReBlockTable.ReBlock(line);

            //Line line2 = new Line(p31, p32);
            //Line3d MidLine = new Line3d(Midpo1, Midpo2);
            //line2.TransformBy(Matrix3d.Mirroring(MidLine));
            //ReBlockTable.ReBlock(line2);
        }
    }
}
