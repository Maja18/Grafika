using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow() {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models"), "Cruiser 2012.obj", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
            }
            catch (Exception e) {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args) {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args) {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args) {
            //m_world.Resize(args.OpenGL, (int)openGLControl.Width, (int)openGLControl.Height);
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (!m_world.Animation)
                switch (e.Key) {
                    case Key.Q: this.Close(); break;
                    case Key.W: m_world.RotationX -= 5.0f; break;
                    case Key.S: m_world.RotationX += 5.0f; break;
                    case Key.A: m_world.RotationY -= 5.0f; break;
                    case Key.D: m_world.RotationY += 5.0f; break;
                    case Key.Add: m_world.SceneDistance -= 10.0f; break;
                    case Key.Subtract: m_world.SceneDistance += 10.0f; break;
                    case Key.C: m_world.DoAnimation(); break;

                }
        }

        private void scaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (m_world != null)
                m_world.Size = (float)e.NewValue;
        }
        private void doorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (m_world != null)
                m_world.Stairs = (float)e.NewValue;
        }

        private void b_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (m_world != null)
                m_world.B = (float)e.NewValue;
        }

        private void g_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (m_world != null)
                m_world.G = (float)e.NewValue;
        }

        private void r_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (m_world != null)
                m_world.R = (float)e.NewValue;
        }

        private void cb_Unchecked(object sender, RoutedEventArgs e) {
                CheckBox c = (CheckBox)sender;
                if (this.m_world == null)
                    return;

                this.m_world.Modulate = c.IsChecked == true;
            }
        private void cb_Unchecked2(object sender, RoutedEventArgs e) {
            CheckBox c = (CheckBox)sender;
            if (this.openGLControl == null)
                return;
            if(c.IsChecked == true)
                this.openGLControl.OpenGL.Enable(OpenGL.GL_LIGHT0);
            else
                this.openGLControl.OpenGL.Disable(OpenGL.GL_LIGHT0);
        }

    }
    
}
