using System;
using System.Windows;
using System.Windows.Media;
using System.IO;

namespace PBL_II_WPF
{
    public partial class App : Application
    {
        // Instância global do MediaPlayer para reprodução da música ambiente
        private static MediaPlayer playerMusicaAmbiente;

        // Variável para controlar se a música está no estado "mute"
        private static bool _isMuted = false;

        /// <summary>
        /// Método para iniciar a música ambiente.
        /// Este método configura o MediaPlayer, define o comportamento para quando a música terminar 
        /// (reiniciar do início) e começa a reprodução.
        /// </summary>
        public static void IniciarMusicaAmbiente()
        {
            // Verifica se o player já foi inicializado
            if (playerMusicaAmbiente == null)
            {
                // Cria a instância do MediaPlayer
                playerMusicaAmbiente = new MediaPlayer();

                // Verifica se o arquivo existe e, se sim, carrega-o
                var caminhoArquivo = "Som-Ambiente.wav"; // Altere o caminho conforme necessário

                if (File.Exists(caminhoArquivo))
                {
                    playerMusicaAmbiente.Open(new Uri(caminhoArquivo, UriKind.Relative));

                    // Evento disparado quando a música termina
                    playerMusicaAmbiente.MediaEnded += (s, e) =>
                    {
                        // Reinicia a posição para o início
                        playerMusicaAmbiente.Position = TimeSpan.Zero;

                        // Inicia a reprodução novamente
                        playerMusicaAmbiente.Play();
                    };

                    // Começa a reprodução da música
                    playerMusicaAmbiente.Play();
                }
                else
                {
                    // Se o arquivo não for encontrado, exibe um erro
                    MessageBox.Show("Arquivo de música não encontrado!");
                }
            }
        }

        /// <summary>
        /// Método para alternar entre mute e unmute da música.
        /// Pausa a música se estiver tocando e retoma a reprodução se estiver pausada.
        /// </summary>
  

        /// <summary>
        /// Propriedade somente leitura para verificar se a música está mutada.
        /// </summary>
        public static bool IsMuted => _isMuted; // Retorna o estado atual do mute

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            App.IniciarMusicaAmbiente(); // Inicia a música ambiente
        }

        /// <summary>
        /// Evento chamado quando o aplicativo está prestes a fechar. 
        /// Paramos a música aqui.
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            if (playerMusicaAmbiente != null)
            {
                // Para a música ambiente quando o aplicativo for fechado
                playerMusicaAmbiente.Stop();
            }

            base.OnExit(e); // Chama o método base para completar o fechamento
        }
    }
}
