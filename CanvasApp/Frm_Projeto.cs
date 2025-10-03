using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using CanvasApp.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        // Controle para Frm_TarefasUsuario - RENOMEADO
        private Frm_TarefasUsuario _frmTarefasUsuario;
        private bool tarefasUsuarioAberto = false;

        // Dicionário para controlar formulários de atribuição abertos
        private Dictionary<int, Frm_AtribuirResponsavelTarefa> formulariosAtribuicaoAbertos = new Dictionary<int, Frm_AtribuirResponsavelTarefa>();

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
            ConfigurarBotoesExistentes();
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
            ConfigurarBotoesExistentes();
        }

        private void ConfigurarBotoesExistentes()
        {
            // Configurar eventos para os botões já existentes no formulário
            Btn_MinhasTarefas.Click += BtnTarefasUsuario_Click;

            if (projetoSelecionado != null)
            {
                Btn_GerenciaMembrosProjeto.Click += BtnGerenciarMembros_Click;
            }
            else
            {
                Btn_GerenciaMembrosProjeto.Visible = false;
            }
        }

        private void BtnTarefasUsuario_Click(object sender, EventArgs e)
        {
            AbrirTarefasUsuario();
        }

        private void BtnGerenciarMembros_Click(object sender, EventArgs e)
        {
            AbrirGerenciarMembros();
        }

        private void AbrirTarefasUsuario()
        {
            try
            {
                if (tarefasUsuarioAberto && _frmTarefasUsuario != null && !_frmTarefasUsuario.IsDisposed)
                {
                    _frmTarefasUsuario.BringToFront();
                    return;
                }

                _frmTarefasUsuario = new Frm_TarefasUsuario(usuarioLogadoId);
                _frmTarefasUsuario.FormClosed += (s, args) => { tarefasUsuarioAberto = false; };
                _frmTarefasUsuario.Show();
                tarefasUsuarioAberto = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao abrir gerenciador de tarefas.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbrirGerenciarMembros()
        {
            try
            {
                if (projetoSelecionado == null) return;

                // CORREÇÃO: Comparação correta de tipos
                if (projetoSelecionado.CodUsuario != usuarioLogadoId)
                {
                    MessageBox.Show("Apenas o proprietário do projeto pode gerenciar membros.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // CORREÇÃO: Usar ShowDialog sem using
                Frm_GerenciarMembros frmGerenciarMembros = new Frm_GerenciarMembros(projetoSelecionado.Codigo, usuarioLogadoId, dbMembros, dbTarefas);
                frmGerenciarMembros.ShowDialog(this);

                // Recarregar a interface após gerenciar membros
                AtualizarInterface();
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao abrir gerenciador de membros.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            // Configurar cores claras para o drawer
            Pnl_MenuLateral1.BackColor = Color.FromArgb(250, 250, 250);
            Pnl_MenuLateral1.BorderStyle = BorderStyle.FixedSingle;
            Pnl_MenuLateral1.AutoScroll = false;

            // Configurar PictureBox e Label do título fixos no topo
            pictureBox1.BackColor = Color.Transparent;

            Lbl_TituloMenu.ForeColor = Color.FromArgb(64, 64, 64);
            Lbl_TituloMenu.Font = new Font("Segoe UI", 12, FontStyle.Bold);
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

            // Configurar botão Novo Projeto com cores claras
            Pnl_NovoProjeto.BackColor = Color.FromArgb(230, 230, 230);
            Pnl_NovoProjeto.Cursor = Cursors.Hand;
            Pnl_NovoProjeto.Click += Pnl_NovoProjeto_Click;
            Pnl_NovoProjeto.BorderStyle = BorderStyle.FixedSingle;

            // Configurar ícone e texto do botão Novo Projeto
            Lbl_Novo.ForeColor = Color.FromArgb(64, 64, 64);
            Lbl_Novo.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            Lbl_Novo.Cursor = Cursors.Hand;
            Lbl_Novo.Click += Pnl_NovoProjeto_Click;

            // Carregar itens do menu
            CarregarMenuLateral();
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

        // =========================================================================
        // MÉTODOS AUXILIARES PARA QUANTIDADES NO MENU LATERAL - CORRIGIDOS
        // =========================================================================

        private int ObterQuantidadeFavoritas(int usuarioId)
        {
            try
            {
                return dbFavoritos.ObterTarefasFavoritas(usuarioId).Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int ObterQuantidadeTarefasHoje(int usuarioId)
        {
            try
            {
                return dbTarefas.ObterQuantidadeTarefasComAlarmeHoje(usuarioId);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int ObterQuantidadeTarefasSemana(int usuarioId)
        {
            try
            {
                return dbTarefas.ObterQuantidadeTarefasComAlarmeSemana(usuarioId);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int ObterQuantidadeTarefasMes(int usuarioId)
        {
            try
            {
                return dbTarefas.ObterQuantidadeTarefasComAlarmeMes(usuarioId);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void AbrirFavoritos(int usuarioId)
        {
            try
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
                    MessageBox.Show("Você ainda não marcou nenhuma tarefa como favorita.", "Informação",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao carregar favoritos.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================================
        // MÉTODOS PARA FILTRO POR ALARME NO Frm_Projeto - CORRIGIDOS
        // =========================================================================

        private void CarregarTarefasHoje(int usuarioId)
        {
            try
            {
                var tarefasHoje = dbTarefas.ObterTarefasComAlarmeHoje(usuarioId);

                this.tarefasPendentes = tarefasHoje;
                this.tarefasConcluidas = new List<Projeto_Tarefas>();
                this.isModoFavoritos = false;
                this.projetoSelecionado = null;
                this.isExibindoConcluidas = false;

                Lbl_Titulo.Text = "Tarefas com Alarme para Hoje";
                AtualizarInterface();

                // Mostrar mensagem se não houver tarefas
                if (!tarefasHoje.Any())
                {
                    MessageBox.Show("Nenhuma tarefa com alarme para hoje encontrada.", "Informação",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao carregar tarefas de hoje.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarTarefasSemana(int usuarioId)
        {
            try
            {
                var tarefasSemana = dbTarefas.ObterTarefasComAlarmeSemana(usuarioId);

                this.tarefasPendentes = tarefasSemana;
                this.tarefasConcluidas = new List<Projeto_Tarefas>();
                this.isModoFavoritos = false;
                this.projetoSelecionado = null;
                this.isExibindoConcluidas = false;

                Lbl_Titulo.Text = "Tarefas com Alarme para Esta Semana";
                AtualizarInterface();

                // Mostrar mensagem se não houver tarefas
                if (!tarefasSemana.Any())
                {
                    MessageBox.Show("Nenhuma tarefa com alarme para esta semana encontrada.", "Informação",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao carregar tarefas da semana.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarTarefasMes(int usuarioId)
        {
            try
            {
                var tarefasMes = dbTarefas.ObterTarefasComAlarmeMes(usuarioId);

                this.tarefasPendentes = tarefasMes;
                this.tarefasConcluidas = new List<Projeto_Tarefas>();
                this.isModoFavoritos = false;
                this.projetoSelecionado = null;
                this.isExibindoConcluidas = false;

                Lbl_Titulo.Text = "Tarefas com Alarme para Este Mês";
                AtualizarInterface();

                // Mostrar mensagem se não houver tarefas
                if (!tarefasMes.Any())
                {
                    MessageBox.Show("Nenhuma tarefa com alarme para este mês encontrada.", "Informação",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao carregar tarefas do mês.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            Lst_ListaTarefas.AutoScroll = true;
            Lbl_Triste.Text = "Nenhuma tarefa criada";

            Txt_Tarefa.Text = "Adicionar uma tarefa...";
            Txt_Tarefa.ForeColor = SystemColors.GrayText;

            // Configurar título do grupo de status
            Grp_Status.Text = "Status do Projeto";

            // REMOVA estas linhas se os eventos já estiverem configurados no designer
            Txt_Tarefa.Enter += Txt_Tarefa_Enter;
            Txt_Tarefa.Leave += Txt_Tarefa_Leave;
            Txt_Tarefa.KeyDown += Txt_Tarefa_KeyDown;

            AtualizarInterface();
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
        }

        private void AtualizarStatusGrafico()
        {
            // Usar os controles existentes do designer
            Lbl_Status.Visible = true;
            Lbl_Pendentes.Visible = true;
            Lbl_Concluidas.Visible = true;
            Flw_Pendentes.Visible = true;
            Flw_Concluidos.Visible = true;

            // Limpar os flow layouts existentes
            Flw_Pendentes.Controls.Clear();
            Flw_Concluidos.Controls.Clear();

            Chrt_Tarefas.Series.Clear();
            var totalTarefas = tarefasPendentes.Count + tarefasConcluidas.Count;

            // Configurar o gráfico
            if (totalTarefas > 0)
            {
                Series seriePizza = new Series("StatusTarefas") { ChartType = SeriesChartType.Pie };

                if (tarefasPendentes.Count > 0)
                {
                    var pontoPendente = seriePizza.Points.Add(tarefasPendentes.Count);
                    pontoPendente.Color = Color.FromArgb(74, 124, 255);
                    pontoPendente.LegendText = "Pendentes";
                    pontoPendente.Label = $"{tarefasPendentes.Count}";
                }

                if (tarefasConcluidas.Count > 0)
                {
                    var pontoConcluido = seriePizza.Points.Add(tarefasConcluidas.Count);
                    pontoConcluido.Color = Color.FromArgb(50, 200, 100);
                    pontoConcluido.LegendText = "Concluídas";
                    pontoConcluido.Label = $"{tarefasConcluidas.Count}";
                }

                Chrt_Tarefas.Series.Add(seriePizza);

                // Configurações do gráfico
                Chrt_Tarefas.Titles.Clear();
                Chrt_Tarefas.Legends.Clear();

                seriePizza.IsValueShownAsLabel = true;
                seriePizza.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                seriePizza.LabelForeColor = Color.White;

                Chrt_Tarefas.ChartAreas[0].Area3DStyle.Enable3D = true;
            }
            else
            {
                Chrt_Tarefas.Series.Clear();
            }

            // Preencher os colaboradores
            if (projetoSelecionado != null)
            {
                var colaboradores = dbMembros.ObterMembrosProjeto(projetoSelecionado.Codigo);
                if (colaboradores.Any())
                {
                    // Lista de cores
                    List<Color> coresDisponiveis = new List<Color>
                    {
                        Color.FromArgb(74, 124, 255),
                        Color.FromArgb(255, 87, 87),
                        Color.FromArgb(50, 200, 100),
                        Color.FromArgb(255, 160, 0),
                        Color.FromArgb(160, 90, 255),
                        Color.FromArgb(0, 200, 200),
                        Color.FromArgb(255, 100, 200),
                        Color.FromArgb(139, 69, 19)
                    };

                    int corIndex = 0;

                    foreach (var colab in colaboradores)
                    {
                        // Verificar status do colaborador
                        bool temTarefasPendentes = tarefasPendentes.Any(t => t.CodUsuario == colab.Codigo);
                        bool temTarefasConcluidas = tarefasConcluidas.Any(t => t.CodUsuario == colab.Codigo);
                        bool vaiParaConcluidos = !temTarefasPendentes && temTarefasConcluidas;

                        Color corColaborador = coresDisponiveis[corIndex % coresDisponiveis.Count];
                        corIndex++;

                        // Criar círculo do colaborador
                        var circuloColab = CriarCirculoColaborador(colab.Nome, corColaborador);

                        if (vaiParaConcluidos)
                        {
                            Flw_Concluidos.Controls.Add(circuloColab);
                        }
                        else if (temTarefasPendentes)
                        {
                            Flw_Pendentes.Controls.Add(circuloColab);
                        }
                    }
                }
                else
                {
                    // Caso não haja colaboradores
                    Label lblSemColaboradores = new Label();
                    lblSemColaboradores.Text = "Nenhum colaborador";
                    lblSemColaboradores.Font = new Font("Segoe UI", 8, FontStyle.Italic);
                    lblSemColaboradores.ForeColor = Color.Gray;
                    lblSemColaboradores.AutoSize = true;
                    Flw_Pendentes.Controls.Add(lblSemColaboradores);
                }
            }
        }

        // Método auxiliar para criar círculo de colaborador
        private Panel CriarCirculoColaborador(string nome, Color cor)
        {
            var panelCirculo = new Panel
            {
                Width = 25,
                Height = 25,
                BackColor = cor,
                Margin = new Padding(2)
            };

            // Tornar o painel circular
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, panelCirculo.Width, panelCirculo.Height);
            panelCirculo.Region = new Region(path);

            // Label com a inicial
            var lblInicial = new Label
            {
                Text = ObterInicialUsuario(nome),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            // Tooltip com nome completo
            var toolTip = new ToolTip();
            toolTip.SetToolTip(panelCirculo, nome);
            toolTip.SetToolTip(lblInicial, nome);
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 500;

            panelCirculo.Controls.Add(lblInicial);
            return panelCirculo;
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
            try
            {
                var panel = new Panel
                {
                    Width = Lst_ListaTarefas.Width - 25,
                    Height = 60,
                    Margin = new Padding(5),
                    BackColor = Color.White,
                    Tag = tarefa,
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Hand
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
                    Width = panel.Width - 150,
                    Height = 20,
                    Location = new Point(40, 13),
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand
                };

                if (isConcluida)
                {
                    labelDescricao.Font = new Font(labelDescricao.Font, FontStyle.Strikeout);
                    labelDescricao.ForeColor = Color.Gray;
                }

                panel.Controls.Add(labelDescricao);

                // --- PRIMEIRO: ÍCONE DE FAVORITO ---
                bool estaFavoritado = dbFavoritos.EhFavorito(usuarioLogadoId, tarefa.Codigo);

                var picEstrela = new PictureBox
                {
                    Width = 20,
                    Height = 20,
                    Location = new Point(panel.Width - 30, 15),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Cursor = Cursors.Hand,
                    Tag = new { EstaFavoritado = estaFavoritado, Tarefa = tarefa }
                };

                picEstrela.Image = estaFavoritado ?
                    Properties.Resources.estrela__1_ :
                    Properties.Resources.estrela;

                picEstrela.Click += (sender, e) =>
                {
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

                // --- SEGUNDO: RESPONSÁVEIS (à esquerda do favorito) ---
                int posicaoResponsaveis = panel.Width - 60;

                try
                {
                    // Obter responsáveis da tarefa
                    var responsaveis = new List<Usuario>();

                    // Se a tarefa tem um usuário atribuído, adicionar como responsável
                    if (!string.IsNullOrEmpty(tarefa.CodUsuario))
                    {
                        var usuarioResponsavel = dbUsuario.ObterUsuarioPorCodigo(tarefa.CodUsuario);
                        if (usuarioResponsavel != null)
                        {
                            responsaveis.Add(usuarioResponsavel);
                        }
                    }

                    // Adicionar círculos dos responsáveis DA DIREITA PARA ESQUERDA
                    if (responsaveis.Any())
                    {
                        // Ordenar para consistência na exibição
                        foreach (var responsavel in responsaveis.OrderBy(r => r.Nome))
                        {
                            var circuloResponsavel = CriarCirculoResponsavel(responsavel);
                            circuloResponsavel.Location = new Point(posicaoResponsaveis, 15);
                            panel.Controls.Add(circuloResponsavel);

                            // Mover para a esquerda para o próximo responsável
                            posicaoResponsaveis -= 30;
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignorar erro
                }

                // --- DATA DO ALARME (se houver) ---
                try
                {
                    var alarme = dbAlarme.ObterAlarmePorTarefa(tarefa.Codigo);
                    if (alarme != null)
                    {
                        var labelDataAlarme = new Label
                        {
                            Text = $"Alarme: {alarme.Data:dd/MM/yyyy} às {alarme.Hora:HH:mm}",
                            AutoSize = true,
                            Location = new Point(40, 35),
                            Font = new Font("Segoe UI", 8, FontStyle.Italic),
                            ForeColor = Color.Gray,
                            Cursor = Cursors.Hand
                        };
                        panel.Controls.Add(labelDataAlarme);

                        // Evento de clique na data do alarme
                        labelDataAlarme.Click += (sender, e) => AbrirDetalhesTarefa(tarefa);
                    }
                }
                catch (Exception)
                {
                    // Ignorar erro
                }

                // --- EVENTOS DE CLIQUE ---
                panel.Click += (sender, e) => AbrirDetalhesTarefa(tarefa);
                labelDescricao.Click += (sender, e) => AbrirDetalhesTarefa(tarefa);

                // --- EVENTOS DE DUPLO CLIQUE - AGORA ABRE Frm_AtribuirResponsavelTarefa ---
                panel.DoubleClick += (sender, e) => AbrirAtribuirResponsavel(tarefa);
                labelDescricao.DoubleClick += (sender, e) => AbrirAtribuirResponsavel(tarefa);

                return panel;
            }
            catch (Exception)
            {
                return new Panel { Width = Lst_ListaTarefas.Width - 25, Height = 60 };
            }
        }

        // --- NOVO MÉTODO PARA ABRIR FORMULÁRIO DE ATRIBUIR RESPONSÁVEL ---
        private void AbrirAtribuirResponsavel(Projeto_Tarefas tarefa)
        {
            try
            {
                // Verificar se o formulário de atribuição já está aberto usando o dicionário
                if (formulariosAtribuicaoAbertos.ContainsKey(tarefa.Codigo))
                {
                    var formExistente = formulariosAtribuicaoAbertos[tarefa.Codigo];
                    if (formExistente != null && !formExistente.IsDisposed)
                    {
                        formExistente.BringToFront();
                        return;
                    }
                    else
                    {
                        // Remover do dicionário se o formulário foi fechado
                        formulariosAtribuicaoAbertos.Remove(tarefa.Codigo);
                    }
                }

                // Se não estiver aberto, criar novo formulário
                Frm_AtribuirResponsavelTarefa novoForm = new Frm_AtribuirResponsavelTarefa(tarefa);

                // Adicionar evento para remover do dicionário quando o formulário for fechado
                novoForm.FormClosed += (s, e) =>
                {
                    if (formulariosAtribuicaoAbertos.ContainsKey(tarefa.Codigo))
                    {
                        formulariosAtribuicaoAbertos.Remove(tarefa.Codigo);
                    }
                };

                novoForm.Show();

                // Adicionar ao dicionário de formulários abertos
                formulariosAtribuicaoAbertos[tarefa.Codigo] = novoForm;
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao abrir formulário de atribuição de responsável.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- MÉTODO AUXILIAR PARA CRIAR CÍRCULO DO RESPONSÁVEL ---
        private Panel CriarCirculoResponsavel(Usuario usuario)
        {
            var panelCirculo = new Panel
            {
                Width = 25,
                Height = 25,
                BackColor = ObterCorAleatoriaResponsavel(usuario.Codigo)
            };

            // Tornar o painel circular
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, panelCirculo.Width, panelCirculo.Height);
            panelCirculo.Region = new Region(path);

            // Label com a inicial
            var lblInicial = new Label
            {
                Text = ObterInicialUsuario(usuario.Nome),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            // Tooltip com nome completo
            var toolTip = new ToolTip();
            toolTip.SetToolTip(panelCirculo, usuario.Nome);
            toolTip.SetToolTip(lblInicial, usuario.Nome);

            panelCirculo.Controls.Add(lblInicial);

            return panelCirculo;
        }

        // --- MÉTODOS AUXILIARES PARA CORES E INICIAIS ---
        private string ObterInicialUsuario(string nome)
        {
            if (string.IsNullOrEmpty(nome)) return "?";
            return nome.Substring(0, 1).ToUpper();
        }

        private Color ObterCorAleatoriaResponsavel(string seed)
        {
            // Usar o código do usuário como seed para cor consistente
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

        private void Lbl_Pendentes_Click(object sender, EventArgs e) { }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            // Fechar Frm_TarefasUsuario se estiver aberto
            if (_frmTarefasUsuario != null && !_frmTarefasUsuario.IsDisposed)
            {
                _frmTarefasUsuario.Close();
            }

            // Fechar todos os formulários de atribuição abertos
            foreach (var form in formulariosAtribuicaoAbertos.Values)
            {
                if (form != null && !form.IsDisposed)
                {
                    form.Close();
                }
            }
            formulariosAtribuicaoAbertos.Clear();
        }
    }
}