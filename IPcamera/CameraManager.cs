using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IPcamera
{

    static class CameraManager
    {
        public static List<Camera> cameras;
        static readonly LibVLC libVLC;

        public static Camera selectedCamera;

        public static MainWindow mainWindow;

        static CameraManager()
        {
            cameras = new List<Camera>();
            Core.Initialize();
            libVLC = new LibVLC();
        }

        public static Camera CreateCamera(string address, string username, string password, string onvifAddress)
        {

            Camera myCamera = new Camera(address, username, password, onvifAddress);

            // LibVLC is needed for VideoViews to work. In order for JSON save data to not be a mess,
            // I don't init the Cameras with LibVLC in the constructor but rather init them using this method after creating the Camera object.
            myCamera.Init(libVLC);
            cameras.Add(myCamera);
            SaveData.cameras = cameras;

            return myCamera;
        }

        public static void LoadSavedCameras()
        {
            Camera[] savedCamerasSnapShot = new Camera[SaveData.cameras.Count];
            SaveData.cameras.CopyTo(savedCamerasSnapShot);

            for (int i = 0; i < savedCamerasSnapShot.Length; i++)
            {
                Application.Current.Dispatcher.Invoke(() => 
                { 
                    CreateCamera(savedCamerasSnapShot[i].address, 
                                    savedCamerasSnapShot[i].username, 
                                    savedCamerasSnapShot[i].password, 
                                    savedCamerasSnapShot[i].onvifAddress);
                });
            }
        }

        public static void RestartAllCameras()
        {
            StopAllCameras();
            StartAllCameras();
        }

        public static void StopAllCameras()
        {
            foreach (Camera camera in cameras)
            {
                camera.view.MediaPlayer.Stop();
            }
        }

        public static void StartAllCameras()
        {
            foreach (Camera camera in cameras)
            {
                camera.view.MediaPlayer.Play();
            }
        }

        public static void DeselectAllCameras()
        {
            foreach (Camera camera in cameras)
            {
                camera.Deselect();
            }
        }

        public static void RemoveCamera(Camera camera)
        {
            camera.view.MediaPlayer.Stop();
            camera.view.Dispose();
            SaveData.cameras.Remove(camera);
            cameras.Remove(camera);
            SaveCameras.Save();
        }

        public static void RestartCamera(Camera camera)
        {
            camera.view.MediaPlayer.Stop();
            camera.view.MediaPlayer.Play();
        }

        /// <summary>
        /// Does null checking. Returns false if no camera is selected and true otherwise.
        /// </summary>
        /// <param name="selectedCamera"></param>
        /// <returns></returns>
        public static bool GetSelectedCamera(out Camera selectedCamera)
        {
            if (CameraManager.selectedCamera != null)
            {
                selectedCamera = CameraManager.selectedCamera;
                return true;
            }
            else
            {
                selectedCamera = null;
                return false;
            }
        }
    }
}
