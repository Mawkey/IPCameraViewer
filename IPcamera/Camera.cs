using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using Timer = System.Timers.Timer;

namespace IPcamera
{
    public enum Dir
    {
        Up,
        Right,
        Down,
        Left
    }

    class Camera
    {
        const double MESSAGE_INTERVAL = 1000;
        const double MESSAGE_INTERVAL_EMPTY_QUEUE = 3000;

        public VideoView view;
        public string address;
        public string username;
        public string password;
        public string onvifAddress;
        LibVLC libVLC;

        ContextMenu cmenu;


        Controller controller;

        bool hasReleasedKey;

        public delegate void CameraClickedDel();
        public event CameraClickedDel CameraClicked;


        // Visual stuff on top of the VLC view.
        Button btn;
        Label label;
        Border border;

        Queue<string> messageQueue;
        Timer messageTimer;
        Timer lastMessageTimer;

        public Camera(string address, string username, string password, string onvifAddress = "")
        {
            Init(address, username, password, onvifAddress);
        }

        void Init(object cam)
        {
            Camera camera = cam as Camera;

            Init(camera.address, camera.username, camera.password, camera.onvifAddress);
        }
        void Init(string address, string username, string password, string onvifAddress)
        {
            this.address = address;
            this.username = username;
            this.password = password;
            this.onvifAddress = onvifAddress;

            CreateContextMenu();

            InitMessageQueue();

            // hasReleasedKey bool is used to make sure there is no spam to ONVIF PTZ.
            hasReleasedKey = true;

            // A label to display messages ontop of the VLC player.
            label = new Label()
            {
                Width = double.NaN,
                Height = double.NaN,
                Foreground = new SolidColorBrush(Colors.White),
            };


            // grid is used to hold all controls that goes ontop of the VLC player.
            Grid grid = new Grid();

            grid.Children.Add(label);
            grid.Children.Add(btn);

            border = new Border()
            {
                Child = grid,
                BorderThickness = new Thickness(0),
                BorderBrush = new SolidColorBrush(Colors.Green)
            };

            // The VLC "player" WPF control.
            view = new VideoView() { Content = border };

            // controller is used to control ONVIF PTZ.
            controller = new Controller(true);

            controller.Initialise(onvifAddress, username, password);
        }

        /// <summary>
        /// Creates a context menu and assigns it to an invisible button that is overlayed ontop of the VLC player control (called view).
        /// </summary>
        void CreateContextMenu()
        {
            // Creating contex menu for this camera (right click menu).
            cmenu = new ContextMenu();

            // btn is used to capture mouse events since VLC player cannot invoke those events.
            // btn is overlayed ontop of VLC player.
            btn = new Button()
            {
                Style = (Style)Application.Current.FindResource("ButtonStyleNoHighlighting"),
                Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)),
                ContextMenu = cmenu
            };

            btn.Click += Btn_Click;
            btn.KeyUp += Btn_KeyUp;
            btn.KeyDown += Btn_KeyDown;

            MenuItem reloadCameraButton = new MenuItem() { Header = "Restart Camera" };
            MenuItem startCameraButton = new MenuItem() { Header = "Start Camera" };
            MenuItem stopCameraButton = new MenuItem() { Header = "Stop Camera" };
            Separator separator = new Separator() { Margin = new Thickness(0, 10, 0, 10) };
            MenuItem removeCameraButton = new MenuItem() { Header = "Remove Camera" };

            cmenu.Items.Add(reloadCameraButton);
            cmenu.Items.Add(startCameraButton);
            cmenu.Items.Add(stopCameraButton);
            cmenu.Items.Add(separator);
            cmenu.Items.Add(removeCameraButton);

