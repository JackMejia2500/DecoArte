using DecoArte.Modelos;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DecoArte
{
    public partial class Historico_Compras_y_Servicios : Form
    {
        private ComprasDAO dao = new ComprasDAO();

        public Historico_Compras_y_Servicios()
        {
            InitializeComponent();
        }

        private void Historico_Compras_y_Servicios_Load(object sender, EventArgs e)
        {
            // Ajustes visuales de la tabla
            dgvHistorico.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistorico.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistorico.ReadOnly = true;
            dgvHistorico.AllowUserToAddRows = false;

            // Cargar datos de hoy al abrir
            dtpFiltroFecha.Value = DateTime.Now;
            CargarHistorico(dtpFiltroFecha.Value);
        }

        private void CargarHistorico(DateTime fecha)
        {
            try
            {
                dgvHistorico.DataSource = dao.ObtenerHistoricoPorFecha(fecha);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Se ejecuta automáticamente al cambiar el día en el calendario
        private void dtpFiltroFecha_ValueChanged(object sender, EventArgs e)
        {
            CargarHistorico(dtpFiltroFecha.Value);
        }

        // Botón Actualizar por si se hicieron compras mientras la ventana estaba abierta
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarHistorico(dtpFiltroFecha.Value);
        }

        // Genera un archivo CSV compatible con Excel de forma nativa
        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dgvHistorico.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos en la tabla para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Archivo Excel/CSV (*.csv)|*.csv",
                FileName = "Historico_Compras_" + dtpFiltroFecha.Value.ToString("dd_MM_yyyy") + ".csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    // 1. Extraer encabezados de las columnas
                    string[] headers = new string[dgvHistorico.Columns.Count];
                    for (int i = 0; i < dgvHistorico.Columns.Count; i++)
                    {
                        headers[i] = dgvHistorico.Columns[i].HeaderText;
                    }
                    sb.AppendLine(string.Join(",", headers));

                    // 2. Extraer filas de la tabla
                    for (int i = 0; i < dgvHistorico.Rows.Count; i++)
                    {
                        string[] cells = new string[dgvHistorico.Columns.Count];
                        for (int j = 0; j < dgvHistorico.Columns.Count; j++)
                        {
                            // Reemplazamos las comas internas por espacios para no romper el formato CSV
                            cells[j] = dgvHistorico.Rows[i].Cells[j].Value?.ToString().Replace(",", " ") ?? "";
                        }
                        sb.AppendLine(string.Join(",", cells));
                    }

                    // 3. Escribir y guardar el archivo
                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Reporte exportado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}