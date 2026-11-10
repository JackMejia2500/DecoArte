namespace DecoArte
{
    partial class Inventario
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            label1 = new Label();
            dgvInventario = new Guna.UI2.WinForms.Guna2DataGridView();
            txtBuscarProducto = new Guna.UI2.WinForms.Guna2TextBox();
            btnActualizar = new Guna.UI2.WinForms.Guna2Button();
            btnBorrar = new Guna.UI2.WinForms.Guna2Button();
            btnExportarExcel = new Guna.UI2.WinForms.Guna2Button();
            guna2GradientPanel1 = new Guna.UI2.WinForms.Guna2GradientPanel();
            ((System.ComponentModel.ISupportInitialize)dgvInventario).BeginInit();
            guna2GradientPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Comic Sans MS", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(28, 33, 30);
            label1.Location = new Point(110, 101);
            label1.Name = "label1";
            label1.Size = new Size(408, 51);
            label1.TabIndex = 0;
            label1.Text = "Gestion de Inventario";
            // 
            // dgvInventario
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvInventario.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvInventario.BackgroundColor = Color.FromArgb(245, 246, 243);
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(245, 246, 243);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(25, 30, 28);
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvInventario.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvInventario.ColumnHeadersHeight = 45;
            dgvInventario.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(245, 246, 243);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(40, 45, 42);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(230, 235, 228);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(25, 30, 28);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvInventario.DefaultCellStyle = dataGridViewCellStyle3;
            dgvInventario.Dock = DockStyle.Fill;
            dgvInventario.GridColor = Color.FromArgb(228, 230, 225);
            dgvInventario.Location = new Point(15, 10);
            dgvInventario.Name = "dgvInventario";
            dgvInventario.RowHeadersVisible = false;
            dgvInventario.RowTemplate.Height = 48;
            dgvInventario.Size = new Size(953, 466);
            dgvInventario.TabIndex = 2;
            dgvInventario.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvInventario.ThemeStyle.BackColor = Color.FromArgb(245, 246, 243);
            dgvInventario.ThemeStyle.GridColor = Color.FromArgb(228, 230, 225);
            dgvInventario.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(245, 246, 243);
            dgvInventario.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dgvInventario.ThemeStyle.HeaderStyle.ForeColor = Color.FromArgb(25, 30, 28);
            dgvInventario.ThemeStyle.HeaderStyle.Height = 45;
            dgvInventario.ThemeStyle.RowsStyle.BackColor = Color.FromArgb(245, 246, 243);
            dgvInventario.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvInventario.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(40, 45, 42);
            dgvInventario.ThemeStyle.RowsStyle.Height = 48;
            dgvInventario.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(230, 235, 228);
            dgvInventario.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(25, 30, 28);
            // 
            // txtBuscarProducto
            // 
            txtBuscarProducto.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtBuscarProducto.BackColor = Color.Transparent;
            txtBuscarProducto.BorderRadius = 10;
            txtBuscarProducto.CustomizableEdges = customizableEdges1;
            txtBuscarProducto.DefaultText = "";
            txtBuscarProducto.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtBuscarProducto.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtBuscarProducto.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtBuscarProducto.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtBuscarProducto.FillColor = Color.FromArgb(242, 244, 240);
            txtBuscarProducto.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtBuscarProducto.Font = new Font("Segoe UI", 9F);
            txtBuscarProducto.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtBuscarProducto.Location = new Point(673, 110);
            txtBuscarProducto.Name = "txtBuscarProducto";
            txtBuscarProducto.PlaceholderText = "🔎 Buscar Producto:";
            txtBuscarProducto.SelectedText = "";
            txtBuscarProducto.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtBuscarProducto.ShadowDecoration.Enabled = true;
            txtBuscarProducto.ShadowDecoration.Shadow = new Padding(0, 2, 5, 0);
            txtBuscarProducto.Size = new Size(280, 42);
            txtBuscarProducto.TabIndex = 3;
            txtBuscarProducto.Click += txtBuscarProducto_TextChanged;
            // 
            // btnActualizar
            // 
            btnActualizar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnActualizar.BackColor = Color.Transparent;
            btnActualizar.BorderRadius = 10;
            btnActualizar.CustomizableEdges = customizableEdges3;
            btnActualizar.DisabledState.BorderColor = Color.DarkGray;
            btnActualizar.DisabledState.CustomBorderColor = Color.DarkGray;
            btnActualizar.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnActualizar.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnActualizar.FillColor = Color.FromArgb(145, 163, 153);
            btnActualizar.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnActualizar.ForeColor = Color.White;
            btnActualizar.Location = new Point(205, 684);
            btnActualizar.Name = "btnActualizar";
            btnActualizar.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnActualizar.ShadowDecoration.Enabled = true;
            btnActualizar.Size = new Size(150, 45);
            btnActualizar.TabIndex = 4;
            btnActualizar.Text = "🔂 Actualizar ";
            btnActualizar.Click += btnActualizar_Click;
            // 
            // btnBorrar
            // 
            btnBorrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnBorrar.BackColor = Color.Transparent;
            btnBorrar.BorderRadius = 10;
            btnBorrar.CustomizableEdges = customizableEdges5;
            btnBorrar.DisabledState.BorderColor = Color.DarkGray;
            btnBorrar.DisabledState.CustomBorderColor = Color.DarkGray;
            btnBorrar.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnBorrar.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnBorrar.FillColor = Color.FromArgb(168, 74, 74);
            btnBorrar.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBorrar.ForeColor = Color.White;
            btnBorrar.Location = new Point(393, 684);
            btnBorrar.Name = "btnBorrar";
            btnBorrar.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnBorrar.ShadowDecoration.Enabled = true;
            btnBorrar.Size = new Size(150, 45);
            btnBorrar.TabIndex = 5;
            btnBorrar.Text = "🗑️ Borrar ";
            btnBorrar.Click += btnBorrar_Click;
            // 
            // btnExportarExcel
            // 
            btnExportarExcel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnExportarExcel.BackColor = Color.Transparent;
            btnExportarExcel.BorderRadius = 10;
            btnExportarExcel.CustomizableEdges = customizableEdges7;
            btnExportarExcel.DisabledState.BorderColor = Color.DarkGray;
            btnExportarExcel.DisabledState.CustomBorderColor = Color.DarkGray;
            btnExportarExcel.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnExportarExcel.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnExportarExcel.FillColor = Color.FromArgb(79, 107, 97);
            btnExportarExcel.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExportarExcel.ForeColor = Color.White;
            btnExportarExcel.Location = new Point(618, 684);
            btnExportarExcel.Name = "btnExportarExcel";
            btnExportarExcel.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnExportarExcel.ShadowDecoration.Enabled = true;
            btnExportarExcel.Size = new Size(150, 45);
            btnExportarExcel.TabIndex = 6;
            btnExportarExcel.Text = "📄 Exportar a Excel ";
            btnExportarExcel.Click += btnExportarExcel_Click;
            // 
            // guna2GradientPanel1
            // 
            guna2GradientPanel1.BackColor = Color.Transparent;
            guna2GradientPanel1.BorderRadius = 15;
            guna2GradientPanel1.Controls.Add(dgvInventario);
            guna2GradientPanel1.CustomizableEdges = customizableEdges9;
            guna2GradientPanel1.FillColor = Color.FromArgb(245, 246, 243);
            guna2GradientPanel1.Location = new Point(12, 170);
            guna2GradientPanel1.Name = "guna2GradientPanel1";
            guna2GradientPanel1.Padding = new Padding(15, 10, 15, 10);
            guna2GradientPanel1.ShadowDecoration.CustomizableEdges = customizableEdges10;
            guna2GradientPanel1.ShadowDecoration.Enabled = true;
            guna2GradientPanel1.Size = new Size(983, 486);
            guna2GradientPanel1.TabIndex = 7;
            // 
            // Inventario
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(206, 212, 202);
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1006, 757);
            Controls.Add(guna2GradientPanel1);
            Controls.Add(label1);
            Controls.Add(btnExportarExcel);
            Controls.Add(btnBorrar);
            Controls.Add(btnActualizar);
            Controls.Add(txtBuscarProducto);
            Name = "Inventario";
            Text = "Inventario";
            ((System.ComponentModel.ISupportInitialize)dgvInventario).EndInit();
            guna2GradientPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Guna.UI2.WinForms.Guna2DataGridView dgvInventario;
        private Guna.UI2.WinForms.Guna2TextBox txtBuscarProducto;
        private Guna.UI2.WinForms.Guna2Button btnActualizar;
        private Guna.UI2.WinForms.Guna2Button btnBorrar;
        private Guna.UI2.WinForms.Guna2Button btnExportarExcel;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel1;
    }
}