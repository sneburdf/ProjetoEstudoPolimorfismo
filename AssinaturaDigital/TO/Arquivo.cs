using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Configuration;
using System.Text;
using System.IO;
using AssinaturaDigital.Util;

namespace AssinaturaDigital.TO
{
    public class Arquivo
    {
        #region Proprieades

        public string CaminhoENome { get; private set; }
        public string Nome { get { return Path.GetFileName(CaminhoENome); } }

        public string Hash { get; private set; }
        public bool ValidadoComSucesso { get; set; }
        public string NomeGeradoParaOArquivo { get; set; }
        public bool SimplesNacional { get; set; }
        public bool ArquivoBaixado { get; set; }
        public bool ProcuracaoDigital { get; set; }
        public string DataHoraValidacao { get; set; }

        private string _caminhoENomeValidado;
        public string CaminhoENomeValidado
        {
            get
            {
                if (_caminhoENomeValidado == null)
                    _caminhoENomeValidado = HelperEstrutura.PegarNomeArquivoDisponivelParaCriacao(Path.Combine(HelperEstrutura.DirDadosValidados, this.Nome));

                return _caminhoENomeValidado;
            }
            private set { _caminhoENomeValidado = value; }
        }

        public string CaminhoENomeCompressao { get { return Path.Combine(HelperEstrutura.DirTemp, Path.GetFileNameWithoutExtension(CaminhoENome) + ".cas"); } }
        public string CaminhoENomeAssinatura { get { return (CaminhoENomeValidado + ".ass"); } }
        public string CaminhoENomeResumoLivro { get { return Path.Combine(Path.GetDirectoryName(CaminhoENomeValidado), Path.GetFileNameWithoutExtension(CaminhoENomeValidado) + ".rl"); } }
        public string CaminhoENomeLogValidacao { get { return Path.Combine(Path.GetDirectoryName(CaminhoENomeValidado), Path.GetFileNameWithoutExtension(CaminhoENomeValidado) + "_log.txt"); } }

        public string CFDF
        {
            get
            {
                return this.PegarCampoArquivoPrimeiraLinha(15);
            }
        }

        public string CNPJ
        {
            get
            {
                return this.PegarCampoArquivoPrimeiraLinha(11);
            }
        }

        public string CPF
        {
            get
            {
                return this.PegarCampoArquivoPrimeiraLinha(12);
            }
        }

        public string NomeEmpresa
        {
            get
            {
                return this.PegarCampoArquivoPrimeiraLinha(10);
            }
        }

        public string PeriodoReferencia
        {
            get
            {
                string strPeriodo = this.PegarCampoArquivoPrimeiraLinha(6);
                if (strPeriodo != null && strPeriodo.Length == 8)
                    return string.Format("{0}{1}", strPeriodo.Substring(4, 4), strPeriodo.Substring(2, 2));
                else
                    return null;
            }
        }

        public string TipoLivro
        {
            get
            {
                string strTipo = this.PegarCampoArquivoPrimeiraLinha(5);

                if (strTipo == "00")
                    return "Normal";
                else if (strTipo == "01")
                    return "Retificadora";
                else
                    return string.Empty;
            }
        }


        #endregion
        public class ExcecaoArquivoValidadoNaoEncontrado : FileNotFoundException
        {
            public ExcecaoArquivoValidadoNaoEncontrado(String mensagem) : base(mensagem) { }
        }
        public Arquivo(string CaminhoENome)
        {
            if (!File.Exists(CaminhoENome))
                throw new ExcecaoArquivoValidadoNaoEncontrado("Arquivo não encontrado");



            this.CaminhoENome = CaminhoENome;
        }

        public Arquivo(String DataHoraValidacao, string CaminhoENome, string CaminhoENomeValidado, string Hash, bool ValidadoComSucesso, string NomeGeradoParaOArquivo)
            : this(CaminhoENome, CaminhoENomeValidado, Hash, ValidadoComSucesso, NomeGeradoParaOArquivo)
        {

            this.DataHoraValidacao = DataHoraValidacao;
        }

        public Arquivo(string CaminhoENome, string CaminhoENomeValidado, string Hash, bool ValidadoComSucesso, string NomeGeradoParaOArquivo)
            : this(CaminhoENome)
        {
            this.CaminhoENomeValidado = CaminhoENomeValidado;
            this.Hash = Hash;
            this.ValidadoComSucesso = ValidadoComSucesso;
            this.NomeGeradoParaOArquivo = NomeGeradoParaOArquivo;
        }

