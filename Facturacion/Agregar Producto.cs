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
    public partial class Agregar_Producto : Form
    {
        public DataTable dt;
        public SqlCeDataAdapter da;

        public Agregar_Producto(DataTable dt,SqlCeDataAdapter da)
        {
            InitializeComponent();
            this.dt = dt;
            this.da = da;
        }
        private void Agregar_Producto_Load(object sender, EventArgs e)
        {
            this.AGREGAR_PRODUCTO_fechallegada.Text = System.DateTime.Now.ToString("dd/MM/yyyy");
        }
        private void AGREGAR_PRODUCTO_agregar_Click(object sender, EventArgs e)
        {
            //VERIFICA QUE HAYA INGRESADO TODO
            foreach (Control c in this.groupBox1.Controls)
                if (c is TextBox)
                    if (string.IsNullOrWhiteSpace(c.Text) || string.IsNullOrWhiteSpace(c.Text) || c.Text=="")
                    {
                        MessageBox.Show("Falta ingresar datos del producto, profavor verifique!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

            this.BuscarAlgoenTabla("SELECT * FROM Productos");
            DataRow row = dt.NewRow();
            row["clave"] = AGREGAR_PRODUCTO_clave.Text;            
            row["nombre"] = AGREGAR_PRODUCTO_nombre.Text;
            row["precio"] = AGREGAR_PRODUCTO_precio.Text;
            row["fecha_entrada"] = AGREGAR_PRODUCTO_fechallegada.Text;
            row["vendido"] = "F";
            row["prod_existencia"] = textBox1.Text;
            dt.Rows.Add(row);

            try
            {
                da.Update(dt);
                dt.AcceptChanges();
            }
            catch (DBConcurrencyException ex)
            {
                MessageBox.Show("Error de concurrencia:\n" + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            MessageBox.Show("Articulo agregado correctamente!");
            this.BuscarAlgoenTabla("SELECT * FROM Factura");
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

        private void AGREGAR_PRODUCTO_clave_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }        

        private void AGREGAR_PRODUCTO_precio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void AGREGAR_PRODUCTO_clave_Leave(object sender, EventArgs e)
        {
            this.BuscarAlgoenTabla("SELECT * FROM Productos WHERE clave = " + AGREGAR_PRODUCTO_clave.Text);
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("La clave del producto que ingreso coincide con un producto ya registrado, profavor verifique!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                AGREGAR_PRODUCTO_clave.Text = "";
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Control c in this.groupBox1.Controls)                
                if (!c.Name.Contains("AGREGAR_PRODUCTO_fechallegada") && c is TextBox)
                c.Text = "";
        }
    }
}
