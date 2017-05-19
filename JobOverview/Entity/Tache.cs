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
        public string NomTache { get; set; }
        [XmlAttribute]
        public bool Annexe { get; set; }
        [XmlAttribute]
        public string CodeActivite { get; set; }
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
        public string CodeLogiciel { get; set; }
        [XmlAttribute]
        public string CodeModule { get; set; }
        [XmlAttribute]
        public string CodeVersion { get; set; }
        [XmlElement]
        public List<Travail> TravauxProd { get; set; }
    }
}
