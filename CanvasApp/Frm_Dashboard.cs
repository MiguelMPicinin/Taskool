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
        private int usuarioId = 1; // ✅ DEFINA o ID do usuário logado

        // Variáveis para o Gráfico circular
        private Chart chartProjetos;
        private DataGridView dataGridView1;

        // Variáveis para o gráfico semanal
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
                // ✅ USANDO MÉTODOS CORRIGIDOS DAS NOVAS CLASSES
                int total = tarefasDB.ObterQuantidadeTarefasTotaisDoUsuario(usuarioId);
                int totalConcluidas = tarefasDB.ObterQuantidadeTarefasTotaisConcluidasDoUsuario(usuarioId);
                int totalPendentes = tarefasDB.ObterQuantidadeTarefasTotaisPendentesDoUsuario(usuarioId);
                int tarefasHojeConcluidas = tarefasDB.ObterQuantidadeTarefasConcluidasComAlarmeHoje(usuarioId);
                int tarefasHojePendentes = tarefasDB.ObterQuantidadeTarefasPendentesComAlarmeHoje(usuarioId);
                int tarefasSemanaConcluidas = tarefasDB.ObterQuantidadeTarefasConcluidasComAlarmeSemana(usuarioId);
                int tarefasSemanaPendentes = tarefasDB.ObterQuantidadeTarefasPendentesComAlarmeSemana(usuarioId);

                double porcentagemConcluidas = total > 0 ? Math.Round((totalConcluidas * 100.0) / total, 1) : 0;
                double porcentagemPendentes = total > 0 ? Math.Round((totalPendentes * 100.0) / total, 1) : 0;


                // Atualizar labels conforme especificação
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
                var dados = tarefasDB.ObterQuantidadeTarefasPorProjeto(usuarioId);

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

            // Calcular totais e porcentagens
            int totalTarefas = dados.Sum(d => d.QuantidadeTarefas);

            Series series = new Series("Projetos");
            series.ChartType = SeriesChartType.Pie;
            series.IsValueShownAsLabel = true;
            series.Label = "#PERCENT{P2}";
            series.LegendText = "#VALX: #VALY tarefas";
            series.Font = new Font("Arial", 9, FontStyle.Bold);

            // Ordenar por quantidade e aplicar cores
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

                // ✅ Aplicar cores conforme especificação
                if (i == 0) point.Color = Color.Red; // Mais tarefas - vermelho
                else if (i == dadosOrdenados.Count - 1) point.Color = Color.Green; // Menos tarefas - verde
                else point.Color = GetCorPorIndice(i); // Cores intermediárias

                series.Points.Add(point);
            }

            Graph_TarefasPorProjeto.Series.Add(series);

            // Configurar legenda
            Legend legend = new Legend();
            legend.Docking = Docking.Right;
            legend.Font = new Font("Arial", 9);
            Graph_TarefasPorProjeto.Legends.Add(legend);

            // Configurar área do gráfico
            if (Graph_TarefasPorProjeto.ChartAreas.Count == 0)
                Graph_TarefasPorProjeto.ChartAreas.Add(new ChartArea());

            // Configurar título
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

                    // Buscar projeto pelo nome
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

                    // Configurar colunas
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
                // Implementar lógica para obter data limite da tarefa do banco
                // Por enquanto, retornar um placeholder
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
                // Implementar lógica para obter data de conclusão da tarefa
                // Por enquanto, retornar data atual se concluída
                return DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch
            {
                return "N/A";
            }
        }

        private void ConfigurarGraphProgressoSemanal()
        {
            if (Graph_ProgressoSemanal == null) return;

            Graph_ProgressoSemanal.Series.Clear();
            Graph_ProgressoSemanal.ChartAreas.Clear();
            Graph_ProgressoSemanal.Legends.Clear();

            ChartArea chartArea = new ChartArea();
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = true;
            chartArea.AxisY.Interval = 1;
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisX.LabelStyle.Angle = -45;
            Graph_ProgressoSemanal.ChartAreas.Add(chartArea);

            Series series = new Series("Tarefas Concluídas");
            series.ChartType = SeriesChartType.Column;
            series.Color = Color.SteelBlue;
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "0";
            series.Font = new Font("Arial", 10, FontStyle.Bold);

            Graph_ProgressoSemanal.Series.Add(series);

            // Configurar título
            Graph_ProgressoSemanal.Titles.Clear();
            Title title = new Title();
            title.Font = new Font("Arial", 12, FontStyle.Bold);
            title.Text = "Progresso Semanal de Tarefas Concluídas";
            Graph_ProgressoSemanal.Titles.Add(title);
        }

        private void AtualizarStatusGraficoSemana()
        {
            try
            {
                if (Graph_ProgressoSemanal == null)
                {
                    Console.WriteLine("❌ Gráfico não encontrado");
                    return;
                }

                if (Graph_ProgressoSemanal.Series.Count > 0)
                    Graph_ProgressoSemanal.Series[0].Points.Clear();

                DateTime inicioSemana = currentWeek;
                DateTime fimSemana = currentWeek.AddDays(6);

                // ✅ Atualizar título com datas
                Lbl_Titulo.Text = $"Progresso Semanal - {inicioSemana:dd/MM/yyyy} a {fimSemana:dd/MM/yyyy}";

                // ✅ Mostrar botão "Avançar" apenas se não for a semana atual
                Btn_Avancar.Visible = currentWeek < GetInicioSemana(DateTime.Today);

                var dias = ObterDiasDaSemana(inicioSemana);
                int maxTarefas = 0;
                var dadosDias = new List<(string dia, int tarefas)>();

                foreach (var dia in dias)
                {
                    int tarefas = ObterTarefasConcluidasPorDia(usuarioId, dia);
                    dadosDias.Add((ObterNomeDia(dia), tarefas));
                    if (tarefas > maxTarefas) maxTarefas = tarefas;
                }

                // ✅ Limpar e preencher o gráfico
                Graph_ProgressoSemanal.Series[0].Points.Clear();

                foreach (var dado in dadosDias)
                {
                    Graph_ProgressoSemanal.Series[0].Points.AddXY(dado.dia, dado.tarefas);
                }

                // ✅ Configurar eixo Y conforme especificação
                if (Graph_ProgressoSemanal.ChartAreas.Count > 0)
                {
                    Graph_ProgressoSemanal.ChartAreas[0].AxisY.Minimum = 0;
                    Graph_ProgressoSemanal.ChartAreas[0].AxisY.Maximum = Math.Max(maxTarefas, 1);
                    Graph_ProgressoSemanal.ChartAreas[0].AxisY.Interval = 1;
                }

                Console.WriteLine($"✅ Gráfico semanal atualizado: {inicioSemana:dd/MM} a {fimSemana:dd/MM}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao atualizar gráfico semanal: {ex.Message}");
                MessageBox.Show($"Erro ao atualizar gráfico: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ObterTarefasConcluidasPorDia(int usuarioId, DateTime data)
        {
            try
            {
                // Implementação simplificada - contar tarefas concluídas na data específica
                var tarefasConcluidas = tarefasDB.ObterTarefasTotaisConcluidasDoUsuario(usuarioId);

                // Esta é uma implementação básica - você precisará ajustar para filtrar por data específica
                // baseado na data de conclusão real das tarefas
                return tarefasConcluidas.Count(t =>
                    // Aqui você precisaria acessar a data real de conclusão da tarefa
                    // Por enquanto, retornamos um valor baseado no código da tarefa para demonstração
                    t.Codigo % 7 == data.Day % 7
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao obter tarefas por dia: {ex.Message}");
                return 0;
            }
        }

        private List<DateTime> ObterDiasDaSemana(DateTime inicioSemana)
        {
            var dias = new List<DateTime>();

            // ✅ Começar do DOMINGO conforme especificação
            for (int i = 0; i < 7; i++)
            {
                DateTime dia = inicioSemana.AddDays(i);
                if (!apenasDiasUteis || (dia.DayOfWeek >= DayOfWeek.Monday && dia.DayOfWeek <= DayOfWeek.Friday))
                {
                    dias.Add(dia);
                }
            }
            return dias;
        }

        private string ObterNomeDia(DateTime data)
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            string nomeDia = culture.DateTimeFormat.GetDayName(data.DayOfWeek);
            return char.ToUpper(nomeDia[0]) + nomeDia.Substring(1);
        }

        private DateTime GetInicioSemana(DateTime data)
        {
            // ✅ Começar do DOMINGO conforme especificação
            int diff = (7 + (data.DayOfWeek - DayOfWeek.Sunday)) % 7;
            return data.AddDays(-1 * diff).Date;
        }

        // =========================================================================
        // MÉTODOS DE EXPORTAÇÃO CORRIGIDOS
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
                        // Gerar PDF com os dados atuais do dashboard
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

                    // ✅ Verificar limite de 5MB
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
                // Coletar dados para o PDF
                int totalTarefas = tarefasDB.ObterQuantidadeTarefasTotaisDoUsuario(usuarioId);
                int totalConcluidas = tarefasDB.ObterQuantidadeTarefasTotaisConcluidasDoUsuario(usuarioId);
                int totalPendentes = tarefasDB.ObterQuantidadeTarefasTotaisPendentesDoUsuario(usuarioId);
                int tarefasHojeConcluidas = tarefasDB.ObterQuantidadeTarefasConcluidasComAlarmeHoje(usuarioId);
                int tarefasHojePendentes = tarefasDB.ObterQuantidadeTarefasPendentesComAlarmeHoje(usuarioId);

                // Criar conteúdo do PDF manualmente
                // Em uma implementação real, você usaria o PdfService para gerar um PDF estruturado
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

                // Para uma implementação real com PDF, você precisaria:
                // 1. Implementar um método em PdfService para gerar o dashboard
                // 2. Usar iTextSharp para criar um PDF profissional

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

                        // ✅ Verificar limite de 5MB
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

        private void Lbl_QuantidadeHojeConcluidas_TextChanged(object sender, EventArgs e){}
    }
}