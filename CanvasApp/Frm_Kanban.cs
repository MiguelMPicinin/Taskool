using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using CanvasApp.Classes.ManipulaçãoDados;
using CanvasApp.Formularios_Pop_Ups;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;

namespace CanvasApp
{
    public partial class Frm_Kanban : Form
    {
        private int codProjeto;
        private string nomeProjeto;
        private int usuarioId;
        private List<Projeto_Tarefas> tarefas;
        private TarefasDB tarefaDB;
        private TarefasHistoricoDB historicoDB;
        private HistoricoDB historicoModificacoesDB = new HistoricoDB();

        // Cores dos post-its
        private List<string> coresPostIt = new List<string>
        {
            "#ffe079", // Amarelo
            "#f097ca", // Rosa
            "#98d366", // Verde
            "#82d3e5"  // Azul
        };

        // Construtor original (para compatibilidade)
        public Frm_Kanban(int codProjeto, string nomeProjeto)
            : this(codProjeto, nomeProjeto, 1)
        {
        }

        // Novo construtor com usuário
        public Frm_Kanban(int codProjeto, string nomeProjeto, int usuarioId)
        {
            InitializeComponent();
            this.codProjeto = codProjeto;
            this.nomeProjeto = nomeProjeto;
            this.usuarioId = usuarioId;
            this.tarefaDB = new TarefasDB();
            this.historicoDB = new TarefasHistoricoDB();

            ConfigurarFormulario();
            ConfigurarPainéis();
            ConfigurarDragDrop();
        }

        // =========================================================================
        // MÉTODOS DE HISTÓRICO E NOTIFICAÇÃO POR E-MAIL
        // =========================================================================

        private void RegistrarHistoricoModificacao(int codTarefa, int codUsuario, string acao, string nomeCartao, string nomeProjeto)
        {
            try
            {
                var historico = new HistoricoModificacoes
                {
                    CodTarefa = codTarefa,
                    CodUsuario = codUsuario,
                    Data = DateTime.Now,
                    Texto = $"{acao} o cartão '{nomeCartao}' no projeto '{nomeProjeto}'"
                };

                // Salvar no banco
                historicoModificacoesDB.InserirHistorico(historico);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao registrar histórico: {ex.Message}");
            }
        }

