using JobOverview.Entity;
using JobOverview.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobOverview.ViewModel
{
    class VMTachesAnnexes : ViewModelBase
    {
        #region Champs privés
        private string _usercourant;
        #endregion

        #region Propriétés
        public ObservableCollection<Personne> Personnes { get;  private set; }
        public ObservableCollection<Activite> ActivitesAnnexes { get; private set; }
        #endregion

        public VMTachesAnnexes()
        {
            // Récupération du code de l'utilisateur courant
            _usercourant = Properties.Settings.Default.CodeDernierUtilisateur;

            Personnes = new ObservableCollection<Personne>(DALTache.RecupererPersonnesTachesAnnexes(_usercourant));
            ActivitesAnnexes = new ObservableCollection<Activite>(DALTache.RecupererActivitesAnnexes());
        }
    }
}
