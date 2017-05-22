using JobOverview.Entity;
using JobOverview.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobOverview.ViewModel
{
	public class VMLogin : ViewModelBase
	{
		// TODO : à remplacer par une vraie liste de personnes
		public static ObservableCollection<Personne> Personnes { get; private set; }

		public VMLogin()
		{
			// TODO : à remplacer par un appel à une méthode de DAL
			Personnes = new ObservableCollection<Personne>();
            Personnes = DALPersonne.RecupererToutesPersonne();
		}
	}
}
