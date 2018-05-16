using System;
using System.Collections.Generic;
using System.Text;

namespace AssinaturaDigital.TO
{
    public class TOLogProcessamentos
    {
        public string StatusValidacao { get; set; }
        public string Descricao { get; private set; }
        public string NomeID { get; private set; }
        public Arquivo Arquivo { get; private set; }
        public string NomeArquivo { get; private set; }
        public DateTime DataHora { get; private set; }
        public Exception Exception { get; private set; }

        public TOLogProcessamentos(Arquivo Arquivo, string DescricaoLog)
        {
            this.Arquivo = Arquivo;
            this.Descricao = DescricaoLog;
            this.DataHora = DateTime.Now;
        }

        public TOLogProcessamentos(string NomeArquivo, string DescricaoLog, string StatusValidacao)
        {
            this.NomeArquivo = NomeArquivo;
            this.Descricao = DescricaoLog;
            this.DataHora = DateTime.Now;
            this.StatusValidacao = StatusValidacao;
        }

        public TOLogProcessamentos(Arquivo Arquivo, string DescricaoLog, Exception Exception)
        {
            this.Arquivo = Arquivo;
            this.Descricao = DescricaoLog;
            this.DataHora = DateTime.Now;
            this.Exception = Exception;
        }


    }
}
