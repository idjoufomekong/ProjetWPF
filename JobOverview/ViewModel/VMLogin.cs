using JobOverview.Entity;
using JobOverview.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace JobOverview.ViewModel
{
    public class VMLogin : ViewModelBase
    {

        public List<Personne> Personnes { get; set; }

        public VMLogin()
        {
            Personnes = DALPersonne.RecupererToutesPersonne();
        }

        public override ValidationResult Validate()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Personnes);

            //Enregistrement de l'identifiant de la personne connectée dans les paramètres de l'appli (portée User)
            Properties.Settings.Default.CodeDernierUtilisateur = ((Personne)view.CurrentItem).CodePersonne;
            Properties.Settings.Default.Manager = false;
            Properties.Settings.Default.Save();


            return new ValidationResult(true);
        }
    }
}
