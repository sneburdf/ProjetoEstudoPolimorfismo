namespace AssinaturaDigital
{
    partial class FormErro
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
            this.label1 = new System.Windows.Forms.Label();
            this.BtnDetalhes = new System.Windows.Forms.Button();
            this.BtnFechar = new System.Windows.Forms.Button();
            this.txtDetalhes = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(454, 72);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ocorreu um erro ao executar esta ação. Tente novamente.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnDetalhes
            // 
            this.BtnDetalhes.Location = new System.Drawing.Point(291, 84);
            this.BtnDetalhes.Name = "BtnDetalhes";
            this.BtnDetalhes.Size = new System.Drawing.Size(131, 23);
            this.BtnDetalhes.TabIndex = 1;
            this.BtnDetalhes.Text = "Exibir detalhes do erro";
            this.BtnDetalhes.UseVisualStyleBackColor = true;
            this.BtnDetalhes.Click += new System.EventHandler(this.BtnDetalhes_Click);
            // 
            // BtnFechar
            // 
            this.BtnFechar.Location = new System.Drawing.Point(30, 84);
            this.BtnFechar.Name = "BtnFechar";
            this.BtnFechar.Size = new System.Drawing.Size(131, 23);
            this.BtnFechar.TabIndex = 2;
            this.BtnFechar.Text = "Fechar";
            this.BtnFechar.UseVisualStyleBackColor = true;
            this.BtnFechar.Click += new System.EventHandler(this.BtnFechar_Click);
            // 
            // txtDetalhes
            // 
            this.txtDetalhes.Location = new System.Drawing.Point(15, 131);
            this.txtDetalhes.Multiline = true;
            this.txtDetalhes.Name = "txtDetalhes";
            this.txtDetalhes.ReadOnly = true;
            this.txtDetalhes.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDetalhes.Size = new System.Drawing.Size(454, 230);
            this.txtDetalhes.TabIndex = 4;
            // 
            // FormErro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ClientSize = new System.Drawing.Size(478, 373);
            this.Controls.Add(this.txtDetalhes);
            this.Controls.Add(this.BtnFechar);
            this.Controls.Add(this.BtnDetalhes);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "FormErro";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnDetalhes;
        private System.Windows.Forms.Button BtnFechar;
        private System.Windows.Forms.TextBox txtDetalhes;
    }
}