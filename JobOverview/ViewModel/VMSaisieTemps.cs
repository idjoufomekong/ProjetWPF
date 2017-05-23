using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobOverview.Entity;
using JobOverview.Model;
using JobOverview.View;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Data;
using System.Text.RegularExpressions;

namespace JobOverview.ViewModel
{
    public class VMSaisieTemps : ViewModelBase
    {
        public ObservableCollection<Logiciel> Logiciels { get; set; }
        public ObservableCollection<TacheApercu> TachesProd { get; set; } =
        new ObservableCollection<TacheApercu>();
        public ObservableCollection<TacheApercu> TachesAnnexe { get; set; } =
        new ObservableCollection<TacheApercu>();
        public Personne Utilisateur { get; set; }
        public TacheApercu TacheCourante {
            get
            {
                ICollectionView current = CollectionViewSource.GetDefaultView(TachesProd);
                return (TacheApercu)current.CurrentItem;
            }
        }
        public bool Choice { get; set; }
        private float _heuresRestante;
        public float HeuresRestantes {
            get { return _heuresRestante; }
            set {  SetProperty(ref _heuresRestante, value); }
        }

        private ICommand _cmdValider;
        public ICommand CmdValider
        {
            get
            {
                if (_cmdValider == null)
                    _cmdValider = new RelayCommand(AfficherTaches,Activation);
                return _cmdValider;
            }
        }

        private ICommand _cmdTriTachProd;
        public ICommand CmdTriTachProd
        {
            get
            {
                if (_cmdTriTachProd == null)
                    _cmdTriTachProd = new RelayCommand(TrierTaches, Activation);
                return _cmdTriTachProd;
            }
        }

        public VMSaisieTemps(ObservableCollection<Logiciel> LogicielsVMMain)
        {
            Utilisateur = (DALTache.RecupererPersonnesTaches(
               Properties.Settings.Default.CodeDernierUtilisateur)).FirstOrDefault();
            Logiciels = LogicielsVMMain;
        }

        private void AfficherTaches (object obj)
        {
            var selecLog = (Logiciel)CollectionViewSource.GetDefaultView(Logiciels).CurrentItem;
            var selecVer = (Entity.Version)CollectionViewSource.GetDefaultView(selecLog.Versions).CurrentItem;

            TachesProd.Clear();
            TachesAnnexe.Clear();

            HeuresRestantes = 8;

            if (Utilisateur.TachesProd != null)
            {
                var triProd = Utilisateur.TachesProd.
                    Where(tp => tp.CodeLogiciel == selecLog.CodeLogiciel &&
                    tp.CodeVersion == selecVer.NumVersion).ToList();
                if (triProd != null)
                {
                    foreach (var tp in triProd)
                    {
                        if (tp.TravauxProd != null)
                        {
                            foreach (var trP in tp.TravauxProd)
                            {
                                if (trP.Date == DateTime.Today)
                                {
                                    HeuresRestantes -= trP.Heures;
                                }
                                var tach = new TacheApercu();
                                tach.Date = trP.Date;
                                tach.DureePrevue = tp.DureePrevue;
                                tach.DureeRestante = tp.DureeRestante;
                                tach.NomTache = tp.NomTache;
                                tach.IdTache = tp.IdTache;
                                tach.Heures = trP.Heures;
                                TachesProd.Add(tach);
                                break;
                            }
                        }
                    }
                }
            }

            if (Utilisateur.TachesAnnexes != null)
            {
                var triAnnex = Utilisateur.TachesAnnexes.ToList();

                if (triAnnex != null)
                {
                    foreach (var ta in triAnnex)
                    {
                        if (ta.TravauxAnnexes != null)
                        {
                            foreach (var trA in ta.TravauxAnnexes)
                            {
                                if (trA.Date == DateTime.Today)
                                {
                                    HeuresRestantes -= trA.Heures;
                                }
                                    var tach = new TacheApercu();
                                    tach.NomTache = ta.NomTache;
                                    tach.IdTache = ta.IdTache;
                                    tach.CodeActivite = ta.CodeActivite;
                                    tach.Date = trA.Date;
                                    tach.Heures = trA.Heures;
                                    TachesAnnexe.Add(tach);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private bool Activation(object obj)
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

        private void TrierTaches (object radioTag)
        {
            
            AfficherTaches(new object());
            ICollectionView triage = CollectionViewSource.GetDefaultView(TachesProd);
            var choix = radioTag.ToString();
            switch (choix)
            {
                case "F":
                    triage.Filter = FiltreTerminee;
                    break;
                case "EC":
                    triage.Filter = FiltreEnCours; 
                    break;
                case "T":
                    triage.Filter = null;
                    break;
                default:
                    break;
            }
        }

        private bool FiltreTerminee(object o)
        {
            return ((TacheApercu)o).DureeRestante == 0;
        }

        private bool FiltreEnCours(object o)
        {
            return ((TacheApercu)o).DureeRestante > 0;
        }

    }
}
