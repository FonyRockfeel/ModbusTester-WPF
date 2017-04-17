using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using ModbusTester_WPF.ViewModels;

namespace ModbusTester_WPF.Navigation
{
    public sealed class Navigator
    {
        #region Fields

        private NavigationService _navService;


        private Page _view;
        private static AbstractViewModel _viewModel;


        #endregion


        #region Properties

        public static NavigationService Service
        {
            get { return Instance._navService; }
            set
            {
                if (Instance._navService != null)
                {
                    Instance._navService.Navigated -= Instance._navService_Navigated;
                }

                Instance._navService = value;
                Instance._navService.Navigated += Instance._navService_Navigated;
            }
        }



        #endregion


        #region Public Methods

        public static void Navigate(Page page, object context)
        {

            if (Instance._navService == null)
            {
                return;
            }

            Instance.Nav(page, context);
        }

        public static void Closing()
        {
            _viewModel?.OnNavigatedFrom();
        }
        private void Nav(Page page, object context)
        {
            var prevView = _view;
            var prevViewModel = _viewModel;

            _navService.Navigate(page, context);
            _view = page;
            _viewModel = context as AbstractViewModel;


            prevViewModel?.OnNavigatedFrom();
        }


        #endregion


        #region Private Methods

        void _navService_Navigated(object sender, NavigationEventArgs e)
        {
            var page = e.Content as Page;

            if (page == null)
            {
                return;
            }

            page.DataContext = e.ExtraData;
        }

        #endregion


        #region Singleton

        private static volatile Navigator _instance;
        private static readonly object SyncRoot = new Object();

        private Navigator()
        {
        }

        private static Navigator Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new Navigator();
                    }
                }

                return _instance;
            }
        }
        #endregion
    }
}
