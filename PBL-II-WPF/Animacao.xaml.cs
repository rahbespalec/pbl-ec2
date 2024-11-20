using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Linq;

namespace PBL_II_WPF
{
    public partial class Animacao : Window
    {
        private double largura;
        private double velocidadeMotor;
        private double velocidadeCorrenteza;
        private double angulo;
        private const double LARGURA_MAXIMA = 600;
        private const double MARGEM_SUPERIOR_INICIAL = -19;
        private const double MARGEM_INFERIOR_INICIAL = 519;
        private const double ALTURA_RIO_ORIGINAL = 474;
        private const double BARCO_TAMANHO = 58;
        private const double MARGEM_PIXELS = 170;
        private const double ALTURA_RIO = 474;  // Altura do rio, já definida
        private const double MARGEM_DIREITA = 450;

        public Animacao(string larguraText, string vmaText, string vcText, string anguloText)
        {
            InitializeComponent();

            largura = double.Parse(larguraText, System.Globalization.CultureInfo.InvariantCulture);
            velocidadeMotor = double.Parse(vmaText, System.Globalization.CultureInfo.InvariantCulture);
            velocidadeCorrenteza = double.Parse(vcText, System.Globalization.CultureInfo.InvariantCulture);
            angulo = double.Parse(anguloText, System.Globalization.CultureInfo.InvariantCulture);

            WidthValue.Text = larguraText;
            AjustarLarguraRio();
        }

        private void AjustarLarguraRio()
        {
            // Animar a largura do rio com base nos parâmetros definidos
            if (largura >= 100)
            {
                var animacaoMargemSuperiorOriginal = new DoubleAnimation
                {
                    From = Canvas.GetTop(MargemSuperior),
                    To = MARGEM_SUPERIOR_INICIAL,
                    Duration = TimeSpan.FromSeconds(1)
                };

                var animacaoMargemInferiorOriginal = new DoubleAnimation
                {
                    From = Canvas.GetTop(MargemInferior),
                    To = MARGEM_INFERIOR_INICIAL,
                    Duration = TimeSpan.FromSeconds(1)
                };

                var rectLarguraTotal = new Rect(0, 0, RioClip.Rect.Width, ALTURA_RIO_ORIGINAL);
                var animacaoClipLarguraTotal = new RectAnimation(RioClip.Rect, rectLarguraTotal, TimeSpan.FromSeconds(1));
                RioClip.BeginAnimation(RectangleGeometry.RectProperty, animacaoClipLarguraTotal);

                MargemSuperior.BeginAnimation(Canvas.TopProperty, animacaoMargemSuperiorOriginal);
                MargemInferior.BeginAnimation(Canvas.TopProperty, animacaoMargemInferiorOriginal);
                return;
            }

            double fatorMovimento = (100 - largura) / 100;
            fatorMovimento = Math.Max(0, Math.Min(1, fatorMovimento));
            double deslocamentoMaximo = 85;
            double deslocamento = deslocamentoMaximo * fatorMovimento;

            var animacaoMargemSuperior = new DoubleAnimation
            {
                From = MARGEM_SUPERIOR_INICIAL,
                To = MARGEM_SUPERIOR_INICIAL + deslocamento,
                Duration = TimeSpan.FromSeconds(1)
            };

            var animacaoMargemInferior = new DoubleAnimation
            {
                From = MARGEM_INFERIOR_INICIAL,
                To = MARGEM_INFERIOR_INICIAL - deslocamento,
                Duration = TimeSpan.FromSeconds(1)
            };

            double novaAltura = ALTURA_RIO_ORIGINAL - (2 * deslocamento);

            var rectLarguraReduzida = new Rect(0, deslocamento, RioClip.Rect.Width, novaAltura);
            var animacaoClipLarguraReduzida = new RectAnimation(RioClip.Rect, rectLarguraReduzida, TimeSpan.FromSeconds(1));
            RioClip.BeginAnimation(RectangleGeometry.RectProperty, animacaoClipLarguraReduzida);

            MargemSuperior.BeginAnimation(Canvas.TopProperty, animacaoMargemSuperior);
            MargemInferior.BeginAnimation(Canvas.TopProperty, animacaoMargemInferior);
        }



        public class RectAnimation : AnimationTimeline
        {
            public RectAnimation(Rect from, Rect to, Duration duration)
            {
                From = from;
                To = to;
                Duration = duration;
            }

            public Rect From { get; set; }
            public Rect To { get; set; }

            public override Type TargetPropertyType => typeof(Rect);

            protected override Freezable CreateInstanceCore() => new RectAnimation(From, To, Duration);

            public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
            {
                if (animationClock.CurrentProgress == null)
                    return defaultDestinationValue;

                double progress = animationClock.CurrentProgress.Value;
                return new Rect(
                    From.X + (To.X - From.X) * progress,
                    From.Y + (To.Y - From.Y) * progress,
                    From.Width + (To.Width - From.Width) * progress,
                    From.Height + (To.Height - From.Height) * progress
                );
            }
        }

