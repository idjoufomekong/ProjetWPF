﻿using System;
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

        private ICommand _cmdEchange;
        public ICommand CmdEchange
        {
            get
            {
                if (_cmdEchange == null)
                    _cmdEchange = new RelayCommand(() => VMCourante = new VMEchange());
                return _cmdEchange;
            }
        }

        private ICommand _cmdSaisieTemps;
        public ICommand CmdSaisieTemps
        {
            get
            {
                if (_cmdSaisieTemps == null)
                    _cmdSaisieTemps = new RelayCommand(() => VMCourante = new VMSaisieTemps());
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
                    _cmdTachesAnnexes = new RelayCommand(() => VMCourante = new VMTachesAnnexes());
                return _cmdTachesAnnexes;
            }
        }

        #endregion

    }
}
