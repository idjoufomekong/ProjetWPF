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

namespace JobOverview.ViewModel
{
    public class VMSaisieTemps : ViewModelBase
    {
        public ObservableCollection<Logiciel> Logiciels { get; set; }
        public ObservableCollection<TacheProd> TachesProd { get; set; } =
        new ObservableCollection<TacheProd>();
        public ObservableCollection<Tache> TachesAnnexe { get; set; } =
        new ObservableCollection<Tache>();
        public Personne Utilisateur { get; set; }
        public int HeuresRestantes { get; set; }

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
            var selecDate = (DateTime)obj;

            TachesProd.Clear();
            TachesAnnexe.Clear();

            HeuresRestantes = 8;

            if (Utilisateur.TachesProd != null)
            {
                var triProd = Utilisateur.TachesProd.
                    Where(tp => tp.CodeLogiciel == selecLog.CodeLogiciel &&
                    tp.CodeVersion == selecVer.NumVersion).ToList();
                var tot = triProd.Sum(c => c.TravauxProd.Count);
                if (triProd != null)
                {
                    foreach (var tp in triProd)
                    {
                        if (tp.TravauxProd != null)
                        {
                            foreach (var trP in tp.TravauxProd)
                            {
                                if (trP.Date == selecDate)
                                {
                                    TachesProd.Add(tp);
                                    break;
                                }
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
                                if (trA.Date == selecDate)
                                {
                                    TachesAnnexe.Add(ta);
                                    break;
                                }
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
    }
}
