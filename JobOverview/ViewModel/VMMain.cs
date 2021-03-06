﻿using JobOverview.Entity;
using JobOverview.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JobOverview.ViewModel
{
    public class VMMain : ViewModelBase
    {

        private ObservableCollection<Logiciel> _logiciel;
        public bool Manager { get; } = Properties.Settings.Default.Manager;
        public ObservableCollection<Logiciel> Logiciels
        {
            get { return _logiciel; }
            set
            {
                SetProperty(ref _logiciel, value);
            }
        }

        private ObservableCollection<Personne> _personnes;
        public ObservableCollection<Personne> Personnes
        {
            get { return _personnes; }
            set
            {
                SetProperty(ref _personnes, value);
            }
        }

        private ObservableCollection<Personne> _personnesTaches;
        public ObservableCollection<Personne> PersonnesTaches
        {
            get { return _personnesTaches; }
            set
            {
                SetProperty(ref _personnesTaches, value);
            }
        }
        public Personne Utilisateur { get; set; }


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

        // Commande associée à la gestion du login
        private ICommand _cmdLogin;
        public ICommand CmdLogin
        {
            get
            {
                if (_cmdLogin == null)
                    _cmdLogin = new RelayCommand((o) => VMCourante = new VMLogin());
                return _cmdLogin;
            }
        }

        //Commande associée à la gestion de l'exportation XML
        private ICommand _cmdEchange;
        public ICommand CmdEchange
        {
            get
            {
                if (Logiciels == null || Logiciels.Count == 0)
                    Logiciels = new ObservableCollection<Logiciel>(DALLogiciel.RecupererLogicielsVersions());
                if (_cmdEchange == null)
                    _cmdEchange = new RelayCommand((o) => VMCourante = new VMEchange(Logiciels), (o) => { return Properties.Settings.Default.Manager; });
                return _cmdEchange;
            }
        }

        //Commande associée à la barre progression de la fenetre d'export
        private ICommand _cmdProgressBar;
        public ICommand CmdProgressBar
        {
            get
            {
                if (_cmdProgressBar == null)
                    _cmdProgressBar = new RelayCommand((o) => VMCourante = new VMProgressBar());
                return _cmdProgressBar;
            }
        }

        //Commande associée à la gestion du temps des travaaux
        private ICommand _cmdSaisieTemps;
        public ICommand CmdSaisieTemps
        {
            get
            {
                if (Logiciels == null || Logiciels.Count == 0 )
                    Logiciels = new ObservableCollection<Logiciel>(DALLogiciel.RecupererLogicielsVersions());
                if (_cmdSaisieTemps == null)
                    _cmdSaisieTemps = new RelayCommand((o) => VMCourante = new VMSaisieTemps(Logiciels));
                return _cmdSaisieTemps;
            }
        }

        // Commande associée à la gestion des tâches annexes
        private ICommand _cmdTachesAnnexes;
        public ICommand CmdTachesAnnexes
        {
            get
            {
                if (_cmdTachesAnnexes == null)
                    _cmdTachesAnnexes = new RelayCommand((o) => VMCourante = new VMTachesAnnexes());
                return _cmdTachesAnnexes;
            }
        }

        //Commande associée à la gestion des tâches de productions
        private ICommand _cmdTachesProd;
        public ICommand CmdTachesProd
        {
            get
            {
                if (Logiciels == null || Logiciels.Count == 0)
                    Logiciels = new ObservableCollection<Logiciel>(DALLogiciel.RecupererLogicielsVersions());
                if (PersonnesTaches == null || PersonnesTaches.Count == 0)
                    PersonnesTaches = new ObservableCollection<Personne>(DALTache.RecupererPersonnesTachesProd(Properties.Settings.Default.CodeDernierUtilisateur));
                if (_cmdTachesProd == null)
                    _cmdTachesProd = new RelayCommand((o) => VMCourante = new VMTachesProd(Logiciels, PersonnesTaches));
                return _cmdTachesProd;
            }
        }

        //Commande associée l'affichage de la vue de synthèse logiciel/version
        private ICommand _cmdSynthese;
        public ICommand CmdSynthese
        {
            get
            {
                if (_cmdSynthese == null)
                    _cmdSynthese = new RelayCommand((o) => VMCourante = new VMSyntheseVersion());
                return _cmdSynthese;
            }
        }

        //Commande associée à l'affichage des métadonnées
        private ICommand _cmdAPropos;
        public ICommand CmdAPropos
        {
            get
            {
                if (_cmdAPropos == null)
                    _cmdAPropos = new RelayCommand((o) => VMCourante = new VMAPropos());
                return _cmdAPropos;
            }
        }
        #endregion

    }
}
