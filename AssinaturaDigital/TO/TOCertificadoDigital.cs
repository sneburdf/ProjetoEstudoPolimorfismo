using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;



namespace AssinaturaDigital.TO
{
    public class TOCertificadoDigital
    {
        #region Atributos

        private String cpf = null;

        public String cnpj = null;

        private String fAltNamesOctet = null;

        private static X509Certificate2Collection fcollection = null;

        #endregion


        public TOCertificadoDigital(X509Certificate2 CertificadoX509)
        {
            this.PreencherDadosAssinaturaDigital(CertificadoX509);
        }

        #region Propriedades

        public string Descricao { get; set; }
        public string Emissor { get; set; }
        public string dataCriacao { get; set; }
        public string dataExpiracao { get; set; }
        public uint Handler { get; set; }
        public string NomePessoaFisica { get; set; }
        public DateTime DataNascimento { get; set; }
        public string CPF { get; set; }
        public string IdentificacaoSocial { get; set; }
        public string RG { get; set; }
        public string RGOrgaoExpeditor { get; set; }
        public string TituloEleitor { get; set; }
        public int TituloEleitorZona { get; set; }
        public int TituloEleitorSecao { get; set; }
        public string TituloEleitorMunicipio { get; set; }
        public string INSS { get; set; }
        public X509Certificate2 X509 { get; set; }

        #endregion

        #region Métodos Privados

        private void PreencherDadosAssinaturaDigital(X509Certificate2 x509)
        {
            string[] tokens = x509.SubjectName.Name.Split(',');
            string[] tokensNomeCPF = tokens[0].Split(':');

            this.X509 = x509;
            this.Descricao = x509.Subject;
            this.Emissor = x509.Issuer;
            this.dataCriacao = x509.GetEffectiveDateString();
            this.dataExpiracao = x509.GetExpirationDateString();
            //certEntidade.Handler = Crypt32.CertDuplicateCertificateContext(pCertContext);
            this.NomePessoaFisica = tokensNomeCPF[0].Replace("CN=", "");
            //entity.DataNascimento = ECPFUtil.DataNascimento;

            if(isECnpj(x509))
                this.cnpj = getCnpj();
            else
                this.CPF = getCpf();
                 

            //ObterExtensions(x509, this);
            // this.IdentificacaoSocial = ECPFUtil.IdentificacaoSocial;
            // this.RG = ECPFUtil.RG;
            // this.RGOrgaoExpeditor = ECPFUtil.RGOrgaoExpeditor;
            //  this.TituloEleitor = ECPFUtil.TituloEleitor;
            //  this.TituloEleitorZona = ECPFUtil.TituloEleitorZona;
            //  this.TituloEleitorSecao = ECPFUtil.TituloEleitorSecao;
            //  this.TituloEleitorMunicipio = ECPFUtil.TituloEleitorMunicipio;
            //  this.INSS = ECPFUtil.INSS;

        }

        private static void ObterExtensions(X509Certificate2 x509, TOCertificadoDigital certEntidade)
        {
            X509Extension extension = x509.Extensions["2.5.29.17"];
            if (extension != null)
            {
                string otherName = extension.Format(false);
                if (!string.IsNullOrEmpty(otherName) &&
                    (otherName.Contains("2.16.76.1.3.1") ||
                    otherName.Contains("2.16.76.1.3.4")))
                {
                    int offset;
                    if (otherName.Contains("2.16.76.1.3.1"))
                    {
                        offset = otherName.IndexOf("2.16.76.1.3.1=");
                    }
                    else
                    {
                        offset = otherName.IndexOf("2.16.76.1.3.4=");
                    }
                    offset += 14;
                    int endPos = otherName.IndexOf(",", offset);
                    string cpfEncValue;
                    if (endPos == -1)
                    {
                        cpfEncValue = otherName.Substring(offset);
                    }
                    else
                    {
                        cpfEncValue = otherName.Substring(offset, endPos - offset);
                    }

                    List<byte> byteArrayCpf = new List<byte>();
                    string[] tokens = cpfEncValue.Split(' ');
                    for (int i = 10; i < 21; i++)
                    {
                        byte value = byte.Parse(tokens[i], System.Globalization.NumberStyles.HexNumber);
                        if ((value >= 48) &&
                            (value <= 57))
                        {
                            byteArrayCpf.Add(value);
                        }
                    }
                    certEntidade.CPF = ASCIIEncoding.ASCII.GetString(byteArrayCpf.ToArray()).PadLeft(14, 'X');

                    certEntidade.DataNascimento = ExtrairDataNascimento(tokens);
                    certEntidade.IdentificacaoSocial = ExtrairIdentificacaoSocial(tokens);
                    certEntidade.RG = ExtrairRG(tokens);
                }
            }

        }

