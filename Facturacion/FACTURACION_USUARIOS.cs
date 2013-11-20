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
    public partial class FACTURACION_USUARIOS : Form
    {
        public FACTURACION_USUARIOS()
        {
            InitializeComponent();
            this.conectaBD();
                                      //si ni tiene registros se regresa
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No hay registros en la tabla de usuarios");
                foreach (Control control1 in this.groupBox1.Controls)
                    control1.Enabled = false;
                return;
            }
            //desabilita solo los textbox
            foreach (Control control1 in this.groupBox1.Controls)
                if (control1 is TextBox)
                    control1.Enabled = false;
            
            this.RellenaDatos();
        }
        //VARIABLES GENERALES 
        public DataTable dt;
        public SqlCeDataAdapter da;
        public SqlCeConnection PathBD;
        public OpenFileDialog file = new OpenFileDialog();
        public DataRow row;
        int numero_registro = 0;

        public void conectaBD()
        {
            SqlCeConnection PathBD = new SqlCeConnection("Data Source=C:\\Facturacion\\Facturacion\\BaseDeDatos.sdf;Persist Security Info=False;");
            //abre la conexion
            try
            {
                da = new SqlCeDataAdapter("SELECT * FROM USUARIOS ORDER BY ID_USUARIO", PathBD);
                // Crear los comandos de insertar, actualizar y eliminar
                SqlCeCommandBuilder cb = new SqlCeCommandBuilder(da);
                // Asignar los comandos al DataAdapter
                // (se supone que lo hace automáticamente, pero...)
                da.UpdateCommand = cb.GetUpdateCommand();
                da.InsertCommand = cb.GetInsertCommand();
                da.DeleteCommand = cb.GetDeleteCommand();
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
        public void RellenaDatos()
        {       
            try
            {
                row = dt.Rows[numero_registro];
                FACTURACION_USUARIOS_ID.Text = row["ID_USUARIO"].ToString();
                FACTURACION_USUARIOS_NOMBRE.Text = row["NOMBRE"].ToString();
                FACTURACION_USUARIOS_APELLIDOM.Text = row["APELLIDOM"].ToString();
                FACTURACION_USUARIOS_APELLIDOP.Text = row["APELLIDOP"].ToString();
                FACTURACION_USUARIOS_CORREO.Text = row["CORREO"].ToString();
                FACTURACION_USUARIOS_DIRECCION.Text = row["DIRECCION"].ToString();
                FACTURACION_USUARIOS_PUESTO.Text = row["PUESTO"].ToString();
                FACTURACION_USUARIOS_TELEFONO.Text = row["TELEFONO"].ToString();
                FACTURACION_USUARIOS_SALARIO.Text = row["SALARIO"].ToString();
                FACTURACION_USUARIOS_IMAGEN.ImageLocation = row["FOTOGRAFIA"].ToString();
                FACTURACION_USUARIOS_IMAGEN.Refresh();
            }
            catch (Exception e)
            {
                e.ToString();
                return;
            }
            
        }

        private void FACTURACION_USUARIOS_ATRAS_Click(object sender, EventArgs e)
        {
            if (numero_registro <= 0)
                return;

            numero_registro--;
            this.RellenaDatos();
        }

        private void FACTURACION_USUARIOS_AGREGARIMAGEN_Click(object sender, EventArgs e)
        {
            file.ShowDialog();
            string PathImagen = file.FileName.ToString();
            if (PathImagen.Contains(".jpg") || PathImagen.Contains(".JPEG") || PathImagen.Contains("png.") || PathImagen.Contains(".PNG") || PathImagen.Contains(".JPG"))
                file.FileName = PathImagen;
            else
                MessageBox.Show("El archivo que selecciono, no es una imagen porfavor verifique!!");
        }

        private void FACTURACION_USUARIOS_AGREGAR_Click(object sender, EventArgs e)
        {
            DataRow row = dt.NewRow();
            row["ID_USUARIO"] = FACTURACION_USUARIOS_AGREGAR_IDUSUARIO.Text.ToString();
            row["APELLIDOP"] = FACTURACION_USUARIOS_AGREGAR_APELLIDOP.Text.ToString();
            row["APELLIDOM"] = FACTURACION_USUARIOS_AGREGAR_APELLIDOM.Text.ToString();
            row["NOMBRE"] = FACTURACION_USUARIOS_AGREGAR_NOMBRE.Text.ToString();
            row["DIRECCION"] = FACTURACION_USUARIOS_AGREGAR_DIRECCION.Text.ToString();
            row["CORREO"] = FACTURACION_USUARIOS_AGREGAR_CORREO.Text.ToString();
            row["PUESTO"] = FACTURACION_USUARIOS_AGREGAR_PUESTO.Text.ToString();
            row["SALARIO"] = FACTURACION_USUARIOS_AGREGAR_SALARIO.Text.ToString();
            row["TELEFONO"] = FACTURACION_USUARIOS_AGREGAR_TELEFONO.Text.ToString();
           // file = new OpenFileDialog();
            if (file.FileName != "")
                row["FOTOGRAFIA"] = file.FileName.ToString();
            dt.Rows.Add(row);
            //agrega fisicamente a la BD-31/ago/11
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
              MessageBox.Show("Usuario agregado correctamente!");

              return;
          
        }

        private void FACTURACION_USUARIOS_SIGUIENTE_Click(object sender, EventArgs e)
        {
            if (numero_registro >= dt.Rows.Count-1)
                return;
            
            numero_registro++;
            this.RellenaDatos();
        }

        private void FACTURACION_USUARIOS_BORRAR_Click(object sender, EventArgs e)
        {

        }

        private void FACTURACION_USUARIOS_REGRESAR_Click(object sender, EventArgs e)
        {

        }        
    }
}
