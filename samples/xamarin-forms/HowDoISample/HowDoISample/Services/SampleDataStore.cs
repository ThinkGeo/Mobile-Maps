using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HowDoISample.Models;

namespace HowDoISample.Services
{
    /// <summary>
    /// WIP
    /// </summary>
    class SampleDataStore : IDataStore<Category>
    {
        readonly List<Category> categories;

        public SampleDataStore()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(SampleDataStore)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("HowDoISample.samples.json");
            using (var reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                categories = JsonConvert.DeserializeObject<List<Category>>(text);
            }
        }

        public async Task<Category> GetItemAsync(string id)
        {
            return await Task.FromResult(categories.FirstOrDefault(s => s.Title == id));
        }

        public async Task<IEnumerable<Category>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(categories);
        }
    }
}
