using System.Windows.Forms;

namespace AssinaturaDigital
{
    public static class FormsExtensions
    {
        public static void Centralizar(this Control Control)
        {
            Control.CentralizarHorizontal();
            Control.CentralizarVertical();
        }

        public static void CentralizarVertical(this Control Control)
        {
            Control.Top = (Control.Parent.Height - Control.Height)/2;
        }

        public static void CentralizarHorizontal(this Control Control)
        {
            Control.Left = (Control.Parent.Width - Control.Width)/2;
            //Comentario teste
        }
    }
}

// Necessário para criar extension method no .Net 2.0

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
                    | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute
    {
    }
}