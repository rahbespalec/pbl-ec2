using System;
using System.Windows;

namespace PBL_II_WPF
{
    /// <summary>
    /// Interaction logic for Dados.xaml
    /// </summary>
    public partial class Dados : Window
    {
        // Propriedade para receber os dados
        public DadosInseridos DadosContexto { get; set; }

        public Dados(DadosInseridos dados)
        {
            InitializeComponent();

            // Atribui o objeto DadosInseridos ao DataContext
            DadosContexto = dados;
            this.DataContext = DadosContexto;
        }

        // Adicionando o método btnVoltar_Click
        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            var principal = new Principal();

            // Mantém o estado de maximizado/restaurado da janela
            if (this.WindowState == WindowState.Maximized)
            {
                principal.WindowState = WindowState.Maximized;
            }
            else
            {
                var windowPosition = this.RestoreBounds;
                principal.Width = windowPosition.Width;
                principal.Height = windowPosition.Height;
                principal.Left = windowPosition.Left;
                principal.Top = windowPosition.Top;
            }

            principal.Show();
            this.Close();
        }
    }

    public class DadosInseridos
    {
        public double Largura { get; set; }
        public double VelocidadeMotor { get; set; }
        public double VelocidadeCorrenteza { get; set; }
        public double Angulo { get; set; }
        public double VelocidadeResultante { get; set; }
        public double TempoTravessia { get; set; }
        public double DistanciaX { get; set; }
        public double DistanciaY { get; set; }
        public double TempoMinimo { get; set; }
    }
}