        private static double Clamp(double valor, double minimo, double maximo)
        {
            if (valor < minimo) return minimo;
            if (valor > maximo) return maximo;
            return valor;
        }

        private void IniciarAnimacao()
        {
            // Posição inicial do barco (logo acima da margem inferior)
            double posicaoInicialX = Canvas.GetLeft(Barco); // Posição inicial do barco na horizontal (no centro)

            // Calcular o deslocamento vertical com base na largura do rio
            double deslocamentoVertical = (largura - 50) * 1;  // Ajuste para fazer o deslocamento mais leve

            // A posição inicial deve ser ajustada para compensar a largura do barco
            double posicaoInicialY = ALTURA_RIO - BARCO_TAMANHO - 145 + deslocamentoVertical; // Inicia logo acima da margem inferior

            // A posição final será ajustada para garantir que o barco suba ou desça com o deslocamento correto
            double posicaoFinalY = 0 + 165 - deslocamentoVertical;  // Ajuste de altura para a margem superior

            // Posição horizontal final não é afetada pela largura
            double posicaoFinalX = posicaoInicialX;

            // Limitar ângulo para valores entre 20° e 160°
            // Implementação manual de Clamp
            angulo = Math.Max(20, Math.Min(angulo, 160));

            // Calcular o vetor de velocidade
            var vetorVelocidade = CalcularVetorVelocidade(velocidadeMotor, velocidadeCorrenteza, angulo);

            // Prevenir divisões por zero ou valores muito baixos
            double velocidadeX = vetorVelocidade[0];
            double velocidadeY = vetorVelocidade[1];

            if (Math.Abs(velocidadeX) < 0.01) velocidadeX = 0.01; // Corrigir valores muito baixos
            if (Math.Abs(velocidadeY) < 0.01) velocidadeY = 0.01;

            // Calcular o tempo necessário para a animação
            double distanciaVertical = Math.Abs(posicaoFinalY - posicaoInicialY);
            double distanciaHorizontal = Math.Abs(posicaoFinalX - posicaoInicialX);

            double tempoVertical = distanciaVertical / Math.Abs(velocidadeY);
            double tempoHorizontal = distanciaHorizontal / Math.Abs(velocidadeX);

            double tempoTotal = Math.Max(tempoVertical, tempoHorizontal);

            // Criar storyboard para a animação
            var storyboard = new Storyboard();

            // Animação vertical
            var animacaoY = new DoubleAnimation
            {
                From = posicaoInicialY,
                To = posicaoFinalY,
                Duration = TimeSpan.FromSeconds(tempoTotal),
                EasingFunction = new QuadraticEase()
            };
            Storyboard.SetTarget(animacaoY, Barco);
            Storyboard.SetTargetProperty(animacaoY, new PropertyPath("(Canvas.Top)"));

            // Animação horizontal
            var animacaoX = new DoubleAnimation
            {
                From = posicaoInicialX,
                To = posicaoFinalX + (velocidadeX * tempoTotal),
                Duration = TimeSpan.FromSeconds(tempoTotal),
                EasingFunction = new QuadraticEase()
            };
            Storyboard.SetTarget(animacaoX, Barco);
            Storyboard.SetTargetProperty(animacaoX, new PropertyPath("(Canvas.Left)"));

            // Animação de rotação
            var rotacaoTransform = Barco.RenderTransform as RotateTransform;
            if (rotacaoTransform != null)
            {
                var animacaoRotacao = new DoubleAnimation
                {
                    From = 90,
                    To = 90 - angulo, // Ajusta a rotação de acordo com o ângulo
                    Duration = TimeSpan.FromSeconds(tempoTotal),
                    EasingFunction = new QuadraticEase()
                };
                Storyboard.SetTarget(animacaoRotacao, rotacaoTransform);
                Storyboard.SetTargetProperty(animacaoRotacao, new PropertyPath(RotateTransform.AngleProperty));
                storyboard.Children.Add(animacaoRotacao);
            }

            // Adicionar animações ao storyboard
            storyboard.Children.Add(animacaoY);
            storyboard.Children.Add(animacaoX);

            // Iniciar a animação
            storyboard.Begin();
        }


        private double[] CalcularVetorVelocidade(double velocidadeMotor, double velocidadeCorrenteza, double angulo)
        {
            // Converter o ângulo de graus para radianos
            double anguloRadianos = angulo * (Math.PI / 180);

            // Calcular os componentes do vetor de velocidade com base no ângulo e nas velocidades
            double velocidadeX = velocidadeMotor * Math.Sin(anguloRadianos) + velocidadeCorrenteza;
            double velocidadeY = velocidadeMotor * Math.Cos(anguloRadianos);

            return new double[] { velocidadeX, velocidadeY };
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