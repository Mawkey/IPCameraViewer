using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Net;
using LibVLCSharp.WPF;
using LibVLCSharp.Shared;
using System.Windows.Media;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;

namespace IPcamera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Grid cameraGrid;
        int gridWidth, gridHeight;
        GridSizeWindow gridSizeWindow;
        AddCameraWindow addCameraWindow;

        public MainWindow()
        {
            InitializeComponent();

            Thread myThread = new Thread(new ThreadStart(Init));
            myThread.IsBackground = true;
            myThread.Name = "MainInitThread";
            myThread.SetApartmentState(ApartmentState.STA);
            myThread.Start();
        }

        void Init()
        {
            CameraManager.mainWindow = this;

            if (File.Exists(SaveCameras.saveDataPath))
            {
                // Load data
                SaveCameras.Load();

                gridWidth = SaveData.gridWidth;
                gridHeight = SaveData.gridHeight;



                CameraManager.LoadSavedCameras();
            }
            else
            {
                // Use default data
                gridWidth = 1;
                gridHeight = 1;
            }

            Dispatcher.Invoke(() => 
            { 
                CreateGrid(gridWidth, gridHeight);
            });
        } 

        private void MenuBtnSetGridSize_Click(object sender, RoutedEventArgs e)
        {
            gridSizeWindow = new GridSizeWindow();
            gridSizeWindow.Closing += GridSizeWindow_Closing;
            gridSizeWindow.ShowDialog();
        }

        private void GridSizeWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!gridSizeWindow.hasUnsavedChanges) return;

            gridWidth = (int)gridSizeWindow.GridWidth;
            gridHeight = (int)gridSizeWindow.GridHeight;

            SaveData.gridHeight = gridHeight;
            SaveData.gridWidth = gridWidth;

            SaveCameras.Save();

            CreateGrid(gridWidth, gridHeight);

            gridSizeWindow.Closing -= GridSizeWindow_Closing;
        }

        void CreateGrid(int width, int height)
        {
            if (cameraGrid != null && mainGrid.Children.Contains(cameraGrid))
            {
                cameraGrid.Children.Clear();
                mainGrid.Children.Remove(cameraGrid);
                CameraManager.StopAllCameras();
            }
            cameraGrid = new Grid();
            cameraGrid.Width = Double.NaN;
            cameraGrid.Height = Double.NaN;

            cameraGrid.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 50, 50, 50));
            Grid.SetRow(cameraGrid, 1);
            mainGrid.Children.Add(cameraGrid);

            for (int i = 0; i < width; i++)
            {
                cameraGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < height; i++)
            {
                cameraGrid.RowDefinitions.Add(new RowDefinition());
            }


            InsertCamerasToGrid();
        }

        private void MenuBtnAddCamera_Click(object sender, RoutedEventArgs e)
        {
            addCameraWindow = new AddCameraWindow();
            addCameraWindow.Closing += AddCameraWindow_Closing;
            addCameraWindow.ShowDialog();
        }

        private void AddCameraWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!addCameraWindow.HasUnsavedChanges) return;

            CameraManager.CreateCamera(addCameraWindow.Address,
                                       addCameraWindow.username,
                                       addCameraWindow.password,
                                       addCameraWindow.onvifAddress);
            SaveCameras.Save();

            CreateGrid(gridWidth, gridHeight);

            addCameraWindow.Closing -= AddCameraWindow_Closing;
        }

        private void MenuBtnRestartCameras_Click(object sender, RoutedEventArgs e)
        {
            CameraManager.RestartAllCameras();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            // Same code can be found on Camera class itself because WPF VLC Control is considered it's own window.
            switch (e.Key)
            {
                case Key.Escape:
                    FullScreenOff();
                    break;
                case Key.F11:
                    FullScreenToggle();
                    break;
                case Key.Up:
                    if (CameraManager.selectedCamera != null) CameraManager.selectedCamera.StopPTZ();
                    break;
                case Key.Right:
                    if (CameraManager.selectedCamera != null) CameraManager.selectedCamera.StopPTZ();
                    break;
                case Key.Down:
                    if (CameraManager.selectedCamera != null) CameraManager.selectedCamera.StopPTZ();
                    break;
                case Key.Left:
                    if (CameraManager.selectedCamera != null) CameraManager.selectedCamera.StopPTZ();
                    break;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Same code can be found on Camera class itself because WPF VLC Control is considered it's own window.
            switch (e.Key)
            {
                case Key.Up:
                    if (CameraManager.selectedCamera != null) CameraManager.selectedCamera.MovePTZ(Dir.Up);
                    break;
                case Key.Right:
                    if (CameraManager.selectedCamera != null) CameraManager.selectedCamera.MovePTZ(Dir.Right);
                    break;
                case Key.Down:
                    if (CameraManager.selectedCamera != null) CameraManager.selectedCamera.MovePTZ(Dir.Down);
                    break;
                case Key.Left:
                    if (CameraManager.selectedCamera != null) CameraManager.selectedCamera.MovePTZ(Dir.Left);
                    break;
            }
        }


        public void FullScreenToggle()
        {
            if (WindowStyle == WindowStyle.SingleBorderWindow)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
            }
        }

        public void FullScreenOff()
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Normal;
        }


        void InsertCamerasToGrid()
        {

            int cameraIndex = 0;
            if (cameraGrid.Children.Count > 0) cameraGrid.Children.Clear();

            Thread camThread;

            for (int y = 0; y < gridWidth; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    if (cameraIndex < CameraManager.cameras.Count)
                    {
                        camThread = new Thread(new ParameterizedThreadStart((object cameraIndexObj) => 
                        {
                            Dispatcher.Invoke(() =>
                            {
                                CameraManager.cameras[(int)cameraIndexObj].Play();
                            });
                        }));
                        camThread.Start(cameraIndex);
                        VideoView view = CameraManager.cameras[cameraIndex].view;
                        cameraGrid.Children.Add(view);
                        Grid.SetColumn(view, x);
                        Grid.SetRow(view, y);

                        cameraIndex++;
                    }
                }
            }

        }
    }
}
