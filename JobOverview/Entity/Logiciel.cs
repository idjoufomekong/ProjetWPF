using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobOverview.Entity
{
    public class Logiciel
    {
        public string CodeLogiciel { get; set; }
        public string  NomLogiciel { get; set; }
        public List<Version> Versions { get; set; }
        public List<Module> Modules { get; set; }
    }

    public class Version
    {
        public float NumVersion { get; set; }
        public short Millesime { get; set; }
        public DateTime DateOuverture { get; set; }
        public DateTime DateSortiePrevue { get; set; }
        public DateTime DateSortieReelle { get; set; }
        public int NombreReleases { get; set; }
        public TimeSpan Ecart {
            get
            {
                return DateSortieReelle.Subtract(DateSortiePrevue);
            }
        }
        public double TempsTotalRealise { get; set; }
    }
    public class Module
    {
        public string CodeModule { get; set; }
        public string NomModule { get; set; }
        public double TempsRealise { get; set; }
    }
}
