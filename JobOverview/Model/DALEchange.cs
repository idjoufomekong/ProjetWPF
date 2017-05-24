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
        public static void ExporterXML(List<Personne> lstPers, string path)
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

                    if (col.TachesProd != null)
                    {
                        writer.WriteStartElement("TâchesProd");
                        foreach (TacheProd a in col.TachesProd)
                        {
                            writer.WriteStartElement("TâcheProd");
                            writer.WriteAttributeString("Nom", a.NomTache.ToString());
                            writer.WriteAttributeString("Activité", a.CodeActivite.ToString());
                            if(!string.IsNullOrEmpty( a.Description))
                            writer.WriteAttributeString("Description", a.Description.ToString());
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
                //TODO: Améliorer les noms des colonnes excel: mettre prod pour les tâches prod et annexes pour les autres
            }
        }
    }
}
