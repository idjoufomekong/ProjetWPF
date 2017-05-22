﻿
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

        public VMEchange()
        {
            //Chargement des ComboBox en observablecollection pour anticiper la MAJ lors de la création de nouvelles versions ou 
            //logiciels
            //Tester auparavant si la liste est vide ou pas
            if(Logiciels==null)
            Logiciels = new ObservableCollection<Logiciel>(DALLogiciel.RecupererLogicielsVersions());

            // J'instancie juste la liste pour initialiser le DataContext car la liste est chargée au clic du bouton
            TachesApercu = new ObservableCollection<TacheApercu>();

            _userCourant = Properties.Settings.Default.CodeDernierUtilisateur;
        }

        #region Commandes
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
        private void Charger()
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

        private void Exporter()
        {
            LogicielCourant = (Logiciel)CollectionViewSource.GetDefaultView(Logiciels).CurrentItem;
            VersionCourante = (Entity.Version)CollectionViewSource.GetDefaultView(LogicielCourant.Versions).CurrentItem;
            PersonnesTaches = DALTache.RecupererPersonnesTaches(LogicielCourant.CodeLogiciel,
                VersionCourante.NumVersion, _userCourant);
            //DALEchange.ExporterXML(PersonnesTaches);

            SaveFileDialog dos = new SaveFileDialog();
            dos.Filter = "XML Files (*.xml)|*.xml";
            dos.DefaultExt = "xml";
            dos.AddExtension = true;
            if (dos.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dos.FileName))
            {
                DALEchange.ExporterXML2(PersonnesTaches, dos.FileName);
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
