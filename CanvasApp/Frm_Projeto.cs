using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using CanvasApp.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CanvasApp
{
    public partial class Frm_Projeto : Form
    {
        private Projetos projetoSelecionado;
        private List<Projeto_Tarefas> tarefasPendentes;
        private List<Projeto_Tarefas> tarefasConcluidas;
        private bool isModoFavoritos = false;
        private bool isExibindoConcluidas = false;
        private int usuarioLogadoId;

        // Instâncias das dependências necessárias
        private readonly NotificacoesDB dbNotificacoes = new NotificacoesDB();
        private readonly ProjetosDB dbProjetos = new ProjetosDB();
        private readonly UsuarioDB dbUsuario = new UsuarioDB();
        private readonly AlarmeDB dbAlarme = new AlarmeDB();
        private readonly SubtarefasDB dbSubtarefas = new SubtarefasDB();
        private readonly ComentariosDB dbComentarios = new ComentariosDB();

        // Agora inicializamos as DBs com as dependências
        private readonly MembrosDB dbMembros;
        private readonly TarefasDB dbTarefas;
        private readonly FavoritosDB dbFavoritos = new FavoritosDB();

        // Painel de notificações
        private Panel Pnl_Notificacoes;
        private bool notificacoesVisiveis = false;

        public Frm_Projeto(Projetos projeto, int usuarioId)
        {
            // Inicializar as DBs que dependem de outras
            dbMembros = new MembrosDB(dbNotificacoes, dbProjetos, dbUsuario);
            dbTarefas = new TarefasDB(dbNotificacoes, dbProjetos, dbUsuario, dbMembros, dbAlarme, dbSubtarefas, dbComentarios);

            InitializeComponent();
            InitializeProjeto(projeto, usuarioId);

            // Configurar o menu lateral e notificações
            ConfigurarMenuLateral();
            ConfigurarPainelNotificacoes();
        }

        public Frm_Projeto(List<Projeto_Tarefas> favoritos, int usuarioId)
        {
            // Inicializar as DBs que dependem de outras
            dbMembros = new MembrosDB(dbNotificacoes, dbProjetos, dbUsuario);
            dbTarefas = new TarefasDB(dbNotificacoes, dbProjetos, dbUsuario, dbMembros, dbAlarme, dbSubtarefas, dbComentarios);

            InitializeComponent();
            InitializeFavoritos(favoritos, usuarioId);

            // Configurar o menu lateral e notificações
            ConfigurarMenuLateral();
            ConfigurarPainelNotificacoes();
        }

        private void ConfigurarPainelNotificacoes()
        {
            // Criar o painel de notificações
            Pnl_Notificacoes = new Panel();
            Pnl_Notificacoes.Size = new Size(400, 500);
            Pnl_Notificacoes.BackColor = Color.White;
            Pnl_Notificacoes.BorderStyle = BorderStyle.FixedSingle;
            Pnl_Notificacoes.Visible = false;
            Pnl_Notificacoes.AutoScroll = true;
            Pnl_Notificacoes.Padding = new Padding(10);

            // Adicionar título
            Label lblTituloNotificacoes = new Label();
            lblTituloNotificacoes.Text = "NOTIFICAÇÕES";
            lblTituloNotificacoes.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTituloNotificacoes.ForeColor = Color.FromArgb(64, 64, 64);
            lblTituloNotificacoes.Size = new Size(380, 30);
            lblTituloNotificacoes.TextAlign = ContentAlignment.MiddleCenter;
            Pnl_Notificacoes.Controls.Add(lblTituloNotificacoes);

            // Botão fechar
            Button btnFechar = new Button();
            btnFechar.Text = "✕";
            btnFechar.Size = new Size(30, 30);
            btnFechar.Location = new Point(360, 5);
            btnFechar.FlatStyle = FlatStyle.Flat;
            btnFechar.ForeColor = Color.Red;
            btnFechar.Font = new Font("Arial", 12, FontStyle.Bold);
            btnFechar.Click += (s, e) => ToggleNotificacoes();
            Pnl_Notificacoes.Controls.Add(btnFechar);

            // Adicionar o painel ao formulário
            this.Controls.Add(Pnl_Notificacoes);
            Pnl_Notificacoes.BringToFront();

            // Configurar evento de clique no Lbl_TituloMenu
            Lbl_TituloMenu.Click += Lbl_TituloMenu_Click;
            Lbl_TituloMenu.Cursor = Cursors.Hand;
        }

        private void Lbl_TituloMenu_Click(object sender, EventArgs e)
        {
            ToggleNotificacoes();
        }

        private void ToggleNotificacoes()
        {
            notificacoesVisiveis = !notificacoesVisiveis;

            if (notificacoesVisiveis)
            {
                CarregarNotificacoes();
                // Posicionar o painel abaixo do Lbl_TituloMenu
                Point posicao = Lbl_TituloMenu.PointToScreen(new Point(0, Lbl_TituloMenu.Height));
                posicao = this.PointToClient(posicao);
                Pnl_Notificacoes.Location = new Point(posicao.X, posicao.Y);
                Pnl_Notificacoes.BringToFront();
            }

            Pnl_Notificacoes.Visible = notificacoesVisiveis;
        }

        private void CarregarNotificacoes()
        {
            if (Sessao.UsuarioLogado == null) return;

            int usuarioId = int.Parse(Sessao.UsuarioLogado.Codigo);

            // Limpar notificações existentes (exceto título e botão fechar)
            for (int i = Pnl_Notificacoes.Controls.Count - 1; i >= 0; i--)
            {
                if (Pnl_Notificacoes.Controls[i] is Panel || Pnl_Notificacoes.Controls[i] is Button)
                {
                    // Manter título e botão fechar
                    if (Pnl_Notificacoes.Controls[i].Text != "NOTIFICAÇÕES" &&
                        Pnl_Notificacoes.Controls[i].Text != "✕")
                    {
                        Pnl_Notificacoes.Controls.RemoveAt(i);
                    }
                }
            }

            var notificacoes = dbNotificacoes.ObterNotificacoesPorUsuario(usuarioId);

            if (!notificacoes.Any())
            {
                // Exibir mensagem quando não há notificações
                Label lblSemNotificacoes = new Label();
                lblSemNotificacoes.Text = "Nenhuma notificação encontrada";
                lblSemNotificacoes.Font = new Font("Segoe UI", 12, FontStyle.Italic);
                lblSemNotificacoes.ForeColor = Color.Gray;
                lblSemNotificacoes.Size = new Size(380, 50);
                lblSemNotificacoes.Location = new Point(10, 50);
                lblSemNotificacoes.TextAlign = ContentAlignment.MiddleCenter;
                Pnl_Notificacoes.Controls.Add(lblSemNotificacoes);
                return;
            }

            int yPos = 50;
            foreach (var notificacao in notificacoes)
            {
                var panelNotificacao = CriarPanelNotificacao(notificacao, yPos);
                Pnl_Notificacoes.Controls.Add(panelNotificacao);
                yPos += panelNotificacao.Height + 10;
            }
        }

        private Panel CriarPanelNotificacao(Notificacoes notificacao, int yPos)
        {
            Panel panel = new Panel();
            panel.Size = new Size(360, 100);
            panel.Location = new Point(10, yPos);
            panel.BackColor = notificacao.isFechada ? Color.FromArgb(250, 250, 250) : Color.FromArgb(255, 255, 200);
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Padding = new Padding(10);

            // Texto da notificação
            Label lblTexto = new Label();
            lblTexto.Text = notificacao.Texto;
            lblTexto.Font = new Font("Segoe UI", 10, notificacao.isFechada ? FontStyle.Regular : FontStyle.Bold);
            lblTexto.ForeColor = notificacao.isFechada ? Color.Gray : Color.Black;
            lblTexto.Size = new Size(300, 40);
            lblTexto.Location = new Point(10, 10);
            lblTexto.AutoSize = false;
            lblTexto.TextAlign = ContentAlignment.MiddleLeft;
            panel.Controls.Add(lblTexto);

            // Data da notificação
            Label lblData = new Label();
            lblData.Text = notificacao.Data.ToString("dd/MM/yyyy HH:mm");
            lblData.Font = new Font("Segoe UI", 8, FontStyle.Italic);
            lblData.ForeColor = Color.Gray;
            lblData.Size = new Size(150, 20);
            lblData.Location = new Point(10, 55);
            lblData.TextAlign = ContentAlignment.MiddleLeft;
            panel.Controls.Add(lblData);

            // Botão de ação (Marcar como lida/Excluir)
            if (!notificacao.isFechada)
            {
                Button btnMarcarLida = new Button();
                btnMarcarLida.Text = "✓";
                btnMarcarLida.Size = new Size(25, 25);
                btnMarcarLida.Location = new Point(320, 10);
                btnMarcarLida.FlatStyle = FlatStyle.Flat;
                btnMarcarLida.ForeColor = Color.Green;
                btnMarcarLida.Font = new Font("Arial", 10, FontStyle.Bold);
                btnMarcarLida.Tag = notificacao.Codigo;
                btnMarcarLida.Click += (s, e) => MarcarNotificacaoComoLida(notificacao.Codigo);
                panel.Controls.Add(btnMarcarLida);
            }

            Button btnExcluir = new Button();
            btnExcluir.Text = "✕";
            btnExcluir.Size = new Size(25, 25);
            btnExcluir.Location = new Point(320, 40);
            btnExcluir.FlatStyle = FlatStyle.Flat;
            btnExcluir.ForeColor = Color.Red;
            btnExcluir.Font = new Font("Arial", 10, FontStyle.Bold);
            btnExcluir.Tag = notificacao.Codigo;
            btnExcluir.Click += (s, e) => ExcluirNotificacao(notificacao.Codigo);
            panel.Controls.Add(btnExcluir);

            return panel;
        }

        private void MarcarNotificacaoComoLida(int codNotificacao)
        {
            if (dbNotificacoes.MarcarNotificacaoComoLida(codNotificacao))
            {
                MessageBox.Show("Notificação marcada como lida!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CarregarNotificacoes();
            }
            else
            {
                MessageBox.Show("Erro ao marcar notificação como lida", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExcluirNotificacao(int codNotificacao)
        {
            var result = MessageBox.Show("Deseja realmente excluir esta notificação?", "Confirmação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (dbNotificacoes.ExcluirNotificacao(codNotificacao))
                {
                    MessageBox.Show("Notificação excluída com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarNotificacoes();
                }
                else
                {
                    MessageBox.Show("Erro ao excluir notificação", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ConfigurarMenuLateral()
        {
            // Configurar largura reduzida (200px)
            Pnl_MenuLateral1.Width = 200;

            // Configurar cores claras para o drawer
            Pnl_MenuLateral1.BackColor = Color.FromArgb(250, 250, 250);
            Pnl_MenuLateral1.BorderStyle = BorderStyle.FixedSingle;
            Pnl_MenuLateral1.AutoScroll = false;

            // Configurar PictureBox e Label do título fixos no topo
            pictureBox1.Location = new Point(10, 10);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.BackColor = Color.Transparent;

            Lbl_TituloMenu.Location = new Point(40, 12);
            Lbl_TituloMenu.ForeColor = Color.FromArgb(64, 64, 64);
            Lbl_TituloMenu.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            Lbl_TituloMenu.AutoSize = true;
            Lbl_TituloMenu.Cursor = Cursors.Hand;

            // Configurar os controles existentes com cores escuras
            Lbl_Categorias1.ForeColor = Color.FromArgb(64, 64, 64);
            Lbl_Categorias1.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            Lbl_ProjetosMenu1.ForeColor = Color.FromArgb(64, 64, 64);
            Lbl_ProjetosMenu1.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // Configurar FlowLayoutPanels
            Flp_Categorias1.FlowDirection = FlowDirection.TopDown;
            Flp_Categorias1.WrapContents = false;
            Flp_Categorias1.AutoSize = true;
            Flp_Categorias1.BackColor = Color.Transparent;

            Flp_Projetos1.FlowDirection = FlowDirection.TopDown;
            Flp_Projetos1.WrapContents = false;
            Flp_Projetos1.AutoSize = true;
            Flp_Projetos1.BackColor = Color.Transparent;

            // Configurar botão Novo Projeto com cores claras - FIXO NO FINAL
            Pnl_NovoProjeto.BackColor = Color.FromArgb(230, 230, 230);
            Pnl_NovoProjeto.Cursor = Cursors.Hand;
            Pnl_NovoProjeto.Click += Pnl_NovoProjeto_Click;
            Pnl_NovoProjeto.BorderStyle = BorderStyle.FixedSingle;
            Pnl_NovoProjeto.Dock = DockStyle.Bottom;
            Pnl_NovoProjeto.Height = 50;

            // Configurar ícone e texto do botão Novo Projeto
            Lbl_Novo.ForeColor = Color.FromArgb(64, 64, 64);
            Lbl_Novo.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            Lbl_Novo.Cursor = Cursors.Hand;
            Lbl_Novo.Click += Pnl_NovoProjeto_Click;

            // Criar um painel de conteúdo para as categorias e projetos (abaixo do título)
            Panel Pnl_Conteudo = new Panel();
            Pnl_Conteudo.Location = new Point(0, 50);
            Pnl_Conteudo.Size = new Size(Pnl_MenuLateral1.Width, Pnl_MenuLateral1.Height - 100);
            Pnl_Conteudo.AutoScroll = true;
            Pnl_Conteudo.BackColor = Color.Transparent;

            // Reorganizar os controles dentro do painel de conteúdo
            Lbl_Categorias1.Parent = Pnl_Conteudo;
            Flp_Categorias1.Parent = Pnl_Conteudo;
            Lbl_ProjetosMenu1.Parent = Pnl_Conteudo;
            Flp_Projetos1.Parent = Pnl_Conteudo;

            // Adicionar o painel de conteúdo ao menu lateral
            Pnl_Conteudo.Parent = Pnl_MenuLateral1;

            // Reposicionar os controles dentro do painel de conteúdo
            Lbl_Categorias1.Location = new Point(10, 10);
            Flp_Categorias1.Location = new Point(10, 40);
            Lbl_ProjetosMenu1.Location = new Point(10, Flp_Categorias1.Bottom + 20);
            Flp_Projetos1.Location = new Point(10, Lbl_ProjetosMenu1.Bottom + 10);

            // Garantir que PictureBox e Lbl_TituloMenu estão no topo
            pictureBox1.BringToFront();
            Lbl_TituloMenu.BringToFront();

            // Carregar itens do menu
            CarregarMenuLateral();

            // Ajustar layout responsivo
            this.Resize += Frm_Projeto_Resize;
        }

        private void CarregarMenuLateral()
        {
            // Limpar painéis
            Flp_Categorias1.Controls.Clear();
            Flp_Projetos1.Controls.Clear();

            if (Sessao.UsuarioLogado == null) return;

            int usuarioId = int.Parse(Sessao.UsuarioLogado.Codigo);

            // Adicionar categorias fixas
            AdicionarCategoriaMenu("Favoritas", ObterQuantidadeFavoritas(usuarioId));
            AdicionarCategoriaMenu("Hoje", ObterQuantidadeTarefasHoje(usuarioId));
            AdicionarCategoriaMenu("Semana", ObterQuantidadeTarefasSemana(usuarioId));
            AdicionarCategoriaMenu("Mês", ObterQuantidadeTarefasMes(usuarioId));

            // Carregar projetos do usuário
            CarregarProjetosMenu(usuarioId);

            // Ajustar responsividade após carregar
            AjustarMenuLateralResponsivo();
        }

        private void AdicionarCategoriaMenu(string nome, int quantidade)
        {
            Panel panelCategoria = new Panel();
            panelCategoria.Width = Flp_Categorias1.Width - 20;
            panelCategoria.Height = 40;
            panelCategoria.Margin = new Padding(0, 0, 0, 5);
            panelCategoria.Cursor = Cursors.Hand;
            panelCategoria.Tag = nome;
            panelCategoria.BackColor = Color.Transparent;
            panelCategoria.BorderStyle = BorderStyle.None;

            // Label do nome
            Label lblNome = new Label();
            lblNome.Text = nome;
            lblNome.ForeColor = Color.FromArgb(64, 64, 64);
            lblNome.Font = new Font("Segoe UI", 11);
            lblNome.Location = new Point(10, 10);
            lblNome.AutoSize = true;
            lblNome.Cursor = Cursors.Hand;
            panelCategoria.Controls.Add(lblNome);

            // Label da quantidade
            Label lblQuantidade = new Label();
            lblQuantidade.Text = quantidade.ToString();
            lblQuantidade.ForeColor = Color.FromArgb(128, 128, 128);
            lblQuantidade.Font = new Font("Segoe UI", 11);
            lblQuantidade.Location = new Point(panelCategoria.Width - 30, 10);
            lblQuantidade.AutoSize = true;
            lblQuantidade.Cursor = Cursors.Hand;
            panelCategoria.Controls.Add(lblQuantidade);

            // Criar método de clique comum
            Action clickAction = () => CategoriaMenuClicada(nome);

            // Evento de clique para o PANEL
            panelCategoria.Click += (sender, e) => clickAction();

            // Evento de clique para o LABEL DO NOME
            lblNome.Click += (sender, e) => clickAction();

            // Evento de clique para o LABEL DA QUANTIDADE
            lblQuantidade.Click += (sender, e) => clickAction();

            // Eventos de hover para o PANEL
            panelCategoria.MouseEnter += (sender, e) =>
            {
                panelCategoria.BackColor = Color.FromArgb(240, 240, 240);
            };
            panelCategoria.MouseLeave += (sender, e) =>
            {
                panelCategoria.BackColor = Color.Transparent;
            };

            // Eventos de hover para os LABELS (propagação do hover)
            lblNome.MouseEnter += (sender, e) => panelCategoria_MouseEnter(panelCategoria, e);
            lblNome.MouseLeave += (sender, e) => panelCategoria_MouseLeave(panelCategoria, e);
            lblQuantidade.MouseEnter += (sender, e) => panelCategoria_MouseEnter(panelCategoria, e);
            lblQuantidade.MouseLeave += (sender, e) => panelCategoria_MouseLeave(panelCategoria, e);

            Flp_Categorias1.Controls.Add(panelCategoria);
        }

        // Métodos auxiliares para os eventos de hover
        private void panelCategoria_MouseEnter(Panel panel, EventArgs e)
        {
            panel.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void panelCategoria_MouseLeave(Panel panel, EventArgs e)
        {
            panel.BackColor = Color.Transparent;
        }

        private void AdicionarProjetoMenu(string nome, int quantidade, Projetos projeto)
        {
            Panel panelProjeto = new Panel();
            panelProjeto.Width = Flp_Projetos1.Width - 20;
            panelProjeto.Height = 40;
            panelProjeto.Margin = new Padding(0, 0, 0, 5);
            panelProjeto.Cursor = Cursors.Hand;
            panelProjeto.Tag = projeto;
            panelProjeto.BackColor = Color.Transparent;
            panelProjeto.BorderStyle = BorderStyle.None;

            // Label do nome
            Label lblNome = new Label();
            lblNome.Text = nome;
            lblNome.ForeColor = Color.FromArgb(64, 64, 64);
            lblNome.Font = new Font("Segoe UI", 11);
            lblNome.Location = new Point(10, 10);
            lblNome.AutoSize = true;
            lblNome.Cursor = Cursors.Hand;
            panelProjeto.Controls.Add(lblNome);

            // Label da quantidade
            Label lblQuantidade = new Label();
            lblQuantidade.Text = quantidade.ToString();
            lblQuantidade.ForeColor = Color.FromArgb(128, 128, 128);
            lblQuantidade.Font = new Font("Segoe UI", 11);
            lblQuantidade.Location = new Point(panelProjeto.Width - 30, 10);
            lblQuantidade.AutoSize = true;
            lblQuantidade.Cursor = Cursors.Hand;
            panelProjeto.Controls.Add(lblQuantidade);

            // Criar método de clique comum
            Action clickAction = () => ProjetoMenuClicado(projeto);

            // Evento de clique para o PANEL
            panelProjeto.Click += (sender, e) => clickAction();

            // Evento de clique para o LABEL DO NOME
            lblNome.Click += (sender, e) => clickAction();

            // Evento de clique para o LABEL DA QUANTIDADE
            lblQuantidade.Click += (sender, e) => clickAction();

            // Eventos de hover para o PANEL
            panelProjeto.MouseEnter += (sender, e) =>
            {
                panelProjeto.BackColor = Color.FromArgb(240, 240, 240);
            };
            panelProjeto.MouseLeave += (sender, e) =>
            {
                panelProjeto.BackColor = Color.Transparent;
            };

            // Eventos de hover para os LABELS (propagação do hover)
            lblNome.MouseEnter += (sender, e) => panelProjeto_MouseEnter(panelProjeto, e);
            lblNome.MouseLeave += (sender, e) => panelProjeto_MouseLeave(panelProjeto, e);
            lblQuantidade.MouseEnter += (sender, e) => panelProjeto_MouseEnter(panelProjeto, e);
            lblQuantidade.MouseLeave += (sender, e) => panelProjeto_MouseLeave(panelProjeto, e);

            Flp_Projetos1.Controls.Add(panelProjeto);
        }

        // Métodos auxiliares para os eventos de hover dos projetos
        private void panelProjeto_MouseEnter(Panel panel, EventArgs e)
        {
            panel.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void panelProjeto_MouseLeave(Panel panel, EventArgs e)
        {
            panel.BackColor = Color.Transparent;
        }

        private void Frm_Projeto_Resize(object sender, EventArgs e)
        {
            // Ajustar responsividade do menu lateral
            AjustarMenuLateralResponsivo();

            // Reposicionar o painel de notificações se estiver visível
            if (notificacoesVisiveis)
            {
                Point posicao = Lbl_TituloMenu.PointToScreen(new Point(0, Lbl_TituloMenu.Height));
                posicao = this.PointToClient(posicao);
                Pnl_Notificacoes.Location = new Point(posicao.X, posicao.Y);
            }

            // Ajustar layout dos controles principais
            AjustarLayoutParaMenuLateral();
        }

        private void AjustarMenuLateralResponsivo()
        {
            // Ajustar largura do menu lateral baseado no tamanho da tela
            if (this.Width < 800)
            {
                Pnl_MenuLateral1.Width = 160;
            }
            else if (this.Width < 1200)
            {
                Pnl_MenuLateral1.Width = 180;
            }
            else
            {
                Pnl_MenuLateral1.Width = 200;
            }

            // Ajustar a largura dos painéis internos
            foreach (Control control in Flp_Categorias1.Controls)
            {
                if (control is Panel panel)
                {
                    panel.Width = Flp_Categorias1.Width - 20;
                    if (panel.Controls.Count >= 2)
                    {
                        panel.Controls[1].Left = panel.Width - 30;
                    }
                }
            }

            foreach (Control control in Flp_Projetos1.Controls)
            {
                if (control is Panel panel)
                {
                    panel.Width = Flp_Projetos1.Width - 20;
                    if (panel.Controls.Count >= 2)
                    {
                        panel.Controls[1].Left = panel.Width - 30;
                    }
                }
            }
        }

        private void Pnl_NovoProjeto_Click(object sender, EventArgs e)
        {
            AbrirCriarProjeto();
        }

        private void AbrirCriarProjeto()
        {
            this.Hide();
            Frm_CriarProjeto frmCriarProjeto = new Frm_CriarProjeto();
            frmCriarProjeto.ShowDialog();
            this.Show();
            CarregarMenuLateral();
        }

        private void CarregarProjetosMenu(int usuarioId)
        {
            var projetos = dbProjetos.ObterTodosProjetosUsuario(usuarioId);

            foreach (var projeto in projetos)
            {
                int tarefasPendentes = dbTarefas.ObterTarefasPendentesPorProjeto(projeto.Codigo).Count;
                AdicionarProjetoMenu(projeto.Nome, tarefasPendentes, projeto);
            }
        }

        private void CategoriaMenuClicada(string categoria)
        {
            if (Sessao.UsuarioLogado == null) return;

            int usuarioId = int.Parse(Sessao.UsuarioLogado.Codigo);

            switch (categoria)
            {
                case "Favoritas":
                    AbrirFavoritos(usuarioId);
                    break;
                case "Hoje":
                    CarregarTarefasHoje(usuarioId);
                    break;
                case "Semana":
                    CarregarTarefasSemana(usuarioId);
                    break;
                case "Mês":
                    CarregarTarefasMes(usuarioId);
                    break;
            }
        }

        private void ProjetoMenuClicado(Projetos projeto)
        {
            if (Sessao.UsuarioLogado == null) return;

            int usuarioId = int.Parse(Sessao.UsuarioLogado.Codigo);
            AbrirFormProjeto(projeto, usuarioId);
        }

        private int ObterQuantidadeFavoritas(int usuarioId)
        {
            return dbFavoritos.ObterTarefasFavoritas(usuarioId).Count;
        }

        private int ObterQuantidadeTarefasHoje(int usuarioId)
        {
            DateTime hoje = DateTime.Today;
            return dbTarefas.ObterTarefasPorPeriodo(usuarioId, hoje, hoje.AddDays(1).AddSeconds(-1)).Count;
        }

        private int ObterQuantidadeTarefasSemana(int usuarioId)
        {
            DateTime inicioSemana = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            DateTime fimSemana = inicioSemana.AddDays(7).AddSeconds(-1);
            return dbTarefas.ObterTarefasPorPeriodo(usuarioId, inicioSemana, fimSemana).Count;
        }

        private int ObterQuantidadeTarefasMes(int usuarioId)
        {
            DateTime inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime fimMes = inicioMes.AddMonths(1).AddSeconds(-1);
            return dbTarefas.ObterTarefasPorPeriodo(usuarioId, inicioMes, fimMes).Count;
        }

        private void AbrirFavoritos(int usuarioId)
        {
            var favoritos = dbFavoritos.ObterTarefasFavoritas(usuarioId);

            if (favoritos.Any())
            {
                this.Hide();
                Frm_Projeto frmFavoritos = new Frm_Projeto(favoritos, usuarioId);
                frmFavoritos.ShowDialog();
                this.Show();
                CarregarMenuLateral();
            }
            else
            {
                MessageBox.Show("Você ainda não marcou nenhuma tarefa como favorita.");
            }
        }

        private void CarregarTarefasHoje(int usuarioId)
        {
            MessageBox.Show("Carregando tarefas de hoje...");
        }

        private void CarregarTarefasSemana(int usuarioId)
        {
            MessageBox.Show("Carregando tarefas da semana...");
        }

        private void CarregarTarefasMes(int usuarioId)
        {
            MessageBox.Show("Carregando tarefas do mês...");
        }

        private void AbrirFormProjeto(Projetos projeto, int usuarioId)
        {
            this.Hide();
            Frm_Projeto frmProjeto = new Frm_Projeto(projeto, usuarioId);
            frmProjeto.ShowDialog();
            this.Show();
            CarregarMenuLateral();
        }

        private void InitializeProjeto(Projetos projeto, int usuarioId)
        {
            this.projetoSelecionado = projeto;
            this.usuarioLogadoId = usuarioId;
            this.isModoFavoritos = false;

            this.Text = "Menu Principal | Taskool";
            Lbl_Titulo.Text = projeto.Nome;

            this.tarefasPendentes = dbTarefas.ObterTarefasPendentesPorProjeto(projeto.Codigo);
            this.tarefasConcluidas = dbTarefas.ObterTarefasConcluidasPorProjeto(projeto.Codigo);

            InicializarComponentesDaInterface();
        }

        private void InitializeFavoritos(List<Projeto_Tarefas> favoritos, int usuarioId)
        {
            this.tarefasPendentes = favoritos;
            this.tarefasConcluidas = new List<Projeto_Tarefas>();
            this.usuarioLogadoId = usuarioId;
            this.projetoSelecionado = null;
            this.isModoFavoritos = true;

            this.Text = "Menu Principal | Taskool";
            Lbl_Titulo.Text = "Favoritos";

            InicializarComponentesDaInterface();
        }

        private void InicializarComponentesDaInterface()
        {
            AjustarLayoutParaMenuLateral();

            Lst_ListaTarefas.AutoScroll = true;
            Lbl_Triste.Text = "Nenhuma tarefa criada";

            Txt_Tarefa.Text = "Adicionar uma tarefa...";
            Txt_Tarefa.ForeColor = SystemColors.GrayText;

            // Configurar título do grupo de status
            Grp_Status.Text = "Status do Projeto";
            Lbl_Status.Text = "Progresso do Projeto";
            Lbl_Status.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            Lbl_Status.ForeColor = Color.FromArgb(64, 64, 64);

            Lbl_Pendentes.Text = "Colaboradores com Tarefas Pendentes";
            Lbl_Concluidas.Text = "Colaboradores com Todas Tarefas Concluídas";

            Lbl_Pendentes.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            Lbl_Concluidas.Font = new Font("Segoe UI", 8, FontStyle.Bold);

            // Configurar flow layouts
            Flw_Pendentes.FlowDirection = FlowDirection.LeftToRight;
            Flw_Pendentes.WrapContents = true;
            Flw_Concluidos.FlowDirection = FlowDirection.LeftToRight;
            Flw_Concluidos.WrapContents = true;

            // REMOVA estas linhas se os eventos já estiverem configurados no designer
            Txt_Tarefa.Enter += Txt_Tarefa_Enter;
            Txt_Tarefa.Leave += Txt_Tarefa_Leave;
            Txt_Tarefa.KeyDown += Txt_Tarefa_KeyDown;

            CentralizarMensagemDeVazio();
            AtualizarInterface();
        }

        private void AjustarLayoutParaMenuLateral()
        {
            int menuWidth = Pnl_MenuLateral1.Width;
            int margin = 10;

            // Posicionar título
            Lbl_Titulo.Left = menuWidth + margin;
            Lbl_Titulo.Width = this.Width - menuWidth - (margin * 2);

            // Posicionar campo de texto para nova tarefa
            Txt_Tarefa.Left = menuWidth + margin;
            Txt_Tarefa.Width = this.Width - menuWidth - (margin * 2) - 40;

            // Posicionar ícone de adicionar
            Pic_IconPlus.Left = Txt_Tarefa.Right + 5;

            // Posicionar lista de tarefas
            Lst_ListaTarefas.Left = menuWidth + margin;
            Lst_ListaTarefas.Width = this.Width - menuWidth - (margin * 2);

            // Calcular altura da lista de tarefas (deixar espaço para o Grp_Status)
            int alturaDisponivel = this.Height - Lst_ListaTarefas.Top - 200; // 200px para o Grp_Status + margens
            Lst_ListaTarefas.Height = Math.Max(alturaDisponivel, 100); // Mínimo de 100px

            // Posicionar Grp_Status à direita do drawer e abaixo da lista de tarefas
            Grp_Status.Left = menuWidth + margin;
            Grp_Status.Top = Lst_ListaTarefas.Bottom + 10;
            Grp_Status.Width = Lst_ListaTarefas.Width;
            Grp_Status.Height = 180;

            // Ajustar posição do link de tarefas concluídas
            Lnk_TarefasConcluidas.Left = menuWidth + margin;
            Lnk_TarefasConcluidas.Top = Lst_ListaTarefas.Bottom - 25;

            CentralizarMensagemDeVazio();
        }

        private void CentralizarMensagemDeVazio()
        {
            int menuWidth = Pnl_MenuLateral1.Width;
            int centerX = menuWidth + (this.Width - menuWidth) / 2;

            Lbl_Triste.Left = centerX - (Lbl_Triste.Width / 2);
            Lbl_Triste.Top = (this.ClientSize.Height / 2) - Lbl_Triste.Height - (Pic_Tristesa.Height / 2) - 10;

            Pic_Tristesa.Left = centerX - (Pic_Tristesa.Width / 2);
            Pic_Tristesa.Top = (this.ClientSize.Height / 2) - (Pic_Tristesa.Height / 2);
        }

        private void AtualizarInterface()
        {
            Lst_ListaTarefas.Controls.Clear();

            bool temTarefas = tarefasPendentes.Any() || tarefasConcluidas.Any();

            Txt_Tarefa.Visible = !isModoFavoritos;
            Grp_Status.Visible = !isModoFavoritos && temTarefas;
            Lnk_TarefasConcluidas.Visible = !isModoFavoritos && tarefasConcluidas.Any();

            if (!temTarefas)
            {
                Lbl_Triste.Visible = true;
                Pic_Tristesa.Visible = true;
                Lst_ListaTarefas.Visible = false;
                Grp_Status.Visible = false;
                CentralizarMensagemDeVazio();
            }
            else
            {
                Lbl_Triste.Visible = false;
                Pic_Tristesa.Visible = false;
                Lst_ListaTarefas.Visible = true;

                var tarefasExibidas = isExibindoConcluidas ? tarefasConcluidas : tarefasPendentes;
                Lnk_TarefasConcluidas.Text = isExibindoConcluidas ?
                    "VOLTAR PARA PENDENTES" :
                    $"TAREFAS CONCLUIDAS: {tarefasConcluidas.Count}";

                int yPos = 5;
                foreach (var tarefa in tarefasExibidas)
                {
                    var tarefaItem = CriarItemTarefa(tarefa, tarefasConcluidas.Contains(tarefa));
                    tarefaItem.Top = yPos;
                    Lst_ListaTarefas.Controls.Add(tarefaItem);
                    yPos += tarefaItem.Height + 5;
                }

                if (!isModoFavoritos)
                {
                    AtualizarStatusGrafico();
                }
            }

            // Reajustar o layout após atualizar a interface
            AjustarLayoutParaMenuLateral();
        }

        private void AtualizarStatusGrafico()
        {
            Flw_Pendentes.Controls.Clear();
            Flw_Concluidos.Controls.Clear();

            Lbl_Pendentes.Visible = false;
            Lbl_Concluidas.Visible = false;

            Chrt_Tarefas.Series.Clear();
            var totalTarefas = tarefasPendentes.Count + tarefasConcluidas.Count;

            if (totalTarefas > 0)
            {
                Series seriePizza = new Series("StatusTarefas") { ChartType = SeriesChartType.Pie };

                if (tarefasPendentes.Count > 0)
                {
                    var pontoPendente = seriePizza.Points.Add(tarefasPendentes.Count);
                    pontoPendente.Color = Color.BlueViolet;
                    pontoPendente.LegendText = "Pendentes";
                }

                if (tarefasConcluidas.Count > 0)
                {
                    var pontoConcluido = seriePizza.Points.Add(tarefasConcluidas.Count);
                    pontoConcluido.Color = Color.Goldenrod;
                    pontoConcluido.LegendText = "Concluídas";
                }

                Chrt_Tarefas.Series.Add(seriePizza);

                Chrt_Tarefas.Titles.Clear();
                Chrt_Tarefas.Titles.Add("Status das Tarefas").Font = new Font("Segoe UI", 8, FontStyle.Bold);

                double porcentagem = totalTarefas > 0 ? (double)tarefasConcluidas.Count / totalTarefas : 0;
                Title titlePorcentagem = new Title("Percentagem")
                {
                    Text = $"{porcentagem:P0}",
                    Docking = Docking.Bottom,
                    Alignment = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    IsDockedInsideChartArea = true
                };
                Chrt_Tarefas.Titles.Add(titlePorcentagem);

                Chrt_Tarefas.Legends.Clear();

                // Adicionar legenda personalizada
                Legend legend = new Legend();
                legend.Docking = Docking.Right;
                legend.Alignment = StringAlignment.Center;
                legend.Font = new Font("Segoe UI", 8);
                Chrt_Tarefas.Legends.Add(legend);

                seriePizza.IsValueShownAsLabel = false;
                Chrt_Tarefas.ChartAreas[0].Area3DStyle.Enable3D = false;

                Chrt_Tarefas.ChartAreas[0].InnerPlotPosition.X = 10;
                Chrt_Tarefas.ChartAreas[0].InnerPlotPosition.Y = 10;
                Chrt_Tarefas.ChartAreas[0].InnerPlotPosition.Width = 80;
                Chrt_Tarefas.ChartAreas[0].InnerPlotPosition.Height = 80;
            }
            else
            {
                Chrt_Tarefas.Series.Clear();
                Chrt_Tarefas.Titles.Clear();
                Chrt_Tarefas.Titles.Add("Nenhuma tarefa para exibir.").Font = new Font("Segoe UI", 10, FontStyle.Italic);
            }

            if (projetoSelecionado != null)
            {
                var colaboradores = dbMembros.ObterMembrosProjeto(projetoSelecionado.Codigo);
                if (colaboradores.Any())
                {
                    Lbl_Pendentes.Visible = true;
                    Lbl_Concluidas.Visible = true;

                    // Lista de cores únicas para colaboradores
                    List<Color> coresDisponiveis = new List<Color>
                    {
                        Color.BlueViolet, Color.Goldenrod, Color.Crimson, Color.DarkCyan,
                        Color.DarkOrange, Color.DodgerBlue, Color.ForestGreen, Color.Indigo,
                        Color.Teal, Color.Tomato, Color.SlateBlue, Color.Chocolate,
                        Color.DarkMagenta, Color.SeaGreen, Color.OrangeRed
                    };

                    Dictionary<string, Color> coresUsadasPendentes = new Dictionary<string, Color>();
                    Dictionary<string, Color> coresUsadasConcluidos = new Dictionary<string, Color>();
                    int indexCor = 0;

                    foreach (var colab in colaboradores)
                    {
                        // Verificar se colaborador tem tarefas pendentes
                        bool temTarefasPendentes = tarefasPendentes.Any(t => t.CodUsuario == colab.Codigo);
                        bool temTarefasConcluidas = tarefasConcluidas.Any(t => t.CodUsuario == colab.Codigo);

                        // Só vai para concluídos se NÃO tiver tarefas pendentes
                        bool vaiParaConcluidos = !temTarefasPendentes && temTarefasConcluidas;

                        Color corColaborador;

                        if (vaiParaConcluidos)
                        {
                            // Atribuir cor única para concluídos
                            if (!coresUsadasConcluidos.ContainsKey(colab.Nome))
                            {
                                corColaborador = coresDisponiveis[indexCor % coresDisponiveis.Count];
                                coresUsadasConcluidos[colab.Nome] = corColaborador;
                                indexCor++;
                            }
                            else
                            {
                                corColaborador = coresUsadasConcluidos[colab.Nome];
                            }

                            var lblColabConc = CriarLabelColaborador(colab.Nome, corColaborador);
                            Flw_Concluidos.Controls.Add(lblColabConc);
                        }
                        else if (temTarefasPendentes)
                        {
                            // Atribuir cor única para pendentes
                            if (!coresUsadasPendentes.ContainsKey(colab.Nome))
                            {
                                corColaborador = coresDisponiveis[indexCor % coresDisponiveis.Count];
                                coresUsadasPendentes[colab.Nome] = corColaborador;
                                indexCor++;
                            }
                            else
                            {
                                corColaborador = coresUsadasPendentes[colab.Nome];
                            }

                            var lblColabPend = CriarLabelColaborador(colab.Nome, corColaborador);
                            Flw_Pendentes.Controls.Add(lblColabPend);
                        }
                    }

                    // Adicionar labels informativas se os flow layouts estiverem vazios
                    if (Flw_Pendentes.Controls.Count == 0)
                    {
                        Label lblSemPendentes = new Label
                        {
                            Text = "Nenhum colaborador com tarefas pendentes",
                            Font = new Font("Segoe UI", 8, FontStyle.Italic),
                            ForeColor = Color.Gray,
                            AutoSize = true
                        };
                        Flw_Pendentes.Controls.Add(lblSemPendentes);
                    }

                    if (Flw_Concluidos.Controls.Count == 0)
                    {
                        Label lblSemConcluidos = new Label
                        {
                            Text = "Nenhum colaborador concluiu todas as tarefas",
                            Font = new Font("Segoe UI", 8, FontStyle.Italic),
                            ForeColor = Color.Gray,
                            AutoSize = true
                        };
                        Flw_Concluidos.Controls.Add(lblSemConcluidos);
                    }
                }
                else
                {
                    // Caso não haja colaboradores no projeto
                    Label lblSemColaboradores = new Label
                    {
                        Text = "Nenhum colaborador no projeto",
                        Font = new Font("Segoe UI", 8, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        AutoSize = true
                    };

                    Panel panelInfo = new Panel();
                    panelInfo.Controls.Add(lblSemColaboradores);
                    panelInfo.Dock = DockStyle.Fill;

                    // Adicionar em ambos os flow layouts
                    Flw_Pendentes.Controls.Add(panelInfo);
                    Flw_Concluidos.Controls.Add(new Panel()); // Panel vazio para manter layout
                }
            }
        }

        private Label CriarLabelColaborador(string nome, Color cor)
        {
            var lblColab = new Label
            {
                Text = nome[0].ToString().ToUpper(),
                Width = 25,
                Height = 25,
                BackColor = cor,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(3),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            // Tooltip com nome completo
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(lblColab, nome);
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 500;

            return lblColab;
        }

        private void AdicionarNovaTarefa()
        {
            if (isModoFavoritos) return;

            string descricao = Txt_Tarefa.Text.Trim();
            if (string.IsNullOrEmpty(descricao) || descricao == "Adicionar uma tarefa...") return;

            var novaTarefa = new Projeto_Tarefas
            {
                Descricao = descricao,
                isConcluida = false,
                CodProjeto = projetoSelecionado.Codigo
            };

            bool sucesso;

            if (projetoSelecionado.CodUsuario != usuarioLogadoId)
            {
                sucesso = dbTarefas.CriarTarefaCompartilhada(novaTarefa, usuarioLogadoId);
            }
            else
            {
                sucesso = dbTarefas.InserirTarefa(novaTarefa);
            }

            if (sucesso)
            {
                tarefasPendentes.Insert(0, novaTarefa);
                Txt_Tarefa.Text = "Adicionar uma tarefa...";
                Txt_Tarefa.ForeColor = SystemColors.GrayText;
                Pic_IconPlus.Visible = true;
                AtualizarInterface();
            }
            else
            {
                MessageBox.Show("Erro ao salvar a tarefa: " + dbTarefas.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Control CriarItemTarefa(Projeto_Tarefas tarefa, bool isConcluida)
        {
            var panel = new Panel
            {
                Width = Lst_ListaTarefas.Width - 25,
                Height = 45,
                Margin = new Padding(5),
                BackColor = Color.White,
                Tag = tarefa,
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand // Adicionar cursor de mão para indicar que é clicável
            };

            var checkBoxConcluida = new CheckBox
            {
                Width = 20,
                Height = 20,
                Location = new Point(10, 12),
                Checked = isConcluida
            };

            checkBoxConcluida.CheckedChanged += (sender, e) =>
            {
                // Impedir que o evento propague para o clique do painel
                ((CheckBox)sender).Tag = "checkedChanged";
                dbTarefas.AtualizarStatusTarefa(tarefa.Codigo, checkBoxConcluida.Checked);

                if (checkBoxConcluida.Checked)
                {
                    tarefasPendentes.Remove(tarefa);
                    tarefasConcluidas.Add(tarefa);
                }
                else
                {
                    tarefasConcluidas.Remove(tarefa);
                    tarefasPendentes.Add(tarefa);
                }
                AtualizarInterface();
            };
            panel.Controls.Add(checkBoxConcluida);

            var labelDescricao = new Label
            {
                Text = tarefa.Descricao,
                AutoSize = false,
                Width = panel.Width - 80,
                Height = 20,
                Location = new Point(40, 13),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand // Adicionar cursor de mão
            };

            if (isConcluida)
            {
                labelDescricao.Font = new Font(labelDescricao.Font, FontStyle.Strikeout);
                labelDescricao.ForeColor = Color.Gray;
            }

            panel.Controls.Add(labelDescricao);

            bool estaFavoritado = dbFavoritos.EhFavorito(usuarioLogadoId, tarefa.Codigo);

            var picEstrela = new PictureBox
            {
                Width = 20,
                Height = 20,
                Location = new Point(panel.Width - 35, 12),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Cursor = Cursors.Hand,
                Tag = new { EstaFavoritado = estaFavoritado, Tarefa = tarefa }
            };

            // Configurar imagem da estrela (ajuste conforme seus recursos)
            picEstrela.Image = estaFavoritado ?
                Properties.Resources.estrela__1_ :
                Properties.Resources.estrela;

            picEstrela.Click += (sender, e) =>
            {
                // Impedir que o evento propague para o clique do painel
                e.GetType().GetProperty("Handled")?.SetValue(e, true);
                var pictureBox = (PictureBox)sender;
                var dados = (dynamic)pictureBox.Tag;
                bool novoEstado = !dados.EstaFavoritado;
                Projeto_Tarefas tarefaClicada = dados.Tarefa;

                if (novoEstado)
                {
                    if (dbFavoritos.AdicionarFavorito(usuarioLogadoId, tarefaClicada.Codigo))
                    {
                        pictureBox.Image = Properties.Resources.estrela__1_;
                        pictureBox.Tag = new { EstaFavoritado = true, Tarefa = tarefaClicada };
                    }
                }
                else
                {
                    if (dbFavoritos.RemoverFavorito(usuarioLogadoId, tarefaClicada.Codigo))
                    {
                        pictureBox.Image = Properties.Resources.estrela;
                        pictureBox.Tag = new { EstaFavoritado = false, Tarefa = tarefaClicada };
                    }
                }
            };

            panel.Controls.Add(picEstrela);

            // Adicionar evento de clique para abrir os detalhes da tarefa
            panel.Click += (sender, e) => AbrirDetalhesTarefa(tarefa);
            labelDescricao.Click += (sender, e) => AbrirDetalhesTarefa(tarefa);

            // Adicionar evento de duplo clique para abrir os detalhes
            panel.DoubleClick += (sender, e) => AbrirDetalhesTarefa(tarefa);
            labelDescricao.DoubleClick += (sender, e) => AbrirDetalhesTarefa(tarefa);

            return panel;
        }

        private void AbrirDetalhesTarefa(Projeto_Tarefas tarefa)
        {
            // Verificar se o formulário de detalhes já está aberto
            foreach (Form form in Application.OpenForms)
            {
                if (form is Frm_TarefasDetalhes detalhesForm && detalhesForm.tarefaAtual?.Codigo == tarefa.Codigo)
                {
                    form.BringToFront();
                    return;
                }
            }

            // Se não estiver aberto, criar novo formulário
            Frm_TarefasDetalhes novoForm = new Frm_TarefasDetalhes(tarefa);
            novoForm.Show();
        }

        // REMOVA estes métodos se já existirem no arquivo Frm_Projeto.Designer.cs
        private void Lnk_TarefasConcluidas_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            isExibindoConcluidas = !isExibindoConcluidas;
            AtualizarInterface();
        }

        private void Txt_Tarefa_Enter(object sender, EventArgs e)
        {
            if (Txt_Tarefa.Text == "Adicionar uma tarefa...")
            {
                Txt_Tarefa.Text = "";
                Txt_Tarefa.ForeColor = SystemColors.WindowText;
                Pic_IconPlus.Visible = false;
            }
        }

        private void Txt_Tarefa_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Txt_Tarefa.Text))
            {
                Txt_Tarefa.Text = "Adicionar uma tarefa...";
                Txt_Tarefa.ForeColor = SystemColors.GrayText;
                Pic_IconPlus.Visible = true;
            }
        }

        private void Txt_Tarefa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AdicionarNovaTarefa();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // Eventos vazios necessários
        private void Frm_Projeto_Load(object sender, EventArgs e) { }
        private void Lbl_Titulo_Click(object sender, EventArgs e) { }
        private void Grp_Status_Enter(object sender, EventArgs e) { }
        private void Txt_Tarefa_TextChanged(object sender, EventArgs e) { }
        private void Pic_IconPlus_Click(object sender, EventArgs e)
        {
            AdicionarNovaTarefa();
        }
    }
}