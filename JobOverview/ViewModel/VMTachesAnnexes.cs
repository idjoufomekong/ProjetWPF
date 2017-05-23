using JobOverview.Entity;
using JobOverview.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace JobOverview.ViewModel
{
    class VMTachesAnnexes : ViewModelBase
    {
        #region Champs privés
        private string _usercourant;
        private List<Tache> _tachesAjoutés;
        #endregion

        #region Propriétés
        public ObservableCollection<Personne> Personnes { get;  private set; }
        public ObservableCollection<Activite> ActivitesAnnexes { get; private set; }
        #endregion

        #region Constructeur
        public VMTachesAnnexes()
        {
            // Récupération du code de l'utilisateur courant
            _usercourant = Properties.Settings.Default.CodeDernierUtilisateur;

            // Récupération de la liste des personnes avec leurs tâches annexes
            var listPers = DALTache.RecupererPersonnesTachesAnnexes(_usercourant);

            // Récupération de la liste des personnes avec leurs tâches annexes étendues
            // On se sert du booléen Assignation pour savoir si l'activité annexe est assignée ou non à l'employé
            DALTache.RecupererPersonnesTachesAnnexesEtendues(listPers);

            Personnes = new ObservableCollection<Personne>(listPers);
        }
        #endregion

        #region Commande
        private ICommand _cmdCheckerAnnexe;
        public ICommand CmdCheckerAnnexe
        {
            get
            {
                if (_cmdCheckerAnnexe == null)
                    _cmdCheckerAnnexe = new RelayCommand(AjouterTachesAnnexes);
                return _cmdCheckerAnnexe;
            }
        }
        #endregion

        private void AjouterTachesAnnexes()
        {
            if (_tachesAjoutés == null)
                _tachesAjoutés = new List<Tache>();

            var empCourant = (Personne)CollectionViewSource.GetDefaultView(Personnes).CurrentItem;
        }
    }
}
