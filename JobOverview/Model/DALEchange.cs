using JobOverview.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Xml;

namespace JobOverview.Model
{
    public class DALEchange
    {
        /// <summary>
        /// Export de la liste complète des tâches de l'équipe du manager connecté
        /// </summary>
        /// <param name="listPersonne">Liste des membres de l'équipe du manager connecté</param>
        /// <param name="path">Chemin du fichier de sauvegarde</param>
        public static void ExporterXML(List<Personne> listPersonne)//,string path)
        {

            SaveFileDialog dos = new SaveFileDialog();
            dos.Filter = "XML Files (*.xml)|*.xml";
            dos.DefaultExt = "xml";
            dos.AddExtension = true;
            if (dos.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dos.FileName))
            {
                // Exportation de la liste sous format xml
                XmlSerializer serializer = new XmlSerializer(typeof(List<Personne>),
                                  new XmlRootAttribute("Personne"));

                using (var sw = new StreamWriter(dos.FileName))
                {
                    serializer.Serialize(sw, listPersonne);
                }

            }

        }

        public static void ExporterXML2(List<Personne> lstPers, string path)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                // Ecriture du prologue
                writer.WriteStartDocument();

                // Ecriture de l'élément racine
                writer.WriteStartElement("Personnes"); //Ajouter espace de nom
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");

                // Ecriture du contenu interne, avec une structure différente
                // de celle de la collection d'objets passée en paramètre
                foreach (Personne col in lstPers)
                {
                    writer.WriteStartElement("Personne");
                    writer.WriteAttributeString("Login", col.CodePersonne);
                    //writer.WriteAttributeString("Activite", col.Activite);
                    //writer.WriteAttributeString("Personne", col.Login);
                    //writer.WriteAttributeString("DureePrev", col.DureePrevue.ToString());
                    //writer.WriteAttributeString("DureeRest", col.DureeRestante.ToString());
                    //writer.WriteAttributeString("Logiciel", col.Logiciel);
                    //writer.WriteAttributeString("Module", col.Module);
                    //writer.WriteAttributeString("Version", col.NumeroVersion.ToString());


                    if (col.TachesProd != null)
                    {
                        writer.WriteStartElement("TâchesProd");
                        foreach (TacheProd a in col.TachesProd)
                        {
                            writer.WriteStartElement("TâcheProd");
                            writer.WriteAttributeString("Numéro", a.NumTache.ToString());
                            writer.WriteAttributeString("DureePrevue", a.DureePrevue.ToString());
                            writer.WriteAttributeString("DureeRestante", a.DureeRestante.ToString());
                            writer.WriteAttributeString("Logiciel", a.CodeLogiciel.ToString());
                            writer.WriteAttributeString("Module", a.CodeModule.ToString());
                            writer.WriteAttributeString("Version", a.CodeVersion.ToString());

                            if (a.TravauxAnnexes != null)
                            {
                                writer.WriteStartElement("TravauxProd");
                                foreach (Travail b in a.TravauxProd)
                                {
                                    writer.WriteStartElement("TravailProd");
                                    writer.WriteAttributeString("Date", b.Date.ToString("yyyy-MM-ddTHH\\:mm\\:ss"));
                                    writer.WriteAttributeString("Heures", b.Heures.ToString());
                                    writer.WriteAttributeString("TauxProductivité", b.TauxProductivite.ToString());
                                    writer.WriteEndElement();
                                }
                            writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    if (col.TachesAnnexes != null)
                    {
                        writer.WriteStartElement("TâchesAnnexes");

                        foreach (Tache c in col.TachesAnnexes)
                        {
                            writer.WriteStartElement("TâcheAnnexe");
                            writer.WriteAttributeString("Nom", c.NomTache.ToString());
                            writer.WriteAttributeString("Annexe", c.Annexe.ToString());
                            writer.WriteAttributeString("Activité", c.CodeActivite.ToString());
                            writer.WriteAttributeString("Description", c.Description.ToString());

                            if (c.TravauxAnnexes != null)
                            {
                                writer.WriteStartElement("TravauxAnnexes");
                                foreach (Travail d in c.TravauxAnnexes)
                                {
                                    writer.WriteStartElement("TravailAnnexe");
                                    writer.WriteAttributeString("Date", d.Date.ToString("yyyy-MM-ddTHH\\:mm\\:ss"));
                                    writer.WriteAttributeString("Heures", d.Heures.ToString());
                                    writer.WriteAttributeString("TauxProductivité", d.TauxProductivite.ToString());
                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                // Ecriture de la balise fermante de l'élément racine et fin du document
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
