using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using AssinaturaDigital.Util.Assinatura;
using AssinaturaDigital.Util.CertificadoDigital;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Runtime.InteropServices;
using System.IO;
using AssinaturaDigital.TO;
using FwkSefaz.FwkSeguranca;


using System.ComponentModel;

using System.Xml;

namespace AssinaturaDigital.Util
{
    public static class HelperCertificadoDigital
    {

        public static List<TOCertificadoDigital> BuscarListaCertificados()
        {
            List<TOCertificadoDigital> lista = new List<TOCertificadoDigital>();            

            IntPtr hCertStore = Crypt32.CertOpenSystemStore(IntPtr.Zero, "My");
            if (hCertStore == IntPtr.Zero)
                throw new Exception("CertOpenSystemStore failed: " + Marshal.GetLastWin32Error().ToString());

            IntPtr pCertContext = Crypt32.CertEnumCertificatesInStore(hCertStore, IntPtr.Zero);
                                    
            while (pCertContext != IntPtr.Zero)
            {
                //X509Certificate2 x509 = new X509Certificate2("c:", "dec2017",X509KeyStorageFlags.MachineKeySet);
                X509Certificate2 x509 = new X509Certificate2(pCertContext);
                //Verifica se o certificado foi emitido pelo ICP-Brasil
                if (x509.Issuer.Contains("O=ICP-Brasil") &&
                    x509.Issuer.Contains("C=BR"))
                {
                    if (ValidarCertificado(x509, DateTime.Now))
                        lista.Add(new TOCertificadoDigital(x509));

                }
                pCertContext = Crypt32.CertEnumCertificatesInStore(hCertStore, pCertContext);
            }

            return lista;

        }


        public static bool ValidarCertificado(X509Certificate2 X509, DateTime DataEHoraDoServidor)
        {
            if (X509 == null)
                return false;

            if (DataEHoraDoServidor > Convert.ToDateTime(X509.GetExpirationDateString()))
                return false;


            X509Chain x509Chain = new X509Chain();
            x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            x509Chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            x509Chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 30);
            x509Chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;

            x509Chain.Build(X509);

            if (x509Chain.ChainStatus.Length > 0 &&
                x509Chain.ChainStatus[0].Status == X509ChainStatusFlags.Revoked)
                return false;

            return X509.HasPrivateKey;
        }

        public static void AssinarArquivo(X509Certificate2 X509, string CaminhoArquivo, string CaminhoAssinatura)
        {
            //byte[] msgBytes = File.ReadAllBytes(CaminhoArquivo);

            //byte[] encodedSignedCms = SignMsg(msgBytes, X509);

            //File.WriteAllBytes(CaminhoAssinatura, encodedSignedCms);

            SignMsgOnStreamFile(X509,
                new FileStream(CaminhoArquivo, FileMode.Open),
                new FileStream(CaminhoAssinatura, FileMode.Create));
        }

