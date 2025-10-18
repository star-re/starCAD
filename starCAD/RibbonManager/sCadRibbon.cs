using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using SWC = System.Windows.Controls;

namespace sCAD
{
    public class sCadRibbon
    {
        [CommandMethod("sCadRibbonLoad")]

        public void sCadRibbonLoad(RibbonTab rt)
        {
            // string ra = RibbonTab.AutomationNameName;
            //RibbonControl rc = ComponentManager.Ribbon;
            //RibbonTab rt = rc.AddTab("sCAD", "Acad.sCadID", true);
            /*******************************************************************************************/
            RibbonPanelSource CurveRibbonPanelSource = rt.AddPanel("曲线");
           
            CurveRibbonPanelSource.AddButton("SLL", "连线", "物体与选定点做连线", Properties.Resources.SLL_32_32, RibbonItemSize.Standard, SWC.Orientation.Horizontal);
            CurveRibbonPanelSource.AddButton("LIANJIEPL", "连接多段线", "连接多段线的每个点", Properties.Resources.LinkPolyline, RibbonItemSize.Standard, SWC.Orientation.Horizontal);



            /*******************************************************************************************/
            RibbonPanelSource DimRibbonPanelSource = rt.AddPanel("标注");
            DimRibbonPanelSource.AddButton("DimSwapV", "标注值互换", "将选定的两个标注值互换", Properties.Resources.DimSwapV, RibbonItemSize.Standard, SWC.Orientation.Vertical);
            DimRibbonPanelSource.AddButton("DimVco", "复制标注值", "复制标注值到剪切板", Properties.Resources.DimChangeText, RibbonItemSize.Standard, SWC.Orientation.Vertical);
            DimRibbonPanelSource.AddButton("DimCv", " 改变标注值", "以预输入值改变标注值", Properties.Resources.DimChangeText, RibbonItemSize.Standard, SWC.Orientation.Vertical);
            DimRibbonPanelSource.AddButton("DimHu", "测量圆弧", "测量弧长半径弦长弦高", Properties.Resources.dimhuchang, RibbonItemSize.Standard, SWC.Orientation.Vertical);
            DimRibbonPanelSource.AddButton("DimPost", "标注前后缀", "更改标注前后缀（后缀功能未写，后期加上）", Properties.Resources.DimChangeText, RibbonItemSize.Standard, SWC.Orientation.Vertical);
            DimRibbonPanelSource.AddButton("DimBoundary", "标注长宽", "标注矩形框的长宽（后期会加上标注位置，是否把多段线全标注等功能）", Properties.Resources.DimBound_32_32, RibbonItemSize.Standard, SWC.Orientation.Vertical);
            DimRibbonPanelSource.AddButton("DimPolyline", "标注多重线", "标注多重曲线矩形框的长宽（后期会加上标注位置，是否把多段线全标注等功能）", Properties.Resources.DimPolyline_32_32, RibbonItemSize.Standard, SWC.Orientation.Vertical);
            DimRibbonPanelSource.AddButton("ChangeText", "改变文字", "通过拾取标注改变文字", Properties.Resources.DimChangeText, RibbonItemSize.Standard, SWC.Orientation.Vertical);
            DimRibbonPanelSource.AddButton("DeconstuctRectangle", "矩形框长宽", "标注矩形框长宽文字", Properties.Resources.DeconstuctRectangle_32_32, RibbonItemSize.Standard, SWC.Orientation.Vertical);





            /*******************************************************************************************/
            RibbonPanelSource TranRibbonPanelSource = rt.AddPanel("移动");
            TranRibbonPanelSource.AddButton("Orient", "两点定位", "将物体以两点定位，操作逻辑与Rhino一致", Properties.Resources.Orient_32_32, RibbonItemSize.Standard, SWC.Orientation.Vertical);
            TranRibbonPanelSource.AddButton("TweenMove", "均分移动", "以物体外框均分移动", Properties.Resources.TweenMove_32_32, RibbonItemSize.Standard, SWC.Orientation.Vertical);

        }
    }
}
