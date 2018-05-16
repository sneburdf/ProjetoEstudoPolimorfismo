using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using AssinaturaDigital.Util;

namespace AssinaturaDigital.TO
{
    [Serializable]
    [XmlRoot("RegistroLivro")]
    public class TOResumoLivro
    {
        public TOResumoLivro()
        {
            Guid = this.GetHashCode().ToString();
            DadosLivro = new TODadosLivro()
            {
                ListaLinhasBloco = new List<TODadosLivro.TOQtdLinhasBloco>()
            };
            DadosEnvio = new TODadosEnvio();
            ApuracaoICMS = new TOApuracaoICMS();
            SaldoDoISSARecolher = new TOSaldoDoISSRecolher();
            ListaICMSsPorCSOP = new List<TOICMSPorCSOP>();
        }

        [XmlElement(ElementName = "Guid")]
        public String Guid { get; set; }

        [XmlElement("DadosLivro")]
        public TODadosLivro DadosLivro { get; set; }
        [XmlElement("DadosEnvio")]
        public TODadosEnvio DadosEnvio { get; set; }
        [XmlElement("ApuracaoICMS")]
        public TOApuracaoICMS ApuracaoICMS { get; set; }
        [XmlElement("SaldoISSRecolher")]
        public TOSaldoDoISSRecolher SaldoDoISSARecolher { get; set; }
        [XmlElement("ICMSporCFOP")]
        public List<TOICMSPorCSOP> ListaICMSsPorCSOP { get; set; }

        [XmlIgnore]
        public List<TODadosLivro.QtdLinhasBloco> ListaDescricoesLinhasPorBloco 
        { 
            get
            {
                var lista = new List<TODadosLivro.QtdLinhasBloco>();
                string linha = "";
                int countPorLinha = 0;
                foreach(var item in DadosLivro.ListaLinhasBloco)
                {
                    if (countPorLinha < 5)
                    {
                        linha += string.Format("\"{0}\": {1} linhas   |   ", item.CodigoBloco, item.LinhasBloco);
                        countPorLinha++;
                    }
                    if (countPorLinha == 5 || 
                        DadosLivro.ListaLinhasBloco.IndexOf(item) == DadosLivro.ListaLinhasBloco.Count - 1)
                    {
                        lista.Add(new TODadosLivro.QtdLinhasBloco(linha.Trim().Trim('|').Trim()));
                        countPorLinha = 0;
                        linha = "";
                    }
                }

                return lista;
            }
        }

        #region Propriedades dos Totais

        public string TotalICMSValorContabil
        {
            get
            {
                if (ListaICMSsPorCSOP.Count == 0)
                    return string.Empty;

                decimal total = 0;
                foreach (var item in ListaICMSsPorCSOP)
                    total += Convert.ToDecimal(HelperConversao.FormatarValorDecimal(item.ValorContabil, true), new CultureInfo("pt-BR"));

                return HelperConversao.FormatarValorDecimal(total, true);
            }
        }
        public string TotalICMSBaseCalculo
        {
            get
            {
                if (ListaICMSsPorCSOP.Count == 0)
                    return string.Empty;

                decimal total = 0;
                foreach (var item in ListaICMSsPorCSOP)
                    total += Convert.ToDecimal(HelperConversao.FormatarValorDecimal(item.ValorBaseCalculo, true), new CultureInfo("pt-BR"));

                return HelperConversao.FormatarValorDecimal(total, true);
            }
        }
        public string TotalICMSImpostoDebitado
        {
            get
            {
                if (ListaICMSsPorCSOP.Count == 0)
                    return string.Empty;

                decimal total = 0;
                foreach (var item in ListaICMSsPorCSOP)
                    total += Convert.ToDecimal(HelperConversao.FormatarValorDecimal(item.ValorICMS, true), new CultureInfo("pt-BR"));

                return HelperConversao.FormatarValorDecimal(total, true);
            }
        }
        public string TotalICMSValorOperacoesIsentas
        {
            get
            {
                if (ListaICMSsPorCSOP.Count == 0)
                    return string.Empty;

                decimal total = 0;
                foreach (var item in ListaICMSsPorCSOP)
                    total += Convert.ToDecimal(HelperConversao.FormatarValorDecimal(item.ValorOperacoesIsentas, true), new CultureInfo("pt-BR"));

                return HelperConversao.FormatarValorDecimal(total, true);
            }
        }
        public string TotalICMSValorOutrasOperacoes
        {
            get
            {
                if (ListaICMSsPorCSOP.Count == 0)
                    return string.Empty;

                decimal total = 0;
                foreach (var item in ListaICMSsPorCSOP)
                    total += Convert.ToDecimal(HelperConversao.FormatarValorDecimal(item.ValorOutrasOperacoes, true), new CultureInfo("pt-BR"));
                
                return HelperConversao.FormatarValorDecimal(total, true);
            }
        }
        

