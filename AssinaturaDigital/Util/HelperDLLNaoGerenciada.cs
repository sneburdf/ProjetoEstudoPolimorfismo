using System;
using System.Text;
using System.Runtime.InteropServices;
using AssinaturaDigital.Enums;

namespace AssinaturaDigital.Util
{
    public static class HelperDLLNaoGerenciada
    {
        #region Membros privados

        private const string SENHA_CRIPTOGRAFIA = "LivEle 587964zy PDigital";

        private static IntPtr _enderecoDLL;

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private extern static IntPtr LoadLibrary(string librayName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr hModule, String procname);

        /// <summary>
        /// </summary>
        /// <typeparam name="T">Delegate que representa a função a ser chamada</typeparam>
        /// <param name="pointerDLL"></param>
        /// <returns></returns>
        private static T GerarInstanciaDeDelegate<T>(IntPtr pointerDLL)
        {
            string nomeFunction = typeof(T).Name;
            IntPtr procAddr = GetProcAddress(pointerDLL, nomeFunction);
            return (T)(object)Marshal.GetDelegateForFunctionPointer(procAddr, typeof(T));
        }

        #endregion

        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate int Criptografa([MarshalAs(UnmanagedType.LPWStr)] StringBuilder arquivoIn, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder arquivoOut, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder senha);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int Descriptografa([MarshalAs(UnmanagedType.LPWStr)] StringBuilder arquivoIn, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder arquivoOut, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder senha);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate int ValidarArquivo([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pXML, ref StringBuilder pDadosValidacao);

        #endregion

        #region Chamadas Públicas

        /// <summary>
        /// Carregar a DLL em memória. Isto só pode ser feito na thread principal.
        /// </summary>
        public static void CarregarDLLValidador()
        {
            try
            {
                _enderecoDLL = LoadLibrary(IntPtr.Size == 8 ? @HelperEstrutura.PathDLLValidador64 : @HelperEstrutura.PathDLLValidador);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar a DLL", ex);
            }
        }

        public static void LiberarDLLValidador()
        {
            if ((int) _enderecoDLL == 0) return;

            while (_enderecoDLL != IntPtr.Zero)
            {
                FreeLibrary(_enderecoDLL);
                _enderecoDLL = IntPtr.Zero;
            }

            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public static void Criptografar(string arquivoEntrada, string arquivoSaida)
        {
            //try
            //{
            if ((int)_enderecoDLL == 0) CarregarDLLValidador();

            var instance = GerarInstanciaDeDelegate<Criptografa>(_enderecoDLL);

            instance(new StringBuilder(arquivoEntrada), new StringBuilder(arquivoSaida), new StringBuilder(SENHA_CRIPTOGRAFIA));
            //}
            //finally
            //{
            //    //FreeLibrary(_enderecoDLL);
            //}
        }

        public static void Descriptografar(string arquivoEntrada, string arquivoSaida)
        {
            //try
            //{
            if ((int)_enderecoDLL == 0) CarregarDLLValidador();

            var instance = GerarInstanciaDeDelegate<Descriptografa>(_enderecoDLL);

            instance(new StringBuilder(arquivoEntrada), new StringBuilder(arquivoSaida), new StringBuilder(SENHA_CRIPTOGRAFIA));
            //}
            //finally
            //{
            //    //FreeLibrary(_enderecoDLL);
            //}
        }

        public static ResultadoValidacao RealizarValidacao(string xmlValidar, out string caminhoArquivoResultado)
        {
            try
            {
                if ((int)_enderecoDLL == 0) CarregarDLLValidador();

                var instance = GerarInstanciaDeDelegate<ValidarArquivo>(_enderecoDLL);

                //Setando um "buffer" para reservar memória suficiente para o retorno
                var tamanhoBuffer = HelperEstrutura.DirLog.Length + 100;
                var resultadoBuilder = new StringBuilder(tamanhoBuffer);
                
                var resultado = instance(new StringBuilder(xmlValidar), ref resultadoBuilder);

                caminhoArquivoResultado = resultadoBuilder.ToString();

                return (ResultadoValidacao)resultado;
            }
            finally
            {
                LiberarDLLValidador();
            }
        }

        #endregion
    }
}
