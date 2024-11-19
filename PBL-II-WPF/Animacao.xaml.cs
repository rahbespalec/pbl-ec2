using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace PBL_II_WPF
{
    public partial class Animacao : Window
    {
        private double largura; // Largura do rio
        private double velocidadeMotor;
        private double velocidadeCorrenteza;
        private double angulo;

        public Animacao(string larguraText, string vmaText, string vcText, string anguloText)
        {
            InitializeComponent();

            // Convertendo os parâmetros passados para double
            largura = double.Parse(larguraText, System.Globalization.CultureInfo.InvariantCulture);
            velocidadeMotor = double.Parse(vmaText, System.Globalization.CultureInfo.InvariantCulture);
            velocidadeCorrenteza = double.Parse(vcText, System.Globalization.CultureInfo.InvariantCulture);
            angulo = double.Parse(anguloText, System.Globalization.CultureInfo.InvariantCulture);

            AjustarLarguraRio();
        }
        private void AjustarLarguraRio()
        {
            // Ajustar o retângulo laranja para preencher todo o canvas
            RetanguloLaranja.Width = CanvasPrincipal.Width;
            RetanguloLaranja.Height = CanvasPrincipal.Height;
            Canvas.SetLeft(RetanguloLaranja, 0);
            Canvas.SetTop(RetanguloLaranja, 0);

            // Manter a imagem do rio preenchendo todo o canvas
            ImagemRio.Width = 769;
            ImagemRio.Height = 394;
            Canvas.SetTop(ImagemRio, -94);
            Canvas.SetLeft(ImagemRio, 2);

            // Calcular posições centrais
            double posicaoMargemEsquerda = (CanvasPrincipal.Width - largura) / 2 - MargemEsquerda.Width;
            double posicaoMargemDireita = (CanvasPrincipal.Width + largura) / 2;

            // Configurar container da margem esquerda
            ContainerMargemEsquerda.Width = MargemEsquerda.Width;
            Canvas.SetLeft(ContainerMargemEsquerda, posicaoMargemEsquerda);
            Canvas.SetLeft(MargemEsquerda, 0);

            // Configurar container da margem direita
            ContainerMargemDireita.Width = 200; // Largura fixa do container
            Canvas.SetLeft(ContainerMargemDireita, posicaoMargemDireita);
            Canvas.SetLeft(MargemDireita, 0);

            // Atualizar o texto da largura
            if (WidthValue != null)
            {
                WidthValue.Text = largura.ToString();
            }
        }

        private void IniciarAnimacao()
        {
            // Calcular o vetor de velocidade resultante
            double[] vetorVelocidadeRes = CalcularVetorVelocidadeRes(velocidadeMotor, velocidadeCorrenteza, angulo);
            double tempoTravessia = largura / vetorVelocidadeRes[0]; // Tempo baseado na velocidade horizontal

            // Criar uma animação
            var storyboard = new Storyboard();

            // Calcular os deslocamentos nas direções X e Y baseados no ângulo
            double deslocamentoX = vetorVelocidadeRes[0] * tempoTravessia; // Deslocamento na direção horizontal
            double deslocamentoY = vetorVelocidadeRes[1] * tempoTravessia; // Deslocamento na direção vertical

            // Adicionar 20 pixels à posição final para que o barco passe 20px além da margem direita
            double deslocamentoXFinal = Canvas.GetLeft(Barco) + deslocamentoX + 25;

            // Animação para o movimento na direção X
            var animacaoX = new DoubleAnimation
            {
                From = Canvas.GetLeft(Barco),
                To = deslocamentoXFinal, // Novo valor de To com o deslocamento extra de 20px
                Duration = TimeSpan.FromSeconds(tempoTravessia)
            };

            Storyboard.SetTarget(animacaoX, Barco);
            Storyboard.SetTargetProperty(animacaoX, new PropertyPath("(Canvas.Left)"));

            // Animação para o movimento na direção Y
            var animacaoY = new DoubleAnimation
            {
                From = Canvas.GetTop(Barco),
                To = Canvas.GetTop(Barco) + deslocamentoY,
                Duration = TimeSpan.FromSeconds(tempoTravessia)
            };

            Storyboard.SetTarget(animacaoY, Barco);
            Storyboard.SetTargetProperty(animacaoY, new PropertyPath("(Canvas.Top)"));

            // Adicionar as animações ao storyboard
            storyboard.Children.Add(animacaoX);
            storyboard.Children.Add(animacaoY);
            storyboard.Begin();
        }

        private double[] CalcularVetorVelocidadeRes(double velocidadeMotor, double velocidadeCorrenteza, double angulo)
        {
            double anguloRad = angulo * (Math.PI / 180);

            // Calcular as componentes da velocidade
            return new double[] {
                velocidadeMotor * Math.Cos(anguloRad) + velocidadeCorrenteza, // Velocidade horizontal
                velocidadeMotor * Math.Sin(anguloRad) // Velocidade vertical
            };
        }

        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            IniciarAnimacao();
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}