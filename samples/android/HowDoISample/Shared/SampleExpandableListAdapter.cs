using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using BuiltInResources = Android.Resource;

namespace ThinkGeo.UI.Android.HowDoI
{
    class SampleExpandableListAdapter : BaseExpandableListAdapter
    {
        // The view context
        private readonly Context _context;
        // The sample category list and their samples
        public List<SampleCategory> Categories { get; }

        /// <summary>
        /// The SampleExpandableListAdapter class adapts a list of sample categories and their sample list items
        /// to be displayed in an ExpandableListView.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sampleCategories"></param>
        public SampleExpandableListAdapter(Context context, List<SampleCategory> sampleCategories)
        {
            _context = context;
            Categories = sampleCategories;
        }

        public override int GroupCount => Categories.Count();

        public override bool HasStableIds => true;

        /// <summary>
        /// Creates the view that is used to display sample categories in the ExpandableListView
        /// </summary>
        /// <param name="groupPosition"></param>
        /// <param name="isExpanded"></param>
        /// <param name="convertView"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                var inflater = _context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
                view = inflater.Inflate(BuiltInResources.Layout.SimpleExpandableListItem1, null);
            }

            var category = Categories[groupPosition];

            var textView = view.FindViewById<TextView>(BuiltInResources.Id.Text1);
            textView.Text = category.Title;

            return view;
        }

        /// <summary>
        /// Creates the view that is used to display sample list items in the ExpandableListView
        /// </summary>
        /// <param name="groupPosition"></param>
        /// <param name="childPosition"></param>
        /// <param name="isLastChild"></param>
        /// <param name="convertView"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                var inflater = _context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
                view = inflater.Inflate(BuiltInResources.Layout.SimpleExpandableListItem1, null);
            }

            var sample = Categories[groupPosition].Children[childPosition];

            var textView = view.FindViewById<TextView>(BuiltInResources.Id.Text1);
            textView.Text = sample.Title;

            return view;
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return null;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return Categories[groupPosition].Children.Count();
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return null;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }

    /// <summary>
    /// Sample item model
    /// </summary>
    public class SampleListItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
    }

    /// <summary>
    /// Sample category model
    /// </summary>
    public class SampleCategory
    {
        public string Title { get; set; }
        public List<SampleListItem> Children { get; set; }
    }
}