        public void GerarHashDoArquivo()
        {
            this.Hash = HelperCriptografia.GerarHashDeArquivo(this.CaminhoENome);
        }

        private string PegarCampoArquivoPrimeiraLinha(int Posicao)
        {
            using (StreamReader file = new StreamReader(this.CaminhoENome, Encoding.Default))
            {
                try
                {
                    var array = file.ReadLine().Split('|');
                    if (array.Length == 20)
                        return array[Posicao];
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        public string ObterCampoDaLinha(int Posicao)
        {
            using (StreamReader file = new StreamReader(this.CaminhoENome, Encoding.Default))
            {
                try
                {
                    var array = file.ReadLine().Split('|');
                    if (array.Length == 20)
                        return array[Posicao];
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        public void ValidarCodificacao(string caminho)
        {

            using (BinaryReader file = new BinaryReader(File.Open(caminho, FileMode.Open)))
            {
                byte b = 0;
                int linha = 1;
                do
                {
                    b = file.ReadByte();
                    PegarCaracterInvalido(b, CaractersExcecoesValidos(), linha, CaractersExcecoes());
                    if (b == 13)
                    {
                        linha++;
                    }
                } while (file.BaseStream.Position < file.BaseStream.Length);
            }
        }

        public string ValidarCampos(Arquivo arquivo)
        {
            TOCampos resumo = new TOCampos();
            
           resumo = resumo.CarregarRegistros(arquivo.CaminhoENome);


            var campo1 = Convert.ToDecimal("0" + resumo.RegistroB470.VlIssqnREC, new CultureInfo("pt-BR")) != 0;
            var campo2 = Convert.ToDecimal("0" + resumo.RegistroE360.Vl01, new CultureInfo("pt-BR")) != 0;
            var campo3 = Convert.ToDecimal("0" + resumo.RegistroE360.Vl05, new CultureInfo("pt-BR")) >
                         Convert.ToDecimal("0" + resumo.RegistroE360.Vl03, new CultureInfo("pt-BR"));
            var mensagemErro = new StringBuilder();

            if (campo2)
            {
                mensagemErro.AppendLine(
                    "--@ ERRO. Contribuinte do Simples Nacional deve escriturar o arquivo do livro fiscal eletrônico conforme disposto no arquivo 10-C da Portaria 210/2016. Campo 2 do registro E360(VL_01 - Valor total dos débitos por " +
                    "Saídas e prestações com débito do imposto) deve ser igual a zero.");
                mensagemErro.AppendLine();
            }

            if (campo1)
            {
                mensagemErro.AppendLine(
                    "--@ ERRO. Contribuinte do Simples Nacional deve escriturar o arquivo do livro fiscal eletrônico conforme disposto no arquivo 10-C da Portaria 210/2016. Campo 13 do registro B470(VL_ISS_REC – Valor total apurado do ISS a recolher) " +
                    "deve ser igual a zero.");
                mensagemErro.AppendLine();
            }

            if (campo3)
            {
                mensagemErro.AppendLine(
                    "--@ ERRO. Contribuinte do Simples Nacional deve escriturar o arquivo do livro fiscal eletrônico conforme disposto no arquivo 10-C da Portaria 210/2006. O valor escriturado no campo 6 do registro E360 (VL_5 – Valor total dos créditos por " +
                    "“Entradas e aquisições com crédito do imposto”) não pode ser maior que o valor escriturado no campo 4 do registro E360 ( VL_03 - Valor total de “Estornos de crédito”).");
                mensagemErro.AppendLine();
            }

            return mensagemErro.ToString();
        }

        public void PegarCaracterInvalido(byte caracter, List<int> caracteresExcecoesValidos, int linha, List<int> caractersExcecoes)
        {

            if (((caracter < 32 || caracter > 126) && (caracter < 160 || caracter > 255)) && !caracteresExcecoesValidos.Contains(caracter) || caractersExcecoes.Contains(caracter))
                throw new Exception(string.Format("O arquivo não está na codificação \"ISO 8859-1\" verifique o {0}({1}), na linha {2}", (char)caracter, caracter, linha));
        }

        public List<int> CaractersExcecoesValidos()
        {
            return new List<int>{
                13,  //(/r)
                10,  //(/n)
            };
        }

        public List<int> CaractersExcecoes()
        {
            return new List<int>{
                161,
                162,
                163,
                164,
                165,
               171,
               187,
               191,
               239
            };
        }
    }
}
