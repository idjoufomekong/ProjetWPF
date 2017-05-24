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
    public enum ModesEdition { Consultation, Edition };
    public class VMSaisieTache:ViewModelBase
    {

        //Champs privés
        private Logiciel _logicielCourant;
        private Entity.Version _versionCourante;
        private Activite _activiteCourante;
        private Module _moduleCourant;
        private Personne _personneCourante;
        private TacheApercu _nouvelleTache;
        private string _libelle;
        private string _dureePrevue;
        private string _dureeRestante;
        private ModesEdition _modeEdition;

        #region Propriétés
        public List<Personne> Personnes { get; set; }
        public List<Module> Modules { get; set; }
        public ObservableCollection<Activite> Activites { get; set; }
        public ObservableCollection<Personne> PersonnesTachesProdAjoutees { get; set; }
        public ObservableCollection<TacheApercu> TachesProdAjoutees { get; set; }
        public string Description { get; set; }
        public TacheApercu NouvelleTache
        {
            get { return _nouvelleTache; }
            private set
            {
                SetProperty(ref _nouvelleTache, value);
            }
        }
        public string DureePrevue
        {
            get { return _dureePrevue; }
            set
            {
                SetProperty(ref _dureePrevue, value);
            }
        }
        public string DureeRestante
        {
            get { return _dureeRestante; }
            set
            {
                SetProperty(ref _dureeRestante, value);
            }
        }
        public string Libelle
        {
            get { return _libelle; }
            set
            {
                SetProperty(ref _libelle, value);
            }
        }
        public ModesEdition ModeEdit
        {
            get { return _modeEdition; }
            private set
            {
                SetProperty(ref _modeEdition, value);
            }
        }
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
            TachesProdAjoutees = new ObservableCollection<TacheApercu>();
            NouvelleTache = new TacheApercu();
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
        //private ICommand _cmdEnregistrer;
        //public ICommand CmdEnregistrer
        //{
        //    get
        //    {
        //        if (_cmdEnregistrer == null)
        //            _cmdEnregistrer = new RelayCommand(ValiderSaisie, TesterConsultation);
        //        return _cmdEnregistrer;
        //    }
        //}
        #endregion

        #region Méthodes privées
        private void AjouterTache(object o)
        {
            //Gestion de la validation des saisies
            string pattern = @"^\d{1,4}[\.]?\d?$";
             Regex rgx = new Regex(pattern);


            if (string.IsNullOrEmpty(Libelle))
            {

                MessageBox.Show("Le libellé de la tâche est obligatoire!", "Attention", MessageBoxButton.OK);
            }

            else if (string.IsNullOrEmpty(DureePrevue))
            {
                MessageBox.Show("La durée prévue est obligatoire!", "Attention", MessageBoxButton.OK);
            }
            else if (!rgx.IsMatch(DureePrevue))
            {

                MessageBox.Show("La durée prévue doit être saisie __._!", "Attention", MessageBoxButton.OK);
            }

            else if (!string.IsNullOrEmpty(DureeRestante) && !rgx.IsMatch(DureeRestante))
            {
                MessageBox.Show("La durée restante doit être saisie __._!", "Attention", MessageBoxButton.OK);
            }

            else
            {

                _moduleCourant = (Module)CollectionViewSource.GetDefaultView(Modules).CurrentItem;
                _personneCourante = (Personne)CollectionViewSource.GetDefaultView(Personnes).CurrentItem;
                _activiteCourante = (Activite)CollectionViewSource.GetDefaultView(_personneCourante.Activites).CurrentItem;

                
                TacheApercu tache = new TacheApercu();
            tache.Login = _personneCourante.CodePersonne;

            tache.IdTache = new Guid();
            tache.NomTache = Libelle;
            tache.Annexe = false;
            tache.CodeActivite = _activiteCourante.CodeActivite;
            tache.Description = NouvelleTache.Description;
            tache.CodeModule = _moduleCourant.CodeModule;
            tache.CodeLogiciel = _logicielCourant.CodeLogiciel;
            tache.CodeVersion = _versionCourante.NumVersion;
               DureePrevue= DureePrevue.Replace(".", ",");
                float i;
                    if(float.TryParse(DureePrevue, out i))
                tache.DureePrevue = i;
                if (string.IsNullOrEmpty( DureeRestante))
                    tache.DureeRestante = i;
                else
                {
                    DureeRestante = DureeRestante.Replace(".", ",");
                    float j;
                    if (float.TryParse(DureeRestante, out j))
                        tache.DureeRestante = j;
                }

                TachesProdAjoutees.Add(tache);
            }
            NouvelleTache = new TacheApercu();
            Libelle = null;
            DureePrevue = null;
            DureeRestante = null;
        }
        private void SupprimerTache(object o)
        {
            TacheApercu tache= (TacheApercu)CollectionViewSource.GetDefaultView(TachesProdAjoutees).CurrentItem;
            TachesProdAjoutees.Remove(tache);
        }

        private bool TesterEdition(object o)
        {
            return ModeEdit == ModesEdition.Consultation;
        }

        private bool TesterConsultation(object o)
        {
            return ModeEdit == ModesEdition.Edition;
        }

        public override ValidationResult Validate()
        {
            //Enregistrer
            if (TachesProdAjoutees.Count ==0)
                return new ValidationResult(true);
            else
            {
            DALTache.EnregistrerTachesProd(TachesProdAjoutees.ToList());
            return new ValidationResult(true);
            }
        }

        #endregion
    }
}
