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
            var membrosDB = new MembrosDB(notificacoesDB, projetosDB, _usuarioDB);
            _tarefasDB = new TarefasDB(notificacoesDB, projetosDB, _usuarioDB, membrosDB, _alarmeDB, _subtarefasDB, _comentariosDB);

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
            Btn_SalvarDataAlarme.Click += Btn_SalvarDataAlarme_Click;

            Cbo_Repeticao.Items.AddRange(new string[] {
                "Nunca repetir", "Repetir todos os dias", "Repetir toda semana", "Repetir todo mês"
            });
            Cbo_Repeticao.SelectedIndex = 0;

            // Configurar valores padrão para os DateTimePicker
            Dtp_Prazo.Value = DateTime.Now.Date;
            Dtp_HoraAlarme.Value = DateTime.Now.Date.AddHours(9); // Hora padrão: 9:00

            // --- Seção Subtarefas ---
            Txt_NovaSubtarefa.KeyDown += Txt_NovaSubtarefa_KeyDown;

            // --- Seção Comentários (Chat) ---
            Btn_AbrirChat.Click += Btn_AbrirChat_Click;
            Btn_FecharChat.Click += Bin_FecharChat_Click;
            Btn_EnviarComentario.Click += Bin_EnviarComentario_Click;
            Txt_NovoComentarioChat.KeyDown += Txt_NovoComentarioChat_KeyDown;

            // --- Seção Atribuição ---
            Btn_AtribuirUsuario.Click += Btn_AtribuirUsuario_Click;
            Pct_Colaborador.Click += Pct_Colaborador_Click;

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
            CarregarAtribuicao();
        }

        // =========================================================================
        // A. PRAZO, LEMBRETE E ALARME
        // =========================================================================

        private void MostrarSelecaoDataAlarme()
        {
            // Sempre mostrar os controles de data e alarme
            Dtp_Prazo.Visible = true;
            Dtp_HoraAlarme.Visible = true;
            Cbo_Repeticao.Visible = true;
            Btn_SalvarDataAlarme.Visible = true;
        }

        private void CarregarPrazoAlarme()
        {
            var alarme = _alarmeDB.ObterAlarmePorTarefa(tarefaAtual.Codigo);

            if (alarme != null)
            {
                Dtp_Prazo.Value = alarme.Data;
                Lbl_DefinirDataLembrete.Text = "Prazo e Lembrete Definidos";

                // Usar o método existente para obter descrição do prazo
                Lbl_PrazoExtenso.Text = _alarmeDB.ObterDescricaoPrazo(alarme.Data);
                Lbl_PrazoExtenso.Visible = true;
                Btn_FecharData.Visible = true;

                // Carregar hora do alarme e repetição se existirem
                if (alarme.Hora != DateTime.MinValue)
                    Dtp_HoraAlarme.Value = alarme.Hora;

                Cbo_Repeticao.SelectedIndex = (int)alarme.Repeticao;
            }
            else
            {
                Lbl_DefinirDataLembrete.Text = "Definir Data e Lembrete";
                Lbl_PrazoExtenso.Visible = false;
                Btn_FecharData.Visible = false;
            }
        }

        private void Lbl_DefinirDataLembrete_Click(object sender, EventArgs e)
        {
            MostrarSelecaoDataAlarme();
            Lbl_DefinirDataLembrete.Text = "Ajustar Prazo e Alarme";
        }

        private void Btn_SalvarDataAlarme_Click(object sender, EventArgs e)
        {
            var repeticao = (RepeticaoAlarme)Cbo_Repeticao.SelectedIndex;
            if (_tarefasDB.DefinirPrazoELembreteTarefa(
                tarefaAtual.Codigo,
                usuarioLogado.Codigo,
                Dtp_Prazo.Value,
                Dtp_HoraAlarme.Value,
                repeticao))
            {
                MessageBox.Show("Prazo e alarme salvos com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CarregarPrazoAlarme();
            }
            else
            {
                MessageBox.Show(_tarefasDB.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Bin_FecharData_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja remover o Prazo e o Alarme?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_tarefasDB.ResetarConfiguracoesTarefa(tarefaAtual.Codigo))
                {
                    CarregarPrazoAlarme();
                    MessageBox.Show("Prazo e alarme removidos com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(_alarmeDB.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =========================================================================
        // B. SUBTAREFAS
        // =========================================================================

        private void CarregarSubtarefas()
        {
            Flw_Subtarefas.Controls.Clear();

            // Carregar subtarefas do banco
            var listaSubtarefas = _subtarefasDB.ObterSubtarefasPorTarefa(tarefaAtual.Codigo);

            foreach (var sub in listaSubtarefas)
            {
                AdicionarControleSubtarefa(sub);
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
            }
            else
            {
                MessageBox.Show(_subtarefasDB.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdicionarControleSubtarefa(Tarefas_SubTarefas sub)
        {
            Panel pnlSub = new Panel
            {
                Height = 35,
                Width = Flw_Subtarefas.Width - 20,
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
                sub.isConcluida = chk.Checked;
                if (!_subtarefasDB.AtualizarSubtarefa(sub))
                {
                    MessageBox.Show(_subtarefasDB.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            Button btnExcluir = new Button
            {
                Text = "×",
                Size = new Size(25, 25),
                Location = new Point(pnlSub.Width - 30, 5),
                Tag = sub.Codigo,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat
            };

            btnExcluir.FlatAppearance.BorderSize = 0;
            btnExcluir.Click += (s, e) =>
            {
                if (MessageBox.Show("Deseja excluir esta subtarefa?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
            };

            pnlSub.Controls.Add(chk);
            pnlSub.Controls.Add(btnExcluir);
            Flw_Subtarefas.Controls.Add(pnlSub);
        }

        // =========================================================================
        // C. COMENTÁRIOS (PREVIEW E CHAT)
        // =========================================================================

        private void CarregarComentarios()
        {
            // Carregar comentários do banco
            var comentarios = _comentariosDB.ObterComentariosPorTarefa(tarefaAtual.Codigo);
            Lbl_ContadorComentarios.Text = $"({comentarios.Count})";
        }

        private void AtualizarPreviewComentarios()
        {
            var comentarios = _comentariosDB.ObterComentariosPorTarefa(tarefaAtual.Codigo);
            int contagem = comentarios.Count;

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

        private void Btn_AbrirChat_Click(object sender, EventArgs e)
        {
            CarregarComentariosNoChat();
            Pnl_ChatComentarios.Visible = true;
            Pnl_ChatComentarios.BringToFront();
        }

        private void Bin_FecharChat_Click(object sender, EventArgs e)
        {
            Pnl_ChatComentarios.Visible = false;
        }

        private void CarregarComentariosNoChat()
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

        private void AdicionarControleComentario(Tarefas_Comentarios com)
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
            string dataFormatada = _comentariosDB.FormatarDataComentario(com.Data);

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

        // =========================================================================
        // D. ATRIBUIÇÃO DE USUÁRIO
        // =========================================================================

        private void CarregarAtribuicao()
        {
            if (!string.IsNullOrEmpty(tarefaAtual.CodUsuario))
            {
                var usuario = _usuarioDB.ObterUsuarioPorCodigo(tarefaAtual.CodUsuario);
                if (usuario != null)
                {
                    Pct_Colaborador.Text = _usuarioDB.ObterInicialUsuario(usuario);
                    Pct_Colaborador.BackColor = Color.LightBlue;
                    Pct_Colaborador.Tag = tarefaAtual.CodUsuario;

                    ToolTip tt = new ToolTip();
                    tt.SetToolTip(Pct_Colaborador, $"Atribuído a: {usuario.NomeUsuario}");

                    Pct_Colaborador.Visible = true;
                    Lbl_UsuarioAtribuido.Text = usuario.NomeUsuario;
                    Lbl_UsuarioAtribuido.Visible = true;
                    return;
                }
            }

            Pct_Colaborador.Visible = false;
            Lbl_UsuarioAtribuido.Text = "Não atribuído";
            Lbl_UsuarioAtribuido.Visible = true;
        }

        private void Btn_AtribuirUsuario_Click(object sender, EventArgs e)
        {
            // Abrir formulário para selecionar usuário
            var formSelecao = new Frm_SelecionarUsuario();
            if (formSelecao.ShowDialog() == DialogResult.OK)
            {
                string codUsuarioSelecionado = formSelecao.UsuarioSelecionado?.Codigo;
                if (!string.IsNullOrEmpty(codUsuarioSelecionado))
                {
                    if (_tarefasDB.AtribuirTarefaUsuario(tarefaAtual.Codigo, codUsuarioSelecionado))
                    {
                        tarefaAtual.CodUsuario = codUsuarioSelecionado;
                        CarregarAtribuicao();
                        MessageBox.Show("Usuário atribuído com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(_tarefasDB.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Pct_Colaborador_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tarefaAtual.CodUsuario)) return;

            if (!_usuarioDB.VerificarPermissaoVisualizacao(usuarioLogado.Codigo, tarefaAtual.CodUsuario))
            {
                MessageBox.Show("Você não tem permissão para visualizar as tarefas de outro colaborador.",
                    "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Abrir perfil do usuário ou lista de tarefas dele
            MessageBox.Show($"Abrir perfil do usuário: {Lbl_UsuarioAtribuido.Text}",
                "Visualizar Usuário", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Bin_FecharJanela_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Método auxiliar para adicionar botão de subtarefas
        private void Btn_AdicionarSubtarefa_Click(object sender, EventArgs e)
        {
            AdicionarNovaSubtarefa();
        }
    }
}