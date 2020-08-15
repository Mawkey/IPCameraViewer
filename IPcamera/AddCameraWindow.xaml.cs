using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IPcamera
{
    /// <summary>
    /// Interaction logic for AddCameraWindow.xaml
    /// </summary>
    public partial class AddCameraWindow : Window
    {
        public bool HasUnsavedChanges { get; private set; }
        public string Address { get; private set; }

        public string onvifAddress { get; private set; }

        public string username { get; private set; }
        public string password { get; private set; }

        public AddCameraWindow()
        {
            InitializeComponent();

            HasUnsavedChanges = false;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Address = TextBoxCameraAddress.Text;
            onvifAddress = textBoxOnvifAddress.Text;
            username = textBoxUsername.Text;
            password = passwordBox.Password;
            HasUnsavedChanges = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            HasUnsavedChanges = false;
            Close();
        }
    }
}
