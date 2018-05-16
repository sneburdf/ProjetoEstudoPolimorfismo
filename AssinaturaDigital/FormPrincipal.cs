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
using AssinaturaDigital.TO;
using System.IO;

namespace AssinaturaDigital
{
    public partial class FormPrincipal : Form
    {
        private readonly TOCertificadoDigital _certificadoDigital;
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = HelperXml.DadosConfiguracao.UltimoDiretorioLFEs;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                HelperXml.SalvarUltimoDiretorioLFEs(Path.GetDirectoryName(openFileDialog1.FileName));
            }
            textBox1.Text = openFileDialog1.FileName;
            var caminhoArquivo = openFileDialog1.FileName;

            if (!File.Exists(caminhoArquivo))
            {
                MessageBox.Show($"{caminhoArquivo}\n não existe.");                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var certificados = HelperCertificadoDigital.BuscarListaCertificados();
            var caminhoArquivo = openFileDialog1.FileName;
            if (!File.Exists(caminhoArquivo))
            {
                MessageBox.Show($"{caminhoArquivo}\n não existe. Favor selecionar o arquivo");
            }
            var formSelecaoCertificado = new FormSelecaoCertificadoDigital(certificados);
            if (formSelecaoCertificado.ShowDialog() != DialogResult.OK) return;
            try
            {
                HelperCertificadoDigital.AssinarArquivo(formSelecaoCertificado.CertificadoDigitalSelecionado.X509, caminhoArquivo,
                            caminhoArquivo.Replace(".", "-") + ".cas");
                var comp = new HelperCompressao();
                var formOk = new FormOk();
                formOk.ShowDialog();
            }
            catch (Exception er)
            {

                var formErro = new FormErro( er);
                formErro.ShowDialog();
            }
            



        }

    }
}
