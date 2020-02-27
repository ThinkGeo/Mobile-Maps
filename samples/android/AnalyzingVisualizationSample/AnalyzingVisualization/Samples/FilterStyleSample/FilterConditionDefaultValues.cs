using System;
using System.Collections.Generic;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    public class FilterConditionDefaultValues : Dictionary<SimpleFilterConditionType, Tuple<string, string>>
    {
        public FilterConditionDefaultValues()
        {
            Add(SimpleFilterConditionType.Equal, "STATE_NAME", "Texas");
            Add(SimpleFilterConditionType.Contains, "STATE_NAME", "T");
            Add(SimpleFilterConditionType.StartsWith, "STATE_NAME", "T");
            Add(SimpleFilterConditionType.EndsWith, "STATE_NAME", "a");
            Add(SimpleFilterConditionType.DoesNotEqual, "STATE_NAME", "Texas");
            Add(SimpleFilterConditionType.DoesNotContain, "STATE_NAME", "Te");
            Add(SimpleFilterConditionType.GreaterThan, "POP1990", "1100000");
            Add(SimpleFilterConditionType.GreaterThanOrEqualTo, "POP1990", "1003464");
            Add(SimpleFilterConditionType.LessThan, "POP1990", "1003464");
            Add(SimpleFilterConditionType.LessThanOrEqualTo, "POP1990", "1003464");
            Add(SimpleFilterConditionType.IsEmpty, "STATE_NAME", string.Empty);
            Add(SimpleFilterConditionType.IsNotEmpty, "STATE_NAME", string.Empty);
        }

        public void Add(SimpleFilterConditionType conditionType, string columnName, string matchValue)
        {
            Add(conditionType, new Tuple<string, string>(columnName, matchValue));
        }
    }
}