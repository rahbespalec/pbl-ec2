using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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

namespace PBL_II_WPF
{
    /// <summary>
    /// Interaction logic for Instrucao.xaml
    /// </summary>
    public partial class Instrucao : Window
    {
        public Instrucao()
        {
            InitializeComponent();
        }
        private void PlayClickSound()
        {
            SoundPlayer player = new SoundPlayer("click-sound.wav");
            player.Play();
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            // Cria uma nova instância da janela instrução
            var principal = new Principal();

            // Verifica o estado atual da janela principal
            if (this.WindowState == WindowState.Maximized)
            {
                // Se a janela principal está maximizada, a nova janela também será maximizada
                principal.WindowState = WindowState.Maximized;
            }
            else
            {
                // Caso contrário, configura as dimensões e posição da janela principal
                var windowPosition = this.RestoreBounds;
                principal.Width = windowPosition.Width; // Define a largura
                principal.Height = windowPosition.Height; // Define a altura
                principal.Left = windowPosition.Left; // Define a posição X
                principal.Top = windowPosition.Top; // Define a posição Y
            }

            principal.Show(); // Exibe a janela de instrução
            this.Close(); // Fecha a janela principal
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            // Cria uma nova instância da janela instrução
            var inicio = new MainWindow();

            // Verifica o estado atual da janela principal
            if (this.WindowState == WindowState.Maximized)
            {
                // Se a janela principal está maximizada, a nova janela também será maximizada
                inicio.WindowState = WindowState.Maximized;
            }
            else
            {
                // Caso contrário, configura as dimensões e posição da janela principal
                var windowPosition = this.RestoreBounds;
                inicio.Width = windowPosition.Width; // Define a largura
                inicio.Height = windowPosition.Height; // Define a altura
                inicio.Left = windowPosition.Left; // Define a posição X
                inicio.Top = windowPosition.Top; // Define a posição Y
            }

            inicio.Show(); // Exibe a janela de instrução
            this.Close(); // Fecha a janela principal
        }

    }
}
