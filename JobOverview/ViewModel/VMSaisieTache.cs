using JobOverview.Entity;
using JobOverview.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace JobOverview.ViewModel
{
    public class VMSaisieTache:ViewModelBase
    {

        //Champs privés
        private Logiciel _logicielCourant;
        private Entity.Version _versionCourante;
        private Activite _activiteCourante;
        private Module _moduleCourant;
        private Personne _personneCourante;

        #region Propriétés
        public List<Personne> Personnes { get; set; }
        public List<Module> Modules { get; set; }
        public ObservableCollection<Activite> Activites { get; set; }
        public ObservableCollection<Personne> PersonnesTachesProdAjoutees { get; set; }
        public ObservableCollection<TacheProd> TachesProdAjoutees { get; set; }
        public string Libelle { get; set; }
        public string Description { get; set; }
        public float DureePrevue { get; set; }
        public float DureeRestante { get; set; }
        #endregion

        public VMSaisieTache(string logiciel, float numVersion)
        {
            Personnes = DALPersonne.RecupererToutesPersonneActivite(Properties.Settings.Default.CodeDernierUtilisateur);
            Modules = DALLogiciel.RecupererModules(logiciel);
            PersonnesTachesProdAjoutees = new ObservableCollection<Personne>();
            _logicielCourant = new Entity.Logiciel();
            _logicielCourant.CodeLogiciel = logiciel;
            _versionCourante = new Entity.Version();
            _versionCourante.NumVersion = numVersion;
            TachesProdAjoutees = new ObservableCollection<TacheProd>();
        }

        #region Commandes
        private ICommand _cmdAjouter;
        public ICommand CmdAjouter
        {
            get
            {
                if (_cmdAjouter == null)
                    _cmdAjouter = new RelayCommand(AjouterTache);
                return _cmdAjouter;
            }
        }
        private ICommand _cmdSupprimer;
        public ICommand CmdSupprimer
        {
            get
            {
                if (_cmdSupprimer == null)
                    _cmdSupprimer = new RelayCommand(SupprimerTache);
                return _cmdSupprimer;
            }
        }
        private ICommand _cmdEnregistrer;
        public ICommand CmdEnregistrer
        {
            get
            {
                if (_cmdEnregistrer == null)
                    _cmdEnregistrer = new RelayCommand(EnregistrerTaches);
                return _cmdEnregistrer;
            }
        }
        private ICommand _cmdAnnuler;
        public ICommand CmdAnnuler
        {
            get
            {
                if (_cmdAnnuler == null)
                    _cmdAnnuler = new RelayCommand(AnnulerTaches);
                return _cmdAnnuler;
            }
        }
        #endregion

        #region Méthodes privées
        private void AjouterTache(object o)
        {
            //Gestion de la validation des saisies
            var Duree = DureePrevue.ToString();
            var DureeRest = DureeRestante.ToString();
            string pattern = @"^\d{1,4}[\,]\d{1}";
             Regex rgx = new Regex(pattern);


            if (string.IsNullOrEmpty(Libelle))
            {

                MessageBox.Show("Le libellé de la tâche est obligatoire!", "Attention", MessageBoxButton.OK);
            }

            else if (!rgx.IsMatch(Duree))
            {

                MessageBox.Show("La durée prévue doit être saisie __._!", "Attention", MessageBoxButton.OK);
            }

            else if (DureeRestante!=0 && !rgx.IsMatch(DureeRest))
            {
                MessageBox.Show("La durée restante doit être saisie __._!", "Attention", MessageBoxButton.OK);
            }

            else if (DureePrevue==0)
            {
                MessageBox.Show("La durée prévue est obligatoire!", "Attention", MessageBoxButton.OK);
            }
            else
            {

                _moduleCourant = (Module)CollectionViewSource.GetDefaultView(Modules).CurrentItem;
                _personneCourante = (Personne)CollectionViewSource.GetDefaultView(Personnes).CurrentItem;
                _activiteCourante = (Activite)CollectionViewSource.GetDefaultView(_personneCourante.Activites).CurrentItem;

                Personne personne = PersonnesTachesProdAjoutees.Where(x => x.CodePersonne == _personneCourante.CodePersonne).FirstOrDefault();
                if (personne == null)
                {
                    personne = new Entity.Personne();
                    personne.TachesProd = new ObservableCollection<TacheProd>();
                }
            personne.CodePersonne = _personneCourante.CodePersonne;

            TacheProd tache = new TacheProd();
            tache.IdTache = new Guid();
            tache.NomTache = Libelle;
            tache.Annexe = false;
            tache.CodeActivite = _activiteCourante.CodeActivite;
            tache.Description = Description;
            tache.CodeModule = _moduleCourant.CodeModule;
            tache.CodeLogiciel = _logicielCourant.CodeLogiciel;
            tache.CodeVersion = _versionCourante.NumVersion;
                    tache.DureePrevue = DureePrevue;
                if(DureePrevue==0)
                    tache.DureeRestante = DureePrevue;
                else
                        tache.DureeRestante = DureeRestante;
                personne.TachesProd.Add(tache);

                PersonnesTachesProdAjoutees.Add(personne);
                TachesProdAjoutees.Add(tache);
            }
        }
        private void SupprimerTache(object o)
        {
            Personne pers= (Personne)CollectionViewSource.GetDefaultView(Personnes).CurrentItem;
            PersonnesTachesProdAjoutees.Remove(pers);
        }
        private void EnregistrerTaches(object o) { }
        private void AnnulerTaches(object o)
        {
            PersonnesTachesProdAjoutees.RemoveAt(PersonnesTachesProdAjoutees.Count - 1);
        }

        #endregion
    }
}
