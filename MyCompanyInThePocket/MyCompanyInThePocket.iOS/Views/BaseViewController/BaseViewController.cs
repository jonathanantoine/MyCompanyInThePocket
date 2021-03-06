﻿using System.ComponentModel;
using MyCompanyInThePocket.Core.ViewModels;
using UIKit;

namespace MyCompanyInThePocket.iOS.Views
{
    public abstract class BaseViewController<TViewModel> : UIViewController, INoHistoryViewController
		where TViewModel : BaseViewModel , new()
	{
        private TViewModel _viewModel;

		public BaseViewController(bool isMainController)
		{

            View.BackgroundColor = UIColor.White;
			EdgesForExtendedLayout = UIRectEdge.None;
            if (isMainController)
            {
                CurrentViewController.Current = this;
            }
		}

		public TViewModel ViewModel
		{
			get
			{
				if (_viewModel == null)
				{
					_viewModel = new TViewModel();
					_viewModel.PropertyChanged += _viewModel_PropertyChanged;
				}

				return _viewModel;
			}
		}

		protected virtual void _viewModel_PropertyChanged(object sender, PropertyChangedEventArgs arg)
		{
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			ScreenHelper.RemoveNoHistoryPage(NavigationController, this);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
		}
	}
}
