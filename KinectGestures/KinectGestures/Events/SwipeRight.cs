using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using Microsoft.Kinect;
using Microsoft.Samples.Kinect.SwipeGestureRecognizer;

namespace KinectGestures
{
    [MayhemModule("Swipe Right", "Triggers when left hand is swiped from left to right in front of a Kinect sensor plugged in")]
    public class SwipeRight : EventBase
    {
        protected override void OnAfterLoad()
        {
            if (KinectManager.IsDisconnected) KinectManager.InitializeSensor();
            KinectManager.swipeRight += new SwipeEventHandler(KinectManager_swipeRight);
        }

        void KinectManager_swipeRight(object sender, EventArgs e)
        {
            Trigger();
        }
    }
}
