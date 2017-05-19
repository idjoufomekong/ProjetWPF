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
		public List<string> Personnes { get; private set; }

		public VMLogin()
		{
			// TODO : à remplacer par un appel à une méthode de DAL
			Personnes = new List<string>()	{ "Pierre", "Paul", "Jacques" };
		}
	}
}
