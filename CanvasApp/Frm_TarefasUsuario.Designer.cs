namespace CanvasApp.Forms
{
    partial class Frm_TarefasUsuario
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
            this.Pnl_Filtros = new System.Windows.Forms.Panel();
            this.Btn_Hoje = new System.Windows.Forms.Button();
            this.Btn_Semana = new System.Windows.Forms.Button();
            this.Btn_Mes = new System.Windows.Forms.Button();
            this.Btn_Todas = new System.Windows.Forms.Button();
            this.Btn_Exportar = new System.Windows.Forms.Button();
            this.Btn_Fechar = new System.Windows.Forms.Button();
            this.Flw_Tarefas = new System.Windows.Forms.FlowLayoutPanel();
            this.Grp_Box = new System.Windows.Forms.GroupBox();
            this.Pnl_Filtros.SuspendLayout();
            this.Flw_Tarefas.SuspendLayout();
            this.SuspendLayout();
            // 
            // Lbl_Titulo
            // 
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Titulo.Location = new System.Drawing.Point(20, 20);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new System.Drawing.Size(159, 30);
            this.Lbl_Titulo.TabIndex = 0;
            this.Lbl_Titulo.Text = "Minhas Tarefas";
            this.Lbl_Titulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Pnl_Filtros
            // 
            this.Pnl_Filtros.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Pnl_Filtros.Controls.Add(this.Btn_Fechar);
            this.Pnl_Filtros.Controls.Add(this.Btn_Exportar);
            this.Pnl_Filtros.Controls.Add(this.Btn_Todas);
            this.Pnl_Filtros.Controls.Add(this.Btn_Mes);
            this.Pnl_Filtros.Controls.Add(this.Btn_Semana);
            this.Pnl_Filtros.Controls.Add(this.Btn_Hoje);
            this.Pnl_Filtros.Location = new System.Drawing.Point(20, 70);
            this.Pnl_Filtros.Name = "Pnl_Filtros";
            this.Pnl_Filtros.Size = new System.Drawing.Size(740, 50);
            this.Pnl_Filtros.TabIndex = 1;
            // 
            // Btn_Hoje
            // 
            this.Btn_Hoje.Location = new System.Drawing.Point(10, 10);
            this.Btn_Hoje.Name = "Btn_Hoje";
            this.Btn_Hoje.Size = new System.Drawing.Size(80, 30);
            this.Btn_Hoje.TabIndex = 0;
            this.Btn_Hoje.Tag = "Hoje";
            this.Btn_Hoje.Text = "Hoje";
            this.Btn_Hoje.UseVisualStyleBackColor = true;
            // 
            // Btn_Semana
            // 
            this.Btn_Semana.Location = new System.Drawing.Point(100, 10);
            this.Btn_Semana.Name = "Btn_Semana";
            this.Btn_Semana.Size = new System.Drawing.Size(80, 30);
            this.Btn_Semana.TabIndex = 1;
            this.Btn_Semana.Tag = "semana";
            this.Btn_Semana.Text = "Semana";
            this.Btn_Semana.UseVisualStyleBackColor = true;
            // 
            // Btn_Mes
            // 
            this.Btn_Mes.Location = new System.Drawing.Point(190, 10);
            this.Btn_Mes.Name = "Btn_Mes";
            this.Btn_Mes.Size = new System.Drawing.Size(80, 30);
            this.Btn_Mes.TabIndex = 2;
            this.Btn_Mes.Tag = "mes";
            this.Btn_Mes.Text = "Mês";
            this.Btn_Mes.UseVisualStyleBackColor = true;
            // 
            // Btn_Todas
            // 
            this.Btn_Todas.Location = new System.Drawing.Point(280, 10);
            this.Btn_Todas.Name = "Btn_Todas";
            this.Btn_Todas.Size = new System.Drawing.Size(80, 30);
            this.Btn_Todas.TabIndex = 3;
            this.Btn_Todas.Tag = "todas";
            this.Btn_Todas.Text = "Todas";
            this.Btn_Todas.UseVisualStyleBackColor = true;
            // 
            // Btn_Exportar
            // 
            this.Btn_Exportar.Location = new System.Drawing.Point(500, 10);
            this.Btn_Exportar.Name = "Btn_Exportar";
            this.Btn_Exportar.Size = new System.Drawing.Size(100, 30);
            this.Btn_Exportar.TabIndex = 4;
            this.Btn_Exportar.Text = "Exportar PDF";
            this.Btn_Exportar.UseVisualStyleBackColor = true;
            // 
            // Btn_Fechar
            // 
            this.Btn_Fechar.Location = new System.Drawing.Point(610, 10);
            this.Btn_Fechar.Name = "Btn_Fechar";
            this.Btn_Fechar.Size = new System.Drawing.Size(80, 30);
            this.Btn_Fechar.TabIndex = 5;
            this.Btn_Fechar.Text = "Fechar";
            this.Btn_Fechar.UseVisualStyleBackColor = true;
            // 
            // Flw_Tarefas
            // 
            this.Flw_Tarefas.AutoScroll = true;
            this.Flw_Tarefas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Flw_Tarefas.Controls.Add(this.Grp_Box);
            this.Flw_Tarefas.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.Flw_Tarefas.Location = new System.Drawing.Point(20, 140);
            this.Flw_Tarefas.Name = "Flw_Tarefas";
            this.Flw_Tarefas.Size = new System.Drawing.Size(740, 400);
            this.Flw_Tarefas.TabIndex = 6;
            this.Flw_Tarefas.WrapContents = false;
            // 
            // Grp_Box
            // 
            this.Grp_Box.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Grp_Box.Location = new System.Drawing.Point(3, 3);
            this.Grp_Box.Name = "Grp_Box";
            this.Grp_Box.Size = new System.Drawing.Size(700, 40);
            this.Grp_Box.TabIndex = 0;
            this.Grp_Box.TabStop = false;
            this.Grp_Box.Text = "groupBox1";
            // 
            // Frm_TarefasUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.Flw_Tarefas);
            this.Controls.Add(this.Pnl_Filtros);
            this.Controls.Add(this.Lbl_Titulo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_TarefasUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Minhas Tarefas | Taskool";
            this.Pnl_Filtros.ResumeLayout(false);
            this.Flw_Tarefas.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lbl_Titulo;
        private System.Windows.Forms.Panel Pnl_Filtros;
        private System.Windows.Forms.Button Btn_Mes;
        private System.Windows.Forms.Button Btn_Semana;
        private System.Windows.Forms.Button Btn_Hoje;
        private System.Windows.Forms.Button Btn_Fechar;
        private System.Windows.Forms.Button Btn_Exportar;
        private System.Windows.Forms.Button Btn_Todas;
        private System.Windows.Forms.FlowLayoutPanel Flw_Tarefas;
        private System.Windows.Forms.GroupBox Grp_Box;
    }
}