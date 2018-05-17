using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AssinaturaDigital.Util;

namespace AssinaturaDigital
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!HelperEstrutura.VerificarEstruturaPastasLocais())
                HelperEstrutura.CriarEstruturaPastasLocais();

            try
            {
                new Atualizador().ExecutarAtualizacao();
            }
            catch (Exception ex)
            {
                new FormErro("Ocorreu um erro de comunicação com o servidor da SEFDF", ex).ShowDialog();
                Application.Exit();
                return;
            }

            HelperDLLNaoGerenciada.CarregarDLLValidador();
            Application.Run(new FormPrincipal());
        }
    }
}
