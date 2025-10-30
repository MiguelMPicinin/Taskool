namespace CanvasApp.Formularios_Pop_Ups
{
    partial class Frm_AdicionarPostIt
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
            this.Lbl_Descricao = new System.Windows.Forms.Label();
            this.Txt_Descricao = new System.Windows.Forms.TextBox();
            this.Lbl_Cor = new System.Windows.Forms.Label();
            this.Cmb_Cores = new System.Windows.Forms.ComboBox();
            this.Pnl_Preview = new System.Windows.Forms.Panel();
            this.Btn_Adicionar = new System.Windows.Forms.Button();
            this.Btn_Cancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Lbl_Descricao
            // 
            this.Lbl_Descricao.AutoSize = true;
            this.Lbl_Descricao.Location = new System.Drawing.Point(20, 20);
            this.Lbl_Descricao.Name = "Lbl_Descricao";
            this.Lbl_Descricao.Size = new System.Drawing.Size(55, 13);
            this.Lbl_Descricao.TabIndex = 0;
            this.Lbl_Descricao.Text = "Descrição";
            // 
            // Txt_Descricao
            // 
            this.Txt_Descricao.Location = new System.Drawing.Point(20, 50);
            this.Txt_Descricao.Multiline = true;
            this.Txt_Descricao.Name = "Txt_Descricao";
            this.Txt_Descricao.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Txt_Descricao.Size = new System.Drawing.Size(340, 100);
            this.Txt_Descricao.TabIndex = 1;
            // 
            // Lbl_Cor
            // 
            this.Lbl_Cor.AutoSize = true;
            this.Lbl_Cor.Location = new System.Drawing.Point(20, 160);
            this.Lbl_Cor.Name = "Lbl_Cor";
            this.Lbl_Cor.Size = new System.Drawing.Size(23, 13);
            this.Lbl_Cor.TabIndex = 2;
            this.Lbl_Cor.Text = "Cor";
            // 
            // Cmb_Cores
            // 
            this.Cmb_Cores.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_Cores.FormattingEnabled = true;
            this.Cmb_Cores.Location = new System.Drawing.Point(20, 190);
            this.Cmb_Cores.Name = "Cmb_Cores";
            this.Cmb_Cores.Size = new System.Drawing.Size(200, 21);
            this.Cmb_Cores.TabIndex = 3;
            // 
            // Pnl_Preview
            // 
            this.Pnl_Preview.Location = new System.Drawing.Point(230, 190);
            this.Pnl_Preview.Name = "Pnl_Preview";
            this.Pnl_Preview.Size = new System.Drawing.Size(50, 20);
            this.Pnl_Preview.TabIndex = 4;
            // 
            // Btn_Adicionar
            // 
            this.Btn_Adicionar.Location = new System.Drawing.Point(200, 230);
            this.Btn_Adicionar.Name = "Btn_Adicionar";
            this.Btn_Adicionar.Size = new System.Drawing.Size(80, 30);
            this.Btn_Adicionar.TabIndex = 5;
            this.Btn_Adicionar.Text = "Adicionar";
            this.Btn_Adicionar.UseVisualStyleBackColor = true;
            // 
            // Btn_Cancelar
            // 
            this.Btn_Cancelar.Location = new System.Drawing.Point(290, 230);
            this.Btn_Cancelar.Name = "Btn_Cancelar";
            this.Btn_Cancelar.Size = new System.Drawing.Size(80, 30);
            this.Btn_Cancelar.TabIndex = 6;
            this.Btn_Cancelar.Text = "Cancelar";
            this.Btn_Cancelar.UseVisualStyleBackColor = true;
            // 
            // Frm_AdicionarPostIt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.Btn_Cancelar);
            this.Controls.Add(this.Btn_Adicionar);
            this.Controls.Add(this.Pnl_Preview);
            this.Controls.Add(this.Cmb_Cores);
            this.Controls.Add(this.Lbl_Cor);
            this.Controls.Add(this.Txt_Descricao);
            this.Controls.Add(this.Lbl_Descricao);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Frm_AdicionarPostIt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Adicionar Tarefa";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lbl_Descricao;
        private System.Windows.Forms.TextBox Txt_Descricao;
        private System.Windows.Forms.Label Lbl_Cor;
        private System.Windows.Forms.ComboBox Cmb_Cores;
        private System.Windows.Forms.Panel Pnl_Preview;
        private System.Windows.Forms.Button Btn_Adicionar;
        private System.Windows.Forms.Button Btn_Cancelar;
    }
}