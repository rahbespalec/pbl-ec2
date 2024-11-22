using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;

namespace PBL_II_WPF
{
    public partial class Historico : Window
    {
        private List<SimulacaoHistorico> todasSimulacoes;

        public Historico()
        {
            InitializeComponent();
            CarregarDados();
        }
        private void PlayClickSound()
        {
            SoundPlayer player = new SoundPlayer("click-sound.wav");
            player.Play();
        }
        private void CarregarDados()
        {
            todasSimulacoes = new List<SimulacaoHistorico>();
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, "Composição de Movimentos.xlsx");

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Nenhum histórico encontrado.");
                return;
            }

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension?.End.Row ?? 1;

                for (int row = 2; row <= rowCount; row++) // Começando da linha 2 para pular o cabeçalho
                {
                    var simulacao = new SimulacaoHistorico
                    {
                        Numero = row - 1,
                        DataHora = DateTime.Parse(worksheet.Cells[row, 1].Text),
                        Largura = double.Parse(worksheet.Cells[row, 2].Text),
                        VelocidadeMotor = double.Parse(worksheet.Cells[row, 3].Text),
                        VelocidadeCorrenteza = double.Parse(worksheet.Cells[row, 4].Text),
                        Angulo = double.Parse(worksheet.Cells[row, 5].Text)
                    };
                    simulacao.Titulo = $"Simulação {simulacao.Numero}";
                    todasSimulacoes.Add(simulacao);
                }
            }

            // Define a ordenação inicial como data mais recente
            SortComboBox.SelectedIndex = 0;
            AtualizarLista(todasSimulacoes.OrderByDescending(s => s.DataHora).ToList());
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarEOrdenarDados();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FiltrarEOrdenarDados();
        }

        private void FiltrarEOrdenarDados()
        {
            var dadosFiltrados = todasSimulacoes;

            // Aplicar filtro de busca
            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                string busca = SearchBox.Text.ToLower();
                dadosFiltrados = dadosFiltrados.Where(s =>
                    s.Titulo.ToLower().Contains(busca) ||
                    s.DataHora.ToString().Contains(busca) ||
                    s.Largura.ToString().Contains(busca) ||
                    s.VelocidadeMotor.ToString().Contains(busca) ||
                    s.VelocidadeCorrenteza.ToString().Contains(busca) ||
                    s.Angulo.ToString().Contains(busca)
                ).ToList();
            }

            // Aplicar ordenação
            switch (SortComboBox.SelectedIndex)
            {
                case 0: // Data (Mais Recente)
                    dadosFiltrados = dadosFiltrados.OrderByDescending(s => s.DataHora).ToList();
                    break;
                case 1: // Data (Mais Antiga)
                    dadosFiltrados = dadosFiltrados.OrderBy(s => s.DataHora).ToList();
                    break;
                case 2: // Largura (Crescente)
                    dadosFiltrados = dadosFiltrados.OrderBy(s => s.Largura).ToList();
                    break;
                case 3: // Largura (Decrescente)
                    dadosFiltrados = dadosFiltrados.OrderByDescending(s => s.Largura).ToList();
                    break;
            }

            AtualizarLista(dadosFiltrados);
        }

        private void AtualizarLista(List<SimulacaoHistorico> simulacoes)
        {
            SimulacoesItemsControl.ItemsSource = simulacoes;
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
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

    public class SimulacaoHistorico
    {
        public int Numero { get; set; }
        public string Titulo { get; set; }
        public DateTime DataHora { get; set; }
        public double Largura { get; set; }
        public double VelocidadeMotor { get; set; }
        public double VelocidadeCorrenteza { get; set; }
        public double Angulo { get; set; }
    }
}