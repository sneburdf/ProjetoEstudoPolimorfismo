using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace AssinaturaDigital.Util
{
    public static class HelperConversao
    {
        /// <summary>
        /// Retorna o valor no formato 9.999,99 como string.
        /// </summary>
        /// <param name="ValorDecimal"></param>
        /// <param name="VazioRetornaZero">Quando vazio ou null - true retorna "0,00", false retorna ""</param>
        /// <returns></returns>
        public static string FormatarValorDecimal(object ValorDecimal, bool VazioRetornaZero)
        {
            if (ValorDecimal == null || string.IsNullOrEmpty(ValorDecimal.ToString()))
            {
                if (VazioRetornaZero)
                {
                    ValorDecimal = "0";
                }
                else
                {
                    return string.Empty;
                }
            }

            try
            {
                decimal valor = Convert.ToDecimal(ValorDecimal, new CultureInfo("pt-BR"));

                return valor.ToString("#,0.00#", new CultureInfo("pt-BR"));
            }
            catch
            {
                throw new InvalidCastException("Não é possível converter o valor \"" + ValorDecimal + "\" para decimal");
            }
        }

        /// <summary>
        /// Retorna o valor no formato 9.999,99 como string. Se o objeto estiver vazio ou nulo, retorna vazio.
        /// </summary>>
        public static string FormatarValorDecimal(object ValorDecimal)
        {
            return FormatarValorDecimal(ValorDecimal, false);
        }

    }
}
