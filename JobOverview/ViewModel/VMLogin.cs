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
            try
            {
                DALPersonne.SauvegardePropriete((Personne)view.CurrentItem);             
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Erreur lors de la connection à l'application!"); 
            }
            return new ValidationResult(true);
        }
    }
}
