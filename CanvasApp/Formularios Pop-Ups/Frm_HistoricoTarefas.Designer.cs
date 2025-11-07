namespace CanvasApp.Formularios_Pop_Ups
{
    partial class Frm_HistoricoTarefas
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.Flw_LayoutHistorico = new System.Windows.Forms.FlowLayoutPanel();
            this.Btn_Fechar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Flw_LayoutHistorico
            // 
            this.Flw_LayoutHistorico.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Flw_LayoutHistorico.AutoScroll = true;
            this.Flw_LayoutHistorico.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Flw_LayoutHistorico.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Flw_LayoutHistorico.Location = new System.Drawing.Point(12, 12);
            this.Flw_LayoutHistorico.Name = "Flw_LayoutHistorico";
            this.Flw_LayoutHistorico.Size = new System.Drawing.Size(460, 400);
            this.Flw_LayoutHistorico.TabIndex = 0;
            this.Flw_LayoutHistorico.Resize += new System.EventHandler(this.Flw_LayoutHistorico_Resize);
            // 
            // Btn_Fechar
            // 
            this.Btn_Fechar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Fechar.BackColor = System.Drawing.Color.SteelBlue;
            this.Btn_Fechar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Fechar.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Fechar.ForeColor = System.Drawing.Color.White;
            this.Btn_Fechar.Location = new System.Drawing.Point(382, 418);
            this.Btn_Fechar.Name = "Btn_Fechar";
            this.Btn_Fechar.Size = new System.Drawing.Size(90, 35);
            this.Btn_Fechar.TabIndex = 1;
            this.Btn_Fechar.Text = "Fechar";
            this.Btn_Fechar.UseVisualStyleBackColor = false;
            this.Btn_Fechar.Click += new System.EventHandler(this.Btn_Fechar_Click);
            // 
            // Frm_Historico
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(484, 465);
            this.Controls.Add(this.Btn_Fechar);
            this.Controls.Add(this.Flw_LayoutHistorico);
            this.Font = new System.Drawing.Font("Arial", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_Historico";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Histórico de Modificações";
            this.Load += new System.EventHandler(this.Frm_Historico_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel Flw_LayoutHistorico;
        private System.Windows.Forms.Button Btn_Fechar;
    }
}