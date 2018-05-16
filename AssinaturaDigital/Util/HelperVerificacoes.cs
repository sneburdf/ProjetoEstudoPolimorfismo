using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;
using System.Diagnostics;

namespace AssinaturaDigital.Util
{
    public class HelperVerificacoes
    {
        public static bool PossuiPermissaoEscrita(string CaminhoArquivoOuDiretorio)
        {
            try
            {
                FileSystemSecurity security;
                if (Directory.Exists(CaminhoArquivoOuDiretorio))
                {
                    security = Directory.GetAccessControl(CaminhoArquivoOuDiretorio);
                }
                else
                {
                    security = Directory.GetAccessControl(Path.GetDirectoryName(CaminhoArquivoOuDiretorio));
                }
                var rules = security.GetAccessRules(true, true, typeof(NTAccount));

                var currentuser = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                bool result = false;
                foreach (FileSystemAccessRule rule in rules)
                {
                    if (0 == (rule.FileSystemRights &
                        (FileSystemRights.WriteData | FileSystemRights.Write)))
                    {
                        continue;
                    }

                    if (rule.IdentityReference.Value.StartsWith("S-1-"))
                    {
                        var sid = new SecurityIdentifier(rule.IdentityReference.Value);
                        if (!currentuser.IsInRole(sid))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!currentuser.IsInRole(rule.IdentityReference.Value))
                        {
                            continue;
                        }
                    }

                    if (rule.AccessControlType == AccessControlType.Deny)
                        return false;
                    if (rule.AccessControlType == AccessControlType.Allow)
                        result = true;
                }
                return result;
            }
            catch
            {
                return false;
            }
        }

        public static void ReiniciarAplicacaoComoAdmin()
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    Verb = "runas",
                    FileName = Application.ExecutablePath
                });
            }
            catch (System.ComponentModel.Win32Exception)
            {
                //Operação cancelada pelo usuário. apenas fechar.
            }

            Environment.Exit(0);
        }

        public static void ReiniciarAplicacao()
        {
            Process.Start(Application.ExecutablePath);
            Environment.Exit(0);
        }
    }
}
