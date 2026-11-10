using DecoArte.Modelos;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DecoArte
{
    public partial class frm_inicio : Form
    {
        public frm_inicio()
        {
            InitializeComponent();
        }

        private void frm_inicio_Load(object sender, EventArgs e)
        {
            try
            {
                this.DoubleBuffered = true;
                pnlGlass.BackColor = Color.Transparent;
                pnlGlass.FillColor = Color.FromArgb(145, 235, 222, 212);
                txtUsuario.FillColor = Color.FromArgb(248, 242, 238);
                txtContraseña.FillColor = Color.FromArgb(248, 242, 238);

                // Que la contraseña se oculte por defecto
                txtContraseña.UseSystemPasswordChar = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Aviso UI: " + ex.Message);
            }
        }

        private void btnSesion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtContraseña.Text))
            {
                MessageBox.Show("Por favor, ingrese su usuario y contraseña.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UsuarioDAO dao = new UsuarioDAO();
                UsuarioEntidad usuarioValido = dao.ValidarLogin(txtUsuario.Text, txtContraseña.Text);

                if (usuarioValido != null)
                {
                    // 1. Guardamos los datos en la memoria global del sistema
                    SesionUsuario.IdUsuario = usuarioValido.IdUsuario;
                    SesionUsuario.NombreUsuario = usuarioValido.NombreUsuario;
                    SesionUsuario.Permisos = usuarioValido.Permisos;

                    // 2. Abrimos el menú principal
                    PanelCentral siguienteFormulario = new PanelCentral();
                    siguienteFormulario.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos. Intente de nuevo.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtContraseña.Clear();
                    txtContraseña.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message, "Fallo Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}