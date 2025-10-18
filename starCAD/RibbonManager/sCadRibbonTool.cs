using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Autodesk.Windows.ToolBars;

namespace sCAD
{
    public static partial class sCadRibbonTool
    {
        public static RibbonTab AddTab(this RibbonControl ribbonCtrl, string title, string ID, bool isActive)
        {
            //RibbonFlowPanel ribbonFlowPanel = new RibbonFlowPanel();
            RibbonTab tab = new RibbonTab();
            tab.Title = title;
            tab.Id = ID;
            ribbonCtrl.Tabs.Add(tab);
            tab.IsActive = isActive;
            return tab;
        }

        public static RibbonPanelSource AddPanel(this RibbonTab tab, string title)
        {
            RibbonPanelSource panelSource = new RibbonPanelSource();
            panelSource.Title = title;
            RibbonPanel ribbonPanel = new RibbonPanel();
            ribbonPanel.Source = panelSource;
            tab.Panels.Add(ribbonPanel);
            return panelSource;
        }

        //public static RibbonFlowPanel AddFlowPanel(this RibbonTab tab, string title)
        //{
        //    RibbonSubPanelSource panelSource = new RibbonSubPanelSource();
        //    panelSource.Name = title;
        //    RibbonFlowPanel ribbonPanel = new RibbonFlowPanel();
        //    ribbonPanel.Source = panelSource;
        //    tab.Panels.Add(ribbonPanel);
        //    return panelSource;
        //}

        public static BitmapImage ToBitmapImage(System.Drawing.Bitmap ImageOriginal)
        {

            System.Drawing.Bitmap ImageOriginalBase = new System.Drawing.Bitmap(ImageOriginal);
            BitmapImage bitmapImage = new BitmapImage();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ImageOriginalBase.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="RibbonPanelSource"></param>
        /// <param name="command">按钮关联的命令</param>
        /// <param name="showtext">按钮显示的文字</param>
        /// <param name="content">说明</param>
        /// <param name="bitmap">按钮Icon</param>
        /// <param name="size">按钮大小</param>
        /// <param name="orientation">按钮排列方式</param>
        public static void AddButton(this RibbonPanelSource RibbonPanelSource, string command, string showtext, string content, System.Drawing.Bitmap bitmap, RibbonItemSize size, System.Windows.Controls.Orientation orientation)
        {
            RibbonButton btn = new RibbonButton();
            btn.Name = command;
            btn.Text = showtext;
            btn.ShowText = true;
            BitmapImage bitmapImage = sCadRibbonTool.ToBitmapImage(bitmap);
            btn.Image = bitmapImage;
            btn.LargeImage = bitmapImage;
            btn.ShowImage = true;
            btn.Size = size;
            btn.Orientation = orientation;
            RibbonToolTip toolTip = new RibbonToolTip();
            toolTip.Title = command;
            toolTip.Content = content;
            toolTip.Command = command;
            btn.ToolTip = toolTip;
            btn.CommandHandler = new RibbonCommandHandler();
            btn.CommandParameter = string.Concat(command, " ");
            RibbonPanelSource.Items.Add(btn);
        }
    }

    public class RibbonCommandHandler : System.Windows.Input.ICommand
    {
        public bool CanExecute(object parameter)
          {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter is RibbonButton)
            {
                RibbonButton btn = (RibbonButton)parameter;
                if (btn.CommandParameter != null)
                {
                    Document doc = Application.DocumentManager.MdiActiveDocument;
                    doc.SendStringToExecute(btn.CommandParameter.ToString(), true, false, true);
                }
            }
        }
    }
}
