using System;

namespace LabelingStyle
{
    public class StyleSettingsChangedStyleSettingsControllerEventArgs : EventArgs
    {
        private StyleSettings styleSettings;

        public StyleSettingsChangedStyleSettingsControllerEventArgs(StyleSettings styleSettings)
        {
            this.styleSettings = styleSettings;
        }

        public StyleSettings StyleSettings
        {
            get { return styleSettings; }
            set { styleSettings = value; }
        }
    }
}
