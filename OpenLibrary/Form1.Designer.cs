namespace OpenLibrary
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cboCategoria;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.DataGridView dgvLivros;
        private System.Windows.Forms.PictureBox picCapa;
        private System.Windows.Forms.DataGridViewImageColumn colCapa;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cboCategoria = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.dgvLivros = new System.Windows.Forms.DataGridView();
            this.colCapa = new System.Windows.Forms.DataGridViewImageColumn();
            this.picCapa = new System.Windows.Forms.PictureBox();
            this.colTitulo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAutor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCategoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colExcluir = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLivros)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCapa)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(16, 16);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(360, 20);
            this.txtSearch.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(384, 14);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 26);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Buscar";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // cboCategoria
            // 
            this.cboCategoria.Location = new System.Drawing.Point(520, 16);
            this.cboCategoria.Name = "cboCategoria";
            this.cboCategoria.Size = new System.Drawing.Size(200, 21);
            this.cboCategoria.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(16, 48);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(704, 20);
            this.lblStatus.TabIndex = 3;
            // 
            // dgvLivros
            // 
            this.dgvLivros.AllowUserToAddRows = false;
            this.dgvLivros.AllowUserToDeleteRows = false;
            this.dgvLivros.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLivros.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLivros.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTitulo,
            this.colAutor,
            this.colAno,
            this.colCategoria,
            this.colExcluir});
            this.dgvLivros.Location = new System.Drawing.Point(152, 71);
            this.dgvLivros.MultiSelect = false;
            this.dgvLivros.Name = "dgvLivros";
            this.dgvLivros.RowTemplate.Height = 64;
            this.dgvLivros.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLivros.Size = new System.Drawing.Size(568, 360);
            this.dgvLivros.TabIndex = 5;
            // 
            // colCapa
            // 
            this.colCapa.FillWeight = 20F;
            this.colCapa.HeaderText = "Capa";
            this.colCapa.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.colCapa.Name = "colCapa";
            // 
            // picCapa
            // 
            this.picCapa.Location = new System.Drawing.Point(16, 72);
            this.picCapa.Name = "picCapa";
            this.picCapa.Size = new System.Drawing.Size(120, 160);
            this.picCapa.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCapa.TabIndex = 4;
            this.picCapa.TabStop = false;
            // 
            // colTitulo
            // 
            this.colTitulo.FillWeight = 83.28172F;
            this.colTitulo.HeaderText = "Título";
            this.colTitulo.Name = "colTitulo";
            // 
            // colAutor
            // 
            this.colAutor.FillWeight = 83.28172F;
            this.colAutor.HeaderText = "Autor";
            this.colAutor.Name = "colAutor";
            // 
            // colAno
            // 
            this.colAno.FillWeight = 83.28172F;
            this.colAno.HeaderText = "Ano";
            this.colAno.Name = "colAno";
            // 
            // colCategoria
            // 
            this.colCategoria.FillWeight = 83.28172F;
            this.colCategoria.HeaderText = "Categoria";
            this.colCategoria.Name = "colCategoria";
            // 
            // colExcluir
            // 
            this.colExcluir.FillWeight = 84.87309F;
            this.colExcluir.HeaderText = "Ações";
            this.colExcluir.Name = "colExcluir";
            this.colExcluir.Text = "Excluir";
            this.colExcluir.UseColumnTextForButtonValue = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 450);
            this.Controls.Add(this.dgvLivros);
            this.Controls.Add(this.picCapa);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.cboCategoria);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Name = "Form1";
            this.Text = "OpenLibrary - Busca de Livros";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLivros)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCapa)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.DataGridViewTextBoxColumn colTitulo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAutor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAno;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategoria;
        private System.Windows.Forms.DataGridViewButtonColumn colExcluir;
    }
}
