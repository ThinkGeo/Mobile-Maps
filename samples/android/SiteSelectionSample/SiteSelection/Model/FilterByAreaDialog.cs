using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using ThinkGeo.MapSuite.Shapes;

namespace MapSuiteSiteSelection
{
    internal class FilterByAreaDialog : AlertDialog
    {
        private Spinner spinner;
        private EditText editBufferValueText;
        private Context context;

        public FilterByAreaDialog(Context context)
            : base(context)
        {

            this.context = context;

            View filterByAreaDialogLayout = View.Inflate(context, Resource.Layout.FilterByAreaDialogLayout, null);
            SetView(filterByAreaDialogLayout);

            spinner = filterByAreaDialogLayout.FindViewById<Spinner>(Resource.Id.distanceUnitSpinner);
            editBufferValueText = filterByAreaDialogLayout.FindViewById<EditText>(Resource.Id.editBufferValueText);
            Button cancelButton = filterByAreaDialogLayout.FindViewById<Button>(Resource.Id.CancelButton);
            Button okButton = filterByAreaDialogLayout.FindViewById<Button>(Resource.Id.OkButton);

            string[] items = { "Miles", "Kilometers" };
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(context, Resource.Layout.SpinnerTextViewLayout, items);
            spinner.Adapter = adapter;

            okButton.Click += OkButton_Click;
            cancelButton.Click += CancelButton_Click;
        }

        private void OkButton_Click(object sender, System.EventArgs e)
        {
            double bufferValue;
            if (double.TryParse(editBufferValueText.Text, out bufferValue))
            {
                SampleMapView.Current.FilterConfiguration.BufferValue = bufferValue;
            }
            SampleMapView.Current.FilterConfiguration.BufferDistanceUnit = spinner.SelectedItemPosition == 0 ? DistanceUnit.Mile : DistanceUnit.Kilometer;
            Cancel();

            SampleMapView.Current.UpdateHighlightOverlay();
        }

        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            Cancel();
        }
    }
}