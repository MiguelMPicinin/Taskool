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
       
        private Button btnAdicionarSubtarefa;

        public Frm_TarefasDetalhes(Projeto_Tarefas tarefa)
        {
            InitializeComponent();
            this.tarefaAtual = tarefa;
            this.usuarioLogado = Sessao.UsuarioLogado;

            // ✅ CORREÇÃO: Inicializar os campos readonly diretamente no construtor
            _alarmeDB = new AlarmeDB();
            _subtarefasDB = new SubtarefasDB();
            _comentariosDB = new ComentariosDB();
            _usuarioDB = new UsuarioDB();

            var notificacoesDB = new NotificacoesDB();
            var projetosDB = new ProjetosDB();
            _membrosDB = new MembrosDB(notificacoesDB, projetosDB, _usuarioDB);
            _tarefasDB = new TarefasDB(notificacoesDB, projetosDB, _usuarioDB, _membrosDB, _alarmeDB, _subtarefasDB, _comentariosDB);

            ConfigurarLayoutDetalhes();
            CarregarDadosTarefa();
        }

        private void ConfigurarLayoutDetalhes()
        {
            ConfigurarEventos();
            ConfigurarComboBoxRepeticao();
            ConfigurarDateTimePickers();
            ConfigurarSubtarefas();
            ConfigurarComentarios();
            ConfigurarBotaoAtribuirResponsavel();

            Pnl_ChatComentarios.Visible = false;
            Pnl_ChatComentarios.BringToFront();
            MostrarSelecaoDataAlarme();
        }

        private void ConfigurarEventos()
        {
            Btn_FecharJanela.Click += Bin_FecharJanela_Click;
            Lbl_DefinirDataLembrete.Click += Lbl_DefinirDataLembrete_Click;
            Btn_FecharData.Click += Bin_FecharData_Click;
            Btn_SalvarData.Click += Btn_SalvarData_Click;
            Btn_AbrirChat.Click += Btn_AbrirChat_Click;
            Btn_FecharChat.Click += Bin_FecharChat_Click;
            Btn_EnviarComentario.Click += Bin_EnviarComentario_Click;
            Txt_NovoComentarioChat.KeyDown += Txt_NovoComentarioChat_KeyDown;
            Dtp_Prazo.ValueChanged += Dtp_Prazo_ValueChanged;
        }

        private void ConfigurarComboBoxRepeticao()
        {
            Cbo_Repeticao.Items.AddRange(new string[] {
                "Nunca repetir (apenas alarmar na data de término)",
                "Repetir todos os dias (até chegar a data de término)",
                "Repetir toda semana (até chegar a data de término, repetir toda semana, na segunda-feira)",
                "Repetir todo mês (até chegar a data de término, repetir todo mês, no primeiro dia útil do mês)"
            });
            Cbo_Repeticao.SelectedIndex = 0;
        }

        private void ConfigurarDateTimePickers()
        {
            Dtp_Prazo.Value = DateTime.Now.Date;
            Dtp_HoraAlarme.Value = DateTime.Now.Date.AddHours(9);
        }

        private void ConfigurarSubtarefas()
        {
            Txt_NovaSubtarefa.KeyDown += Txt_NovaSubtarefa_KeyDown;

            btnAdicionarSubtarefa = CriarBotaoSubtarefa();
            if (Flw_Subtarefas.Parent != null)
            {
                Flw_Subtarefas.Parent.Controls.Add(btnAdicionarSubtarefa);
            }
        }

        private Button CriarBotaoSubtarefa()
        {
            return new Button
            {
                Text = "+",
                Size = new Size(30, 23),
                Location = new Point(Txt_NovaSubtarefa.Right + 5, Txt_NovaSubtarefa.Top),
                Name = "Btn_AdicionarSubtarefa"
            };
        }

        private void ConfigurarComentarios()
        {
            // Configuração básica já feita em ConfigurarEventos()
        }

        private void ConfigurarBotaoAtribuirResponsavel()
        {
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
                using (var frmAtribuir = new Frm_AtribuirResponsavelTarefa(tarefaAtual))
                {
                    frmAtribuir.ShowDialog();
                    CarregarDadosTarefa();
                }
            };

            this.Controls.Add(btnAtribuirResponsavel);
        }

        private void CarregarDadosTarefa()
        {
            Txt_TituloTarefa.Text = tarefaAtual.Descricao;

            // NOVO: Carregar dataLimite se existir
            if (tarefaAtual.dataLimite != DateTime.MinValue && tarefaAtual.dataLimite >= new DateTime(1753, 1, 1))
            {
                // Você pode adicionar um DateTimePicker no Frm_TarefasDetalhes também se quiser
                Console.WriteLine($"Data limite da tarefa: {tarefaAtual.dataLimite:dd/MM/yyyy}");
            }

            CarregarPrazoAlarme();
            CarregarSubtarefas();
            AtualizarPreviewComentarios();
        }

        private void MostrarSelecaoDataAlarme()
        {
            var controles = new Control[] { Dtp_Prazo, Dtp_HoraAlarme, Cbo_Repeticao, Btn_SalvarData };

            foreach (var controle in controles)
            {
                controle.Visible = true;
                controle.BringToFront();
            }
        }

        private void CarregarPrazoAlarme()
        {
            try
            {
                var alarme = _alarmeDB.ObterAlarmePorTarefa(tarefaAtual.Codigo);

                if (alarme != null)
                {
                    ConfigurarControlesComAlarme(alarme);
                }
                else
                {
                    ConfigurarControlesSemAlarme();
                }
            }
            catch (Exception ex)
            {
                TratarErro("carregar prazo e alarme", ex);
            }
        }

        private void ConfigurarControlesComAlarme(Alarme alarme)
        {
            Dtp_Prazo.Value = alarme.Data;
            Dtp_HoraAlarme.Value = DateTime.Today.Add(alarme.Hora);

            if (Cbo_Repeticao.Items.Count > 0)
            {
                int indexRepeticao = (int)alarme.Repeticao;
                Cbo_Repeticao.SelectedIndex = (indexRepeticao >= 0 && indexRepeticao < Cbo_Repeticao.Items.Count)
                    ? indexRepeticao : 0;
            }

            Lbl_DefinirDataLembrete.Text = "Prazo e Lembrete Definidos";
            Lbl_PrazoExtenso.Text = _alarmeDB.ObterDescricaoPrazo(alarme.Data);
            Lbl_PrazoExtenso.Visible = true;
            Btn_FecharData.Visible = true;
        }

        private void ConfigurarControlesSemAlarme()
        {
            Lbl_DefinirDataLembrete.Text = "Definir Data e Lembrete";
            Lbl_PrazoExtenso.Visible = false;
            Btn_FecharData.Visible = false;

            ConfigurarDateTimePickers();
            if (Cbo_Repeticao.Items.Count > 0)
                Cbo_Repeticao.SelectedIndex = 0;
        }

        private void Lbl_DefinirDataLembrete_Click(object sender, EventArgs e)
        {
            MostrarSelecaoDataAlarme();
            Lbl_DefinirDataLembrete.Text = "Ajustar Prazo e Alarme";
        }

        private void Btn_SalvarData_Click(object sender, EventArgs e)
        {
            SalvarDataAlarme();
        }

        private void SalvarDataAlarme()
        {
            try
            {
                if (!ValidarDataAlarme()) return;

                var repeticao = (RepeticaoAlarme)Cbo_Repeticao.SelectedIndex;

                if (!int.TryParse(usuarioLogado.Codigo.ToString(), out int codUsuarioInt))
                {
                    MessageBox.Show("Erro: Código do usuário inválido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_alarmeDB.DefinirPrazoELembrete(
                    tarefaAtual.Codigo,
                    codUsuarioInt,
                    Dtp_Prazo.Value.Date,
                    Dtp_HoraAlarme.Value.TimeOfDay,
                    repeticao))
                {
                    MessageBox.Show("Prazo e alarme salvos com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarPrazoAlarme();
                }
                else
                {
                    MessageBox.Show($"Erro ao salvar alarme: {_alarmeDB.Mensagem}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                TratarErro("salvar alarme", ex);
            }
        }

        private bool ValidarDataAlarme()
        {
            if (Dtp_Prazo.Value < DateTime.Today)
            {
                MessageBox.Show("A data não pode ser anterior a hoje!", "Data Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void Bin_FecharData_Click(object sender, EventArgs e)
        {
            RemoverDataAlarme();
        }

        private void RemoverDataAlarme()
        {
            if (MessageBox.Show("Deseja remover o Prazo e o Alarme?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_alarmeDB.ResetarConfiguracoesTarefa(tarefaAtual.Codigo))
                    {
                        CarregarPrazoAlarme();
                        MessageBox.Show("Prazo e alarme removidos com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Erro ao remover alarme: {_alarmeDB.Mensagem}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    TratarErro("remover alarme", ex);
                }
            }
        }

        private void CarregarSubtarefas()
        {
            try
            {
                Flw_Subtarefas.Controls.Clear();

                var listaSubtarefas = _subtarefasDB.ObterSubtarefasPorTarefa(tarefaAtual.Codigo);

                foreach (var sub in listaSubtarefas)
                {
                    AdicionarControleSubtarefa(sub);
                }

                if (!listaSubtarefas.Any())
                {
                    AdicionarLabelSemItens(Flw_Subtarefas, "Nenhuma subtarefa adicionada");
                }
            }
            catch (Exception ex)
            {
                TratarErro("carregar subtarefas", ex);
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
                    MessageBox.Show("Digite uma descrição para a subtarefa!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    CarregarSubtarefas();
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
                TratarErro("adicionar subtarefa", ex);
            }
        }

        private void AdicionarControleSubtarefa(Tarefas_SubTarefas sub)
        {
            try
            {
                var pnlSub = CriarPanelSubtarefa(sub);
                var chk = CriarCheckboxSubtarefa(sub);
                var btnExcluir = CriarBotaoExcluirSubtarefa(sub);

                pnlSub.Controls.Add(chk);
                pnlSub.Controls.Add(btnExcluir);
                Flw_Subtarefas.Controls.Add(pnlSub);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar controle de subtarefa: {ex.Message}");
            }
        }

        private Panel CriarPanelSubtarefa(Tarefas_SubTarefas sub)
        {
            return new Panel
            {
                Height = 35,
                Width = Flw_Subtarefas.Width - 25,
                Tag = sub.Codigo,
                Margin = new Padding(0, 3, 0, 3),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private CheckBox CriarCheckboxSubtarefa(Tarefas_SubTarefas sub)
        {
            var chk = new CheckBox
            {
                Checked = sub.isConcluida,
                Text = sub.Texto,
                Location = new Point(8, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                Tag = sub.Codigo
            };

            chk.CheckedChanged += (s, e) => AtualizarStatusSubtarefa(sub, chk);
            return chk;
        }

        private void AtualizarStatusSubtarefa(Tarefas_SubTarefas sub, CheckBox chk)
        {
            try
            {
                sub.isConcluida = chk.Checked;
                if (!_subtarefasDB.AtualizarSubtarefa(sub))
                {
                    MessageBox.Show(_subtarefasDB.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    chk.Checked = !chk.Checked;
                }
            }
            catch (Exception ex)
            {
                TratarErro("atualizar subtarefa", ex);
            }
        }

        private Button CriarBotaoExcluirSubtarefa(Tarefas_SubTarefas sub)
        {
            var btnExcluir = new Button
            {
                Text = "×",
                Size = new Size(25, 25),
                Location = new Point(Flw_Subtarefas.Width - 60, 5),
                Tag = sub.Codigo,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnExcluir.FlatAppearance.BorderSize = 0;
            btnExcluir.Click += (s, e) => ExcluirSubtarefa(sub);
            return btnExcluir;
        }

        private void ExcluirSubtarefa(Tarefas_SubTarefas sub)
        {
            if (MessageBox.Show("Deseja excluir esta subtarefa?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_subtarefasDB.ExcluirSubtarefa(sub.Codigo))
                    {
                        CarregarSubtarefas();
                    }
                    else
                    {
                        MessageBox.Show(_subtarefasDB.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    TratarErro("excluir subtarefa", ex);
                }
            }
        }

        private void AtualizarPreviewComentarios()
        {
            try
            {
                var comentarios = _comentariosDB.ObterComentariosPorTarefa(tarefaAtual.Codigo);
                int contagem = comentarios.Count;

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
                TratarErro("abrir chat", ex);
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

                if (Flw_ChatComentarios.Controls.Count > 0)
                {
                    Flw_ChatComentarios.ScrollControlIntoView(
                        Flw_ChatComentarios.Controls[Flw_ChatComentarios.Controls.Count - 1]);
                }

                if (!comentarios.Any())
                {
                    AdicionarLabelSemItens(Flw_ChatComentarios, "Nenhum comentário ainda. Seja o primeiro a comentar!");
                }
            }
            catch (Exception ex)
            {
                TratarErro("carregar comentários no chat", ex);
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
                    MessageBox.Show("Digite um comentário antes de enviar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                }
                else
                {
                    MessageBox.Show(_comentariosDB.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                TratarErro("enviar comentário", ex);
            }
        }

        private void AdicionarControleComentario(Tarefas_Comentarios com)
        {
            try
            {
                var pnlCom = CriarPanelComentario(com);
                var lblInicial = CriarLabelInicialComentario(com);
                var lblHeader = CriarLabelHeaderComentario(com);
                var txtComentario = CriarTextBoxComentario(com, pnlCom);

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

        private Panel CriarPanelComentario(Tarefas_Comentarios com)
        {
            return new Panel
            {
                Width = Flw_ChatComentarios.Width - 25,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                Tag = com.Codigo,
                BackColor = com.CodUsuario == usuarioLogado.Codigo ? Color.LightCyan : Color.White
            };
        }

        private Label CriarLabelInicialComentario(Tarefas_Comentarios com)
        {
            var usuario = _usuarioDB.ObterUsuarioPorCodigo(com.CodUsuario);
            string nomeUsuario = usuario?.NomeUsuario ?? "Usuário";

            return new Label
            {
                Text = nomeUsuario.Substring(0, 1).ToUpper(),
                Location = new Point(8, 8),
                Size = new Size(25, 25),
                BackColor = Color.LightBlue,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White
            };
        }

        private Label CriarLabelHeaderComentario(Tarefas_Comentarios com)
        {
            var usuario = _usuarioDB.ObterUsuarioPorCodigo(com.CodUsuario);
            string nomeUsuario = usuario?.NomeUsuario ?? "Usuário";
            string dataFormatada = com.Data.ToString("dd/MM/yyyy HH:mm");

            return new Label
            {
                Text = $"{nomeUsuario} - {dataFormatada}",
                Location = new Point(40, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.DarkGray
            };
        }

        private TextBox CriarTextBoxComentario(Tarefas_Comentarios com, Panel pnlCom)
        {
            var txtComentario = new TextBox
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

            using (Graphics g = CreateGraphics())
            {
                SizeF size = g.MeasureString(txtComentario.Text, txtComentario.Font, txtComentario.Width);
                txtComentario.Height = (int)Math.Ceiling(size.Height) + 10;
            }

            pnlCom.Height = txtComentario.Bottom + 10;
            return txtComentario;
        }

        private void AdicionarLabelSemItens(FlowLayoutPanel panel, string texto)
        {
            var label = new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                AutoSize = false,
                Height = 50
            };
            panel.Controls.Add(label);
        }

        private void TratarErro(string operacao, Exception ex)
        {
            Console.WriteLine($"Erro ao {operacao}: {ex.Message}");
            MessageBox.Show($"Erro ao {operacao}: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Bin_FecharJanela_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ValidarCamposData()
        {
            if (Dtp_Prazo.Value < DateTime.Today)
            {
                MessageBox.Show("A data não pode ser anterior a hoje!", "Data Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Dtp_Prazo.Value = DateTime.Today;
            }
        }

        private void Dtp_Prazo_ValueChanged(object sender, EventArgs e)
        {
            ValidarCamposData();
        }

        private void Frm_TarefasDetalhes_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            Txt_TituloTarefa.Focus();
        }

        private void Frm_TarefasDetalhes_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnAdicionarSubtarefa?.Dispose();
        }

        // Eventos vazios necessários do designer
        private void Txt_TituloTarefa_TextChanged(object sender, EventArgs e) { }
        private void Dtp_HoraAlarme_ValueChanged(object sender, EventArgs e) { }
        private void Cbo_Repeticao_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}