using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.ComponentModel;

namespace AssinaturaDigital.Util
{
    public class HelperCompressao
    {
        private ZipOutputStream ZipOutputStream;

        /// <summary>
        /// Caminho e nome do arquivo que está sendo adicionado atualmente no .zip
        /// </summary>
        public string ArquivoAtual { get; private set; }

        public void ForcarParadaCompressao()
        {
            try
            {
                this.ZipOutputStream.Finish();
            }
            catch { }
        }

        public void ZiparArquivo(string PathArquivoZip, string PathArquivoAZipar)
        {
            ZiparArquivo(PathArquivoZip, PathArquivoAZipar, null);
        }

        /// <summary>
        /// Comprime arquivo e permite o acompanhamento do progresso através de um evento.
        /// </summary>
        public void ZiparArquivo(string PathArquivoZip, string PathArquivoAZipar, 
            EventHandler<StreamWithProgress.ProgressChangedEventArgs> EventoProgressChanged)
        {
            using (FileStream fileStream = File.Create(PathArquivoZip))
            {
                using (ZipOutputStream = new ZipOutputStream(fileStream))
                {
                    ZipOutputStream.SetLevel(5);

                    AdicionarArquivo(PathArquivoAZipar, Path.GetFileName(PathArquivoAZipar), ZipOutputStream, EventoProgressChanged);

                    ZipOutputStream.IsStreamOwner = true;//Makes the Close also Close the underlying stream
                }
            }
        }

        /// <summary>
        /// Comprime arquivos e permite o acompanhamento do progresso através de um evento.
        /// </summary>
        public void ZiparArquivos(string PathArquivoZip, string[] PathsArquivosAZipar,
            EventHandler<StreamWithProgress.ProgressChangedEventArgs> EventoProgressChanged)
        {
            IDictionary<string,string> dicArquivosAZipar = new Dictionary<string,string>();
            foreach(string pathArquivoAZipar in PathsArquivosAZipar){
                dicArquivosAZipar.Add(Path.GetFileName(pathArquivoAZipar), pathArquivoAZipar);
            }
            ZiparArquivos(PathArquivoZip, dicArquivosAZipar, EventoProgressChanged);
        }
        public void ZiparArquivos(string PathArquivoZip, IDictionary<string,string> PathsArquivosAZipar,
            EventHandler<StreamWithProgress.ProgressChangedEventArgs> EventoProgressChanged)
        {
            using (FileStream fileStream = File.Create(PathArquivoZip))
            {
                using (ZipOutputStream = new ZipOutputStream(fileStream))
                {
                    ZipOutputStream.SetLevel(5);

                    ZipOutputStream.Password = Seguranca.SenhaCompressao;

                    foreach (string nomeDentroDoZip in PathsArquivosAZipar.Keys)
                    {
                        AdicionarArquivo(PathsArquivosAZipar[nomeDentroDoZip], nomeDentroDoZip, ZipOutputStream, EventoProgressChanged);
                    }

                    ZipOutputStream.IsStreamOwner = true;//Makes the Close also Close the underlying stream
                }
            }
        }

        public void ZiparDiretorio(string PathArquivoZip, string PathDiretorioAZipar)
        {
            ZiparDiretorio(PathArquivoZip, PathDiretorioAZipar, null);
        }

        public void ZiparDiretorio(string PathArquivoZip, string PathDiretorioAZipar, string Password)
        {
            using (FileStream fsOut = File.Create(PathArquivoZip))
            {
                using (this.ZipOutputStream = new ZipOutputStream(fsOut))
                {

                    ZipOutputStream.SetLevel(5); //0-9, 9 being the highest level of compression

                    ZipOutputStream.Password = Password;	// optional. Null is the same as not setting. Required if using AES.

                    AdicionarDiretorioRecursivamente(PathDiretorioAZipar, ZipOutputStream);

                    ZipOutputStream.IsStreamOwner = true;	// Makes the Close also Close the underlying stream
                }
            }
        }

