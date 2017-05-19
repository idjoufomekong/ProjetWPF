using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JobOverview.Entity
{
    public class Tache
    {
        public Guid IdTache { get; set; }
        [XmlAttribute]
        public string Libelle { get; set; }
        [XmlAttribute]
        public bool Annexe { get; set; }
        [XmlAttribute]
        public string Activite { get; set; }
        [XmlAttribute]
        public string Description { get; set; }
        [XmlElement]
        public List<Travail> TravauxAnnexes { get; set; }
    }

    public class Travail
    {
        [XmlAttribute]
        public DateTime Date { get; set; }
        [XmlAttribute]
        public float Heures { get; set; }
        [XmlAttribute]
        public float TauxProductivite { get; set; }
    }

    public class TacheProd : Tache
    {
        public int NumTache { get; set; }
        [XmlAttribute]
        public float DureePrevue { get; set; }
        [XmlAttribute]
        public float DureeRestante { get; set; }
        [XmlAttribute]
        public string Logiciel { get; set; }
        [XmlAttribute]
        public string Module { get; set; }
        [XmlAttribute]
        public string Version { get; set; }
        [XmlElement]
        public List<Travail> TravauxProd { get; set; }
    }
}
