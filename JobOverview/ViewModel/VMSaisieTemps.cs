using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobOverview.Entity;
using JobOverview.Model;
using JobOverview.View;
using System.Collections.ObjectModel;

namespace JobOverview.ViewModel
{
    public class VMSaisieTemps : ViewModelBase
    {
        public ObservableCollection<Logiciel> Logiciels { get; set; }
        public Personne Utilisateur { get; set; }
        public VMSaisieTemps(ObservableCollection<Logiciel> LogicielsVMMain)
        {
            Utilisateur = DALPersonne.RecupererPersonneConnecte(Properties.Settings
                .Default.CodeDernierUtilisateur).FirstOrDefault();
            Logiciels = LogicielsVMMain;
        }
    }

}
