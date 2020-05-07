using MonoTouch.Dialog;
using System;
using UIKit;

namespace LabelingStyle
{
    public class StyleSettingsController<T> : UINavigationController where T : StyleSettings, new()
    {
        private T styleSettings;

        public event EventHandler<StyleSettingsChangedStyleSettingsControllerEventArgs> StyleSettingsChanged;

        public StyleSettingsController()
            : this(new T())
        {
        }

        public StyleSettingsController(T setting)
        {
            styleSettings = setting;
            Title = styleSettings.Title;
            
            BindingContext context = new BindingContext(this, styleSettings, styleSettings.Title);
            styleSettings.BindingContext = context;
            DialogViewController dialogViewController = new DialogViewController(context.Root);

            UIBarButtonItem cancelItem = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, new EventHandler((s, e) => DismissViewController(true, null)));
            UIBarButtonItem saveItem = new UIBarButtonItem("Save", UIBarButtonItemStyle.Plain, new EventHandler((s, e) => OnStyleSettingsChanged()));
            dialogViewController.NavigationItem.SetRightBarButtonItem(saveItem, true);
            dialogViewController.NavigationItem.SetLeftBarButtonItem(cancelItem, true);

            PushViewController(dialogViewController, true);
        }

        public T StyleSettings
        {
            get { return styleSettings; }
        }

        protected void OnStyleSettingsChanged()
        {
            EventHandler<StyleSettingsChangedStyleSettingsControllerEventArgs> handler = StyleSettingsChanged;
            if (handler != null)
            {
                styleSettings.Sync();
                handler(this, new StyleSettingsChangedStyleSettingsControllerEventArgs(styleSettings));
                DismissViewController(true, null);
            }
        }

    }
}