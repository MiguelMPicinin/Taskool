using CanvasApp;
using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using CanvasApp.Classes.Tema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace Home
{
    public partial class Frm_Home : Form
    {
        private WindowsMediaPlayer player;
        private string musicaSelecionada;
        private bool musicaTocando = false;
        private Timer timerAtualizacao = new Timer();
        private string musicasPath = @"C:\Users\migue\OneDrive\Área de Trabalho\Codigo Fonte\Taskool\CanvasApp\musicas-testes\";

        private readonly UsuarioDB dbUsuario = new UsuarioDB();
        private readonly TarefasDB dbTarefas = new TarefasDB();
        private readonly ProjetosDB dbProjetos = new ProjetosDB();
        private readonly FavoritosDB dbFavoritos = new FavoritosDB();
        private readonly NotificacoesDB dbNotificacoes = new NotificacoesDB();

        // Painel de notificações
        private Panel Pnl_Notificacoes;
        private bool notificacoesVisiveis = false;

        public class MensagemMotivacional
        {
            public string mensagem { get; set; }
            public string autor { get; set; }
        }

        private List<MensagemMotivacional> mensagensMotivacionais = new List<MensagemMotivacional>();

        public Frm_Home()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            player = new WindowsMediaPlayer();
            player.settings.autoStart = false;

            InitializeUI();

            timerAtualizacao.Interval = 1000;
            timerAtualizacao.Tick += (s, e) => AtualizarHora();
            timerAtualizacao.Start();

            player.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);

            Pic_PlayPause.Click += Pic_PlayPause_Click;
            Pic_Foto.Click += Pic_Foto_Click;
            Btn_EditarDados.Click += Btn_EditarDados_Click;
            Btn_Sair.Click += Btn_Sair_Click;
            Btn_Pt.Click += Btn_Pt_Click;
            Btn_En.Click += Btn_En_Click;
            Lbl_NomeMusica.Click += Lbl_NomeMusica_Click;

            // Configurar o menu lateral
            ConfigurarMenuLateral();

            // Configurar o painel de notificações
            ConfigurarPainelNotificacoes();

            this.Resize += Frm_Home_Resize;
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
            this.Resize += Frm_Home_Resize;
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

        private void Frm_Home_Resize(object sender, EventArgs e)
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

        private void InitializeUI()
        {
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Tema.corAtual;
            Btn_En.Text = "EN";
            Btn_Pt.Text = "PT";
            Btn_Sair.Text = "Sair";
            Btn_EditarDados.Text = "Editar Dados";

            Pic_PlayPause.Cursor = Cursors.Hand;
            Lbl_NomeMusica.Cursor = Cursors.Hand;

            ConfigurarBotoesIdioma("PT");
            AtualizarHora();
            DefinirSaudacao("PT");

            Lbl_NomeMusica.Text = "Carregando música...";
            TocarMusicaAleatoria();

            CarregarMensagensMotivacionais();
            ExibirMensagemAleatoria();

            Pnl_Menu.Visible = false;
        }

        private void TocarMusicaAleatoria()
        {
            try
            {
                if (!Directory.Exists(musicasPath))
                {
                    Lbl_NomeMusica.Text = "Pasta de músicas não encontrada";
                    return;
                }

                string[] arquivosMusicas = Directory.GetFiles(musicasPath, "*.mp3");

                if (arquivosMusicas.Length == 0)
                {
                    Lbl_NomeMusica.Text = "Nenhuma música encontrada";
                    return;
                }

                Random rnd = new Random();
                int indiceAleatorio = rnd.Next(arquivosMusicas.Length);
                musicaSelecionada = arquivosMusicas[indiceAleatorio];

                player.controls.stop();
                player.URL = musicaSelecionada;
                player.settings.volume = 80;
                player.settings.autoStart = true;
                Lbl_NomeMusica.Text = Path.GetFileNameWithoutExtension(musicaSelecionada);
                player.controls.play();
                musicaTocando = true;
            }
            catch (Exception ex)
            {
                Lbl_NomeMusica.Text = "Erro ao carregar música";
            }
        }

        private void AtualizarHora()
        {
            Lbl_Hora.Text = DateTime.Now.ToString("HH:mm");
        }

        private void ExibirMensagemAleatoria()
        {
            if (mensagensMotivacionais != null && mensagensMotivacionais.Count > 0)
            {
                Random rnd = new Random();
                int indice = rnd.Next(mensagensMotivacionais.Count);
                var msg = mensagensMotivacionais[indice];
                Lbl_Mensagem.Text = $"\"{msg.mensagem}\" - {msg.autor}";
            }
            else
            {
                Lbl_Mensagem.Text = "Adicione mensagens motivacionais no arquivo JSON.";
            }
        }

        private void DefinirSaudacao(string idioma)
        {
            int hora = DateTime.Now.Hour;
            string saudacao = "";

            if (hora >= 12 && hora <= 17)
                saudacao = (idioma == "PT") ? "Boa Tarde, " : "Good afternoon, ";
            else if (hora >= 18 && hora <= 23)
                saudacao = (idioma == "PT") ? "Boa Noite, " : "Good evening, ";
            else if (hora >= 0 && hora <= 3)
                saudacao = (idioma == "PT") ? "Boa Madrugada, " : "Good sun-up, ";
            else if (hora >= 4 && hora <= 11)
                saudacao = (idioma == "PT") ? "Bom Dia, " : "Good morning, ";
            else
                saudacao = (idioma == "PT") ? "Olá, " : "Hello, ";

            if (Sessao.UsuarioLogado != null)
            {
                Lbl_BoasVindas.Text = saudacao + Sessao.UsuarioLogado.Nome + "!";
            }
        }

        private void ConfigurarBotoesIdioma(string idiomaAtual)
        {
            if (idiomaAtual == "PT")
            {
                Btn_Pt.BackColor = Color.LightBlue;
                Btn_En.BackColor = SystemColors.Control;
            }
            else
            {
                Btn_En.BackColor = Color.LightBlue;
                Btn_Pt.BackColor = SystemColors.Control;
            }
        }

        private void CarregarMensagensMotivacionais()
        {
            try
            {
                string caminhoJson = Path.Combine(Application.StartupPath, "Mensagens", "mensagens.json");

                if (!Directory.Exists(Path.GetDirectoryName(caminhoJson)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(caminhoJson));
                }

                if (File.Exists(caminhoJson))
                {
                    string json = File.ReadAllText(caminhoJson);
                    mensagensMotivacionais = JsonConvert.DeserializeObject<List<MensagemMotivacional>>(json);
                }
                else
                {
                    mensagensMotivacionais = new List<MensagemMotivacional>
                    {
                        new MensagemMotivacional { mensagem = "A persistência é o caminho do êxito", autor = "Charles Chaplin" },
                        new MensagemMotivacional { mensagem = "Eu faço da dificuldade a minha motivação. A volta por cima vem na continuação.", autor = "Charlie Brown Jr" }
                    };

                    string json = JsonConvert.SerializeObject(mensagensMotivacionais, Formatting.Indented);
                    File.WriteAllText(caminhoJson, json);
                }
            }
            catch (Exception ex)
            {
                mensagensMotivacionais = new List<MensagemMotivacional>();
            }
        }

        private void Pic_PlayPause_Click(object sender, EventArgs e)
        {
            try
            {
                if (musicaTocando)
                {
                    player.controls.pause();
                    musicaTocando = false;
                }
                else if (!string.IsNullOrEmpty(musicaSelecionada))
                {
                    player.controls.play();
                    musicaTocando = true;
                }
                else
                {
                    TocarMusicaAleatoria();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao controlar música: {ex.Message}");
            }
        }

        private void Player_PlayStateChange(int NewState)
        {
            if (NewState == (int)WMPPlayState.wmppsMediaEnded && musicaTocando)
            {
                TocarProximaMusica();
            }
        }

        private void TocarProximaMusica()
        {
            try
            {
                if (Directory.Exists(musicasPath))
                {
                    string[] arquivosMusicas = Directory.GetFiles(musicasPath, "*.mp3");
                    if (arquivosMusicas.Length > 0)
                    {
                        int indexAtual = Array.IndexOf(arquivosMusicas, musicaSelecionada);
                        int proximoIndex = (indexAtual + 1) % arquivosMusicas.Length;
                        musicaSelecionada = arquivosMusicas[proximoIndex];

                        player.controls.stop();
                        player.URL = musicaSelecionada;
                        player.controls.play();
                        Lbl_NomeMusica.Text = Path.GetFileNameWithoutExtension(musicaSelecionada);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao tocar próxima música: {ex.Message}");
            }
        }

        private void Btn_Pt_Click(object sender, EventArgs e)
        {
            ConfigurarBotoesIdioma("PT");
            DefinirSaudacao("PT");
        }

        private void Btn_En_Click(object sender, EventArgs e)
        {
            ConfigurarBotoesIdioma("EN");
            DefinirSaudacao("EN");
        }

        private void Pic_Foto_Click(object sender, EventArgs e)
        {
            Pnl_Menu.Visible = !Pnl_Menu.Visible;
            Pnl_Menu.BringToFront();
        }

        private void Btn_Sair_Click(object sender, EventArgs e)
        {
            player.controls.stop();
            this.Close();
            Frm_Autenticacao login = new Frm_Autenticacao();
            login.Show();
        }

        private void Btn_EditarDados_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funcionalidade de edição de dados será implementada em breve.");
        }

        private void AbrirFormProjeto(Projetos projeto, int usuarioId)
        {
            this.Hide();
            Frm_Projeto frmProjeto = new Frm_Projeto(projeto, usuarioId);
            frmProjeto.ShowDialog();
            this.Show();
            CarregarMenuLateral();
        }

        private void Lbl_NomeMusica_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Selecione uma música MP3";
                openFileDialog.Filter = "Arquivos MP3|*.mp3";
                openFileDialog.InitialDirectory = musicasPath;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    musicaSelecionada = openFileDialog.FileName;
                    player.controls.stop();
                    player.URL = musicaSelecionada;
                    player.settings.volume = 80;
                    player.controls.play();
                    musicaTocando = true;
                    Lbl_NomeMusica.Text = Path.GetFileNameWithoutExtension(musicaSelecionada);
                }
            }
        }

        // Eventos vazios necessários
        private void Frm_Home_Load(object sender, EventArgs e) { }
        private void Lbl_Taskool_Click(object sender, EventArgs e) { }
        private void Lbl_Titulo_Click(object sender, EventArgs e) { }
    }
}