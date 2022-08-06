using DomainTricks_WPF.ViewModels;
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

namespace DomainTricks_WPF
{
    /// <summary>
    /// Interaction logic for PreferencesView.xaml
    /// </summary>
    public partial class PreferencesView : Window
    {
        public PreferencesView(PreferencesViewModel dataContextVM)
        {
            this.DataContext = dataContextVM;
            
            // Connects the Action in the ViewModel to this action.
            if (dataContextVM.CloseAction == null)
                dataContextVM.CloseAction = new Action(() => this.Close());
        
        InitializeComponent();
        }
    }
}
