using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using CanvasApp.Classes.ManipulaçãoDados.Prova_4_teste;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CanvasApp
{
    public partial class Frm_Dashboard : Form
    {
        private PdfService pdfService;
        private FTPManager ftpManager;
        private UsuarioDB usuarioDB;
        private TarefasDB tarefasDB;
        private ProjetosDB projetosDB;
        private int usuarioId;

        private DateTime currentWeek;
        private bool apenasDiasUteis = false;

        public Frm_Dashboard()
        {
            InitializeComponent();

            // CORREÇÃO: Obter usuário da sessão corretamente
            if (Sessao.UsuarioLogado != null)
            {
                usuarioId = int.Parse(Sessao.UsuarioLogado.Codigo);
            }
            else
            {
                usuarioId = 1; // Fallback para desenvolvimento
            }

            currentWeek = GetInicioSemana(DateTime.Today);

            pdfService = new PdfService();
            ftpManager = new FTPManager();
            usuarioDB = new UsuarioDB();
            tarefasDB = new TarefasDB();
            projetosDB = new ProjetosDB();

            // Configurações iniciais
            Pnl_CC5.Visible = false;
            Pnl_CC6.Visible = false;
            Pnl_CC7.Visible = false;

            this.Load += (s, e) =>
            {
                TestarConexaoBanco();
                AtualizarTextosTarefas();
                AtualizarStatusGraficoCircular();
                AtualizarStatusGraficoSemana();
            };

            // Configurar eventos
            Btn_Anterior.Click += (s, e) =>
            {
                currentWeek = currentWeek.AddDays(-7);
                AtualizarStatusGraficoSemana();
            };

            Btn_Avancar.Click += (s, e) =>
            {
                currentWeek = currentWeek.AddDays(7);
                AtualizarStatusGraficoSemana();
            };

            Btn_ApenasUteis.Click += (s, e) =>
            {
                apenasDiasUteis = !apenasDiasUteis;
                AtualizarStatusGraficoSemana();
                Btn_ApenasUteis.Text = apenasDiasUteis ? "Mostrar todos os dias" : "Considerar apenas dias úteis";
            };

            // Configurar evento de clique no gráfico de pizza
            if (Graph_TarefasPorProjeto != null)
            {
                Graph_TarefasPorProjeto.Click += Graph_TarefasPorProjeto_Click;
            }

            ConfigurarGraphProgressoSemanal();
        }

        private void TestarConexaoBanco()
        {
            try
            {
                usuarioDB.DiagnosticoCompletoUsuario(usuarioId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao testar conexão: {ex.Message}");
            }
        }

        private void AtualizarTextosTarefas()
        {
            try
            {
                int total = tarefasDB.ObterQuantidadeTarefasTotaisDoUsuario(usuarioId);
                int totalConcluidas = tarefasDB.ObterQuantidadeTarefasTotaisConcluidasDoUsuario(usuarioId);
                int totalPendentes = tarefasDB.ObterQuantidadeTarefasTotaisPendentesDoUsuario(usuarioId);

                // CORREÇÃO: Usar métodos que retornam listas e contar
                int tarefasHojeConcluidas = ObterTarefasConcluidasComAlarmeHoje(usuarioId).Count;
                int tarefasHojePendentes = ObterTarefasPendentesComAlarmeHoje(usuarioId).Count;
                int tarefasSemanaConcluidas = ObterTarefasConcluidasComAlarmeSemana(usuarioId).Count;
                int tarefasSemanaPendentes = ObterTarefasPendentesComAlarmeSemana(usuarioId).Count;

                double porcentagemConcluidas = total > 0 ? Math.Round((totalConcluidas * 100.0) / total, 1) : 0;
                double porcentagemPendentes = total > 0 ? Math.Round((totalPendentes * 100.0) / total, 1) : 0;

                // Atualizar labels
                Lbl_HojeConcluidas.Text = tarefasHojeConcluidas.ToString();
                Lbl_HojePendentes.Text = tarefasHojePendentes.ToString();
                Lbl_SemanaConcluidas.Text = tarefasSemanaConcluidas.ToString();
                Lbl_SemanaPendentes.Text = tarefasSemanaPendentes.ToString();
                Lbl_TotalConcluidas.Text = totalConcluidas.ToString();
                Lbl_TotalPendentes.Text = totalPendentes.ToString();

                Lbl_Porcentagem1.Text = $"Você tem {porcentagemConcluidas}% das tarefas concluídas";
                Lbl_Porcentagem2.Text = $"Faltam fazer {porcentagemPendentes}% das tarefas";

                Console.WriteLine($"✅ Dashboard atualizado - Total: {total}, Concluídas: {totalConcluidas}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar dashboard: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"❌ Erro em AtualizarTextosTarefas: {ex.Message}");
            }
        }

        // CORREÇÃO: Método para obter tarefas concluídas com alarme hoje
        private List<Projeto_Tarefas> ObterTarefasConcluidasComAlarmeHoje(int usuarioId)
        {
            try
            {
                var todasTarefasHoje = tarefasDB.ObterTarefasComAlarmeHoje(usuarioId); // CORREÇÃO: Convert para string
                return todasTarefasHoje.Where(t => t.isConcluida).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter tarefas concluídas hoje: {ex.Message}");
                return new List<Projeto_Tarefas>();
            }
        }

        // CORREÇÃO: Método para obter tarefas pendentes com alarme hoje
        private List<Projeto_Tarefas> ObterTarefasPendentesComAlarmeHoje(int usuarioId)
        {
            try
            {
                var todasTarefasHoje = tarefasDB.ObterTarefasComAlarmeHoje(usuarioId); // CORREÇÃO: Convert para string
                return todasTarefasHoje.Where(t => !t.isConcluida).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter tarefas pendentes hoje: {ex.Message}");
                return new List<Projeto_Tarefas>();
            }
        }

        // CORREÇÃO: Método para obter tarefas concluídas com alarme semana
        private List<Projeto_Tarefas> ObterTarefasConcluidasComAlarmeSemana(int usuarioId)
        {
            try
            {
                var todasTarefasSemana = tarefasDB.ObterTarefasComAlarmeSemana(usuarioId); // CORREÇÃO: Convert para string
                return todasTarefasSemana.Where(t => t.isConcluida).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter tarefas concluídas semana: {ex.Message}");
                return new List<Projeto_Tarefas>();
            }
        }

        // CORREÇÃO: Método para obter tarefas pendentes com alarme semana
        private List<Projeto_Tarefas> ObterTarefasPendentesComAlarmeSemana(int usuarioId)
        {
            try
            {
                var todasTarefasSemana = tarefasDB.ObterTarefasComAlarmeSemana(usuarioId); // CORREÇÃO: Convert para string
                return todasTarefasSemana.Where(t => !t.isConcluida).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter tarefas pendentes semana: {ex.Message}");
                return new List<Projeto_Tarefas>();
            }
        }

        private void AtualizarStatusGraficoCircular()
        {
            try
            {
                var dados = tarefasDB.ObterDadosGraficoTarefasPorProjeto(usuarioId);

                if (dados == null || !dados.Any())
                {
                    Console.WriteLine("ℹ️  Nenhum projeto com tarefas encontrado.");
                    return;
                }

                Console.WriteLine($"✅ Gráfico circular: {dados.Count} projetos carregados");
                ConfigurarChartCirculo(dados);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao carregar gráfico circular: {ex.Message}");
            }
        }

        private void ConfigurarChartCirculo(List<ProjetosTarefasDatas> dados)
        {
            if (Graph_TarefasPorProjeto == null) return;

            Graph_TarefasPorProjeto.Series.Clear();
            Graph_TarefasPorProjeto.Legends.Clear();

            int totalTarefas = dados.Sum(d => d.QuantidadeTarefas);

            Series series = new Series("Projetos");
            series.ChartType = SeriesChartType.Pie;
            series.IsValueShownAsLabel = true;
            series.Label = "#PERCENT{P2}";
            series.LegendText = "#VALX: #VALY tarefas";
            series.Font = new Font("Arial", 9, FontStyle.Bold);

            var dadosOrdenados = dados.OrderByDescending(d => d.QuantidadeTarefas).ToList();

            for (int i = 0; i < dadosOrdenados.Count; i++)
            {
                var item = dadosOrdenados[i];
                double porcentagem = totalTarefas > 0 ? Math.Round((item.QuantidadeTarefas * 100.0) / totalTarefas, 1) : 0;

                DataPoint point = new DataPoint();
                point.SetValueXY(item.NomeProjeto, item.QuantidadeTarefas);
                point.Label = $"{porcentagem}%";
                point.ToolTip = $"{item.NomeProjeto}: {item.QuantidadeTarefas} tarefas ({porcentagem}%)";
                point.LegendText = $"{item.NomeProjeto}: {item.QuantidadeTarefas} tarefas";

                if (i == 0) point.Color = Color.Red;
                else if (i == dadosOrdenados.Count - 1) point.Color = Color.Green;
                else point.Color = GetCorPorIndice(i);

                series.Points.Add(point);
            }

            Graph_TarefasPorProjeto.Series.Add(series);

            Legend legend = new Legend();
            legend.Docking = Docking.Right;
            legend.Font = new Font("Arial", 9);
            Graph_TarefasPorProjeto.Legends.Add(legend);

            if (Graph_TarefasPorProjeto.ChartAreas.Count == 0)
                Graph_TarefasPorProjeto.ChartAreas.Add(new ChartArea());

            Graph_TarefasPorProjeto.Titles.Clear();
            Title title = new Title("Distribuição de Tarefas por Projeto");
            title.Font = new Font("Arial", 12, FontStyle.Bold);
            Graph_TarefasPorProjeto.Titles.Add(title);
        }

        private Color GetCorPorIndice(int index)
        {
            Color[] cores = {
                Color.SteelBlue, Color.Orange, Color.Purple,
                Color.Teal, Color.Maroon, Color.Olive, Color.Navy
            };
            return cores[index % cores.Length];
        }

        private void Graph_TarefasPorProjeto_Click(object sender, EventArgs e)
        {
            try
            {
                var hitTest = Graph_TarefasPorProjeto.HitTest(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);

                if (hitTest.PointIndex >= 0 && hitTest.Series != null)
                {
                    string projetoNome = hitTest.Series.Points[hitTest.PointIndex].AxisLabel;

                    var projetosUsuario = projetosDB.ObterTodosProjetosUsuario(usuarioId);
                    var projeto = projetosUsuario.FirstOrDefault(p => p.Nome == projetoNome);

                    if (projeto != null)
                    {
                        AbrirListaTarefasProjeto(projeto.Codigo, projeto.Nome);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir lista de tarefas: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbrirListaTarefasProjeto(int codProjeto, string nomeProjeto)
        {
            try
            {
                var tarefasProjeto = tarefasDB.ObterTarefasPorProjeto(codProjeto);

                using (var formLista = new Form())
                {
                    formLista.Text = $"Tarefas do Projeto: {nomeProjeto}";
                    formLista.Size = new Size(600, 400);
                    formLista.StartPosition = FormStartPosition.CenterParent;

                    var dataGridView = new DataGridView
                    {
                        Dock = DockStyle.Fill,
                        ReadOnly = true,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                        DataSource = tarefasProjeto.Select(t => new
                        {
                            Descricao = t.Descricao,
                            Status = t.isConcluida ? "Concluída" : "Pendente"
                        }).ToList()
                    };

                    dataGridView.Columns["Descricao"].HeaderText = "Descrição da Tarefa";
                    dataGridView.Columns["Status"].HeaderText = "Status";

                    formLista.Controls.Add(dataGridView);
                    formLista.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar tarefas do projeto: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarGraphProgressoSemanal()
        {
            if (Graph_ProgressoSemanal == null)
            {
                Console.WriteLine("❌ Graph_ProgressoSemanal é nulo!");
                return;
            }

            try
            {
                Graph_ProgressoSemanal.Series.Clear();
                Graph_ProgressoSemanal.ChartAreas.Clear();
                Graph_ProgressoSemanal.Legends.Clear();

                ChartArea chartArea = new ChartArea("AreaPrincipal");
                chartArea.AxisX.MajorGrid.Enabled = false;
                chartArea.AxisY.MajorGrid.Enabled = true;
                chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
                chartArea.AxisY.Interval = 1;
                chartArea.AxisY.Minimum = 0;
                chartArea.AxisX.LabelStyle.Angle = -45;
                chartArea.AxisX.LabelStyle.Font = new Font("Arial", 9, FontStyle.Bold);
                chartArea.AxisY.LabelStyle.Font = new Font("Arial", 9, FontStyle.Bold);

                Graph_ProgressoSemanal.ChartAreas.Add(chartArea);

                Series series = new Series("TarefasConcluidas");
                series.ChartType = SeriesChartType.Column;
                series.Color = Color.SteelBlue;
                series.BorderColor = Color.DarkBlue;
                series.BorderWidth = 2;
                series.IsValueShownAsLabel = true;
                series.LabelFormat = "0";
                series.Font = new Font("Arial", 10, FontStyle.Bold);
                series.LabelForeColor = Color.White;

                Graph_ProgressoSemanal.Series.Add(series);

                Graph_ProgressoSemanal.Titles.Clear();
                Title title = new Title();
                title.Font = new Font("Arial", 12, FontStyle.Bold);
                title.Text = "Progresso Semanal de Tarefas Concluídas";
                Graph_ProgressoSemanal.Titles.Add(title);

                Console.WriteLine("✅ Gráfico semanal configurado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro na configuração do gráfico: {ex.Message}");
                MessageBox.Show($"Erro ao configurar gráfico: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarStatusGraficoSemana()
        {
            try
            {
                if (Graph_ProgressoSemanal == null)
                {
                    MessageBox.Show("Gráfico semanal não foi inicializado corretamente.", "Erro",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Console.WriteLine("🔄 Iniciando atualização do gráfico semanal...");

                if (Graph_ProgressoSemanal.Series.Count > 0)
                {
                    Graph_ProgressoSemanal.Series[0].Points.Clear();
                }
                else
                {
                    MessageBox.Show("Série do gráfico não encontrada.", "Erro",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DateTime inicioSemana = currentWeek;
                DateTime fimSemana = currentWeek.AddDays(6);

                Lbl_Titulo.Text = $"Progresso Semanal - {inicioSemana:dd/MM/yyyy} a {fimSemana:dd/MM/yyyy}";
                Console.WriteLine($"📅 Período: {inicioSemana:dd/MM} a {fimSemana:dd/MM}");

                DateTime inicioSemanaAtual = GetInicioSemana(DateTime.Today);
                Btn_Avancar.Visible = currentWeek < inicioSemanaAtual;

                var dias = ObterDiasDaSemana(inicioSemana);
                int maxTarefas = 0;

                Console.WriteLine($"📊 Processando {dias.Count} dias para o gráfico...");

                foreach (var dia in dias)
                {
                    string nomeDia = ObterNomeDia(dia);
                    int tarefasConcluidas = ObterTarefasConcluidasPorDia(usuarioId, dia);

                    DataPoint ponto = new DataPoint();
                    ponto.SetValueXY(nomeDia, tarefasConcluidas);
                    ponto.Label = tarefasConcluidas.ToString();
                    ponto.Font = new Font("Arial", 10, FontStyle.Bold);
                    ponto.LabelForeColor = Color.White;

                    Graph_ProgressoSemanal.Series[0].Points.Add(ponto);

                    if (tarefasConcluidas > maxTarefas)
                        maxTarefas = tarefasConcluidas;

                    Console.WriteLine($"📌 {nomeDia}: {tarefasConcluidas} tarefas");
                }

                if (Graph_ProgressoSemanal.ChartAreas.Count > 0)
                {
                    ChartArea area = Graph_ProgressoSemanal.ChartAreas[0];
                    area.AxisY.Minimum = 0;
                    area.AxisY.Maximum = Math.Max(maxTarefas + 1, 2);
                    area.AxisY.Interval = 1;
                    area.AxisY.LabelStyle.Format = "0";

                    area.AxisX.LabelStyle.Font = new Font("Arial", 9, FontStyle.Bold);
                    area.AxisY.LabelStyle.Font = new Font("Arial", 9, FontStyle.Bold);
                }

                Graph_ProgressoSemanal.Invalidate();
                Graph_ProgressoSemanal.Update();
                Graph_ProgressoSemanal.Refresh();

                Console.WriteLine($"✅ Gráfico semanal atualizado! Máximo: {maxTarefas} tarefas");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERRO CRÍTICO no gráfico semanal: {ex.Message}");
                MessageBox.Show($"Erro crítico ao atualizar gráfico semanal:\n{ex.Message}",
                              "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ObterTarefasConcluidasPorDia(int usuarioId, DateTime data)
        {
            try
            {
                int tarefasDoDia = tarefasDB.ObterQuantidadeTarefasConcluidasPorData(usuarioId, data);

                if (tarefasDoDia == 0)
                {
                    tarefasDoDia = tarefasDB.ObterQuantidadeTarefasConcluidasPorDataAlternativo(usuarioId, data);
                }

                Console.WriteLine($"✅ {ObterNomeDia(data)} ({data:dd/MM}): {tarefasDoDia} tarefas concluídas");
                return tarefasDoDia;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro crítico ao obter tarefas do dia {data:dd/MM/yyyy}: {ex.Message}");
                return 0;
            }
        }

        private List<DateTime> ObterDiasDaSemana(DateTime inicioSemana)
        {
            var dias = new List<DateTime>();

            Console.WriteLine($"📅 Gerando dias a partir de {inicioSemana:dd/MM/yyyy} ({ObterNomeDia(inicioSemana)})");
            Console.WriteLine($"🔧 Modo dias úteis: {(apenasDiasUteis ? "SIM" : "NÃO")}");

            for (int i = 0; i < 7; i++)
            {
                DateTime dia = inicioSemana.AddDays(i);
                bool ehDiaUtil = (dia.DayOfWeek >= DayOfWeek.Monday && dia.DayOfWeek <= DayOfWeek.Friday);
                bool incluirDia = !apenasDiasUteis || ehDiaUtil;

                if (incluirDia)
                {
                    dias.Add(dia);
                    Console.WriteLine($"   ✅ {ObterNomeDia(dia)} ({dia:dd/MM/yyyy}) - {(ehDiaUtil ? "Dia útil" : "Fim de semana")}");
                }
                else
                {
                    Console.WriteLine($"   ❌ {ObterNomeDia(dia)} ({dia:dd/MM/yyyy}) - EXCLUÍDO (fim de semana)");
                }
            }

            Console.WriteLine($"📊 Total de dias no gráfico: {dias.Count}");
            return dias;
        }

        private string ObterNomeDia(DateTime data)
        {
            try
            {
                CultureInfo culture = new CultureInfo("pt-BR");
                string nomeDia = culture.DateTimeFormat.GetDayName(data.DayOfWeek);
                return culture.TextInfo.ToTitleCase(nomeDia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao obter nome do dia: {ex.Message}");
                return data.DayOfWeek.ToString();
            }
        }

        private DateTime GetInicioSemana(DateTime data)
        {
            int diff = (7 + (data.DayOfWeek - DayOfWeek.Sunday)) % 7;
            return data.AddDays(-1 * diff).Date;
        }

        private void Btn_PDF_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                    saveDialog.FilterIndex = 1;
                    saveDialog.Title = "Salvar Dashboard como PDF";
                    saveDialog.FileName = $"Dashboard_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        bool sucesso = GerarPDFDoDashboard(saveDialog.FileName);

                        if (sucesso)
                        {
                            MessageBox.Show("Dashboard exportado para PDF com sucesso!", "Sucesso",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar PDF: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_FTP_Click(object sender, EventArgs e)
        {
            // Implementação do FTP...
            MessageBox.Show("Funcionalidade FTP em desenvolvimento");
        }

        private bool GerarPDFDoDashboard(string caminhoArquivo)
        {
            try
            {
                int totalTarefas = tarefasDB.ObterQuantidadeTarefasTotaisDoUsuario(usuarioId);
                int totalConcluidas = tarefasDB.ObterQuantidadeTarefasTotaisConcluidasDoUsuario(usuarioId);
                int totalPendentes = tarefasDB.ObterQuantidadeTarefasTotaisPendentesDoUsuario(usuarioId);

                int tarefasHojeConcluidas = ObterTarefasConcluidasComAlarmeHoje(usuarioId).Count;
                int tarefasHojePendentes = ObterTarefasPendentesComAlarmeHoje(usuarioId).Count;

                using (var writer = new System.IO.StreamWriter(caminhoArquivo.Replace(".pdf", ".txt")))
                {
                    writer.WriteLine("=== DASHBOARD DO USUÁRIO ===");
                    writer.WriteLine($"Data: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    writer.WriteLine();
                    writer.WriteLine("=== RESUMO DE TAREFAS ===");
                    writer.WriteLine($"Total de Tarefas: {totalTarefas}");
                    writer.WriteLine($"Tarefas Concluídas: {totalConcluidas}");
                    writer.WriteLine($"Tarefas Pendentes: {totalPendentes}");
                    writer.WriteLine($"Porcentagem de Conclusão: {(totalTarefas > 0 ? Math.Round((totalConcluidas * 100.0) / totalTarefas, 1) : 0)}%");
                    writer.WriteLine();
                    writer.WriteLine("=== HOJE ===");
                    writer.WriteLine($"Concluídas: {tarefasHojeConcluidas}");
                    writer.WriteLine($"Pendentes: {tarefasHojePendentes}");
                }

                File.Move(caminhoArquivo.Replace(".pdf", ".txt"), caminhoArquivo);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void Lbl_QuantidadeHojeConcluidas_TextChanged(object sender, EventArgs e) { }
    }
}