using JobOverview.Entity;
using JobOverview.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace JobOverview.ViewModel
{
    class VMTachesAnnexes : ViewModelBase
    {
        #region Champs privés
        private string _usercourant;
        private List<Personne> _listPers;
        #endregion

        #region Propriétés
        public ObservableCollection<Personne> Personnes { get; private set; }
        #endregion

        #region Constructeur
        public VMTachesAnnexes()
        {
            // Récupération du code de l'utilisateur courant
            _usercourant = Properties.Settings.Default.CodeDernierUtilisateur;

            // Récupération de la liste des personnes avec leurs tâches annexes
            _listPers = DALTache.RecupererPersonnesTachesAnnexes(_usercourant);

            // Récupération de la liste des personnes avec leurs tâches annexes étendues
            // On se sert du booléen Assignation pour savoir si l'activité annexe est assignée ou non à l'employé
            DALTache.RecupererPersonnesTachesAnnexesEtendues(_listPers);

            Personnes = new ObservableCollection<Personne>(_listPers);
        }
        #endregion

        #region Commande
        private ICommand _cmdEnregistrer;
        public ICommand CmdEnregistrer
        {
            get
            {
                if (_cmdEnregistrer == null)
                    _cmdEnregistrer = new RelayCommand(EnregistrerTachesAnnexes);
                return _cmdEnregistrer;
            }
        }
        #endregion

        #region Méthode associée à la commande
        /// <summary>
        /// Ajoute ou supprime des tâches annexes
        /// </summary>
        /// <param name="obj"></param>
        private void EnregistrerTachesAnnexes(object obj)
        {
            // On récupère l'employé courant
            var empCourant = (Personne)CollectionViewSource.GetDefaultView(Personnes).CurrentItem;

            // Liste des tâches annexes de départ (soit avant toute modification)
            var listPers = DALTache.RecupererPersonnesTachesAnnexes(_usercourant);
            DALTache.RecupererPersonnesTachesAnnexesEtendues(listPers);
            var listTachesDépart = listPers.Where(p => p.CodePersonne == empCourant.CodePersonne).FirstOrDefault().TachesAnnexes;

            // On compare la liste actuelle des tâches annexes de l'employé à celle de départ
            // Cela permet de détecter les changements effectués par l'utilisateur.
            foreach (var t in empCourant.TachesAnnexes)
            {
                // Assignation de départ de la tâche annexes courante
                var assignationDépart = listTachesDépart.Where(a => a.CodeActivite == t.CodeActivite).FirstOrDefault().Assignation;

                // On ajoute ou supprimer une tâche annexe si l'assignation associée change de valeur
                if (t.Assignation != assignationDépart)
                {
                    if (t.Assignation)
                        try
                        {
                            DALTache.AjouterTacheAnnexe(empCourant.CodePersonne, t);                        // Ajout
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Erreur");
                        }
                    else
                    {
                        try
                        {
                            DALTache.SupprimerTacheAnnexe(empCourant.CodePersonne, t.CodeActivite);         // Suppression
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Erreur");
                        }

                        //TODO : [Gestion des tâches annexes] --> RAZ du champ description, améliorer la gestion de ce champ

                        //TODO : [Gestion des tâches annexes] --> Le champ description est limité à 1000 caractères
                    }
                }
            }
        }
        #endregion
    }
}
