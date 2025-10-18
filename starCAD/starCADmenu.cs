using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Customization;
using System.Collections.Specialized;

namespace sCAD
{
    public class starCADmenu
    {
        [CommandMethod("starmenu")]


        public void starmenu()
        {
            //自定义的组名
            string strMyGroupName = "sCADGroup";
            //保存的CUI文件名（从CAD2010开始，后缀改为了cuix）
            string strCuiFileName = "sCADMenu.cui";
            string menuPath = string.Format(@"C:\Users\{0}\Documents\sCADMenu.cui", Environment.UserName);
            string menuGroupName = "sCADGroup";

            List<double> vs = new List<double>();
            //for (int i = 0; i < length; i++)
            //{
                
            //}
            //创建一个自定义组（这个组中将包含我们自定义的命令、菜单、工具栏、面板等）
            CustomizationSection myCSection = new CustomizationSection();
            myCSection.MenuGroupName = strMyGroupName;

            //创建自定义命令组
            MacroGroup mg = new MacroGroup("MyMethod", myCSection.MenuGroup);
            MenuMacro mm1 = new MenuMacro(mg, "标注值互换", "DimSwapV", "");
            MenuMacro mm2 = new MenuMacro(mg, "复制标注值到剪切板", "DimVCo ", "");
            MenuMacro mm3 = new MenuMacro(mg, "测量弧长半径弦长弦高", "DimHu", "");
            MenuMacro mm4 = new MenuMacro(mg, "物体与选定点做连线", "SLL", "");
            MenuMacro mm5 = new MenuMacro(mg, "以物体外框均分移动", "TweenMove", "");
            MenuMacro mm6 = new MenuMacro(mg, "以预输入值改变标注值", "DimCV", "");
            MenuMacro mm7 = new MenuMacro(mg, "两点定位", "Orient", "");
            MenuMacro mm8 = new MenuMacro(mg, "连接多段线", "LianJiePL", "");
            MenuMacro mm9 = new MenuMacro(mg, "标注前后缀", "DimPost", "");
            MenuMacro mm10 = new MenuMacro(mg, "标注矩形框长宽", "DimBoundary", "");
            MenuMacro mm11 = new MenuMacro(mg, "以分缝线标注", "DimCurve", "");
            MenuMacro mm12 = new MenuMacro(mg, "快速画圆", "PolyCircle", "");
            MenuMacro mm13 = new MenuMacro(mg, "最近点画线", "ZJD", "");
            MenuMacro mm14 = new MenuMacro(mg, "两点画×", "fork", "");
            MenuMacro mm15 = new MenuMacro(mg, "真彩色转索引色", "LayerPatchColor", "");
            MenuMacro mm16 = new MenuMacro(mg, "标注空间尺寸", "DynamicDimLinear", "");

            //声明菜单别名
            StringCollection scMyMenuAlias = new StringCollection();
            scMyMenuAlias.Add("MyPop1");
            scMyMenuAlias.Add("MyTestPop");

            //菜单项（将显示在项部菜单栏中）
            PopMenu pmParent = new PopMenu("sCAD", scMyMenuAlias, "sCAD", myCSection.MenuGroup);

            //子项的菜单（多级）
            PopMenu pm1 = new PopMenu("曲线", new StringCollection(), "", myCSection.MenuGroup);
            PopMenuItem pmi4 = new PopMenuItem(mm4, "物体与选定点做连线 <-----【SLL】", pm1, -1);
            PopMenuItem pmi8 = new PopMenuItem(mm8, "连接多段线 <-----【LianJiePL】", pm1, -1);
            PopMenuItem pmi12 = new PopMenuItem(mm12, "快速画圆 <-----【PolyCircle】", pm1, -1);
            PopMenuItem pmi13 = new PopMenuItem(mm13, "最近点画线 <-----【ZJD】", pm1, -1);
            PopMenuItem pmi14 = new PopMenuItem(mm14, "两点画× <-----【fork】", pm1, -1);
            PopMenuRef pmr1 = new PopMenuRef(pm1, pmParent, -1);
            /************************************************************************************************************************************/
            PopMenu pm2 = new PopMenu("标注", new StringCollection(), "", myCSection.MenuGroup);
            PopMenuItem pmi1 = new PopMenuItem(mm1, "标注值互换 <-----【DimSwapV】", pm2, -1);
            PopMenuItem pmi2 = new PopMenuItem(mm2, "复制标注值到剪切板 <-----【DimVCo】", pm2, -1);
            PopMenuItem pmi6 = new PopMenuItem(mm6, "以预输入值改变标注值 <-----【DimCV】", pm2, -1);
            PopMenuItem pmi3 = new PopMenuItem(mm3, "测量弧长半径弦长弦高 <-----【DimHu】", pm2, -1);
            PopMenuItem pmi9 = new PopMenuItem(mm9, "标注前后缀 <-----【DimPost】", pm2, -1);
            PopMenuItem pmi10 = new PopMenuItem(mm10, "标注矩形框长宽 <-----【DimBoundary】", pm2, -1);
            PopMenuItem pmi11 = new PopMenuItem(mm11, "以分缝线标注 <-----【DimCurve】", pm2, -1);
            PopMenuItem pmi16 = new PopMenuItem(mm16, "标注空间尺寸 <-----【DynamicDimLinear】", pm2, -1);
            PopMenuRef pmr2 = new PopMenuRef(pm2, pmParent, -1);
            /************************************************************************************************************************************/
            PopMenu pm3 = new PopMenu("移动", new StringCollection(), "", myCSection.MenuGroup);
            PopMenuItem pmi5 = new PopMenuItem(mm5, "以物体外框均分移动 <-----【TweenMove】", pm3, -1);
            PopMenuItem pmi7 = new PopMenuItem(mm7, "两点定位 <-----【Orient】", pm3, -1);
            PopMenuRef pmr3 = new PopMenuRef(pm3, pmParent, -1);
            /************************************************************************************************************************************/
            PopMenu pm4 = new PopMenu("图层", new StringCollection(), "", myCSection.MenuGroup);
            PopMenuItem pmi15 = new PopMenuItem(mm15, "真彩色转索引色 <-----【LayerPatchColor】", pm4, -1);
            PopMenuRef pmr4 = new PopMenuRef(pm4, pmParent, -1);
            /************************************************************************************************************************************/
            //子项的菜单（单级）
            //PopMenuItem pmi3 = new PopMenuItem(mm3, "保存(&S)", pmParent, -1);
            //PopMenuItem pmi1 = new PopMenuItem(mm1, "标注值互换 <-----【DimSwapV】", pm1, -1);
            //PopMenuItem pmi2 = new PopMenuItem(mm2, "复制标注值到剪切板 <-----【DimVCo】", pm1, -1);
            //PopMenuItem pmi3 = new PopMenuItem(mm3, "测量弧长半径弦长弦高 <-----【DimHu】", pmParent, -1);
            //PopMenuItem pmi4 = new PopMenuItem(mm4, "物体与选定点做连线 <-----【SLL】", pmParent, -1);
            //PopMenuItem pmi5 = new PopMenuItem(mm5, "以物体外框均分移动 <-----【TweenMove】", pmParent, -1);
            //PopMenuItem pmi6 = new PopMenuItem(mm6, "以预输入值改变标注值 <-----【DimCV】", pm1, -1);
            //PopMenuItem pmi7 = new PopMenuItem(mm7, "两点定位 <-----【Orient】", pmParent, -1);
            //PopMenuItem pmi8 = new PopMenuItem(mm8, "连接多段线 <-----【LianJiePL】", pmParent, -1);
            //PopMenuItem pmi9 = new PopMenuItem(mm9, "标注前后缀 <-----【DimPost】", pm1, -1);


            // 最后保存文件
            myCSection.RemovePartialMenu(menuPath, menuGroupName);
            PartialCuiFileCollection aa = myCSection.PartialCuiFiles;
            myCSection.SaveAs(menuPath);
            //bool unret = Autodesk.AutoCAD.ApplicationServices.Application.UnloadPartialMenu(menuPath);
            //Autodesk.AutoCAD.ApplicationServices.Application.ReloadAllMenus();
        }
    }
}