        #endregion

    }

    [Serializable]
    [XmlRootAttribute("DadosLivro")]
    public class TODadosLivro
    {
        [XmlElement(ElementName = "CF_DF")]
        public string CF_DF { get; set; }

        public string CNPJ { get; set; }

        public string CPF { get; set; }

        private string _PeriodoReferencia;

        [XmlElement(ElementName = "IdadeDaPessoa")]
        public string PeriodoReferencia
        {
            get 
            {
                if (_PeriodoReferencia != null && !_PeriodoReferencia.Contains("/"))
                    return _PeriodoReferencia.Substring(4, 2) + "/" + _PeriodoReferencia.Substring(0, 4);
                else
                    return _PeriodoReferencia;
            }
            set { _PeriodoReferencia = value; }
        }

        [XmlElement(ElementName = "RazaoSocial")]
        public string RazaoSocial { get; set; }

        private string _TipoEmpresa;
        //(N ou R) normal ou retificadora;
        [XmlElement(ElementName = "TipoEmpresa")]
        public string TipoEmpresa
        {
            get { return (_TipoEmpresa == "00" || _TipoEmpresa == "N") ? "N" : "R"; ; }
            set { _TipoEmpresa = value; }
        }

        //- Quantidade total de linhas do arquivo;
        [XmlElement(ElementName = "QtdLinhasArquivo")]
        public int QtdLinhasArquivo { get; set; }

        //Quantidade de linhas por bloco;
        [XmlElement(ElementName = "QtdLinhasBloco")]
        public List<TOQtdLinhasBloco> ListaLinhasBloco { get; set; }

        [Serializable]
        public class TOQtdLinhasBloco
        {
            [XmlElement(ElementName = "CodigoBloco")]
            public string CodigoBloco { get; set; }

            [XmlElement(ElementName = "LinhasBloco")]
            public int LinhasBloco { get; set; }
        }

        public class QtdLinhasBloco
        {
            public QtdLinhasBloco() { }

            public QtdLinhasBloco(string Descricao)
            {
                this.DescricaoQtdLinhas = Descricao;
            }

            public string DescricaoQtdLinhas { get; set; }
        }
    }

    [Serializable]
    [XmlRoot("ApuracaoICMS")]
    public class TOApuracaoICMS
    {
        public TOApuracaoICMS() { }

        [XmlElement(ElementName = "PeriodoEnvioTransmissao")]
        public string PeriodoEnvioTransmissao { get; set; }

        private string _ValDebitoSaida_01;
        //01 - Valor total dos débitos por "Saída e prestações com débito do imposto"				
        [XmlElement(ElementName = "ValDebitoSaida_01")]
        public string ValDebitoSaida_01 { get { return HelperConversao.FormatarValorDecimal(_ValDebitoSaida_01); } set { _ValDebitoSaida_01 = value; } }

        private string _ValOutrosDebitos_02;
        //02 - Valor Total de "Outros Débitos"							
        [XmlElement(ElementName = "ValOutrosDebitos_02")]
        public string ValOutrosDebitos_02 { get { return HelperConversao.FormatarValorDecimal(_ValOutrosDebitos_02); } set { _ValOutrosDebitos_02 = value; } }

        private string _ValOutrosDebitos_03;
        //03 - Valor total de "Estornos de crédito"					
        [XmlElement(ElementName = "ValOutrosDebitos_03")]
        public string ValOutrosDebitos_03 { get { return HelperConversao.FormatarValorDecimal(_ValOutrosDebitos_03); } set { _ValOutrosDebitos_03 = value; } }

        private string _ValOutrosDebitos_04;
        //04 - Valor total dos "Débitos (01 + 02 + 03)"					
        [XmlElement(ElementName = "ValOutrosDebitos_04")]
        public string ValOutrosDebitos_04 { get { return HelperConversao.FormatarValorDecimal(_ValOutrosDebitos_04); } set { _ValOutrosDebitos_04 = value; } }