            reloadCameraButton.Click += ReloadCameraButton_Click;
            startCameraButton.Click += StartCameraButton_Click;
            stopCameraButton.Click += StopCameraButton_Click;
            removeCameraButton.Click += RemoveCameraButton_Click;
        }

        private void StartCameraButton_Click(object sender, RoutedEventArgs e)
        {
            if (view.MediaPlayer.State == VLCState.Stopped)
            {
                view.MediaPlayer.Play();
            }
        }

        private void StopCameraButton_Click(object sender, RoutedEventArgs e)
        {
            if (view.MediaPlayer.State == VLCState.Playing)
            {
                view.MediaPlayer.Stop();
            }
        }

        private void ReloadCameraButton_Click(object sender, RoutedEventArgs e)
        {
            CameraManager.RestartCamera(this);
        }

        private void RemoveCameraButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(CameraManager.mainWindow,
                "Are you sure you want to remove this camera?",
                "Removing Camera",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,
                MessageBoxResult.No,
                MessageBoxOptions.None);

            if (result == MessageBoxResult.Yes)
            {
                CameraManager.RemoveCamera(this);
            }
        }

        /// <summary>
        /// Invoked when user selects this camera by clicking on it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (CameraManager.selectedCamera == this)
            {
                CameraManager.selectedCamera = null;
                Deselect();
            }
            else
            {
                CameraManager.selectedCamera = this;

                CameraManager.DeselectAllCameras();

                Select();
            }

            CameraClicked?.Invoke();
        }

        /// <summary>
        /// Initializes stuff for the message queue used by a label that is overlayed ontop of the VLC player (called view).
        /// </summary>
        void InitMessageQueue()
        {
            // Used for the simple message queue that is overlayed ontop of VLC player
            messageQueue = new Queue<string>();
            messageTimer = new Timer(MESSAGE_INTERVAL);
            messageTimer.Elapsed += MessageTimer_Elapsed;
            lastMessageTimer = new Timer(MESSAGE_INTERVAL_EMPTY_QUEUE);
            lastMessageTimer.Elapsed += LastMessageTimer_Elapsed;
        }

        private void LastMessageTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (label != null) label.Content = "";
            });
        }

        private void MessageTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (messageQueue.Count > 0)
                        label.Content = messageQueue.Dequeue();
                });

                CheckMessageQueue();
            }
        }

        private void Btn_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    CameraManager.mainWindow.FullScreenOff();
                    break;
                case Key.F11:
                    CameraManager.mainWindow.FullScreenToggle();
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

        private void Btn_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
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

        public void StopPTZ()
        {
            hasReleasedKey = true;

            controller.Stop();
        }

        public void MovePTZ(Dir direction) 
        {
            if (!hasReleasedKey) return;

            switch (direction)
            {
                case Dir.Up:
                    controller.TiltUp();
                    break;
                case Dir.Right:
                    controller.PanRight();
                    break;
                case Dir.Down:
                    controller.TiltDown();
                    break;
                case Dir.Left:
                    controller.PanLeft();
                    break;
            }
            hasReleasedKey = false;
        }

        /// <summary>
        /// User for saving Camera to a file.
        /// </summary>
        /// <returns></returns>
        public SimpleCamera GetSimpleCamera()
        {
            return new SimpleCamera(address, username, password, onvifAddress);
        }

        /// <summary>
        /// Deselects this camera visually using a border.
        /// </summary>
        public void Deselect()
        {
            border.BorderThickness = new Thickness(0);
        }

        /// <summary>
        /// Selects this camera visually using a border.
        /// </summary>
        public void Select()
        {
            border.BorderThickness = new Thickness(2);
        }





        public void Init(LibVLC libVLC)
        {
            this.libVLC = libVLC;
            view.MediaPlayer = new LibVLCSharp.Shared.MediaPlayer(libVLC);
        }

        public void Play()
        {
            Media mediaStream = new Media(libVLC, new Uri(address));
            mediaStream.StateChanged += MediaStream_StateChanged;
            view.MediaPlayer.Play(mediaStream);
            view.MediaPlayer.Mute = true;
        }

        private void MediaStream_StateChanged(object sender, MediaStateChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => 
            {
                //label.Content = e.State == VLCState.Playing ? "" : e.State.ToString();
                if (messageQueue.Count == 0)
                {
                    label.Content = e.State.ToString();
                }

                messageQueue.Enqueue(e.State.ToString());
                CheckMessageQueue();
            });
        }

        void CheckMessageQueue()
        {
            if (messageQueue.Count > 0)
            {
                lastMessageTimer.Stop();

                messageTimer.Stop();
                messageTimer.Start();
            }
            else
            {
                messageTimer.Stop();

                lastMessageTimer.Stop();
                lastMessageTimer.Start();
            }
        }

    }
}
