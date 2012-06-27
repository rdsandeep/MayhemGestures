using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.Kinect.SwipeGestureRecognizer;
using Microsoft.Kinect;
using System.IO;

namespace KinectGestures
{
    public delegate void SwipeEventHandler(object sender, EventArgs e);

    public static class KinectManager
    {
        public static Recognizer activeRecognizer;
        public static KinectSensor kinect;
        public static bool isDisconnectedField = true;
        public static Skeleton[] skeletons = new Skeleton[0];
        public static bool isInitialized = false;
        public static event SwipeEventHandler swipeLeft;
        public static event SwipeEventHandler swipeRight;

        public static bool IsDisconnected
        {
            get
            {
                return isDisconnectedField;
            }

            private set
            {
                if (isDisconnectedField != value)
                {
                    isDisconnectedField = value;
                }
            }
        }

        public static void InitializeSensor()
        {
            UninitializeSensor();
            activeRecognizer = CreateRecognizer();
            var index = 0;
            while (kinect == null && index < KinectSensor.KinectSensors.Count)
            {
                try
                {
                    kinect = KinectSensor.KinectSensors[index];

                    kinect.Start();

                    IsDisconnected = false;
                }
                catch (IOException ex)
                {
                    kinect = null;
                }
                catch (InvalidOperationException ex)
                {
                    kinect = null;
                }

                index++;
            }

            if (kinect != null)
            {
                kinect.SkeletonStream.Enable();

                kinect.SkeletonFrameReady += OnSkeletonFrameReady;
            }
        }

        public static void UninitializeSensor()
        {
            if (kinect != null)
            {
                kinect.SkeletonFrameReady -= OnSkeletonFrameReady;

                kinect.Stop();

                kinect = null;
            }

            IsDisconnected = true;
        }

        public static void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    if (skeletons.Length != frame.SkeletonArrayLength)
                    {
                        skeletons = new Skeleton[frame.SkeletonArrayLength];
                    }

                    frame.CopySkeletonDataTo(skeletons);

                    var newNearestId = -1;
                    var nearestDistance2 = double.MaxValue;

                    foreach (var skeleton in skeletons)
                    {
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            var distance2 = (skeleton.Position.X * skeleton.Position.X) +
                                (skeleton.Position.Y * skeleton.Position.Y) +
                                (skeleton.Position.Z * skeleton.Position.Z);

                            if (distance2 < nearestDistance2)
                            {
                                newNearestId = skeleton.TrackingId;
                                nearestDistance2 = distance2;
                            }
                        }
                    }
                    activeRecognizer.Recognize(sender, frame, skeletons);
                }
            }
        }

        public static Recognizer CreateRecognizer()
        {
            var recognizer = new Recognizer();
            recognizer.SwipeRightDetected += (s, e) =>
            {
                swipeLeft(s, e);
            };
            recognizer.SwipeLeftDetected += (s, e) =>
            {
                swipeRight(s, e);
            };
            return recognizer;
        }
    }
}
