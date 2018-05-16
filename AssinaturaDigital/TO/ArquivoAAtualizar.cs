using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using AssinaturaDigital.Util;


namespace AssinaturaDigital.TO
{
    public class ArquivoAAtualizar
    {
        public Uri Url { get; private set; }

        /// <summary> Necessário para o cálculo da porcentagem de conclusão do download</summary>
        public long TamanhoAproximadoEmBytes { get; set; }

        public TipoArquivo TipoArquivo { get; private set; }

        public string NomeArquivo { get { return Path.GetFileName(Url.ToString()); } }

        public string LocalASalvar { get; private set; }

        public ArquivoAAtualizar(string Uri, long TamanhoAproximadoEmBytes)
        {
            this.Url = new Uri(Uri);
            this.TamanhoAproximadoEmBytes = TamanhoAproximadoEmBytes;
            this.TipoArquivo = Path.GetExtension(Uri).ToUpper() == ".XML" ? TipoArquivo.XML : TipoArquivo.DLL;

            this.LocalASalvar = TipoArquivo == TipoArquivo.XML ?
                Path.Combine(HelperEstrutura.DirDadosEncrypt, Path.GetFileNameWithoutExtension(this.NomeArquivo) + ".cas") :
                HelperEstrutura.PathDLLValidador;
        }

        public ArquivoAAtualizar(string Uri, long TamanhoAproximadoEmBytes, string LocalASalvar)
        {
            this.Url = new Uri(Uri);
            this.TamanhoAproximadoEmBytes = TamanhoAproximadoEmBytes;
            this.TipoArquivo = Path.GetExtension(Uri).ToUpper() == ".XML" ? TipoArquivo.XML : TipoArquivo.DLL;

            this.LocalASalvar = LocalASalvar;
        }
    }

    public enum TipoArquivo
    {
        XML,
        DLL
    }
}
