using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace CanvasApp.Forms
{
    public partial class Frm_AtribuirResponsavelTarefa : Form
    {
        private Projeto_Tarefas _tarefa;
        private Usuario _usuarioLogado;
        private TarefasDB _tarefasDB;
        private MembrosDB _membrosDB;
        private UsuarioDB _usuarioDB;
        private List<Usuario> _responsaveisAdicionados = new List<Usuario>();

        // Controles existentes no designer
        private Label Lbl_Titulo;
        private FlowLayoutPanel Pnl_Responsaveis;
        private Label Lbl_Ilustracao;
        private TextBox Lst_Sugestoes;
        private Button Btn_Concluir1;
        private ListBox Lst_SugestoesResponsavel;

        public Frm_AtribuirResponsavelTarefa(Projeto_Tarefas tarefa)
        {
            _tarefa = tarefa;
            _usuarioLogado = Sessao.UsuarioLogado;

            var notificacoesDB = new NotificacoesDB();
            var projetosDB = new ProjetosDB();
            _usuarioDB = new UsuarioDB();
            _membrosDB = new MembrosDB(notificacoesDB, projetosDB, _usuarioDB);
            _tarefasDB = new TarefasDB(notificacoesDB, projetosDB, _usuarioDB, _membrosDB,
                                      new AlarmeDB(), new SubtarefasDB(), new ComentariosDB());

            InitializeComponent();
            ConfigurarInterfaceResponsaveis();
            CarregarResponsaveisExistentes();
        }

        private void InitializeComponent()
        {
            this.Lbl_Titulo = new Label();
            this.Pnl_Responsaveis = new FlowLayoutPanel();
            this.Lbl_Ilustracao = new Label();
            this.Lst_Sugestoes = new TextBox();
            this.Btn_Concluir1 = new Button();
            this.SuspendLayout();

            // Lbl_Titulo
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new Font("Segoe UI", 12F);
            this.Lbl_Titulo.Location = new Point(20, 20);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new Size(193, 21);
            this.Lbl_Titulo.TabIndex = 0;
            this.Lbl_Titulo.Text = "Responsaveis desta Tarefa:";

            // Pnl_Responsaveis
            this.Pnl_Responsaveis.AutoScroll = true;
            this.Pnl_Responsaveis.BorderStyle = BorderStyle.FixedSingle;
            this.Pnl_Responsaveis.Location = new Point(20, 50);
            this.Pnl_Responsaveis.Name = "Pnl_Responsaveis";
            this.Pnl_Responsaveis.Size = new Size(450, 80);
            this.Pnl_Responsaveis.TabIndex = 1;

            // Lbl_Ilustracao
            this.Lbl_Ilustracao.AutoSize = true;
            this.Lbl_Ilustracao.Font = new Font("Segoe UI", 9F);
            this.Lbl_Ilustracao.Location = new Point(20, 150);
            this.Lbl_Ilustracao.Name = "Lbl_Ilustracao";
            this.Lbl_Ilustracao.Size = new Size(217, 15);
            this.Lbl_Ilustracao.TabIndex = 0;
            this.Lbl_Ilustracao.Text = "Digite para buscar membros do projeto:";

            // Lst_Sugestoes
            this.Lst_Sugestoes.Location = new Point(20, 175);
            this.Lst_Sugestoes.Name = "Lst_Sugestoes";
            this.Lst_Sugestoes.Size = new Size(300, 20);
            this.Lst_Sugestoes.TabIndex = 2;

            // Btn_Concluir1
            this.Btn_Concluir1.Font = new Font("Segoe UI", 9F);
            this.Btn_Concluir1.Location = new Point(350, 400);
            this.Btn_Concluir1.Name = "Btn_Concluir1";
            this.Btn_Concluir1.Size = new Size(100, 30);
            this.Btn_Concluir1.TabIndex = 3;
            this.Btn_Concluir1.Text = "Concluir";
            this.Btn_Concluir1.UseVisualStyleBackColor = true;

            // Form
            this.BackColor = Color.White;
            this.ClientSize = new Size(500, 500);
            this.Controls.Add(this.Btn_Concluir1);
            this.Controls.Add(this.Lst_Sugestoes);
            this.Controls.Add(this.Lbl_Ilustracao);
            this.Controls.Add(this.Pnl_Responsaveis);
            this.Controls.Add(this.Lbl_Titulo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_AtribuirResponsavelTarefa";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "5";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ConfigurarInterfaceResponsaveis()
        {
            this.Text = $"Atribuir Responsáveis - Tarefa #{_tarefa.Codigo}";

            Lbl_Titulo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            Lbl_Titulo.ForeColor = Color.FromArgb(64, 64, 64);

            Pnl_Responsaveis.FlowDirection = FlowDirection.LeftToRight;
            Pnl_Responsaveis.WrapContents = true;
            Pnl_Responsaveis.BackColor = Color.FromArgb(250, 250, 250);

            Lst_Sugestoes.Text = "Digite nome, usuário ou email...";
            Lst_Sugestoes.ForeColor = Color.Gray;
            Lst_Sugestoes.Font = new Font("Segoe UI", 9);

            Lst_Sugestoes.Enter += Txt_Responsavel_Enter_Placeholder;
            Lst_Sugestoes.Leave += Txt_Responsavel_Leave_Placeholder;

            Btn_Concluir1.BackColor = Color.FromArgb(74, 124, 255);
            Btn_Concluir1.ForeColor = Color.White;
            Btn_Concluir1.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Criar ListBox para sugestões
            Lst_SugestoesResponsavel = new ListBox
            {
                Location = new Point(20, 205),
                Size = new Size(300, 100),
                Visible = false,
                Font = new Font("Segoe UI", 9),
                DisplayMember = "Nome"
            };

            this.Controls.Add(Lst_SugestoesResponsavel);

            // Configurar eventos
            Lst_Sugestoes.TextChanged += Txt_Responsavel_TextChanged;
            Lst_Sugestoes.KeyDown += Txt_Responsavel_KeyDown;
            Lst_Sugestoes.Enter += Txt_Responsavel_Enter;
            Lst_Sugestoes.Leave += Txt_Responsavel_Leave;
            Lst_SugestoesResponsavel.KeyDown += Lst_SugestoesResponsavel_KeyDown;
            Lst_SugestoesResponsavel.DoubleClick += Lst_SugestoesResponsavel_DoubleClick;
            Btn_Concluir1.Click += Btn_Concluir_Click;
        }

        private void Txt_Responsavel_Enter_Placeholder(object sender, EventArgs e)
        {
            if (Lst_Sugestoes.Text == "Digite nome, usuário ou email...")
            {
                Lst_Sugestoes.Text = "";
                Lst_Sugestoes.ForeColor = SystemColors.WindowText;
            }
        }

        private void Txt_Responsavel_Leave_Placeholder(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Lst_Sugestoes.Text))
            {
                Lst_Sugestoes.Text = "Digite nome, usuário ou email...";
                Lst_Sugestoes.ForeColor = Color.Gray;
            }
        }

        private void Txt_Responsavel_TextChanged(object sender, EventArgs e)
        {
            string textoBusca = Lst_Sugestoes.Text.Trim();

            if (string.IsNullOrEmpty(textoBusca) || textoBusca == "Digite nome, usuário ou email...")
            {
                Lst_SugestoesResponsavel.Visible = false;
                return;
            }

            try
            {
                // ✅ CORREÇÃO: Converter int para string
                var membrosProjeto = _membrosDB.ObterMembrosProjeto(_tarefa.CodProjeto);

                var resultados = membrosProjeto
                    .Where(u => (u.Nome?.IndexOf(textoBusca, StringComparison.OrdinalIgnoreCase) >= 0) ||
                               (u.NomeUsuario?.IndexOf(textoBusca, StringComparison.OrdinalIgnoreCase) >= 0) ||
                               (u.Email?.IndexOf(textoBusca, StringComparison.OrdinalIgnoreCase) >= 0))
                    .Where(u => !_responsaveisAdicionados.Any(r => r.Codigo == u.Codigo))
                    .ToList();

                Lst_SugestoesResponsavel.Items.Clear();

                foreach (var usuario in resultados)
                {
                    Lst_SugestoesResponsavel.Items.Add(usuario);
                }

                Lst_SugestoesResponsavel.Visible = resultados.Any();
                Lst_SugestoesResponsavel.Location = new Point(Lst_Sugestoes.Left, Lst_Sugestoes.Bottom + 2);
                Lst_SugestoesResponsavel.Width = Lst_Sugestoes.Width;
            }
            catch
            {
                Lst_SugestoesResponsavel.Visible = false;
            }
        }

        private void Txt_Responsavel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (Lst_SugestoesResponsavel.Visible && Lst_SugestoesResponsavel.Items.Count > 0)
                {
                    if (Lst_SugestoesResponsavel.SelectedItem == null)
                        Lst_SugestoesResponsavel.SelectedIndex = 0;

                    AdicionarResponsavel(Lst_SugestoesResponsavel.SelectedItem as Usuario);
                }
                else if (!string.IsNullOrWhiteSpace(Lst_Sugestoes.Text) && Lst_Sugestoes.Text != "Digite nome, usuário ou email...")
                {
                    // ✅ CORREÇÃO: Converter int para string
                    var membrosProjeto = _membrosDB.ObterMembrosProjeto(_tarefa.CodProjeto);
                    var usuarioEncontrado = membrosProjeto
                        .FirstOrDefault(u => (u.Nome?.Equals(Lst_Sugestoes.Text.Trim(), StringComparison.OrdinalIgnoreCase) == true) ||
                                           (u.NomeUsuario?.Equals(Lst_Sugestoes.Text.Trim(), StringComparison.OrdinalIgnoreCase) == true) ||
                                           (u.Email?.Equals(Lst_Sugestoes.Text.Trim(), StringComparison.OrdinalIgnoreCase) == true));

                    if (usuarioEncontrado != null && !_responsaveisAdicionados.Any(r => r.Codigo == usuarioEncontrado.Codigo))
                    {
                        AdicionarResponsavel(usuarioEncontrado);
                    }
                    else
                    {
                        MessageBox.Show("Usuário não encontrado ou já adicionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Lst_SugestoesResponsavel.Visible = false;
                Lst_Sugestoes.Clear();
            }
            else if (e.KeyCode == Keys.Down && Lst_SugestoesResponsavel.Visible && Lst_SugestoesResponsavel.Items.Count > 0)
            {
                Lst_SugestoesResponsavel.Focus();
                if (Lst_SugestoesResponsavel.SelectedIndex == -1)
                    Lst_SugestoesResponsavel.SelectedIndex = 0;
            }
        }

        private void Lst_SugestoesResponsavel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Lst_SugestoesResponsavel.SelectedItem != null)
            {
                AdicionarResponsavel(Lst_SugestoesResponsavel.SelectedItem as Usuario);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Lst_SugestoesResponsavel.Visible = false;
                Lst_Sugestoes.Focus();
                Lst_Sugestoes.Clear();
            }
        }

        private void Lst_SugestoesResponsavel_DoubleClick(object sender, EventArgs e)
        {
            if (Lst_SugestoesResponsavel.SelectedItem != null)
            {
                AdicionarResponsavel(Lst_SugestoesResponsavel.SelectedItem as Usuario);
            }
        }

        private void Txt_Responsavel_Enter(object sender, EventArgs e)
        {
            Lst_SugestoesResponsavel.Location = new Point(Lst_Sugestoes.Left, Lst_Sugestoes.Bottom + 2);
            Lst_SugestoesResponsavel.Width = Lst_Sugestoes.Width;
        }

        private void Txt_Responsavel_Leave(object sender, EventArgs e)
        {
            System.Threading.Tasks.Task.Delay(150).ContinueWith(t =>
            {
                if (this.IsHandleCreated && !Lst_SugestoesResponsavel.Focused)
                {
                    this.Invoke(new Action(() => Lst_SugestoesResponsavel.Visible = false));
                }
            });
        }

        private void AdicionarResponsavel(Usuario usuario)
        {
            if (usuario == null) return;

            try
            {
                if (_responsaveisAdicionados.Any(r => r.Codigo == usuario.Codigo))
                {
                    MessageBox.Show("Este usuário já foi adicionado como responsável.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _responsaveisAdicionados.Add(usuario);
                AtualizarFigurasResponsaveis();

                // ✅ CORREÇÃO: Usar int.Parse para converter string para int
                if (!_tarefasDB.AtribuirTarefaUsuario(_tarefa.Codigo, Convert.ToInt32(usuario.Codigo)))
                {
                    MessageBox.Show($"Erro ao atribuir tarefa: {_tarefasDB.Mensagem}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _responsaveisAdicionados.Remove(usuario);
                    AtualizarFigurasResponsaveis();
                    return;
                }

                _tarefa.CodUsuario = Convert.ToInt32(usuario.Codigo);

                Lst_Sugestoes.Clear();
                Lst_SugestoesResponsavel.Visible = false;
                Lst_Sugestoes.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar responsável: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoverResponsavel(Usuario usuario)
        {
            try
            {
                if (MessageBox.Show($"Deseja remover {usuario.Nome} como responsável?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _responsaveisAdicionados.RemoveAll(r => r.Codigo == usuario.Codigo);
                    AtualizarFigurasResponsaveis();

                    if (_responsaveisAdicionados.Count == 0)
                    {
                        if (!_tarefasDB.AtribuirTarefaUsuario(_tarefa.Codigo, 0))
                        {
                            MessageBox.Show($"Erro ao remover responsável: {_tarefasDB.Mensagem}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            _tarefa.CodUsuario = 0;
                        }
                    }
                    else
                    {
                        var primeiroResponsavel = _responsaveisAdicionados.First();
                        if (!_tarefasDB.AtribuirTarefaUsuario(_tarefa.Codigo, Convert.ToInt32(primeiroResponsavel.Codigo)))
                        {
                            MessageBox.Show($"Erro ao atualizar responsável principal: {_tarefasDB.Mensagem}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            _tarefa.CodUsuario = Convert.ToInt32(primeiroResponsavel.Codigo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao remover responsável: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarFigurasResponsaveis()
        {
            Pnl_Responsaveis.Controls.Clear();

            foreach (var responsavel in _responsaveisAdicionados)
            {
                AdicionarFiguraResponsavel(responsavel);
            }

            if (!_responsaveisAdicionados.Any())
            {
                var lblSemResponsaveis = new Label
                {
                    Text = "Nenhum responsável atribuído",
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true
                };
                Pnl_Responsaveis.Controls.Add(lblSemResponsaveis);
            }
        }

        private void AdicionarFiguraResponsavel(Usuario usuario)
        {
            try
            {
                var panel = new Panel
                {
                    Width = 45,
                    Height = 60,
                    Margin = new Padding(5),
                    Tag = usuario
                };

                var circulo = new Panel
                {
                    Width = 40,
                    Height = 40,
                    BackColor = ObterCorAleatoria(usuario.Codigo.ToString()),
                    Location = new Point(2, 0)
                };

                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(0, 0, circulo.Width, circulo.Height);
                circulo.Region = new Region(path);

                var lblInicial = new Label
                {
                    Text = ObterInicialUsuario(usuario.Nome),
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent
                };
                circulo.Controls.Add(lblInicial);

                var lblNome = new Label
                {
                    Text = AbreviarNome(usuario.Nome),
                    Font = new Font("Segoe UI", 7),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.TopCenter,
                    Location = new Point(0, 42),
                    Width = 45,
                    Height = 15,
                    AutoSize = false
                };

                var btnRemover = new Button
                {
                    Text = "×",
                    Size = new Size(16, 16),
                    Location = new Point(27, -2),
                    Tag = usuario,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    ForeColor = Color.Red,
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnRemover.FlatAppearance.BorderSize = 0;
                btnRemover.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 200, 200);
                btnRemover.Click += (s, e) => RemoverResponsavel(usuario);

                panel.Controls.Add(circulo);
                panel.Controls.Add(lblNome);
                panel.Controls.Add(btnRemover);

                var toolTip = new ToolTip();
                toolTip.SetToolTip(panel, usuario.Nome);
                toolTip.SetToolTip(circulo, usuario.Nome);
                toolTip.SetToolTip(lblNome, usuario.Nome);

                Pnl_Responsaveis.Controls.Add(panel);
            }
            catch
            {
                // Ignorar erro
            }
        }

        private void CarregarResponsaveisExistentes()
        {
            try
            {
                _responsaveisAdicionados.Clear();

                if (_tarefa.CodUsuario > 0)
                {
                    // ✅ CORREÇÃO: Converter int para string
                    var usuarioResponsavel = _usuarioDB.ObterUsuarioPorCodigo(_tarefa.CodUsuario.ToString());
                    if (usuarioResponsavel != null)
                    {
                        _responsaveisAdicionados.Add(usuarioResponsavel);
                    }
                }

                AtualizarFigurasResponsaveis();
            }
            catch
            {
                // Ignorar erro
            }
        }

        private string ObterInicialUsuario(string nome)
        {
            if (string.IsNullOrEmpty(nome)) return "?";
            return nome.Substring(0, 1).ToUpper();
        }

        private string AbreviarNome(string nome)
        {
            if (string.IsNullOrEmpty(nome)) return "";

            var partes = nome.Split(' ');
            if (partes.Length == 1)
                return nome.Length > 6 ? nome.Substring(0, 6) + "..." : nome;

            return $"{partes[0]} {partes[1][0]}.";
        }

        private Color ObterCorAleatoria(string seed)
        {
            int hash = seed.GetHashCode();
            Random rnd = new Random(hash);

            Color[] cores = {
                Color.FromArgb(74, 124, 255),
                Color.FromArgb(255, 87, 87),
                Color.FromArgb(50, 200, 100),
                Color.FromArgb(255, 160, 0),
                Color.FromArgb(160, 90, 255),
                Color.FromArgb(0, 200, 200),
                Color.FromArgb(255, 100, 200)
            };

            return cores[Math.Abs(hash) % cores.Length];
        }

        private void Btn_Concluir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}