using JobOverview.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
            XmlSerializer serializer = new XmlSerializer(typeof(List<Personne>),
                              new XmlRootAttribute("Personne"));

            using (var sw = new StreamWriter(@"..\..\Taches.xml"))
            {
                serializer.Serialize(sw, listPersonne);
            }

        }
    }
}
