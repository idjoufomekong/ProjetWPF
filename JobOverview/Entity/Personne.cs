using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JobOverview.Entity
{
    public class Personne
    {
        [XmlAttribute]
        public string CodePersonne { get; set; }
        public string NomPrenom { get; set; }
        public string CodeMetier { get; set; }
        public bool Manager { get; set; } = false;
        [XmlElement]
        public List<TacheProd> TachesProd { get; set; }
        [XmlElement]
        public List<Tache> TachesAnnexes { get; set; }
    }

    public class Metier
    {
        public string  CodeMetier { get; set; }
        public List<Activite> Activites { get; set; }
    }

    public class Activite
    {
        public string CodeActivite { get; set; }
        public string NomActivite { get; set; }
    }
}
