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

namespace PBL_II_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnJogar_Click(object sender, RoutedEventArgs e)
        {
            // Cria uma nova instância da janela instrução
            var instrução = new Instrucao();

            // Verifica o estado atual da janela principal
            if (this.WindowState == WindowState.Maximized)
            {
                // Se a janela principal está maximizada, a nova janela também será maximizada
                instrução.WindowState = WindowState.Maximized;
            }
            else
            {
                // Caso contrário, configura as dimensões e posição da janela principal
                var windowPosition = this.RestoreBounds;
                instrução.Width = windowPosition.Width; // Define a largura
                instrução.Height = windowPosition.Height; // Define a altura
                instrução.Left = windowPosition.Left; // Define a posição X
                instrução.Top = windowPosition.Top; // Define a posição Y
            }

            instrução.Show(); // Exibe a janela de instrução
            this.Close(); // Fecha a janela principal
        }

    }
}
