using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using CanvasApp.Classes.ManipulaçãoDados;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CanvasApp.Forms
{
    public partial class Frm_PlanningPoker : Form
    {
        private GerenciadorEsforco _gerenciadorEsforco;
        private List<JogadorCarta> _cartasEscolhidas;
        private List<Projeto_Tarefas> _tarefasDisponiveis;
        private TarefasDB _tarefasDB;
        private ProjetosDB _projetosDB;
        private int _quantidadeJogadores;
        private bool _jogoIniciado = false;
        private List<Panel> _panelsCartas;
        private FlowLayoutPanel _pnlCartasEscolhidas;

        public Frm_PlanningPoker()
        {
            InitializeComponent();
            _gerenciadorEsforco = new GerenciadorEsforco();
            _cartasEscolhidas = new List<JogadorCarta>();
            _tarefasDB = new TarefasDB();
            _projetosDB = new ProjetosDB();
            _panelsCartas = new List<Panel>();

            CriarPainelCartasEscolhidas();
            InicializarComponentes();
            CarregarTarefas(); // Carregar tarefas após inicialização
        }

        private void CriarPainelCartasEscolhidas()
        {
            _pnlCartasEscolhidas = new FlowLayoutPanel();
            _pnlCartasEscolhidas.Location = new Point(450, 320);
            _pnlCartasEscolhidas.Size = new Size(250, 90);
            _pnlCartasEscolhidas.BackColor = Color.Transparent;
            _pnlCartasEscolhidas.AutoScroll = true;
            _pnlCartasEscolhidas.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(_pnlCartasEscolhidas);

            var lblTitulo = new Label();
            lblTitulo.Text = "Cartas Escolhidas:";
            lblTitulo.Font = new Font("Arial", 9, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(450, 300);
            lblTitulo.AutoSize = true;
            this.Controls.Add(lblTitulo);
        }

        private void InicializarComponentes()
        {
            _panelsCartas = new List<Panel>
            {
                Pnl_Card1, Pnl_Card2, Pnl_Card3, Pnl_Card4,
                Pnl_Card5, Pnl_Card6, Pnl_Card7, Pnl_Card8
            };

            int[] valoresCartas = { 1, 2, 3, 5, 8, 13, 21, 34 };
            for (int i = 0; i < _panelsCartas.Count && i < valoresCartas.Length; i++)
            {
                _panelsCartas[i].Tag = valoresCartas[i];
                _panelsCartas[i].BackColor = Color.White;

                _panelsCartas[i].Controls.Clear();

                var lblValor = new Label();
                lblValor.Text = valoresCartas[i].ToString();
                lblValor.Font = new Font("Arial", 16, FontStyle.Bold);
                lblValor.ForeColor = Color.Blue;
                lblValor.TextAlign = ContentAlignment.MiddleCenter;
                lblValor.Dock = DockStyle.Fill;
                _panelsCartas[i].Controls.Add(lblValor);
            }

            DesabilitarCartas();

            Btn_Play.Click += Btn_Play_Click;
            Btn_Resultado.Click += Btn_Resultado_Click;
            Chb_Tarefas.SelectedIndexChanged += Chb_Tarefas_SelectedIndexChanged;

            foreach (var panel in _panelsCartas)
            {
                panel.Click += PanelCarta_Click;
                foreach (Control control in panel.Controls)
                {
                    control.Click += PanelCarta_Click;
                }
            }

            _quantidadeJogadores = 3;
            AtualizarInterface();
        }

        private void CarregarTarefas()
        {
            try
            {
                // Verificar se há usuário logado
                if (UsuarioSessao.Codigo <= 0)
                {
                    Lbl_NomeProjeto.Text = "Projeto Demo";
                    CarregarTarefasDemo();
                    return;
                }

                // Obter todos os projetos do usuário
                var projetos = _projetosDB.ObterTodosProjetosUsuario(UsuarioSessao.Codigo);

                if (projetos != null && projetos.Any())
                {
                    var projetoAtual = projetos.First();
                    Lbl_NomeProjeto.Text = projetoAtual.Nome;

                    // Carregar tarefas não concluídas do projeto
                    _tarefasDisponiveis = _tarefasDB.ObterTarefasPorProjeto(projetoAtual.Codigo)
                        .Where(t => !t.isConcluida)
                        .OrderBy(t => t.Descricao)
                        .ToList();

                    if (_tarefasDisponiveis.Any())
                    {
                        Chb_Tarefas.DataSource = _tarefasDisponiveis;
                        Chb_Tarefas.DisplayMember = "Descricao";
                        Chb_Tarefas.ValueMember = "Codigo";
                        Lbl_Tarefa.Text = "Tarefa: " + _tarefasDisponiveis.First().Descricao;
                    }
                    else
                    {
                        CarregarTarefasDemo();
                    }
                }
                else
                {
                    // Se não houver projetos, carregar tarefas do usuário
                    _tarefasDisponiveis = _tarefasDB.ObterTarefasPorUsuario(UsuarioSessao.Codigo)
                        .Where(t => !t.isConcluida)
                        .OrderBy(t => t.Descricao)
                        .ToList();

                    if (_tarefasDisponiveis.Any())
                    {
                        Chb_Tarefas.DataSource = _tarefasDisponiveis;
                        Chb_Tarefas.DisplayMember = "Descricao";
                        Chb_Tarefas.ValueMember = "Codigo";
                        Lbl_Tarefa.Text = "Tarefa: " + _tarefasDisponiveis.First().Descricao;
                    }
                    else
                    {
                        CarregarTarefasDemo();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar tarefas: {ex.Message}\nCarregando tarefas demo...",
                              "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CarregarTarefasDemo();
            }
        }

        private void CarregarTarefasDemo()
        {
            // Tarefas demo para teste
            _tarefasDisponiveis = new List<Projeto_Tarefas>
            {
                new Projeto_Tarefas { Codigo = 1, Descricao = "Criar interface do usuário", isConcluida = false },
                new Projeto_Tarefas { Codigo = 2, Descricao = "Implementar funcionalidade de login", isConcluida = false },
                new Projeto_Tarefas { Codigo = 3, Descricao = "Desenvolver módulo de relatórios", isConcluida = false },
                new Projeto_Tarefas { Codigo = 4, Descricao = "Testar sistema completo", isConcluida = false },
                new Projeto_Tarefas { Codigo = 5, Descricao = "Documentar API", isConcluida = false }
            };

            Chb_Tarefas.DataSource = _tarefasDisponiveis;
            Chb_Tarefas.DisplayMember = "Descricao";
            Chb_Tarefas.ValueMember = "Codigo";
            Lbl_Tarefa.Text = "Tarefa: " + _tarefasDisponiveis.First().Descricao;

            Lbl_NomeProjeto.Text = "Projeto Demo";
        }

        private void DesabilitarCartas()
        {
            foreach (var panel in _panelsCartas)
            {
                panel.Enabled = false;
                panel.BackColor = Color.LightGray;
                panel.Cursor = Cursors.No;
            }
        }

        private void HabilitarCartas()
        {
            foreach (var panel in _panelsCartas)
            {
                panel.Enabled = true;
                panel.BackColor = Color.White;
                panel.Cursor = Cursors.Hand;
            }
        }

        private void Btn_Play_Click(object sender, EventArgs e)
        {
            if (Chb_Tarefas.SelectedItem == null || _tarefasDisponiveis.Count == 0)
            {
                MessageBox.Show("Selecione uma tarefa para estimar.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _quantidadeJogadores = 3;
            _jogoIniciado = true;
            _cartasEscolhidas.Clear();
            LimparCartasEscolhidas();

            HabilitarCartas();
            Btn_Play.Enabled = false;
            Btn_Resultado.Enabled = false;

            AtualizarInterface();

            MessageBox.Show("Jogo iniciado! Clique nas cartas para fazer suas estimativas.\n\nCada clique representa um jogador diferente.",
                          "Planning Poker Iniciado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PanelCarta_Click(object sender, EventArgs e)
        {
            if (!_jogoIniciado) return;

            Panel panelCarta = null;
            if (sender is Panel)
                panelCarta = (Panel)sender;
            else if (sender is Control)
                panelCarta = ((Control)sender).Parent as Panel;

            if (panelCarta != null && panelCarta.Enabled)
            {
                int valorCarta = Convert.ToInt32(panelCarta.Tag);
                RegistrarCartaEscolhida(valorCarta);
            }
        }

        private void RegistrarCartaEscolhida(int valorCarta)
        {
            int numeroJogador = _cartasEscolhidas.Count + 1;

            var carta = new JogadorCarta
            {
                NomeJogador = $"Jogador {numeroJogador}",
                ValorCarta = valorCarta,
                DataEscolha = DateTime.Now
            };

            _cartasEscolhidas.Add(carta);
            AdicionarCartaEscolhidaUI(carta);

            if (_cartasEscolhidas.Count >= _quantidadeJogadores)
            {
                DesabilitarCartas();
                Btn_Resultado.Enabled = true;
                MessageBox.Show($"Todos os {_quantidadeJogadores} jogadores escolheram suas cartas!\n\nClique em 'Resultado' para ver a estimativa final.",
                              "Todas as Cartas Escolhidas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            AtualizarInterface();
        }

        private void AdicionarCartaEscolhidaUI(JogadorCarta carta)
        {
            var panelCartaEscolhida = new Panel();
            panelCartaEscolhida.Size = new Size(50, 70);
            panelCartaEscolhida.BorderStyle = BorderStyle.FixedSingle;
            panelCartaEscolhida.BackColor = Color.White;
            panelCartaEscolhida.Margin = new Padding(3);

            var lblJogador = new Label();
            lblJogador.Text = carta.NomeJogador;
            lblJogador.Font = new Font("Arial", 6, FontStyle.Regular);
            lblJogador.ForeColor = Color.DarkGray;
            lblJogador.Location = new Point(2, 2);
            lblJogador.AutoSize = true;

            var lblValor = new Label();
            lblValor.Text = carta.ValorCarta.ToString();
            lblValor.Font = new Font("Arial", 12, FontStyle.Bold);
            lblValor.ForeColor = Color.Blue;
            lblValor.TextAlign = ContentAlignment.MiddleCenter;
            lblValor.Dock = DockStyle.Fill;

            panelCartaEscolhida.Controls.Add(lblJogador);
            panelCartaEscolhida.Controls.Add(lblValor);

            _pnlCartasEscolhidas.Controls.Add(panelCartaEscolhida);
        }

        private void LimparCartasEscolhidas()
        {
            _pnlCartasEscolhidas.Controls.Clear();
        }

        private void Btn_Resultado_Click(object sender, EventArgs e)
        {
            if (_cartasEscolhidas.Count == 0)
            {
                MessageBox.Show("Nenhuma carta foi escolhida. Clique no botão 'Play' para iniciar o jogo.",
                              "Nenhuma Carta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CalcularResultado();
        }

        private void CalcularResultado()
        {
            try
            {
                double mediaCartas = _cartasEscolhidas.Average(c => c.ValorCarta);
                double diasAdicionais = _gerenciadorEsforco.CalcularDiasEntrega(mediaCartas);
                DateTime dataEntrega = DateTime.Now.AddDays(diasAdicionais);

                Lbl_DataEntrega.Text = dataEntrega.ToString("dd/MM/yyyy");
                Lbl_Numero.Text = _cartasEscolhidas.Count.ToString();

                string detalhesCartas = string.Join(", ", _cartasEscolhidas.Select(c => $"{c.NomeJogador}: {c.ValorCarta}"));
                string resultado = $"📊 **RESULTADO DO PLANNING POKER**\n\n" +
                                 $"📋 Tarefa: {((Projeto_Tarefas)Chb_Tarefas.SelectedItem)?.Descricao}\n" +
                                 $"🎴 Média das cartas: {mediaCartas:F2}\n" +
                                 $"📅 Dias estimados: {diasAdicionais} dias\n" +
                                 $"📅 Data de entrega: {dataEntrega:dd/MM/yyyy}\n\n" +
                                 $"👥 Cartas escolhidas:\n{detalhesCartas}\n\n" +
                                 $"⏰ Calculado em: {DateTime.Now:HH:mm:ss}";

                MessageBox.Show(resultado, "🎯 Resultado do Planning Poker",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);

                _jogoIniciado = false;
                Btn_Play.Enabled = true;
                Btn_Resultado.Enabled = false;

                AtualizarInterface();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Erro ao calcular resultado: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarInterface()
        {
            Lbl_Numero.Text = $"Jogadores: {_cartasEscolhidas.Count}/{_quantidadeJogadores}";
            Btn_Resultado.Enabled = _cartasEscolhidas.Count >= _quantidadeJogadores;

            if (Chb_Tarefas.SelectedItem is Projeto_Tarefas tarefa)
            {
                Lbl_Tarefa.Text = $"Tarefa: {tarefa.Descricao}";
            }

            if (_cartasEscolhidas.Count == 0)
            {
                Lbl_DataEntrega.Text = "--/--/----";
            }
        }

        private void Chb_Tarefas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Chb_Tarefas.SelectedItem is Projeto_Tarefas tarefa)
            {
                Lbl_Tarefa.Text = $"Tarefa: {tarefa.Descricao}";
            }
        }

        // Método para recarregar tarefas
        public void RecarregarTarefas()
        {
            CarregarTarefas();
            MessageBox.Show("Tarefas recarregadas com sucesso!", "Tarefas Atualizadas",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ConfigurarQuantidadeJogadores(int quantidade)
        {
            if (quantidade > 0 && quantidade <= 10)
            {
                _quantidadeJogadores = quantidade;
                AtualizarInterface();
            }
        }

        public void RecarregarRegras()
        {
            _gerenciadorEsforco.CarregarConfiguracoes();
            MessageBox.Show("Regras do Planning Poker recarregadas com sucesso!", "Regras Atualizadas",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ReiniciarJogo()
        {
            _jogoIniciado = false;
            _cartasEscolhidas.Clear();
            LimparCartasEscolhidas();
            DesabilitarCartas();
            Btn_Play.Enabled = true;
            Btn_Resultado.Enabled = false;
            AtualizarInterface();
        }

        public void DefinirProjeto(int codigoProjeto, string nomeProjeto)
        {
            Lbl_NomeProjeto.Text = nomeProjeto;
            CarregarTarefas();
        }

        // Métodos de Paint (mantidos para compatibilidade)
        private void Pnl_Card1_Paint(object sender, PaintEventArgs e) { }
        private void Pnl_Card2_Paint(object sender, PaintEventArgs e) { }
        private void Pnl_Card3_Paint(object sender, PaintEventArgs e) { }
        private void Pnl_Card4_Paint(object sender, PaintEventArgs e) { }
        private void Pnl_Card5_Paint(object sender, PaintEventArgs e) { }
        private void Pnl_Card6_Paint(object sender, PaintEventArgs e) { }
        private void Pnl_Card7_Paint(object sender, PaintEventArgs e) { }
        private void panel9_Paint(object sender, PaintEventArgs e) { }
    }
}