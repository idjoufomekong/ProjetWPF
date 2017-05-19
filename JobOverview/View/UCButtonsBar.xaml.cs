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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JobOverview.View
{
	/// <summary>
	/// Logique d'interaction pour UCButtonsBar.xaml
	/// </summary>
	public partial class UCButtonsBar : UserControl
	{
		public UCButtonsBar()
		{
			InitializeComponent();
		}

		#region Propriétés de dépendance pour les commandes
		// Commande Ajouter
		public ICommand CmdAdd
		{
			get { return (ICommand)GetValue(CmdAddProperty); }
			set { SetValue(CmdAddProperty, value); }
		}

		public static readonly DependencyProperty CmdAddProperty =
			 DependencyProperty.Register("CmdAdd", typeof(ICommand), typeof(UCButtonsBar));

		// Commande Supprimer
		public ICommand CmdDelete
		{
			get { return (ICommand)GetValue(CmdDeleteProperty); }
			set { SetValue(CmdDeleteProperty, value); }
		}

		public static readonly DependencyProperty CmdDeleteProperty =
			 DependencyProperty.Register("CmdDelete", typeof(ICommand), typeof(UCButtonsBar));

		// Commande Modifier
		public ICommand CmdEdit
		{
			get { return (ICommand)GetValue(CmdEditProperty); }
			set { SetValue(CmdEditProperty, value); }
		}

		public static readonly DependencyProperty CmdEditProperty =
			 DependencyProperty.Register("CmdEdit", typeof(ICommand), typeof(UCButtonsBar));

		// Commande Enregistrer
		public ICommand CmdSave
		{
			get { return (ICommand)GetValue(CmdSaveProperty); }
			set { SetValue(CmdSaveProperty, value); }
		}

		public static readonly DependencyProperty CmdSaveProperty =
			 DependencyProperty.Register("CmdSave", typeof(ICommand), typeof(UCButtonsBar));

		// Commande Annuler
		public ICommand CmdCancel
		{
			get { return (ICommand)GetValue(CmdCancelProperty); }
			set { SetValue(CmdCancelProperty, value); }
		}

		public static readonly DependencyProperty CmdCancelProperty =
			 DependencyProperty.Register("CmdCancel", typeof(ICommand), typeof(UCButtonsBar));

		#endregion

		#region Propriétés de dépendance pour la visibilité des boutons
		public Visibility ButtonAddVisibility
		{
			get { return (Visibility)GetValue(ButtonAddVisibilityProperty); }
			set { SetValue(ButtonAddVisibilityProperty, value); }
		}

		public static readonly DependencyProperty ButtonAddVisibilityProperty =
			 DependencyProperty.Register("ButtonAddVisibility", typeof(Visibility), typeof(UCButtonsBar), new PropertyMetadata(Visibility.Visible));

		public Visibility ButtonDeleteVisibility
		{
			get { return (Visibility)GetValue(ButtonDeleteVisibilityProperty); }
			set { SetValue(ButtonDeleteVisibilityProperty, value); }
		}

		public static readonly DependencyProperty ButtonDeleteVisibilityProperty =
			 DependencyProperty.Register("ButtonDeleteVisibility", typeof(Visibility), typeof(UCButtonsBar), new PropertyMetadata(Visibility.Visible));

		public Visibility ButtonEditVisibility
		{
			get { return (Visibility)GetValue(ButtonEditVisibilityProperty); }
			set { SetValue(ButtonEditVisibilityProperty, value); }
		}

		public static readonly DependencyProperty ButtonEditVisibilityProperty =
			 DependencyProperty.Register("ButtonEditVisibility", typeof(Visibility), typeof(UCButtonsBar), new PropertyMetadata(Visibility.Collapsed));

		public Visibility ButtonsSaveCancelVisibility
		{
			get { return (Visibility)GetValue(ButtonsSaveCancelVisibilityProperty); }
			set { SetValue(ButtonsSaveCancelVisibilityProperty, value); }
		}

		public static readonly DependencyProperty ButtonsSaveCancelVisibilityProperty =
			 DependencyProperty.Register("ButtonsSaveCancelVisibility", typeof(Visibility), typeof(UCButtonsBar), new PropertyMetadata(Visibility.Visible));

		// Visibilité des libellés dans les boutons
		public Visibility LabelsVisibility
		{
			get { return (Visibility)GetValue(LabelsVisibilityProperty); }
			set { SetValue(LabelsVisibilityProperty, value); }
		}

		public static readonly DependencyProperty LabelsVisibilityProperty =
			 DependencyProperty.Register("LabelsVisibility", typeof(Visibility), typeof(UCButtonsBar), new PropertyMetadata(Visibility.Visible));


		#endregion

	}
}
