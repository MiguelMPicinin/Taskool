using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using CanvasApp.Classes.ManipulaçãoDados;
using CanvasApp.Formularios_Pop_Ups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        private readonly AnexosDB _anexosDB;
        private readonly HistoricoDB _historicoDB;

        private Button btnAdicionarSubtarefa;

        public Frm_TarefasDetalhes(Projeto_Tarefas tarefa)
        {
            InitializeComponent();
            this.tarefaAtual = tarefa;
            this.usuarioLogado = Sessao.UsuarioLogado;

            // Inicializar os DBs
            _alarmeDB = new AlarmeDB();
            _subtarefasDB = new SubtarefasDB();
            _comentariosDB = new ComentariosDB();
            _usuarioDB = new UsuarioDB();
            _anexosDB = new AnexosDB();
            _historicoDB = new HistoricoDB();

            var notificacoesDB = new NotificacoesDB();
            var projetosDB = new ProjetosDB();
            _membrosDB = new MembrosDB(notificacoesDB, projetosDB, _usuarioDB);
            _tarefasDB = new TarefasDB(notificacoesDB, projetosDB, _usuarioDB, _membrosDB, _alarmeDB, _subtarefasDB, _comentariosDB);

            ConfigurarLayoutDetalhes();
            CarregarDadosTarefa();
            CarregarHistoricoTarefa();
        }

        private void ConfigurarLayoutDetalhes()
        {
            ConfigurarEventos();
            ConfigurarComboBoxRepeticao();
            ConfigurarDateTimePickers();
            ConfigurarSubtarefas();
            ConfigurarComentarios();
            ConfigurarBotaoAtribuirResponsavel();
            ConfigurarAnexos();
            ConfigurarHistorico();
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
            Btn_Anexar.Click += Btn_Anexar_Click;
            Txt_TituloTarefa.Leave += Txt_TituloTarefa_Leave;
        }

        private void ConfigurarHistorico()
        {
            // Configurar ListView do histórico
            Lst_Historico.View = View.Details;
            Lst_Historico.FullRowSelect = true;
            Lst_Historico.GridLines = true;
            Lst_Historico.MultiSelect = false;

            // Adicionar colunas
            Lst_Historico.Columns.Clear();
            Lst_Historico.Columns.Add("Usuário", 150);
            Lst_Historico.Columns.Add("Ação", 300);
            Lst_Historico.Columns.Add("Data/Hora", 120);
        }

        private void CarregarHistoricoTarefa()
        {
            try
            {
                Lst_Historico.Items.Clear();

                var historicos = _historicoDB.ObterHistoricoPorTarefa(tarefaAtual.Codigo);

                if (!historicos.Any())
                {
                    // Adicionar item indicando que não há histórico
                    var item = new ListViewItem("Sistema");
                    item.SubItems.Add("Tarefa criada");
                    item.SubItems.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm")); // CORREÇÃO: Usar DateTime.Now
                    Lst_Historico.Items.Add(item);
                    return;
                }

                foreach (var historico in historicos)
                {
                    var item = new ListViewItem(historico.NomeUsuario ?? "Usuário Desconhecido");
                    item.SubItems.Add(historico.Texto);
                    item.SubItems.Add(historico.Data.ToString("dd/MM/yyyy HH:mm"));
                    Lst_Historico.Items.Add(item);
                }

                // Ajustar largura das colunas
                Lst_Historico.Columns[1].Width = Lst_Historico.Width - 270;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar histórico: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegistrarHistorico(string acao)
        {
            try
            {
                if (usuarioLogado == null) return;

                var historico = new HistoricoModificacoes
                {
                    CodTarefa = tarefaAtual.Codigo,
                    CodUsuario = usuarioLogado.Codigo,
                    Data = DateTime.Now,
                    Texto = acao
                };

                _historicoDB.InserirHistorico(historico);
                CarregarHistoricoTarefa();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao registrar histórico: {ex.Message}");
            }
        }

        // NOVO: Método para configurar anexos
        private void ConfigurarAnexos()
        {
            CarregarAnexos();
        }

        // NOVO: Evento do botão anexar
        private void Btn_Anexar_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Arquivos permitidos|*.txt;*.pdf;*.xlsx;*.xls;*.docx;*.doc;*.html;*.sql";
                openFileDialog.Multiselect = true;
                openFileDialog.Title = "Selecionar arquivos para anexar";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        AnexarArquivo(filePath);
                    }
                }
            }
        }

        // NOVO: Método para anexar arquivo
        private void AnexarArquivo(string filePath)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);

                // Validar tipo de arquivo
                if (!_anexosDB.ValidarTipoArquivo(fileName))
                {
                    MessageBox.Show($"Tipo de arquivo {Path.GetExtension(filePath)} não é permitido.\n\nArquivos permitidos: .txt, .pdf, .xlsx, .xls, .docx, .doc, .html, .sql",
                                  "Tipo de Arquivo Não Permitido",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Verificar se o arquivo não está vazio
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length == 0)
                {
                    MessageBox.Show("O arquivo selecionado está vazio.", "Arquivo Vazio",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ler arquivo e converter para base64
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string fileBase64 = Convert.ToBase64String(fileBytes);

                // Criar objeto anexo
                var anexo = new Tarefas_Anexos
                {
                    CodTarefa = tarefaAtual.Codigo,
                    NomeArquivo = fileName,
                    Arquivo = fileBase64,
                    DataUpload = DateTime.Now
                };

                // Inserir no banco
                if (_anexosDB.InserirAnexo(anexo))
                {
                    CarregarAnexos();
                    RegistrarHistorico($"Anexou o arquivo: {fileName}");
                    MessageBox.Show($"Arquivo '{fileName}' anexado com sucesso!", "Sucesso",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Erro ao anexar arquivo: {_anexosDB.Mensagem}", "Erro",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao anexar arquivo: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // NOVO: Método para carregar anexos
        private void CarregarAnexos()
        {
            try
            {
                Pnl_ArquivosIndexados.Controls.Clear();

                var anexos = _anexosDB.ListarAnexosPorTarefa(tarefaAtual.Codigo);

                foreach (var anexo in anexos)
                {
                    AdicionarControleAnexo(anexo);
                }

                if (!anexos.Any())
                {
                    AdicionarLabelSemAnexos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar anexos: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // NOVO: Adicionar controle de anexo
        private void AdicionarControleAnexo(Tarefas_Anexos anexo)
        {
            var pnlAnexo = new Panel
            {
                Width = Pnl_ArquivosIndexados.Width - 30,
                Height = 40,
                Margin = new Padding(0, 5, 0, 5),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = anexo,
                Cursor = Cursors.Hand
            };

            // Ícone baseado na extensão
            var picIcone = new PictureBox
            {
                Image = ObterIconePorExtensao(Path.GetExtension(anexo.NomeArquivo)),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(30, 30),
                Location = new Point(5, 5),
                Tag = anexo
            };
            picIcone.Click += (s, e) => AbrirAnexo(anexo);

            // Nome do arquivo (clicável)
            var lblNomeArquivo = new Label
            {
                Text = anexo.NomeArquivo,
                Location = new Point(40, 5),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(74, 124, 255),
                Cursor = Cursors.Hand,
                Tag = anexo,
                TextAlign = ContentAlignment.MiddleLeft
            };
            lblNomeArquivo.Click += (s, e) => AbrirAnexo(anexo);

            // Descrição do tipo
            var lblTipo = new Label
            {
                Text = _anexosDB.ObterDescricaoTipoArquivo(anexo.NomeArquivo),
                Location = new Point(250, 5),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Data do anexo
            var lblData = new Label
            {
                Text = anexo.DataUpload.ToString("dd/MM/yy HH:mm"),
                Location = new Point(380, 5),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Botão excluir
            var btnExcluir = new Button
            {
                Text = "×",
                Size = new Size(25, 25),
                Location = new Point(pnlAnexo.Width - 30, 8),
                Tag = anexo.Codigo,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExcluir.FlatAppearance.BorderSize = 0;
            btnExcluir.FlatAppearance.MouseOverBackColor = Color.LightCoral;
            btnExcluir.Click += (s, e) => ExcluirAnexo(anexo.Codigo);

            pnlAnexo.Controls.Add(picIcone);
            pnlAnexo.Controls.Add(lblNomeArquivo);
            pnlAnexo.Controls.Add(lblTipo);
            pnlAnexo.Controls.Add(lblData);
            pnlAnexo.Controls.Add(btnExcluir);

            Pnl_ArquivosIndexados.Controls.Add(pnlAnexo);
        }

        // NOVO: Obter ícone por extensão
        private Image ObterIconePorExtensao(string extensao)
        {
            var bmp = new Bitmap(30, 30);
            using (var g = Graphics.FromImage(bmp))
            {
                Color corFundo = ObterCorPorExtensao(extensao);
                g.Clear(corFundo);
                g.DrawRectangle(Pens.DarkGray, 0, 0, 29, 29);

                using (var font = new Font("Arial", 7, FontStyle.Bold))
                {
                    string textoExtensao = extensao.Replace(".", "").ToUpper();
                    if (textoExtensao.Length > 4) textoExtensao = textoExtensao.Substring(0, 4);

                    var tamanhoTexto = g.MeasureString(textoExtensao, font);
                    g.DrawString(textoExtensao, font, Brushes.White,
                                (30 - tamanhoTexto.Width) / 2,
                                (30 - tamanhoTexto.Height) / 2);
                }
            }
            return bmp;
        }

        private Color ObterCorPorExtensao(string extensao)
        {
            switch (extensao.ToLower())
            {
                case ".txt": return Color.SteelBlue;
                case ".pdf": return Color.IndianRed;
                case ".xlsx": case ".xls": return Color.ForestGreen;
                case ".docx": case ".doc": return Color.RoyalBlue;
                case ".html": return Color.OrangeRed;
                case ".sql": return Color.Purple;
                default: return Color.Gray;
            }
        }

        // NOVO: Label quando não há anexos
        private void AdicionarLabelSemAnexos()
        {
            var label = new Label
            {
                Text = "Nenhum anexo adicionado\nClique em 'Anexar' para adicionar arquivos",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Height = 60
            };
            Pnl_ArquivosIndexados.Controls.Add(label);
        }

        // NOVO: Abrir anexo
        private void AbrirAnexo(Tarefas_Anexos anexo)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Criar arquivo temporário
                string tempPath = Path.GetTempPath();
                string tempFilePath = Path.Combine(tempPath, anexo.NomeArquivo);

                // Converter base64 para bytes e salvar arquivo
                byte[] fileBytes = Convert.FromBase64String(anexo.Arquivo);
                File.WriteAllBytes(tempFilePath, fileBytes);

                string extensao = Path.GetExtension(anexo.NomeArquivo).ToLower();

                switch (extensao)
                {
                    case ".txt":
                        Process.Start("notepad.exe", tempFilePath);
                        break;
                    case ".pdf":
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = tempFilePath,
                            UseShellExecute = true
                        });
                        break;
                    case ".xlsx":
                    case ".xls":
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "excel.exe",
                            Arguments = $"\"{tempFilePath}\"",
                            UseShellExecute = true
                        });
                        break;
                    case ".docx":
                    case ".doc":
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "winword.exe",
                            Arguments = $"\"{tempFilePath}\"",
                            UseShellExecute = true
                        });
                        break;
                    case ".html":
                        AbrirHtmlNoTaskool(tempFilePath);
                        break;
                    case ".sql":
                        ExecutarScriptSQL(tempFilePath);
                        break;
                    default:
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = tempFilePath,
                            UseShellExecute = true
                        });
                        break;
                }

                MessageBox.Show($"Arquivo '{anexo.NomeArquivo}' aberto com sucesso!", "Sucesso",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir arquivo: {ex.Message}\n\nCertifique-se de que o programa associado está instalado.", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // NOVO: Abrir HTML no Taskool
        private void AbrirHtmlNoTaskool(string caminhoArquivo)
        {
            var frmVisualizador = new Form
            {
                Text = "Visualizador HTML - Taskool",
                Size = new Size(1024, 768),
                StartPosition = FormStartPosition.CenterParent,
                Icon = this.Icon
            };

            var webBrowser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                Url = new Uri($"file:///{caminhoArquivo.Replace("\\", "/")}"),
                ScrollBarsEnabled = true
            };

            var btnFechar = new Button
            {
                Text = "Fechar",
                Size = new Size(75, 30),
                Location = new Point(10, 10),
                BackColor = Color.FromArgb(74, 124, 255),
                ForeColor = Color.White
            };
            btnFechar.Click += (s, e) => frmVisualizador.Close();

            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.LightGray
            };
            panel.Controls.Add(btnFechar);

            frmVisualizador.Controls.Add(webBrowser);
            frmVisualizador.Controls.Add(panel);
            frmVisualizador.ShowDialog();
        }

        // NOVO: Executar script SQL
        private void ExecutarScriptSQL(string caminhoArquivo)
        {
            using (var frmSQL = new Frm_ExecutarSQL())
            {
                frmSQL.ShowDialog();
            }
        }

        // NOVO: Excluir anexo
        private void ExcluirAnexo(int codAnexo)
        {
            if (MessageBox.Show("Deseja excluir este anexo?\nEsta ação não pode ser desfeita.", "Confirmar Exclusão",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_anexosDB.ExcluirAnexo(codAnexo))
                    {
                        CarregarAnexos();
                        RegistrarHistorico("Removeu um anexo");
                        MessageBox.Show("Anexo excluído com sucesso!", "Sucesso",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Erro ao excluir anexo: {_anexosDB.Mensagem}", "Erro",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir anexo: {ex.Message}", "Erro",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CarregarDadosTarefa()
        {
            Txt_TituloTarefa.Text = tarefaAtual.Descricao;

            if (tarefaAtual.dataLimite != DateTime.MinValue && tarefaAtual.dataLimite >= new DateTime(1753, 1, 1))
            {
                Console.WriteLine($"Data limite da tarefa: {tarefaAtual.dataLimite:dd/MM/yyyy}");
            }

            CarregarPrazoAlarme();
            CarregarSubtarefas();
            CarregarAnexos();
            AtualizarPreviewComentarios();
        }

        private void Txt_TituloTarefa_Leave(object sender, EventArgs e)
        {
            if (tarefaAtual.Descricao != Txt_TituloTarefa.Text.Trim())
            {
                string descricaoAntiga = tarefaAtual.Descricao;
                tarefaAtual.Descricao = Txt_TituloTarefa.Text.Trim();

                if (_tarefasDB.AtualizarTarefa(tarefaAtual))
                {
                    RegistrarHistorico($"Alterou o título de '{descricaoAntiga}' para '{tarefaAtual.Descricao}'");
                    MessageBox.Show("Título atualizado com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Erro ao atualizar título: {_tarefasDB.Mensagem}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Txt_TituloTarefa.Text = descricaoAntiga;
                    tarefaAtual.Descricao = descricaoAntiga;
                }
            }
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
                    CarregarHistoricoTarefa();
                }
            };

            this.Controls.Add(btnAtribuirResponsavel);
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
                    RegistrarHistorico($"Definiu prazo para {Dtp_Prazo.Value:dd/MM/yyyy} com alarme às {Dtp_HoraAlarme.Value:HH:mm}");
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
                        RegistrarHistorico("Removeu o prazo e alarme");
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
                    RegistrarHistorico($"Adicionou subtarefa: {Txt_NovaSubtarefa.Text.Trim()}");
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
                if (_subtarefasDB.AtualizarSubtarefa(sub))
                {
                    string acao = sub.isConcluida ? "Concluiu" : "Reabriu";
                    RegistrarHistorico($"{acao} a subtarefa: {sub.Texto}");
                }
                else
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
                        RegistrarHistorico($"Removeu a subtarefa: {sub.Texto}");
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
                    RegistrarHistorico("Adicionou um comentário");
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