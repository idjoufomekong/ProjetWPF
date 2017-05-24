
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using JobOverview.Entity;
using JobOverview.Model;
using JobOverview.View;

namespace JobOverview.ViewModel
{
    public class VMEchange : ViewModelBase
    {
        //Champs privés
        private Logiciel _logicielCourant;
        private Entity.Version _version;
        private string _userCourant;

        #region Propriétés

        public List<Personne> PersonnesTaches { get; private set; }
        public ObservableCollection<TacheApercu> TachesApercu { get; private set; }
        public ObservableCollection<Logiciel> Logiciels { get; private set; }
        public ObservableCollection<Entity.Version> Versions { get; set; }
        public Logiciel LogicielCourant
        {
            get { return _logicielCourant; }
            set { SetProperty(ref _logicielCourant, value); }
        }
        public Entity.Version VersionCourante
        {
            get { return _version; }
            set { SetProperty(ref _version, value); }
        }
        #endregion

        public VMEchange(ObservableCollection<Logiciel> logicielVMMain)
        {
            Logiciels = logicielVMMain;

            TachesApercu = new ObservableCollection<TacheApercu>();

            _userCourant = Properties.Settings.Default.CodeDernierUtilisateur;
        }

        #region Commandes

        /// <summary>
        /// Commande d'exportation
        /// </summary>
        private ICommand _cmdExporter;
        public ICommand CmdExporter
        {
            get
            {
                if (_cmdExporter == null)
                    _cmdExporter = new RelayCommand(Exporter);
                return _cmdExporter;
            }
        }

        /// <summary>
        /// Commande d'aperçu
        /// </summary>
        private ICommand _cmdCharger;
        public ICommand CmdCharger
        {
            get
            {
                if (_cmdCharger == null)
                    _cmdCharger = new RelayCommand(Charger);
                return _cmdCharger;
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Charge la liste des personnes et tâches en brut pour le logiciel la version sélectionnés
        /// </summary>
        private void Charger(object obj)
        {
            LogicielCourant = (Logiciel)CollectionViewSource.GetDefaultView(Logiciels).CurrentItem;
            VersionCourante = (Entity.Version)CollectionViewSource.GetDefaultView(LogicielCourant.Versions).CurrentItem;
            var t = new ObservableCollection<TacheApercu>(DALTache.RecupererTachesApercu(LogicielCourant.CodeLogiciel,
                VersionCourante.NumVersion, _userCourant));

            //Chargement du DataContext
            TachesApercu.Clear();
            foreach (var a in t)
            {
                TachesApercu.Add(a);
            }
        }

        /// <summary>
        /// Exportation au format xml
        /// </summary>
        /// <param name="obj">Paramètre de la commande</param>
        private void Exporter(object obj)
        {

            LogicielCourant = (Logiciel)CollectionViewSource.GetDefaultView(Logiciels).CurrentItem;
            VersionCourante = (Entity.Version)CollectionViewSource.GetDefaultView(LogicielCourant.Versions).CurrentItem;
            PersonnesTaches = DALTache.RecupererPersonnesTaches( _userCourant);

            foreach (var b in PersonnesTaches)
            {
                // Si la personne en cours (b) à des taches de production associées
                if (b.TachesProd != null)
                {
                    var p = b.TachesProd.Where(x => (x.CodeVersion == VersionCourante.NumVersion)
                    && (x.CodeLogiciel == LogicielCourant.CodeLogiciel)).ToList();
                    b.TachesProd = new ObservableCollection<Entity.TacheProd>(p);
                }
            }

            // Ouverture de la fenètre pour choisir le chemin d'accès au fichier exporté
            SaveFileDialog dos = new SaveFileDialog();
            dos.Filter = "XML Files (*.xml)|*.xml";
            dos.DefaultExt = "xml";
            dos.AddExtension = true;
            if (dos.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dos.FileName))
            {
                DALEchange.ExporterXML(PersonnesTaches, dos.FileName);
            }

            #region TestProgressBar
            
            var dlg = new ModalWindow(new VMProgressBar());
            dlg.Title = "Progression de l'export";
            bool? res = dlg.ShowDialog();

            #endregion
        }

        #endregion



    }
}
