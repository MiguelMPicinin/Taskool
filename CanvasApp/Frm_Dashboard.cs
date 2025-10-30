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
        private AlarmeDB alarmeDB;
        private int usuarioId;

        private DateTime currentWeek;
        private bool apenasDiasUteis = false;

        // VARIÁVEIS DO CAMINHO CRÍTICO
        private List<Projeto_Tarefas> tarefasCaminhoCritico;
        private bool exibindoMaisTarefas = false;

        public Frm_Dashboard()
        {
            InitializeComponent();

            // Inicialização do usuário
            if (Sessao.UsuarioLogado != null)
            {
                usuarioId = Sessao.UsuarioLogado.Codigo;
            }
            else
            {
                usuarioId = 1;
            }

            currentWeek = GetInicioSemana(DateTime.Today);

            // Inicialização dos serviços e bancos de dados
            pdfService = new PdfService();
            ftpManager = new FTPManager();
            usuarioDB = new UsuarioDB();
            tarefasDB = new TarefasDB();
            projetosDB = new ProjetosDB();
            alarmeDB = new AlarmeDB();

            // CONFIGURAÇÃO INICIAL DO CAMINHO CRÍTICO
            InicializarCaminhoCritico();

            // Configuração inicial da UI
            Pnl_CC5.Visible = false;
            Pnl_CC6.Visible = false;
            Pnl_CC7.Visible = false;

            // Configuração de eventos
            ConfigurarEventos();

            // Configuração dos gráficos
            ConfigurarGraphProgressoSemanal();

            // CORREÇÃO: Atualizar visibilidade do botão na inicialização
            AtualizarVisibilidadeBotaoAvancar();
        }

        private void InicializarCaminhoCritico()
        {
            tarefasCaminhoCritico = new List<Projeto_Tarefas>();

            // Configurar evento do botão Ver Mais
            Btn_VerMais.Click += Btn_VerMais_Click;

            // Carregar dados iniciais
            CarregarCaminhoCritico();
        }

        private void CarregarCaminhoCritico()
        {
            try
            {
                // Obter tarefas não concluídas com data limite futura, ordenadas pela mais próxima
                tarefasCaminhoCritico = tarefasDB.ObterTarefasParaCaminhoCritico(usuarioId);

                Console.WriteLine($"📊 Caminho Crítico: {tarefasCaminhoCritico.Count} tarefas encontradas");

                AtualizarExibicaoCaminhoCritico();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao carregar caminho crítico: {ex.Message}");
                LimparCaminhoCritico();
            }
        }

        private void AtualizarExibicaoCaminhoCritico()
        {
            try
            {
                // Limpar todos os painéis primeiro
                LimparCaminhoCritico();

                if (!tarefasCaminhoCritico.Any())
                {
                    // Não há tarefas para exibir
                    Btn_VerMais.Visible = false;
                    return;
                }

                // Determinar quantas tarefas exibir
                int tarefasParaExibir = exibindoMaisTarefas ?
                    Math.Min(tarefasCaminhoCritico.Count, 7) :
                    Math.Min(tarefasCaminhoCritico.Count, 4);

                // Array de painéis e labels
                Panel[] panels = { Pnl_CC1, Pnl_CC2, Pnl_CC3, Pnl_CC4, Pnl_CC5, Pnl_CC6, Pnl_CC7 };
                Label[] labelsConteudo = { Lbl_Conteudo1, Lbl_Conteudo2, Lbl_Conteudo3, Lbl_Conteudo4,
                                         Lbl_Conteudo5, Lbl_Conteudo6, Lbl_Conteudo7 };
                Label[] labelsTarefa = { Lbl_Tarefa1, Lbl_Tarefa2, Lbl_Tarefa3, Lbl_Tarefa4,
                                       Lbl_Tarefa5, Lbl_Tarefa6, Lbl_Tarefa7 };

                // Preencher os painéis visíveis
                for (int i = 0; i < tarefasParaExibir; i++)
                {
                    var tarefa = tarefasCaminhoCritico[i];
                    panels[i].Visible = true;

                    // Descrição da tarefa (limitando o tamanho se necessário)
                    string descricao = tarefa.Descricao.Length > 50 ?
                        tarefa.Descricao.Substring(0, 47) + "..." :
                        tarefa.Descricao;
                    labelsConteudo[i].Text = descricao;

                    // Data limite formatada
                    labelsTarefa[i].Text = $"Limite: {tarefa.dataLimite:dd/MM/yyyy}";

                    // Aplicar estilo baseado na urgência
                    AplicarEstiloUrgencia(panels[i], labelsTarefa[i], tarefa.dataLimite);
                }

                // Configurar visibilidade do botão Ver Mais
                Btn_VerMais.Visible = tarefasCaminhoCritico.Count > 4;
                Btn_VerMais.Text = exibindoMaisTarefas ? "Ver Menos" : "Ver Mais";

                // Configurar scrollbar se necessário
                ConfigurarScrollbarCaminhoCritico();

                Console.WriteLine($"✅ Caminho Crítico atualizado: {tarefasParaExibir} tarefas exibidas");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao atualizar caminho crítico: {ex.Message}");
                LimparCaminhoCritico();
            }
        }

        private void AplicarEstiloUrgencia(Panel panel, Label labelTarefa, DateTime dataLimite)
        {
            var diasRestantes = (dataLimite - DateTime.Today).TotalDays;

            if (diasRestantes <= 1)
            {
                // Muito urgente (hoje ou amanhã) - vermelho
                panel.BackColor = Color.FromArgb(255, 240, 240);
                labelTarefa.ForeColor = Color.Red;
                labelTarefa.Font = new Font(labelTarefa.Font, FontStyle.Bold);
            }
            else if (diasRestantes <= 3)
            {
                // Urgente (2-3 dias) - laranja
                panel.BackColor = Color.FromArgb(255, 250, 240);
                labelTarefa.ForeColor = Color.OrangeRed;
                labelTarefa.Font = new Font(labelTarefa.Font, FontStyle.Bold);
            }
            else if (diasRestantes <= 7)
            {
                // Atenção (4-7 dias) - amarelo
                panel.BackColor = Color.FromArgb(255, 255, 240);
                labelTarefa.ForeColor = Color.Goldenrod;
            }
            else
            {
                // Normal - verde
                panel.BackColor = Color.FromArgb(240, 255, 240);
                labelTarefa.ForeColor = Color.DarkGreen;
            }
        }

        private void LimparCaminhoCritico()
        {
            // Array de painéis
            Panel[] panels = { Pnl_CC1, Pnl_CC2, Pnl_CC3, Pnl_CC4, Pnl_CC5, Pnl_CC6, Pnl_CC7 };
            Label[] labelsConteudo = { Lbl_Conteudo1, Lbl_Conteudo2, Lbl_Conteudo3, Lbl_Conteudo4,
                                     Lbl_Conteudo5, Lbl_Conteudo6, Lbl_Conteudo7 };
            Label[] labelsTarefa = { Lbl_Tarefa1, Lbl_Tarefa2, Lbl_Tarefa3, Lbl_Tarefa4,
                                   Lbl_Tarefa5, Lbl_Tarefa6, Lbl_Tarefa7 };

            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].Visible = false;
                labelsConteudo[i].Text = "";
                labelsTarefa[i].Text = "";
            }

            Btn_VerMais.Visible = false;
        }

        private void ConfigurarScrollbarCaminhoCritico()
        {
            if (exibindoMaisTarefas && tarefasCaminhoCritico.Count > 4)
            {
                Pnl_CaminhoCritico.AutoScroll = true;
                Pnl_CaminhoCritico.VerticalScroll.Visible = true;
                Pnl_CaminhoCritico.VerticalScroll.Enabled = true;
            }
            else
            {
                Pnl_CaminhoCritico.AutoScroll = false;
                Pnl_CaminhoCritico.VerticalScroll.Visible = false;
            }
        }

        private void Btn_VerMais_Click(object sender, EventArgs e)
        {
            exibindoMaisTarefas = !exibindoMaisTarefas;
            AtualizarExibicaoCaminhoCritico();
        }

        // Método para atualizar quando há mudanças nas tarefas
        public void AtualizarCaminhoCritico()
        {
            CarregarCaminhoCritico();
        }

        // MÉTODOS EXISTENTES DO DASHBOARD (mantidos conforme seu código original)
        private void ConfigurarEventos()
        {
            this.Load += (s, e) =>
            {
                TestarConexaoBanco();
                AtualizarTextosTarefas();
                AtualizarGraficoCircular();
                AtualizarGraficoSemanal();
                AtualizarVisibilidadeBotaoAvancar();
                CarregarCaminhoCritico(); // Garantir que o caminho crítico seja carregado
            };

            Btn_Anterior.Click += (s, e) =>
            {
                currentWeek = currentWeek.AddDays(-7);
                AtualizarGraficoSemanal();
                AtualizarVisibilidadeBotaoAvancar();
            };

            Btn_Avancar.Click += (s, e) =>
            {
                DateTime nextWeek = currentWeek.AddDays(7);
                DateTime currentDate = DateTime.Today;
                DateTime maxWeek = GetInicioSemana(currentDate);

                if (nextWeek <= maxWeek)
                {
                    currentWeek = nextWeek;
                    AtualizarGraficoSemanal();
                    AtualizarVisibilidadeBotaoAvancar();
                }
                else
                {
                    MessageBox.Show("Não é possível visualizar semanas futuras.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            Btn_ApenasUteis.Click += (s, e) =>
            {
                apenasDiasUteis = !apenasDiasUteis;
                AtualizarGraficoSemanal();
                Btn_ApenasUteis.Text = apenasDiasUteis ? "Mostrar todos os dias" : "Considerar apenas dias úteis";
            };

            if (Graph_TarefasPorProjeto != null)
            {
                Graph_TarefasPorProjeto.MouseClick += Graph_TarefasPorProjeto_MouseClick;
            }

            Btn_PDF.Click += Btn_PDF_Click;
            Btn_FTP.Click += Btn_FTP_Click;
        }

        private void AtualizarVisibilidadeBotaoAvancar()
        {
            try
            {
                DateTime semanaAtual = GetInicioSemana(DateTime.Today);
                DateTime proximaSemana = currentWeek.AddDays(7);

                Btn_Avancar.Visible = (proximaSemana <= semanaAtual);

                Console.WriteLine($"🔍 Controle de botão: CurrentWeek: {currentWeek:dd/MM/yyyy}");
                Console.WriteLine($"🔍 Controle de botão: Próxima semana: {proximaSemana:dd/MM/yyyy}");
                Console.WriteLine($"🔍 Controle de botão: Semana atual: {semanaAtual:dd/MM/yyyy}");
                Console.WriteLine($"🔍 Controle de botão: Botão visível: {Btn_Avancar.Visible}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao atualizar visibilidade do botão: {ex.Message}");
                Btn_Avancar.Visible = false;
            }
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
                // Obter totais gerais
                var todasTarefas = tarefasDB.ObterTodasTarefasDoUsuario(usuarioId);
                int total = todasTarefas.Count;
                int totalConcluidas = todasTarefas.Count(t => t.isConcluida);
                int totalPendentes = todasTarefas.Count(t => !t.isConcluida);

                // CORREÇÃO: Usar métodos baseados em DATA LIMITE
                var resultadoHoje = tarefasDB.ContarTarefasComDataLimiteHoje(usuarioId);
                int tarefasHojeConcluidas = resultadoHoje.concluidas;
                int tarefasHojePendentes = resultadoHoje.pendentes;

                var resultadoSemana = tarefasDB.ContarTarefasComDataLimiteSemana(usuarioId);
                int tarefasSemanaConcluidas = resultadoSemana.concluidas;
                int tarefasSemanaPendentes = resultadoSemana.pendentes;

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
                MessageBox.Show($"Erro ao atualizar dashboard: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarGraficoSemanal()
        {
            try
            {
                if (Graph_ProgressoSemanal == null) return;

                // Limpar séries
                foreach (var series in Graph_ProgressoSemanal.Series)
                {
                    series.Points.Clear();
                }

                DateTime inicioSemana = currentWeek;
                DateTime fimSemana = currentWeek.AddDays(6);

                // Não permitir semanas futuras
                DateTime semanaAtual = GetInicioSemana(DateTime.Today);
                if (inicioSemana > semanaAtual)
                {
                    currentWeek = semanaAtual;
                    inicioSemana = currentWeek;
                    fimSemana = currentWeek.AddDays(6);
                }

                Lbl_Titulo.Text = $"Progresso Semanal - {inicioSemana:dd/MM/yyyy} a {fimSemana:dd/MM/yyyy}";

                // CORREÇÃO: Obter tarefas com DATA LIMITE na semana
                var tarefasComDataLimiteSemana = tarefasDB.ObterTarefasComDataLimiteNoPeriodo(usuarioId, inicioSemana, fimSemana.AddDays(1))
                    .ToList();

                var dias = ObterDiasDaSemana(inicioSemana);

                int maxTarefas = 0;

                foreach (var dia in dias)
                {
                    string nomeDia = ObterNomeDia(dia);

                    // CORREÇÃO: Contagem baseada em DATA LIMITE
                    var tarefasDoDia = tarefasComDataLimiteSemana
                        .Where(t => t.dataLimite.Date == dia.Date)
                        .ToList();

                    int concluidas = tarefasDoDia.Count(t => t.isConcluida);
                    int pendentes = tarefasDoDia.Count(t => !t.isConcluida);

                    Console.WriteLine($"📅 {nomeDia} ({dia:dd/MM}): {concluidas} concluídas, {pendentes} pendentes (por DATA LIMITE)");

                    // Adicionar às séries
                    if (Graph_ProgressoSemanal.Series.Count >= 2)
                    {
                        Graph_ProgressoSemanal.Series["TarefasConcluidas"].Points.AddXY(nomeDia, concluidas);
                        Graph_ProgressoSemanal.Series["TarefasPendentes"].Points.AddXY(nomeDia, pendentes);
                    }

                    if (concluidas + pendentes > maxTarefas)
                        maxTarefas = concluidas + pendentes;
                }

                // Ajustar escala do eixo Y
                if (Graph_ProgressoSemanal.ChartAreas.Count > 0)
                {
                    ChartArea area = Graph_ProgressoSemanal.ChartAreas[0];
                    area.AxisY.Minimum = 0;
                    area.AxisY.Maximum = Math.Max(maxTarefas + 1, 2);
                    area.AxisY.Interval = 1;
                }

                Graph_ProgressoSemanal.Invalidate();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERRO no gráfico semanal: {ex.Message}");
            }
        }

        private void AtualizarGraficoCircular()
        {
            try
            {
                var dados = tarefasDB.ObterDadosGraficoTarefasPorProjeto(usuarioId);

                if (dados == null || !dados.Any())
                {
                    Console.WriteLine("ℹ️  Nenhum projeto com tarefas encontrado.");
                    return;
                }

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

        private void Graph_TarefasPorProjeto_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var hit = Graph_TarefasPorProjeto.HitTest(e.X, e.Y);

                if (hit != null && hit.PointIndex >= 0 && hit.Series != null)
                {
                    // Obter o nome do projeto clicado
                    string projetoNome = hit.Series.Points[hit.PointIndex].AxisLabel;

                    if (!string.IsNullOrEmpty(projetoNome))
                    {
                        Console.WriteLine($"🔍 Projeto clicado: {projetoNome}");

                        // Buscar o projeto pelo nome
                        var projetosUsuario = projetosDB.ObterTodosProjetosUsuario(usuarioId);
                        var projeto = projetosUsuario.FirstOrDefault(p =>
                            p.Nome.Equals(projetoNome, StringComparison.OrdinalIgnoreCase));

                        if (projeto != null)
                        {
                            Console.WriteLine($"✅ Projeto encontrado: {projeto.Nome} (ID: {projeto.Codigo})");
                            AbrirListaTarefasProjeto(projeto.Codigo, projeto.Nome);
                        }
                        else
                        {
                            MessageBox.Show($"Projeto '{projetoNome}' não encontrado.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ℹ️  Nenhuma fatia do gráfico foi clicada");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao processar clique no gráfico: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"❌ Erro no clique do gráfico: {ex.Message}");
            }
        }

        // CORREÇÃO: MÉTODO MELHORADO PARA ABRIR LISTA DE TAREFAS
        private void AbrirListaTarefasProjeto(int codProjeto, string nomeProjeto)
        {
            try
            {
                var tarefasProjeto = tarefasDB.ObterTarefasPorProjeto(codProjeto);

                if (!tarefasProjeto.Any())
                {
                    MessageBox.Show($"O projeto '{nomeProjeto}' não possui tarefas.", "Informação",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var formLista = new Form())
                {
                    formLista.Text = $"Tarefas do Projeto: {nomeProjeto}";
                    formLista.Size = new Size(900, 500);
                    formLista.StartPosition = FormStartPosition.CenterParent;
                    formLista.BackColor = Color.White;
                    formLista.FormBorderStyle = FormBorderStyle.FixedDialog;
                    formLista.MaximizeBox = false;

                    // Criar DataGridView
                    var dataGridView = new DataGridView
                    {
                        Dock = DockStyle.Fill,
                        ReadOnly = true,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                        BackgroundColor = Color.White,
                        BorderStyle = BorderStyle.None,
                        Font = new Font("Arial", 10),
                        AllowUserToAddRows = false,
                        AllowUserToDeleteRows = false,
                        AllowUserToOrderColumns = false,
                        RowHeadersVisible = false
                    };

                    // Configurar colunas
                    dataGridView.Columns.Add("Descricao", "Descrição da Tarefa");
                    dataGridView.Columns.Add("DataLimite", "Data Limite");
                    dataGridView.Columns.Add("DataConclusao", "Data Conclusão");
                    dataGridView.Columns.Add("Status", "Status");

                    // Preencher dados
                    foreach (var tarefa in tarefasProjeto)
                    {
                        string dataLimite = tarefa.dataLimite != DateTime.MinValue &&
                                           tarefa.dataLimite >= new DateTime(1753, 1, 1)
                            ? tarefa.dataLimite.ToString("dd/MM/yyyy")
                            : "Não definida";

                        string dataConclusao = tarefa.isConcluida &&
                                              tarefa.dataConclusao != DateTime.MinValue &&
                                              tarefa.dataConclusao >= new DateTime(1753, 1, 1)
                            ? tarefa.dataConclusao.ToString("dd/MM/yyyy")
                            : "";

                        string status = tarefa.isConcluida ? "Concluída" : "Pendente";

                        dataGridView.Rows.Add(tarefa.Descricao, dataLimite, dataConclusao, status);
                    }

                    // Ajustar largura das colunas
                    dataGridView.Columns["Descricao"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView.Columns["DataLimite"].Width = 120;
                    dataGridView.Columns["DataConclusao"].Width = 120;
                    dataGridView.Columns["Status"].Width = 100;

                    formLista.Controls.Add(dataGridView);
                    formLista.ShowDialog();
                }

                Console.WriteLine($"✅ Lista de tarefas aberta: {tarefasProjeto.Count} tarefas do projeto '{nomeProjeto}'");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar tarefas do projeto: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"❌ Erro ao abrir lista de tarefas: {ex.Message}");
            }
        }

        private void ConfigurarGraphProgressoSemanal()
        {
            if (Graph_ProgressoSemanal == null) return;

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

                // Série para tarefas concluídas
                Series seriesConcluidas = new Series("TarefasConcluidas");
                seriesConcluidas.ChartType = SeriesChartType.Column;
                seriesConcluidas.Color = Color.SteelBlue;
                seriesConcluidas.BorderColor = Color.DarkBlue;
                seriesConcluidas.BorderWidth = 2;
                seriesConcluidas.IsValueShownAsLabel = true;
                seriesConcluidas.LabelFormat = "0";
                seriesConcluidas.Font = new Font("Arial", 10, FontStyle.Bold);
                seriesConcluidas.LabelForeColor = Color.White;

                // Série para tarefas pendentes
                Series seriesPendentes = new Series("TarefasPendentes");
                seriesPendentes.ChartType = SeriesChartType.Column;
                seriesPendentes.Color = Color.Orange;
                seriesPendentes.BorderColor = Color.DarkOrange;
                seriesPendentes.BorderWidth = 2;
                seriesPendentes.IsValueShownAsLabel = true;
                seriesPendentes.LabelFormat = "0";
                seriesPendentes.Font = new Font("Arial", 10, FontStyle.Bold);
                seriesPendentes.LabelForeColor = Color.White;

                Graph_ProgressoSemanal.Series.Add(seriesConcluidas);
                Graph_ProgressoSemanal.Series.Add(seriesPendentes);

                Graph_ProgressoSemanal.Titles.Clear();
                Title title = new Title();
                title.Font = new Font("Arial", 12, FontStyle.Bold);
                title.Text = "Progresso Semanal de Tarefas";
                Graph_ProgressoSemanal.Titles.Add(title);

                // Adicionar legenda
                Legend legend = new Legend();
                legend.Docking = Docking.Top;
                legend.Font = new Font("Arial", 10, FontStyle.Bold);
                Graph_ProgressoSemanal.Legends.Add(legend);

                Console.WriteLine("✅ Gráfico semanal configurado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro na configuração do gráfico: {ex.Message}");
            }
        }

        // MÉTODOS AUXILIARES
        private List<DateTime> ObterDiasDaSemana(DateTime inicioSemana)
        {
            var dias = new List<DateTime>();

            for (int i = 0; i < 7; i++)
            {
                DateTime dia = inicioSemana.AddDays(i);
                bool ehDiaUtil = (dia.DayOfWeek >= DayOfWeek.Monday && dia.DayOfWeek <= DayOfWeek.Friday);
                bool incluirDia = !apenasDiasUteis || ehDiaUtil;

                if (incluirDia)
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
            return culture.TextInfo.ToTitleCase(nomeDia);
        }

        private DateTime GetInicioSemana(DateTime data)
        {
            int diff = (7 + (data.DayOfWeek - DayOfWeek.Sunday)) % 7;
            return data.AddDays(-1 * diff).Date;
        }

        private Color GetCorPorIndice(int index)
        {
            Color[] cores = {
                Color.SteelBlue, Color.Orange, Color.Purple,
                Color.Teal, Color.Maroon, Color.Olive, Color.Navy
            };
            return cores[index % cores.Length];
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
                            MessageBox.Show("Dashboard exportado para PDF com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_FTP_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funcionalidade FTP em desenvolvimento");
        }

        private bool GerarPDFDoDashboard(string caminhoArquivo)
        {
            try
            {
                var todasTarefas = tarefasDB.ObterTodasTarefasDoUsuario(usuarioId);
                int totalTarefas = todasTarefas.Count;
                int totalConcluidas = todasTarefas.Count(t => t.isConcluida);
                int totalPendentes = todasTarefas.Count(t => !t.isConcluida);

                // CORREÇÃO: Usar métodos baseados em DATA LIMITE
                var resultadoHojePDF = tarefasDB.ContarTarefasComDataLimiteHoje(usuarioId);
                int tarefasHojeConcluidasPDF = resultadoHojePDF.concluidas;
                int tarefasHojePendentesPDF = resultadoHojePDF.pendentes;

                var resultadoSemanaPDF = tarefasDB.ContarTarefasComDataLimiteSemana(usuarioId);
                int tarefasSemanaConcluidasPDF = resultadoSemanaPDF.concluidas;
                int tarefasSemanaPendentesPDF = resultadoSemanaPDF.pendentes;

                using (var writer = new StreamWriter(caminhoArquivo.Replace(".pdf", ".txt")))
                {
                    writer.WriteLine("=== DASHBOARD DO USUÁRIO ===");
                    writer.WriteLine($"Data: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    writer.WriteLine($"Usuário ID: {usuarioId}");
                    writer.WriteLine();
                    writer.WriteLine("=== RESUMO DE TAREFAS ===");
                    writer.WriteLine($"Total de Tarefas: {totalTarefas}");
                    writer.WriteLine($"Tarefas Concluídas: {totalConcluidas}");
                    writer.WriteLine($"Tarefas Pendentes: {totalPendentes}");
                    writer.WriteLine($"Porcentagem de Conclusão: {(totalTarefas > 0 ? Math.Round((totalConcluidas * 100.0) / totalTarefas, 1) : 0)}%");
                    writer.WriteLine();
                    writer.WriteLine("=== HOJE (COM DATA LIMITE) ===");
                    writer.WriteLine($"Concluídas: {tarefasHojeConcluidasPDF}");
                    writer.WriteLine($"Pendentes: {tarefasHojePendentesPDF}");
                    writer.WriteLine();
                    writer.WriteLine("=== SEMANA ATUAL (COM DATA LIMITE) ===");
                    writer.WriteLine($"Concluídas: {tarefasSemanaConcluidasPDF}");
                    writer.WriteLine($"Pendentes: {tarefasSemanaPendentesPDF}");
                }

                File.Move(caminhoArquivo.Replace(".pdf", ".txt"), caminhoArquivo);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void Lbl_QuantidadeHojeConcluidas_TextChanged(object sender, EventArgs e) { }

        private void Pnl_CC1_Paint(object sender, PaintEventArgs e) { }
    }
}