using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public float TotalRealise
        {
            get
            {
                if (TachesProd != null)
                    return TachesProd.Sum(x => x.Tempstravaille);
                else return 0;
            }
        }
        public float TotalRestant
        {
            get
            {
                if (TachesProd != null)
                    return TachesProd.Sum(x => x.DureeRestante);
                else return 0;
            }
        }
        [XmlElement]
        public ObservableCollection<TacheProd> TachesProd { get; set; }
        [XmlElement]
        public List<Tache> TachesAnnexes { get; set; }
        public List<Activite> Activites { get; set; }
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
