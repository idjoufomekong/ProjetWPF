using JobOverview.Entity;
using JobOverview.Model;
using JobOverview.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace JobOverview.View
{
	/// <summary>
	/// Fenêtre principale de l'application
	/// </summary>
	public partial class MainWindow : Window
	{
        public static ObservableCollection<Personne> Personnes { get; private set; }
        public MainWindow()
		{
			InitializeComponent();
			DataContext = new VMMain();

			Loaded += MainWindow_Loaded;
		}

		// Après chargement de la fenêtre
		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			// Affichage d'une fenêtre modale d'identification
			var dlg = new ModalWindow(new VMLogin());
			dlg.Title = "Identification";
			bool? res = dlg.ShowDialog();

			// Si l'utilisateur annule, on ferme l'application
			if (!res.Value) Close();

            if (res.Value)
            {
                var p = DALPersonne.RecupererPersonneConnecte(Properties.Settings.Default.CodeDernierUtilisateur);
                if (p.Count > 1)
                {
                    Properties.Settings.Default.Manager = true;
                    Properties.Settings.Default.Save();
                }               
            }
        }
	}
}
