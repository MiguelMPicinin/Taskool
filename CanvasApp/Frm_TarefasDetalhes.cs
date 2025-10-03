using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CanvasApp.Forms
{
    public partial class Frm_TarefasDetalhes : Form
    {
        public Projeto_Tarefas tarefaAtual;
        private readonly Usuario usuarioLogado;
        private readonly TarefasDB _tarefasDB;
        private readonly AlarmeDB _alarmeDB;
        private readonly SubtarefasDB _subtarefasDB;
        private readonly ComentariosDB _comentariosDB;
        private readonly UsuarioDB _usuarioDB;
        private readonly MembrosDB _membrosDB;

        // Controles criados programaticamente
        private Button btnAdicionarSubtarefa;

        // REMOVIDO: Funcionalidade de responsáveis

        public Frm_TarefasDetalhes(Projeto_Tarefas tarefa)
        {
            InitializeComponent();
            this.tarefaAtual = tarefa;
            this.usuarioLogado = Sessao.UsuarioLogado;

            // Inicializar as classes de banco de dados
            _alarmeDB = new AlarmeDB();
            _subtarefasDB = new SubtarefasDB();
            _comentariosDB = new ComentariosDB();
            _usuarioDB = new UsuarioDB();

            // Inicializar TarefasDB com as dependências necessárias
            var notificacoesDB = new NotificacoesDB();
            var projetosDB = new ProjetosDB();
            _membrosDB = new MembrosDB(notificacoesDB, projetosDB, _usuarioDB);
            _tarefasDB = new TarefasDB(notificacoesDB, projetosDB, _usuarioDB, _membrosDB, _alarmeDB, _subtarefasDB, _comentariosDB);

            ConfigurarLayoutDetalhes();
            CarregarDadosTarefa();
        }

        private void ConfigurarLayoutDetalhes()
        {
            // Configuração dos eventos de clique/interação
            Btn_FecharJanela.Click += Bin_FecharJanela_Click;

            // --- Seção Data e Alarme ---
            Lbl_DefinirDataLembrete.Click += Lbl_DefinirDataLembrete_Click;
            Btn_FecharData.Click += Bin_FecharData_Click;

            // CONFIGURAR O BOTÃO DO DESIGNER - CORRIGIDO
            Btn_SalvarData.Click += Btn_SalvarData_Click;
            Btn_SalvarData.Visible = true;

            Cbo_Repeticao.Items.AddRange(new string[] {
                "Nunca repetir", "Repetir todos os dias", "Repetir toda semana", "Repetir todo mês"
            });
            Cbo_Repeticao.SelectedIndex = 0;

            // Configurar valores padrão para os DateTimePicker
            Dtp_Prazo.Value = DateTime.Now.Date;
            Dtp_HoraAlarme.Value = DateTime.Now.Date.AddHours(9); // Hora padrão: 9:00

            // --- Seção Subtarefas ---
            Txt_NovaSubtarefa.KeyDown += Txt_NovaSubtarefa_KeyDown;

            // Adicionar botão para adicionar subtarefa
            btnAdicionarSubtarefa = new Button
            {
                Text = "+",
                Size = new Size(30, 23),
                Location = new Point(Txt_NovaSubtarefa.Right + 5, Txt_NovaSubtarefa.Top),
                Name = "Btn_AdicionarSubtarefa"
            };
            btnAdicionarSubtarefa.Click += (s, e) => AdicionarNovaSubtarefa();
            if (Flw_Subtarefas.Parent != null)
            {
                Flw_Subtarefas.Parent.Controls.Add(btnAdicionarSubtarefa);
            }

            // --- Seção Comentários (Chat) ---
            Btn_AbrirChat.Click += Btn_AbrirChat_Click;
            Btn_FecharChat.Click += Bin_FecharChat_Click;
            Btn_EnviarComentario.Click += Bin_EnviarComentario_Click;
            Txt_NovoComentarioChat.KeyDown += Txt_NovoComentarioChat_KeyDown;

            // REMOVIDO: ConfigurarSelecaoResponsaveis();

            // Botão para atribuir responsáveis
            var btnAtribuirResponsavel = new Button
            {
                Text = "Atribuir Responsáveis",
                Location = new Point(20, 350),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(74, 124, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAtribuirResponsavel.Click += (s, e) =>
            {
                Frm_AtribuirResponsavelTarefa frmAtribuir = new Frm_AtribuirResponsavelTarefa(tarefaAtual);
                frmAtribuir.ShowDialog();
                // Recarregar dados se necessário
                CarregarDadosTarefa();
            };
            this.Controls.Add(btnAtribuirResponsavel);

            // Configuração do Painel de Chat (Oculto inicialmente)
            Pnl_ChatComentarios.Visible = false;
            Pnl_ChatComentarios.BringToFront();

            // Mostrar seção de data sempre
            MostrarSelecaoDataAlarme();
        }

        private void CarregarDadosTarefa()
        {
            Txt_TituloTarefa.Text = tarefaAtual.Descricao;
            CarregarPrazoAlarme();
            CarregarSubtarefas();
            CarregarComentarios();
            AtualizarPreviewComentarios();
        }

        // =========================================================================
        // A. PRAZO, LEMBRETE E ALARME - CORRIGIDO
        // =========================================================================

        private void MostrarSelecaoDataAlarme()
        {
            // Sempre mostrar os controles de data e alarme
            Dtp_Prazo.Visible = true;
            Dtp_HoraAlarme.Visible = true;
            Cbo_Repeticao.Visible = true;

            // Mostrar o botão do designer
            Btn_SalvarData.Visible = true;
        }

        private void CarregarPrazoAlarme()
        {
            try
            {
                var alarme = _alarmeDB.ObterAlarmePorTarefa(tarefaAtual.Codigo);

                if (alarme != null)
                {
                    // CORREÇÃO: Garantir que os valores são atualizados nos controles
                    Dtp_Prazo.Value = alarme.Data;

                    // CORREÇÃO: Tratamento robusto para valores de hora
                    if (alarme.Hora != DateTime.MinValue && alarme.Hora.Year > 1900)
                    {
                        Dtp_HoraAlarme.Value = alarme.Hora;
                    }
                    else
                    {
                        Dtp_HoraAlarme.Value = DateTime.Now.Date.AddHours(9); // Hora padrão
                    }

                    // CORREÇÃO: Garantir que o combobox está com o valor correto
                    if (Cbo_Repeticao.Items.Count > 0)
                    {
                        int indexRepeticao = (int)alarme.Repeticao;
                        if (indexRepeticao >= 0 && indexRepeticao < Cbo_Repeticao.Items.Count)
                        {
                            Cbo_Repeticao.SelectedIndex = indexRepeticao;
                        }
                        else
                        {
                            Cbo_Repeticao.SelectedIndex = 0;
                        }
                    }

                    Lbl_DefinirDataLembrete.Text = "Prazo e Lembrete Definidos";

                    // Usar o método existente para obter descrição do prazo
                    Lbl_PrazoExtenso.Text = _alarmeDB.ObterDescricaoPrazo(alarme.Data);
                    Lbl_PrazoExtenso.Visible = true;
                    Btn_FecharData.Visible = true;

                    Console.WriteLine($"Alarme carregado: Tarefa {tarefaAtual.Codigo}, Data: {alarme.Data:dd/MM/yyyy}, Hora: {alarme.Hora:HH:mm}, Repetição: {alarme.Repeticao}");
                }
                else
                {
                    Lbl_DefinirDataLembrete.Text = "Definir Data e Lembrete";
                    Lbl_PrazoExtenso.Visible = false;
                    Btn_FecharData.Visible = false;

                    // CORREÇÃO: Resetar para valores padrão quando não há alarme
                    Dtp_Prazo.Value = DateTime.Now.Date;
                    Dtp_HoraAlarme.Value = DateTime.Now.Date.AddHours(9);
                    if (Cbo_Repeticao.Items.Count > 0)
                        Cbo_Repeticao.SelectedIndex = 0;

                    Console.WriteLine($"Nenhum alarme encontrado para tarefa {tarefaAtual.Codigo}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar prazo e alarme: {ex.Message}");
                MessageBox.Show($"Erro ao carregar prazo: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Lbl_DefinirDataLembrete_Click(object sender, EventArgs e)
        {
            MostrarSelecaoDataAlarme();
            Lbl_DefinirDataLembrete.Text = "Ajustar Prazo e Alarme";
        }

        // MÉTODO CORRIGIDO - USANDO O BOTÃO DO DESIGNER
        private void Btn_SalvarData_Click(object sender, EventArgs e)
        {
            SalvarDataAlarme();
        }

        private void SalvarDataAlarme()
        {
            try
            {
                var repeticao = (RepeticaoAlarme)Cbo_Repeticao.SelectedIndex;

                // DEBUG: Verificar os valores antes de salvar
                Console.WriteLine($"Salvando alarme - Tarefa: {tarefaAtual.Codigo}, " +
                                 $"Data: {Dtp_Prazo.Value:dd/MM/yyyy}, " +
                                 $"Hora: {Dtp_HoraAlarme.Value:HH:mm}, " +
                                 $"Repetição: {repeticao}, " +
                                 $"Usuário: {usuarioLogado.Codigo}");

                // Verificar se os valores são válidos
                if (Dtp_Prazo.Value < DateTime.Today)
                {
                    MessageBox.Show("A data não pode ser anterior a hoje!", "Data Inválida",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // CORREÇÃO: Converter o código do usuário de string para int
                if (!int.TryParse(usuarioLogado.Codigo, out int codUsuarioInt))
                {
                    MessageBox.Show("Código de usuário inválido!", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Usar o método correto do AlarmeDB para salvar
                if (_alarmeDB.DefinirPrazoELembrete(
                    tarefaAtual.Codigo,
                    codUsuarioInt,
                    Dtp_Prazo.Value,
                    Dtp_HoraAlarme.Value,
                    repeticao))
                {
                    MessageBox.Show("Prazo e alarme salvos com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // CORREÇÃO: Atualizar imediatamente os controles com os valores salvos
                    CarregarPrazoAlarme();

                    // Debug para verificar se foi salvo
                    var alarmeVerificado = _alarmeDB.ObterAlarmePorTarefa(tarefaAtual.Codigo);
                    if (alarmeVerificado != null)
                    {
                        Console.WriteLine($"Alarme verificado após salvar: Data: {alarmeVerificado.Data:dd/MM/yyyy}, Hora: {alarmeVerificado.Hora:HH:mm}");

                        // CORREÇÃO: Forçar atualização visual dos controles
                        Dtp_Prazo.Value = alarmeVerificado.Data;
                        Dtp_HoraAlarme.Value = alarmeVerificado.Hora;
                        Cbo_Repeticao.SelectedIndex = (int)alarmeVerificado.Repeticao;
                    }
                    else
                    {
                        Console.WriteLine("ALARME NÃO ENCONTRADO APÓS SALVAR!");
                    }
                }
                else
                {
                    MessageBox.Show($"Erro ao salvar alarme: {_alarmeDB.Mensagem}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar alarme: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"ERRO DETALHADO ao salvar alarme: {ex}");
            }
        }

        private void Bin_FecharData_Click(object sender, EventArgs e)
        {
            RemoverDataAlarme();
        }

        private void RemoverDataAlarme()
        {
            if (MessageBox.Show("Deseja remover o Prazo e o Alarme?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_alarmeDB.ResetarConfiguracoesTarefa(tarefaAtual.Codigo))
                    {
                        CarregarPrazoAlarme();
                        MessageBox.Show("Prazo e alarme removidos com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Erro ao remover alarme: {_alarmeDB.Mensagem}", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao remover alarme: {ex.Message}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =========================================================================
        // B. SUBTAREFAS
        // =========================================================================

        private void CarregarSubtarefas()
        {
            try
            {
                Flw_Subtarefas.Controls.Clear();

                // Carregar subtarefas do banco
                var listaSubtarefas = _subtarefasDB.ObterSubtarefasPorTarefa(tarefaAtual.Codigo);

                foreach (var sub in listaSubtarefas)
                {
                    AdicionarControleSubtarefa(sub);
                }

                // Mostrar mensagem se não houver subtarefas
                if (!listaSubtarefas.Any())
                {
                    var lblSemSubtarefas = new Label
                    {
                        Text = "Nenhuma subtarefa adicionada",
                        Font = new Font("Segoe UI", 9, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill
                    };
                    Flw_Subtarefas.Controls.Add(lblSemSubtarefas);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar subtarefas: {ex.Message}");
                MessageBox.Show($"Erro ao carregar subtarefas: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Txt_NovaSubtarefa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(Txt_NovaSubtarefa.Text))
            {
                e.SuppressKeyPress = true;
                AdicionarNovaSubtarefa();
            }
        }

        private void AdicionarNovaSubtarefa()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Txt_NovaSubtarefa.Text))
                {
                    MessageBox.Show("Digite uma descrição para a subtarefa!", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var novaSub = new Tarefas_SubTarefas
                {
                    CodTarefa = tarefaAtual.Codigo,
                    Texto = Txt_NovaSubtarefa.Text.Trim(),
                    isConcluida = false
                };

                if (_subtarefasDB.InserirSubtarefa(novaSub))
                {
                    CarregarSubtarefas(); // Recarregar do banco
                    Txt_NovaSubtarefa.Clear();
                    Txt_NovaSubtarefa.Focus();
                }
                else
                {
                    MessageBox.Show(_subtarefasDB.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar subtarefa: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdicionarControleSubtarefa(Tarefas_SubTarefas sub)
        {
            try
            {
                Panel pnlSub = new Panel
                {
                    Height = 35,
                    Width = Flw_Subtarefas.Width - 25,
                    Tag = sub.Codigo,
                    Margin = new Padding(0, 3, 0, 3),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                CheckBox chk = new CheckBox
                {
                    Checked = sub.isConcluida,
                    Text = sub.Texto,
                    Location = new Point(8, 8),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9),
                    Tag = sub.Codigo
                };

                chk.CheckedChanged += (s, e) =>
                {
                    try
                    {
                        sub.isConcluida = chk.Checked;
                        if (!_subtarefasDB.AtualizarSubtarefa(sub))
                        {
                            MessageBox.Show(_subtarefasDB.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            // Reverter a mudança visual em caso de erro
                            chk.Checked = !chk.Checked;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao atualizar subtarefa: {ex.Message}", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                Button btnExcluir = new Button
                {
                    Text = "×",
                    Size = new Size(25, 25),
                    Location = new Point(pnlSub.Width - 35, 5),
                    Tag = sub.Codigo,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.Red,
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };

                btnExcluir.FlatAppearance.BorderSize = 0;
                btnExcluir.Click += (s, e) =>
                {
                    if (MessageBox.Show("Deseja excluir esta subtarefa?", "Confirmar",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            if (_subtarefasDB.ExcluirSubtarefa(sub.Codigo))
                            {
                                CarregarSubtarefas(); // Recarregar do banco
                            }
                            else
                            {
                                MessageBox.Show(_subtarefasDB.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao excluir subtarefa: {ex.Message}", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                };

                pnlSub.Controls.Add(chk);
                pnlSub.Controls.Add(btnExcluir);
                Flw_Subtarefas.Controls.Add(pnlSub);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar controle de subtarefa: {ex.Message}");
            }
        }

        // =========================================================================
        // C. COMENTÁRIOS (PREVIEW E CHAT)
        // =========================================================================

        private void CarregarComentarios()
        {
            try
            {
                // Carregar comentários do banco - método atualizado para usar dados reais
                var comentarios = _comentariosDB.ObterComentariosPorTarefa(tarefaAtual.Codigo);

                // Atualizar contador no botão de abrir chat
                Btn_AbrirChat.Text = $"Comentários ({comentarios.Count})";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar comentários: {ex.Message}");
            }
        }

        private void AtualizarPreviewComentarios()
        {
            try
            {
                var comentarios = _comentariosDB.ObterComentariosPorTarefa(tarefaAtual.Codigo);
                int contagem = comentarios.Count;

                // Atualizar botão com a contagem
                Btn_AbrirChat.Text = $"Comentários ({contagem})";

                if (contagem > 0)
                {
                    var ultimo = comentarios.OrderByDescending(c => c.Codigo).First();
                    var usuario = _usuarioDB.ObterUsuarioPorCodigo(ultimo.CodUsuario);
                    string nomeUsuario = usuario?.NomeUsuario ?? "Usuário";

                    string previewTexto = ultimo.Comentario.Length > 35 ?
                        ultimo.Comentario.Substring(0, 35) + "..." : ultimo.Comentario;

                    Lbl_PreviewComentarios.Text = $"{nomeUsuario}: {previewTexto}";
                }
                else
                {
                    Lbl_PreviewComentarios.Text = "Nenhum comentário ainda.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar preview de comentários: {ex.Message}");
                Lbl_PreviewComentarios.Text = "Erro ao carregar comentários.";
            }
        }

        private void Btn_AbrirChat_Click(object sender, EventArgs e)
        {
            try
            {
                CarregarComentariosNoChat();
                Pnl_ChatComentarios.Visible = true;
                Pnl_ChatComentarios.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir chat: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Bin_FecharChat_Click(object sender, EventArgs e)
        {
            Pnl_ChatComentarios.Visible = false;
        }

        private void CarregarComentariosNoChat()
        {
            try
            {
                Flw_ChatComentarios.Controls.Clear();

                var comentarios = _comentariosDB.ObterComentariosPorTarefa(tarefaAtual.Codigo);

                foreach (var com in comentarios.OrderBy(c => c.Codigo))
                {
                    AdicionarControleComentario(com);
                }

                // Rolagem automática para o final
                if (Flw_ChatComentarios.Controls.Count > 0)
                {
                    Flw_ChatComentarios.ScrollControlIntoView(
                        Flw_ChatComentarios.Controls[Flw_ChatComentarios.Controls.Count - 1]);
                }

                // Mostrar mensagem se não houver comentários
                if (!comentarios.Any())
                {
                    var lblSemComentarios = new Label
                    {
                        Text = "Nenhum comentário ainda. Seja o primeiro a comentar!",
                        Font = new Font("Segoe UI", 10, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill,
                        AutoSize = false,
                        Height = 50
                    };
                    Flw_ChatComentarios.Controls.Add(lblSemComentarios);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar comentários no chat: {ex.Message}");
                MessageBox.Show($"Erro ao carregar comentários: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Txt_NovoComentarioChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Control)
            {
                e.SuppressKeyPress = true;
                EnviarComentario();
            }
        }

        private void Bin_EnviarComentario_Click(object sender, EventArgs e)
        {
            EnviarComentario();
        }

        private void EnviarComentario()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Txt_NovoComentarioChat.Text))
                {
                    MessageBox.Show("Digite um comentário antes de enviar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var novoCom = new Tarefas_Comentarios
                {
                    CodTarefa = tarefaAtual.Codigo,
                    CodUsuario = usuarioLogado.Codigo,
                    Comentario = Txt_NovoComentarioChat.Text.Trim(),
                    Data = DateTime.Now
                };

                if (_comentariosDB.InserirComentario(novoCom))
                {
                    CarregarComentariosNoChat();
                    Txt_NovoComentarioChat.Clear();
                    AtualizarPreviewComentarios();
                    CarregarComentarios();
                }
                else
                {
                    MessageBox.Show(_comentariosDB.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar comentário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdicionarControleComentario(Tarefas_Comentarios com)
        {
            try
            {
                Panel pnlCom = new Panel
                {
                    Width = Flw_ChatComentarios.Width - 25,
                    Margin = new Padding(5),
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = com.Codigo,
                    BackColor = com.CodUsuario == usuarioLogado.Codigo ?
                        Color.LightCyan : Color.White
                };

                var usuario = _usuarioDB.ObterUsuarioPorCodigo(com.CodUsuario);
                string nomeUsuario = usuario?.NomeUsuario ?? "Usuário";
                string dataFormatada = com.Data.ToString("dd/MM/yyyy HH:mm");

                Label lblInicial = new Label
                {
                    Text = nomeUsuario.Substring(0, 1).ToUpper(),
                    Location = new Point(8, 8),
                    Size = new Size(25, 25),
                    BackColor = Color.LightBlue,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Color.White
                };

                Label lblHeader = new Label
                {
                    Text = $"{nomeUsuario} - {dataFormatada}",
                    Location = new Point(40, 8),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    ForeColor = Color.DarkGray
                };

                TextBox txtComentario = new TextBox
                {
                    Text = com.Comentario,
                    Location = new Point(40, 30),
                    Size = new Size(pnlCom.Width - 50, 0),
                    BorderStyle = BorderStyle.None,
                    Font = new Font("Segoe UI", 9),
                    BackColor = pnlCom.BackColor,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.None
                };

                // Ajustar altura do TextBox baseado no conteúdo
                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(txtComentario.Text, txtComentario.Font, txtComentario.Width);
                    txtComentario.Height = (int)Math.Ceiling(size.Height) + 10;
                }

                pnlCom.Height = txtComentario.Bottom + 10;

                pnlCom.Controls.Add(lblInicial);
                pnlCom.Controls.Add(lblHeader);
                pnlCom.Controls.Add(txtComentario);

                Flw_ChatComentarios.Controls.Add(pnlCom);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar controle de comentário: {ex.Message}");
            }
        }

        // =========================================================================
        // EVENTOS DO FORMULÁRIO
        // =========================================================================

        private void Bin_FecharJanela_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ValidarCamposData()
        {
            // Validar se a data não é anterior a hoje
            if (Dtp_Prazo.Value < DateTime.Today)
            {
                MessageBox.Show("A data não pode ser anterior a hoje!", "Data Inválida",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Dtp_Prazo.Value = DateTime.Today;
            }
        }

        private void Dtp_Prazo_ValueChanged(object sender, EventArgs e)
        {
            ValidarCamposData();
        }

        private void Frm_TarefasDetalhes_Load(object sender, EventArgs e)
        {
            // Configurar eventos adicionais
            Dtp_Prazo.ValueChanged += Dtp_Prazo_ValueChanged;

            // Centralizar formulário na tela
            this.StartPosition = FormStartPosition.CenterScreen;

            // Focar no campo de título
            Txt_TituloTarefa.Focus();
        }

        private void Frm_TarefasDetalhes_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Limpar recursos se necessário
            if (btnAdicionarSubtarefa != null)
            {
                btnAdicionarSubtarefa.Dispose();
            }
        }

        // Eventos vazios necessários do designer
        private void Txt_TituloTarefa_TextChanged(object sender, EventArgs e) { }
        private void Dtp_HoraAlarme_ValueChanged(object sender, EventArgs e) { }
        private void Cbo_Repeticao_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}