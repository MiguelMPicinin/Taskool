namespace CanvasApp
{
    partial class Frm_AlteraCor
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
            this.Btn_Salvar = new System.Windows.Forms.Button();
            this.Btn_Selecionar = new System.Windows.Forms.Button();
            this.Txt_Hexadecimal = new System.Windows.Forms.TextBox();
            this.Txt_RGB = new System.Windows.Forms.TextBox();
            this.Clr_Windows = new System.Windows.Forms.ColorDialog();
            this.SuspendLayout();
            // 
            // Btn_Salvar
            // 
            this.Btn_Salvar.Location = new System.Drawing.Point(213, 150);
            this.Btn_Salvar.Name = "Btn_Salvar";
            this.Btn_Salvar.Size = new System.Drawing.Size(125, 41);
            this.Btn_Salvar.TabIndex = 1;
            this.Btn_Salvar.Text = "button2";
            this.Btn_Salvar.UseVisualStyleBackColor = true;
            this.Btn_Salvar.Click += new System.EventHandler(this.Btn_Salvar_Click);
            // 
            // Btn_Selecionar
            // 
            this.Btn_Selecionar.Location = new System.Drawing.Point(63, 150);
            this.Btn_Selecionar.Name = "Btn_Selecionar";
            this.Btn_Selecionar.Size = new System.Drawing.Size(125, 41);
            this.Btn_Selecionar.TabIndex = 2;
            this.Btn_Selecionar.Text = "button3";
            this.Btn_Selecionar.UseVisualStyleBackColor = true;
            this.Btn_Selecionar.Click += new System.EventHandler(this.Btn_Selecionar_Click);
            // 
            // Txt_Hexadecimal
            // 
            this.Txt_Hexadecimal.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Txt_Hexadecimal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Txt_Hexadecimal.Location = new System.Drawing.Point(63, 54);
            this.Txt_Hexadecimal.Name = "Txt_Hexadecimal";
            this.Txt_Hexadecimal.Size = new System.Drawing.Size(302, 24);
            this.Txt_Hexadecimal.TabIndex = 3;
            this.Txt_Hexadecimal.BackColorChanged += new System.EventHandler(this.Txt_Hexadecimal_BackColorChanged);
            this.Txt_Hexadecimal.TextChanged += new System.EventHandler(this.Txt_Hexadecimal_TextChanged);
            // 
            // Txt_RGB
            // 
            this.Txt_RGB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Txt_RGB.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Txt_RGB.Location = new System.Drawing.Point(63, 100);
            this.Txt_RGB.Name = "Txt_RGB";
            this.Txt_RGB.Size = new System.Drawing.Size(302, 24);
            this.Txt_RGB.TabIndex = 4;
            this.Txt_RGB.BackColorChanged += new System.EventHandler(this.Txt_RGB_BackColorChanged);
            this.Txt_RGB.TextChanged += new System.EventHandler(this.Txt_RGB_TextChanged);
            // 
            // Frm_AlteraCor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 203);
            this.Controls.Add(this.Txt_RGB);
            this.Controls.Add(this.Txt_Hexadecimal);
            this.Controls.Add(this.Btn_Selecionar);
            this.Controls.Add(this.Btn_Salvar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_AlteraCor";
            this.Text = "AlteraCor";
            this.Load += new System.EventHandler(this.Frm_AlteraCor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_Salvar;
        private System.Windows.Forms.Button Btn_Selecionar;
        private System.Windows.Forms.TextBox Txt_Hexadecimal;
        private System.Windows.Forms.TextBox Txt_RGB;
        private System.Windows.Forms.ColorDialog Clr_Windows;
    }
}