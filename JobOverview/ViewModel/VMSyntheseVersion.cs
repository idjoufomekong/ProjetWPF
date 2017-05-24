using JobOverview.Entity;
using JobOverview.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobOverview.ViewModel
{
    public class VMSyntheseVersion:ViewModelBase
    {
        public ObservableCollection<Logiciel> Logiciels { get; set; }

        public VMSyntheseVersion()
        {
            Logiciels = new ObservableCollection<Entity.Logiciel>( DALLogiciel.RecupererLogicielSynthese());
        }
    }

}