        private string _ValTotalCredEntradaAquisicoes_05;
        //05 - Valor total dos créditos por "Entradas e aquisições com crédito do imposto"					
        [XmlElement(ElementName = "ValTotalCredEntradaAquisicoes_05")]
        public string ValTotalCredEntradaAquisicoes_05 { get { return HelperConversao.FormatarValorDecimal(_ValTotalCredEntradaAquisicoes_05); } set { _ValTotalCredEntradaAquisicoes_05 = value; } }

        private string _ValTotalOutrosCreditos_06;
        //06 - Valor total de "Outros Créditos"					
        [XmlElement(ElementName = "ValTotalOutrosCreditos_06")]
        public string ValTotalOutrosCreditos_06 { get { return HelperConversao.FormatarValorDecimal(_ValTotalOutrosCreditos_06); } set { _ValTotalOutrosCreditos_06 = value; } }

        private string _ValTotalEstornosDebito_07;
        //07 - Valor total de "Estornos de débito"					
        [XmlElement(ElementName = "ValTotalEstornosDebito_07")]
        public string ValTotalEstornosDebito_07 { get { return HelperConversao.FormatarValorDecimal(_ValTotalEstornosDebito_07); } set { _ValTotalEstornosDebito_07 = value; } }

        private string _ValSubTotalCreditos_08;
        //08 - Valor subtotal dos "Créditos (05 + 06+ 07)"					
        [XmlElement(ElementName = "ValSubTotalCreditos_08")]
        public string ValSubTotalCreditos_08 { get { return HelperConversao.FormatarValorDecimal(_ValSubTotalCreditos_08); } set { _ValSubTotalCreditos_08 = value; } }

        private string _ValTotalSaldoCredorAnte_09;
        //09 - Valor total de "Saldo credor a transportar opara o periodo seguinte"					
        [XmlElement(ElementName = "ValTotalSaldoCredorAnte_09")]
        public string ValTotalSaldoCredorAnte_09 { get { return HelperConversao.FormatarValorDecimal(_ValTotalSaldoCredorAnte_09); } set { _ValTotalSaldoCredorAnte_09 = value; } }

        private string _ValTotalCreditos_10;
        //10 - Valor total dos "Créditos" (08 + 09)					
        [XmlElement(ElementName = "ValTotalCreditos_10")]
        public string ValTotalCreditos_10 { get { return HelperConversao.FormatarValorDecimal(_ValTotalCreditos_10); } set { _ValTotalCreditos_10 = value; } }

        private string _ValorSaldoCredorSeg_11;
        //11 - Valor total de "Saldo credor a transportar para o período seguinte (10 - 04)"					
        [XmlElement(ElementName = "ValorSaldoCredorSeg_11")]
        public string ValorSaldoCredorSeg_11 { get { return HelperConversao.FormatarValorDecimal(_ValorSaldoCredorSeg_11); } set { _ValorSaldoCredorSeg_11 = value; } }

        private string _ValorSaldoDevedor_12;
        //12 - Valor total de "Saldo devedor" (04 - 10)		
        [XmlElement(ElementName = "ValorSaldoDevedor_12")]
        public string ValorSaldoDevedor_12 { get { return HelperConversao.FormatarValorDecimal(_ValorSaldoDevedor_12); } set { _ValorSaldoDevedor_12 = value; } }

        private string _ValorDeducoes_13;
        //13 - Valor total de "Deduções"					
        [XmlElement(ElementName = "ValorDeducoes_13")]
        public string ValorDeducoes_13 { get { return HelperConversao.FormatarValorDecimal(_ValorDeducoes_13); } set { _ValorDeducoes_13 = value; } }

        private string _ValorICMSRecolher_14;
        //14 - Valor total de "ICMS a recolher"			
        [XmlElement(ElementName = "ValorICMSRecolher_14")]
        public string ValorICMSRecolher_14 { get { return HelperConversao.FormatarValorDecimal(_ValorICMSRecolher_14); } set { _ValorICMSRecolher_14 = value; } }

        private string _ICMSSubstiEntrada_15;
        //15 - Valor total de "ICMS substituto pelas entradas"					
        [XmlElement(ElementName = "ICMSSubstiEntrada_15")]
        public string ICMSSubstiEntrada_15 { get { return HelperConversao.FormatarValorDecimal(_ICMSSubstiEntrada_15); } set { _ICMSSubstiEntrada_15 = value; } }

