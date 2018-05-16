using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sefaz.CCR.Utilitarios;


namespace AssinaturaDigital.TO
{
    public class TORegistroB470 : TORegistro
    {
        //Débito
        public decimal VlIssqn { get; set; }
        public decimal VlIssqnST { get; set; }
        public decimal VlIssqnREC { get; set; }

        //Crédito
        public decimal VlIssqnRT { get; set; }
        public decimal VlDed { get; set; }

        //Retificacao
        public decimal VlBcIssqn { get; set; }
        public decimal VlBcIssqRt { get; set; }
        public decimal VlCont { get; set; }
        public decimal VlDedBc { get; set; }
        public decimal VlIsnt { get; set; }
        public decimal VlIssqnFil { get; set; }
        public decimal VlIssqnRtRec { get; set; }
        public decimal VlMatProp { get; set; }
        public decimal VlMatTerc { get; set; }
        public decimal VlSub { get; set; }

        public string DataRecepcao { get; set; }
        public string RazaoSocial { get; set; }
        public string Periodo { get; set; }
        public string CFDF { get; set; }

        public override void Ler(string line)
        {
            try
            {
                var campos = line.Split('|');
                VlIssqn = Convert.ToDecimal("0" + campos[10]);
                VlIssqnRT = Convert.ToDecimal("0" + campos[11]);
                VlDed = Convert.ToDecimal("0" + campos[12]);
                VlIssqnREC = Convert.ToDecimal("0" + campos[13]);
                VlIssqnST = Convert.ToDecimal("0" + campos[14]);
            }
            catch (Exception ex)
            {
                WriteToAnEventLog.gravar("Erro em TORegistroB470 / Ler: " + ex.Message);
                throw;
            }
        }

        public override string Chave()
        {
            return "B470";
        }
    }
}
