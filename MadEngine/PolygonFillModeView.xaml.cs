using OpenTK.Graphics.OpenGL;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MadEngine
{
    /// <summary>
    /// Interaction logic for PolygonFillModeView.xaml
    /// </summary>
    public partial class PolygonFillModeView : UserControl
    {
        public PolygonFillModeView()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            rbLines.IsChecked = true;
        }

        private void rbLines_Checked(object sender, RoutedEventArgs e)
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        }

        private void rbFill_Checked(object sender, RoutedEventArgs e)
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
    }
}
