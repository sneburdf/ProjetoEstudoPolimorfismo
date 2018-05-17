using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssinaturaDigital.Util;
using AssinaturaDigital.TO;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace AssinaturaDigital
{
    public class ValidadorExcecao : Exception
    {
        public ValidadorExcecao(String message, Exception exception) : base(message, exception) { }
        public ValidadorExcecao(String message) : base(message) { }

    }

    public class AtualizacoesDownloadExececao : ValidadorExcecao
    {
        public AtualizacoesDownloadExececao(String message, Exception exception)
            : base(message, exception)
        {
        }
        public AtualizacoesDownloadExececao(String message)
            : base(message)
        {
        }
    }


    public class DownloadXmlExcecao : ValidadorExcecao
    {
        public DownloadXmlExcecao(String message, Exception exception)
            : base(message, exception)
        {
        }
        public DownloadXmlExcecao(String message) : base(message) { }
    }
    public class DllCorrompidaExcecao : ValidadorExcecao
    {
        public DllCorrompidaExcecao(String message, Exception exception)
            : base(message, exception)
        {
        }
        public DllCorrompidaExcecao(String message) : base(message) { }
    }
    public class Atualizador
    {
        private XmlDocument XmlConfigRemoto;
        public static string versaoAtual;
        public static string versaoAtualizador;

        private bool HaAtualicacaoXMLs;
        private bool HaAtualizacaoDLL;
#if DEBUG
        //private const string EnderecoAtualizacoes = "http://fazendasrv047:8004/validador/";        
        //private const string EnderecoAtualizacoes = "http://www.livroeletronico.fazenda.df.gov.br/novovalidador/";
        private const string EnderecoAtualizacoes = "http://www.livroeletronico.fazenda.df.gov.br/novovalidadorteste/"; // alfranco
#else
        private const string EnderecoAtualizacoes = "http://www.livroeletronico.fazenda.df.gov.br/novovalidadorteste/";
#endif

        private List<ArquivoAAtualizar> DLLValidador
        {
            get
            {
                return new List<ArquivoAAtualizar>(){
                    new ArquivoAAtualizar(Path.Combine(EnderecoAtualizacoes, "neValidador.dll"), 8500000),
                    new ArquivoAAtualizar(Path.Combine(EnderecoAtualizacoes, "64/neValidador.dll"), 8500000, HelperEstrutura.PathDLLValidador64)

                };

                //return new ArquivoAAtualizar("http://arasrvvirist001:8754/AGnet/Validador/ArquivosAtualizacoes/neValidadorCast.dll", 8500000);
                //return new ArquivoAAtualizar("http://fazendasrv078v:8004/AgNet/Validador/ArquivosAtualizacoes/neValidadorCast.dll", 8500000);

            }
        }

        private List<ArquivoAAtualizar> XMLsValidacao
        {
            get
            {
                return new List<ArquivoAAtualizar>()
                {
                    new ArquivoAAtualizar(Path.Combine(EnderecoAtualizacoes, "configuracaolfe.xml"), 1000),
                    new ArquivoAAtualizar(Path.Combine(EnderecoAtualizacoes, "configuracao.xml"), 1000),
                    new ArquivoAAtualizar(Path.Combine(EnderecoAtualizacoes, "tabelas.xml"), 310000),
                    new ArquivoAAtualizar(Path.Combine(EnderecoAtualizacoes, "campo.xml"), 625000),
                    new ArquivoAAtualizar(Path.Combine(EnderecoAtualizacoes, "registro.xml"), 50000),
                    new ArquivoAAtualizar(Path.Combine(EnderecoAtualizacoes, "registro1.xml"), 8000),
                    new ArquivoAAtualizar(Path.Combine(EnderecoAtualizacoes, "bloco.xml"), 1000),
                    new ArquivoAAtualizar(Path.Combine(EnderecoAtualizacoes, "layout.xml"), 1000),
                };
            }
        }

        private Uri EnderecoXmlConfiguracoes { get { return XMLsValidacao[0].Url; } }

        private void VerificarAtualizacaoDLL()
        {
            if (!File.Exists(HelperEstrutura.PathDLLValidador) && !File.Exists(HelperEstrutura.PathDLLValidador64))
            {
                this.HaAtualizacaoDLL = true;
                return;
            }

            //if (XmlConfigRemoto == null)
            //    XmlConfigRemoto = FazerDownloadXml(EnderecoXmlConfiguracoes.ToString());

            var tags = XmlConfigRemoto.GetElementsByTagName("versao");
            Atualizador.versaoAtual = null;
            for (int i = 0; i < tags.Count; i++)
            {
                var versao = tags.Item(i);
                if (versao.ParentNode.Name.Equals("validador") && versao.ParentNode.ParentNode.Name.Equals("componentes"))
                {
                    Atualizador.versaoAtual = versao.InnerText;
                    break;
                }

            }

            if (Atualizador.versaoAtual == null)
                throw new Exception("Arquivo de Configuração Inválido");

            //string numVersao = Application.ProductVersion;
            //System.IO.File.WriteAllText(@"C:\Versao.txt", ("Versão Validador - ") + numVersao + (" Versão XML - ") + versaoAtual);

            //if (!Atualizador.versaoAtual.Equals(Application.ProductVersion))
            //{

            //    //Se a versão atual não for igual a versão do application, criar rotina para o usuário realizar
            //    //o download do servidor e reinstalar o validador.
            //    FormConfirmarAtualizacao rfm = new FormConfirmarAtualizacao();
            //    if (rfm.ShowDialog() == DialogResult.Cancel)
            //    {
            //        Application.Exit();
            //    }
            //}
            try
            {
                this.HaAtualizacaoDLL = GetVersaoDLLLocal() < new Version(versaoAtual);
            }
            catch (DllCorrompidaExcecao e)
            {
                this.HaAtualizacaoDLL = true;
                //Precisa depois inserir essa excessão em algum log
                //para verificar recorrência.

            }


#if DEBUG
            //   this.HaAtualizacaoDLL = true;
#endif

        }

        private void VerificarSeHaAtualizacaoXMLs()
        {
            try
            {
                //De acordo com o comentário no XML de configuração, deve-se atualizar os XMLs quando houver alteração na versão do APP.
                if (this.HaAtualizacaoDLL)
                {
                    this.HaAtualicacaoXMLs = true;
                    return;
                }

                foreach (var xml in XMLsValidacao)
                {
                    if (!File.Exists(xml.LocalASalvar))
                        this.HaAtualicacaoXMLs = true;
                    return;
                }

                //if (this.XmlConfigRemoto == null)
                //    XmlConfigRemoto = FazerDownloadXml(EnderecoXmlConfiguracoes.ToString());

                bool atualizarSempre = false;
                int versaoXmls = 0;
                XmlNodeList parametros = XmlConfigRemoto.GetElementsByTagName("parametro");

                foreach (XmlNode item in parametros)
                {
                    if (item.Attributes["id"].Value == "6")
                        atualizarSempre = item.Attributes["valor"].Value.Trim().ToUpper().Equals("Y");

                    if (item.Attributes["id"].Value == "7")
                        versaoXmls = int.Parse(item.Attributes["valor"].Value);
                }

                if (atualizarSempre)
                    this.HaAtualicacaoXMLs = true;
                else
                    this.HaAtualicacaoXMLs = GetVersaoXmlsLocal() < versaoXmls;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //GravarLog.gravar("Erro no método: VerificarSeHaAtualizacaoXMLs" + Environment.NewLine + "Mensagem: " + ex.Message);
                //WriteToAnEventLog.gravar(ex.Message);
            }

        }

        public void ExecutarAtualizacao()
        {
            try
            {
                Exception excessaoVerificacao = null;

                //var formCarregamento = new FormCarregamento();

                var backgroundWorker = new BackgroundWorker();

                backgroundWorker.DoWork += (object wSender, DoWorkEventArgs wArgs) =>
                {
                    this.VerificarAtualizacoes();
                };

                backgroundWorker.RunWorkerCompleted += (object cSender, RunWorkerCompletedEventArgs cArgs) =>
                {
                    //formCarregamento.Close();

                    if (cArgs.Error != null)
                        excessaoVerificacao = cArgs.Error;
                    else if (HaAtualizacaoDLL || HaAtualicacaoXMLs)
                        this.Atualizar();
                };

                backgroundWorker.RunWorkerAsync();

                //formCarregamento.ShowDialog();

                if (excessaoVerificacao != null)
                    throw excessaoVerificacao;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void VerificarAtualizacoes()
        {
            try
            {
                //VerificarAtualizacaoDLL();
                VerificarSeHaAtualizacaoXMLs();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void Atualizar()
        {
            try
            {
                long totalBytesArquivos = 0;

                if (HaAtualizacaoDLL)
                    totalBytesArquivos += DLLValidador.Sum(a => a.TamanhoAproximadoEmBytes);

                if (HaAtualicacaoXMLs)
                {
                    foreach (var arquivo in XMLsValidacao)
                        totalBytesArquivos += arquivo.TamanhoAproximadoEmBytes;
                }

                //FormAtualizacoes formAtualizacoes = null;

                //if (HaAtualizacaoDLL)
                //{
                //    HelperDLLNaoGerenciada.LiberarDLLValidador();

                //    formAtualizacoes = new FormAtualizacoes(DLLValidador, totalBytesArquivos);

                //    if (formAtualizacoes.ShowDialog() != DialogResult.OK)
                //        throw formAtualizacoes.ExcecaoInterna;
                //}


                //if (HaAtualicacaoXMLs)
                //{
                //    HelperDLLNaoGerenciada.CarregarDLLValidador();

                //    if (formAtualizacoes == null)
                //        formAtualizacoes = new FormAtualizacoes(XMLsValidacao, totalBytesArquivos);
                //    else
                //        formAtualizacoes.AdicionarNovosArquivos(XMLsValidacao);

                //    if (formAtualizacoes.ShowDialog() != DialogResult.OK)
                //        throw formAtualizacoes.ExcecaoInterna;
                //}

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private Version GetVersaoDLLLocal()
        {
            try
            {
                string version = FileVersionInfo.GetVersionInfo(HelperEstrutura.PathDLLValidador).FileVersion;

                return new Version(version);
            }
            catch (ArgumentNullException e)
            {
                throw new DllCorrompidaExcecao("versão da biblioteca de validação (Dll) local é nula.", e);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new DllCorrompidaExcecao("Um dos componentes da versão da biblioteca de validação (Dll) é menor do que zero", e);
            }
            catch (ArgumentException e)
            {
                throw new DllCorrompidaExcecao("Versão da biblioteca de validação (Dll) possui, ou menos de dois componentes, ou mais de quatro componentes.", e);
            }

            catch (FormatException e)
            {
                throw new DllCorrompidaExcecao("Pelo menos um componente de versão da biblioteca de validação (Dll) não é um inteiro válido.", e);
            }
            catch (OverflowException e)
            {
                throw new DllCorrompidaExcecao("Um dos componentes de versão da biblioteca de validação (Dll) possui um valor maior que o permitido.", e);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro inesperado.", ex);
            }

        }

        private int GetVersaoXmlsLocal()
        {

            if (!File.Exists(HelperEstrutura.PathXmlConfiguracoesValidador))
                return 0;

            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.LoadXml(HelperCriptografia.Descriptografar(File.ReadAllBytes(HelperEstrutura.PathXmlConfiguracoesValidador)));

            int versaoXmls = 0;
            XmlNodeList parametros = xmlConfig.GetElementsByTagName("parametro");

            foreach (XmlNode item in parametros)
            {
                if (item.Attributes["id"].Value == "7")
                    versaoXmls = int.Parse(item.Attributes["valor"].Value);
            }

            return versaoXmls;

        }

        /// <summary>
        ///     Realiza o download do xml com as configurações do validador.
        /// </summary>
        /// <param name="Url"></param>
        /// <returns>
        ///     XmlDocument 
        /// </returns>
        /// 
        public static void checarConexaoDeInternet(Uri url, Exception e)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                throw new Exception("Nenhuma conexão de rede disponível. "
                        + "Por favor configure, ou conecte-se a uma rede e tente novamente. "
                        + url.Host, e);

            }
            bool networkAvailable = false;
            //try
            //{
            //    using (var client = new WebClient())
            //    using (var stream = client.OpenRead("http://www.brasil.gov.br"))
            //    {
            //        networkAvailable = true;
            //    }
            //}
            //catch
            //{ }
            //try
            //{
            //    using (var client = new WebClient())
            //    using (var stream = client.OpenRead("http://www.google.com"))
            //    {
            //        networkAvailable = true;
            //    }
            //}
            //catch
            //{ }
            //if (!networkAvailable)
            //{
            //    throw new Exception("Foi identificado um problema na sua conexão com a internet. "
            //            + url.Host, e);
            //}

        }
        //private XmlDocument FazerDownloadXml(string Url)
        //{
        //    if (String.IsNullOrEmpty(Url))
        //    {
        //        throw new DownloadXmlExcecao("A url informada é nula");
        //    }
        //    //try
        //    //{
        //    //    var xmlDoc = new XmlDocument();

        //    //    using (var client = new WebClient())
        //    //    {
        //    //        var bytes = client.DownloadData(Url + "?_=" + Guid.NewGuid().ToString());

        //    //        using (var memoryStream = new MemoryStream(bytes))
        //    //        {
        //    //            xmlDoc.Load(memoryStream);
        //    //        }
        //    //    }

        //    //    return xmlDoc;
        //    //}
        //    //catch (ArgumentNullException e)
        //    //{
        //    //    throw new DownloadXmlExcecao("O arquivo de retorno é nulo. ", e);
        //    //}
        //    //catch (WebException e)
        //    //{
        //    //    checarConexaoDeInternet(new Uri(Url), e);
        //    //    throw new DownloadXmlExcecao("Houve um erro ao fazer o download, verifique se sua internet encontra-se disponível. não foi possível realizar o download da URL: " + Url, e);
        //    //}
        //    catch (NotSupportedException e)
        //    {
        //        throw new DownloadXmlExcecao("Método chamado em múltiplas instancias ", e);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new DownloadXmlExcecao("Ocorreu um erro inesperado. ", ex);
        //    }

        //}
    }
}

