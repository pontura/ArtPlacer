/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NatCamU {
    
    namespace Internals {
        
        public static class NatCamDispatch {
            
            public static bool isRunning;
            
            private static int mainThread;
            private static List<Action> invocationQueue = new List<Action>();
            private static List<Action> executionQueue = new List<Action>();
            private static MonoBehaviour mono;
            private static Coroutine routine;
            private static readonly object queueLock = new object();
            
            public static void Prepare (MonoBehaviour Mono = null) {
                mainThread = Thread.CurrentThread.ManagedThreadId;
                mono = Mono;
                isRunning = true;
                invocationQueue = new List<Action>();
                executionQueue = new List<Action>();
                if (mono) routine = NatCamExtensions.Routine<Camera>(Update, new WaitForEndOfFrame()).Invoke(mono);
                else Camera.onPostRender += Update;
                //Log
                Debug.Log("NatCam: Initialized Dispatcher");
            }
            
            public static void Release () {
                if (!isRunning) return;
                if (routine != null) routine.Terminate(mono);
                else Camera.onPostRender -= Update;
                ClearQueue();
                isRunning = false;
                //Log
                Debug.Log("NatCam: Released Dispatcher");
            }
            
            public static void Dispatch (Action action) {
                //Check that we aren't already on the main thread
                if (Thread.CurrentThread.ManagedThreadId == mainThread) {
                    "NatCamDispatch Execute".Assert();
                    action();
                }
                //Enqueue
                else lock(queueLock) {
                    "NatCamDispatch Enqueue".Assert();
                    invocationQueue.Add(action);
                }
            }
            
            private static void Update (Camera unused) {
                "NatCamDispatch Update".Assert();
                //Lock
                lock(queueLock) {
                    "NatCamDispatch Synchronized Update".Assert();
                    executionQueue.AddRange(invocationQueue);
                    invocationQueue.Clear();
                    executionQueue.ForEach(e => e());
                    executionQueue.Clear();
                }
            }
            
            private static void ClearQueue () {
                //Lock
                lock(queueLock) {
                    invocationQueue.Clear();
                    executionQueue.Clear();
                }
            }
        }
    }
}