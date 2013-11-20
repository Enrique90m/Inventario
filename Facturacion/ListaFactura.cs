using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlServerCe;


namespace Facturacion
{
    public partial class ListaFactura : Form
    {

        public DataTable dt;
        public SqlCeDataAdapter da;
        public int proc;

        public ListaFactura(DataTable dt,SqlCeDataAdapter da, int procedimiento)
        {
            InitializeComponent();   
            this.dt = dt;
            this.da = da;
            proc = procedimiento;
        }

        private void ListaFactura_Load(object sender, EventArgs e)
        {
            if(proc == 1)
            this.BuscarAlgoenTabla("SELECT * FROM Factura ORDER BY Num_Factura");   
            else
                this.BuscarAlgoenTabla("SELECT * FROM Clientes ORDER BY NUM_CLIENTE");   

            dataGridView1.DataSource = dt;
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
    }
}
