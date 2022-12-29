// <copyright file="AppDelegate.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using AVFoundation;
using Drastic.PureLayout;
using Drastic.Tray;
using Drastic.TrayWindow;

namespace Drastic.Camera.MacOS;

[Register("AppDelegate")]
public class AppDelegate : NSApplicationDelegate
{
    private TrayIcon? trayIcon;

    private NSTrayWindow? trayWindow;

    public override void DidFinishLaunching(NSNotification notification)
    {
        var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVAuthorizationMediaType.Video);
        var menuItems = new List<TrayMenuItem>();
        var trayImage = new TrayImage(NSImage.GetSystemSymbol("web.camera", null)!);
        menuItems.Add(new TrayMenuItem("Quit", null, async () => { NSApplication.SharedApplication.Terminate(this); }, "q"));
        this.trayIcon = new Drastic.Tray.TrayIcon("Drastic.Camera", trayImage, menuItems, true);
        this.trayWindow = new NSTrayWindow(this.trayIcon, new TrayWindowOptions(width: 300, height: 200), new WebcamViewController(authorizationStatus));
        this.trayIcon.LeftClicked += (object? sender, TrayClickedEventArgs e) => this.trayWindow.ToggleVisibility();
        this.trayIcon.RightClicked += (object? sender, TrayClickedEventArgs e) => this.trayIcon.OpenMenu();
    }

    public override void WillTerminate(NSNotification notification)
    {
        // Insert code here to tear down your application
    }
}
