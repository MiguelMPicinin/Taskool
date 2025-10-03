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
        private List<Usuario> _membrosProjeto;

        // CONSTRUTOR CORRETO - para gerenciar membros do projeto
        public Frm_GerenciarMembros(int codProjeto, int usuarioLogadoId, MembrosDB membrosDB, TarefasDB tarefasDB)
        {
            _codProjeto = codProjeto;
            _usuarioLogadoId = usuarioLogadoId;
            _membrosDB = membrosDB;
            _tarefasDB = tarefasDB;

            InitializeComponent();
            ConfigurarInterface();
            CarregarMembros();
        }

        private void InitializeComponent()
        {
            this.Lbl_Titulo = new System.Windows.Forms.Label();
            this.Lst_Membros = new System.Windows.Forms.ListBox();
            this.Btn_Adicionar = new System.Windows.Forms.Button();
            this.Btn_Remover = new System.Windows.Forms.Button();
            this.Btn_Fechar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Lbl_Titulo
            // 
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Titulo.Location = new System.Drawing.Point(20, 20);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new System.Drawing.Size(197, 25);
            this.Lbl_Titulo.TabIndex = 0;
            this.Lbl_Titulo.Text = "Membros do Projeto";
            // 
            // Lst_Membros
            // 
            this.Lst_Membros.DisplayMember = "Nome";
            this.Lst_Membros.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lst_Membros.FormattingEnabled = true;
            this.Lst_Membros.ItemHeight = 17;
            this.Lst_Membros.Location = new System.Drawing.Point(20, 70);
            this.Lst_Membros.Name = "Lst_Membros";
            this.Lst_Membros.Size = new System.Drawing.Size(350, 191);
            this.Lst_Membros.TabIndex = 1;
            this.Lst_Membros.ValueMember = "Codigo";
            // 
            // Btn_Adicionar
            // 
            this.Btn_Adicionar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_Adicionar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Adicionar.Location = new System.Drawing.Point(20, 290);
            this.Btn_Adicionar.Name = "Btn_Adicionar";
            this.Btn_Adicionar.Size = new System.Drawing.Size(120, 35);
            this.Btn_Adicionar.TabIndex = 2;
            this.Btn_Adicionar.Text = "Adicionar Membro";
            this.Btn_Adicionar.UseVisualStyleBackColor = true;
            // 
            // Btn_Remover
            // 
            this.Btn_Remover.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_Remover.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Remover.Location = new System.Drawing.Point(150, 290);
            this.Btn_Remover.Name = "Btn_Remover";
            this.Btn_Remover.Size = new System.Drawing.Size(120, 35);
            this.Btn_Remover.TabIndex = 3;
            this.Btn_Remover.Text = "Remover Membro";
            this.Btn_Remover.UseVisualStyleBackColor = true;
            // 
            // Btn_Fechar
            // 
            this.Btn_Fechar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_Fechar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Fechar.Location = new System.Drawing.Point(280, 290);
            this.Btn_Fechar.Name = "Btn_Fechar";
            this.Btn_Fechar.Size = new System.Drawing.Size(90, 35);
            this.Btn_Fechar.TabIndex = 4;
            this.Btn_Fechar.Text = "Fechar";
            this.Btn_Fechar.UseVisualStyleBackColor = true;
            // 
            // Frm_GerenciarMembros
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 361);
            this.Controls.Add(this.Btn_Fechar);
            this.Controls.Add(this.Btn_Remover);
            this.Controls.Add(this.Btn_Adicionar);
            this.Controls.Add(this.Lst_Membros);
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
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // REMOVIDO: Criação programática dos controles
            // AGORA usando apenas os controles que já existem no Designer

            // Configurar eventos dos botões existentes
            Btn_Adicionar.Click += Btn_AdicionarMembro_Click;
            Btn_Remover.Click += Btn_RemoverMembro_Click;
            Btn_Fechar.Click += (s, e) => this.Close();

            // Configurar cores dos botões existentes
            Btn_Adicionar.BackColor = Color.FromArgb(74, 124, 255);
            Btn_Adicionar.ForeColor = Color.White;

            Btn_Remover.BackColor = Color.FromArgb(255, 87, 87);
            Btn_Remover.ForeColor = Color.White;

            Btn_Fechar.BackColor = Color.FromArgb(240, 240, 240);

            // Configurar cor do título
            Lbl_Titulo.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void CarregarMembros()
        {
            try
            {
                _membrosProjeto = _membrosDB.ObterMembrosProjeto(_codProjeto);
                Lst_Membros.DataSource = _membrosProjeto; // Usando Lst_Membros do Designer

                if (!_membrosProjeto.Any())
                {
                    MessageBox.Show("Não há membros neste projeto.", "Informação",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao carregar membros.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_AdicionarMembro_Click(object sender, EventArgs e)
        {
            // Implementar lógica para adicionar novo membro ao projeto
            MessageBox.Show("Funcionalidade de adicionar membro em desenvolvimento", "Informação",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Btn_RemoverMembro_Click(object sender, EventArgs e)
        {
            if (Lst_Membros.SelectedItem == null) // Usando Lst_Membros do Designer
            {
                MessageBox.Show("Selecione um membro para remover.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var usuarioSelecionado = (Usuario)Lst_Membros.SelectedItem; // Usando Lst_Membros do Designer

                if (MessageBox.Show($"Deseja remover {usuarioSelecionado.Nome} do projeto?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // CORREÇÃO: Usar método correto para remover membro
                    if (_membrosDB.RemoverMembroProjeto(_codProjeto, usuarioSelecionado.Codigo))
                    {
                        MessageBox.Show("Membro removido com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CarregarMembros();
                    }
                    else
                    {
                        MessageBox.Show($"Erro ao remover membro: {_membrosDB.Mensagem}", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao remover membro.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Label Lbl_Titulo;
        private ListBox Lst_Membros;
        private Button Btn_Adicionar;
        private Button Btn_Remover;
        private Button Btn_Fechar;
    }
}