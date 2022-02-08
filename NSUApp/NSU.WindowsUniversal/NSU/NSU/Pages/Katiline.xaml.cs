// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft Corporation. All rights reserved 

using Windows.UI.Xaml.Controls;
using NSU.Shared.NSUUI;
using NSU.NSU_UWP.UI;
using NSU.NSU_UWP.UI.Elements;
using NSU.Shared.NSUUI.NSUWindows;
using NSU.Shared;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace NSU.NSU_UWP.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PageKatiline : Page
    {
        const string LogTag = "PageKatiline";
        NSUWindow mainWindow;
        Shared.NSUUI.Scenario scn;
        WindowsManager winman;

        public PageKatiline()
        {
            this.InitializeComponent();

            scn = new Shared.NSUUI.Scenario();
            mainWindow = new NSUWindow(KatilineContent);
            //Play Scenario
            winman = new WindowsManager();
            scn.PlayScenario(winman, null);

            winman.OnWindowsGroupChanged += OnWindowsGroupChangedHandler;
            winman.OnWindowChanged += OnWindowChangedHandler;

            var grp = winman.CurrentGroup;
            if (grp != null)
            {
                winman.ActivateWindowsGroup(grp.Name);
            }
        }

        private void OnWindowChangedHandler(object sender, System.EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnWindowChangedHandler");
            mainWindow.AttachNSUWindow(sender as INSUWindow);
        }

        private void OnWindowsGroupChangedHandler(object sender, System.EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnWindowsGroupChangedHandler");
            INSUWindowsGroup grp = sender as INSUWindowsGroup;
            //Change SideWindow
            mainWindow.AttachNSUSideWindow(grp.SideWindow);
            mainWindow.AttachNSUWindow(grp.CurrentWindow);
        }

    }
}
