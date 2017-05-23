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
        private Entity.Version _version;
        private string _userCourant;
        private bool _enCours;
        private bool _termine;
        private string _selection;
        private ObservableCollection<Personne> _listPersTachesProd;
        private Personne _personneCourante;
        #region Propriétés

        public List<Personne> PersonnesTaches { get; private set; }
        public ObservableCollection<Personne> PersonnesTachesProd { get; private set; }
        public ObservableCollection<Logiciel> Logiciels { get; private set; }
        public ObservableCollection<Entity.Version> Versions { get; set; }
        public bool EnCours
        {
            get { return _enCours; }
            set { SetProperty(ref _enCours, value); }
        }
        public string Selection
        {
            get { return _selection; }
            set {  _selection= value;
                Trier();
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
            get { return _version; }
            set { SetProperty(ref _version, value); }
        }

        public Personne PersonneCourante
        {
            get
            {
                return (Personne)CollectionViewSource.GetDefaultView(PersonnesTachesProd).CurrentItem;
            }
        }
        //Permettet de gérer l'affichage directement avec les listes
        //private string _nomLogiciel;
        //public string NomLogiciel
        //{
        //    get
        //    {
        //        return _nomLogiciel;
        //    }
        //    set
        //    {
        //        _nomLogiciel = value;
        //         Afficher();
        //    }
        //}

        //private float _numVersion;
        //public float NumVersion
        //{
        //    get
        //    {
        //        return _numVersion;
        //    }
        //    set
        //    {
        //        _numVersion = value;
        //         Afficher();
        //    }
        //}
        #endregion

        public VMTachesProd(ObservableCollection<Logiciel> logicielVMMain, ObservableCollection<Personne> persTacheProdVMMain)
        {
            Logiciels = logicielVMMain;
            _listPersTachesProd = persTacheProdVMMain;
            _userCourant = Properties.Settings.Default.CodeDernierUtilisateur;
            PersonnesTachesProd = new ObservableCollection<Personne>();
            EnCours = true;
            Termine = false;
        }

        #region Commandes
        private ICommand _cmdAfficher;
        public ICommand CmdAfficher
        {
            get
            {
                if (_cmdAfficher == null)
                    _cmdAfficher = new RelayCommand(Afficher);
                return _cmdAfficher;
            }
        }

        private ICommand _cmdChecker;
        public ICommand CmdChecker
        {
            get
            {
                if (_cmdChecker == null)
                    _cmdChecker = new RelayCommand(TrierTachesTerminees);
                return _cmdChecker;
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

        private ICommand _cmdAjoutTachesProd;
        public ICommand CmdAjoutTachesProd
        {
            get
            {
                if (_cmdAjoutTachesProd == null)
                    _cmdAjoutTachesProd = new RelayCommand(AjouterTaches);
                return _cmdAjoutTachesProd;
            }
        }
        #endregion

        #region Méthodes privées
        //Charge le datacontext en fonction de la personne sélectionnée
        private void Afficher(object obj)
        {
            //PersonnesTaches = DALTache.RecupererPersonnesTaches(LogicielCourant.CodeLogiciel,
            //   VersionCourante.NumVersion, _userCourant);
            //DALEchange.ExporterXML(PersonnesTaches);

            //Récupération de la liste et sélection des tâches en fonction de la version
            //var listTache = DALTache.RecupererPersonnesTaches(LogicielCourant.CodeLogiciel,
            //    VersionCourante.NumVersion, _userCourant);
            //var listCourante = new List<Personne>();
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

        }

        //Suppression de la tâche sélectionnée
        private void SupprimerTache()
        {
            TacheProd t= (TacheProd)CollectionViewSource.GetDefaultView(PersonneCourante.TachesProd).CurrentItem;
            try
            {
            DALTache.SupprimerTacheProd(t.IdTache);

            }
            catch (SqlException e)
            {
                if(e.Number==547)
                MessageBox.Show("Cette Tâche contient des heures de travail. Il n'est pas possible de la supprimer."
                    + "\n" + "", "Attention", MessageBoxButton.OK);
                else
                    MessageBox.Show(e.Message + "\n" + "", "Attention", MessageBoxButton.OK);
            }
        }

        //Gère l'affichage des tâches en cours
        private void TrierTachesTerminees(object obj)
        {
            _personneCourante = (Personne)CollectionViewSource.GetDefaultView(PersonnesTachesProd).CurrentItem;
            CollectionView vue = (CollectionView)CollectionViewSource.GetDefaultView(PersonnesTachesProd);
            PersonnesTachesProd.Clear();
            LogicielCourant = (Logiciel)CollectionViewSource.GetDefaultView(Logiciels).CurrentItem;
            VersionCourante = (Entity.Version)CollectionViewSource.GetDefaultView(LogicielCourant.Versions).CurrentItem;
            foreach (var a in _listPersTachesProd)
            {
                PersonnesTachesProd.Add(a);
            }
            vue.MoveCurrentTo(_personneCourante);
            var b = PersonnesTachesProd.Where(x => x.CodePersonne == _personneCourante.CodePersonne).FirstOrDefault();

            if (b.TachesProd != null)
            {
                var p = b.TachesProd.Where(x => (x.CodeVersion == VersionCourante.NumVersion)
                && (x.CodeLogiciel == LogicielCourant.CodeLogiciel));
                if (p != null)
                    b.TachesProd = new ObservableCollection<Entity.TacheProd>(p.ToList());
            }
            if (EnCours && Termine)//Les 2 checkbox sont cochées, on affiche toutes les tâches de la personne
            {
                vue.MoveCurrentTo(_personneCourante);
            }
            else if (EnCours)//La checkbox Encours est cochée seule, on affiche les tâches en cours de la personne
            {
                if (b.TachesProd != null)
                    CollectionViewSource.GetDefaultView(b.TachesProd).Filter = FiltrerEncours;
                vue.MoveCurrentTo(_personneCourante);
            }
            else if (Termine)//La checkbox Termine est cochée seule, on affiche les tâches terminées de la personne
            {
                if (b.TachesProd != null)
                    CollectionViewSource.GetDefaultView(b.TachesProd).Filter = FiltrerTermine;
                vue.MoveCurrentTo(_personneCourante);
            }
            else //Aucune checkbox n'est cochée, on affiche toutes les tâches de la personne
            {
                if (b.TachesProd != null)
                {
                    b.TachesProd.Clear();
                }
            }
        }



        private void Trier()
        {
            _personneCourante = (Personne)CollectionViewSource.GetDefaultView(PersonnesTachesProd).CurrentItem;
            CollectionView vue = (CollectionView)CollectionViewSource.GetDefaultView(PersonnesTachesProd);
            PersonnesTachesProd.Clear();
            LogicielCourant = (Logiciel)CollectionViewSource.GetDefaultView(Logiciels).CurrentItem;
            VersionCourante = (Entity.Version)CollectionViewSource.GetDefaultView(LogicielCourant.Versions).CurrentItem;
            foreach (var a in _listPersTachesProd)
            {
                PersonnesTachesProd.Add(a);
            }
            vue.MoveCurrentTo(_personneCourante);
            var b = PersonnesTachesProd.Where(x => x.CodePersonne == _personneCourante.CodePersonne).FirstOrDefault();

            if (b.TachesProd != null)
            {

                var p = b.TachesProd.Where(x => (x.CodeVersion == VersionCourante.NumVersion)
                && (x.CodeLogiciel == LogicielCourant.CodeLogiciel));
                if (p != null)
                    b.TachesProd = new ObservableCollection<Entity.TacheProd>(p.ToList());
            }
            if (Selection== " En cours") {
                if (b.TachesProd != null)
                {
                    CollectionViewSource.GetDefaultView(b.TachesProd).Filter = FiltrerEncours;
                }
            }
            else if (Selection == "Terminées")
            {
                if (b.TachesProd != null)
                    CollectionViewSource.GetDefaultView(b.TachesProd).Filter = FiltrerTermine;
            }
            else { }
        }

        private bool FiltrerEncours(object o)
        {
            return ((TacheProd)o).DureeRestante > 0;
        }

        private bool FiltrerTermine(object o)
        {
            return ((TacheProd)o).DureeRestante == 0;
        }

        private void AjouterTaches()
        {
            var dlg = new ModalWindow(new VMSaisieTache());
            dlg.Title = "Saisie des tâches";
            dlg.Height = 800; 
            dlg.Width = 600;
            bool? res = dlg.ShowDialog();
        }
        #endregion
    }
}
