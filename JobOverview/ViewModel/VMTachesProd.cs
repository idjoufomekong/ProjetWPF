using JobOverview.Entity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JobOverview.ViewModel
{
    public class VMTachesProd : ViewModelBase
    {
        //Champs privés
        private Logiciel _logicielCourant;
        private Entity.Version _version;
        private string _userCourant;
        #region Propriétés

        public List<Personne> PersonnesTaches { get; private set; }
        public ObservableCollection<Personne> PersonnesTachesProd { get; private set; }
        public ObservableCollection<Logiciel> Logiciels { get; private set; }
        public ObservableCollection<Entity.Version> Versions { get; set; }
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
        private string _nomLogiciel;
        public string NomLogiciel
        {
            get
            {
                return _nomLogiciel;
            }
            set
            {
                _nomLogiciel = value;
                AfficherTacheProd();
            }
        }

        private float _numVersion;
        public float NumVersion
        {
            get
            {
                return _numVersion;
            }
            set
            {
                _numVersion = value;
                AfficherTacheProd();
            }
        }
        #endregion

        public VMTachesProd(ObservableCollection<Logiciel> logicielVMMain, ObservableCollection<Personne> persTacheProdVMMain)
        {
            Logiciels = logicielVMMain;
            PersonnesTachesProd = persTacheProdVMMain;
            _userCourant = Properties.Settings.Default.CodeDernierUtilisateur;
        }

        private void AfficherTacheProd()
        {
            //PersonnesTaches = DALTache.RecupererPersonnesTaches(LogicielCourant.CodeLogiciel,
            //   VersionCourante.NumVersion, _userCourant);
            //DALEchange.ExporterXML(PersonnesTaches);

            //Récupération de la liste et sélection des tâches en fonction de la version
            //var listTache = DALTache.RecupererPersonnesTaches(LogicielCourant.CodeLogiciel,
            //    VersionCourante.NumVersion, _userCourant);
            //var listCourante = new List<Personne>();
            foreach (var b in PersonnesTachesProd)
            {
                if (b.TachesProd != null)
                {

                var p = b.TachesProd.Where(x => (x.CodeVersion == VersionCourante.NumVersion)
                && (x.CodeLogiciel == LogicielCourant.CodeLogiciel));
                if (p!=null)
                b.TachesProd = p.ToList();
                }
            }
        }
    }
}