        private void EnviarEmailNotificacao(string acao, string nomeCartao, string nomeProjeto)
        {
            try
            {
                if (CanvasApp.Classes.Databases.UsuarioCL.Sessao.UsuarioLogado == null) return;

                using (SmtpClient smtpClient = new SmtpClient("127.0.0.1", 8087))
                {
                    smtpClient.EnableSsl = false;
                    smtpClient.UseDefaultCredentials = true;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress("worldskills2019@gmail.com");
                        mailMessage.To.Add(CanvasApp.Classes.Databases.UsuarioCL.Sessao.UsuarioLogado.Email ?? "destinatario@email.com");
                        mailMessage.Subject = $"Nova ação no projeto {nomeProjeto}";
                        mailMessage.Body = $"Olá, {CanvasApp.Classes.Databases.UsuarioCL.Sessao.UsuarioLogado.Nome}. Você acabou de realizar uma nova ação no projeto {nomeProjeto}: - {acao} o cartão {nomeCartao}.";
                        mailMessage.IsBodyHtml = false;

                        smtpClient.Send(mailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }

        // =========================================================================
        // MÉTODOS EXISTENTES (mantidos da versão anterior)
        // =========================================================================

        private void ConfigurarFormulario()
        {
            Text = $"Quadro Kanban - {nomeProjeto}";
            Lbl_Titulo.Text = $"Quadro Kanban - {nomeProjeto}";
            Lbl_Titulo.Font = new Font("Arial", 14, FontStyle.Bold);
        }

        private void ConfigurarPainéis()
        {
            // Configurar FlowLayoutPanels
            Flw_AFazer.BackColor = Color.LightGray;
            Flw_Fazendo.BackColor = Color.LightGray;
            Flw_Feito.BackColor = Color.LightGray;

            Flw_AFazer.FlowDirection = FlowDirection.TopDown;
            Flw_Fazendo.FlowDirection = FlowDirection.TopDown;
            Flw_Feito.FlowDirection = FlowDirection.TopDown;

            Flw_AFazer.WrapContents = false;
            Flw_Fazendo.WrapContents = false;
            Flw_Feito.WrapContents = false;

            Flw_AFazer.AutoScroll = true;
            Flw_Fazendo.AutoScroll = true;
            Flw_Feito.AutoScroll = true;

            Flw_AFazer.AllowDrop = true;
            Flw_Fazendo.AllowDrop = true;
            Flw_Feito.AllowDrop = true;
        }

        private void ConfigurarDragDrop()
        {
            // Configurar eventos de Drag and Drop para os FlowLayoutPanels
            Flw_AFazer.DragEnter += Painel_DragEnter;
            Flw_Fazendo.DragEnter += Painel_DragEnter;
            Flw_Feito.DragEnter += Painel_DragEnter;

            Flw_AFazer.DragDrop += Painel_DragDrop;
            Flw_Fazendo.DragDrop += Painel_DragDrop;
            Flw_Feito.DragDrop += Painel_DragDrop;
        }

        private void Frm_Kanban_Load(object sender, EventArgs e)
        {
            CarregarTarefas();
        }

        private void CarregarTarefas()
        {
            try
            {
                tarefas = tarefaDB.ObterTarefasKanbanPorProjeto(codProjeto);

                if (tarefas == null)
                {
                    tarefas = new List<Projeto_Tarefas>();
                }

                AtualizarQuadro();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar tarefas: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                tarefas = new List<Projeto_Tarefas>();
            }
        }

        private void AtualizarQuadro()
        {
            LimparTodosPostIts();

            if (tarefas == null || !tarefas.Any())
            {
                AtualizarContadores();
                return;
            }

            // Distribuir tarefas pelos painéis
            foreach (var tarefa in tarefas)
            {
                var postIt = CriarPostIt(tarefa);
                if (postIt != null)
                {
                    if (tarefa.isConcluida)
                    {
                        Flw_Feito.Controls.Add(postIt);
                    }
                    else if (tarefa.isFazendo)
                    {
                        Flw_Fazendo.Controls.Add(postIt);
                    }
                    else
                    {
                        Flw_AFazer.Controls.Add(postIt);
                    }
                }
            }

            AtualizarContadores();
        }

        private void AtualizarContadores()
        {
            int aFazer = Flw_AFazer.Controls.Count;
            int fazendo = Flw_Fazendo.Controls.Count;
            int feito = Flw_Feito.Controls.Count;

            Lbl_aFazer.Text = $"A Fazer ({aFazer})";
            Lbl_Fazendo.Text = $"Fazendo ({fazendo})";
            Lbl_Feito.Text = $"Feito ({feito})";
        }

        private Panel CriarPostIt(Projeto_Tarefas tarefa)
        {
            try
            {
                var panel = new Panel
                {
                    Size = new Size(180, 150),
                    BackColor = ObterCor(tarefa.Cor),
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = tarefa,
                    Cursor = Cursors.Hand,
                    Margin = new Padding(10),
                    Padding = new Padding(5)
                };

                // TextBox para descrição (editável)
                var txtDescricao = new TextBox
                {
                    Text = tarefa.Descricao,
                    Location = new Point(5, 30),
                    Size = new Size(160, 80),
                    Multiline = true,
                    BorderStyle = BorderStyle.None,
                    BackColor = ObterCor(tarefa.Cor),
                    Font = new Font("Arial", 9),
                    ForeColor = Color.Black,
                    ScrollBars = ScrollBars.Vertical,
                    Tag = tarefa
                };

                txtDescricao.TextChanged += (s, e) => AtualizarDescricaoTarefa(tarefa.Codigo, txtDescricao.Text);

                // Label para ID da tarefa
                var lblId = new Label
                {
                    Text = $"ID: {tarefa.Codigo}",
                    Location = new Point(5, 5),
                    Size = new Size(120, 20),
                    Font = new Font("Arial", 7),
                    ForeColor = Color.Gray,
                    BackColor = Color.Transparent
                };

                // Label para data de conclusão (se existir)
                if (tarefa.isConcluida && tarefa.dataConclusao != DateTime.MinValue)
                {
                    var lblConclusao = new Label
                    {
                        Text = $"Concluído: {tarefa.dataConclusao:dd/MM/yy}",
                        Location = new Point(5, 125),
                        Size = new Size(120, 20),
                        Font = new Font("Arial", 7, FontStyle.Bold),
                        ForeColor = Color.DarkGreen,
                        BackColor = Color.Transparent
                    };
                    panel.Controls.Add(lblConclusao);
                }

                // Botão excluir
                var btnExcluir = new Button
                {
                    Text = "X",
                    Size = new Size(20, 20),
                    Location = new Point(155, 5),
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.Red,
                    Font = new Font("Arial", 8, FontStyle.Bold),
                    Tag = tarefa
                };
                btnExcluir.FlatAppearance.BorderSize = 0;
                btnExcluir.Click += (s, e) => ExcluirTarefa(tarefa);

                // Botão histórico
                var btnHistorico = new Button
                {
                    Text = "H",
                    Size = new Size(20, 20),
                    Location = new Point(135, 5),
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.Blue,
                    Font = new Font("Arial", 8, FontStyle.Bold),
                    Tag = tarefa
                };
                btnHistorico.FlatAppearance.BorderSize = 0;
                btnHistorico.Click += (s, e) => MostrarHistoricoTarefa(tarefa);

                panel.Controls.Add(txtDescricao);
                panel.Controls.Add(lblId);
                panel.Controls.Add(btnExcluir);
                panel.Controls.Add(btnHistorico);

                // Eventos de Drag and Drop
                panel.MouseDown += PostIt_MouseDown;
                panel.DragEnter += PostIt_DragEnter;

                return panel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar post-it: {ex.Message}", "Erro");
                return null;
            }
        }

        private void PostIt_MouseDown(object sender, MouseEventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null && e.Button == MouseButtons.Left)
            {
                panel.DoDragDrop(panel, DragDropEffects.Move);
            }
        }

        private void PostIt_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void Painel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        // =========================================================================
        // MÉTODO MODIFICADO: Painel_DragDrop COM HISTÓRICO E EMAIL
        // =========================================================================
        private void Painel_DragDrop(object sender, DragEventArgs e)
        {
            var painelDestino = sender as FlowLayoutPanel;
            var panelPostIt = e.Data.GetData(typeof(Panel)) as Panel;

            if (painelDestino != null && panelPostIt != null)
            {
                var tarefa = panelPostIt.Tag as Projeto_Tarefas;
                var painelOrigem = panelPostIt.Parent as FlowLayoutPanel;

                if (painelOrigem != painelDestino && tarefa != null)
                {
                    // Mover visualmente
                    painelOrigem.Controls.Remove(panelPostIt);
                    painelDestino.Controls.Add(panelPostIt);

                    // Atualizar status no banco
                    AtualizarStatusTarefa(tarefa, painelDestino);

                    // Registrar no histórico
                    RegistrarMovimentacao(tarefa, painelOrigem, painelDestino);

                    // REGISTRAR HISTÓRICO DE MODIFICAÇÃO E ENVIAR EMAIL
                    string deStatus = ObterNomeStatus(painelOrigem);
                    string paraStatus = ObterNomeStatus(painelDestino);

                    RegistrarHistoricoModificacao(tarefa.Codigo, usuarioId, $"Moveu de {deStatus} para {paraStatus}", tarefa.Descricao, nomeProjeto);
                    EnviarEmailNotificacao($"Moveu de {deStatus} para {paraStatus}", tarefa.Descricao, nomeProjeto);

                    AtualizarContadores();
                }
            }
        }

        private void AtualizarStatusTarefa(Projeto_Tarefas tarefa, FlowLayoutPanel painelDestino)
        {
            bool concluida = false;
            bool fazendo = false;

            if (painelDestino == Flw_AFazer)
            {
                concluida = false;
                fazendo = false;
            }
            else if (painelDestino == Flw_Fazendo)
            {
                concluida = false;
                fazendo = true;
            }
            else if (painelDestino == Flw_Feito)
            {
                concluida = true;
                fazendo = false;
            }

            if (!tarefaDB.AtualizarStatusKanban(tarefa.Codigo, concluida, fazendo))
            {
                MessageBox.Show($"Erro ao atualizar status: {tarefaDB.Mensagem}", "Erro");
            }
        }

        private void RegistrarMovimentacao(Projeto_Tarefas tarefa, FlowLayoutPanel origem, FlowLayoutPanel destino)
        {
            string deStatus = ObterNomeStatus(origem);
            string paraStatus = ObterNomeStatus(destino);

            historicoDB.RegistrarMovimentacaoTarefa(tarefa.Codigo, usuarioId, deStatus, paraStatus);
        }

        private string ObterNomeStatus(FlowLayoutPanel painel)
        {
            if (painel == Flw_AFazer) return "A Fazer";
            if (painel == Flw_Fazendo) return "Fazendo";
            if (painel == Flw_Feito) return "Feito";
            return "Desconhecido";
        }

        private Color ObterCor(string corHex)
        {
            try
            {
                if (string.IsNullOrEmpty(corHex) || !corHex.StartsWith("#"))
                    return ColorTranslator.FromHtml("#ffe079");

                return ColorTranslator.FromHtml(corHex);
            }
            catch
            {
                return ColorTranslator.FromHtml("#ffe079");
            }
        }

        private void LimparTodosPostIts()
        {
            LimparPainel(Flw_AFazer);
            LimparPainel(Flw_Fazendo);
            LimparPainel(Flw_Feito);
        }

        private void LimparPainel(FlowLayoutPanel painel)
        {
            var controlesParaRemover = painel.Controls.OfType<Panel>().ToList();

            foreach (var controle in controlesParaRemover)
            {
                painel.Controls.Remove(controle);
                controle.Dispose();
            }
        }

        private void Btn_AddPostIt_Click(object sender, EventArgs e)
        {
            AdicionarNovaTarefa();
        }

        // =========================================================================
        // MÉTODO MODIFICADO: AdicionarNovaTarefa COM HISTÓRICO E EMAIL
        // =========================================================================
        private void AdicionarNovaTarefa()
        {
            try
            {
                using (var form = new Frm_AdicionarPostIt())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        var novaTarefa = new Projeto_Tarefas
                        {
                            CodProjeto = codProjeto,
                            Descricao = form.DescricaoTarefa,
                            Cor = form.CorSelecionada,
                            isConcluida = false,
                            isFazendo = false,
                            dataConclusao = DateTime.MinValue,
                            dataLimite = DateTime.Now.AddDays(7)
                        };

                        if (tarefaDB.InserirTarefaKanban(novaTarefa))
                        {
                            // Registrar criação no histórico
                            var tarefaInserida = tarefaDB.ObterTarefasKanbanPorProjeto(codProjeto)
                                .OrderByDescending(t => t.Codigo)
                                .FirstOrDefault();

                            if (tarefaInserida != null)
                            {
                                historicoDB.RegistrarCriacaoTarefa(tarefaInserida.Codigo, usuarioId);

                                // REGISTRAR HISTÓRICO DE MODIFICAÇÃO E ENVIAR EMAIL
                                RegistrarHistoricoModificacao(tarefaInserida.Codigo, usuarioId, "Criou", form.DescricaoTarefa, nomeProjeto);
                                EnviarEmailNotificacao("Criou", form.DescricaoTarefa, nomeProjeto);
                            }

                            CarregarTarefas();
                        }
                        else
                        {
                            MessageBox.Show($"Erro ao criar tarefa: {tarefaDB.Mensagem}", "Erro");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar tarefa: {ex.Message}", "Erro");
            }
        }

        // =========================================================================
        // MÉTODO MODIFICADO: AtualizarDescricaoTarefa COM HISTÓRICO E EMAIL
        // =========================================================================
        private void AtualizarDescricaoTarefa(int codTarefa, string novaDescricao)
        {
            try
            {
                var tarefa = tarefas.FirstOrDefault(t => t.Codigo == codTarefa);
                if (tarefa != null && tarefa.Descricao != novaDescricao)
                {
                    string descricaoAntiga = tarefa.Descricao;
                    tarefa.Descricao = novaDescricao;

                    if (!tarefaDB.AtualizarTarefa(tarefa))
                    {
                        MessageBox.Show($"Erro ao atualizar descrição: {tarefaDB.Mensagem}", "Erro");
                    }
                    else
                    {
                        // Registrar edição no histórico
                        historicoDB.RegistrarEdicaoDescricao(codTarefa, usuarioId, novaDescricao);

                        // REGISTRAR HISTÓRICO DE MODIFICAÇÃO E ENVIAR EMAIL
                        RegistrarHistoricoModificacao(codTarefa, usuarioId, "Editou", $"{descricaoAntiga} para {novaDescricao}", nomeProjeto);
                        EnviarEmailNotificacao("Editou", $"{descricaoAntiga} para {novaDescricao}", nomeProjeto);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar descrição: {ex.Message}", "Erro");
            }
        }

        // =========================================================================
        // MÉTODO MODIFICADO: ExcluirTarefa COM HISTÓRICO E EMAIL
        // =========================================================================
        private void ExcluirTarefa(Projeto_Tarefas tarefa)
        {
            try
            {
                var resultado = MessageBox.Show($"Deseja excluir a tarefa '{tarefa.Descricao}'?",
                                              "Confirmar Exclusão",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    // Registrar exclusão no histórico antes de excluir
                    historicoDB.RegistrarExclusaoTarefa(tarefa.Codigo, usuarioId);

                    // REGISTRAR HISTÓRICO DE MODIFICAÇÃO E ENVIAR EMAIL
                    RegistrarHistoricoModificacao(tarefa.Codigo, usuarioId, "Excluiu", tarefa.Descricao, nomeProjeto);
                    EnviarEmailNotificacao("Excluiu", tarefa.Descricao, nomeProjeto);

                    if (tarefaDB.ExcluirTarefa(tarefa.Codigo))
                    {
                        CarregarTarefas();
                        MessageBox.Show("Tarefa excluída com sucesso!", "Sucesso",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Erro ao excluir tarefa: {tarefaDB.Mensagem}", "Erro",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir tarefa: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarHistoricoTarefa(Projeto_Tarefas tarefa)
        {
            try
            {
                var historicos = historicoDB.ObterHistoricoPorTarefa(tarefa.Codigo);
                var historicosModificacoes = historicoModificacoesDB.ObterHistoricoPorTarefa(tarefa.Codigo);

                var historicoFormatado = new List<string>();

                // Adicionar histórico de movimentação
                foreach (var historico in historicos)
                {
                    historicoFormatado.Add($"{historico.DataAcao:dd/MM/yyyy HH:mm} - {historico.Acao}");
                }

                // Adicionar histórico de modificações
                foreach (var historico in historicosModificacoes)
                {
                    historicoFormatado.Add($"{historico.Data:dd/MM/yyyy HH:mm} - {historico.Texto}");
                }

                // Ordenar por data
                historicoFormatado = historicoFormatado.OrderByDescending(h => h).ToList();

                using (var formHistorico = new Frm_HistoricoTarefas(historicoFormatado))
                {
                    formHistorico.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar histórico: {ex.Message}", "Erro");
            }
        }

        private void Frm_Kanban_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Limpar recursos se necessário
        }

        private void Btn_Atividade_Click(object sender, EventArgs e)
        {
            try
            {
                // Mostrar histórico geral do projeto
                var todasTarefas = tarefaDB.ObterTarefasKanbanPorProjeto(codProjeto);
                var todosHistoricos = new List<string>();

                foreach (var tarefa in todasTarefas)
                {
                    var historicos = historicoDB.ObterHistoricoPorTarefa(tarefa.Codigo);
                    var historicosModificacoes = historicoModificacoesDB.ObterHistoricoPorTarefa(tarefa.Codigo);

                    foreach (var historico in historicos)
                    {
                        todosHistoricos.Add($"Tarefa {tarefa.Codigo}: {historico.DataAcao:dd/MM/yyyy HH:mm} - {historico.Acao}");
                    }

                    foreach (var historico in historicosModificacoes)
                    {
                        todosHistoricos.Add($"Tarefa {tarefa.Codigo}: {historico.Data:dd/MM/yyyy HH:mm} - {historico.Texto}");
                    }
                }

                todosHistoricos = todosHistoricos.OrderByDescending(h => h).ToList();

                using (var formHistorico = new Frm_HistoricoTarefas(todosHistoricos))
                {
                    formHistorico.Text = "Histórico Completo do Projeto";
                    formHistorico.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar histórico completo: {ex.Message}", "Erro");
            }
        }
    }
}