using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AssinaturaDigital.Util;
using AssinaturaDigital.Properties;

namespace AssinaturaDigital
{
    public partial class FormErro : Form
    {
        private const int ALTURA_DETALHES_FECHADO = 150;
        private const int ALTURA_DETALHES_ABERTO = 400;
        private readonly Exception _exception;
        private readonly string _mensagemErro;
        public FormErro(Exception exception)
        {
            InitializeComponent();

            _exception = exception;
        }
        public FormErro(string mensagemErro, Exception exception)
        {
            InitializeComponent();
            _exception = exception;
            _mensagemErro = mensagemErro;
        }
        private void FormErroLoad(object sender, EventArgs e)
        {
            if (_mensagemErro != null)
            {
                label1.Text = _mensagemErro;
            }

            label1.CentralizarHorizontal();

            Height = ALTURA_DETALHES_FECHADO;

            txtDetalhes.Text = PreencherDetalhesErro();
        }
        private void BtnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void BtnDetalhes_Click(object sender, EventArgs e)
        {
            Height = Height == ALTURA_DETALHES_FECHADO ? ALTURA_DETALHES_ABERTO : ALTURA_DETALHES_FECHADO;
        }
        private string PreencherDetalhesErro()
        {
            var builder = new StringBuilder();

            var exceptionAtual = _exception;

            do
            {
                builder.AppendLine(exceptionAtual.GetType().ToString());

                builder.AppendLine();

                builder.AppendLine(exceptionAtual.Message);

                builder.AppendLine();
                builder.AppendLine();

                builder.AppendLine(exceptionAtual.StackTrace);

                if (exceptionAtual.InnerException != null)
                {
                    builder.AppendLine();
                    builder.AppendLine();

                    builder.AppendLine("Inner _exception:");
                    builder.AppendLine();
                }

                exceptionAtual = exceptionAtual.InnerException;
            } while (exceptionAtual != null);

            return builder.ToString();
        }
    }
}
