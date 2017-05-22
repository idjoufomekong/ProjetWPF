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
        public float CodeVersion { get; set; }
        [XmlElement]
        public List<Travail> TravauxProd { get; set; }
    }

    public class TacheBrut
    {
        public Guid IdTache { get; set; }
        public string NomTache { get; set; }
        public bool Annexe { get; set; }
        public string CodeActivite { get; set; }
        public string Description { get; set; }
        public string Login { get; set; }
        public int NumTache { get; set; }
        public float DureePrevue { get; set; }
        public float DureeRestante { get; set; }
        public string CodeLogiciel { get; set; }
        public string CodeModule { get; set; }
        public float CodeVersion { get; set; }
        public DateTime Date { get; set; }
        public float Heures { get; set; }
        public float TauxProductivite { get; set; }
    }
}
