using Android.App;
using Android.Content;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    public class FilterStyleSample : BaseSample
    {
        private TextView nameTextView;
        private TextView valueEditText;
        private FilterStyle filterStyle;
        private Spinner conditionSpinner;
        private LinearLayout settingsLinearLayout;
        private FilterConditionDefaultValues filterConditionDefaultValues;

        public FilterStyleSample(Context context)
            : base(context)
        { }


        protected override void UpdateSampleLayoutCore()
        {
            FrameLayout mapContainerView = SampleView.FindViewById<FrameLayout>(Resource.Id.MapContainerView);

            FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            layoutParams.Gravity = GravityFlags.Bottom;

            settingsLinearLayout = (LinearLayout)View.Inflate(Application.Context, Resource.Layout.FilterStyleSettings, null);
            mapContainerView.AddView(settingsLinearLayout, layoutParams);

            InitializeSettingView();

            ImageButton settingsButton = SampleView.FindViewById<ImageButton>(Resource.Id.SettingsButton);
            settingsButton.Click += SettingsButtonClick;
            settingsButton.SetImageResource(Resource.Drawable.settings40);
            settingsButton.SetBackgroundResource(Resource.Layout.ButtonBackgroundSelector);
        }

        protected override void InitializeMap()
        {
            MapView.CurrentExtent = new RectangleShape(-13768645, 7689721, -9044012, 110621);

            ShapeFileFeatureLayer statesLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("states.shp"));
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(statesLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);

            filterStyle = new FilterStyle();
            SimpleFilterCondition newCondition = new SimpleFilterCondition("STATE_NAME", SimpleFilterConditionType.Equal, "Texas");
            filterStyle.Conditions.Add(newCondition);

            GeoColor fillColor = GeoColor.FromArgb(130, GeoColor.FromHtml("#ffb74c"));
            GeoColor outlineColor = GeoColor.FromHtml("#333333");
            filterStyle.Styles.Add(AreaStyles.CreateSimpleAreaStyle(fillColor, outlineColor));
            statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(filterStyle);
            statesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }

        private void SettingsButtonClick(object sender, EventArgs e)
        {
            if (settingsLinearLayout.Visibility == ViewStates.Visible)
            {
                Animation animation = AnimationUtils.LoadAnimation(Application.Context, Resource.Animation.bottomViewOut);
                settingsLinearLayout.StartAnimation(animation);
                settingsLinearLayout.Visibility = ViewStates.Invisible;
            }
            else
            {
                settingsLinearLayout.Visibility = ViewStates.Visible;
                Animation animation = AnimationUtils.LoadAnimation(Application.Context, Resource.Animation.bottomViewIn);
                settingsLinearLayout.StartAnimation(animation);
            }
        }

        private void ApplyButtonClick(object sender, EventArgs e)
        {
            filterStyle.Conditions.Clear();
            SimpleFilterCondition newCondition = new SimpleFilterCondition(nameTextView.Text, (SimpleFilterConditionType)conditionSpinner.SelectedItemId, valueEditText.Text);
            filterStyle.Conditions.Add(newCondition);
            MapView.Overlays["LayerOverlay"].Refresh();
        }

        private void InitializeSettingView()
        {
            Button applyButton = settingsLinearLayout.FindViewById<Button>(Resource.Id.applyButton);
            ImageButton moreButton = settingsLinearLayout.FindViewById<ImageButton>(Resource.Id.moreImageButton);
            LinearLayout headerLinearLayout = settingsLinearLayout.FindViewById<LinearLayout>(Resource.Id.headerLinearLayout);
            conditionSpinner = settingsLinearLayout.FindViewById<Spinner>(Resource.Id.conditionSpinner);
            ArrayAdapter<String> adapter = new ArrayAdapter<String>(Application.Context, Resource.Layout.SampleSpinnerCheckedText, Enum.GetNames(typeof(SimpleFilterConditionType)));
            filterConditionDefaultValues = new FilterConditionDefaultValues();

            applyButton.Click += ApplyButtonClick;
            moreButton.Click += SettingsButtonClick;
            headerLinearLayout.Click += SettingsButtonClick;

            conditionSpinner.Adapter = adapter;
            nameTextView = settingsLinearLayout.FindViewById<TextView>(Resource.Id.nameTextView);
            nameTextView.Text = filterConditionDefaultValues.FirstOrDefault().Value.Item1;
            valueEditText = settingsLinearLayout.FindViewById<TextView>(Resource.Id.valueEditText);
            valueEditText.Text = filterConditionDefaultValues.FirstOrDefault().Value.Item2;

            conditionSpinner.SetSelection(0);
            conditionSpinner.ItemSelected += SpinnerItemSelected;
        }

        private void SpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string condition = conditionSpinner.GetItemAtPosition(e.Position).ToString();
            SimpleFilterConditionType conditionType = (SimpleFilterConditionType)Enum.Parse(typeof(SimpleFilterConditionType), condition);
            nameTextView.Text = filterConditionDefaultValues.SingleOrDefault(t => t.Key.Equals(conditionType)).Value.Item1;
            valueEditText.Text = filterConditionDefaultValues.SingleOrDefault(t => t.Key.Equals(conditionType)).Value.Item2;
            valueEditText.Visibility = condition.Contains("Empty") ? ViewStates.Invisible : ViewStates.Visible;
        }
    }
}