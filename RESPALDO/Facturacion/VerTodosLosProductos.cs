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
    public partial class VerTodosLosProductos : Form
    {
        public DataTable dt;
        public SqlCeDataAdapter da;

        public VerTodosLosProductos(DataTable dt, SqlCeDataAdapter da)
        {
            InitializeComponent();
            this.dt = dt;
            this.da = da;
            this.BuscarAlgoenTabla("SELECT * FROM Productos");           
        }
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
        private void VerTodosLosProductos_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = dt;
        }
    }
}
