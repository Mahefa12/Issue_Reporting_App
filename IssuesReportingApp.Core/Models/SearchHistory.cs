using System;
using System.Collections.Generic;
using System.Linq;

namespace IssuesReportingApp.Models
{
    public class SearchEntry
    {
        public string Query { get; set; }
        public DateTime Timestamp { get; set; }
        public string Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ResultsCount { get; set; }
        public bool FoundResults { get; set; }

        public SearchEntry()
        {
            Timestamp = DateTime.Now;
            Query = string.Empty;
            Category = string.Empty;
        }

        public SearchEntry(string query, string category = "", DateTime? startDate = null, DateTime? endDate = null, int resultsCount = 0)
        {
            Query = query ?? string.Empty;
            Category = category ?? string.Empty;
            StartDate = startDate;
            EndDate = endDate;
            ResultsCount = resultsCount;
            FoundResults = resultsCount > 0;
            Timestamp = DateTime.Now;
        }
    }

    public class SearchHistory
    {
        private readonly LinkedList<SearchEntry> _searchEntries;
        private readonly Dictionary<string, int> _queryFrequency;
        private readonly Dictionary<string, int> _categorySearchCount;
        private readonly Dictionary<string, DateTime> _lastSearchByQuery;
        private readonly int _maxHistorySize;

        public SearchHistory(int maxHistorySize = 100)
        {
            _maxHistorySize = maxHistorySize;
            _searchEntries = new LinkedList<SearchEntry>();
            _queryFrequency = new Dictionary<string, int>();
            _categorySearchCount = new Dictionary<string, int>();
            _lastSearchByQuery = new Dictionary<string, DateTime>();
        }

