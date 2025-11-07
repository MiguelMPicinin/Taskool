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
            this.Txt_Texto = new System.Windows.Forms.TextBox();
            this.Btn_Cores1 = new System.Windows.Forms.Button();
            this.Btn_Cores2 = new System.Windows.Forms.Button();
            this.Btn_Cores3 = new System.Windows.Forms.Button();
            this.Btn_Cores4 = new System.Windows.Forms.Button();
            this.Btn_Ok = new System.Windows.Forms.Button();
            this.Btn_Cancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Txt_Texto
            // 
            this.Txt_Texto.Location = new System.Drawing.Point(20, 60);
            this.Txt_Texto.Multiline = true;
            this.Txt_Texto.Name = "Txt_Texto";
            this.Txt_Texto.Size = new System.Drawing.Size(250, 80);
            this.Txt_Texto.TabIndex = 0;
            // 
            // Btn_Cores1
            // 
            this.Btn_Cores1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(121)))));
            this.Btn_Cores1.Location = new System.Drawing.Point(20, 10);
            this.Btn_Cores1.Name = "Btn_Cores1";
            this.Btn_Cores1.Size = new System.Drawing.Size(40, 40);
            this.Btn_Cores1.TabIndex = 1;
            this.Btn_Cores1.UseVisualStyleBackColor = false;
            this.Btn_Cores1.Click += new System.EventHandler(this.Btn_Cores1_Click);
            // 
            // Btn_Cores2
            // 
            this.Btn_Cores2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(151)))), ((int)(((byte)(202)))));
            this.Btn_Cores2.Location = new System.Drawing.Point(80, 10);
            this.Btn_Cores2.Name = "Btn_Cores2";
            this.Btn_Cores2.Size = new System.Drawing.Size(40, 40);
            this.Btn_Cores2.TabIndex = 2;
            this.Btn_Cores2.UseVisualStyleBackColor = false;
            this.Btn_Cores2.Click += new System.EventHandler(this.Btn_Cores2_Click);
            // 
            // Btn_Cores3
            // 
            this.Btn_Cores3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(211)))), ((int)(((byte)(102)))));
            this.Btn_Cores3.Location = new System.Drawing.Point(140, 10);
            this.Btn_Cores3.Name = "Btn_Cores3";
            this.Btn_Cores3.Size = new System.Drawing.Size(40, 40);
            this.Btn_Cores3.TabIndex = 3;
            this.Btn_Cores3.UseVisualStyleBackColor = false;
            this.Btn_Cores3.Click += new System.EventHandler(this.Btn_Cores3_Click);
            // 
            // Btn_Cores4
            // 
            this.Btn_Cores4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(211)))), ((int)(((byte)(229)))));
            this.Btn_Cores4.Location = new System.Drawing.Point(200, 10);
            this.Btn_Cores4.Name = "Btn_Cores4";
            this.Btn_Cores4.Size = new System.Drawing.Size(40, 40);
            this.Btn_Cores4.TabIndex = 4;
            this.Btn_Cores4.UseVisualStyleBackColor = false;
            this.Btn_Cores4.Click += new System.EventHandler(this.Btn_Cores4_Click);
            // 
            // Btn_Ok
            // 
            this.Btn_Ok.Location = new System.Drawing.Point(115, 150);
            this.Btn_Ok.Name = "Btn_Ok";
            this.Btn_Ok.Size = new System.Drawing.Size(75, 23);
            this.Btn_Ok.TabIndex = 5;
            this.Btn_Ok.Text = "Adicionar";
            this.Btn_Ok.UseVisualStyleBackColor = true;
            this.Btn_Ok.Click += new System.EventHandler(this.Btn_Ok_Click);
            // 
            // Btn_Cancelar
            // 
            this.Btn_Cancelar.Location = new System.Drawing.Point(195, 150);
            this.Btn_Cancelar.Name = "Btn_Cancelar";
            this.Btn_Cancelar.Size = new System.Drawing.Size(75, 23);
            this.Btn_Cancelar.TabIndex = 6;
            this.Btn_Cancelar.Text = "Cancelar";
            this.Btn_Cancelar.UseVisualStyleBackColor = true;
            this.Btn_Cancelar.Click += new System.EventHandler(this.Btn_Cancelar_Click);
            // 
            // Frm_AdicionarPostIt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 182);
            this.Controls.Add(this.Btn_Cancelar);
            this.Controls.Add(this.Btn_Ok);
            this.Controls.Add(this.Btn_Cores4);
            this.Controls.Add(this.Btn_Cores3);
            this.Controls.Add(this.Btn_Cores2);
            this.Controls.Add(this.Btn_Cores1);
            this.Controls.Add(this.Txt_Texto);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_AdicionarPostIt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Adicionar Tarefa";
            this.Load += new System.EventHandler(this.Frm_AdicionarPostIt_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Txt_Texto;
        private System.Windows.Forms.Button Btn_Cores1;
        private System.Windows.Forms.Button Btn_Cores2;
        private System.Windows.Forms.Button Btn_Cores3;
        private System.Windows.Forms.Button Btn_Cores4;
        private System.Windows.Forms.Button Btn_Ok;
        private System.Windows.Forms.Button Btn_Cancelar;
    }
}