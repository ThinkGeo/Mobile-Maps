using Android.Content;
using Android.Media;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class SampleListAdapter : BaseExpandableListAdapter
    {
        private Context context;

        public SampleListAdapter(Context context, System.IO.Stream sampleListXml)
        {
            this.context = context;

            Samples = new List<SampleInfo>();
            foreach (XElement element in XDocument.Load(sampleListXml).Root.Elements())
            {
                Samples.Add(new SampleInfo(element));
            }
        }

        public List<SampleInfo> Samples { get; }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return Samples.ElementAt(groupPosition).Children.ElementAt(childPosition).Name;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            String name = Samples.ElementAt(groupPosition).Children.ElementAt(childPosition).Name;
            TextView textView = GetGenericView(name);
            textView.SetPadding((int)(36 * context.Resources.DisplayMetrics.Density), 0, 0, 0);
            return textView;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return Samples.ElementAt(groupPosition).Children.Count;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return Samples.ElementAt(groupPosition).Name;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            String name = Samples.ElementAt(groupPosition).Name;
            TextView textView = GetGenericView(name);
            textView.Text = textView.Text + string.Format("({0})", Samples.ElementAt(groupPosition).Children.Count);
            textView.SetPadding((int)(10 * context.Resources.DisplayMetrics.Density), 0, 0, 0);
            return textView;
        }

        public override int GroupCount
        {
            get { return Samples.Count; }
        }

        public override bool HasStableIds
        {
            get { return false; }
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

        public TextView GetGenericView(String name)
        {
            AbsListView.LayoutParams layoutParams = new AbsListView.LayoutParams(ViewGroup.LayoutParams.FillParent, (int)(40 * context.Resources.DisplayMetrics.Density));
            TextView textView = new TextView(context, null, 0);
            textView.LayoutParameters = layoutParams;
            textView.Gravity = GravityFlags.CenterVertical | GravityFlags.Left;
            textView.Text = name;
            return textView;
        }
    }
}