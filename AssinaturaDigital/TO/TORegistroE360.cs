using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sefaz.CCR.Utilitarios;

namespace AssinaturaDigital.TO
{
    public class TORegistroE360 : TORegistro
    {
        //Débito
        public decimal Vl01 { get; set; }
        public decimal Vl02 { get; set; }
        public decimal Vl03 { get; set; }
        public decimal Vl04 { get; set; }
        public decimal Vl15 { get; set; }
        public decimal Vl16 { get; set; }
        public decimal Vl18 { get; set; }

        //Crédito
        public decimal Vl05 { get; set; }
        public decimal Vl06 { get; set; }
        public decimal Vl07 { get; set; }
        public decimal Vl08 { get; set; }
        public decimal Vl09 { get; set; }
        public decimal Vl13 { get; set; }

        //Retificao
        public decimal Vl10 { get; set; }
        public decimal Vl11 { get; set; }
        public decimal Vl12 { get; set; }
        public decimal Vl14 { get; set; }
        public decimal Vl19 { get; set; }
        public decimal Vl17 { get; set; }
        public decimal Vl20 { get; set; }
        public decimal Vl99 { get; set; }


        public string DataRecepcao { get; set; }
        public string RazaoSocial { get; set; }
        public string Periodo { get; set; }
        public string CFDF { get; set; }

        public override void Ler(string line)
        {
            try
            {
                var campos = line.Split('|');
                Vl01 = Convert.ToDecimal("0" + campos[02], new CultureInfo("pt-BR"));
                Vl02 = Convert.ToDecimal("0" + campos[03], new CultureInfo("pt-BR"));
                Vl03 = Convert.ToDecimal("0" + campos[04], new CultureInfo("pt-BR"));
                Vl04 = Convert.ToDecimal("0" + campos[05], new CultureInfo("pt-BR"));
                Vl05 = Convert.ToDecimal("0" + campos[06], new CultureInfo("pt-BR"));
                Vl06 = Convert.ToDecimal("0" + campos[07], new CultureInfo("pt-BR"));
                Vl07 = Convert.ToDecimal("0" + campos[08], new CultureInfo("pt-BR"));
                Vl08 = Convert.ToDecimal("0" + campos[09], new CultureInfo("pt-BR"));
                Vl09 = Convert.ToDecimal("0" + campos[10], new CultureInfo("pt-BR"));
                Vl13 = Convert.ToDecimal("0" + campos[14], new CultureInfo("pt-BR"));
                Vl15 = Convert.ToDecimal("0" + campos[16], new CultureInfo("pt-BR"));
                Vl16 = Convert.ToDecimal("0" + campos[17], new CultureInfo("pt-BR"));
                Vl17 = Convert.ToDecimal("0" + campos[18], new CultureInfo("pt-BR"));
                Vl18 = Convert.ToDecimal("0" + campos[19], new CultureInfo("pt-BR"));

            }
            catch (Exception ex)
            {
                WriteToAnEventLog.gravar("Erro em TORegistroE360 / Ler: " + ex.Message);
                throw;
            }
        }

        public override string Chave()
        {
            return "E360";
        }
    }
}
