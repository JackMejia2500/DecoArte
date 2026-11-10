namespace DecoArte
{
    public partial class frm_inicio : Form
    {
        public frm_inicio()
        {
            InitializeComponent();
        }

        private void btnSesion_Click(object sender, EventArgs e)
        {
            PanelCentral siguienteFormulario = new PanelCentral();
            siguienteFormulario.Show();
            this.Hide();
        }

        private void frm_inicio_Load(object sender, EventArgs e)
        {
            // 1. Evita que la imagen de fondo parpadee al maximizar
            this.DoubleBuffered = true;

            // 2. Efecto Liquid Glass en el panel (145 de opacidad sobre un tono crema cálido)
            pnlGlass.FillColor = Color.FromArgb(145, 235, 222, 212);

            // 3. Efecto traslúcido más claro para las cajas de texto
            txtUsuario.FillColor = Color.FromArgb(180, 248, 242, 238);
            txtContraseña.FillColor = Color.FromArgb(180, 248, 242, 238);

        }

       
    }
}
