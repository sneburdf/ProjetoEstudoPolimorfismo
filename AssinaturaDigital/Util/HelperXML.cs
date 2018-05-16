using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using AssinaturaDigital.TO;
using System.Xml.Serialization;

namespace AssinaturaDigital.Util
{
    public static class HelperXml
    {
        private static TOXmlConfiguracao dadosConfiguracao;

        public static TOXmlConfiguracao DadosConfiguracao
        {
            get
            {
                if (dadosConfiguracao != null)
                    return dadosConfiguracao;

                if (!File.Exists(HelperEstrutura.PathXmlConfiguracoesUsuario))
                    return new TOXmlConfiguracao();

                var objeto = DeserializeXml<TOXmlConfiguracao>(File.ReadAllText(HelperEstrutura.PathXmlConfiguracoesUsuario));

                if (objeto == null)
                    return new TOXmlConfiguracao();

                dadosConfiguracao = objeto;

                return dadosConfiguracao;
            }
        }

        public static void SalvarConfiguracoes(TOXmlConfiguracao Dados)
        {
            var xml = SerializeToXml<TOXmlConfiguracao>(Dados);

            File.WriteAllText(HelperEstrutura.PathXmlConfiguracoesUsuario, xml);

            dadosConfiguracao = Dados;
        }

        public static void SalvarUltimoDiretorioLFEs(string Dir)
        {
            DadosConfiguracao.UltimoDiretorioLFEs = Dir;

            SalvarConfiguracoes(DadosConfiguracao);
        }

        public static void SalvarUltimoDiretorioPDFs(string Dir)
        {
            DadosConfiguracao.UltimoDiretorioSalvamentoPDFs = Dir;

            SalvarConfiguracoes(DadosConfiguracao);
        }

        public static void AdicionarArquivoValidadoNoXml(String DataHoraValidacao, string PathOriginal, string PathValidado, string Hash, string NomeGeradoParaOArquivo, bool ValidadoComSucesso)
        {
            XmlDocument doc;

            if (File.Exists(HelperEstrutura.PathXmlArquivosValidados))
            {
                doc = CarregarXmlCriptografado(HelperEstrutura.PathXmlArquivosValidados);
            }
            else
            {
                doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "ISO-8859-1", null));
                doc.AppendChild(doc.CreateElement("arquivos"));
                SalvarXmlCriptografado(doc, HelperEstrutura.PathXmlArquivosValidados);
            }

            bool arquivoJaExisteNoXml = false;

            foreach (XmlNode arq in doc.GetElementsByTagName("arquivo"))
                if (arq.Attributes["caminhoArquivoOriginal"].Value == PathOriginal)
                    arquivoJaExisteNoXml = true;

            if (arquivoJaExisteNoXml)
            {
                foreach (XmlNode arq in doc.GetElementsByTagName("arquivo"))
                {
                    if (arq.Attributes["caminhoArquivoOriginal"].Value == PathOriginal)
                    {
                        arq.Attributes["caminhoArquivoValidado"].Value = PathValidado;
                        arq.Attributes["hash"].Value = Hash;
                        arq.Attributes["sucessoValidacao"].Value = ValidadoComSucesso.ToString();
                        arq.Attributes["nomeGeradoParaOArquivo"].Value = NomeGeradoParaOArquivo;
                        arq.Attributes["DataHoraValidacao"].Value = DataHoraValidacao;
                    }
                }
            }
            else
            {
                XmlElement novoArquivo = doc.CreateElement("arquivo");

                XmlAttribute pathArquivoOriginal = doc.CreateAttribute("caminhoArquivoOriginal");
                pathArquivoOriginal.Value = PathOriginal;

                XmlAttribute pathArquivoValidado = doc.CreateAttribute("caminhoArquivoValidado");
                pathArquivoValidado.Value = PathValidado;

                XmlAttribute hash = doc.CreateAttribute("hash");
                hash.Value = Hash;

                XmlAttribute sucesso = doc.CreateAttribute("sucessoValidacao");
                sucesso.Value = ValidadoComSucesso.ToString();

                XmlAttribute nomeGeradoArquivo = doc.CreateAttribute("nomeGeradoParaOArquivo");
                nomeGeradoArquivo.Value = NomeGeradoParaOArquivo;

                XmlAttribute dataHoraValidacao = doc.CreateAttribute("DataHoraValidacao");
                dataHoraValidacao.Value = DataHoraValidacao;

                novoArquivo.Attributes.Append(pathArquivoOriginal);
                novoArquivo.Attributes.Append(pathArquivoValidado);
                novoArquivo.Attributes.Append(hash);
                novoArquivo.Attributes.Append(sucesso);
                novoArquivo.Attributes.Append(nomeGeradoArquivo);
                novoArquivo.Attributes.Append(dataHoraValidacao);

                XmlElement elemPai = (XmlElement)doc.GetElementsByTagName("arquivos")[0];
                elemPai.AppendChild(novoArquivo);
            }

