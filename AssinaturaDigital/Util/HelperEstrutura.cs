using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Deployment.Application;
using System.Diagnostics;

namespace AssinaturaDigital.Util
{
    public static class HelperEstrutura
    {
        private static string dirUsuario = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static string dirValidador = Path.Combine(dirUsuario, "SEFDFValidador");
        private static string DirDados = Path.Combine(dirValidador, "Dados");
        private static string DirConfig = Path.Combine(dirValidador, "Config");

        //public static string DirExecutavelAtual = AppDomain.CurrentDomain.BaseDirectory;
        public static string DirDadosEncrypt = Path.Combine(DirDados, "Encrypt");
        public static string DirDadosValidados = Path.Combine(DirDados, "Validados");
        public static string DirTemp = Path.Combine(dirValidador, "Temp");
        public static string DirBin = Path.Combine(dirValidador, "Bin");
        public static string DirBin64 = Path.Combine(dirValidador, "Bin\\64");
        public static string DirLog = Path.Combine(dirValidador, "Log");

        public static string PathXmlArquivosValidados = Path.Combine(DirConfig, "ArquivosPendentes.xml");
        public static string PathXmlConfiguracoesUsuario = Path.Combine(DirConfig, "configuracaolfe.xml");
        public static string PathArquivoLogValidacao = Path.Combine(DirLog, "logValidador.log");        
        public static string PathDLLValidador = Path.Combine(DirBin, "neValidador.dll");
        public static string PathDLLValidador64 = Path.Combine(DirBin64, "neValidador.dll");
        public static string PathXmlConfiguracoesValidador = Path.Combine(DirDadosEncrypt, "configuracaolfe.cas");

        private static string PathTxtVersaoClickOnce = Path.Combine(DirConfig, "clickOnceVer.txt");

        /// <summary>
        /// Cria a estrutura de arquivos locais caso algum não exista.
        /// </summary>
        public static void CriarEstruturaPastasLocais()
        {
            Directory.CreateDirectory(DirConfig);
            Directory.CreateDirectory(DirDadosEncrypt);
            Directory.CreateDirectory(DirDadosValidados);
            Directory.CreateDirectory(DirTemp);
            Directory.CreateDirectory(DirBin);
            Directory.CreateDirectory(DirBin64);
            Directory.CreateDirectory(DirLog);

            File.WriteAllText(PathTxtVersaoClickOnce, GetVersaoAtual().ToString());
        }

        /// <summary>
        /// Verifica se a estrutura de arquivos locais está correta para este usuário /
        /// Deleta arquivos quando uma atualização foi feita no aplicativo (clickonce).
        /// </summary>
        public static bool VerificarEstruturaPastasLocais()
        {
            if (!File.Exists(PathTxtVersaoClickOnce) ||
                GetVersaoAtual() > new Version(File.ReadAllText(PathTxtVersaoClickOnce)))
            {
                TentarDeletarArquivosDoDiretorio(DirDados);
                TentarDeletarArquivosDoDiretorio(DirTemp);

                if (File.Exists(PathXmlArquivosValidados))
                    File.Delete(PathXmlArquivosValidados);

                if (File.Exists(PathDLLValidador) && File.Exists(PathDLLValidador64))
                    File.Delete(PathDLLValidador);

                return false;
            }

            return Directory.Exists(DirConfig) &&
                   Directory.Exists(DirDadosEncrypt) &&
                   Directory.Exists(DirDadosValidados) &&
                   Directory.Exists(DirTemp) &&
                   Directory.Exists(DirBin) &&
                   Directory.Exists(DirBin64) &&
                   Directory.Exists(DirLog);
        }

        //private static void GerarXmlVerificacoes()
        //{
        //    Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Validador.Core.DadosAtualizacoes.xml");

        //    byte[] conteudo;

        //    using (BinaryReader br = new BinaryReader(stream))
        //    {
        //        conteudo = br.ReadBytes((int)stream.Length);
        //    }

        //    File.WriteAllBytes(PathXmlAtualizacoes, conteudo);
        //}

        public static void TentarDeletarArquivosDoDiretorio(string PathDiretorio)
        {
            if (!Directory.Exists(PathDiretorio))
                return;

            try
            {
                foreach (string arquivo in Directory.GetFiles(PathDiretorio))
                {
                    File.Delete(arquivo);
                }
            }
            catch { }
        }

        /// <summary>
        /// Quando o arquivo especificado já existir, retorna um nome no formato c:\caminho\nome(1).txt
        /// </summary>
        public static string PegarNomeArquivoDisponivelParaCriacao(string CaminhoENomeArquivo)
        {
            if (!File.Exists(CaminhoENomeArquivo))
                return CaminhoENomeArquivo;

            int diferenciador = 1;
            string novoNome;

            do
            {
                novoNome = string.Format("{0}\\{1}({2}){3}",
                    Path.GetDirectoryName(CaminhoENomeArquivo),
                    Path.GetFileNameWithoutExtension(CaminhoENomeArquivo),
                    diferenciador,
                    Path.GetExtension(CaminhoENomeArquivo));

                diferenciador++;
            }
            while (File.Exists(novoNome));

            return novoNome;
        }
        public static Version GetVersaoAtual()
        {
            Version versao;
            if (ApplicationDeployment.IsNetworkDeployed)
            {

                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            else
            {
                FileVersionInfo fv = System.Diagnostics.FileVersionInfo.GetVersionInfo
                (Assembly.GetExecutingAssembly().Location);
                versao = new Version(fv.FileVersion);
                return versao;
            }
        }


    }
}
