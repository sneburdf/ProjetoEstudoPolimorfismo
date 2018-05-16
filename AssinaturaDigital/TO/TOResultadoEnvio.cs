using System;
using System.Collections.Generic;
using System.Text;

namespace AssinaturaDigital.TO
{
    public class TOResultadoEnvio
    {
        public Arquivo Arquivo { get; private set; }
        public bool Sucesso { get; private set; }
        public bool NaoValidado { get; private set; }
        public string DiretorioLocal { get; private set; }
        public Exception Exception { get; private set; }
        public DateTime DataHoraConclusao { get; private set; }

        /// <summary>
        /// Cria um novo objeto que representa SUCESSO no envio
        /// </summary>
        public TOResultadoEnvio(Arquivo Arquivo)
        {

            this.DiretorioLocal = Arquivo.CaminhoENome;
            
            this.Arquivo = Arquivo;

            this.Sucesso = true;

            this.DataHoraConclusao = DateTime.Now;
        }

        /// <summary>
        /// Cria um novo objeto que representa ERRO no envio
        /// </summary>
        public TOResultadoEnvio(Arquivo Arquivo, Exception Exception)
        {
            this.DiretorioLocal = Arquivo.CaminhoENome;

            this.Arquivo = Arquivo;

            this.Exception = Exception;
            this.Sucesso = false;

            this.DataHoraConclusao = DateTime.Now;

        }

        /////// <summary>
        /////// Cria um novo objeto que representa arquivo não validado
        /////// </summary>
        //public TOResultadoEnvio(Arquivo Arquivo)
        //{
        //    this.Arquivo = Arquivo;

        //    this.Sucesso = false;

        //    this.NaoValidado = true;

        //    this.DataHoraConclusao = DateTime.Now;
    }
}
