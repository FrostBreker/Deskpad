using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DeskPad.Objects
{
    public class Session
    {
        private const string FILENAME = "session.xml";

        private static string _applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string _applicationPath = Path.Combine(_applicationDataPath, "Deskpad");

        private readonly XmlWriterSettings _writerSettings;

        public static string BackupPath = Path.Combine(_applicationDataPath, "Deskpad", "backup");

        /// <summary>
        /// Chemin d'acces et nom du fichgier repreésentant la séssion
        /// </summary>
        public static string FileName { get; } = Path.Combine(_applicationPath, FILENAME);

        [XmlAttribute(AttributeName = "ActiveIndex")]
        public int ActiveIndex { get; set; } = 0;

        [XmlElement(ElementName = "File")]
        public List<TextFile> TextFiles { get; set; }


        public Session()
        {
            TextFiles = new List<TextFile>();
            _writerSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = ("\t"),
                OmitXmlDeclaration = true
            };

            if (!Directory.Exists(_applicationPath))
            {
                Directory.CreateDirectory(_applicationPath);
            }
        }

        public static async Task<Session> Load()
        {
            var session = new Session();

            if (File.Exists(FileName))
            {
                var serializer = new XmlSerializer(typeof(Session));
                var streamReader = new StreamReader(FileName);

                try
                {
                    session = (Session)serializer.Deserialize(streamReader);

                    foreach (var file in session.TextFiles)
                    {
                        var fileName = file.FileName;
                        file.SafeFileName = Path.GetFileName(fileName);
                        var backupFileName = file.BackupFileName;

                        if (File.Exists(fileName))
                        {
                            using (StreamReader reader = new StreamReader(fileName))
                            {
                                file.Content = await reader.ReadToEndAsync();
                            }
                        };

                        if (File.Exists(backupFileName))
                        {
                            using (StreamReader reader = new StreamReader(backupFileName))
                            {
                                file.Content = await reader.ReadToEndAsync();
                            }
                        };
                    }
                }
                catch (Exception ex)
                {

                    System.Windows.Forms.MessageBox.Show("Une erreur s'est produite :" + ex.Message);
                }

                streamReader.Close();
            };

            return session;
        }

        public void Save()
        {
            var emptyNamespace = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(typeof(Session));
            using (XmlWriter writer = XmlWriter.Create(FileName, _writerSettings))
            {
                serializer.Serialize(writer, this, emptyNamespace);
            };
        }

        public async void BackupFile(TextFile file)
        {
            if (!Directory.Exists(BackupPath))
            {
                await Task.Run(() => Directory.CreateDirectory(BackupPath));
            }

            if (file.FileName.StartsWith("Sans titre"))
            {
                using (StreamWriter writer = File.CreateText(file.BackupFileName))
                {
                    await writer.WriteAsync(file.Content);
                }
            };
        }


    }
}