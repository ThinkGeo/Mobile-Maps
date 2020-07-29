using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HowDoISample.Models
{
    class CategoryMenuItem : ObservableCollection<SampleMenuItem>
    {
        public string Title { get; set; }
    }
}
