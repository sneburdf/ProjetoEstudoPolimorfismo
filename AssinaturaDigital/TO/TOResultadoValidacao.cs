using System;
using System.Collections.Generic;
using System.Text;
using AssinaturaDigital.Enums;

namespace AssinaturaDigital.TO
{
    public class TOResultadoValidacao
    {
        public string StatusValidacao { get; set; }
        public Arquivo Arquivo { get; set; }
        public string LogValidacao { get; set; }
        public string XmlResumoLivro { get; set; }
        public string NomeGeradoParaOArquivo { get; set; }
        public ResultadoValidacao Resultado { get; set; }
        public string DataHoraValidacao { get; set; }
        public string DiretorioLocal { get; set; }

    }
}
