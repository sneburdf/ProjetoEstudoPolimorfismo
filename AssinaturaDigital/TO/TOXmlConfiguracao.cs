using System;
using System.Collections.Generic;
using System.Text;

namespace AssinaturaDigital.TO
{
    public class TOXmlConfiguracao
    {
        public TOXmlConfiguracao()
        {
            this.UltimoDiretorioLFEs = string.Empty;
            this.UltimoDiretorioSalvamentoPDFs = string.Empty;
        }

        public int QuantidadeErros { get; set; }

        public enumTipoAutenticacao TipoAutenticacao { get; set; }
        public string Host { get; set; }
        public int Porta { get; set; }
        public bool AutenticacaoAtiva { get; set; }
        public String Usuario { get; set; }
        public String Senha { get; set; }

        /// <summary>Utilizado para salvar o diretório que o usuário abriu a última LFE.</summary>
        public string UltimoDiretorioLFEs { get; set; }
        /// <summary>Utilizado para salvar o diretório que o usuário salvou o último PDF de resumo de LFE.</summary>
        public string UltimoDiretorioSalvamentoPDFs { get; set; }


        public enum enumTipoAutenticacao
        {
            DetectarAutomaticamente,
            SOCKS5
        }
    }
}
