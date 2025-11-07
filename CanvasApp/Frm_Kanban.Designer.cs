namespace CanvasApp
{
    partial class Frm_Kanban
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
            this.Lbl_Titulo = new System.Windows.Forms.Label();
            this.Lbl_aFazer = new System.Windows.Forms.Label();
            this.Lbl_Fazendo = new System.Windows.Forms.Label();
            this.Lbl_Feito = new System.Windows.Forms.Label();
            this.Btn_Atividade = new System.Windows.Forms.Button();
            this.Flw_AFazer = new System.Windows.Forms.FlowLayoutPanel();
            this.Flw_Fazendo = new System.Windows.Forms.FlowLayoutPanel();
            this.Flw_Feito = new System.Windows.Forms.FlowLayoutPanel();
            this.Btn_AddPostIt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Lbl_Titulo
            // 
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Titulo.Location = new System.Drawing.Point(290, 24);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new System.Drawing.Size(131, 25);
            this.Lbl_Titulo.TabIndex = 0;
            this.Lbl_Titulo.Text = "Meu Quadro";
            // 
            // Lbl_aFazer
            // 
            this.Lbl_aFazer.AutoSize = true;
            this.Lbl_aFazer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_aFazer.Location = new System.Drawing.Point(90, 69);
            this.Lbl_aFazer.Name = "Lbl_aFazer";
            this.Lbl_aFazer.Size = new System.Drawing.Size(41, 16);
            this.Lbl_aFazer.TabIndex = 4;
            this.Lbl_aFazer.Text = "Fazer";
            // 
            // Lbl_Fazendo
            // 
            this.Lbl_Fazendo.AutoSize = true;
            this.Lbl_Fazendo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Fazendo.Location = new System.Drawing.Point(336, 69);
            this.Lbl_Fazendo.Name = "Lbl_Fazendo";
            this.Lbl_Fazendo.Size = new System.Drawing.Size(60, 16);
            this.Lbl_Fazendo.TabIndex = 5;
            this.Lbl_Fazendo.Text = "Fazendo";
            // 
            // Lbl_Feito
            // 
            this.Lbl_Feito.AutoSize = true;
            this.Lbl_Feito.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Feito.Location = new System.Drawing.Point(604, 69);
            this.Lbl_Feito.Name = "Lbl_Feito";
            this.Lbl_Feito.Size = new System.Drawing.Size(37, 16);
            this.Lbl_Feito.TabIndex = 6;
            this.Lbl_Feito.Text = "Feito";
            // 
            // Btn_Atividade
            // 
            this.Btn_Atividade.Location = new System.Drawing.Point(12, 12);
            this.Btn_Atividade.Name = "Btn_Atividade";
            this.Btn_Atividade.Size = new System.Drawing.Size(83, 37);
            this.Btn_Atividade.TabIndex = 7;
            this.Btn_Atividade.Text = "Histórico de Modificações";
            this.Btn_Atividade.UseVisualStyleBackColor = true;
            this.Btn_Atividade.Click += new System.EventHandler(this.Btn_Atividade_Click);
            // 
            // Flw_AFazer
            // 
            this.Flw_AFazer.AllowDrop = true;
            this.Flw_AFazer.Location = new System.Drawing.Point(14, 98);
            this.Flw_AFazer.Name = "Flw_AFazer";
            this.Flw_AFazer.Size = new System.Drawing.Size(204, 329);
            this.Flw_AFazer.TabIndex = 8;
            // 
            // Flw_Fazendo
            // 
            this.Flw_Fazendo.AllowDrop = true;
            this.Flw_Fazendo.Location = new System.Drawing.Point(267, 98);
            this.Flw_Fazendo.Name = "Flw_Fazendo";
            this.Flw_Fazendo.Size = new System.Drawing.Size(204, 329);
            this.Flw_Fazendo.TabIndex = 9;
            // 
            // Flw_Feito
            // 
            this.Flw_Feito.AllowDrop = true;
            this.Flw_Feito.Location = new System.Drawing.Point(522, 98);
            this.Flw_Feito.Name = "Flw_Feito";
            this.Flw_Feito.Size = new System.Drawing.Size(204, 329);
            this.Flw_Feito.TabIndex = 10;
            // 
            // Btn_AddPostIt
            // 
            this.Btn_AddPostIt.Location = new System.Drawing.Point(638, 20);
            this.Btn_AddPostIt.Name = "Btn_AddPostIt";
            this.Btn_AddPostIt.Size = new System.Drawing.Size(75, 38);
            this.Btn_AddPostIt.TabIndex = 11;
            this.Btn_AddPostIt.Text = "Adicionar Post-It";
            this.Btn_AddPostIt.UseVisualStyleBackColor = true;
            this.Btn_AddPostIt.Click += new System.EventHandler(this.Btn_AddPostIt_Click);
            // 
            // Frm_Kanban
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 450);
            this.Controls.Add(this.Btn_AddPostIt);
            this.Controls.Add(this.Flw_Feito);
            this.Controls.Add(this.Flw_Fazendo);
            this.Controls.Add(this.Flw_AFazer);
            this.Controls.Add(this.Btn_Atividade);
            this.Controls.Add(this.Lbl_Feito);
            this.Controls.Add(this.Lbl_Fazendo);
            this.Controls.Add(this.Lbl_aFazer);
            this.Controls.Add(this.Lbl_Titulo);
            this.Name = "Frm_Kanban";
            this.Text = "Frm_Kanban";
            this.Load += new System.EventHandler(this.Frm_Kanban_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lbl_Titulo;
        private System.Windows.Forms.Label Lbl_aFazer;
        private System.Windows.Forms.Label Lbl_Fazendo;
        private System.Windows.Forms.Label Lbl_Feito;
        private System.Windows.Forms.Button Btn_Atividade;
        private System.Windows.Forms.FlowLayoutPanel Flw_AFazer;
        private System.Windows.Forms.FlowLayoutPanel Flw_Fazendo;
        private System.Windows.Forms.FlowLayoutPanel Flw_Feito;
        private System.Windows.Forms.Button Btn_AddPostIt;
    }
}