using OfficeOpenXml;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PBL_II_WPF
{
    /// <summary>
    /// Interaction logic for Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {
        public Principal()
        {
            InitializeComponent();
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
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

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxLargura.Text) ||
                string.IsNullOrWhiteSpace(TextBoxVMA.Text) ||
                string.IsNullOrWhiteSpace(TextBoxVC.Text) ||
                string.IsNullOrWhiteSpace(TextBoxangulo.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos.");
                return;
            }

            double largura = double.Parse(TextBoxLargura.Text, System.Globalization.CultureInfo.InvariantCulture);
            double velocidadeMotor = double.Parse(TextBoxVMA.Text, System.Globalization.CultureInfo.InvariantCulture);
            double velocidadeCorrenteza = double.Parse(TextBoxVC.Text, System.Globalization.CultureInfo.InvariantCulture);
            double angulo = double.Parse(TextBoxangulo.Text, System.Globalization.CultureInfo.InvariantCulture);

            if (!ValidarEntradas(largura, velocidadeMotor, velocidadeCorrenteza, angulo))
                return;

            double[] vetorVelocidadeRes = CalcularVetorVelocidadeRes(velocidadeMotor, velocidadeCorrenteza, angulo);
            double tempoTravessia = largura / vetorVelocidadeRes[1];

            var pontosTrajeto = CalcularCoordenadas(vetorVelocidadeRes, tempoTravessia);
            SalvarEmPlanilha(largura, velocidadeMotor, velocidadeCorrenteza, angulo);

            // Cálculo do tempo mínimo de travessia (orientação perpendicular)
            double tempoMinimo = largura / velocidadeMotor;

            // Geração do gráfico
            GerarGrafico(pontosTrajeto, tempoTravessia);

            // Alternar visibilidade
            MainGrid.Visibility = Visibility.Collapsed;
            GraphGrid.Visibility = Visibility.Visible;
        }
        private void btnAnimacao_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxLargura.Text) ||
                string.IsNullOrWhiteSpace(TextBoxVMA.Text) ||
                string.IsNullOrWhiteSpace(TextBoxVC.Text) ||
                string.IsNullOrWhiteSpace(TextBoxangulo.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos.");
                return;
            }

            double largura = double.Parse(TextBoxLargura.Text, System.Globalization.CultureInfo.InvariantCulture);
            double velocidadeMotor = double.Parse(TextBoxVMA.Text, System.Globalization.CultureInfo.InvariantCulture);
            double velocidadeCorrenteza = double.Parse(TextBoxVC.Text, System.Globalization.CultureInfo.InvariantCulture);
            double angulo = double.Parse(TextBoxangulo.Text, System.Globalization.CultureInfo.InvariantCulture);

            if (!ValidarEntradas(largura, velocidadeMotor, velocidadeCorrenteza, angulo))
                return;

            double[] vetorVelocidadeRes = CalcularVetorVelocidadeRes(velocidadeMotor, velocidadeCorrenteza, angulo);
            double tempoTravessia = largura / vetorVelocidadeRes[1];

            var pontosTrajeto = CalcularCoordenadas(vetorVelocidadeRes, tempoTravessia);
            SalvarEmPlanilha(largura, velocidadeMotor, velocidadeCorrenteza, angulo);

            // Cálculo do tempo mínimo de travessia (orientação perpendicular)
            double tempoMinimo = largura / velocidadeMotor;

            // Geração do gráfico
            GerarGrafico(pontosTrajeto, tempoTravessia);

            // Alternar visibilidade (para ocultar a janela anterior, caso seja necessário)
            MainGrid.Visibility = Visibility.Collapsed;
            GraphGrid.Visibility = Visibility.Visible;

            // Fecha a janela principal (ou minimiza, se preferir)
            var Principal = Application.Current.MainWindow;
            Principal.Hide();  // Isso vai ocultar a janela principal

            // Abrir a janela de animação com os parâmetros necessários
            Animacao animacaoWindow = new Animacao(TextBoxLargura.Text, TextBoxVMA.Text, TextBoxVC.Text, TextBoxangulo.Text);
            animacaoWindow.ShowDialog();  // Abre a janela de animação em modo modal (bloqueia a interação com a janela anterior)

            // Quando a janela de animação for fechada, mostramos a janela principal novamente
            Principal.Show();
        }




        #region Planilha
        public void SalvarEmPlanilha(double largura, double velocidadeMotor, double velocidadeCorrenteza, double angulo)
        {
            // Definir o caminho onde o arquivo será salvo
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = System.IO.Path.Combine(documentsPath, "Composição de Movimentos.xlsx");

            // Criar uma nova instância do pacote Excel
            using (var package = new ExcelPackage())
            {
                // Se o arquivo já existir, abrir a planilha
                FileInfo fileInfo = new FileInfo(filePath);
                ExcelWorksheet worksheet;

                if (fileInfo.Exists)
                {
                    // Carregar o arquivo existente usando o Stream
                    using (var stream = fileInfo.OpenRead())
                    {
                        package.Load(stream); // Carregar o arquivo existente
                    }

                    worksheet = package.Workbook.Worksheets[0]; // Seleciona a primeira planilha
                }
                else
                {
                    // Criar uma nova planilha se o arquivo não existir
                    worksheet = package.Workbook.Worksheets.Add("Composição de Movimentos");

                    // Definir os nomes das colunas na primeira linha (em negrito)
                    worksheet.Cells[1, 1].Value = "DATA";
                    worksheet.Cells[1, 2].Value = "LARGURA";
                    worksheet.Cells[1, 3].Value = "VELOCIDADE DO MOTOR";
                    worksheet.Cells[1, 4].Value = "VELOCIDADE DA CORRENTEZA";
                    worksheet.Cells[1, 5].Value = "ÂNGULO";

                    // Definir o estilo das células de título (negrito)
                    using (var range = worksheet.Cells[1, 1, 1, 5])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }
                }

                // Encontrar a última linha preenchida
                int lastRow = worksheet.Dimension?.End.Row ?? 1;

                // Adicionar os dados na próxima linha disponível
                worksheet.Cells[lastRow + 1, 1].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cells[lastRow + 1, 2].Value = largura;
                worksheet.Cells[lastRow + 1, 3].Value = velocidadeMotor;
                worksheet.Cells[lastRow + 1, 4].Value = velocidadeCorrenteza;
                worksheet.Cells[lastRow + 1, 5].Value = angulo;

                // Centralizar todas as células de dados
                using (var range = worksheet.Cells[lastRow + 1, 1, lastRow + 1, 5])
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Ajustar o tamanho das colunas para que o conteúdo se ajuste automaticamente
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Salvar o arquivo Excel
                package.SaveAs(fileInfo);
            }
        }

        #endregion

        #region Gráfico
        private void GerarGrafico(List<System.Windows.Point> pontos, double tempoTravessia)
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
            for (int i = 0; i < pontos.Count; i++)
            {
                var ponto = pontos[i];
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

        private void VoltarDoGrafico_Click(object sender, RoutedEventArgs e)
        {
            GraphGrid.Visibility = Visibility.Collapsed;
            MainGrid.Visibility = Visibility.Visible;
        }

        #endregion

        #region Cálculos

        private double[] CalcularVetorVelocidadeRes(double velocidadeMotor, double velocidadeCorrenteza, double angulo)
        {
            double anguloRad = angulo * (Math.PI / 180);

            double[] vetorVelocidadeRel = new double[] {
                velocidadeMotor * Math.Cos(anguloRad),
                velocidadeMotor * Math.Sin(anguloRad)
            };

            return new double[] {
                vetorVelocidadeRel[0] + velocidadeCorrenteza,
                vetorVelocidadeRel[1]
            };
        }

        private List<System.Windows.Point> CalcularCoordenadas(double[] vetorVelocidadeRes, double tempoTravessia)
        {
            int numIntervalos = 10;
            double deltaT = tempoTravessia / numIntervalos;
            var pontos = new List<System.Windows.Point>();

            for (int i = 0; i <= numIntervalos; i++)
            {
                double t = i * deltaT;

                double coordX = vetorVelocidadeRes[0] * t;
                double coordY = vetorVelocidadeRes[1] * t;

                pontos.Add(new System.Windows.Point(coordX, coordY));
            }

            return pontos;
        }

        #endregion

        #region Validações
        private bool ValidarEntradas(double largura, double velocidadeMotor, double velocidadeCorrenteza, double angulo)
        {
            if (!ValidarIntervalo(largura, 20, 100, "largura")) return false;
            if (!ValidarIntervalo(velocidadeMotor, 2, 10, "velocidade do motor")) return false;
            if (!ValidarIntervalo(velocidadeCorrenteza, 1, 4, "velocidade da correnteza")) return false;
            if (!ValidarIntervalo(angulo, 20, 160, "ângulo")) return false;

            return true;
        }

        private bool ValidarIntervalo(double valor, double min, double max, string campo)
        {
            if (valor < min || valor > max)
            {
                MessageBox.Show($"Por favor, insira um valor entre {min} e {max} para {campo}.");
                return false;
            }
            return true;
        }
        #endregion
    }
}
