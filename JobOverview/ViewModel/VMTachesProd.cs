using JobOverview.Entity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Data;
using System;
using JobOverview.Model;
using System.Windows;
using System.Data.SqlClient;
using JobOverview.View;

namespace JobOverview.ViewModel
{
    public class VMTachesProd : ViewModelBase
    {
        //Champs privés
        private Logiciel _logicielCourant;
        private Entity.Version _versionCourante;
        private string _userCourant;
        private bool _termine;
        private ObservableCollection<Personne> _listPersTachesProd;
        private Personne _personneCourante;
        private CollectionView vue;
        private ModesEdition _modeEdition;
        #region Propriétés

        public List<Personne> PersonnesTaches { get; private set; }
        public ObservableCollection<Personne> PersonnesTachesProd { get; private set; }
        public ObservableCollection<Logiciel> Logiciels { get; private set; }
        public ObservableCollection<Entity.Version> Versions { get; set; }
        public ModesEdition ModeEdit
        {
            get { return _modeEdition; }
            private set
            {
                SetProperty(ref _modeEdition, value);
            }
        }

        public bool Termine
        {
            get { return _termine; }
            set { SetProperty(ref _termine, value); }
        }
        public Logiciel LogicielCourant
        {
            get { return _logicielCourant; }
            set { SetProperty(ref _logicielCourant, value); }
        }
        public Entity.Version VersionCourante
        {
            get { return _versionCourante; }
            set { SetProperty(ref _versionCourante, value); }
        }

        public Personne PersonneCourante
        {
            get
            {
                return (Personne)CollectionViewSource.GetDefaultView(PersonnesTachesProd).CurrentItem;
            }
        }
        #endregion

        public VMTachesProd(ObservableCollection<Logiciel> logicielVMMain, ObservableCollection<Personne> persTacheProdVMMain)
        {
            Logiciels = logicielVMMain;
            _listPersTachesProd = persTacheProdVMMain;
            _userCourant = Properties.Settings.Default.CodeDernierUtilisateur;
            PersonnesTachesProd = new ObservableCollection<Personne>();
        }

        #region Commandes
        private ICommand _cmdAfficher;
        public ICommand CmdAfficher
        {
            get
            {
                if (_cmdAfficher == null)
                    _cmdAfficher = new RelayCommand(Afficher, Validation);
                return _cmdAfficher;
            }
        }

        private ICommand _cmdChecker;
        public ICommand CmdChecker
        {
            get
            {
                if (_cmdChecker == null)
                    _cmdChecker = new RelayCommand(TrierTaches);
                return _cmdChecker;
            }
        }

        private ICommand _cmdSupprimer;
        public ICommand CmdSupprimer
        {
            get
            {
                if (_cmdSupprimer == null)
                    _cmdSupprimer = new RelayCommand(SupprimerTache, Validation);
                return _cmdSupprimer;
            }
        }

        private ICommand _cmdAjoutTachesProd;
        public ICommand CmdAjoutTachesProd
        {
            get
            {
                if (_cmdAjoutTachesProd == null)
                    _cmdAjoutTachesProd = new RelayCommand(AjouterTaches, Validation);
                return _cmdAjoutTachesProd;
            }
        }
        #endregion

        #region Méthodes privées
        //Charge le datacontext en fonction de la personne sélectionnée
        private void Afficher(object obj)
        {
            PersonnesTachesProd.Clear();
            LogicielCourant = (Logiciel)CollectionViewSource.GetDefaultView(Logiciels).CurrentItem;
            VersionCourante = (Entity.Version)CollectionViewSource.GetDefaultView(LogicielCourant.Versions).CurrentItem;

            //Chargement du datacontext

            foreach (var a in _listPersTachesProd)
            {
                PersonnesTachesProd.Add(a);
            }

            //Sélection des tâches en fonction de la version et du logiciel
            foreach (var b in PersonnesTachesProd)
            {
                if (b.TachesProd != null)
                {

                    var p = b.TachesProd.Where(x => (x.CodeVersion == VersionCourante.NumVersion)
                    && (x.CodeLogiciel == LogicielCourant.CodeLogiciel));
                    if (p != null)
                        b.TachesProd = new ObservableCollection<Entity.TacheProd>(p.ToList());

                    CollectionViewSource.GetDefaultView(b.TachesProd).Filter = FiltrerEncours;
                }
            }
            ModeEdit = ModesEdition.Edition;
        }

        //Suppression de la tâche sélectionnée
        private void SupprimerTache(object o)
        {
            if (PersonneCourante == null) return;
            TacheProd t = (TacheProd)CollectionViewSource.GetDefaultView(PersonneCourante.TachesProd).CurrentItem;
            try
            {
                if (t == null) return;
                DALTache.SupprimerTacheProd(t.IdTache);

            }
            catch (SqlException e)
            {
                if (e.Number == 547)
                    MessageBox.Show("Cette Tâche contient des heures de travail. Il n'est pas possible de la supprimer."
                        + "\n" + "", "Attention", MessageBoxButton.OK);
                else
                    MessageBox.Show(e.Message + "\n" + "", "Attention", MessageBoxButton.OK);
            }
        }

        //Trie les tâches en cours
        private void TrierTaches(object radioTag)
        {
            _personneCourante = (Personne)CollectionViewSource.GetDefaultView(PersonnesTachesProd).CurrentItem;
            LogicielCourant = (Logiciel)CollectionViewSource.GetDefaultView(Logiciels).CurrentItem;
            VersionCourante = (Entity.Version)CollectionViewSource.GetDefaultView(LogicielCourant.Versions).CurrentItem;

            if (_personneCourante == null) return;
            var b = PersonnesTachesProd.Where(x => x.CodePersonne == _personneCourante.CodePersonne).FirstOrDefault();
            vue = (CollectionView)CollectionViewSource.GetDefaultView(b.TachesProd);

            var choice = radioTag.ToString();
            switch (choice)
            {
                case "F":
                    vue.Filter = FiltrerTermine;
                    break;
                case "EC":
                    vue.Filter = FiltrerEncours;
                    break;
                case "T":
                    vue.Filter = null;
                    break;
                default:
                    break;
            }

        }

        private bool FiltrerEncours(object o)
        {
            return ((TacheProd)o).DureeRestante > 0;
        }

        private bool FiltrerTermine(object o)
        {
            return ((TacheProd)o).DureeRestante == 0;
        }

        private void AjouterTaches(object o)
        {
            LogicielCourant = (Logiciel)CollectionViewSource.GetDefaultView(Logiciels).CurrentItem;
            VersionCourante = (Entity.Version)CollectionViewSource.GetDefaultView(LogicielCourant.Versions).CurrentItem;
            var dlg = new ModalWindow(new VMSaisieTache(LogicielCourant.CodeLogiciel, VersionCourante.NumVersion));
            dlg.Title = "saisie des tâches";
            bool? res = dlg.ShowDialog();

        }

        private bool Validation(object o)
        {
            var selecLog = (Logiciel)CollectionViewSource.GetDefaultView(Logiciels).CurrentItem;
            if (selecLog != null)
            {
                var selecVer = (Entity.Version)CollectionViewSource.GetDefaultView(selecLog.Versions).CurrentItem;
                if (selecVer != null)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
