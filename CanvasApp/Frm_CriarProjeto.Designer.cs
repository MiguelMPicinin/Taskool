namespace CanvasApp
{
    partial class Frm_CriarProjeto
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Lbl_Criar = new System.Windows.Forms.Label();
            this.Tb_Membros = new System.Windows.Forms.TabControl();
            this.Tbp_Membros = new System.Windows.Forms.TabPage();
            this.Lst_Sugestoes = new System.Windows.Forms.ListView();
            this.Lst_ListaMembros = new System.Windows.Forms.FlowLayoutPanel();
            this.Pic_AddPerson = new System.Windows.Forms.PictureBox();
            this.Lbl_NomeEndereco = new System.Windows.Forms.Label();
            this.Txt_NomeEndereco = new System.Windows.Forms.TextBox();
            this.Tbp_Opcoes = new System.Windows.Forms.TabPage();
            this.Rbtn_ToggleSwitch = new System.Windows.Forms.CheckBox();
            this.Txt_NomeProjeto = new System.Windows.Forms.TextBox();
            this.Btn_Criar = new System.Windows.Forms.Button();
            this.Btn_Cancelar = new System.Windows.Forms.Button();
            this.Lbl_Nome = new System.Windows.Forms.Label();
            this.Pic_List = new System.Windows.Forms.PictureBox();
            this.Lbl_Notificações = new System.Windows.Forms.Label();
            this.Tb_Membros.SuspendLayout();
            this.Tbp_Membros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pic_AddPerson)).BeginInit();
            this.Tbp_Opcoes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pic_List)).BeginInit();
            this.SuspendLayout();
            // 
            // Lbl_Criar
            // 
            this.Lbl_Criar.AutoSize = true;
            this.Lbl_Criar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Criar.Location = new System.Drawing.Point(12, 11);
            this.Lbl_Criar.Name = "Lbl_Criar";
            this.Lbl_Criar.Size = new System.Drawing.Size(51, 20);
            this.Lbl_Criar.TabIndex = 0;
            this.Lbl_Criar.Text = "label1";
            // 
            // Tb_Membros
            // 
            this.Tb_Membros.Controls.Add(this.Tbp_Membros);
            this.Tb_Membros.Controls.Add(this.Tbp_Opcoes);
            this.Tb_Membros.Location = new System.Drawing.Point(3, 67);
            this.Tb_Membros.Name = "Tb_Membros";
            this.Tb_Membros.SelectedIndex = 0;
            this.Tb_Membros.Size = new System.Drawing.Size(522, 344);
            this.Tb_Membros.TabIndex = 1;
            // 
            // Tbp_Membros
            // 
            this.Tbp_Membros.Controls.Add(this.Lst_Sugestoes);
            this.Tbp_Membros.Controls.Add(this.Lst_ListaMembros);
            this.Tbp_Membros.Controls.Add(this.Pic_AddPerson);
            this.Tbp_Membros.Controls.Add(this.Lbl_NomeEndereco);
            this.Tbp_Membros.Controls.Add(this.Txt_NomeEndereco);
            this.Tbp_Membros.Location = new System.Drawing.Point(4, 22);
            this.Tbp_Membros.Name = "Tbp_Membros";
            this.Tbp_Membros.Padding = new System.Windows.Forms.Padding(3);
            this.Tbp_Membros.Size = new System.Drawing.Size(514, 318);
            this.Tbp_Membros.TabIndex = 0;
            this.Tbp_Membros.Text = "tabPage1";
            this.Tbp_Membros.UseVisualStyleBackColor = true;
            // 
            // Lst_Sugestoes
            // 
            this.Lst_Sugestoes.HideSelection = false;
            this.Lst_Sugestoes.Location = new System.Drawing.Point(36, 37);
            this.Lst_Sugestoes.Name = "Lst_Sugestoes";
            this.Lst_Sugestoes.Size = new System.Drawing.Size(436, 66);
            this.Lst_Sugestoes.TabIndex = 8;
            this.Lst_Sugestoes.UseCompatibleStateImageBehavior = false;
            this.Lst_Sugestoes.ItemActivate += new System.EventHandler(this.Lst_Sugestoes_ItemActivate);
            // 
            // Lst_ListaMembros
            // 
            this.Lst_ListaMembros.Location = new System.Drawing.Point(6, 46);
            this.Lst_ListaMembros.Name = "Lst_ListaMembros";
            this.Lst_ListaMembros.Size = new System.Drawing.Size(497, 266);
            this.Lst_ListaMembros.TabIndex = 9;
            // 
            // Pic_AddPerson
            // 
            this.Pic_AddPerson.Location = new System.Drawing.Point(10, 11);
            this.Pic_AddPerson.Name = "Pic_AddPerson";
            this.Pic_AddPerson.Size = new System.Drawing.Size(25, 25);
            this.Pic_AddPerson.TabIndex = 7;
            this.Pic_AddPerson.TabStop = false;
            // 
            // Lbl_NomeEndereco
            // 
            this.Lbl_NomeEndereco.AutoSize = true;
            this.Lbl_NomeEndereco.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Lbl_NomeEndereco.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_NomeEndereco.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Lbl_NomeEndereco.Location = new System.Drawing.Point(40, 16);
            this.Lbl_NomeEndereco.Name = "Lbl_NomeEndereco";
            this.Lbl_NomeEndereco.Size = new System.Drawing.Size(44, 16);
            this.Lbl_NomeEndereco.TabIndex = 6;
            this.Lbl_NomeEndereco.Text = "label1";
            // 
            // Txt_NomeEndereco
            // 
            this.Txt_NomeEndereco.Location = new System.Drawing.Point(5, 6);
            this.Txt_NomeEndereco.Multiline = true;
            this.Txt_NomeEndereco.Name = "Txt_NomeEndereco";
            this.Txt_NomeEndereco.Size = new System.Drawing.Size(498, 34);
            this.Txt_NomeEndereco.TabIndex = 0;
            this.Txt_NomeEndereco.TextChanged += new System.EventHandler(this.Txt_NomeEndereco_TextChanged);
            this.Txt_NomeEndereco.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_NomeEndereco_KeyDown);
            // 
            // Tbp_Opcoes
            // 
            this.Tbp_Opcoes.Controls.Add(this.Lbl_Notificações);
            this.Tbp_Opcoes.Controls.Add(this.Rbtn_ToggleSwitch);
            this.Tbp_Opcoes.Location = new System.Drawing.Point(4, 22);
            this.Tbp_Opcoes.Name = "Tbp_Opcoes";
            this.Tbp_Opcoes.Padding = new System.Windows.Forms.Padding(3);
            this.Tbp_Opcoes.Size = new System.Drawing.Size(514, 318);
            this.Tbp_Opcoes.TabIndex = 1;
            this.Tbp_Opcoes.Text = "tabPage2";
            this.Tbp_Opcoes.UseVisualStyleBackColor = true;
            // 
            // Rbtn_ToggleSwitch
            // 
            this.Rbtn_ToggleSwitch.AutoSize = true;
            this.Rbtn_ToggleSwitch.Location = new System.Drawing.Point(12, 42);
            this.Rbtn_ToggleSwitch.Name = "Rbtn_ToggleSwitch";
            this.Rbtn_ToggleSwitch.Size = new System.Drawing.Size(80, 17);
            this.Rbtn_ToggleSwitch.TabIndex = 0;
            this.Rbtn_ToggleSwitch.Text = "checkBox1";
            this.Rbtn_ToggleSwitch.UseVisualStyleBackColor = true;
            this.Rbtn_ToggleSwitch.CheckedChanged += new System.EventHandler(this.Rbtn_ToggleSwitch_CheckedChanged_1);
            // 
            // Txt_NomeProjeto
            // 
            this.Txt_NomeProjeto.Location = new System.Drawing.Point(12, 35);
            this.Txt_NomeProjeto.Multiline = true;
            this.Txt_NomeProjeto.Name = "Txt_NomeProjeto";
            this.Txt_NomeProjeto.Size = new System.Drawing.Size(466, 26);
            this.Txt_NomeProjeto.TabIndex = 2;
            this.Txt_NomeProjeto.TextChanged += new System.EventHandler(this.Txt_NomeProjeto_TextChanged);
            // 
            // Btn_Criar
            // 
            this.Btn_Criar.Location = new System.Drawing.Point(12, 413);
            this.Btn_Criar.Name = "Btn_Criar";
            this.Btn_Criar.Size = new System.Drawing.Size(84, 35);
            this.Btn_Criar.TabIndex = 3;
            this.Btn_Criar.Text = "button1";
            this.Btn_Criar.UseVisualStyleBackColor = true;
            this.Btn_Criar.Click += new System.EventHandler(this.Btn_Criar_Click);
            // 
            // Btn_Cancelar
            // 
            this.Btn_Cancelar.Location = new System.Drawing.Point(102, 413);
            this.Btn_Cancelar.Name = "Btn_Cancelar";
            this.Btn_Cancelar.Size = new System.Drawing.Size(85, 35);
            this.Btn_Cancelar.TabIndex = 4;
            this.Btn_Cancelar.Text = "button2";
            this.Btn_Cancelar.UseVisualStyleBackColor = true;
            this.Btn_Cancelar.Click += new System.EventHandler(this.Btn_Cancelar_Click);
            // 
            // Lbl_Nome
            // 
            this.Lbl_Nome.AutoSize = true;
            this.Lbl_Nome.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Lbl_Nome.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Lbl_Nome.Location = new System.Drawing.Point(45, 42);
            this.Lbl_Nome.Name = "Lbl_Nome";
            this.Lbl_Nome.Size = new System.Drawing.Size(35, 13);
            this.Lbl_Nome.TabIndex = 5;
            this.Lbl_Nome.Text = "label1";
            // 
            // Pic_List
            // 
            this.Pic_List.Location = new System.Drawing.Point(19, 38);
            this.Pic_List.Name = "Pic_List";
            this.Pic_List.Size = new System.Drawing.Size(22, 19);
            this.Pic_List.TabIndex = 6;
            this.Pic_List.TabStop = false;
            // 
            // Lbl_Notificações
            // 
            this.Lbl_Notificações.AutoSize = true;
            this.Lbl_Notificações.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Notificações.Location = new System.Drawing.Point(9, 14);
            this.Lbl_Notificações.Name = "Lbl_Notificações";
            this.Lbl_Notificações.Size = new System.Drawing.Size(96, 20);
            this.Lbl_Notificações.TabIndex = 1;
            this.Lbl_Notificações.Text = "Notificações";
            // 
            // Frm_CriarProjeto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 450);
            this.Controls.Add(this.Pic_List);
            this.Controls.Add(this.Lbl_Nome);
            this.Controls.Add(this.Btn_Cancelar);
            this.Controls.Add(this.Btn_Criar);
            this.Controls.Add(this.Txt_NomeProjeto);
            this.Controls.Add(this.Tb_Membros);
            this.Controls.Add(this.Lbl_Criar);
            this.Name = "Frm_CriarProjeto";
            this.Text = "Frm_CriarProjeto";
            this.Load += new System.EventHandler(this.Frm_CriarProjeto_Load);
            this.Tb_Membros.ResumeLayout(false);
            this.Tbp_Membros.ResumeLayout(false);
            this.Tbp_Membros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pic_AddPerson)).EndInit();
            this.Tbp_Opcoes.ResumeLayout(false);
            this.Tbp_Opcoes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pic_List)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lbl_Criar;
        private System.Windows.Forms.TabControl Tb_Membros;
        private System.Windows.Forms.TabPage Tbp_Membros;
        private System.Windows.Forms.TabPage Tbp_Opcoes;
        private System.Windows.Forms.TextBox Txt_NomeProjeto;
        private System.Windows.Forms.Button Btn_Criar;
        private System.Windows.Forms.Button Btn_Cancelar;
        private System.Windows.Forms.TextBox Txt_NomeEndereco;
        private System.Windows.Forms.Label Lbl_Nome;
        private System.Windows.Forms.Label Lbl_NomeEndereco;
        private System.Windows.Forms.PictureBox Pic_AddPerson;
        private System.Windows.Forms.PictureBox Pic_List;
        private System.Windows.Forms.ListView Lst_Sugestoes;
        private System.Windows.Forms.FlowLayoutPanel Lst_ListaMembros;
        private System.Windows.Forms.CheckBox Rbtn_ToggleSwitch;
        private System.Windows.Forms.Label Lbl_Notificações;
    }
}