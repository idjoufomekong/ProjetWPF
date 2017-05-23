using JobOverview.Entity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Data;

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
        #endregion

        #region Méthodes privées
        //Charge le datacontext en fonction de la personne sélectionnée
        private void Afficher()
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
             
            foreach(var a in _listPersTachesProd)
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
                if (p!=null)
                b.TachesProd = new ObservableCollection<Entity.TacheProd>(p.ToList());
                }
            }
        }

        //Gère l'affichage des tâches en cours
        private void TrierTachesTerminees()
        {
            _personneCourante= (Personne)CollectionViewSource.GetDefaultView(PersonnesTachesProd).CurrentItem;
            CollectionView vue = (CollectionView)CollectionViewSource.GetDefaultView(PersonnesTachesProd);
            PersonnesTachesProd.Clear();
            LogicielCourant = (Logiciel)CollectionViewSource.GetDefaultView(Logiciels).CurrentItem;
            VersionCourante = (Entity.Version)CollectionViewSource.GetDefaultView(LogicielCourant.Versions).CurrentItem;
            foreach (var a in _listPersTachesProd)
            {
                PersonnesTachesProd.Add(a);
            }
            vue.MoveCurrentTo(_personneCourante);

            if (EnCours && Termine)//Les 2 checkbox sont cochées, on affiche toutes les tâches de la personne
            {
                //foreach (var a in _listPersTachesProd)
                //{
                //    PersonnesTachesProd.Add(a);
                //}
                //Sélection des tâches en fonction de la version et du logiciel
                //foreach (var b in PersonnesTachesProd)
                //{
                    var b = PersonnesTachesProd.Where(x => x.CodePersonne == _personneCourante.CodePersonne).FirstOrDefault();
                    if (b.TachesProd != null)
                    {

                        var p = b.TachesProd.Where(x => (x.CodeVersion == VersionCourante.NumVersion)
                        && (x.CodeLogiciel == LogicielCourant.CodeLogiciel));
                        if (p != null)
                        b.TachesProd = new ObservableCollection<Entity.TacheProd>(p.ToList());
                }
               // }
            }
            else if(EnCours)//La checkbox Encours est cochée seule, on affiche les tâches en cours de la personne
            {
                //foreach (var a in _listPersTachesProd)
                //{
                //    PersonnesTachesProd.Add(a);
                //}
                //Sélection des tâches en fonction de la version et du logiciel
                //foreach (var b in PersonnesTachesProd)
                //{
                var b = PersonnesTachesProd.Where(x => x.CodePersonne == _personneCourante.CodePersonne).FirstOrDefault();
                if (b.TachesProd != null)
                    {

                        var p = b.TachesProd.Where(x => (x.CodeVersion == VersionCourante.NumVersion)
                        && (x.CodeLogiciel == LogicielCourant.CodeLogiciel) && (x.DureeRestante > 0));
                        if (p != null)
                        b.TachesProd = new ObservableCollection<Entity.TacheProd>(p.ToList());
                }
               // }
            }
            else if (Termine)//La checkbox Termine est cochée seule, on affiche les tâches terminées de la personne
            {
                //foreach (var a in _listPersTachesProd)
                //{
                //    PersonnesTachesProd.Add(a);
                //}
                //Sélection des tâches en fonction de la version et du logiciel
                //foreach (var b in PersonnesTachesProd)
                //{
                var b = PersonnesTachesProd.Where(x => x.CodePersonne == _personneCourante.CodePersonne).FirstOrDefault();
                if (b.TachesProd != null)
                    {

                        var p = b.TachesProd.Where(x => (x.CodeVersion == VersionCourante.NumVersion)
                        && (x.CodeLogiciel == LogicielCourant.CodeLogiciel) && (x.DureeRestante == 0));
                        if (p != null)
                            b.TachesProd = new ObservableCollection<Entity.TacheProd>(p.ToList());
                    }
               // }
            }
            else //Aucune checkbox n'est cochée, on affiche toutes les tâches de la personne
            {
                //foreach (var a in _listPersTachesProd)
                //{
                //    PersonnesTachesProd.Add(a);
                //}
                //Sélection des tâches en fonction de la version et du logiciel
                //foreach (var b in PersonnesTachesProd)
                //{
                var b = PersonnesTachesProd.Where(x => x.CodePersonne == _personneCourante.CodePersonne).FirstOrDefault();
                if (b.TachesProd != null)
                    {

                        var p = b.TachesProd.Where(x => (x.CodeVersion == VersionCourante.NumVersion)
                        && (x.CodeLogiciel == LogicielCourant.CodeLogiciel));
                        if (p != null)
                        b.TachesProd = new ObservableCollection<Entity.TacheProd>(p.ToList());
                }
               // }
            }
        }
        #endregion
    }
}
