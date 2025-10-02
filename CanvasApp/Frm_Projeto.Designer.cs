namespace CanvasApp
{
    partial class Frm_Projeto
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Projeto));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.Pnl_Titulo = new System.Windows.Forms.Panel();
            this.Lbl_Titulo = new System.Windows.Forms.Label();
            this.Txt_Tarefa = new System.Windows.Forms.TextBox();
            this.Pic_IconPlus = new System.Windows.Forms.PictureBox();
            this.Lst_ListaTarefas = new System.Windows.Forms.Panel();
            this.Lbl_Triste = new System.Windows.Forms.Label();
            this.Pic_Tristesa = new System.Windows.Forms.PictureBox();
            this.Lnk_TarefasConcluidas = new System.Windows.Forms.LinkLabel();
            this.Grp_Status = new System.Windows.Forms.GroupBox();
            this.Flw_Concluidos = new System.Windows.Forms.FlowLayoutPanel();
            this.Flw_Pendentes = new System.Windows.Forms.FlowLayoutPanel();
            this.Pnl_Grafico = new System.Windows.Forms.Panel();
            this.Chrt_Tarefas = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Pnl_Header = new System.Windows.Forms.Panel();
            this.Lbl_Status = new System.Windows.Forms.Label();
            this.Lbl_Concluidas = new System.Windows.Forms.Label();
            this.Lbl_Pendentes = new System.Windows.Forms.Label();
            this.Pnl_MenuLateral1 = new System.Windows.Forms.Panel();
            this.Pnl_NovoProjeto = new System.Windows.Forms.Panel();
            this.Lbl_Novo = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.Flp_Projetos1 = new System.Windows.Forms.FlowLayoutPanel();
            this.Flp_Categorias1 = new System.Windows.Forms.FlowLayoutPanel();
            this.Lbl_ProjetosMenu1 = new System.Windows.Forms.Label();
            this.Lbl_Categorias1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Lbl_TituloMenu = new System.Windows.Forms.Label();
            this.Pnl_Titulo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pic_IconPlus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pic_Tristesa)).BeginInit();
            this.Grp_Status.SuspendLayout();
            this.Pnl_Grafico.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Chrt_Tarefas)).BeginInit();
            this.Pnl_Header.SuspendLayout();
            this.Pnl_MenuLateral1.SuspendLayout();
            this.Pnl_NovoProjeto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Pnl_Titulo
            // 
            this.Pnl_Titulo.BackColor = System.Drawing.SystemColors.Info;
            this.Pnl_Titulo.Controls.Add(this.Lbl_Titulo);
            this.Pnl_Titulo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Pnl_Titulo.Location = new System.Drawing.Point(150, 1);
            this.Pnl_Titulo.Name = "Pnl_Titulo";
            this.Pnl_Titulo.Size = new System.Drawing.Size(648, 39);
            this.Pnl_Titulo.TabIndex = 1;
            // 
            // Lbl_Titulo
            // 
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Titulo.Location = new System.Drawing.Point(16, 5);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new System.Drawing.Size(79, 29);
            this.Lbl_Titulo.TabIndex = 0;
            this.Lbl_Titulo.Text = "label2";
            // 
            // Txt_Tarefa
            // 
            this.Txt_Tarefa.Location = new System.Drawing.Point(180, 82);
            this.Txt_Tarefa.Name = "Txt_Tarefa";
            this.Txt_Tarefa.Size = new System.Drawing.Size(595, 20);
            this.Txt_Tarefa.TabIndex = 2;
            this.Txt_Tarefa.Text = "Adicionar uma tarefa...";
            // 
            // Pic_IconPlus
            // 
            this.Pic_IconPlus.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Pic_IconPlus.Image = ((System.Drawing.Image)(resources.GetObject("Pic_IconPlus.Image")));
            this.Pic_IconPlus.Location = new System.Drawing.Point(164, 82);
            this.Pic_IconPlus.Name = "Pic_IconPlus";
            this.Pic_IconPlus.Size = new System.Drawing.Size(18, 20);
            this.Pic_IconPlus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Pic_IconPlus.TabIndex = 3;
            this.Pic_IconPlus.TabStop = false;
            // 
            // Lst_ListaTarefas
            // 
            this.Lst_ListaTarefas.AutoScroll = true;
            this.Lst_ListaTarefas.Location = new System.Drawing.Point(159, 120);
            this.Lst_ListaTarefas.Name = "Lst_ListaTarefas";
            this.Lst_ListaTarefas.Size = new System.Drawing.Size(618, 172);
            this.Lst_ListaTarefas.TabIndex = 4;
            // 
            // Lbl_Triste
            // 
            this.Lbl_Triste.AutoSize = true;
            this.Lbl_Triste.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Triste.Location = new System.Drawing.Point(441, 170);
            this.Lbl_Triste.Name = "Lbl_Triste";
            this.Lbl_Triste.Size = new System.Drawing.Size(70, 25);
            this.Lbl_Triste.TabIndex = 1;
            this.Lbl_Triste.Text = "label2";
            // 
            // Pic_Tristesa
            // 
            this.Pic_Tristesa.Image = global::CanvasApp.Properties.Resources.rosto_triste;
            this.Pic_Tristesa.Location = new System.Drawing.Point(364, 196);
            this.Pic_Tristesa.Name = "Pic_Tristesa";
            this.Pic_Tristesa.Size = new System.Drawing.Size(220, 161);
            this.Pic_Tristesa.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Pic_Tristesa.TabIndex = 0;
            this.Pic_Tristesa.TabStop = false;
            // 
            // Lnk_TarefasConcluidas
            // 
            this.Lnk_TarefasConcluidas.AutoSize = true;
            this.Lnk_TarefasConcluidas.Location = new System.Drawing.Point(162, 300);
            this.Lnk_TarefasConcluidas.Name = "Lnk_TarefasConcluidas";
            this.Lnk_TarefasConcluidas.Size = new System.Drawing.Size(55, 13);
            this.Lnk_TarefasConcluidas.TabIndex = 5;
            this.Lnk_TarefasConcluidas.TabStop = true;
            this.Lnk_TarefasConcluidas.Text = "linkLabel1";
            // 
            // Grp_Status
            // 
            this.Grp_Status.Controls.Add(this.Lbl_Concluidas);
            this.Grp_Status.Controls.Add(this.Flw_Concluidos);
            this.Grp_Status.Controls.Add(this.Flw_Pendentes);
            this.Grp_Status.Controls.Add(this.Pnl_Grafico);
            this.Grp_Status.Controls.Add(this.Pnl_Header);
            this.Grp_Status.Controls.Add(this.Lbl_Pendentes);
            this.Grp_Status.Location = new System.Drawing.Point(160, 317);
            this.Grp_Status.Name = "Grp_Status";
            this.Grp_Status.Size = new System.Drawing.Size(617, 138);
            this.Grp_Status.TabIndex = 9;
            this.Grp_Status.TabStop = false;
            // 
            // Flw_Concluidos
            // 
            this.Flw_Concluidos.Location = new System.Drawing.Point(109, 100);
            this.Flw_Concluidos.Name = "Flw_Concluidos";
            this.Flw_Concluidos.Size = new System.Drawing.Size(492, 33);
            this.Flw_Concluidos.TabIndex = 3;
            // 
            // Flw_Pendentes
            // 
            this.Flw_Pendentes.Location = new System.Drawing.Point(109, 51);
            this.Flw_Pendentes.Name = "Flw_Pendentes";
            this.Flw_Pendentes.Size = new System.Drawing.Size(492, 33);
            this.Flw_Pendentes.TabIndex = 2;
            this.Flw_Pendentes.Tag = "";
            // 
            // Pnl_Grafico
            // 
            this.Pnl_Grafico.Controls.Add(this.Chrt_Tarefas);
            this.Pnl_Grafico.Location = new System.Drawing.Point(4, 42);
            this.Pnl_Grafico.Name = "Pnl_Grafico";
            this.Pnl_Grafico.Size = new System.Drawing.Size(96, 82);
            this.Pnl_Grafico.TabIndex = 1;
            // 
            // Chrt_Tarefas
            // 
            chartArea3.Name = "ChartArea1";
            this.Chrt_Tarefas.ChartAreas.Add(chartArea3);
            this.Chrt_Tarefas.Dock = System.Windows.Forms.DockStyle.Fill;
            legend3.Name = "Legend1";
            this.Chrt_Tarefas.Legends.Add(legend3);
            this.Chrt_Tarefas.Location = new System.Drawing.Point(0, 0);
            this.Chrt_Tarefas.Name = "Chrt_Tarefas";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.Chrt_Tarefas.Series.Add(series3);
            this.Chrt_Tarefas.Size = new System.Drawing.Size(96, 82);
            this.Chrt_Tarefas.TabIndex = 4;
            this.Chrt_Tarefas.Text = "chart1";
            // 
            // Pnl_Header
            // 
            this.Pnl_Header.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Pnl_Header.Controls.Add(this.Lbl_Status);
            this.Pnl_Header.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Pnl_Header.Location = new System.Drawing.Point(1, 4);
            this.Pnl_Header.Name = "Pnl_Header";
            this.Pnl_Header.Size = new System.Drawing.Size(617, 33);
            this.Pnl_Header.TabIndex = 0;
            // 
            // Lbl_Status
            // 
            this.Lbl_Status.AutoSize = true;
            this.Lbl_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Status.Location = new System.Drawing.Point(275, 4);
            this.Lbl_Status.Name = "Lbl_Status";
            this.Lbl_Status.Size = new System.Drawing.Size(73, 25);
            this.Lbl_Status.TabIndex = 0;
            this.Lbl_Status.Text = "Status";
            // 
            // Lbl_Concluidas
            // 
            this.Lbl_Concluidas.AutoSize = true;
            this.Lbl_Concluidas.Location = new System.Drawing.Point(102, 85);
            this.Lbl_Concluidas.Name = "Lbl_Concluidas";
            this.Lbl_Concluidas.Size = new System.Drawing.Size(59, 13);
            this.Lbl_Concluidas.TabIndex = 11;
            this.Lbl_Concluidas.Text = "Concluidas";
            // 
            // Lbl_Pendentes
            // 
            this.Lbl_Pendentes.AutoSize = true;
            this.Lbl_Pendentes.Location = new System.Drawing.Point(103, 36);
            this.Lbl_Pendentes.Name = "Lbl_Pendentes";
            this.Lbl_Pendentes.Size = new System.Drawing.Size(58, 13);
            this.Lbl_Pendentes.TabIndex = 10;
            this.Lbl_Pendentes.Text = "Pendentes";
            this.Lbl_Pendentes.Click += new System.EventHandler(this.Lbl_Pendentes_Click);
            // 
            // Pnl_MenuLateral1
            // 
            this.Pnl_MenuLateral1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Pnl_MenuLateral1.AutoScroll = true;
            this.Pnl_MenuLateral1.Controls.Add(this.Pnl_NovoProjeto);
            this.Pnl_MenuLateral1.Controls.Add(this.Flp_Projetos1);
            this.Pnl_MenuLateral1.Controls.Add(this.Flp_Categorias1);
            this.Pnl_MenuLateral1.Controls.Add(this.Lbl_ProjetosMenu1);
            this.Pnl_MenuLateral1.Controls.Add(this.Lbl_Categorias1);
            this.Pnl_MenuLateral1.Controls.Add(this.pictureBox1);
            this.Pnl_MenuLateral1.Controls.Add(this.Lbl_TituloMenu);
            this.Pnl_MenuLateral1.Location = new System.Drawing.Point(-6, -8);
            this.Pnl_MenuLateral1.Name = "Pnl_MenuLateral1";
            this.Pnl_MenuLateral1.Size = new System.Drawing.Size(155, 494);
            this.Pnl_MenuLateral1.TabIndex = 16;
            // 
            // Pnl_NovoProjeto
            // 
            this.Pnl_NovoProjeto.Controls.Add(this.Lbl_Novo);
            this.Pnl_NovoProjeto.Controls.Add(this.pictureBox2);
            this.Pnl_NovoProjeto.Location = new System.Drawing.Point(12, 436);
            this.Pnl_NovoProjeto.Name = "Pnl_NovoProjeto";
            this.Pnl_NovoProjeto.Size = new System.Drawing.Size(140, 32);
            this.Pnl_NovoProjeto.TabIndex = 6;
            // 
            // Lbl_Novo
            // 
            this.Lbl_Novo.AutoSize = true;
            this.Lbl_Novo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Novo.Location = new System.Drawing.Point(42, 7);
            this.Lbl_Novo.Name = "Lbl_Novo";
            this.Lbl_Novo.Size = new System.Drawing.Size(96, 18);
            this.Lbl_Novo.TabIndex = 1;
            this.Lbl_Novo.Text = "Novo Projeto";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(1, 1);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(34, 29);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // Flp_Projetos1
            // 
            this.Flp_Projetos1.Location = new System.Drawing.Point(12, 260);
            this.Flp_Projetos1.Name = "Flp_Projetos1";
            this.Flp_Projetos1.Size = new System.Drawing.Size(138, 171);
            this.Flp_Projetos1.TabIndex = 5;
            // 
            // Flp_Categorias1
            // 
            this.Flp_Categorias1.Location = new System.Drawing.Point(12, 96);
            this.Flp_Categorias1.Name = "Flp_Categorias1";
            this.Flp_Categorias1.Size = new System.Drawing.Size(140, 134);
            this.Flp_Categorias1.TabIndex = 4;
            // 
            // Lbl_ProjetosMenu1
            // 
            this.Lbl_ProjetosMenu1.AutoSize = true;
            this.Lbl_ProjetosMenu1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_ProjetosMenu1.Location = new System.Drawing.Point(12, 233);
            this.Lbl_ProjetosMenu1.Name = "Lbl_ProjetosMenu1";
            this.Lbl_ProjetosMenu1.Size = new System.Drawing.Size(111, 24);
            this.Lbl_ProjetosMenu1.TabIndex = 3;
            this.Lbl_ProjetosMenu1.Text = "PROJETOS";
            // 
            // Lbl_Categorias1
            // 
            this.Lbl_Categorias1.AutoSize = true;
            this.Lbl_Categorias1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Categorias1.Location = new System.Drawing.Point(12, 69);
            this.Lbl_Categorias1.Name = "Lbl_Categorias1";
            this.Lbl_Categorias1.Size = new System.Drawing.Size(132, 24);
            this.Lbl_Categorias1.TabIndex = 2;
            this.Lbl_Categorias1.Text = "CATEGORIAS";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 35);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // Lbl_TituloMenu
            // 
            this.Lbl_TituloMenu.AutoSize = true;
            this.Lbl_TituloMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_TituloMenu.Location = new System.Drawing.Point(58, 17);
            this.Lbl_TituloMenu.Name = "Lbl_TituloMenu";
            this.Lbl_TituloMenu.Size = new System.Drawing.Size(88, 25);
            this.Lbl_TituloMenu.TabIndex = 0;
            this.Lbl_TituloMenu.Text = "Taskool";
            // 
            // Frm_Projeto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 471);
            this.Controls.Add(this.Pnl_MenuLateral1);
            this.Controls.Add(this.Lbl_Triste);
            this.Controls.Add(this.Pic_Tristesa);
            this.Controls.Add(this.Grp_Status);
            this.Controls.Add(this.Lnk_TarefasConcluidas);
            this.Controls.Add(this.Lst_ListaTarefas);
            this.Controls.Add(this.Pic_IconPlus);
            this.Controls.Add(this.Txt_Tarefa);
            this.Controls.Add(this.Pnl_Titulo);
            this.Name = "Frm_Projeto";
            this.Text = "Frm_Projeto";
            this.Pnl_Titulo.ResumeLayout(false);
            this.Pnl_Titulo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pic_IconPlus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pic_Tristesa)).EndInit();
            this.Grp_Status.ResumeLayout(false);
            this.Grp_Status.PerformLayout();
            this.Pnl_Grafico.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Chrt_Tarefas)).EndInit();
            this.Pnl_Header.ResumeLayout(false);
            this.Pnl_Header.PerformLayout();
            this.Pnl_MenuLateral1.ResumeLayout(false);
            this.Pnl_MenuLateral1.PerformLayout();
            this.Pnl_NovoProjeto.ResumeLayout(false);
            this.Pnl_NovoProjeto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel Pnl_Titulo;
        private System.Windows.Forms.Label Lbl_Titulo;
        private System.Windows.Forms.TextBox Txt_Tarefa;
        private System.Windows.Forms.PictureBox Pic_IconPlus;
        private System.Windows.Forms.Panel Lst_ListaTarefas;
        private System.Windows.Forms.LinkLabel Lnk_TarefasConcluidas;
        private System.Windows.Forms.GroupBox Grp_Status;
        private System.Windows.Forms.Panel Pnl_Header;
        private System.Windows.Forms.Label Lbl_Status;
        private System.Windows.Forms.Panel Pnl_Grafico;
        private System.Windows.Forms.FlowLayoutPanel Flw_Concluidos;
        private System.Windows.Forms.FlowLayoutPanel Flw_Pendentes;
        private System.Windows.Forms.Label Lbl_Triste;
        private System.Windows.Forms.PictureBox Pic_Tristesa;
        private System.Windows.Forms.DataVisualization.Charting.Chart Chrt_Tarefas;
        private System.Windows.Forms.Label Lbl_Concluidas;
        private System.Windows.Forms.Label Lbl_Pendentes;
        private System.Windows.Forms.Panel Pnl_MenuLateral1;
        private System.Windows.Forms.Panel Pnl_NovoProjeto;
        private System.Windows.Forms.Label Lbl_Novo;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.FlowLayoutPanel Flp_Projetos1;
        private System.Windows.Forms.FlowLayoutPanel Flp_Categorias1;
        private System.Windows.Forms.Label Lbl_ProjetosMenu1;
        private System.Windows.Forms.Label Lbl_Categorias1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label Lbl_TituloMenu;
    }
}