using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using System;
using acApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.Customization;
using System.Linq;
using System.Collections.Generic;

namespace sCAD
{
    public class AutoLoadRibbon : IExtensionApplication
    {
        private string menuPath = string.Format(@"C:\Users\{0}\Documents\sCADMenu.cuix", Environment.UserName);
        private string menuGroupName = "sCADGroup";


        void IExtensionApplication.Initialize()
        {
            Autodesk.Windows.ComponentManager.ItemInitialized += ComponentManager_ItemInitialized;
            //bool unret = acApp.UnloadPartialMenu(menuPath);
            //acApp.ReloadAllMenus();
            //Terminate();
            //bool ret = acApp.LoadPartialMenu(menuPath);
            //acApp.LoadPartialMenu("sCADMenu.cuix");
        }

        public void ComponentManager_ItemInitialized(object sender, RibbonItemEventArgs e)
        {
            if (Autodesk.Windows.ComponentManager.Ribbon != null)
            {
                //if (Autodesk.Windows.ComponentManager.Ribbon.Name)
                //{

                //}
                
                Autodesk.Windows.RibbonControl rc = ComponentManager.Ribbon;
               List<string> titles =   rc.Tabs.Select(i => i.Title).ToList();
                if (!titles.Contains("sCAD"))
                {
                    RibbonTab rt = rc.AddTab("sCAD", "Acad.sCadID", true);
                    sCadRibbon sCadRibbon = new sCadRibbon();
                    sCadRibbon.sCadRibbonLoad(rt);//添加ribbon菜单的函数
                    Autodesk.Windows.ComponentManager.ItemInitialized -= ComponentManager_ItemInitialized;
                }
            }
        }
        public void Terminate()
        {
            //string mainCuiFile = string.Format("{0}.cuix",

            //  (string)acApp.GetSystemVariable("MENUNAME"));

            //starCADmenu starCADmenu = new starCADmenu();
            //starCADmenu.starmenu();
            //CustomizationSection cs =

            //  new CustomizationSection(mainCuiFile);

            //cs.RemovePartialMenu(menuPath, menuGroupName);

            //if (cs.IsModified == true)
            //{
            //    cs.Save();
            //}
        }
    }
}
