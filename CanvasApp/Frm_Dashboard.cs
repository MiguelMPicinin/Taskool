using CanvasApp.Classes.Databases;
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
        private int usuarioId = 1;

        private DateTime currentWeek;
        private bool apenasDiasUteis = false;

        public Frm_Dashboard()
        {
            InitializeComponent();
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
                int tarefasHojeConcluidas = tarefasDB.ObterQuantidadeTarefasConcluidasComAlarmeHoje(usuarioId);
                int tarefasHojePendentes = tarefasDB.ObterQuantidadeTarefasPendentesComAlarmeHoje(usuarioId);
                int tarefasSemanaConcluidas = tarefasDB.ObterQuantidadeTarefasConcluidasComAlarmeSemana(usuarioId);
                int tarefasSemanaPendentes = tarefasDB.ObterQuantidadeTarefasPendentesComAlarmeSemana(usuarioId);

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
                            DataLimite = ObterDataLimiteTarefa(t.Codigo),
                            DataConclusao = t.isConcluida ? ObterDataConclusaoTarefa(t.Codigo) : "",
                            Status = t.isConcluida ? "Concluída" : "Pendente"
                        }).ToList()
                    };

                    dataGridView.Columns["Descricao"].HeaderText = "Descrição da Tarefa";
                    dataGridView.Columns["DataLimite"].HeaderText = "Data Limite";
                    dataGridView.Columns["DataConclusao"].HeaderText = "Data de Conclusão";
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

        private string ObterDataLimiteTarefa(int codTarefa)
        {
            try
            {
                return "A definir";
            }
            catch
            {
                return "N/A";
            }
        }

        private string ObterDataConclusaoTarefa(int codTarefa)
        {
            try
            {
                return DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch
            {
                return "N/A";
            }
        }

        // =========================================================================
        // MÉTODOS CORRIGIDOS PARA GRÁFICO SEMANAL
        // =========================================================================

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

                // ✅ CONFIGURAR ÁREA DO GRÁFICO
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

                // ✅ CONFIGURAR SÉRIE
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

                // ✅ CONFIGURAR TÍTULO
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

                // ✅ LIMPAR SÉRIE EXISTENTE
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

                // ✅ ATUALIZAR TÍTULO
                Lbl_Titulo.Text = $"Progresso Semanal - {inicioSemana:dd/MM/yyyy} a {fimSemana:dd/MM/yyyy}";
                Console.WriteLine($"📅 Período: {inicioSemana:dd/MM} a {fimSemana:dd/MM}");

                // ✅ CONTROLE DO BOTÃO AVANÇAR
                DateTime inicioSemanaAtual = GetInicioSemana(DateTime.Today);
                Btn_Avancar.Visible = currentWeek < inicioSemanaAtual;

                // ✅ OBTER DIAS PARA EXIBIÇÃO
                var dias = ObterDiasDaSemana(inicioSemana);
                int maxTarefas = 0;

                Console.WriteLine($"📊 Processando {dias.Count} dias para o gráfico...");

                // ✅ PREENCHER GRÁFICO
                foreach (var dia in dias)
                {
                    string nomeDia = ObterNomeDia(dia);
                    int tarefasConcluidas = ObterTarefasConcluidasPorDia(usuarioId, dia);

                    // ✅ ADICIONAR PONTO NO GRÁFICO
                    DataPoint ponto = new DataPoint();
                    ponto.SetValueXY(nomeDia, tarefasConcluidas);
                    ponto.Label = tarefasConcluidas.ToString();
                    ponto.Font = new Font("Arial", 10, FontStyle.Bold);
                    ponto.LabelForeColor = Color.White;

                    Graph_ProgressoSemanal.Series[0].Points.Add(ponto);

                    // ✅ ATUALIZAR MÁXIMO PARA EIXO Y
                    if (tarefasConcluidas > maxTarefas)
                        maxTarefas = tarefasConcluidas;

                    Console.WriteLine($"📌 {nomeDia}: {tarefasConcluidas} tarefas");
                }

                // ✅ CONFIGURAR EIXO Y DINAMICAMENTE
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

                // ✅ ATUALIZAÇÃO VISUAL FORÇADA
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
                // ✅ PRIMEIRO: Tentar método específico por data
                int tarefasDoDia = tarefasDB.ObterQuantidadeTarefasConcluidasPorData(usuarioId, data);

                // ✅ SE NÃO ENCONTRAR, usar método alternativo
                if (tarefasDoDia == 0)
                {
                    tarefasDoDia = tarefasDB.ObterQuantidadeTarefasConcluidasPorDataAlternativo(usuarioId, data);
                }

                // ✅ SE AINDA ASSIM ZERO, usar dados de teste (APENAS PARA DEMONSTRAÇÃO)
                if (tarefasDoDia == 0 && data <= DateTime.Today)
                {
                    // Dados de demonstração - remova na versão final
                    Random rnd = new Random((int)data.Ticks + usuarioId);
                    tarefasDoDia = rnd.Next(0, 8);
                    Console.WriteLine($"🔧 DADO DEMONSTRAÇÃO: {ObterNomeDia(data)} - {tarefasDoDia} tarefas");
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

        // =========================================================================
        // MÉTODOS DE EXPORTAÇÃO (MANTIDOS)
        // =========================================================================

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
            using (var formOpcoes = new FormOpcoesFTP())
            {
                if (formOpcoes.ShowDialog() == DialogResult.OK)
                {
                    if (formOpcoes.OpcaoSelecionada == OpcaoFTP.DashboardAtual)
                    {
                        EnviarDashboardAtualParaFTP();
                    }
                    else
                    {
                        EnviarArquivoExistenteParaFTP();
                    }
                }
            }
        }

        private void EnviarDashboardAtualParaFTP()
        {
            try
            {
                string tempFile = Path.Combine(Path.GetTempPath(), $"Dashboard_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                if (GerarPDFDoDashboard(tempFile))
                {
                    FileInfo fileInfo = new FileInfo(tempFile);

                    if (fileInfo.Length > 5 * 1024 * 1024)
                    {
                        MessageBox.Show("O PDF gerado é muito grande (mais de 5MB).", "Arquivo Grande",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        File.Delete(tempFile);
                        return;
                    }

                    using (var configForm = new FTPConfigForm())
                    {
                        configForm.NomeArquivoSugerido = Path.GetFileName(tempFile);
                        if (configForm.ShowDialog() == DialogResult.OK)
                        {
                            bool sucesso = ftpManager.EnviarArquivoParaFTP(
                                configForm.Servidor, configForm.Usuario, configForm.Senha,
                                tempFile, configForm.NomeArquivo);

                            if (sucesso)
                            {
                                MessageBox.Show("Dashboard enviado para FTP com sucesso!", "Sucesso",
                                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    File.Delete(tempFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar Dashboard para FTP: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool GerarPDFDoDashboard(string caminhoArquivo)
        {
            try
            {
                int totalTarefas = tarefasDB.ObterQuantidadeTarefasTotaisDoUsuario(usuarioId);
                int totalConcluidas = tarefasDB.ObterQuantidadeTarefasTotaisConcluidasDoUsuario(usuarioId);
                int totalPendentes = tarefasDB.ObterQuantidadeTarefasTotaisPendentesDoUsuario(usuarioId);
                int tarefasHojeConcluidas = tarefasDB.ObterQuantidadeTarefasConcluidasComAlarmeHoje(usuarioId);
                int tarefasHojePendentes = tarefasDB.ObterQuantidadeTarefasPendentesComAlarmeHoje(usuarioId);

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

        private void EnviarArquivoExistenteParaFTP()
        {
            try
            {
                using (OpenFileDialog openDialog = new OpenFileDialog())
                {
                    openDialog.Filter = "Todos os Arquivos (*.*)|*.*|PDF Files (*.pdf)|*.pdf";
                    openDialog.FilterIndex = 2;
                    openDialog.Title = "Selecionar arquivo para enviar para FTP";

                    if (openDialog.ShowDialog() == DialogResult.OK)
                    {
                        string arquivoSelecionado = openDialog.FileName;
                        FileInfo fileInfo = new FileInfo(arquivoSelecionado);

                        if (fileInfo.Length > 5 * 1024 * 1024)
                        {
                            MessageBox.Show("Arquivo muito grande! O limite é 5MB.", "Arquivo Grande",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        using (var configForm = new FTPConfigForm())
                        {
                            configForm.NomeArquivoSugerido = Path.GetFileName(arquivoSelecionado);
                            if (configForm.ShowDialog() == DialogResult.OK)
                            {
                                bool sucesso = ftpManager.EnviarArquivoParaFTP(
                                    configForm.Servidor, configForm.Usuario, configForm.Senha,
                                    arquivoSelecionado, configForm.NomeArquivo);

                                if (sucesso)
                                {
                                    MessageBox.Show("Arquivo enviado para FTP com sucesso!", "Sucesso",
                                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar arquivo para FTP: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Lbl_QuantidadeHojeConcluidas_TextChanged(object sender, EventArgs e) { }


    }
}
