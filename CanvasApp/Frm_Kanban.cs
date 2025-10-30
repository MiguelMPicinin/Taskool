using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using CanvasApp.Classes.ManipulaçãoDados;
using CanvasApp.Formularios_Pop_Ups;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CanvasApp
{
    public partial class Frm_Kanban : Form
    {
        private int codProjeto;
        private string nomeProjeto;
        private List<Projeto_Tarefas> tarefas;
        private TarefasDB tarefaDB;
        private TarefasHistoricoDB historicoDB;

        private readonly Dictionary<string, string> coresPostIt = new Dictionary<string, string>
        {
            { "Amarelo", "#ffe079" },
            { "Rosa", "#f097ca" },
            { "Verde", "#98d366" },
            { "Azul", "#82d3e5" }
        };

        public Frm_Kanban(int codProjeto, string nomeProjeto)
        {
            InitializeComponent();
            this.codProjeto = codProjeto;
            this.nomeProjeto = nomeProjeto;
            this.tarefaDB = new TarefasDB();
            this.historicoDB = new TarefasHistoricoDB();

            Text = $"Quadro Kanban - {nomeProjeto}";
            CarregarTarefas();
            ConfigurarDragDrop();
            ConfigurarBotoesAdicionar();
        }

        private void ConfigurarBotoesAdicionar()
        {
            // Configurar os labels de adicionar para parecerem botões
            var labelsAdicionar = new[] { Lbl_Adicionar1, Lbl_Adicionar2, Lbl_Adicionar3 };

            foreach (var label in labelsAdicionar)
            {
                label.Cursor = Cursors.Hand;
                label.BackColor = Color.LightGray;
                label.BorderStyle = BorderStyle.FixedSingle;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Padding = new Padding(5);
            }
        }

        private void CarregarTarefas()
        {
            tarefas = tarefaDB.ListarTarefasPorProjeto(codProjeto);
            AtualizarQuadro();
        }

        private void AtualizarQuadro()
        {
            LimparColunas();

            var aFazer = tarefas.Where(t => !t.isConcluida && !t.isFazendo).ToList();
            var fazendo = tarefas.Where(t => !t.isConcluida && t.isFazendo).ToList();
            var feito = tarefas.Where(t => t.isConcluida).ToList();

            AdicionarTarefasNaColuna(Pnl_AFazer, aFazer, "A Fazer");
            AdicionarTarefasNaColuna(Pnl_Fazendo, fazendo, "Fazendo");
            AdicionarTarefasNaColuna(Pnl_Feito, feito, "Feito");

            // Atualizar contadores nos labels
            Lbl_aFazer.Text = $"A Fazer ({aFazer.Count})";
            Lbl_Fazendo.Text = $"Fazendo ({fazendo.Count})";
            Lbl_Feito.Text = $"Feito ({feito.Count})";
        }

        private void AdicionarTarefasNaColuna(Panel painel, List<Projeto_Tarefas> tarefasColuna, string nomeColuna)
        {
            // Remove apenas os post-its (Panels com Tag do tipo Projeto_Tarefas)
            var controlesParaRemover = painel.Controls.OfType<Panel>()
                .Where(p => p.Tag is Projeto_Tarefas).ToList();

            foreach (var controle in controlesParaRemover)
            {
                painel.Controls.Remove(controle);
                controle.Dispose();
            }

            int yPos = 60;
            foreach (var tarefa in tarefasColuna)
            {
                var postIt = CriarPostIt(tarefa);
                postIt.Location = new Point(10, yPos);
                painel.Controls.Add(postIt);
                yPos += postIt.Height + 10;
            }
        }

        private Panel CriarPostIt(Projeto_Tarefas tarefa)
        {
            var panel = new Panel
            {
                Size = new Size(360, 120),
                BackColor = ColorTranslator.FromHtml(tarefa.Cor),
                Margin = new Padding(5),
                Padding = new Padding(8),
                Tag = tarefa,
                Cursor = Cursors.Hand,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblDescricao = new Label
            {
                Text = tarefa.Descricao,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Arial", 9),
                ForeColor = Color.Black,
                BackColor = Color.Transparent
            };

            var btnHistorico = new Button
            {
                Text = "⏰",
                Size = new Size(30, 30),
                Location = new Point(325, 5),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Tag = tarefa,
                Cursor = Cursors.Hand
            };
            btnHistorico.FlatAppearance.BorderSize = 0;
            btnHistorico.Click += BtnHistorico_Click;

            // Botão de editar
            var btnEditar = new Button
            {
                Text = "✏️",
                Size = new Size(30, 30),
                Location = new Point(290, 5),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Tag = tarefa,
                Cursor = Cursors.Hand
            };
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.Click += BtnEditar_Click;

            panel.Controls.Add(lblDescricao);
            panel.Controls.Add(btnHistorico);
            panel.Controls.Add(btnEditar);

            var toolTip = new ToolTip();
            toolTip.SetToolTip(panel, $"Criado em: {tarefa.dataLimite:dd/MM/yyyy}");
            toolTip.SetToolTip(btnHistorico, "Ver histórico");
            toolTip.SetToolTip(btnEditar, "Editar tarefa");

            panel.MouseDown += PostIt_MouseDown;
            panel.AllowDrop = true;

            return panel;
        }

        private void ConfigurarDragDrop()
        {
            var paineis = new[] { Pnl_AFazer, Pnl_Fazendo, Pnl_Feito };
            foreach (var painel in paineis)
            {
                painel.AllowDrop = true;
                painel.DragEnter += Painel_DragEnter;
                painel.DragDrop += Painel_DragDrop;
            }
        }

        private void PostIt_MouseDown(object sender, MouseEventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null && e.Button == MouseButtons.Left)
            {
                panel.DoDragDrop(panel, DragDropEffects.Move);
            }
        }

        private void Painel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Panel)))
                e.Effect = DragDropEffects.Move;
        }

        private void Painel_DragDrop(object sender, DragEventArgs e)
        {
            var painelDestino = sender as Panel;
            var panelPostIt = e.Data.GetData(typeof(Panel)) as Panel;
            var tarefa = panelPostIt?.Tag as Projeto_Tarefas;

            if (painelDestino != null && tarefa != null)
            {
                var painelOrigem = panelPostIt.Parent as Panel;
                var statusOrigem = ObterStatusPorPainel(painelOrigem);
                var statusDestino = ObterStatusPorPainel(painelDestino);

                if (painelOrigem != painelDestino)
                {
                    painelOrigem?.Controls.Remove(panelPostIt);
                    painelDestino.Controls.Add(panelPostIt);

                    ReorganizarPostIts(painelDestino);
                    AtualizarStatusTarefa(tarefa, statusDestino);

                    historicoDB.RegistrarMovimentacaoTarefa(
                        tarefa.Codigo,
                        Sessao.UsuarioLogado.Codigo,
                        statusOrigem,
                        statusDestino
                    );

                    // Recarregar tarefas para atualizar contadores
                    CarregarTarefas();
                }
            }
        }

        private string ObterStatusPorPainel(Panel painel)
        {
            if (painel == Pnl_AFazer) return "A Fazer";
            if (painel == Pnl_Fazendo) return "Fazendo";
            if (painel == Pnl_Feito) return "Feito";
            return "Desconhecido";
        }

        private void ReorganizarPostIts(Panel painel)
        {
            var postIts = painel.Controls.OfType<Panel>().Where(p => p.Tag is Projeto_Tarefas).ToList();
            int yPos = 60;

            foreach (var postIt in postIts)
            {
                postIt.Location = new Point(10, yPos);
                yPos += postIt.Height + 10;
            }
        }

        private void AtualizarStatusTarefa(Projeto_Tarefas tarefa, string statusDestino)
        {
            bool isConcluida = false;
            bool isFazendo = false;

            switch (statusDestino)
            {
                case "A Fazer":
                    isConcluida = false;
                    isFazendo = false;
                    break;
                case "Fazendo":
                    isConcluida = false;
                    isFazendo = true;
                    break;
                case "Feito":
                    isConcluida = true;
                    isFazendo = false;
                    break;
            }

            tarefaDB.AtualizarStatusKanban(tarefa.Codigo, isConcluida, isFazendo);
        }

        private void Lbl_Adicionar1_Click(object sender, EventArgs e)
        {
            AdicionarTarefaComPopUp(false, false);
        }

        private void Lbl_Adicionar2_Click(object sender, EventArgs e)
        {
            AdicionarTarefaComPopUp(false, true);
        }

        private void Lbl_Adicionar3_Click(object sender, EventArgs e)
        {
            AdicionarTarefaComPopUp(true, false);
        }

        private void AdicionarTarefaComPopUp(bool concluida, bool fazendo)
        {
            using (var popup = new Frm_AdicionarPostIt())
            {
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    var novaTarefa = new Projeto_Tarefas
                    {
                        CodProjeto = codProjeto,
                        Descricao = popup.DescricaoTarefa,
                        Cor = popup.CorSelecionada,
                        isConcluida = concluida,
                        isFazendo = fazendo,
                        dataConclusao = concluida ? DateTime.Now : DateTime.MinValue,
                        dataLimite = DateTime.Now.AddDays(7),
                        NomeProjeto = nomeProjeto
                    };

                    if (tarefaDB.InserirTarefaComCor(novaTarefa))
                    {
                        CarregarTarefas();

                        var tarefasRecentes = tarefaDB.ListarTarefasPorProjeto(codProjeto);
                        var tarefaCriada = tarefasRecentes.FirstOrDefault(t => t.Descricao == popup.DescricaoTarefa);

                        if (tarefaCriada != null)
                        {
                            historicoDB.RegistrarCriacaoTarefa(tarefaCriada.Codigo, Sessao.UsuarioLogado.Codigo);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Erro ao criar tarefa: {tarefaDB.Mensagem}", "Erro",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var tarefa = btn?.Tag as Projeto_Tarefas;
            if (tarefa != null)
            {
                EditarTarefa(tarefa);
            }
        }

        private void EditarTarefa(Projeto_Tarefas tarefa)
        {
            using (var popup = new Frm_AdicionarPostIt())
            {
                // Preencher com dados atuais
                var descricaoField = popup.Controls.OfType<TextBox>().FirstOrDefault();
                if (descricaoField != null)
                    descricaoField.Text = tarefa.Descricao;

                if (popup.ShowDialog() == DialogResult.OK)
                {
                    tarefa.Descricao = popup.DescricaoTarefa;
                    tarefa.Cor = popup.CorSelecionada;

                    if (tarefaDB.AtualizarTarefa(tarefa))
                    {
                        CarregarTarefas();
                        MessageBox.Show("Tarefa atualizada com sucesso!", "Sucesso",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Erro ao atualizar tarefa: {tarefaDB.Mensagem}", "Erro",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnHistorico_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var tarefa = btn?.Tag as Projeto_Tarefas;
            if (tarefa != null)
            {
                var historico = historicoDB.ObterHistoricoPorTarefa(tarefa.Codigo);

                string mensagem = $"Histórico da Tarefa: {tarefa.Descricao}\n\n";
                foreach (var item in historico)
                {
                    mensagem += $"{item.DataAcao:dd/MM/yyyy HH:mm}: {item.Acao}\n";
                }

                MessageBox.Show(mensagem, "Histórico da Tarefa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LimparColunas()
        {
            LimparPainel(Pnl_AFazer);
            LimparPainel(Pnl_Fazendo);
            LimparPainel(Pnl_Feito);
        }

        private void LimparPainel(Panel painel)
        {
            var controlesParaRemover = painel.Controls.OfType<Panel>()
                .Where(p => p.Tag is Projeto_Tarefas).ToList();

            foreach (var controle in controlesParaRemover)
            {
                painel.Controls.Remove(controle);
                controle.Dispose();
            }
        }

        private void BtnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Btn_Atividade_Click(object sender, EventArgs e)
        {
            // Adicionar tarefa genérica na coluna "A Fazer"
            AdicionarTarefaComPopUp(false, false);
        }
    }
}