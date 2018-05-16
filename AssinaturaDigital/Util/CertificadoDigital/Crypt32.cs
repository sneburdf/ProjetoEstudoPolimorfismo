using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace AssinaturaDigital.Util.CertificadoDigital
{
    public class Crypt32
    {
        [DllImport("Crypt32.dll", CharSet = CharSet.Auto)]
        public extern static IntPtr CertOpenSystemStore(IntPtr hprov, string szSubsystemProtocol);
        
        [DllImport("Crypt32.dll", CharSet = CharSet.Auto)]
        public extern static IntPtr CertEnumCertificatesInStore(IntPtr hCertStore, IntPtr pPrevCertContext);
        
        [DllImport("Crypt32.dll", CharSet = CharSet.Auto)]
        public extern static uint CertDuplicateCertificateContext(uint pPrevCertContext);
        
        [DllImport("Crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool CertGetCertificateContextProperty(int pCertContext, int dwPropId,
           IntPtr pvData, ref uint pcbData);
        
        [DllImport("Crypt32.dll", CharSet = CharSet.Auto)]
        public extern static uint CertCreateCertificateContext(uint dwCertEncodingType,
           [MarshalAs(UnmanagedType.LPArray)]byte[] pbCertEncoded, int cbCertEncoded);
        
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool CryptAcquireContext(ref uint phProv, string pszContainer,
           string pszProvider, uint dwProvType, uint dwFlags);
        
        [DllImport("Crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool CryptImportPublicKeyInfoEx(uint hCryptProv, uint dwCertEncodingType,
           IntPtr pInfo, uint aiKeyAlg, uint dwFlags, uint pvAuxInfo, ref uint phKey);
        
        [DllImport("Crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool CryptImportPublicKeyInfo(uint hCryptProv, uint dwCertEncodingType,
           IntPtr pInfo, ref uint phKey);
        
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool CryptExportKey(uint hKey, uint hExpKey, uint dwBlobType,
           uint dwFlags, uint pbData, ref uint pdwDataLen);
        
        [DllImport("Crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool CertFreeCertificateContext(int pCertContext);
        
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool CryptReleaseContext(uint hProv, uint dwFlags);
    }
}
