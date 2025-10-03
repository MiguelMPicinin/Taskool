using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CanvasApp.Forms
{
    public partial class Frm_GerenciarMembros : Form
    {
        private int _codProjeto;
        private int _usuarioLogadoId;
        private MembrosDB _membrosDB;
        private TarefasDB _tarefasDB;
        private ProjetosDB _projetosDB;
        private UsuarioDB _usuarioDB;
        private List<Usuario> _membrosProjeto;
        private List<Usuario> _todosUsuarios;
        private PdfService _pdfService;

        // CONSTRUTOR ATUALIZADO
        public Frm_GerenciarMembros(int codProjeto, int usuarioLogadoId, MembrosDB membrosDB, TarefasDB tarefasDB)
        {
            InitializeComponent();

            _codProjeto = codProjeto;
            _usuarioLogadoId = usuarioLogadoId;
            _membrosDB = membrosDB;
            _tarefasDB = tarefasDB;
            _projetosDB = new ProjetosDB();
            _usuarioDB = new UsuarioDB();
            _pdfService = new PdfService();

            ConfigurarInterface();
            CarregarMembros();
            CarregarTodosUsuarios();
        }

        private void InitializeComponent()
        {
            this.Lbl_Titulo = new System.Windows.Forms.Label();
            this.Lst_Membros = new System.Windows.Forms.ListBox();
            this.Btn_Adicionar = new System.Windows.Forms.Button();
            this.Btn_Remover = new System.Windows.Forms.Button();
            this.Btn_ExportarPDF = new System.Windows.Forms.Button();
            this.Btn_Fechar = new System.Windows.Forms.Button();
            this.Cmb_UsuariosDisponiveis = new System.Windows.Forms.ComboBox();
            this.Lbl_SelecionarUsuario = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // Lbl_Titulo
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Titulo.Location = new System.Drawing.Point(20, 20);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new System.Drawing.Size(197, 25);
            this.Lbl_Titulo.TabIndex = 0;
            this.Lbl_Titulo.Text = "Membros do Projeto";

            // Lbl_SelecionarUsuario
            this.Lbl_SelecionarUsuario.AutoSize = true;
            this.Lbl_SelecionarUsuario.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_SelecionarUsuario.Location = new System.Drawing.Point(25, 55);
            this.Lbl_SelecionarUsuario.Name = "Lbl_SelecionarUsuario";
            this.Lbl_SelecionarUsuario.Size = new System.Drawing.Size(105, 15);
            this.Lbl_SelecionarUsuario.TabIndex = 1;
            this.Lbl_SelecionarUsuario.Text = "Adicionar Usuário:";

            // Cmb_UsuariosDisponiveis
            this.Cmb_UsuariosDisponiveis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_UsuariosDisponiveis.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cmb_UsuariosDisponiveis.FormattingEnabled = true;
            this.Cmb_UsuariosDisponiveis.Location = new System.Drawing.Point(135, 52);
            this.Cmb_UsuariosDisponiveis.Name = "Cmb_UsuariosDisponiveis";
            this.Cmb_UsuariosDisponiveis.Size = new System.Drawing.Size(200, 23);
            this.Cmb_UsuariosDisponiveis.TabIndex = 2;

            // Lst_Membros
            this.Lst_Membros.DisplayMember = "Nome";
            this.Lst_Membros.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lst_Membros.FormattingEnabled = true;
            this.Lst_Membros.ItemHeight = 17;
            this.Lst_Membros.Location = new System.Drawing.Point(25, 85);
            this.Lst_Membros.Name = "Lst_Membros";
            this.Lst_Membros.Size = new System.Drawing.Size(440, 191);
            this.Lst_Membros.TabIndex = 3;
            this.Lst_Membros.ValueMember = "Codigo";

            // Btn_Adicionar
            this.Btn_Adicionar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_Adicionar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Adicionar.Location = new System.Drawing.Point(345, 50);
            this.Btn_Adicionar.Name = "Btn_Adicionar";
            this.Btn_Adicionar.Size = new System.Drawing.Size(120, 27);
            this.Btn_Adicionar.TabIndex = 4;
            this.Btn_Adicionar.Text = "Adicionar Membro";
            this.Btn_Adicionar.UseVisualStyleBackColor = true;

            // Btn_Remover
            this.Btn_Remover.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_Remover.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Remover.Location = new System.Drawing.Point(25, 290);
            this.Btn_Remover.Name = "Btn_Remover";
            this.Btn_Remover.Size = new System.Drawing.Size(120, 35);
            this.Btn_Remover.TabIndex = 5;
            this.Btn_Remover.Text = "Remover Membro";
            this.Btn_Remover.UseVisualStyleBackColor = true;

            // Btn_ExportarPDF
            this.Btn_ExportarPDF.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_ExportarPDF.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_ExportarPDF.Location = new System.Drawing.Point(155, 290);
            this.Btn_ExportarPDF.Name = "Btn_ExportarPDF";
            this.Btn_ExportarPDF.Size = new System.Drawing.Size(120, 35);
            this.Btn_ExportarPDF.TabIndex = 6;
            this.Btn_ExportarPDF.Text = "Exportar PDF";
            this.Btn_ExportarPDF.UseVisualStyleBackColor = true;

            // Btn_Fechar
            this.Btn_Fechar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_Fechar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Fechar.Location = new System.Drawing.Point(285, 290);
            this.Btn_Fechar.Name = "Btn_Fechar";
            this.Btn_Fechar.Size = new System.Drawing.Size(120, 35);
            this.Btn_Fechar.TabIndex = 7;
            this.Btn_Fechar.Text = "Fechar";
            this.Btn_Fechar.UseVisualStyleBackColor = true;

            // Frm_GerenciarMembros
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 341);
            this.Controls.Add(this.Btn_Fechar);
            this.Controls.Add(this.Btn_ExportarPDF);
            this.Controls.Add(this.Btn_Remover);
            this.Controls.Add(this.Btn_Adicionar);
            this.Controls.Add(this.Lst_Membros);
            this.Controls.Add(this.Cmb_UsuariosDisponiveis);
            this.Controls.Add(this.Lbl_SelecionarUsuario);
            this.Controls.Add(this.Lbl_Titulo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_GerenciarMembros";
            this.Text = "Gerenciar Membros do Projeto | Taskool";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ConfigurarInterface()
        {
            // Configuração do formulário
            this.Text = "Gerenciar Membros do Projeto | Taskool";
            this.Size = new Size(500, 380);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Configurar eventos dos botões
            Btn_Adicionar.Click += Btn_AdicionarMembro_Click;
            Btn_Remover.Click += Btn_RemoverMembro_Click;
            Btn_ExportarPDF.Click += Btn_ExportarPDF_Click;
            Btn_Fechar.Click += (s, e) => this.Close();

            // Configurar cores dos botões
            Btn_Adicionar.BackColor = Color.FromArgb(74, 124, 255);
            Btn_Adicionar.ForeColor = Color.White;
            Btn_Adicionar.FlatStyle = FlatStyle.Flat;
            Btn_Adicionar.FlatAppearance.BorderSize = 0;

            Btn_Remover.BackColor = Color.FromArgb(255, 87, 87);
            Btn_Remover.ForeColor = Color.White;
            Btn_Remover.FlatStyle = FlatStyle.Flat;
            Btn_Remover.FlatAppearance.BorderSize = 0;

            Btn_ExportarPDF.BackColor = Color.FromArgb(46, 204, 113);
            Btn_ExportarPDF.ForeColor = Color.White;
            Btn_ExportarPDF.FlatStyle = FlatStyle.Flat;
            Btn_ExportarPDF.FlatAppearance.BorderSize = 0;

            Btn_Fechar.BackColor = Color.FromArgb(240, 240, 240);
            Btn_Fechar.FlatStyle = FlatStyle.Flat;
            Btn_Fechar.FlatAppearance.BorderSize = 0;

            // Configurar cor do título
            Lbl_Titulo.ForeColor = Color.FromArgb(64, 64, 64);

            // Configurar ListBox
            Lst_Membros.BackColor = Color.FromArgb(250, 250, 250);
            Lst_Membros.BorderStyle = BorderStyle.FixedSingle;

            // Configurar ComboBox
            Cmb_UsuariosDisponiveis.BackColor = Color.FromArgb(250, 250, 250);
            Cmb_UsuariosDisponiveis.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void CarregarMembros()
        {
            try
            {
                _membrosProjeto = _membrosDB.ObterMembrosProjeto(_codProjeto);
                Lst_Membros.DataSource = null;
                Lst_Membros.DataSource = _membrosProjeto;
                Lst_Membros.DisplayMember = "Nome";
                Lst_Membros.ValueMember = "Codigo";

                if (!_membrosProjeto.Any())
                {
                    MessageBox.Show("Não há membros neste projeto.", "Informação",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar membros: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarTodosUsuarios()
        {
            try
            {
                // CORREÇÃO: Usando o método existente BuscarUsuariosPorTexto com string vazia para obter todos os usuários
                _todosUsuarios = _usuarioDB.BuscarUsuariosPorTexto("");

                // Filtrar usuários que ainda não são membros do projeto
                var usuariosDisponiveis = _todosUsuarios
                    .Where(u => !_membrosProjeto.Any(m => m.Codigo == u.Codigo))
                    .ToList();

                Cmb_UsuariosDisponiveis.DataSource = null;
                Cmb_UsuariosDisponiveis.DataSource = usuariosDisponiveis;
                Cmb_UsuariosDisponiveis.DisplayMember = "Nome";
                Cmb_UsuariosDisponiveis.ValueMember = "Codigo";

                if (!usuariosDisponiveis.Any())
                {
                    Cmb_UsuariosDisponiveis.Enabled = false;
                    Btn_Adicionar.Enabled = false;
                    Cmb_UsuariosDisponiveis.Text = "Nenhum usuário disponível";
                }
                else
                {
                    Cmb_UsuariosDisponiveis.Enabled = true;
                    Btn_Adicionar.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuários: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_AdicionarMembro_Click(object sender, EventArgs e)
        {
            if (Cmb_UsuariosDisponiveis.SelectedItem == null)
            {
                MessageBox.Show("Selecione um usuário para adicionar ao projeto.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var usuarioSelecionado = (Usuario)Cmb_UsuariosDisponiveis.SelectedItem;

                if (MessageBox.Show($"Deseja adicionar {usuarioSelecionado.Nome} ao projeto?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // CORREÇÃO: Usando o método existente AdicionarMembroAoProjeto
                    // Convertendo Codigo (string) para int
                    if (int.TryParse(usuarioSelecionado.Codigo, out int codUsuarioInt))
                    {
                        bool sucesso = _membrosDB.AdicionarMembroAoProjeto(_codProjeto, codUsuarioInt);

                        if (sucesso)
                        {
                            MessageBox.Show("Membro adicionado com sucesso!", "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Recarregar as listas
                            CarregarMembros();
                            CarregarTodosUsuarios();
                        }
                        else
                        {
                            // CORREÇÃO: Acessando a propriedade Mensagem corretamente
                            string mensagemErro = _membrosDB.Mensagem ?? "Erro ao adicionar membro ao projeto.";
                            MessageBox.Show(mensagemErro, "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Código do usuário inválido.", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar membro: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_RemoverMembro_Click(object sender, EventArgs e)
        {
            if (Lst_Membros.SelectedItem == null)
            {
                MessageBox.Show("Selecione um membro para remover.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var usuarioSelecionado = (Usuario)Lst_Membros.SelectedItem;

                // CORREÇÃO: Convertendo para o mesmo tipo para comparação
                if (usuarioSelecionado.Codigo == _usuarioLogadoId.ToString())
                {
                    MessageBox.Show("Você não pode remover a si mesmo do projeto.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show($"Deseja remover {usuarioSelecionado.Nome} do projeto?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_membrosDB.RemoverMembroProjeto(_codProjeto, usuarioSelecionado.Codigo))
                    {
                        MessageBox.Show("Membro removido com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Recarregar as listas
                        CarregarMembros();
                        CarregarTodosUsuarios();
                    }
                    else
                    {
                        // CORREÇÃO: Acessando a propriedade Mensagem corretamente
                        string mensagemErro = _membrosDB.Mensagem ?? "Erro ao remover membro do projeto.";
                        MessageBox.Show(mensagemErro, "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao remover membro: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_ExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_membrosProjeto.Any())
                {
                    MessageBox.Show("Não há membros para exportar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obter nome do projeto
                string nomeProjeto = _projetosDB.ObterNomeProjeto(_codProjeto);
                if (string.IsNullOrEmpty(nomeProjeto))
                {
                    nomeProjeto = $"Projeto_{_codProjeto}";
                }

                // Exportar PDF
                _pdfService.ExportarMembrosParaPdf(_membrosProjeto, nomeProjeto, _codProjeto);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar PDF: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Controles da interface
        private Label Lbl_Titulo;
        private Label Lbl_SelecionarUsuario;
        private ComboBox Cmb_UsuariosDisponiveis;
        private ListBox Lst_Membros;
        private Button Btn_Adicionar;
        private Button Btn_Remover;
        private Button Btn_ExportarPDF;
        private Button Btn_Fechar;
    }
}