        private static DateTime ExtrairDataNascimento(string[] tokens)
        {
            List<byte> byteNiverCpf = new List<byte>();
            for (int i = 2; i < 10; i++)
            {
                byte value = byte.Parse(tokens[i], System.Globalization.NumberStyles.HexNumber);
                {
                    byteNiverCpf.Add(value);
                }
            }
            string data = ASCIIEncoding.ASCII.GetString(byteNiverCpf.ToArray());

            return new DateTime(int.Parse(data.Substring(4, 4)),
                                int.Parse(data.Substring(2, 2)),
                                int.Parse(data.Substring(0, 2)));

        }

        private static string ExtrairIdentificacaoSocial(string[] tokens)
        {
            List<byte> bytedentificacaoSocial = new List<byte>();
            for (int i = 22; i < 32; i++)
            {
                byte value = byte.Parse(tokens[i], System.Globalization.NumberStyles.HexNumber);

                bytedentificacaoSocial.Add(value);
            }
            return ASCIIEncoding.ASCII.GetString(bytedentificacaoSocial.ToArray());

        }

        private static string ExtrairRG(string[] tokens)
        {
            List<byte> byteRG = new List<byte>();
            for (int i = 33; i < 38; i++)
            {
                byte value = byte.Parse(tokens[i], System.Globalization.NumberStyles.HexNumber);
                byteRG.Add(value);
            }
            return ASCIIEncoding.ASCII.GetString(byteRG.ToArray());

        }

        public Boolean isECnpj(X509Certificate2 X509Cert)
        {
            if (alternativeName(X509Cert))
            {
                cnpj = getOID("2.16.76.1.3.3").Trim();
                if ((cnpj.Length <= 0) || (cnpj == null))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public Boolean alternativeName(X509Certificate2 X509Cert)
        {
            fAltNamesOctet = "";
            for (int k = 0; k < X509Cert.Extensions.Count; k++)
            {
                String x = (X509Cert.Extensions[k].Format(true));
                if (X509Cert.Extensions[k].Oid.Value.Equals("2.5.29.17"))
                {
                    fAltNamesOctet = X509Cert.Extensions[k].Format(true);
                    break;
                }
            }
            if (fAltNamesOctet != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // extraindo os OID do certificado

        public String getOID(String oid)
        {
            if (fAltNamesOctet.Contains(oid))
            {
                int idx = fAltNamesOctet.IndexOf(oid) + oid.Length + 1;
                
                String sAltNamesStr = "";

                for (int y = idx; y < fAltNamesOctet.Length; y++)
                {
                    try
                    {
                        sAltNamesStr += byte.Parse(fAltNamesOctet.Substring(idx, 2), System.Globalization.NumberStyles.HexNumber).ToString() + " ";
                        idx = idx + 3;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }

                String[] sAltNamesArr = sAltNamesStr.Trim().Split(' ');
                String sAltNames = "";

                for (int j = 2; j < sAltNamesArr.Length; j++)
                {
                    sAltNames += (char)Convert.ToInt32(sAltNamesArr[j]);
                }
                return sAltNames;
            }
            else
            {
                return "";
            }
        }

        #region MetodosGetSet

        //retorno CPF do certificado
        public String getCpf()
        {
            //finalizar o metodo para q nao haja cpf  carregar a variavel fAltNamesOctet com os oids
            if (cpf == null)
            {
                string oid = getOID("2.16.76.1.3.1");
                if (String.IsNullOrEmpty(oid) || oid.Length < 18)
                    return null;
                cpf = oid.Substring(8, 11);
            }
            return cpf;
        }

        //retorno CNPJ do certificado
        public String getCnpj()
        {
            if (cnpj == null)
            {//cnpj tamanho ... verificar
                cnpj = getOID("2.16.76.1.3.3");
            }
            return cnpj;
        }
        #endregion

        #endregion


    }
}
