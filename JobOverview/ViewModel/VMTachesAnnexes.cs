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
    /// <summary>
    /// ViewModel associée au UserControl "UCTachesAnnexes"
    /// </summary>
    class VMTachesAnnexes : ViewModelBase
    {
        //TODO : [Gestion des tâches annexes] --> Lors de la modification d'une tâche d'une personne, il faudrait verrouiller la liste des employés.
        //                                        Il faudrait alors cliquer sur le bouton enregistrer pour pouvoir sélectionner un autre employé.
        //                                        Ce cas se présente seulement si l'utilisateur de l'application a le statut de manager.

        //TODO : [Gestion des tâches annexes] --> Il reste à gérer le rafraîchissement du champ description suite à une suppression.

        #region Champs privés
        private string _usercourant;
        private List<Personne> _listPers;
        #endregion

        #region Propriétés
        public ObservableCollection<Personne> Personnes { get; private set; }
        public bool StatutManager { get; private set; }
        public bool Manager { get; } = Properties.Settings.Default.Manager;
        #endregion

        #region Constructeur
        public VMTachesAnnexes()
        {
            // Récupération du statut manager de la personne connecté
            // True = la personne connectée est manager
            StatutManager = Properties.Settings.Default.Manager;

            // Récupération du code de l'utilisateur courant
            _usercourant = Properties.Settings.Default.CodeDernierUtilisateur;

            // Récupération de la liste des personnes avec leurs tâches annexes
            _listPers = DALTache.RecupererPersonnesTachesAnnexes(_usercourant);

            // Récupération de la liste des personnes avec leurs tâches annexes étendues
            // On se sert du booléen Assignation pour savoir si l'activité annexe est affectée ou non à l'employé
            DALTache.RecupererPersonnesTachesAnnexesEtendues(_listPers);

            Personnes = new ObservableCollection<Personne>(_listPers);
        }
        #endregion

        #region Commande
        /// <summary>
        /// Commande d'enregistrement et de suppression des tâches annexes
        /// </summary>
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
        /// Ajoute ou supprime des tâches annexes dans la base de données
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

                // On ajoute ou supprime une tâche annexe si l'assignation associée change de valeur
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
                    }
                }
            }
        }
        #endregion
    }
}
