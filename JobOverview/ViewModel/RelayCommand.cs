// The following code is inspired by the work of Josh Smith
// http://joshsmithonwpf.wordpress.com/
using System;
using System.Windows.Input;

namespace JobOverview.ViewModel
{

    public class RelayCommand : ICommand
    {
        // Champs privés
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        // Constructeurs
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        // Implémentation de l'interface ICommand
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
    }


    ///// <summary>
    ///// A command whose sole purpose is to relay its functionality to other objects
    ///// by invoking delegates. The default return value for the CanExecute method is 'true'.
    ///// </summary>
    //public class RelayCommand : ICommand
    //{
    //    #region private fields
    //    private readonly Action execute;
    //    private readonly Func<bool> canExecute;
    //    #endregion

    //    public event EventHandler CanExecuteChanged
    //    {
    //        add
    //        {
    //            if (this.canExecute != null)
    //                CommandManager.RequerySuggested += value;
    //        }
    //        remove
    //        {
    //            if (this.canExecute != null)
    //                CommandManager.RequerySuggested -= value;
    //        }
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the RelayCommand class
    //    /// </summary>
    //    /// <param name="execute">The execution logic.</param>
    //    public RelayCommand(Action execute) : this(execute, null)
    //    {
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the RelayCommand class
    //    /// </summary>
    //    /// <param name="execute">The execution logic.</param>
    //    /// <param name="canExecute">The execution status logic.</param>
    //    public RelayCommand(Action execute, Func<bool> canExecute)
    //    {
    //        if (execute == null)
    //            throw new ArgumentNullException("execute");

    //        this.execute = execute;
    //        this.canExecute = canExecute;
    //    }

    //    public void Execute(object parameter)
    //    {
    //        this.execute();
    //    }

    //    public bool CanExecute(object parameter)
    //    {
    //        return this.canExecute == null ? true : this.canExecute();
    //    }
    //}
}
