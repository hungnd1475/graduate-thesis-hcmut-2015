﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace HCMUT.EMRCorefResol
{
    public class ClassifierSerializer
    {
        public void Serialize(IClassifier classifier, string path)
        {
            var writer = new XmlTextWriter(path, Encoding.Unicode);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;

            writer.WriteStartElement("Classifier");
            writer.WriteAttributeString("Type", classifier.GetType().AssemblyQualifiedName);
            classifier.WriteXml(writer, GetDirectory(path));
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }

        public IClassifier Deserialize(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var reader = new XmlTextReader(fs);
                reader.WhitespaceHandling = WhitespaceHandling.Significant;
                reader.Normalization = true;
                reader.XmlResolver = null;
                reader.Read();

                var type = Type.GetType(reader["Type"]);
                reader.ReadStartElement("Classifier");

                var classifier = (IClassifier)Activator.CreateInstance(type, reader, path);
                
                reader.Close();
                return classifier;
            }
        }

        private string GetDirectory(string filePath)
        {
            return Path.GetDirectoryName(Path.GetFullPath(filePath));
        }
    }
}
