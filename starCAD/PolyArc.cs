using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using sCAD;

namespace sCAD
{
    public class PolyArc
    {
        [CommandMethod("PolyCircle")]//"DimCV")]


        public void PolyCircle()
        {
            PolyArcWindow mainWindow = new PolyArcWindow();
            mainWindow.TopMost = true;
            mainWindow.Show();
           //Application.ShowModelessWindow(myMainWindow);
        }

        public PolyArcWindow myMainWindow
        {
            get
            {
                PolyArcWindow mainWindow = new PolyArcWindow();
                return mainWindow;
            }
            set
            {
            }
        }
    }
}