        private string _ValorTotalICMSSaidasEstado_16;
        //16 - Valor total de "ICMS substituto pelas saídas para o Estado"	
        [XmlElement(ElementName = "ValorTotalICMSSaidasEstado_16")]
        public string ValorTotalICMSSaidasEstado_16 { get { return HelperConversao.FormatarValorDecimal(_ValorTotalICMSSaidasEstado_16); } set { _ValorTotalICMSSaidasEstado_16 = value; } }

        private string _ValorTotalDifAliquotasICMS_17;
        //17 - Valor total de "Diferença de alíquotas do ICMS" 		
        [XmlElement(ElementName = "ValorTotalDifAliquotasICMS_17")]
        public string ValorTotalDifAliquotasICMS_17 { get { return HelperConversao.FormatarValorDecimal(_ValorTotalDifAliquotasICMS_17); } set { _ValorTotalDifAliquotasICMS_17 = value; } }

        private string _ValorTotalICMSImportacao_18;
        //18 - Valor total de "ICMS da importação"					
        [XmlElement(ElementName = "ValorTotalICMSImportacao_18")]
        public string ValorTotalICMSImportacao_18 { get { return HelperConversao.FormatarValorDecimal(_ValorTotalICMSImportacao_18); } set { _ValorTotalICMSImportacao_18 = value; } }

        private string _ValorTotalOutrasObICMS_19;
        //19 - Valor total de "Outras obrigações do ICMS"	
        [XmlElement(ElementName = "ValorTotalOutrasObICMS_19")]
        public string ValorTotalOutrasObICMS_19 { get { return HelperConversao.FormatarValorDecimal(_ValorTotalOutrasObICMS_19); } set { _ValorTotalOutrasObICMS_19 = value; } }

        private string _ValorTotalICMSRecolher_20;
        //20 - Valor total das "Obrigações do ICMS a recolher"		
        [XmlElement(ElementName = "ValorTotalICMSRecolher_20")]
        public string ValorTotalICMSRecolher_20 { get { return HelperConversao.FormatarValorDecimal(_ValorTotalICMSRecolher_20); } set { _ValorTotalICMSRecolher_20 = value; } }

        private string _ValorTotalSubsTribSaiOutEstados_99;
        //99 - Valor total de "ICMS da substituição tributária pelas saídas para outros estados"					
        [XmlElement(ElementName = "ValorTotalSubsTribSaiOutEstados_99")]
        public string ValorTotalSubsTribSaiOutEstados_99 { get { return HelperConversao.FormatarValorDecimal(_ValorTotalSubsTribSaiOutEstados_99); } set { _ValorTotalSubsTribSaiOutEstados_99 = value; } }
    }

    [Serializable]
    [XmlRootAttribute("DadosEnvio")]
    public class TODadosEnvio
    {
        public TODadosEnvio() { }

        //Nome do arquivo/protocolo (Composição do arquivo);
        [XmlElement(ElementName = "NomeArquivo")]
        public string NomeArquivo { get; set; }

        //Identificação do certificado digital que enviou do arquivo
        [XmlElement(ElementName = "DadosCertificado")]
        public string DadosCertificado { get; set; }

        //Data e hora de emissão do recibo;
        [XmlElement(ElementName = "DtEmissaoRecibo")]
        public string DtEmissaoRecibo { get; set; }

        [XmlElement(ElementName = "DataHoraValidacao")]
        public string DataHoraValidacao { get; set; }

        [XmlElement(ElementName = "DiretorioLocal")]
        public string DiretorioLocal { get; set; }

    }
    
    [Serializable]
    [XmlRootAttribute("SaldoDoISSRecolher")]
    public class TOSaldoDoISSRecolher
    {
        private string _VlrTotRefPrestServPeriodo_A;
        //A- Valor total referente às prestações de serviço do período
        [XmlElement(ElementName = "VlrTotRefPrestServPeriodo_A")]
        public string VlrTotRefPrestServPeriodo_A { get { return HelperConversao.FormatarValorDecimal(_VlrTotRefPrestServPeriodo_A); } set { _VlrTotRefPrestServPeriodo_A = value; } }
    