        public static void AssinarArquivoComXml(X509Certificate2 X509, string nomeArquivo, string CaminhoArquivo, string CaminhoAssinatura)
        {
            Byte[] dados = System.IO.File.ReadAllBytes(CaminhoArquivo);
            BOCertificadoEmXML certificado = new BOCertificadoEmXML();

            gerarXml(dados, CaminhoArquivo, nomeArquivo, CaminhoAssinatura);
            certificado.AssinaXML(X509, CaminhoAssinatura, true, "ccr", "conteudo");

            Trace.Indent();
            Trace.TraceInformation("Caminho da Assinatura: {0} Certificado: ",CaminhoAssinatura);
            Trace.TraceInformation("Apelido {0}",X509.FriendlyName);
            Trace.TraceInformation("Emissor {0}", X509.IssuerName.Name);
            Trace.TraceInformation("SubjectName {0}", X509.SubjectName.Name);
            Trace.Unindent();
            Trace.Write("------------------------------");
            Trace.Flush();
        }
        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).Replace(" ","");
        }

        public static void gerarXml(Byte[] dados, string pathArquivo, string nomeArquivo, string CaminhoAssinatura)
        {
            string[] conteudo;
            XmlDocument docXml = new XmlDocument();
            try
            {



                XmlNode xmlNode = docXml.CreateXmlDeclaration("1.0", "ISO-8859-1", null);
                docXml.AppendChild(xmlNode);

                XmlNode noLivros = docXml.CreateElement("livros");

                XmlNode noLivro = docXml.CreateElement("livros", "livro", null);
                noLivros.AppendChild(noLivro);

                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                XmlNode noConteudo = docXml.CreateElement("livro", "conteudo", null);

                conteudo = File.ReadAllLines(pathArquivo, Encoding.GetEncoding("ISO-8859-1"));
                noConteudo.InnerText = String.Join("\n", conteudo);

                //foreach (var linha in conteudo)
                //{
                //    noConteudo.InnerText += linha;
                //}

                XmlAttribute attrId = docXml.CreateAttribute("Id");
                attrId.Value = RemoveDiacritics(nomeArquivo.Remove(nomeArquivo.Length - 4));
                noConteudo.Attributes.Append(attrId);
                noLivro.AppendChild(noConteudo);

                docXml.AppendChild(noLivros);

                Trace.Listeners.Add(new TextWriterTraceListener("TextWriterOutput.log", "myListener"));
                Trace.TraceInformation(DateTime.Now + docXml.InnerXml);
              

                Trace.Flush();

                docXml.Save(CaminhoAssinatura);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public static void DecodificarArquivoAssinado(string CaminhoArquivo, string CaminhoSaida)
        {
            DecodeSignedStreamFile(new FileStream(CaminhoArquivo, FileMode.Open),
                                   new FileStream(CaminhoSaida, FileMode.Create));
        }

        /// <summary>
        /// Retorna false quando é solicitada a senha do token, e o usuário cancela a operação
        /// </summary>
        public static bool FazerLoginToken(X509Certificate2 X509)
        {
            try
            {
                //(Assinando um array de byte qualquer, apenas para pedir a autenticação)
                SignMsg(new byte[] { 0 }, X509);

                return true;
            }
            catch (CryptographicException ex)
            {
                if (ex.Message.Contains("A operação foi cancelada pelo usuário."))
                    return false;
                else
                    throw ex;
            }


        }

        private static byte[] SignMsg(Byte[] msg, X509Certificate2 signerCert)
        {
            ContentInfo contentInfo = new ContentInfo(msg);

            // true = dessatachado e false = atachado
            SignedCms signedCms = new SignedCms(contentInfo, false);

            CmsSigner cmsSigner = new CmsSigner(signerCert);

            //  Sign the PKCS #7 message.
            //MessageBox.Show("Computing signature with signer subject name:... " + signerCert.SubjectName.Name);
            signedCms.ComputeSignature(cmsSigner, false);

            //  Encode the PKCS #7 message.
            return signedCms.Encode();
        }


        #region Codificar (assinar) e decodificar arquivos digitalmente, suportando arquivos grandes.

        // Encode CMS with streaming to support large data
        private static void SignMsgOnStreamFile(X509Certificate2 cert, FileStream inFile, FileStream outFile)
        {
            // Variables
            Win32.CMSG_SIGNER_ENCODE_INFO SignerInfo;
            Win32.CMSG_SIGNED_ENCODE_INFO SignedInfo;
            Win32.CMSG_STREAM_INFO StreamInfo;
            Win32.CERT_CONTEXT[] CertContexts = null;
            Win32.BLOB[] CertBlobs;

            X509Chain chain = null;
            X509ChainElement[] chainElements = null;
            X509Certificate2[] certs = null;
            RSACryptoServiceProvider key = null;
            BinaryReader stream = null;
            GCHandle gchandle = new GCHandle();

            IntPtr hProv = IntPtr.Zero;
            IntPtr SignerInfoPtr = IntPtr.Zero;
            IntPtr CertBlobsPtr = IntPtr.Zero;
            IntPtr hMsg = IntPtr.Zero;
            IntPtr pbPtr = IntPtr.Zero;
            Byte[] pbData;
            int dwFileSize;
            int dwRemaining;
            int dwSize;
            Boolean bResult = false;

            FileStream m_callbackFile = outFile;

            try
            {
                // Get data to encode
                dwFileSize = (int)inFile.Length;
                stream = new BinaryReader(inFile);
                pbData = stream.ReadBytes(dwFileSize);

                // Get cert chain
                chain = new X509Chain();
                chain.Build(cert);
                chainElements = new X509ChainElement[chain.ChainElements.Count];
                chain.ChainElements.CopyTo(chainElements, 0);

                // Get certs in chain
                certs = new X509Certificate2[chainElements.Length];
                for (int i = 0; i < chainElements.Length; i++)
                {
                    certs[i] = chainElements[i].Certificate;
                }

                // Get context of all certs in chain
                CertContexts = new Win32.CERT_CONTEXT[certs.Length];
                for (int i = 0; i < certs.Length; i++)
                {
                    CertContexts[i] = (Win32.CERT_CONTEXT)Marshal.PtrToStructure(certs[i].Handle, typeof(Win32.CERT_CONTEXT));
                }

                // Get cert blob of all certs
                CertBlobs = new Win32.BLOB[CertContexts.Length];
                for (int i = 0; i < CertContexts.Length; i++)
                {
                    CertBlobs[i].cbData = CertContexts[i].cbCertEncoded;
                    CertBlobs[i].pbData = CertContexts[i].pbCertEncoded;
                }

                // Get CSP of client certificate
                key = (RSACryptoServiceProvider)certs[0].PrivateKey;

                bResult = Win32.CryptAcquireContext(
                    ref hProv,
                    key.CspKeyContainerInfo.KeyContainerName,
                    key.CspKeyContainerInfo.ProviderName,
                    key.CspKeyContainerInfo.ProviderType,
                    0
                );
                if (!bResult)
                {
                    throw new Exception("CryptAcquireContext error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }

                // Populate Signer Info struct
                SignerInfo = new Win32.CMSG_SIGNER_ENCODE_INFO();
                SignerInfo.cbSize = Marshal.SizeOf(SignerInfo);
                SignerInfo.pCertInfo = CertContexts[0].pCertInfo;
                SignerInfo.hCryptProvOrhNCryptKey = hProv;
                SignerInfo.dwKeySpec = (int)key.CspKeyContainerInfo.KeyNumber;
                SignerInfo.HashAlgorithm.pszObjId = Win32.szOID_OIWSEC_sha1;

                // Populate Signed Info struct
                SignedInfo = new Win32.CMSG_SIGNED_ENCODE_INFO();
                SignedInfo.cbSize = Marshal.SizeOf(SignedInfo);

                SignedInfo.cSigners = 1;
                SignerInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(SignerInfo));
                Marshal.StructureToPtr(SignerInfo, SignerInfoPtr, false);
                SignedInfo.rgSigners = SignerInfoPtr;

                SignedInfo.cCertEncoded = CertBlobs.Length;
                CertBlobsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(CertBlobs[0]) * CertBlobs.Length);
                for (int i = 0; i < CertBlobs.Length; i++)
                {
                    Marshal.StructureToPtr(CertBlobs[i], new IntPtr(CertBlobsPtr.ToInt64() + (Marshal.SizeOf(CertBlobs[i]) * i)), false);
                }
                SignedInfo.rgCertEncoded = CertBlobsPtr;

                // Populate Stream Info struct
                StreamInfo = new Win32.CMSG_STREAM_INFO();
                StreamInfo.cbContent = dwFileSize;

                StreamInfo.pfnStreamOutput = new Win32.StreamOutputCallbackDelegate(
                    (IntPtr pvArg, IntPtr pbDataCallback, int cbData, Boolean fFinal) =>
                    {
                        // Write all bytes to encoded file
                        Byte[] bytes = new Byte[cbData];
                        Marshal.Copy(pbDataCallback, bytes, 0, cbData);
                        m_callbackFile.Write(bytes, 0, cbData);

                        if (fFinal)
                        {
                            // This is the last piece. Close the file
                            m_callbackFile.Flush();
                            m_callbackFile.Close();
                            m_callbackFile = null;
                        }

                        return true;
                    });

                // TODO: CMSG_DETACHED_FLAG

                // Open message to encode
                hMsg = Win32.CryptMsgOpenToEncode(
                    Win32.X509_ASN_ENCODING | Win32.PKCS_7_ASN_ENCODING,
                    0,
                    Win32.CMSG_SIGNED,
                    ref SignedInfo,
                    null,
                    ref StreamInfo
                );
                if (hMsg.Equals(IntPtr.Zero))
                {
                    throw new Exception("CryptMsgOpenToEncode error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }

                // Process the whole message
                gchandle = GCHandle.Alloc(pbData, GCHandleType.Pinned);
                pbPtr = gchandle.AddrOfPinnedObject();
                dwRemaining = dwFileSize;
                dwSize = (dwFileSize < 1024 * 1000 * 100) ? dwFileSize : 1024 * 1000 * 100;
                while (dwRemaining > 0)
                {
                    // Update message piece by piece     
                    bResult = Win32.CryptMsgUpdate(
                        hMsg,
                        pbPtr,
                        dwSize,
                        (dwRemaining <= dwSize) ? true : false
                    );
                    if (!bResult)
                    {
                        throw new Exception("CryptMsgUpdate error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                    }

                    // Move to the next piece
                    pbPtr = new IntPtr(pbPtr.ToInt64() + dwSize);
                    dwRemaining -= dwSize;
                    if (dwRemaining < dwSize)
                    {
                        dwSize = dwRemaining;
                    }
                }
            }
            finally
            {
                // Clean up
                if (gchandle.IsAllocated)
                {
                    gchandle.Free();
                }
                if (stream != null)
                {
                    stream.Close();
                }
                if (m_callbackFile != null)
                {
                    m_callbackFile.Close();
                }
                if (!CertBlobsPtr.Equals(IntPtr.Zero))
                {
                    Marshal.FreeHGlobal(CertBlobsPtr);
                }

                if (!SignerInfoPtr.Equals(IntPtr.Zero))
                {
                    Marshal.FreeHGlobal(SignerInfoPtr);
                }
                if (!hProv.Equals(IntPtr.Zero))
                {
                    Win32.CryptReleaseContext(hProv, 0);
                }
                if (!hMsg.Equals(IntPtr.Zero))
                {
                    Win32.CryptMsgClose(hMsg);
                }
            }
        }

        // Decode CMS with streaming to support large data
        private static void DecodeSignedStreamFile(FileStream inFile, FileStream outFile)
        {
            // Variables
            Win32.CMSG_STREAM_INFO StreamInfo;
            Win32.CERT_CONTEXT SignerCertContext;

            BinaryReader stream = null;
            GCHandle gchandle = new GCHandle();

            IntPtr hMsg = IntPtr.Zero;
            IntPtr pSignerCertInfo = IntPtr.Zero;
            IntPtr pSignerCertContext = IntPtr.Zero;
            IntPtr pbPtr = IntPtr.Zero;
            IntPtr hStore = IntPtr.Zero;
            Byte[] pbData;
            Boolean bResult = false;
            int dwFileSize;
            int dwRemaining;
            int dwSize;
            int cbSignerCertInfo;
            FileStream m_callbackFile = outFile;

            try
            {
                // Get data to decode
                dwFileSize = (int)inFile.Length;
                stream = new BinaryReader(inFile);
                pbData = stream.ReadBytes(dwFileSize);

                // Populate Stream Info struct
                StreamInfo = new Win32.CMSG_STREAM_INFO();
                StreamInfo.cbContent = dwFileSize;
                StreamInfo.pfnStreamOutput = new Win32.StreamOutputCallbackDelegate(
                    (IntPtr pvArg, IntPtr pbDataCallBack, int cbData, Boolean fFinal) =>
                    {
                        // Write all bytes to encoded file
                        Byte[] bytes = new Byte[cbData];
                        Marshal.Copy(pbDataCallBack, bytes, 0, cbData);
                        m_callbackFile.Write(bytes, 0, cbData);

                        if (fFinal)
                        {
                            // This is the last piece. Close the file
                            m_callbackFile.Flush();
                            m_callbackFile.Close();
                            m_callbackFile = null;
                        }

                        return true;
                    });

                // Open message to decode
                hMsg = Win32.CryptMsgOpenToDecode(
                    Win32.X509_ASN_ENCODING | Win32.PKCS_7_ASN_ENCODING,
                    0,
                    0,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    ref StreamInfo
                );
                if (hMsg.Equals(IntPtr.Zero))
                {
                    throw new Exception("CryptMsgOpenToDecode error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }

                // Process the whole message
                gchandle = GCHandle.Alloc(pbData, GCHandleType.Pinned);
                pbPtr = gchandle.AddrOfPinnedObject();
                dwRemaining = dwFileSize;
                dwSize = (dwFileSize < 1024 * 1000 * 100) ? dwFileSize : 1024 * 1000 * 100;
                while (dwRemaining > 0)
                {
                    // Update message piece by piece     
                    bResult = Win32.CryptMsgUpdate(
                        hMsg,
                        pbPtr,
                        dwSize,
                        (dwRemaining <= dwSize) ? true : false
                    );
                    if (!bResult)
                    {
                        throw new Exception("CryptMsgUpdate error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                    }

                    // Move to the next piece
                    pbPtr = new IntPtr(pbPtr.ToInt64() + dwSize);
                    dwRemaining -= dwSize;
                    if (dwRemaining < dwSize)
                    {
                        dwSize = dwRemaining;
                    }
                }

                // Get signer certificate info
                cbSignerCertInfo = 0;
                bResult = Win32.CryptMsgGetParam(
                    hMsg,
                    Win32.CMSG_SIGNER_CERT_INFO_PARAM,
                    0,
                    IntPtr.Zero,
                    ref cbSignerCertInfo
                );
                if (!bResult)
                {
                    throw new Exception("CryptMsgGetParam error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }

                pSignerCertInfo = Marshal.AllocHGlobal(cbSignerCertInfo);

                bResult = Win32.CryptMsgGetParam(
                    hMsg,
                    Win32.CMSG_SIGNER_CERT_INFO_PARAM,
                    0,
                    pSignerCertInfo,
                    ref cbSignerCertInfo
                );
                if (!bResult)
                {
                    throw new Exception("CryptMsgGetParam error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }

                // Open a cert store in memory with the certs from the message
                hStore = Win32.CertOpenStore(
                    Win32.CERT_STORE_PROV_MSG,
                    Win32.X509_ASN_ENCODING | Win32.PKCS_7_ASN_ENCODING,
                    IntPtr.Zero,
                    0,
                    hMsg
                );
                if (hStore.Equals(IntPtr.Zero))
                {
                    throw new Exception("CertOpenStore error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }

                // Find the signer's cert in the store
                pSignerCertContext = Win32.CertGetSubjectCertificateFromStore(
                    hStore,
                    Win32.X509_ASN_ENCODING | Win32.PKCS_7_ASN_ENCODING,
                    pSignerCertInfo
                );
                if (pSignerCertContext.Equals(IntPtr.Zero))
                {
                    throw new Exception("CertGetSubjectCertificateFromStore error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }

                // Set message for verifying
                SignerCertContext = (Win32.CERT_CONTEXT)Marshal.PtrToStructure(pSignerCertContext, typeof(Win32.CERT_CONTEXT));
                bResult = Win32.CryptMsgControl(
                    hMsg,
                    0,
                    Win32.CMSG_CTRL_VERIFY_SIGNATURE,
                    SignerCertContext.pCertInfo
                );
                if (!bResult)
                {
                    throw new Exception("CryptMsgControl error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }
            }
            finally
            {
                // Clean up
                if (gchandle.IsAllocated)
                {
                    gchandle.Free();
                }
                if (!pSignerCertContext.Equals(IntPtr.Zero))
                {
                    Win32.CertFreeCertificateContext(pSignerCertContext);
                }
                if (!pSignerCertInfo.Equals(IntPtr.Zero))
                {
                    Marshal.FreeHGlobal(pSignerCertInfo);
                }
                if (!hStore.Equals(IntPtr.Zero))
                {
                    Win32.CertCloseStore(hStore, Win32.CERT_CLOSE_STORE_FORCE_FLAG);
                }
                if (stream != null)
                {
                    stream.Close();
                }
                if (m_callbackFile != null)
                {
                    m_callbackFile.Close();
                }
                if (!hMsg.Equals(IntPtr.Zero))
                {
                    Win32.CryptMsgClose(hMsg);
                }
            }
        }


        #endregion
    }
}
