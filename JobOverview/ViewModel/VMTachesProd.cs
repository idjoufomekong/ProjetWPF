using JobOverview.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        #endregion

        public VMTachesProd(ObservableCollection<Logiciel> logicielVMMain, ObservableCollection<Personne> persTacheProdVMMain)
        {
            Logiciels = logicielVMMain;
            PersonnesTachesProd = persTacheProdVMMain;
            _userCourant = Properties.Settings.Default.CodeDernierUtilisateur;
        }
    }
}
