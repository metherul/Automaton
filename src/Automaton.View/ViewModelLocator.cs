/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Automaton"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Automaton.ViewModel;

namespace Automaton.View
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ViewModel.MainWindow>();
            SimpleIoc.Default.Register<ViewModel.LoadModpack>();
            SimpleIoc.Default.Register<ViewModel.InitialSetup>();
            SimpleIoc.Default.Register<ViewModel.SetupAssistant>();
            SimpleIoc.Default.Register<ViewModel.ValidateMods>();

            SimpleIoc.Default.Register<ViewModel.Controllers.SnackbarController>();


        }

        public ViewModel.MainWindow MainWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewModel.MainWindow>();
            }
        }

        public ViewModel.LoadModpack LoadModpack
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewModel.LoadModpack>();
            }
        }

        public ViewModel.InitialSetup InitialSetup
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewModel.InitialSetup>();
            }
        }

        public ViewModel.SetupAssistant SetupAssistant
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewModel.SetupAssistant>();
            }
        }

        public ViewModel.ValidateMods ValidateMods
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewModel.ValidateMods>();
            }
        }

        public ViewModel.Controllers.SnackbarController SnackbarController
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewModel.Controllers.SnackbarController>();
            }
        }


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}