        public void Dezipar(string PathDoZip, string PathDiretorioDestino)
        {
            FileStream fileStreamIn = new FileStream(PathDoZip, FileMode.Open, FileAccess.Read);
            ZipInputStream zipInStream = new ZipInputStream(fileStreamIn);
            ZipEntry entry = zipInStream.GetNextEntry();

            if (!Directory.Exists(PathDiretorioDestino))
                Directory.CreateDirectory(PathDiretorioDestino);

            FileStream fileStreamOut = new FileStream
                (PathDiretorioDestino + @"\" + entry.Name, FileMode.Create, FileAccess.Write);
            int size;
            byte[] buffer = new byte[4096];
            do
            {
                size = zipInStream.Read(buffer, 0, buffer.Length);
                fileStreamOut.Write(buffer, 0, size);
            } while (size > 0);

            zipInStream.Close();
            fileStreamOut.Close();
            fileStreamIn.Close();
        }

        /// <summary>
        /// Adiciona ao zip o diretório, com seus subdiretórios e arquivos
        /// </summary>
        private void AdicionarDiretorioRecursivamente(string Path, ZipOutputStream ZipStream)
        {
            foreach (string filename in Directory.GetFiles(Path))
            {
                // This setting will strip the leading part of the folder path in the entries, to
                // make the entries relative to the starting folder.
                // To include the full path for each entry up to the drive root, assign folderOffset = 0.
                int folderOffset = Path.Length + (Path.EndsWith("\\") ? 0 : 1);
                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction

                AdicionarArquivo(filename, entryName, ZipStream, null);
            }
            string[] folders = Directory.GetDirectories(Path);
            foreach (string folder in folders)
            {
                AdicionarDiretorioRecursivamente(folder, ZipStream);
            }
        }

        private void AdicionarArquivo(string PathArquivo, string PathDentroDoZip, ZipOutputStream ZipStream, 
            EventHandler<StreamWithProgress.ProgressChangedEventArgs> EventoProgressChanged)
        {
            this.ArquivoAtual = PathArquivo;

            ZipEntry newEntry = new ZipEntry(PathDentroDoZip);

            FileInfo fileInfo = new FileInfo(PathArquivo);

            newEntry.DateTime = fileInfo.LastWriteTime; // Note the zip format stores 2 second granularity

            // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
            // A password on the ZipOutputStream is required if using AES.
            //   newEntry.AESKeySize = 256;

            // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
            // you need to do one of the following: Specify UseZip64.Off, or set the Size.
            // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
            // but the zip will be in Zip64 format which not all utilities can understand.
            //   zipStream.UseZip64 = UseZip64.Off;
            newEntry.Size = fileInfo.Length;

            ZipStream.PutNextEntry(newEntry);

            // Zip the file in buffered chunks
            // the "using" will close the stream even if an exception occurs
            byte[] buffer = new byte[4096];
            using (FileStream streamReader = File.OpenRead(PathArquivo))
            {
                using (StreamWithProgress streamWithProgress = new StreamWithProgress(streamReader))
                {
                    streamWithProgress.ProgressChanged += EventoProgressChanged;

                    StreamUtils.Copy(streamWithProgress, ZipStream, buffer);
                }
            }
            ZipStream.CloseEntry();
        }



        /// <summary>
        /// Estima o resultado de uma compressão (tamanho em Bytes)
        /// </summary>
        public long EstimarTamanhoCompressao(string PathArquivo)
        {
            long tamanhoEstimado = 0;

            string arquivoTemp = Path.Combine(HelperEstrutura.DirTemp, Guid.NewGuid().ToString() + ".zip");

            using (FileStream fileStream = File.Create(arquivoTemp))
            {
                ZipOutputStream zipOutputStreamLocal = new ZipOutputStream(fileStream);

                //Definição do evento
                EventHandler<StreamWithProgress.ProgressChangedEventArgs> eventoProgressChanged =
                    (object obj, StreamWithProgress.ProgressChangedEventArgs args) =>
                    {
                        long lengthArquivoOrigem = new FileInfo(PathArquivo).Length;

                        //realizar o cálculo com 5% de compressão do arquivo
                        if (args.BytesRead >= (lengthArquivoOrigem / 100) * 5)
                        {
                            long lengthArquivoComprimindo = new FileInfo(arquivoTemp).Length;
                            tamanhoEstimado = (lengthArquivoComprimindo * lengthArquivoOrigem) / args.BytesRead;

                            //forçar parada
                            try
                            {
                                zipOutputStreamLocal.Close();
                            }
                            catch { }
                        }
                    };

                zipOutputStreamLocal.SetLevel(5);

                zipOutputStreamLocal.IsStreamOwner = true;

                try //Tratando exceção gerada por cancelar o processamento
                {
                    AdicionarArquivo(PathArquivo, Path.GetFileName(PathArquivo), zipOutputStreamLocal, eventoProgressChanged);
                }
                catch { }
            }

            File.Delete(arquivoTemp);

            return tamanhoEstimado;
        }



    }
}
