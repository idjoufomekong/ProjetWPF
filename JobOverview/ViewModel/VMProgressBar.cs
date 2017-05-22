using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobOverview.ViewModel
{
    public class VMProgressBar: ViewModelBase
    {//TODO: Cacher bouton OK pendant le téléchargement et l'afficher à la fin de celui-ci
        public VMProgressBar()
        {
            onLoad(); 
        }
        private void onLoad()
        {
            Task.Factory.StartNew(() =>
            {
                StartLoading();
                for (int i = 0; i <= 100; ++i)
                {
                    UpdateText(string.Format("{0}% exporté", i));
                    UpdateProgress(i);
                    Thread.Sleep(30);
                }
                StopLoading();
                UpdateText("Export terminé");
            });
        }
        #region IAdvancedLoader

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        private string _textToDisplay;
        public string TextToDisplay
        {
            get { return _textToDisplay; }
            set { SetProperty(ref _textToDisplay, value); }
        }

        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }

        public void StartLoading()
        {
            IsLoading = true;
        }

        public void StopLoading()
        {
            IsLoading = false;
        }

        public void UpdateText(string text)
        {
            TextToDisplay = text;
        }

        public void UpdateProgress(int progress)
        {
            Progress = progress;
        }

        #endregion
        public override ValidationResult Validate()
        {
            if (Progress == 100)
                return new ValidationResult(true);
            else return new ValidationResult(false,"Merci de patienter jusqu'à la fin de l'export");
        }
    }
}
