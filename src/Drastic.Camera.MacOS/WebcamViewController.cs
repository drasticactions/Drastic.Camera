// <copyright file="WebcamViewController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using AVFoundation;
using CoreAnimation;
using Drastic.PureLayout;

namespace Drastic.Camera.MacOS
{
    public class WebcamViewController : NSViewController
    {
        private AVCaptureSession session;
        private PreviewView previewView;
        private PermissionView permissionView = new PermissionView();
        private AVAuthorizationStatus status;
        private AVCaptureDevice? device;

        private bool Granted => this.status == AVAuthorizationStatus.Authorized;

        public WebcamViewController(AVAuthorizationStatus status)
        {
            this.session = new AVCaptureSession();
            this.previewView = new PreviewView(this.session);
            this.status = status;
            this.permissionView.Requested += this.PermissionView_Requested;
        }

        private void PermissionView_Requested(object? sender, AVCaptureDeviceRequestEventArgs e)
        {
            this.status = e.Status;
            if (this.Granted)
            {
                this.SetupPreviewView();
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Not granted, show permission screen.
            if (!this.Granted)
            {
                this.SetupPermissionsView();
            }
            else
            {
                this.SetupPreviewView();
            }

            this.View.AutoSetDimensionsToSize(new CGSize(300, 200));
        }

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();

            if (this.Granted && this.device is not null)
            {
                this.session.StartRunning();
                this.previewView.AutoPinEdgesToSuperviewEdges();
            }
        }

        public override void ViewDidDisappear()
        {
            base.ViewDidDisappear();

            if (this.session.Running)
            {
                this.session.StopRunning();
            }
        }

        private void SetupPreviewView()
        {
            this.permissionView.RemoveFromSuperview();

            this.View.AddSubview(this.previewView);
            this.previewView.AutoPinEdgesToSuperviewEdges();

            if (!this.session.Running)
            {
                this.StartTempSession();
            }
        }

        private void StartTempSession()
        {
            if (this.device is null)
            {
                this.device = AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);
                if (this.device is not null)
                {
                    var avcaptureinputdevice = new AVCaptureDeviceInput(this.device, out _);
                    this.session.AddInput(avcaptureinputdevice);
                    this.session.StartRunning();
                }
            }
        }

        private void SetupPermissionsView()
        {
            this.previewView.RemoveFromSuperview();
            this.View.AddSubview(this.permissionView);

            this.permissionView.AutoPinEdgesToSuperviewEdgesWithInsets(new NSEdgeInsets(10, 10, 10, 10));
        }

        public override void LoadView()
        {
            this.View = new NSView();
        }

        private class PreviewView : NSView
        {
            private AVCaptureSession session;
            private AVCaptureVideoPreviewLayer preview;

            public PreviewView(AVCaptureSession session)
            {
                this.session = session;
                this.preview = new AVCaptureVideoPreviewLayer(this.session);
                this.preview.Frame = this.Frame;
                this.preview.ContentsGravity = CALayer.GravityResizeAspectFill;
                this.preview.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
                this.Layer = this.preview;
            }
        }

        private class PermissionView : NSView
        {
            private NSTextField permissionLabel = new NSTextField()
            {
                StringValue = "In order to use this application, you must allow access to your webcam.",
                Editable = false,
                Bezeled = false,
                MaximumNumberOfLines = 0,
                BackgroundColor = NSColor.FromRgba(0, 0, 0, 0),
            };

            private NSButton allowButton = new NSButton() { BezelStyle = NSBezelStyle.Rounded, Title = "Allow", };

            public PermissionView()
            {
                this.AddSubview(this.permissionLabel);
                this.AddSubview(this.allowButton);

                this.permissionLabel.AutoPinEdgeToSuperviewEdge(ALEdge.Left);
                this.permissionLabel.AutoPinEdgeToSuperviewEdge(ALEdge.Right);
                this.permissionLabel.AutoAlignAxis(ALAxis.Horizontal, this);

                this.allowButton.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, this.permissionLabel, 15f);
                this.allowButton.AutoAlignAxis(ALAxis.Vertical, this.permissionLabel);

                this.allowButton.Activated += this.AllowButton_Activated;
            }

            public event EventHandler<AVCaptureDeviceRequestEventArgs>? Requested;

            private void AllowButton_Activated(object? sender, EventArgs e)
            {
                //this.Requested?.Invoke(this, new AVCaptureDeviceRequestEventArgs(AVAuthorizationStatus.Authorized));
                AVCaptureDevice.RequestAccessForMediaType(AVAuthorizationMediaType.Video, (complete) => {
                    this.Requested?.Invoke(this, new AVCaptureDeviceRequestEventArgs(AVCaptureDevice.GetAuthorizationStatus(AVAuthorizationMediaType.Video)));
                });
            }
        }

        private class AVCaptureDeviceRequestEventArgs : EventArgs
        {
            public AVAuthorizationStatus Status { get; }

            public AVCaptureDeviceRequestEventArgs(AVAuthorizationStatus complete)
            {
                this.Status = complete;
            }
        }
    }
}