            SalvarXmlCriptografado(doc, HelperEstrutura.PathXmlArquivosValidados);
        }

        public static void RemoverArquivoValidadoDoXml(string PathArquivoOriginal)
        {
            XmlDocument doc = CarregarXmlCriptografado(HelperEstrutura.PathXmlArquivosValidados);

            XmlElement elemPai = (XmlElement)doc.GetElementsByTagName("arquivos")[0];

            foreach (XmlNode arquivo in elemPai.ChildNodes)
            {
                if (arquivo.Attributes["caminhoArquivoOriginal"].Value.ToLower() == PathArquivoOriginal.ToLower())
                    elemPai.RemoveChild(arquivo);
            }

            SalvarXmlCriptografado(doc, HelperEstrutura.PathXmlArquivosValidados);
        }

        public static List<Arquivo> PegarArquivosValidadosDoXml()
        {
            var retorno = new List<Arquivo>();

            if (!File.Exists(HelperEstrutura.PathXmlArquivosValidados))
                return retorno;

            XmlDocument doc = CarregarXmlCriptografado(HelperEstrutura.PathXmlArquivosValidados);

            var arquivos = doc.GetElementsByTagName("arquivo");

            if (arquivos != null)
            {
                foreach (XmlNode arquivo in arquivos)
                {
                    string caminhoOriginal = arquivo.Attributes["caminhoArquivoOriginal"].Value;
                    string caminhoValidado = arquivo.Attributes["caminhoArquivoValidado"].Value;
                    string hash = arquivo.Attributes["hash"].Value;
                    bool validado = Convert.ToBoolean(arquivo.Attributes["sucessoValidacao"].Value);
                    string nomeGerado = arquivo.Attributes["nomeGeradoParaOArquivo"].Value;
                    string dataHoraValidacao = null;
                    if(arquivo.Attributes["DataHoraValidacao"]!=null)
                        dataHoraValidacao = arquivo.Attributes["DataHoraValidacao"].Value;

                    if (File.Exists(caminhoOriginal) || (File.Exists(caminhoValidado) && validado))
                    {
                        try
                        {
                            retorno.Add(new Arquivo(dataHoraValidacao, caminhoOriginal, caminhoValidado, hash, validado, nomeGerado));
                            GC.Collect();
                            continue;
                        }
                        catch (Arquivo.ExcecaoArquivoValidadoNaoEncontrado){}
                    }
                    RemoverArquivoValidadoDoXml(caminhoOriginal);
                    
                    GC.Collect();
                }
            }

            //Limpando diretório de dados quando não há nenhum arquivo. É uma forma simples para não manter uma grande 
            //quantidade de arquivos que não serão mais utilizados.
            if (retorno.Count == 0)
                HelperEstrutura.TentarDeletarArquivosDoDiretorio(HelperEstrutura.DirDadosValidados);

            return retorno;
        }

        private static void SalvarXmlCriptografado(XmlDocument XmlDocument, string PathASalvar)
        {
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                XmlDocument.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                string xmlString = stringWriter.GetStringBuilder().ToString();

                File.WriteAllBytes(PathASalvar, HelperCriptografia.Criptografar(xmlString));
            }
        }

        public static XmlDocument CarregarXmlCriptografado(string PathACarregar)
        {
            XmlDocument doc = new XmlDocument();

            string xmlDescript = HelperCriptografia.Descriptografar(File.ReadAllBytes(PathACarregar));

            using (var textReader = new StringReader(xmlDescript))
            {
                doc.Load(textReader);

                return doc;
            }
        }

        public static void SalvarResumoLivroEmXml(TOResumoLivro ObjResumo, string CaminhoASalvar)
        {
            string xml = SerializeToXml<TOResumoLivro>(ObjResumo);

            byte[] criptografado = HelperCriptografia.Criptografar(xml);

            File.WriteAllBytes(CaminhoASalvar, criptografado);
        }

        public static TOResumoLivro PegarResumoLivroDoXml(string CaminhoXml)
        {
            if (!File.Exists(CaminhoXml))
                throw new Exception("O arquivo de resumo foi excluído ou está inconsistente!");

            string xml = HelperCriptografia.Descriptografar(File.ReadAllBytes(CaminhoXml));
            
            return DeserializeXml<TOResumoLivro>(xml);
        }

        /// <summary>
        /// Atualiza o resumo no xml e no objeto
        /// </summary>
        public static TOResumoLivro AtualizarResumoLivroComEnvio(string CaminhoXml, TODadosEnvio DadosEnvio)
        {
            if (!File.Exists(CaminhoXml))
                throw new Exception("O arquivo de resumo foi excluído ou está inconsistente!");

            string xml = HelperCriptografia.Descriptografar(File.ReadAllBytes(CaminhoXml));

            var resumo = DeserializeXml<TOResumoLivro>(xml);

            resumo.DadosEnvio = DadosEnvio;

            SalvarResumoLivroEmXml(resumo, CaminhoXml);

            return resumo;
        }


        private static string SerializeToXml<T>(T obj)
        {
            using (StringWriter textWriter = new StringWriter())
            {
                XmlSerializer xmlSer = new XmlSerializer(typeof(T));

                xmlSer.Serialize(textWriter, obj);

                return textWriter.ToString();
            }
        }

        public static T DeserializeXml<T>(string xml)
        {
            T objeto = default(T);

            StringReader strReader = new StringReader(xml);
            XmlReader xmlReader = XmlReader.Create(strReader);
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            if (serializer.CanDeserialize(xmlReader))
                objeto = (T)serializer.Deserialize(xmlReader);

            xmlReader.Close();

            return objeto;
        }

    }

}
