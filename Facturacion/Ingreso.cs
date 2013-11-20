using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.SqlServerCe;

namespace Facturacion
{
    public partial class Ingreso : Form
    {
        public Ingreso()
        {
            InitializeComponent();
        }

        public DataTable dt;
        public SqlCeDataAdapter da;     

        private void BuscarAlgoenTabla(string comando)
        {

            SqlCeConnection PathBD = new SqlCeConnection("Data Source=C:\\Facturacion\\Facturacion\\BaseDeDatos.sdf;Persist Security Info=False;");
            //abre la conexion
            try
            {
                da = new SqlCeDataAdapter(comando, PathBD);

                // Crear los comandos de insertar, actualizar y eliminar
                SqlCeCommandBuilder cb = new SqlCeCommandBuilder(da);
                dt = new DataTable();
                // Llenar la tabla con los datos indicados
                da.Fill(dt);

                PathBD.Open();
            }
            catch (Exception w)
            {
                MessageBox.Show(w.ToString());
                return;
            }
        }

        private void DatosCliente_cancelar_Click(object sender, EventArgs e)
        {
            if (contrasena.Text == "" || string.IsNullOrWhiteSpace(contrasena.Text) )
                return;

            string contrasena1 = contrasena.Text;
            this.BuscarAlgoenTabla("SELECT * FROM USUARIOS WHERE CONTRASENA = " + contrasena1);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Error contraseña incorrecta, porfavor verifique!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            string usuario = "- Nombre : ";
            usuario = dt.Rows[0]["NOMBRE"].ToString() + " " + dt.Rows[0]["APELLIDOP"].ToString() + " -Puesto: " + dt.Rows[0]["PUESTO"].ToString();
            Factura fac = new Factura(usuario);
            fac.Show();
            this.Hide();
        }

        private void contrasena_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
            if (e.KeyChar == (char)13)
            {
                if (contrasena.Text == "" || string.IsNullOrWhiteSpace(contrasena.Text))
                    return;

                string contrasena1 = contrasena.Text;
                this.BuscarAlgoenTabla("SELECT * FROM USUARIOS WHERE CONTRASENA = " + contrasena1);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Error contraseña incorrecta, porfavor verifique!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                string usuario = "- Nombre : ";
                usuario = dt.Rows[0]["NOMBRE"].ToString() + " " + dt.Rows[0]["APELLIDOP"].ToString() + " -Puesto: " + dt.Rows[0]["PUESTO"].ToString();
                Factura fac = new Factura(usuario);
                fac.Show();
                this.Hide();
            }
                
        }
    }
}
