using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows.Forms;
using AssinaturaDigital.TO;
using AssinaturaDigital.Util;
using AssinaturaDigital.Properties;

namespace AssinaturaDigital
{
    public partial class FormSelecaoCertificadoDigital : Form
    {
        private readonly List<TOCertificadoDigital> _certificadosDigitais;
        public TOCertificadoDigital CertificadoDigitalSelecionado;
        public FormSelecaoCertificadoDigital(List<TOCertificadoDigital> certificadosDigitais)
        {
            InitializeComponent();
            _certificadosDigitais = certificadosDigitais;
        }
        private void FormSelecaoCertificadoDigitalLoad(object sender, EventArgs e)
        {
            foreach (var certificado in _certificadosDigitais)
            {
                var newRow = new DataGridViewRow();
                if (certificado.isECnpj(certificado.X509))
                {
                    newRow.Cells.Add(new DataGridViewTextBoxCell
                    {
                        Value =
                            !string.IsNullOrEmpty(certificado.NomePessoaFisica)
                                ? certificado.NomePessoaFisica
                                : certificado.IdentificacaoSocial
                    });
                    newRow.Cells.Add(new DataGridViewTextBoxCell
                    {
                        Value = certificado.cnpj.Insert(12, "-").Insert(8, "/").Insert(5, ".").Insert(2, ".")
                    });
                }
                else
                {
                    newRow.Cells.Add(new DataGridViewTextBoxCell
                    {
                        Value =
                            !string.IsNullOrEmpty(certificado.NomePessoaFisica)
                                ? certificado.NomePessoaFisica
                                : certificado.IdentificacaoSocial
                    });
                    newRow.Cells.Add(new DataGridViewTextBoxCell
                    {
                        Value =
                            certificado.CPF.Replace("x", "")
                                .Replace("X", "")
                                .Insert(3, ".")
                                .Insert(7, ".")
                                .Insert(11, "-")
                    });
                }
                dataGridView.Rows.Add(newRow);
            }
        }
        private void BtnSelecionarClick(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show(Resources.FormSelecaoCertificadoDigital_btnSelecionar_Click_Selecione_um_Certificado_Digital);
                return;
            }

            var index = dataGridView.Rows.IndexOf(dataGridView.SelectedRows[0]);

            var certSelecionado = _certificadosDigitais[index];

            DateTime dataHoraServidor;
            //try
            //{
            //    using (var client = new ClienteValidacaoLVR(certSelecionado))
            //        dataHoraServidor = client.PegarHoraServidorSEF();
            //}
            //catch (Exception ex)
            //{
            //    new FormErro(ex).ShowDialog();
            //    return;
            //}
            dataHoraServidor = DateTime.Now;

            if (!HelperCertificadoDigital.ValidarCertificado(certSelecionado.X509, dataHoraServidor))
            {
                MessageBox.Show(Resources.FormSelecaoCertificadoDigital_BtnSelecionarClick_Este_certificado_digital_não_está_válido_);
                return;
            }

            try
            {
                if (!HelperCertificadoDigital.FazerLoginToken(certSelecionado.X509)) return;
            }
            catch (CryptographicException ex)
            {
                const string erroCrypt = "O certificado não pôde ser validado. Por favor, verifique se ele está inserido no computador e tente novamente.";
                new FormErro(new CryptographicException(erroCrypt, ex)).ShowDialog();
                return;
            }

            CertificadoDigitalSelecionado = certSelecionado;

            DialogResult = DialogResult.OK;
        }

        private void BtnCancelarClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
