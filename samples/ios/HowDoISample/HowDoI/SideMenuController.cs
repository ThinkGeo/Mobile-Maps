using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
	public partial class SideMenuController : BaseController
	{
		public UITableView _tableView;
		public SideMenuController() : base(null, null)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var width = View.Bounds.Width;
			var height = View.Bounds.Height;

			_tableView = new UITableView(new CGRect(0, 0, width, height));
			_tableView.TableFooterView = new UIView(System.Drawing.Rectangle.Empty);
			_tableView.AutoresizingMask = UIViewAutoresizing.All;

			// Get the sample list from XML.
			SampleInfo[] sampleInfos = new SampleInfo[] { };
			var xmlSampleFilePath = NSBundle.MainBundle.PathForResource("SampleList", "xml");
			try
			{
				using (TextReader sampleListXml = new StreamReader(xmlSampleFilePath))
				{
					List<SampleInfo> Samples = new List<SampleInfo>();
					foreach (XElement element in XDocument.Load(sampleListXml).Root.Elements())
					{
						Samples.Add(new SampleInfo(element));
					}
					sampleInfos = Samples.ToArray();
				}
			}
			catch(Exception ex)
			{
			}


			_tableView.Source = new MenuTableSource(sampleInfos, _tableView, this);
			_tableView.RowHeight = UITableView.AutomaticDimension;
			_tableView.EstimatedRowHeight = 60;

			View.BackgroundColor = UIColor.FromRGB(.9f, .9f, .9f);


   //         var title = new UILabel(new RectangleF(0, 50, 230, 20));
   //         title.Font = UIFont.SystemFontOfSize(12.0f);
   //         title.TextAlignment = UITextAlignment.Center;
   //         title.TextColor = UIColor.Blue;
   //         title.Text = "Menu";

			//View.Add(title);

			Add(_tableView);
			
		}

		public void LaunchSampleByClassName(string className, string title, string description)
		{
			try
			{
				BaseController sampleViewController = (BaseController)Activator.CreateInstance(null, className).Unwrap();
				sampleViewController.Title = title;
				NavController.PopToRootViewController(false);
				NavController.PushViewController(sampleViewController, false);
				(UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.UpdateInstructionDescription(description);
				SidebarController.CloseMenu();
			}
			catch (Exception ex)
			{
				ShowAlert("Not Implement.", "Infromation");
				//ShowToast("Not Implement.");
			}
		}



	
		
	}
}

