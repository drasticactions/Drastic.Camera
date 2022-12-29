// <copyright file="Main.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Camera.MacOS;

// This is the main entry point of the application.
NSApplication.Init();

var application = NSApplication.SharedApplication!;
application.ActivationPolicy = NSApplicationActivationPolicy.Accessory;
application.Delegate = new AppDelegate();

NSApplication.Main(args);