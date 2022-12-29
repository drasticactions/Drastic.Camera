# Drastic.Camera

Drastic.Camera is an open source implementation of [Hand Mirror](https://apps.apple.com/jp/app/hand-mirror/id1502839586?mt=12), written using `dotnet7.0-macos`. This is not intended to be a full clone of that software, but as a sample to show how to access various features within the macOS Binding and dotnet.

<img width="333" alt="スクリーンショット 2022-12-29 17 04 44" src="https://user-images.githubusercontent.com/898335/209922263-23f58613-f953-4540-820d-6a2895e37960.png">

Libraries Used:
- Drastic.Tray
- Drastic.TrayWindow
- Drastic.PureLayout

This app will create a tray icon, ask for permission to use your webcam, and then, when allowed, show your webcam inside of an `NSPopover`

## Known Issues:

The application may crash while the debugger is attached (such as when using VSMac) after requesting the webcam. You can work around this by running the app from CLI.