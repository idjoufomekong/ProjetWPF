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
                AffichTacheProd(NumVersion, DateSelectionnee, value);
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
                AffichTacheProd(value, DateSelectionnee, NomLogiciel);
            }
        }

        private DateTime _dateSelectionee = new DateTime(2016, 01, 01);
        public DateTime DateSelectionnee
        {
            get
            {
                return _dateSelectionee;
            }
            set
            {
                _dateSelectionee = value;
                AffichTacheProd(NumVersion, value, NomLogiciel);
                AffichTacheAnnexe(value);
            }
        }


        public VMSaisieTemps(ObservableCollection<Logiciel> LogicielsVMMain)
        {
            Utilisateur = (DALTache.RecupererPersonnesTaches(
               Properties.Settings.Default.CodeDernierUtilisateur)).FirstOrDefault();
            Logiciels = LogicielsVMMain;
        }

        private void AffichTacheProd(float numVersion, DateTime dateSelectionnee, string nomLogiciel)
        {
            TachesProd.Clear();
            if (Utilisateur.TachesProd != null)
            {
                var p = (Utilisateur.TachesProd.Where(tp => tp.CodeLogiciel == nomLogiciel && tp.CodeVersion == numVersion)).ToList();
                foreach (var t in p)
                {
                    foreach (Travail tr in t.TravauxProd)
                    {
                        if (tr.Date == dateSelectionnee)
                        {
                            TachesProd.Add(t);
                            break;
                        }
                    }
                }
            }
        }

        private void AffichTacheAnnexe(DateTime dateSelectionnee)
        {
            TachesAnnexe.Clear();
            if (Utilisateur.TachesAnnexes != null)
            {
                foreach (var t in Utilisateur.TachesAnnexes)
                {
                    if (t.TravauxAnnexes != null)
                    {
                        foreach (Travail tr in t.TravauxAnnexes)
                        {
                            if (tr.Date == dateSelectionnee)
                            {
                                TachesAnnexe.Add(t);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
