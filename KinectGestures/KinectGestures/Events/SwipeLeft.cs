using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using Microsoft.Kinect;
using Microsoft.Samples.Kinect.SwipeGestureRecognizer;

namespace KinectGestures
{
    [MayhemModule("Swipe Left", "Triggers when right hand is swiped from right to left in front of a Kinect sensor plugged in")]
    public class SwipeLeft : EventBase
    {
        protected override void OnAfterLoad()
        {
            if(KinectManager.IsDisconnected) KinectManager.InitializeSensor();
            KinectManager.swipeLeft += new SwipeEventHandler(KinectManager_swipeLeft);
        }

        void KinectManager_swipeLeft(object sender, EventArgs e)
        {
            Trigger();
        }
    }
}
