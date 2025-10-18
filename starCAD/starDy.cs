using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sCAD
{


    public static class starMathdy
    {
        public static Point3d Pointaverage(List<Point3d> points)
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

        public static Point3d[] pointsXSort(double[] keys, Point3d[] points)
        {
            double[] newkeys = new double[keys.Length];
            Array.Copy(keys, newkeys, keys.Length);
            Array.Sort(newkeys, points);
            return points;
        }

        public static Entity[] EntitySort(double[] keys, Entity[] entities)
        {
            double[] newkeys = new double[keys.Length];
            Array.Copy(keys, newkeys, keys.Length);
            Array.Sort(newkeys, entities);
            return entities;
        }

        public static Extents3d[] ExtentSort(double[] keys, Extents3d[] extents)
        {
            double[] newkeys = new double[keys.Length];
            Array.Copy(keys, newkeys, keys.Length);
            Array.Sort(newkeys, extents);
            return extents;
        }

        public static Vector3d Unitize(Vector3d v3)
        {
            double x = v3.X * v3.X;
            double y = v3.Y * v3.Y;
            double z = v3.Z * v3.Z;
            double addxyz = 1 / Math.Sqrt(x + y + z);
            Vector3d result = new Vector3d(addxyz * v3.X, addxyz * v3.Y, addxyz * v3.Z);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        #region 角度转弧度
        public static double Radians(this double angle)
        {
            double a = angle * Math.PI / 180;
            return a;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        #region 弧度转角度
        public static double Dreeges(this double radians)
        {
            double a = 180 / Math.PI * radians;
            return a;
        }
        #endregion
    }
}