        private string _VlrTotMatFornTercPrestServ_B;
        //B- Valor total do material fornecido por terceiros na prestação do serviço
        [XmlElement(ElementName = "VlrTotMatFornTercPrestServ_B")]
        public string VlrTotMatFornTercPrestServ_B { get { return HelperConversao.FormatarValorDecimal(_VlrTotMatFornTercPrestServ_B); } set { _VlrTotMatFornTercPrestServ_B = value; } }
        
        private string _VlrMatPropUtilPrestServ_C;
        //C- Valor do material próprio utilizado na prestação do serviço
        [XmlElement(ElementName = "VlrMatPropUtilPrestServ_C")]
        public string VlrMatPropUtilPrestServ_C { get { return HelperConversao.FormatarValorDecimal(_VlrMatPropUtilPrestServ_C); } set { _VlrMatPropUtilPrestServ_C = value; } }

        private string _VlrTotSubEmpreitadas_D;
        //D- Valor total das subempreitadas
        [XmlElement(ElementName = "VlrTotSubEmpreitadas_D")]
        public string VlrTotSubEmpreitadas_D { get { return HelperConversao.FormatarValorDecimal(_VlrTotSubEmpreitadas_D); } set { _VlrTotSubEmpreitadas_D = value; } }

        private string _VlrTotOperIsentasISSQN_E;
        //E- Valor total das operações isentas ou não-tributadas pelo ISSQN
        [XmlElement(ElementName = "VlrTotOperIsentasISSQN_E")]
        public string VlrTotOperIsentasISSQN_E { get { return HelperConversao.FormatarValorDecimal(_VlrTotOperIsentasISSQN_E); } set { _VlrTotOperIsentasISSQN_E = value; } }
        
        private string _VlrTotDedBaseCalculo_F;
        //F- Valor total das deduções da base de cálculo (B + C + D + E)
        [XmlElement(ElementName = "VlrTotDedBaseCalculo_F")]
        public string VlrTotDedBaseCalculo_F { get { return HelperConversao.FormatarValorDecimal(_VlrTotDedBaseCalculo_F); } set { _VlrTotDedBaseCalculo_F = value; } }

        private string _VlrTotBaseCalculoISSQN_G;
        //G- Valor total da base de cálculo do ISSQN (A - F)
        [XmlElement(ElementName = "VlrTotBaseCalculoISSQN_G")]
        public string VlrTotBaseCalculoISSQN_G { get { return HelperConversao.FormatarValorDecimal(_VlrTotBaseCalculoISSQN_G); } set { _VlrTotBaseCalculoISSQN_G = value; } }

        private string _VlrTotBaseCalculoRetencaoISSQN_H;
        //H- Valor total da base de cálculo de retenção do ISSQN
        [XmlElement(ElementName = "VlrTotBaseCalculoRetencaoISSQN_H")]
        public string VlrTotBaseCalculoRetencaoISSQN_H { get { return HelperConversao.FormatarValorDecimal(_VlrTotBaseCalculoRetencaoISSQN_H); } set { _VlrTotBaseCalculoRetencaoISSQN_H = value; } }

        private string _VlrTotISSQNDestacado_I;
        //I- Valor total do ISSQN destacado
        [XmlElement(ElementName = "VlrTotISSQNDestacado_I")]
        public string VlrTotISSQNDestacado_I { get { return HelperConversao.FormatarValorDecimal(_VlrTotISSQNDestacado_I); } set { _VlrTotISSQNDestacado_I = value; } }

        private string _VlrTotISSQNRetidoTomador_J;
        //J- Valor total do ISSQN retido pelo tomador
        [XmlElement(ElementName = "VlrTotISSQNRetidoTomador_J")]
        public string VlrTotISSQNRetidoTomador_J { get { return HelperConversao.FormatarValorDecimal(_VlrTotISSQNRetidoTomador_J); } set { _VlrTotISSQNRetidoTomador_J = value; } }

        private string _VlrTotDeducoesISSQN_K;
        //K- Valor total das deduções do ISSQN
        [XmlElement(ElementName = "VlrTotDeducoesISSQN_K")]
        public string VlrTotDeducoesISSQN_K { get { return HelperConversao.FormatarValorDecimal(_VlrTotDeducoesISSQN_K); } set { _VlrTotDeducoesISSQN_K = value; } }

        private string _VlrTotApuradoISSQNRecolher_L;
        //L- Valor total apurado do ISSQN a recolher (I - J - K)
        [XmlElement(ElementName = "VlrTotApuradoISSQNRecolher_L")]
        public string VlrTotApuradoISSQNRecolher_L { get { return HelperConversao.FormatarValorDecimal(_VlrTotApuradoISSQNRecolher_L); } set { _VlrTotApuradoISSQNRecolher_L = value; } }
    
