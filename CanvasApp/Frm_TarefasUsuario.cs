using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CanvasApp.Forms
{
    public partial class Frm_TarefasUsuario : Form
    {
        private int _usuarioId;
        private TarefasDB _tarefasDB;
        private List<Projeto_Tarefas> _todasTarefas;
        private List<Projeto_Tarefas> _tarefasFiltradas;
        private AlarmeDB _alarmeDB;

        public Frm_TarefasUsuario(int usuarioId)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            _tarefasDB = new TarefasDB();
            _alarmeDB = new AlarmeDB();

            ConfigurarInterface();
            CarregarTarefas();
        }

        private void ConfigurarInterface()
        {
            // Configuração básica do formulário
            this.Text = $"Minhas Tarefas | Taskool";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Configurar eventos dos botões que já existem no designer
            ConfigurarEventosBotoes();
        }

        private void ConfigurarEventosBotoes()
        {
            // Configurar eventos para os botões de filtro que já existem no designer
            Btn_Hoje.Click += (s, e) => AplicarFiltro("hoje");
            Btn_Semana.Click += (s, e) => AplicarFiltro("semana");
            Btn_Mes.Click += (s, e) => AplicarFiltro("mes");
            Btn_Todas.Click += (s, e) => AplicarFiltro("todas");
            Btn_Exportar.Click += Btn_ExportarPDF_Click;
            Btn_Fechar.Click += (s, e) => this.Close();

            // Configurar tags e cores iniciais dos botões
            Btn_Hoje.Tag = "hoje";
            Btn_Semana.Tag = "semana";
            Btn_Mes.Tag = "mes";
            Btn_Todas.Tag = "todas";

            Btn_Hoje.BackColor = Color.FromArgb(180, 220, 255);
            Btn_Semana.BackColor = Color.FromArgb(240, 240, 240);
            Btn_Mes.BackColor = Color.FromArgb(240, 240, 240);
            Btn_Todas.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void CarregarTarefas()
        {
            try
            {
                _todasTarefas = _tarefasDB.ObterTarefasPorUsuario(_usuarioId);
                AplicarFiltro("todas");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar tarefas: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DateTime? ObterDataAlarmeTarefa(Projeto_Tarefas tarefa)
        {
            try
            {
                var alarme = _alarmeDB.ObterAlarmePorTarefa(tarefa.Codigo);
                return alarme?.Data;
            }
            catch
            {
                return null;
            }
        }

        private void AplicarFiltro(string filtro)
        {
            if (_todasTarefas == null) return;

            DateTime hoje = DateTime.Today;
            DateTime inicioSemana = hoje.AddDays(-(int)hoje.DayOfWeek);
            DateTime fimSemana = inicioSemana.AddDays(6);
            DateTime inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            DateTime fimMes = inicioMes.AddMonths(1).AddDays(-1);

            switch (filtro)
            {
                case "hoje":
                    _tarefasFiltradas = _todasTarefas
                        .Where(t =>
                        {
                            var dataAlarme = ObterDataAlarmeTarefa(t);
                            return dataAlarme.HasValue && dataAlarme.Value.Date == hoje;
                        })
                        .ToList();
                    break;
                case "semana":
                    _tarefasFiltradas = _todasTarefas
                        .Where(t =>
                        {
                            var dataAlarme = ObterDataAlarmeTarefa(t);
                            return dataAlarme.HasValue &&
                                   dataAlarme.Value.Date >= inicioSemana &&
                                   dataAlarme.Value.Date <= fimSemana;
                        })
                        .ToList();
                    break;
                case "mes":
                    _tarefasFiltradas = _todasTarefas
                        .Where(t =>
                        {
                            var dataAlarme = ObterDataAlarmeTarefa(t);
                            return dataAlarme.HasValue &&
                                   dataAlarme.Value.Date >= inicioMes &&
                                   dataAlarme.Value.Date <= fimMes;
                        })
                        .ToList();
                    break;
                case "todas":
                default:
                    _tarefasFiltradas = _todasTarefas;
                    break;
            }

            AtualizarBotoesFiltro(filtro);
            ExibirTarefas();
        }

        private void AtualizarBotoesFiltro(string filtroAtivo)
        {
            // Atualizar apenas os botões de filtro (que têm Tag)
            foreach (Control control in Pnl_Filtros.Controls)
            {
                if (control is Button btn && btn.Tag != null)
                {
                    btn.BackColor = btn.Tag.ToString() == filtroAtivo ?
                        Color.FromArgb(180, 220, 255) :
                        Color.FromArgb(240, 240, 240);
                }
            }
        }

        private void ExibirTarefas()
        {
            Flw_Tarefas.Controls.Clear();

            if (!_tarefasFiltradas.Any())
            {
                var lblSemTarefas = new Label
                {
                    Text = "Nenhuma tarefa encontrada",
                    Font = new Font("Segoe UI", 12, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    Size = new Size(700, 100),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                Flw_Tarefas.Controls.Add(lblSemTarefas);
                return;
            }

            var tarefasPorProjeto = _tarefasFiltradas
                .GroupBy(t => t.CodProjeto)
                .OrderBy(g => g.Key);

            foreach (var grupo in tarefasPorProjeto)
            {
                AdicionarGrupoProjeto(Flw_Tarefas, grupo.Key, grupo.ToList());
            }
        }

        private void AdicionarGrupoProjeto(FlowLayoutPanel container, int codProjeto, List<Projeto_Tarefas> tarefas)
        {
            try
            {
                var nomeProjeto = ObterNomeProjeto(codProjeto);

                var groupBox = new GroupBox
                {
                    Text = $"{nomeProjeto} ({tarefas.Count} tarefas)",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(64, 64, 64),
                    Size = new Size(700, 40 + (tarefas.Count * 55)),
                    Margin = new Padding(5)
                };

                var pnlConteudo = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = false
                };

                int yPos = 10;
                foreach (var tarefa in tarefas.OrderBy(t => ObterDataAlarmeTarefa(t)))
                {
                    var pnlTarefa = CriarPanelTarefa(tarefa);
                    pnlTarefa.Location = new Point(10, yPos);
                    pnlConteudo.Controls.Add(pnlTarefa);
                    yPos += pnlTarefa.Height + 5;
                }

                groupBox.Controls.Add(pnlConteudo);
                container.Controls.Add(groupBox);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar grupo de projeto: {ex.Message}");
            }
        }

        private Panel CriarPanelTarefa(Projeto_Tarefas tarefa)
        {
            var panel = new Panel
            {
                Width = 680,
                Height = 50,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Checkbox de conclusão
            var chkConcluida = new CheckBox
            {
                Checked = tarefa.isConcluida,
                Location = new Point(10, 15),
                Size = new Size(20, 20),
                Enabled = false
            };
            panel.Controls.Add(chkConcluida);

            // Descrição da tarefa
            var lblDescricao = new Label
            {
                Text = tarefa.Descricao,
                Location = new Point(40, 10),
                Size = new Size(500, 20),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleLeft
            };
            if (tarefa.isConcluida)
            {
                lblDescricao.Font = new Font(lblDescricao.Font, FontStyle.Strikeout);
                lblDescricao.ForeColor = Color.Gray;
            }
            panel.Controls.Add(lblDescricao);

            // Data do alarme
            var alarme = _alarmeDB.ObterAlarmePorTarefa(tarefa.Codigo);
            if (alarme != null)
            {
                var lblData = new Label
                {
                    Text = $"{alarme.Data:dd/MM/yyyy} {alarme.Hora:HH:mm}",
                    Location = new Point(40, 30),
                    Size = new Size(150, 15),
                    Font = new Font("Segoe UI", 8),
                    ForeColor = Color.Gray
                };
                panel.Controls.Add(lblData);
            }

            return panel;
        }

        private string ObterNomeProjeto(int codProjeto)
        {
            try
            {
                var projetosDB = new ProjetosDB();
                var projeto = projetosDB.ObterProjetoPorCodigo(codProjeto);
                return projeto?.Nome ?? "Projeto Desconhecido";
            }
            catch
            {
                return "Projeto Desconhecido";
            }
        }

        private void Btn_ExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (_tarefasFiltradas == null || !_tarefasFiltradas.Any())
                {
                    MessageBox.Show("Não há tarefas para exportar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Implementar exportação PDF aqui
                MessageBox.Show("Funcionalidade de exportação PDF em desenvolvimento", "Informação",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar PDF: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}