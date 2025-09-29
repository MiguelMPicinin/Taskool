namespace CanvasApp
{
    partial class Frm_Autenticacao
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Autenticacao));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Lnk_Teclado = new System.Windows.Forms.LinkLabel();
            this.Lnk_Cadastrar = new System.Windows.Forms.LinkLabel();
            this.Lbl_Credencial = new System.Windows.Forms.Label();
            this.Txt_Login = new System.Windows.Forms.TextBox();
            this.Btn_Entrar = new System.Windows.Forms.Button();
            this.Lbl_User = new System.Windows.Forms.Label();
            this.Lbl_Titulo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(127, 154);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(82, 75);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // Lnk_Teclado
            // 
            this.Lnk_Teclado.AutoSize = true;
            this.Lnk_Teclado.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lnk_Teclado.Location = new System.Drawing.Point(289, 90);
            this.Lnk_Teclado.Name = "Lnk_Teclado";
            this.Lnk_Teclado.Size = new System.Drawing.Size(46, 12);
            this.Lnk_Teclado.TabIndex = 14;
            this.Lnk_Teclado.TabStop = true;
            this.Lnk_Teclado.Text = "linkLabel2";
            this.Lnk_Teclado.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Lnk_Teclado_LinkClicked);
            // 
            // Lnk_Cadastrar
            // 
            this.Lnk_Cadastrar.AutoSize = true;
            this.Lnk_Cadastrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lnk_Cadastrar.Location = new System.Drawing.Point(138, 321);
            this.Lnk_Cadastrar.Name = "Lnk_Cadastrar";
            this.Lnk_Cadastrar.Size = new System.Drawing.Size(64, 15);
            this.Lnk_Cadastrar.TabIndex = 13;
            this.Lnk_Cadastrar.TabStop = true;
            this.Lnk_Cadastrar.Text = "linkLabel1";
            this.Lnk_Cadastrar.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Lnk_Cadastrar_LinkClicked);
            // 
            // Lbl_Credencial
            // 
            this.Lbl_Credencial.AutoSize = true;
            this.Lbl_Credencial.Location = new System.Drawing.Point(145, 138);
            this.Lbl_Credencial.Name = "Lbl_Credencial";
            this.Lbl_Credencial.Size = new System.Drawing.Size(35, 13);
            this.Lbl_Credencial.TabIndex = 12;
            this.Lbl_Credencial.Text = "label2";
            // 
            // Txt_Login
            // 
            this.Txt_Login.Location = new System.Drawing.Point(35, 87);
            this.Txt_Login.Name = "Txt_Login";
            this.Txt_Login.ShortcutsEnabled = false;
            this.Txt_Login.Size = new System.Drawing.Size(248, 20);
            this.Txt_Login.TabIndex = 11;
            this.Txt_Login.TextChanged += new System.EventHandler(this.Txt_Login_TextChanged);
            // 
            // Btn_Entrar
            // 
            this.Btn_Entrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Entrar.Location = new System.Drawing.Point(106, 245);
            this.Btn_Entrar.Name = "Btn_Entrar";
            this.Btn_Entrar.Size = new System.Drawing.Size(126, 35);
            this.Btn_Entrar.TabIndex = 10;
            this.Btn_Entrar.Text = "button1";
            this.Btn_Entrar.UseVisualStyleBackColor = true;
            this.Btn_Entrar.Click += new System.EventHandler(this.Btn_Entrar_Click);
            // 
            // Lbl_User
            // 
            this.Lbl_User.AutoSize = true;
            this.Lbl_User.Location = new System.Drawing.Point(132, 64);
            this.Lbl_User.Name = "Lbl_User";
            this.Lbl_User.Size = new System.Drawing.Size(35, 13);
            this.Lbl_User.TabIndex = 9;
            this.Lbl_User.Text = "label2";
            // 
            // Lbl_Titulo
            // 
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Titulo.Location = new System.Drawing.Point(52, 9);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new System.Drawing.Size(70, 25);
            this.Lbl_Titulo.TabIndex = 8;
            this.Lbl_Titulo.Text = "label1";
            // 
            // Frm_Autenticacao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 343);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Lnk_Teclado);
            this.Controls.Add(this.Lnk_Cadastrar);
            this.Controls.Add(this.Lbl_Credencial);
            this.Controls.Add(this.Txt_Login);
            this.Controls.Add(this.Btn_Entrar);
            this.Controls.Add(this.Lbl_User);
            this.Controls.Add(this.Lbl_Titulo);
            this.Name = "Frm_Autenticacao";
            this.Text = "Autenticação | Taskbool";
            this.Load += new System.EventHandler(this.Autenticacao_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel Lnk_Teclado;
        private System.Windows.Forms.LinkLabel Lnk_Cadastrar;
        private System.Windows.Forms.Label Lbl_Credencial;
        private System.Windows.Forms.TextBox Txt_Login;
        private System.Windows.Forms.Button Btn_Entrar;
        private System.Windows.Forms.Label Lbl_User;
        private System.Windows.Forms.Label Lbl_Titulo;
    }
}