        private string _VlrTotISSQNSubstitudoDevidoTomador_M;
        //M- Valor total do ISSQN substituto devido pelo tomador
        [XmlElement(ElementName = "VlrTotISSQNSubstitudoDevidoTomador_M")]
        public string VlrTotISSQNSubstitudoDevidoTomador_M { get { return HelperConversao.FormatarValorDecimal(_VlrTotISSQNSubstitudoDevidoTomador_M); } set { _VlrTotISSQNSubstitudoDevidoTomador_M = value; } }

        private string _VlrTotApuradoISSQNFiliais_N;
        //N- Valor total apurado do ISSQN das filiais
        [XmlElement(ElementName = "VlrTotApuradoISSQNFiliais_N")]
        public string VlrTotApuradoISSQNFiliais_N { get { return HelperConversao.FormatarValorDecimal(_VlrTotApuradoISSQNFiliais_N); } set { _VlrTotApuradoISSQNFiliais_N = value; } }

        private string _VlrTotApuradoISSQNRecolherOutrosMunicipios_O;
        //O- Valor total apurado do ISSQN a recolher para outros municípios
        [XmlElement(ElementName = "VlrTotApuradoISSQNRecolherOutrosMunicipios_O")]
        public string VlrTotApuradoISSQNRecolherOutrosMunicipios_O { get { return HelperConversao.FormatarValorDecimal(_VlrTotApuradoISSQNRecolherOutrosMunicipios_O); } set { _VlrTotApuradoISSQNRecolherOutrosMunicipios_O = value; } }
    }

    public class TOICMSPorCSOP
    {
        //Código Fiscal de Operação e Prestação
        [XmlElement(ElementName = "CFOP")]
        public string CFOP { get; set; }

        private string _ValorContabil;
        //Totalização do "Valor contábil" por CFOP
        [XmlElement(ElementName = "VL_CONT")]
        public string ValorContabil { get { return HelperConversao.FormatarValorDecimal(_ValorContabil); } set { _ValorContabil = value; } }

        private string _ValorBaseCalculo;
        //Totalização do "Valor da base de cálculo do ICMS" por CFOP
        [XmlElement(ElementName = "VL_BC_ICMS")]
        public string ValorBaseCalculo { get { return HelperConversao.FormatarValorDecimal(_ValorBaseCalculo); } set { _ValorBaseCalculo = value; } }

        private string _ValorICMS;
        //Totalização do "Valor do ICMS creditado/ debitado" por CFOP
        [XmlElement(ElementName = "VL_ICMS")]
        public string ValorICMS { get { return HelperConversao.FormatarValorDecimal(_ValorICMS); } set { _ValorICMS = value; } }

        private string _ValorSubstituicao;
        //Totalização do "Valor do ICMS da substituição tributária creditado/debitado" por CFOP
        [XmlElement(ElementName = "VL_ST")]
        public string ValorSubstituicao { get { return HelperConversao.FormatarValorDecimal(_ValorSubstituicao); } set { _ValorSubstituicao = value; } }

        private string _ValorComplementarCreditado;
        //Totalização do "Valor complementar do ICMS creditado" por CFOP
        [XmlElement(ElementName = "VL_COMPL")]
        public string ValorComplementarCreditado { get { return HelperConversao.FormatarValorDecimal(_ValorComplementarCreditado); } set { _ValorComplementarCreditado = value; } }

        private string _ValorOperacoesIsentas;
        //Totalização do "Valor das operações isentas ou não-tributadas pelo ICMS" por CFOP
        [XmlElement(ElementName = "VL_ISNT_ICMS")]
        public string ValorOperacoesIsentas { get { return HelperConversao.FormatarValorDecimal(_ValorOperacoesIsentas); } set { _ValorOperacoesIsentas = value; } }

        private string _ValorOutrasOperacoes;
        //Totalização do "Valor das outras operações do ICMS" por CFOP
        [XmlElement(ElementName = "VL_OUT_ICMS")]
        public string ValorOutrasOperacoes { get { return HelperConversao.FormatarValorDecimal(_ValorOutrasOperacoes); } set { _ValorOutrasOperacoes = value; } }
    }
}
