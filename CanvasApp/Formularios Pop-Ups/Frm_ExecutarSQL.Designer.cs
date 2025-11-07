namespace CanvasApp.Formularios_Pop_Ups
{
    partial class Frm_ExecutarSQL
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
            this.Chk_AutenticacaoWindows = new System.Windows.Forms.CheckBox();
            this.Lbl_Usuario = new System.Windows.Forms.Label();
            this.Txt_Usuario = new System.Windows.Forms.TextBox();
            this.Lbl_Senha = new System.Windows.Forms.Label();
            this.Txt_Senha = new System.Windows.Forms.TextBox();
            this.Btn_Executar = new System.Windows.Forms.Button();
            this.Txt_Resultado = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Lbl_Titulo
            // 
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Titulo.Location = new System.Drawing.Point(10, 10);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new System.Drawing.Size(150, 17);
            this.Lbl_Titulo.TabIndex = 0;
            this.Lbl_Titulo.Text = "Autenticação SQL Server";
            // 
            // Chk_AutenticacaoWindows
            // 
            this.Chk_AutenticacaoWindows.AutoSize = true;
            this.Chk_AutenticacaoWindows.Checked = true;
            this.Chk_AutenticacaoWindows.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Chk_AutenticacaoWindows.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Chk_AutenticacaoWindows.Location = new System.Drawing.Point(10, 40);
            this.Chk_AutenticacaoWindows.Name = "Chk_AutenticacaoWindows";
            this.Chk_AutenticacaoWindows.Size = new System.Drawing.Size(191, 19);
            this.Chk_AutenticacaoWindows.TabIndex = 1;
            this.Chk_AutenticacaoWindows.Text = "Usar Autenticação do Windows";
            this.Chk_AutenticacaoWindows.UseVisualStyleBackColor = true;
            // 
            // Lbl_Usuario
            // 
            this.Lbl_Usuario.AutoSize = true;
            this.Lbl_Usuario.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Usuario.Location = new System.Drawing.Point(10, 70);
            this.Lbl_Usuario.Name = "Lbl_Usuario";
            this.Lbl_Usuario.Size = new System.Drawing.Size(50, 15);
            this.Lbl_Usuario.TabIndex = 2;
            this.Lbl_Usuario.Text = "Usuário:";
            // 
            // Txt_Usuario
            // 
            this.Txt_Usuario.Enabled = false;
            this.Txt_Usuario.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Txt_Usuario.Location = new System.Drawing.Point(80, 67);
            this.Txt_Usuario.Name = "Txt_Usuario";
            this.Txt_Usuario.Size = new System.Drawing.Size(150, 23);
            this.Txt_Usuario.TabIndex = 3;
            // 
            // Lbl_Senha
            // 
            this.Lbl_Senha.AutoSize = true;
            this.Lbl_Senha.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Senha.Location = new System.Drawing.Point(10, 100);
            this.Lbl_Senha.Name = "Lbl_Senha";
            this.Lbl_Senha.Size = new System.Drawing.Size(42, 15);
            this.Lbl_Senha.TabIndex = 4;
            this.Lbl_Senha.Text = "Senha:";
            // 
            // Txt_Senha
            // 
            this.Txt_Senha.Enabled = false;
            this.Txt_Senha.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Txt_Senha.Location = new System.Drawing.Point(80, 97);
            this.Txt_Senha.Name = "Txt_Senha";
            this.Txt_Senha.Size = new System.Drawing.Size(150, 23);
            this.Txt_Senha.TabIndex = 5;
            this.Txt_Senha.UseSystemPasswordChar = true;
            // 
            // Btn_Executar
            // 
            this.Btn_Executar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(124)))), ((int)(((byte)(255)))));
            this.Btn_Executar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Executar.ForeColor = System.Drawing.Color.White;
            this.Btn_Executar.Location = new System.Drawing.Point(250, 67);
            this.Btn_Executar.Name = "Btn_Executar";
            this.Btn_Executar.Size = new System.Drawing.Size(100, 50);
            this.Btn_Executar.TabIndex = 6;
            this.Btn_Executar.Text = "Executar Script";
            this.Btn_Executar.UseVisualStyleBackColor = false;
            // 
            // Txt_Resultado
            // 
            this.Txt_Resultado.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Txt_Resultado.Location = new System.Drawing.Point(10, 130);
            this.Txt_Resultado.Multiline = true;
            this.Txt_Resultado.Name = "Txt_Resultado";
            this.Txt_Resultado.ReadOnly = true;
            this.Txt_Resultado.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Txt_Resultado.Size = new System.Drawing.Size(565, 320);
            this.Txt_Resultado.TabIndex = 7;
            // 
            // Frm_ExecutarSQL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.Txt_Resultado);
            this.Controls.Add(this.Btn_Executar);
            this.Controls.Add(this.Txt_Senha);
            this.Controls.Add(this.Lbl_Senha);
            this.Controls.Add(this.Txt_Usuario);
            this.Controls.Add(this.Lbl_Usuario);
            this.Controls.Add(this.Chk_AutenticacaoWindows);
            this.Controls.Add(this.Lbl_Titulo);
            this.Name = "Frm_ExecutarSQL";
            this.Text = "Executar Script SQL";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lbl_Titulo;
        private System.Windows.Forms.CheckBox Chk_AutenticacaoWindows;
        private System.Windows.Forms.Label Lbl_Usuario;
        private System.Windows.Forms.TextBox Txt_Usuario;
        private System.Windows.Forms.Label Lbl_Senha;
        private System.Windows.Forms.TextBox Txt_Senha;
        private System.Windows.Forms.Button Btn_Executar;
        private System.Windows.Forms.TextBox Txt_Resultado;
    }
}