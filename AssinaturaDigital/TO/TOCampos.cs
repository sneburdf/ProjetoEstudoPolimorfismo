using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssinaturaDigital.Enums;
using Sefaz.CCR.Utilitarios;


namespace AssinaturaDigital.TO
{
    public class TOCampos
    {
        public TORegistroE360 RegistroE360 { get; set; }

        public TORegistroB470 RegistroB470 { get; set; }

        public TORegistro Registro(string chave)
        {
            var registros = new Dictionary<string, TORegistro>
            {
                {RegistroB470.Chave(), RegistroB470},
                {RegistroE360.Chave(), RegistroE360}
            };

            if (registros.ContainsKey(chave)) return registros[chave];
            return null;
        }

        public TOCampos CarregarRegistros(string dirArquivo)
        {
            var toRegistros = new TOCampos();
            toRegistros.RegistroB470 = new TORegistroB470();
            toRegistros.RegistroE360 = new TORegistroE360();
            TOCampos result;
            try
            {
                var arquivo = new StreamReader(dirArquivo, Encoding.GetEncoding("ISO-8859-1"));
                for (var line = arquivo.ReadLine(); line != null; line = arquivo.ReadLine())
                {
                    var chave = line.Split('|')[1];
                    var registro = toRegistros.Registro(chave);
                    if (registro != null)
                    {
                        registro.Ler(line);
                    }
                }
                result = toRegistros;
            }
            catch (Exception ex)
            {
                WriteToAnEventLog.gravar("Erro");
                throw;
            }
            return result;
        }
    }
}