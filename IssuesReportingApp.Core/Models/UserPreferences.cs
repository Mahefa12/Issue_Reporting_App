using System;
using System.Collections.Generic;
using System.Linq;

namespace IssuesReportingApp.Models
{
    public class UserPreferences
    {
        public Dictionary<string, int> CategoryPreferences { get; set; }
        public Dictionary<string, int> LocationPreferences { get; set; }
        public Dictionary<int, int> TimePreferences { get; set; }
        public Dictionary<DayOfWeek, int> DayPreferences { get; set; }
        public List<string> SearchHistory { get; set; }
        public Dictionary<string, int> SearchFrequency { get; set; }
        public List<int> ViewedEventIds { get; set; }
        public HashSet<int> LikedEventIds { get; set; }
        public HashSet<int> DislikedEventIds { get; set; }
        public DateTime LastUpdated { get; set; }
        public int TotalInteractions { get; set; }

        public UserPreferences()
        {
            CategoryPreferences = new Dictionary<string, int>();
            LocationPreferences = new Dictionary<string, int>();
            TimePreferences = new Dictionary<int, int>();
            DayPreferences = new Dictionary<DayOfWeek, int>();
            SearchHistory = new List<string>();
            SearchFrequency = new Dictionary<string, int>();
            ViewedEventIds = new List<int>();
            LikedEventIds = new HashSet<int>();
            DislikedEventIds = new HashSet<int>();
            LastUpdated = DateTime.Now;
            TotalInteractions = 0;
        }

        public void RecordCategoryInteraction(string category, int weight = 1)
        {
            if (string.IsNullOrEmpty(category)) return;
            if (CategoryPreferences.ContainsKey(category))
                CategoryPreferences[category] += weight;
            else
                CategoryPreferences[category] = weight;
            TotalInteractions++;
            LastUpdated = DateTime.Now;
        }

        public void RecordLocationInteraction(string location, int weight = 1)
        {
            if (string.IsNullOrEmpty(location)) return;
            if (LocationPreferences.ContainsKey(location))
                LocationPreferences[location] += weight;
            else
                LocationPreferences[location] = weight;
        }

        public void RecordTimePreference(DateTime eventTime, int weight = 1)
        {
            int hour = eventTime.Hour;
            if (TimePreferences.ContainsKey(hour))
                TimePreferences[hour] += weight;
            else
                TimePreferences[hour] = weight;

            DayOfWeek day = eventTime.DayOfWeek;
            if (DayPreferences.ContainsKey(day))
                DayPreferences[day] += weight;
            else
                DayPreferences[day] = weight;
        }

        public void RecordSearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return;
            searchTerm = searchTerm.ToLower().Trim();
            SearchHistory.Insert(0, searchTerm);
            if (SearchHistory.Count > 50)
                SearchHistory.RemoveAt(SearchHistory.Count - 1);
            if (SearchFrequency.ContainsKey(searchTerm))
                SearchFrequency[searchTerm]++;
            else
                SearchFrequency[searchTerm] = 1;
        }

        public void RecordEventView(int eventId)
        {
            if (!ViewedEventIds.Contains(eventId))
            {
                ViewedEventIds.Insert(0, eventId);
                if (ViewedEventIds.Count > 100)
                    ViewedEventIds.RemoveAt(ViewedEventIds.Count - 1);
            }
        }

        public List<string> GetTopCategories(int count = 5)
        {
            return CategoryPreferences
                .OrderByDescending(kvp => kvp.Value)
                .Take(count)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public List<string> GetTopLocations(int count = 3)
        {
            return LocationPreferences
                .OrderByDescending(kvp => kvp.Value)
                .Take(count)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public List<int> GetPreferredHours(int count = 3)
        {
            return TimePreferences
                .OrderByDescending(kvp => kvp.Value)
                .Take(count)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public List<string> GetRecentSearches(int count = 10)
        {
            return SearchHistory.Take(count).ToList();
        }

        public double GetCategoryAffinity(string category)
        {
            if (!CategoryPreferences.ContainsKey(category) || TotalInteractions == 0)
                return 0.0;
            return (double)CategoryPreferences[category] / TotalInteractions;
        }

        public double GetLocationAffinity(string location)
        {
            if (!LocationPreferences.ContainsKey(location))
                return 0.0;
            int totalLocationInteractions = LocationPreferences.Values.Sum();
            return totalLocationInteractions > 0 ? (double)LocationPreferences[location] / totalLocationInteractions : 0.0;
        }

        public void RecordFeedback(Models.Event e, bool liked)
        {
            if (e == null) return;
            if (liked)
            {
                LikedEventIds.Add(e.Id);
                RecordCategoryInteraction(e.Category, 3);
                RecordLocationInteraction(e.Location, 3);
                RecordTimePreference(e.StartDate, 2);
            }
            else
            {
                DislikedEventIds.Add(e.Id);
                // Reduce affinity by subtracting, clamped at 0
                if (!string.IsNullOrEmpty(e.Category))
                {
                    CategoryPreferences[e.Category] = Math.Max(0, CategoryPreferences.GetValueOrDefault(e.Category, 0) - 2);
                }
                if (!string.IsNullOrEmpty(e.Location))
                {
                    LocationPreferences[e.Location] = Math.Max(0, LocationPreferences.GetValueOrDefault(e.Location, 0) - 2);
                }
                RecordTimePreference(e.StartDate, 1);
            }
            LastUpdated = DateTime.Now;
        }
    }
}