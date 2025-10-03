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
        private TextBox Lst_Sugestoes; // Este é o TextBox de busca
        private Button Btn_Concluir1;

        // ListBox para sugestões (será criada dinamicamente)
        private ListBox Lst_SugestoesResponsavel;

        public Frm_AtribuirResponsavelTarefa(Projeto_Tarefas tarefa)
        {
            _tarefa = tarefa;
            _usuarioLogado = Sessao.UsuarioLogado;

            // Inicializar DBs
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
            this.Lbl_Titulo = new System.Windows.Forms.Label();
            this.Pnl_Responsaveis = new System.Windows.Forms.FlowLayoutPanel();
            this.Lbl_Ilustracao = new System.Windows.Forms.Label();
            this.Lst_Sugestoes = new System.Windows.Forms.TextBox();
            this.Btn_Concluir1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Lbl_Titulo
            // 
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Titulo.Location = new System.Drawing.Point(20, 20);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new System.Drawing.Size(193, 21);
            this.Lbl_Titulo.TabIndex = 0;
            this.Lbl_Titulo.Text = "Responsaveis desta Tarefa:";
            // 
            // Pnl_Responsaveis
            // 
            this.Pnl_Responsaveis.AutoScroll = true;
            this.Pnl_Responsaveis.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Pnl_Responsaveis.Location = new System.Drawing.Point(20, 50);
            this.Pnl_Responsaveis.Name = "Pnl_Responsaveis";
            this.Pnl_Responsaveis.Size = new System.Drawing.Size(450, 80);
            this.Pnl_Responsaveis.TabIndex = 1;
            // 
            // Lbl_Ilustracao
            // 
            this.Lbl_Ilustracao.AutoSize = true;
            this.Lbl_Ilustracao.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Ilustracao.Location = new System.Drawing.Point(20, 150);
            this.Lbl_Ilustracao.Name = "Lbl_Ilustracao";
            this.Lbl_Ilustracao.Size = new System.Drawing.Size(217, 15);
            this.Lbl_Ilustracao.TabIndex = 0;
            this.Lbl_Ilustracao.Text = "Digite para buscar membros do projeto:";
            // 
            // Lst_Sugestoes
            // 
            this.Lst_Sugestoes.Location = new System.Drawing.Point(20, 175);
            this.Lst_Sugestoes.Name = "Lst_Sugestoes";
            this.Lst_Sugestoes.Size = new System.Drawing.Size(300, 20);
            this.Lst_Sugestoes.TabIndex = 2;
            // 
            // Btn_Concluir1
            // 
            this.Btn_Concluir1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Concluir1.Location = new System.Drawing.Point(350, 400);
            this.Btn_Concluir1.Name = "Btn_Concluir1";
            this.Btn_Concluir1.Size = new System.Drawing.Size(100, 30);
            this.Btn_Concluir1.TabIndex = 3;
            this.Btn_Concluir1.Text = "Concluir";
            this.Btn_Concluir1.UseVisualStyleBackColor = true;
            // 
            // Frm_AtribuirResponsavelTarefa
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(500, 500);
            this.Controls.Add(this.Btn_Concluir1);
            this.Controls.Add(this.Lst_Sugestoes);
            this.Controls.Add(this.Lbl_Ilustracao);
            this.Controls.Add(this.Pnl_Responsaveis);
            this.Controls.Add(this.Lbl_Titulo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_AtribuirResponsavelTarefa";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "5";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ConfigurarInterfaceResponsaveis()
        {
            // Configurar título do formulário
            this.Text = $"Atribuir Responsáveis - Tarefa #{_tarefa.Codigo}";

            // Configurar elementos existentes
            Lbl_Titulo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            Lbl_Titulo.ForeColor = Color.FromArgb(64, 64, 64);

            Pnl_Responsaveis.FlowDirection = FlowDirection.LeftToRight;
            Pnl_Responsaveis.WrapContents = true;
            Pnl_Responsaveis.BackColor = Color.FromArgb(250, 250, 250);

            // Configurar TextBox de busca (Lst_Sugestoes)
            Lst_Sugestoes.Text = "Digite nome, usuário ou email...";
            Lst_Sugestoes.ForeColor = Color.Gray;
            Lst_Sugestoes.Font = new Font("Segoe UI", 9);

            // Configurar placeholder manualmente
            Lst_Sugestoes.Enter += Txt_Responsavel_Enter_Placeholder;
            Lst_Sugestoes.Leave += Txt_Responsavel_Leave_Placeholder;

            // Configurar botão concluir
            Btn_Concluir1.BackColor = Color.FromArgb(74, 124, 255);
            Btn_Concluir1.ForeColor = Color.White;
            Btn_Concluir1.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Criar ListBox para sugestões (dinâmica)
            Lst_SugestoesResponsavel = new ListBox
            {
                Location = new Point(20, 205),
                Size = new Size(300, 100),
                Visible = false,
                Font = new Font("Segoe UI", 9),
                DisplayMember = "Nome"
            };

            // Adicionar ListBox ao formulário
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

        // Métodos para placeholder manual
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
                // Buscar apenas membros do projeto atual
                var membrosProjeto = _membrosDB.ObterMembrosProjeto(_tarefa.CodProjeto);

                // Filtrar membros pelo texto de busca e remover já adicionados
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

                // Reposicionar a lista abaixo do TextBox
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
                    // Se não tem item selecionado, seleciona o primeiro
                    if (Lst_SugestoesResponsavel.SelectedItem == null)
                        Lst_SugestoesResponsavel.SelectedIndex = 0;

                    AdicionarResponsavel(Lst_SugestoesResponsavel.SelectedItem as Usuario);
                }
                else if (!string.IsNullOrWhiteSpace(Lst_Sugestoes.Text) && Lst_Sugestoes.Text != "Digite nome, usuário ou email...")
                {
                    // Tentar encontrar usuário pelo texto digitado
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
                        MessageBox.Show("Usuário não encontrado ou já adicionado.", "Aviso",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            // Garantir que a lista está posicionada corretamente
            Lst_SugestoesResponsavel.Location = new Point(Lst_Sugestoes.Left, Lst_Sugestoes.Bottom + 2);
            Lst_SugestoesResponsavel.Width = Lst_Sugestoes.Width;
        }

        private void Txt_Responsavel_Leave(object sender, EventArgs e)
        {
            // Esconder a lista de sugestões quando o campo perde o foco (com delay para permitir clique na lista)
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
                // Verificar se o usuário já foi adicionado
                if (_responsaveisAdicionados.Any(r => r.Codigo == usuario.Codigo))
                {
                    MessageBox.Show("Este usuário já foi adicionado como responsável.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Adicionar à lista local
                _responsaveisAdicionados.Add(usuario);

                // Atualizar interface
                AtualizarFigurasResponsaveis();

                // Atualizar no banco de dados
                if (!_tarefasDB.AtribuirTarefaUsuario(_tarefa.Codigo, usuario.Codigo))
                {
                    MessageBox.Show($"Erro ao atribuir tarefa: {_tarefasDB.Mensagem}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Reverter na interface em caso de erro
                    _responsaveisAdicionados.Remove(usuario);
                    AtualizarFigurasResponsaveis();
                    return;
                }

                // Atualizar a tarefa atual
                _tarefa.CodUsuario = usuario.Codigo;

                // Limpar campo de pesquisa
                Lst_Sugestoes.Clear();
                Lst_SugestoesResponsavel.Visible = false;
                Lst_Sugestoes.Focus();

            }
            catch
            {
                MessageBox.Show("Erro ao adicionar responsável.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoverResponsavel(Usuario usuario)
        {
            try
            {
                if (MessageBox.Show($"Deseja remover {usuario.Nome} como responsável?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Remover da lista local
                    _responsaveisAdicionados.RemoveAll(r => r.Codigo == usuario.Codigo);

                    // Atualizar interface
                    AtualizarFigurasResponsaveis();

                    // Se era o único responsável, limpar no banco
                    if (_responsaveisAdicionados.Count == 0)
                    {
                        if (!_tarefasDB.AtribuirTarefaUsuario(_tarefa.Codigo, null))
                        {
                            MessageBox.Show($"Erro ao remover responsável: {_tarefasDB.Mensagem}", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            _tarefa.CodUsuario = null;
                        }
                    }
                    else
                    {
                        // Se havia múltiplos responsáveis, manter o primeiro como principal
                        var primeiroResponsavel = _responsaveisAdicionados.First();
                        if (!_tarefasDB.AtribuirTarefaUsuario(_tarefa.Codigo, primeiroResponsavel.Codigo))
                        {
                            MessageBox.Show($"Erro ao atualizar responsável principal: {_tarefasDB.Mensagem}", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            _tarefa.CodUsuario = primeiroResponsavel.Codigo;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Erro ao remover responsável.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarFigurasResponsaveis()
        {
            Pnl_Responsaveis.Controls.Clear();

            foreach (var responsavel in _responsaveisAdicionados)
            {
                AdicionarFiguraResponsavel(responsavel);
            }

            // Mostrar mensagem se não houver responsáveis
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

                // Criar círculo com a inicial
                var circulo = new Panel
                {
                    Width = 40,
                    Height = 40,
                    BackColor = ObterCorAleatoria(usuario.Codigo),
                    Location = new Point(2, 0)
                };

                // Deixar o círculo redondo
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(0, 0, circulo.Width, circulo.Height);
                circulo.Region = new Region(path);

                // Label com a inicial
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

                // Label com o nome (abreviado)
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

                // Botão de remover (X)
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

                // ToolTip com nome completo
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

                // Carregar responsável atual da tarefa
                if (!string.IsNullOrEmpty(_tarefa.CodUsuario))
                {
                    var usuarioResponsavel = _usuarioDB.ObterUsuarioPorCodigo(_tarefa.CodUsuario);
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

        // Métodos auxiliares para responsáveis
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
            // Usar o código do usuário como seed para cor consistente
            int hash = seed.GetHashCode();
            Random rnd = new Random(hash);

            Color[] cores = {
                Color.FromArgb(74, 124, 255),   // Azul
                Color.FromArgb(255, 87, 87),    // Vermelho
                Color.FromArgb(50, 200, 100),   // Verde
                Color.FromArgb(255, 160, 0),    // Laranja
                Color.FromArgb(160, 90, 255),   // Roxo
                Color.FromArgb(0, 200, 200),    // Ciano
                Color.FromArgb(255, 100, 200)   // Rosa
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
            if (disposing)
            {
                // Dispose de componentes se necessário
            }
            base.Dispose(disposing);
        }
    }
}