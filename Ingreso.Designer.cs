namespace Facturacion
{
    partial class Ingreso
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
            this.label7 = new System.Windows.Forms.Label();
            this.contrasena = new System.Windows.Forms.TextBox();
            this.DatosCliente_cancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Franklin Gothic Medium", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label7.Location = new System.Drawing.Point(52, -4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(248, 34);
            this.label7.TabIndex = 8;
            this.label7.Text = " Ingrese Contraseña";
            // 
            // contrasena
            // 
            this.contrasena.Font = new System.Drawing.Font("Franklin Gothic Medium Cond", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contrasena.Location = new System.Drawing.Point(58, 33);
            this.contrasena.Name = "contrasena";
            this.contrasena.Size = new System.Drawing.Size(240, 22);
            this.contrasena.TabIndex = 9;
            this.contrasena.UseSystemPasswordChar = true;
            this.contrasena.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.contrasena_KeyPress);
            // 
            // DatosCliente_cancelar
            // 
            this.DatosCliente_cancelar.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.DatosCliente_cancelar.Image = global::Facturacion.Properties.Resources.Arrow_Right;
            this.DatosCliente_cancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.DatosCliente_cancelar.Location = new System.Drawing.Point(102, 61);
            this.DatosCliente_cancelar.Name = "DatosCliente_cancelar";
            this.DatosCliente_cancelar.Size = new System.Drawing.Size(141, 34);
            this.DatosCliente_cancelar.TabIndex = 46;
            this.DatosCliente_cancelar.Text = "Ingresar";
            this.DatosCliente_cancelar.UseVisualStyleBackColor = true;
            this.DatosCliente_cancelar.Click += new System.EventHandler(this.DatosCliente_cancelar_Click);
            // 
            // Ingreso
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(353, 102);
            this.Controls.Add(this.DatosCliente_cancelar);
            this.Controls.Add(this.contrasena);
            this.Controls.Add(this.label7);
            this.Name = "Ingreso";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ingreso";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox contrasena;
        private System.Windows.Forms.Button DatosCliente_cancelar;
    }
}