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
        private Usuario _responsavelAtual;

        // Controles existentes no designer
        private Label Lbl_Titulo;
        private FlowLayoutPanel Pnl_Responsaveis;
        private Label Lbl_Ilustracao;
        private TextBox Txt_BuscaResponsavel;
        private Button Btn_Concluir;
        private ListView Lst_SugestoesResponsavel;

        public Frm_AtribuirResponsavelTarefa(Projeto_Tarefas tarefa)
        {
            _tarefa = tarefa;
            _usuarioLogado = Sessao.UsuarioLogado; // CORREÇÃO: Agora são do mesmo tipo

            // Inicializar as dependências corretamente
            var notificacoesDB = new NotificacoesDB();
            var projetosDB = new ProjetosDB();
            _usuarioDB = new UsuarioDB();
            _membrosDB = new MembrosDB(notificacoesDB, projetosDB, _usuarioDB);
            var alarmeDB = new AlarmeDB();
            var subtarefasDB = new SubtarefasDB();
            var comentariosDB = new ComentariosDB();

            _tarefasDB = new TarefasDB(notificacoesDB, projetosDB, _usuarioDB, _membrosDB, alarmeDB, subtarefasDB, comentariosDB);

            InitializeComponent();
            ConfigurarInterfaceResponsaveis();
            CarregarResponsavelExistente();
        }

        private void InitializeComponent()
        {
            this.Lbl_Titulo = new Label();
            this.Pnl_Responsaveis = new FlowLayoutPanel();
            this.Lbl_Ilustracao = new Label();
            this.Txt_BuscaResponsavel = new TextBox();
            this.Btn_Concluir = new Button();
            this.Lst_SugestoesResponsavel = new ListView();
            this.SuspendLayout();

            // Lbl_Titulo
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.Lbl_Titulo.Location = new Point(20, 20);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new Size(220, 21);
            this.Lbl_Titulo.TabIndex = 0;
            this.Lbl_Titulo.Text = "Responsável desta Tarefa:";

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
            this.Lbl_Ilustracao.TabIndex = 2;
            this.Lbl_Ilustracao.Text = "Digite para buscar membros do projeto:";

            // Txt_BuscaResponsavel
            this.Txt_BuscaResponsavel.Location = new Point(20, 175);
            this.Txt_BuscaResponsavel.Name = "Txt_BuscaResponsavel";
            this.Txt_BuscaResponsavel.Size = new Size(300, 20);
            this.Txt_BuscaResponsavel.TabIndex = 3;

            // Lst_SugestoesResponsavel
            this.Lst_SugestoesResponsavel.Location = new Point(20, 205);
            this.Lst_SugestoesResponsavel.Name = "Lst_SugestoesResponsavel";
            this.Lst_SugestoesResponsavel.Size = new Size(300, 100);
            this.Lst_SugestoesResponsavel.TabIndex = 4;
            this.Lst_SugestoesResponsavel.Visible = false;
            this.Lst_SugestoesResponsavel.View = View.Details;
            this.Lst_SugestoesResponsavel.FullRowSelect = true;
            this.Lst_SugestoesResponsavel.MultiSelect = false;
            this.Lst_SugestoesResponsavel.Columns.Add("Usuário", 120);
            this.Lst_SugestoesResponsavel.Columns.Add("Email", 160);

            // Btn_Concluir
            this.Btn_Concluir.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.Btn_Concluir.Location = new Point(350, 350);
            this.Btn_Concluir.Name = "Btn_Concluir";
            this.Btn_Concluir.Size = new Size(100, 30);
            this.Btn_Concluir.TabIndex = 5;
            this.Btn_Concluir.Text = "Concluir";
            this.Btn_Concluir.UseVisualStyleBackColor = true;

            // Form
            this.BackColor = Color.White;
            this.ClientSize = new Size(500, 400);
            this.Controls.Add(this.Lst_SugestoesResponsavel);
            this.Controls.Add(this.Btn_Concluir);
            this.Controls.Add(this.Txt_BuscaResponsavel);
            this.Controls.Add(this.Lbl_Ilustracao);
            this.Controls.Add(this.Pnl_Responsaveis);
            this.Controls.Add(this.Lbl_Titulo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_AtribuirResponsavelTarefa";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Atribuir Responsável";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ConfigurarInterfaceResponsaveis()
        {
            this.Text = $"Atribuir Responsável - Tarefa #{_tarefa.Codigo}";

            Lbl_Titulo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            Lbl_Titulo.ForeColor = Color.FromArgb(64, 64, 64);

            Pnl_Responsaveis.FlowDirection = FlowDirection.LeftToRight;
            Pnl_Responsaveis.WrapContents = true;
            Pnl_Responsaveis.BackColor = Color.FromArgb(250, 250, 250);

            Txt_BuscaResponsavel.Text = "Digite nome, usuário ou email...";
            Txt_BuscaResponsavel.ForeColor = Color.Gray;
            Txt_BuscaResponsavel.Font = new Font("Segoe UI", 9);

            // Configurar placeholder
            Txt_BuscaResponsavel.Enter += Txt_Responsavel_Enter_Placeholder;
            Txt_BuscaResponsavel.Leave += Txt_Responsavel_Leave_Placeholder;

            Btn_Concluir.BackColor = Color.FromArgb(74, 124, 255);
            Btn_Concluir.ForeColor = Color.White;
            Btn_Concluir.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            Lst_SugestoesResponsavel.Font = new Font("Segoe UI", 9);
            Lst_SugestoesResponsavel.BackColor = Color.White;
            Lst_SugestoesResponsavel.BorderStyle = BorderStyle.FixedSingle;

            // Configurar eventos
            Txt_BuscaResponsavel.TextChanged += Txt_Responsavel_TextChanged;
            Txt_BuscaResponsavel.KeyDown += Txt_Responsavel_KeyDown;
            Txt_BuscaResponsavel.Enter += Txt_Responsavel_Enter;
            Txt_BuscaResponsavel.Leave += Txt_Responsavel_Leave;
            Lst_SugestoesResponsavel.KeyDown += Lst_SugestoesResponsavel_KeyDown;
            Lst_SugestoesResponsavel.DoubleClick += Lst_SugestoesResponsavel_DoubleClick;
            Lst_SugestoesResponsavel.Leave += Lst_SugestoesResponsavel_Leave;
            Btn_Concluir.Click += Btn_Concluir_Click;
        }

        private void Txt_Responsavel_Enter_Placeholder(object sender, EventArgs e)
        {
            if (Txt_BuscaResponsavel.Text == "Digite nome, usuário ou email...")
            {
                Txt_BuscaResponsavel.Text = "";
                Txt_BuscaResponsavel.ForeColor = SystemColors.WindowText;
            }
        }

        private void Txt_Responsavel_Leave_Placeholder(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Txt_BuscaResponsavel.Text))
            {
                Txt_BuscaResponsavel.Text = "Digite nome, usuário ou email...";
                Txt_BuscaResponsavel.ForeColor = Color.Gray;
            }
        }

        private void Txt_Responsavel_TextChanged(object sender, EventArgs e)
        {
            string textoBusca = Txt_BuscaResponsavel.Text.Trim();

            if (string.IsNullOrEmpty(textoBusca) || textoBusca == "Digite nome, usuário ou email...")
            {
                Lst_SugestoesResponsavel.Visible = false;
                return;
            }

            try
            {
                var resultados = _usuarioDB.BuscarUsuariosPorTexto(textoBusca);

                // Filtrar apenas membros do projeto atual
                var membrosProjeto = _membrosDB.ObterMembrosProjeto(_tarefa.CodProjeto);
                var membrosIds = membrosProjeto.Select(m => m.Codigo).ToList();

                var resultadosFiltrados = resultados
                    .Where(u => membrosIds.Contains(u.Codigo))
                    .Where(u => _responsavelAtual == null || u.Codigo != _responsavelAtual.Codigo)
                    .ToList();

                Lst_SugestoesResponsavel.Items.Clear();

                foreach (var usuario in resultadosFiltrados)
                {
                    var item = new ListViewItem(usuario.NomeUsuario);
                    item.Tag = usuario;
                    item.SubItems.Add(usuario.Email);
                    Lst_SugestoesResponsavel.Items.Add(item);
                }

                Lst_SugestoesResponsavel.Visible = resultadosFiltrados.Any();
                Lst_SugestoesResponsavel.Location = new Point(Txt_BuscaResponsavel.Left, Txt_BuscaResponsavel.Bottom + 2);
                Lst_SugestoesResponsavel.Width = Txt_BuscaResponsavel.Width;
                Lst_SugestoesResponsavel.Height = Math.Min(resultadosFiltrados.Count * 20 + 10, 150);
            }
            catch (Exception ex)
            {
                Lst_SugestoesResponsavel.Visible = false;
                Console.WriteLine($"Erro ao buscar membros: {ex.Message}");
            }
        }

        private void Txt_Responsavel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (Lst_SugestoesResponsavel.Visible && Lst_SugestoesResponsavel.SelectedItems.Count > 0)
                {
                    Usuario usuarioSelecionado = (Usuario)Lst_SugestoesResponsavel.SelectedItems[0].Tag;
                    AdicionarResponsavel(usuarioSelecionado);
                }
                else if (!string.IsNullOrWhiteSpace(Txt_BuscaResponsavel.Text) &&
                         Txt_BuscaResponsavel.Text != "Digite nome, usuário ou email...")
                {
                    // Buscar diretamente pelo texto
                    var resultados = _usuarioDB.BuscarUsuariosPorTexto(Txt_BuscaResponsavel.Text.Trim());
                    var membrosProjeto = _membrosDB.ObterMembrosProjeto(_tarefa.CodProjeto);
                    var membrosIds = membrosProjeto.Select(m => m.Codigo).ToList();

                    var usuarioEncontrado = resultados
                        .FirstOrDefault(u => membrosIds.Contains(u.Codigo) &&
                                           (_responsavelAtual == null || u.Codigo != _responsavelAtual.Codigo));

                    if (usuarioEncontrado != null)
                    {
                        AdicionarResponsavel(usuarioEncontrado);
                    }
                    else
                    {
                        MessageBox.Show("Usuário não encontrado ou já é o responsável atual.", "Aviso",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Lst_SugestoesResponsavel.Visible = false;
                Txt_BuscaResponsavel.Clear();
            }
            else if (e.KeyCode == Keys.Down && Lst_SugestoesResponsavel.Visible && Lst_SugestoesResponsavel.Items.Count > 0)
            {
                Lst_SugestoesResponsavel.Focus();
                if (Lst_SugestoesResponsavel.SelectedItems.Count == 0)
                    Lst_SugestoesResponsavel.Items[0].Selected = true;
            }
        }

        private void Lst_SugestoesResponsavel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Lst_SugestoesResponsavel.SelectedItems.Count > 0)
            {
                Usuario usuarioSelecionado = (Usuario)Lst_SugestoesResponsavel.SelectedItems[0].Tag;
                AdicionarResponsavel(usuarioSelecionado);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Lst_SugestoesResponsavel.Visible = false;
                Txt_BuscaResponsavel.Focus();
                Txt_BuscaResponsavel.Clear();
            }
            else if (e.KeyCode == Keys.Up && Lst_SugestoesResponsavel.SelectedItems.Count > 0)
            {
                if (Lst_SugestoesResponsavel.SelectedItems[0].Index == 0)
                {
                    Txt_BuscaResponsavel.Focus();
                    Lst_SugestoesResponsavel.SelectedItems.Clear();
                }
            }
        }

        private void Lst_SugestoesResponsavel_DoubleClick(object sender, EventArgs e)
        {
            if (Lst_SugestoesResponsavel.SelectedItems.Count > 0)
            {
                Usuario usuarioSelecionado = (Usuario)Lst_SugestoesResponsavel.SelectedItems[0].Tag;
                AdicionarResponsavel(usuarioSelecionado);
            }
        }

        private void Txt_Responsavel_Enter(object sender, EventArgs e)
        {
            Lst_SugestoesResponsavel.Location = new Point(Txt_BuscaResponsavel.Left, Txt_BuscaResponsavel.Bottom + 2);
            Lst_SugestoesResponsavel.Width = Txt_BuscaResponsavel.Width;
        }

        private void Txt_Responsavel_Leave(object sender, EventArgs e)
        {
            var timer = new Timer { Interval = 150 };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                timer.Dispose();

                if (this.IsHandleCreated && !Lst_SugestoesResponsavel.Focused &&
                    !Lst_SugestoesResponsavel.ClientRectangle.Contains(Lst_SugestoesResponsavel.PointToClient(Cursor.Position)))
                {
                    this.Invoke(new Action(() => Lst_SugestoesResponsavel.Visible = false));
                }
            };
            timer.Start();
        }

        private void Lst_SugestoesResponsavel_Leave(object sender, EventArgs e)
        {
            var timer = new Timer { Interval = 150 };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                timer.Dispose();

                if (this.IsHandleCreated && !Txt_BuscaResponsavel.Focused &&
                    !Txt_BuscaResponsavel.ClientRectangle.Contains(Txt_BuscaResponsavel.PointToClient(Cursor.Position)))
                {
                    this.Invoke(new Action(() => Lst_SugestoesResponsavel.Visible = false));
                }
            };
            timer.Start();
        }

        private void AdicionarResponsavel(Usuario usuario)
        {
            if (usuario == null) return;

            try
            {
                _responsavelAtual = usuario;
                AtualizarFiguraResponsavel();

                if (!_tarefasDB.AtribuirTarefaUsuario(_tarefa.Codigo, usuario.Codigo))
                {
                    MessageBox.Show($"Erro ao atribuir tarefa: {_tarefasDB.Mensagem}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _responsavelAtual = null;
                    AtualizarFiguraResponsavel();
                    return;
                }

                _tarefa.CodUsuario = usuario.Codigo;

                Txt_BuscaResponsavel.Clear();
                Lst_SugestoesResponsavel.Visible = false;
                Txt_BuscaResponsavel.Focus();

                MessageBox.Show($"Responsável atribuído com sucesso: {usuario.Nome}", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar responsável: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoverResponsavel()
        {
            try
            {
                if (MessageBox.Show($"Deseja remover o responsável atual?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _responsavelAtual = null;
                    AtualizarFiguraResponsavel();

                    if (!_tarefasDB.AtribuirTarefaUsuario(_tarefa.Codigo, 0))
                    {
                        MessageBox.Show($"Erro ao remover responsável: {_tarefasDB.Mensagem}", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        _tarefa.CodUsuario = 0;
                        MessageBox.Show("Responsável removido com sucesso.", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao remover responsável: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarFiguraResponsavel()
        {
            Pnl_Responsaveis.Controls.Clear();

            if (_responsavelAtual != null)
            {
                AdicionarFiguraResponsavel(_responsavelAtual);
            }
            else
            {
                var lblSemResponsaveis = new Label
                {
                    Text = "Nenhum responsável atribuído",
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
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
                    Width = 60,
                    Height = 75,
                    Margin = new Padding(5),
                    Tag = usuario
                };

                var circulo = new Panel
                {
                    Width = 45,
                    Height = 45,
                    BackColor = ObterCorAleatoria(usuario.Codigo.ToString()),
                    Location = new Point(7, 0)
                };

                // Tornar o painel circular
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
                    Location = new Point(0, 48),
                    Width = 60,
                    Height = 15,
                    AutoSize = false
                };

                var btnRemover = new Button
                {
                    Text = "×",
                    Size = new Size(18, 18),
                    Location = new Point(37, -2),
                    Tag = usuario,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Color.Red,
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnRemover.FlatAppearance.BorderSize = 0;
                btnRemover.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 200, 200);
                btnRemover.Click += (s, e) => RemoverResponsavel();

                panel.Controls.Add(circulo);
                panel.Controls.Add(lblNome);
                panel.Controls.Add(btnRemover);

                // Tooltip com nome completo
                var toolTip = new ToolTip();
                toolTip.SetToolTip(panel, usuario.Nome);
                toolTip.SetToolTip(circulo, usuario.Nome);
                toolTip.SetToolTip(lblNome, usuario.Nome);
                toolTip.SetToolTip(btnRemover, "Remover responsável");

                Pnl_Responsaveis.Controls.Add(panel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar figura do responsável: {ex.Message}");
            }
        }

        private void CarregarResponsavelExistente()
        {
            try
            {
                _responsavelAtual = null;

                if (_tarefa.CodUsuario > 0)
                {
                    var usuarioResponsavel = _usuarioDB.ObterUsuarioPorCodigo(_tarefa.CodUsuario.Value); if (usuarioResponsavel != null)
                    {
                        _responsavelAtual = usuarioResponsavel;
                    }
                }

                AtualizarFiguraResponsavel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar responsável existente: {ex.Message}");
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

            return $"{partes[0]} {partes[partes.Length - 1][0]}.";
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
                Color.FromArgb(255, 100, 200),
                Color.FromArgb(100, 100, 100),
                Color.FromArgb(139, 69, 19),
                Color.FromArgb(75, 0, 130)
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