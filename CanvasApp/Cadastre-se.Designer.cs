namespace CanvasApp
{
    partial class Frm_Cadastro
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Cadastro));
            this.Lbl_Titulo = new System.Windows.Forms.Label();
            this.Txt_Nome = new System.Windows.Forms.TextBox();
            this.Txt_Telefone = new System.Windows.Forms.TextBox();
            this.Txt_Email = new System.Windows.Forms.TextBox();
            this.Txt_Usuario = new System.Windows.Forms.TextBox();
            this.Btn_Salvar = new System.Windows.Forms.Button();
            this.Lbl_Nome = new System.Windows.Forms.Label();
            this.Lbl_Email = new System.Windows.Forms.Label();
            this.Lbl_Usuario = new System.Windows.Forms.Label();
            this.Lbl_Telefone = new System.Windows.Forms.Label();
            this.Lbl_Data = new System.Windows.Forms.Label();
            this.Msk_Data = new System.Windows.Forms.MaskedTextBox();
            this.Btn_Selecionar = new System.Windows.Forms.Button();
            this.Btn_Aleatorio = new System.Windows.Forms.Button();
            this.Pic_Foto = new System.Windows.Forms.PictureBox();
            this.Lbl_Credencial = new System.Windows.Forms.Label();
            this.Txt_Credencial = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Pic_Foto)).BeginInit();
            this.SuspendLayout();
            // 
            // Lbl_Titulo
            // 
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Titulo.Location = new System.Drawing.Point(232, 46);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new System.Drawing.Size(93, 33);
            this.Lbl_Titulo.TabIndex = 0;
            this.Lbl_Titulo.Text = "label1";
            // 
            // Txt_Nome
            // 
            this.Txt_Nome.Location = new System.Drawing.Point(155, 114);
            this.Txt_Nome.Name = "Txt_Nome";
            this.Txt_Nome.Size = new System.Drawing.Size(246, 20);
            this.Txt_Nome.TabIndex = 1;
            this.Txt_Nome.StyleChanged += new System.EventHandler(this.Txt_Nome_StyleChanged);
            // 
            // Txt_Telefone
            // 
            this.Txt_Telefone.Location = new System.Drawing.Point(155, 189);
            this.Txt_Telefone.Name = "Txt_Telefone";
            this.Txt_Telefone.Size = new System.Drawing.Size(246, 20);
            this.Txt_Telefone.TabIndex = 2;
            // 
            // Txt_Email
            // 
            this.Txt_Email.Location = new System.Drawing.Point(155, 151);
            this.Txt_Email.Name = "Txt_Email";
            this.Txt_Email.Size = new System.Drawing.Size(246, 20);
            this.Txt_Email.TabIndex = 3;
            this.Txt_Email.TextChanged += new System.EventHandler(this.Txt_Email_TextChanged);
            this.Txt_Email.Validated += new System.EventHandler(this.Txt_Email_Validated);
            // 
            // Txt_Usuario
            // 
            this.Txt_Usuario.Location = new System.Drawing.Point(155, 225);
            this.Txt_Usuario.Name = "Txt_Usuario";
            this.Txt_Usuario.Size = new System.Drawing.Size(246, 20);
            this.Txt_Usuario.TabIndex = 4;
            // 
            // Btn_Salvar
            // 
            this.Btn_Salvar.Location = new System.Drawing.Point(184, 428);
            this.Btn_Salvar.Name = "Btn_Salvar";
            this.Btn_Salvar.Size = new System.Drawing.Size(196, 26);
            this.Btn_Salvar.TabIndex = 6;
            this.Btn_Salvar.Text = "button1";
            this.Btn_Salvar.UseVisualStyleBackColor = true;
            this.Btn_Salvar.Click += new System.EventHandler(this.Btn_Salvar_Click);
            // 
            // Lbl_Nome
            // 
            this.Lbl_Nome.AutoSize = true;
            this.Lbl_Nome.Location = new System.Drawing.Point(114, 117);
            this.Lbl_Nome.Name = "Lbl_Nome";
            this.Lbl_Nome.Size = new System.Drawing.Size(35, 13);
            this.Lbl_Nome.TabIndex = 7;
            this.Lbl_Nome.Text = "label1";
            // 
            // Lbl_Email
            // 
            this.Lbl_Email.AutoSize = true;
            this.Lbl_Email.Location = new System.Drawing.Point(114, 154);
            this.Lbl_Email.Name = "Lbl_Email";
            this.Lbl_Email.Size = new System.Drawing.Size(35, 13);
            this.Lbl_Email.TabIndex = 8;
            this.Lbl_Email.Text = "label1";
            // 
            // Lbl_Usuario
            // 
            this.Lbl_Usuario.AutoSize = true;
            this.Lbl_Usuario.Location = new System.Drawing.Point(114, 225);
            this.Lbl_Usuario.Name = "Lbl_Usuario";
            this.Lbl_Usuario.Size = new System.Drawing.Size(35, 13);
            this.Lbl_Usuario.TabIndex = 9;
            this.Lbl_Usuario.Text = "label1";
            // 
            // Lbl_Telefone
            // 
            this.Lbl_Telefone.AutoSize = true;
            this.Lbl_Telefone.Location = new System.Drawing.Point(104, 192);
            this.Lbl_Telefone.Name = "Lbl_Telefone";
            this.Lbl_Telefone.Size = new System.Drawing.Size(35, 13);
            this.Lbl_Telefone.TabIndex = 10;
            this.Lbl_Telefone.Text = "label1";
            // 
            // Lbl_Data
            // 
            this.Lbl_Data.AutoSize = true;
            this.Lbl_Data.Location = new System.Drawing.Point(83, 266);
            this.Lbl_Data.Name = "Lbl_Data";
            this.Lbl_Data.Size = new System.Drawing.Size(35, 13);
            this.Lbl_Data.TabIndex = 11;
            this.Lbl_Data.Text = "label1";
            // 
            // Msk_Data
            // 
            this.Msk_Data.Location = new System.Drawing.Point(155, 263);
            this.Msk_Data.Mask = "00/00/0000";
            this.Msk_Data.Name = "Msk_Data";
            this.Msk_Data.Size = new System.Drawing.Size(67, 20);
            this.Msk_Data.TabIndex = 12;
            this.Msk_Data.ValidatingType = typeof(System.DateTime);
            // 
            // Btn_Selecionar
            // 
            this.Btn_Selecionar.Location = new System.Drawing.Point(233, 312);
            this.Btn_Selecionar.Name = "Btn_Selecionar";
            this.Btn_Selecionar.Size = new System.Drawing.Size(111, 26);
            this.Btn_Selecionar.TabIndex = 13;
            this.Btn_Selecionar.Text = "button1";
            this.Btn_Selecionar.UseVisualStyleBackColor = true;
            this.Btn_Selecionar.Click += new System.EventHandler(this.Btn_Selecionar_Click);
            // 
            // Btn_Aleatorio
            // 
            this.Btn_Aleatorio.Location = new System.Drawing.Point(409, 223);
            this.Btn_Aleatorio.Name = "Btn_Aleatorio";
            this.Btn_Aleatorio.Size = new System.Drawing.Size(76, 39);
            this.Btn_Aleatorio.TabIndex = 14;
            this.Btn_Aleatorio.Text = "button2";
            this.Btn_Aleatorio.UseVisualStyleBackColor = true;
            this.Btn_Aleatorio.Click += new System.EventHandler(this.Btn_Aleatorio_Click);
            // 
            // Pic_Foto
            // 
            this.Pic_Foto.Image = ((System.Drawing.Image)(resources.GetObject("Pic_Foto.Image")));
            this.Pic_Foto.Location = new System.Drawing.Point(122, 312);
            this.Pic_Foto.Name = "Pic_Foto";
            this.Pic_Foto.Size = new System.Drawing.Size(100, 91);
            this.Pic_Foto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.Pic_Foto.TabIndex = 15;
            this.Pic_Foto.TabStop = false;
            this.Pic_Foto.VisibleChanged += new System.EventHandler(this.Pic_Foto_VisibleChanged);
            this.Pic_Foto.Click += new System.EventHandler(this.Pic_Foto_Click);
            this.Pic_Foto.DoubleClick += new System.EventHandler(this.Pic_Foto_DoubleClick);
            this.Pic_Foto.StyleChanged += new System.EventHandler(this.Pic_Foto_StyleChanged);
            // 
            // Lbl_Credencial
            // 
            this.Lbl_Credencial.AutoSize = true;
            this.Lbl_Credencial.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Credencial.Location = new System.Drawing.Point(17, 325);
            this.Lbl_Credencial.Name = "Lbl_Credencial";
            this.Lbl_Credencial.Size = new System.Drawing.Size(60, 24);
            this.Lbl_Credencial.TabIndex = 16;
            this.Lbl_Credencial.Text = "label1";
            // 
            // Txt_Credencial
            // 
            this.Txt_Credencial.AutoSize = true;
            this.Txt_Credencial.Location = new System.Drawing.Point(22, 6);
            this.Txt_Credencial.Name = "Txt_Credencial";
            this.Txt_Credencial.Size = new System.Drawing.Size(0, 13);
            this.Txt_Credencial.TabIndex = 17;
            // 
            // Frm_Cadastro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 471);
            this.Controls.Add(this.Txt_Credencial);
            this.Controls.Add(this.Lbl_Credencial);
            this.Controls.Add(this.Pic_Foto);
            this.Controls.Add(this.Btn_Aleatorio);
            this.Controls.Add(this.Btn_Selecionar);
            this.Controls.Add(this.Msk_Data);
            this.Controls.Add(this.Lbl_Data);
            this.Controls.Add(this.Lbl_Telefone);
            this.Controls.Add(this.Lbl_Usuario);
            this.Controls.Add(this.Lbl_Email);
            this.Controls.Add(this.Lbl_Nome);
            this.Controls.Add(this.Btn_Salvar);
            this.Controls.Add(this.Txt_Usuario);
            this.Controls.Add(this.Txt_Email);
            this.Controls.Add(this.Txt_Telefone);
            this.Controls.Add(this.Txt_Nome);
            this.Controls.Add(this.Lbl_Titulo);
            this.Name = "Frm_Cadastro";
            this.Text = "Cadastro | Taskbool";
            ((System.ComponentModel.ISupportInitialize)(this.Pic_Foto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lbl_Titulo;
        private System.Windows.Forms.TextBox Txt_Nome;
        private System.Windows.Forms.TextBox Txt_Telefone;
        private System.Windows.Forms.TextBox Txt_Email;
        private System.Windows.Forms.TextBox Txt_Usuario;
        private System.Windows.Forms.Button Btn_Salvar;
        private System.Windows.Forms.Label Lbl_Nome;
        private System.Windows.Forms.Label Lbl_Email;
        private System.Windows.Forms.Label Lbl_Usuario;
        private System.Windows.Forms.Label Lbl_Telefone;
        private System.Windows.Forms.Label Lbl_Data;
        private System.Windows.Forms.MaskedTextBox Msk_Data;
        private System.Windows.Forms.Button Btn_Selecionar;
        private System.Windows.Forms.Button Btn_Aleatorio;
        private System.Windows.Forms.PictureBox Pic_Foto;
        private System.Windows.Forms.Label Lbl_Credencial;
        private System.Windows.Forms.Label Txt_Credencial;
    }
}

