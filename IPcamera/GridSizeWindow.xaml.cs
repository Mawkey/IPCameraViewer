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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IPcamera
{
    /// <summary>
    /// Interaction logic for GridSizeWindow.xaml
    /// </summary>
    public partial class GridSizeWindow : Window
    {
        public bool hasUnsavedChanges { get; private set; }
        int? gridWidth;
        int? gridHeight;

        public int? GridWidth
        {
            get { return gridWidth; }
            set
            {
                if (value <= 0 || value == null)
                {
                    throw new ArgumentException("Width needs to be more than 0.");
                }
                gridWidth = value;
            }
        }

        public int? GridHeight
        {
            get { return gridHeight; }
            set
            {
                if (value <= 0 || value == null)
                {
                    throw new ArgumentException("Height needs to be more than 0.");
                }
                gridHeight = value;
            }
        }

        public GridSizeWindow()
        {
            InitializeComponent();
            hasUnsavedChanges = false;
            
            if (SaveData.gridWidth <= 0 || SaveData.gridHeight <= 0)
            {
                GridWidth = 1;
                GridHeight = 1;
            }
            else
            {
                GridWidth = SaveData.gridWidth;
                GridHeight = SaveData.gridHeight;
            }


            intInputWidth.Value = GridWidth;
            intInputHeight.Value = GridHeight;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (intInputWidth.Value > 0 && intInputHeight.Value > 0 && intInputWidth.Value != null && intInputHeight.Value != null)
            {
                GridWidth = intInputWidth.Value;
                GridHeight = intInputHeight.Value;
                hasUnsavedChanges = true;
                Close();
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            hasUnsavedChanges = false;
            Close();
        }
    }
}
