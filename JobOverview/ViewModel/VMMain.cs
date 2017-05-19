using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JobOverview.ViewModel
{
	public class VMMain : ViewModelBase
	{
		// Vue-modèle courante sur laquelle est liées le ContentControl
		// de la zone principale
		private ViewModelBase _VMCourante;
		public ViewModelBase VMCourante
		{
			get { return _VMCourante; }
			private set
			{
				SetProperty(ref _VMCourante, value);
			}
		}

		#region Commandes
		private ICommand _cmdLogin;
		public ICommand CmdLogin
		{
			get
			{
				if (_cmdLogin == null)
					_cmdLogin = new RelayCommand(() => VMCourante = new VMLogin());
				return _cmdLogin;
			}
		}

		#endregion

	}
}