        public void AddSearch(SearchEntry entry)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.Query)) return;

            _searchEntries.AddFirst(entry);

            string normalizedQuery = entry.Query.ToLower().Trim();
            _queryFrequency[normalizedQuery] = _queryFrequency.GetValueOrDefault(normalizedQuery, 0) + 1;
            _lastSearchByQuery[normalizedQuery] = entry.Timestamp;

            if (!string.IsNullOrWhiteSpace(entry.Category))
            {
                _categorySearchCount[entry.Category] = _categorySearchCount.GetValueOrDefault(entry.Category, 0) + 1;
            }

            while (_searchEntries.Count > _maxHistorySize)
            {
                var removedEntry = _searchEntries.Last.Value;
                _searchEntries.RemoveLast();

                string removedQuery = removedEntry.Query.ToLower().Trim();
                if (_queryFrequency.ContainsKey(removedQuery))
                {
                    _queryFrequency[removedQuery]--;
                    if (_queryFrequency[removedQuery] <= 0)
                    {
                        _queryFrequency.Remove(removedQuery);
                        _lastSearchByQuery.Remove(removedQuery);
                    }
                }
            }
        }

        public void AddSearch(string query, string category = "", DateTime? startDate = null, DateTime? endDate = null, int resultsCount = 0)
        {
            var entry = new SearchEntry(query, category, startDate, endDate, resultsCount);
            AddSearch(entry);
        }

        public List<SearchEntry> GetRecentSearches(int count = 10)
        {
            return _searchEntries.Take(count).ToList();
        }

        public List<string> GetMostFrequentQueries(int count = 10)
        {
            return _queryFrequency
                .OrderByDescending(kvp => kvp.Value)
                .ThenByDescending(kvp => _lastSearchByQuery.GetValueOrDefault(kvp.Key, DateTime.MinValue))
                .Take(count)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public List<string> GetMostSearchedCategories(int count = 5)
        {
            return _categorySearchCount
                .OrderByDescending(kvp => kvp.Value)
                .Take(count)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public Dictionary<string, int> GetCategorySearchFrequency()
        {
            return new Dictionary<string, int>(_categorySearchCount);
        }

        public List<SearchEntry> GetSearchesByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return new List<SearchEntry>();

            return _searchEntries
                .Where(entry => string.Equals(entry.Category, category, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<SearchEntry> GetSearchesByDateRange(DateTime startDate, DateTime endDate)
        {
            return _searchEntries
                .Where(entry => entry.Timestamp >= startDate && entry.Timestamp <= endDate)
                .ToList();
        }

        public List<string> GetRelatedQueries(string query, int count = 5)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<string>();

            var normalizedQuery = query.ToLower().Trim();
            var queryWords = normalizedQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var relatedQueries = _queryFrequency.Keys
                .Where(q => q != normalizedQuery)
                .Where(q => queryWords.Any(word => q.Contains(word)))
                .OrderByDescending(q => _queryFrequency[q])
                .Take(count)
                .ToList();

            return relatedQueries;
        }

        public double GetQueryPopularityScore(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return 0.0;

            var normalizedQuery = query.ToLower().Trim();
            if (!_queryFrequency.ContainsKey(normalizedQuery)) return 0.0;

            int frequency = _queryFrequency[normalizedQuery];
            int totalSearches = _searchEntries.Count;

            if (totalSearches == 0) return 0.0;

            double popularityScore = (double)frequency / totalSearches;

            if (_lastSearchByQuery.ContainsKey(normalizedQuery))
            {
                var daysSinceLastSearch = (DateTime.Now - _lastSearchByQuery[normalizedQuery]).TotalDays;
                var recencyBoost = Math.Max(0, 1.0 - (daysSinceLastSearch / 30.0));
                popularityScore *= (1.0 + recencyBoost);
            }

            return popularityScore;
        }

        public List<string> GetTrendingQueries(int days = 7, int count = 5)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            var recentSearches = _searchEntries
                .Where(entry => entry.Timestamp >= cutoffDate)
                .ToList();

            var recentQueryFrequency = new Dictionary<string, int>();
            foreach (var entry in recentSearches)
            {
                var normalizedQuery = entry.Query.ToLower().Trim();
                recentQueryFrequency[normalizedQuery] = recentQueryFrequency.GetValueOrDefault(normalizedQuery, 0) + 1;
            }

            return recentQueryFrequency
                .OrderByDescending(kvp => kvp.Value)
                .Take(count)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public SearchPattern GetSearchPattern()
        {
            var pattern = new SearchPattern();

            if (_searchEntries.Count == 0) return pattern;

            var searchHours = _searchEntries.Select(e => e.Timestamp.Hour).ToList();
            pattern.PreferredSearchHours = searchHours
                .GroupBy(h => h)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            var searchDays = _searchEntries.Select(e => e.Timestamp.DayOfWeek).ToList();
            pattern.PreferredSearchDays = searchDays
                .GroupBy(d => d)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            if (_searchEntries.Count > 0)
            {
                var firstSearch = _searchEntries.Last().Timestamp;
                var daysSinceFirst = Math.Max(1, (DateTime.Now - firstSearch).TotalDays);
                pattern.AverageSearchesPerDay = _searchEntries.Count / daysSinceFirst;
            }

            var queryLengths = _searchEntries.Select(e => e.Query.Length).ToList();
            if (queryLengths.Count > 0)
            {
                pattern.AverageQueryLength = queryLengths.Average();
            }

            return pattern;
        }

        public void Clear()
        {
            _searchEntries.Clear();
            _queryFrequency.Clear();
            _categorySearchCount.Clear();
            _lastSearchByQuery.Clear();
        }

        public int Count => _searchEntries.Count;

        public bool HasSearchHistory => _searchEntries.Count > 0;
    }

    public class SearchPattern
    {
        public List<int> PreferredSearchHours { get; set; } = new List<int>();
        public List<DayOfWeek> PreferredSearchDays { get; set; } = new List<DayOfWeek>();
        public double AverageSearchesPerDay { get; set; }
        public double AverageQueryLength { get; set; }
    }
}