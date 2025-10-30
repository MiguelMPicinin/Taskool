namespace CanvasApp.Formularios_Pop_Ups
{
    partial class Frm_Historico
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
            this.Flw_LayoutHistorico = new System.Windows.Forms.FlowLayoutPanel();
            this.Btn_Fechar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Flw_LayoutHistorico
            // 
            this.Flw_LayoutHistorico.AutoScroll = true;
            this.Flw_LayoutHistorico.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Flw_LayoutHistorico.Location = new System.Drawing.Point(10, 10);
            this.Flw_LayoutHistorico.Name = "Flw_LayoutHistorico";
            this.Flw_LayoutHistorico.Size = new System.Drawing.Size(370, 440);
            this.Flw_LayoutHistorico.TabIndex = 0;
            // 
            // Btn_Fechar
            // 
            this.Btn_Fechar.Location = new System.Drawing.Point(300, 460);
            this.Btn_Fechar.Name = "Btn_Fechar";
            this.Btn_Fechar.Size = new System.Drawing.Size(80, 30);
            this.Btn_Fechar.TabIndex = 0;
            this.Btn_Fechar.Text = "Fechar";
            this.Btn_Fechar.UseVisualStyleBackColor = true;
            // 
            // Frm_Historico
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 493);
            this.Controls.Add(this.Btn_Fechar);
            this.Controls.Add(this.Flw_LayoutHistorico);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Frm_Historico";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Historico de Modificações";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel Flw_LayoutHistorico;
        private System.Windows.Forms.Button Btn_Fechar;
    }
}