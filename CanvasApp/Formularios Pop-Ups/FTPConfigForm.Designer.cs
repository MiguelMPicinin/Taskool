namespace CanvasApp
{
    partial class FTPConfigForm
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
            this.Lbl_Servidor = new System.Windows.Forms.Label();
            this.Lbl_Usuario = new System.Windows.Forms.Label();
            this.Lbl_Senha = new System.Windows.Forms.Label();
            this.Lbl_NomeArquivo = new System.Windows.Forms.Label();
            this.Txt_Servidor = new System.Windows.Forms.TextBox();
            this.Txt_Usuario = new System.Windows.Forms.TextBox();
            this.Txt_Senha = new System.Windows.Forms.TextBox();
            this.Txt_NomeArquivo = new System.Windows.Forms.TextBox();
            this.Btn_OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Lbl_Servidor
            // 
            this.Lbl_Servidor.AutoSize = true;
            this.Lbl_Servidor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Servidor.Location = new System.Drawing.Point(20, 20);
            this.Lbl_Servidor.Name = "Lbl_Servidor";
            this.Lbl_Servidor.Size = new System.Drawing.Size(100, 20);
            this.Lbl_Servidor.TabIndex = 0;
            this.Lbl_Servidor.Text = "Servidor FTP";
            this.Lbl_Servidor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Lbl_Usuario
            // 
            this.Lbl_Usuario.AutoSize = true;
            this.Lbl_Usuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Usuario.Location = new System.Drawing.Point(20, 50);
            this.Lbl_Usuario.Name = "Lbl_Usuario";
            this.Lbl_Usuario.Size = new System.Drawing.Size(64, 20);
            this.Lbl_Usuario.TabIndex = 1;
            this.Lbl_Usuario.Text = "Usuario";
            // 
            // Lbl_Senha
            // 
            this.Lbl_Senha.AutoSize = true;
            this.Lbl_Senha.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Senha.Location = new System.Drawing.Point(20, 80);
            this.Lbl_Senha.Name = "Lbl_Senha";
            this.Lbl_Senha.Size = new System.Drawing.Size(56, 20);
            this.Lbl_Senha.TabIndex = 2;
            this.Lbl_Senha.Text = "Senha";
            // 
            // Lbl_NomeArquivo
            // 
            this.Lbl_NomeArquivo.AutoSize = true;
            this.Lbl_NomeArquivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_NomeArquivo.Location = new System.Drawing.Point(2, 114);
            this.Lbl_NomeArquivo.Name = "Lbl_NomeArquivo";
            this.Lbl_NomeArquivo.Size = new System.Drawing.Size(112, 16);
            this.Lbl_NomeArquivo.TabIndex = 3;
            this.Lbl_NomeArquivo.Text = "Nome do Arquivo";
            // 
            // Txt_Servidor
            // 
            this.Txt_Servidor.Location = new System.Drawing.Point(120, 20);
            this.Txt_Servidor.Name = "Txt_Servidor";
            this.Txt_Servidor.Size = new System.Drawing.Size(240, 20);
            this.Txt_Servidor.TabIndex = 4;
            // 
            // Txt_Usuario
            // 
            this.Txt_Usuario.Location = new System.Drawing.Point(120, 50);
            this.Txt_Usuario.Name = "Txt_Usuario";
            this.Txt_Usuario.Size = new System.Drawing.Size(240, 20);
            this.Txt_Usuario.TabIndex = 5;
            // 
            // Txt_Senha
            // 
            this.Txt_Senha.Location = new System.Drawing.Point(120, 80);
            this.Txt_Senha.Name = "Txt_Senha";
            this.Txt_Senha.Size = new System.Drawing.Size(240, 20);
            this.Txt_Senha.TabIndex = 6;
            // 
            // Txt_NomeArquivo
            // 
            this.Txt_NomeArquivo.Location = new System.Drawing.Point(120, 112);
            this.Txt_NomeArquivo.Name = "Txt_NomeArquivo";
            this.Txt_NomeArquivo.Size = new System.Drawing.Size(240, 20);
            this.Txt_NomeArquivo.TabIndex = 7;
            // 
            // Btn_OK
            // 
            this.Btn_OK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Btn_OK.Location = new System.Drawing.Point(200, 150);
            this.Btn_OK.Name = "Btn_OK";
            this.Btn_OK.Size = new System.Drawing.Size(80, 30);
            this.Btn_OK.TabIndex = 8;
            this.Btn_OK.Text = "OK";
            this.Btn_OK.UseVisualStyleBackColor = true;
            // 
            // FTPConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 211);
            this.Controls.Add(this.Btn_OK);
            this.Controls.Add(this.Txt_NomeArquivo);
            this.Controls.Add(this.Txt_Senha);
            this.Controls.Add(this.Txt_Usuario);
            this.Controls.Add(this.Txt_Servidor);
            this.Controls.Add(this.Lbl_NomeArquivo);
            this.Controls.Add(this.Lbl_Senha);
            this.Controls.Add(this.Lbl_Usuario);
            this.Controls.Add(this.Lbl_Servidor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FTPConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuração FTP";
            this.Load += new System.EventHandler(this.FTPConfigForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lbl_Servidor;
        private System.Windows.Forms.Label Lbl_Usuario;
        private System.Windows.Forms.Label Lbl_Senha;
        private System.Windows.Forms.Label Lbl_NomeArquivo;
        private System.Windows.Forms.TextBox Txt_Servidor;
        private System.Windows.Forms.TextBox Txt_Usuario;
        private System.Windows.Forms.TextBox Txt_Senha;
        private System.Windows.Forms.TextBox Txt_NomeArquivo;
        private System.Windows.Forms.Button Btn_OK;
    }
}