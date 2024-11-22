
```
<Window x:Class="PBL_II_WPF.Animacao"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Animação" MinHeight="450" MinWidth="800"
        Height="450" Width="800" >
    <Grid>
        <Grid.Background>
            <ImageBrush ImageSource="/assets/fundo.jpeg" Stretch="UniformToFill"/>
        </Grid.Background>

        <!-- Define ViewBox para escalar todo o conteúdo -->
        <Viewbox Stretch="Uniform" Margin="59,50,21,50" >
            <Grid Width="700" Height="450">
                <Canvas Name="CanvasPrincipal" Background="Transparent">
                    <!-- Rio Container -->
                    <Canvas Name="RioContainer" Width="774" Height="474" Canvas.Left="-62" Canvas.Top="-18">
                        <Canvas.Clip>
                            <RectangleGeometry x:Name="RioClip" Rect="0,0,774,474"/>
                        </Canvas.Clip>
                        <!-- Rio -->
                        <Image Name="ImagemRio" Source="/assets/rio tela cheia.png" 
                               Width="774" Height="474" 
                               HorizontalAlignment="Center" 
                               VerticalAlignment="Center" />
                    </Canvas>

                    <!-- Margem Superior -->
                    <Image Name="MargemSuperior" Source="/assets/margem-de-cima.png"
                           Width="395" Height="308" 
                           Canvas.Left="88" Canvas.Top="-18"
                           HorizontalAlignment="Center" 
                           VerticalAlignment="Top">
                        <Image.RenderTransform>
                            <ScaleTransform ScaleX="1.2" ScaleY="1.2"/>
                        </Image.RenderTransform>
                    </Image>

                    <!-- Margem Inferior -->
                    <Image Name="MargemInferior" Source="/assets/margem-de-baixo.png"
                           Width="151" Height="395" 
                           Canvas.Top="454" Canvas.Left="87" 
                           HorizontalAlignment="Left" 
                           VerticalAlignment="Top">
                        <Image.RenderTransform>
                            <TransformGroup>
                                <ScaleTransform ScaleX="1.2" ScaleY="1.2"/>
                                <RotateTransform Angle="269.874"/>
                                <TranslateTransform X="1" Y="1"/>
                            </TransformGroup>
                        </Image.RenderTransform>
                    </Image>

                    <!-- Barco -->
                    <Image Name="Barco" Source="/assets/barco-de-cima.png" 
                           Width="58" Height="58" 
                           Canvas.Left="119" Canvas.Top="251" 
                           RenderTransformOrigin="0.5,0.5" 
                           HorizontalAlignment="Center" 
                           VerticalAlignment="Top">
                        <Image.RenderTransform>
                            <TransformGroup>
                                <ScaleTransform ScaleX="1.5" ScaleY="1.5"/>
                                <RotateTransform x:Name="RotateBarco" Angle="90"/>
                            </TransformGroup>
                        </Image.RenderTransform>
                    </Image>
                </Canvas>
            </Grid>
        </Viewbox>

        <!-- Botões fora do Viewbox para manterem tamanho consistente -->
        <Button Style="{StaticResource ImageButtonStyle}" Name="btnVoltar" Click="btnVoltar_Click"
                Width="120" Height="80" 
                Background="Transparent" 
                BorderThickness="0"
                HorizontalAlignment="Left" 
                VerticalAlignment="Top" 
                 Margin="50,10,30,10">
            <Image Source="/assets/botao-voltar.png" Stretch="Uniform" Height="63" Width="111"/>
        </Button>

        <Button Style="{StaticResource ImageButtonStyle}" Name="btnIniciar" Click="btnIniciar_Click"
                Width="250" Height="100"
                Background="Transparent" 
                BorderThickness="0"
                HorizontalAlignment="Right" 
                VerticalAlignment="Bottom"
                Margin="15,0,10,25">
            <Image Source="/assets/botao-iniciar-animacao.png" Stretch="Uniform" Height="88" Width="129"/>
        </Button>
    </Grid>
</Window>
```
























```
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Media;

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

        private void PlayClickSound()
        {
            SoundPlayer player = new SoundPlayer("click-sound.wav");
            player.Play();
        }
        public Animacao(string larguraText, string vmaText, string vcText, string anguloText)
        {
            InitializeComponent();

            largura = double.Parse(larguraText, System.Globalization.CultureInfo.InvariantCulture);
            velocidadeMotor = double.Parse(vmaText, System.Globalization.CultureInfo.InvariantCulture);
            velocidadeCorrenteza = double.Parse(vcText, System.Globalization.CultureInfo.InvariantCulture);
            angulo = double.Parse(anguloText, System.Globalization.CultureInfo.InvariantCulture);
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

            // Posição horizontal final deve ser limitada pela largura do rio
            double posicaoFinalX = posicaoInicialX;

            // Limitar ângulo para valores entre 20° e 160°
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

            // Garantir que o barco não ultrapasse a largura do rio
            double larguraMaxima = LARGURA_MAXIMA - BARCO_TAMANHO; // Considerar o tamanho do barco para limitar
            double deslocamentoHorizontal = velocidadeX * tempoTotal;
            posicaoFinalX = Clamp(posicaoFinalX + deslocamentoHorizontal, 0, larguraMaxima);

            // Ajuste adicional para garantir que o barco não ultrapasse as margens
            if (posicaoFinalX + BARCO_TAMANHO > larguraMaxima)
            {
                posicaoFinalX = larguraMaxima - BARCO_TAMANHO;  // Coloca o barco na borda direita
            }
            if (posicaoFinalX < 0)
            {
                posicaoFinalX = 0;  // Coloca o barco na borda esquerda
            }

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
                To = posicaoFinalX,
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

            // Tratar casos especiais para ângulos 90° e 270° para evitar valores muito pequenos
            if (angulo == 90 || angulo == 270)
            {
                return new double[] { 0, velocidadeMotor * Math.Sign(Math.Cos(anguloRadianos)) };
            }

            // Calcular os componentes do vetor de velocidade com base no ângulo e nas velocidades
            double velocidadeX = velocidadeMotor * Math.Sin(anguloRadianos) + velocidadeCorrenteza;
            double velocidadeY = velocidadeMotor * Math.Cos(anguloRadianos);

            return new double[] { velocidadeX, velocidadeY };
        }
        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            IniciarAnimacao();
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            // Criar uma nova instância da tela Principal
            Principal principal = new Principal();
            principal.Show(); // Mostrar a janela Principal

            // Fechar a janela atual (Animacao)
            this.Close();
        }

        public class MultiplierConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is double doubleValue && parameter is string multiplierString)
                {
                    if (double.TryParse(multiplierString, out double multiplier))
                    {
                        return doubleValue * multiplier;
                    }
                }
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
```
