using MonoTouch.Dialog;
using System;

namespace LabelingStyle
{
    public abstract class StyleSettings
    {
        public string Title;
        private BindingContext bindingContext;

        public StyleSettings()
        {
        }

        public RootElement Root
        {
            get { return bindingContext.Root; }
        }

        public BindingContext BindingContext
        {
            get { return bindingContext; }
            set { bindingContext = value; }
        }

        public virtual void Sync()
        {
        }

        protected bool GetCheckBoxElementValue(int index)
        {
            return ((CheckboxElement)Root[0][index]).Value;
        }

        protected string GetEntryElementValue(int index)
        {
            EntryElement element = (EntryElement) Root[0][index];
            //element.FetchValue();

            bindingContext.Fetch();
            return element.Value;
        }

        protected T GetRadioElementValue<T>(int index)
        {
            RootElement subRoot = (RootElement)Root[0][index];
            RadioElement radioElement = (RadioElement)subRoot[0][subRoot.RadioSelected];

            return (T)Enum.Parse(typeof(T), radioElement.Caption.Replace(" ", string.Empty));
        }

        protected static double ParseToDouble(string content, double defaultValue)
        {
            double temp;
            double result = defaultValue;
            if (double.TryParse(content, out temp))
            {
                result = temp;
            }

            return result;
        }

        protected static int ParseToInt(string content, int defaultValue)
        {
            int temp;
            int result = defaultValue;
            if (int.TryParse(content, out temp))
            {
                result = temp;
            }

            return result;
        }

        protected static float ParseToFloat(string content, float defaultValue)
        {
            float temp;
            float result = defaultValue;
            if (float.TryParse(content, out temp))
            {
                result = temp;
            }

            return result;
        }
    }
}