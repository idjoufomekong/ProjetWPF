using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using JobOverview.ViewModel;

namespace JobOverview.View
{
	/// <summary>
	/// Logique d'interaction pour ModalWindow.xaml
	/// </summary>
	public partial class ModalWindow : Window
	{
		private ViewModelBase _vm;

		/// <summary>
		/// Crée une fenêtre modale qui affichera la vue associée
		/// à la vue-modèle passée en paramètre
		/// </summary>
		/// <param name="vm"></param>
		public ModalWindow(ViewModelBase vm)
		{
			InitializeComponent();

			_vm = vm;
			ccPrinc.Content = vm;
			btnOK.Click += BtnOK_Click;
		}

		// Au clic sur le bouton OK
		private void BtnOK_Click(object sender, RoutedEventArgs e)
		{
			// Valide la vue-modèle
			var res = _vm.Validate();

			// Si le résultat est OK, on ferme la fenêtre, sinon on affiche le message d'erreur
			if (res.IsOK)
				DialogResult = true;
			else
				MessageBox.Show(res.ErrorMessage, "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
		}
	}
}
