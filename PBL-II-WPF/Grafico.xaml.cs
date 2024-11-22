using OxyPlot.Wpf;
using System.Collections.Generic;
using System.Media;
using System.Windows;

namespace PBL_II_WPF
{
    public partial class GraficoWindow : Window
    {
        private readonly double[] _vetorVelocidadeRes;
        private readonly double _tempoTravessia;
        private readonly List<System.Windows.Point> _pontosTrajeto;

        public GraficoWindow(double[] vetorVelocidadeRes, double tempoTravessia, List<System.Windows.Point> pontosTrajeto)
        {
            InitializeComponent();
            _vetorVelocidadeRes = vetorVelocidadeRes;
            _tempoTravessia = tempoTravessia;
            _pontosTrajeto = pontosTrajeto;
            // Gerar o gráfico assim que a janela for carregada
            GerarGrafico();
        }
        private void PlayClickSound()
        {
            SoundPlayer player = new SoundPlayer("click-sound.wav");
            player.Play();
        }
        private void GerarGrafico()
        {
            // Criar o modelo do gráfico
            var model = new OxyPlot.PlotModel { Title = "Trajetória do Barco" };

            // Adicionar uma série de pontos
            var series = new OxyPlot.Series.ScatterSeries
            {
                Title = "Posições no Tempo",
                MarkerType = OxyPlot.MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyPlot.OxyColors.Blue
            };

            // Adicionar uma linha para a trajetória
            var lineSeries = new OxyPlot.Series.LineSeries
            {
                Title = "Trajetória",
            };

            // Adicionar os pontos com base no cálculo
            for (int i = 0; i < _pontosTrajeto.Count; i++)
            {
                var ponto = _pontosTrajeto[i];
                lineSeries.Points.Add(new OxyPlot.DataPoint(ponto.X, ponto.Y));

                // Adicionar marcadores de tempo espaçados
                if (i % 1 == 0)
                {
                    series.Points.Add(new OxyPlot.Series.ScatterPoint(ponto.X, ponto.Y));
                }
            }

            // Adicionar as séries ao modelo do gráfico
            model.Series.Add(lineSeries);
            model.Series.Add(series);

            // Configurar o PlotView
            PlotView.Model = model;
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            // Cria uma nova instância da janela principal
            var principal = new Principal();

            // Verifica o estado atual da janela
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
}