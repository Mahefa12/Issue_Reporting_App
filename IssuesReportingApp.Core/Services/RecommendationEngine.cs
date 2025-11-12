using System;
using System.Collections.Generic;
using System.Linq;
using IssuesReportingApp.Models;
using IssuesReportingApp.DataStructures;

namespace IssuesReportingApp.Services
{
    public class RecommendationResult : IComparable<RecommendationResult>
    {
        public Event Event { get; set; }
        public double Score { get; set; }
        public string Reason { get; set; }
        public RecommendationType Type { get; set; }

        public RecommendationResult(Event eventItem, double score, string reason, RecommendationType type)
        {
            Event = eventItem;
            Score = score;
            Reason = reason;
            Type = type;
        }

        public int CompareTo(RecommendationResult other)
        {
            if (other == null) return 1;
            return Score.CompareTo(other.Score);
        }
    }

    public enum RecommendationType
    {
        ContentBased,
        Collaborative,
        Popular,
        Trending,
        Similar,
        CategoryBased,
        LocationBased,
        TimeBased,
        Serendipity
    }

    public class RecommendationEngine
    {
        private readonly UserPreferences _userPreferences;
        private readonly SearchHistory _searchHistory;
        private readonly CustomTrie _searchTrie;
        private readonly List<Event> _allEvents;
        private readonly Dictionary<string, List<Event>> _eventsByCategory;
        private readonly Dictionary<string, List<Event>> _eventsByLocation;
        private readonly CustomPriorityQueue<Event> _eventPriorityQueue;

        public RecommendationEngine(UserPreferences userPreferences, SearchHistory searchHistory, List<Event> events)
        {
            _userPreferences = userPreferences ?? new UserPreferences();
            _searchHistory = searchHistory ?? new SearchHistory();
            _searchTrie = new CustomTrie();
            _allEvents = events ?? new List<Event>();
            _eventsByCategory = new Dictionary<string, List<Event>>();
            _eventsByLocation = new Dictionary<string, List<Event>>();
            _eventPriorityQueue = new CustomPriorityQueue<Event>();

            InitializeDataStructures();
        }

        private void InitializeDataStructures()
        {
            foreach (var eventItem in _allEvents)
            {
                if (!string.IsNullOrWhiteSpace(eventItem.Title))
                {
                    _searchTrie.Insert(eventItem.Title);
                }
                if (!string.IsNullOrWhiteSpace(eventItem.Description))
                {
                    var words = eventItem.Description.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words.Where(w => w.Length > 3))
                    {
                        _searchTrie.Insert(word);
                    }
                }
            }

            foreach (var eventItem in _allEvents)
            {
                if (!string.IsNullOrWhiteSpace(eventItem.Category))
                {
                    if (!_eventsByCategory.ContainsKey(eventItem.Category))
                        _eventsByCategory[eventItem.Category] = new List<Event>();
                    _eventsByCategory[eventItem.Category].Add(eventItem);
                }

                if (!string.IsNullOrWhiteSpace(eventItem.Location))
                {
                    if (!_eventsByLocation.ContainsKey(eventItem.Location))
                        _eventsByLocation[eventItem.Location] = new List<Event>();
                    _eventsByLocation[eventItem.Location].Add(eventItem);
                }
            }
        }

        public List<RecommendationResult> GetRecommendations(int maxRecommendations = 10)
        {
            var recommendations = new List<RecommendationResult>();

            recommendations.AddRange(GetContentBasedRecommendations(maxRecommendations / 4));
            recommendations.AddRange(GetCategoryBasedRecommendations(maxRecommendations / 4));
            recommendations.AddRange(GetPopularRecommendations(maxRecommendations / 4));
            recommendations.AddRange(GetSerendipityRecommendations(maxRecommendations / 4));

            var uniqueRecommendations = recommendations
                .GroupBy(r => r.Event.Id)
                .Select(g => g.OrderByDescending(r => r.Score).First())
                .OrderByDescending(r => r.Score)
                .Take(maxRecommendations)
                .ToList();

            return uniqueRecommendations;
        }

        private List<RecommendationResult> GetContentBasedRecommendations(int count)
        {
            var recommendations = new List<RecommendationResult>();
            var viewedEventIds = new HashSet<int>(_userPreferences.ViewedEventIds);

            foreach (var eventItem in _allEvents.Where(e => !viewedEventIds.Contains(e.Id)))
            {
                double score = CalculateContentBasedScore(eventItem);
                if (score > 0.3)
                {
                    recommendations.Add(new RecommendationResult(
                        eventItem,
                        score,
                        "Based on your interests and filter usage",
                        RecommendationType.ContentBased));
                }
            }

            return recommendations.OrderByDescending(r => r.Score).Take(count).ToList();
        }

        private double CalculateContentBasedScore(Event eventItem)
        {
            double score = 0.0;

            if (!string.IsNullOrWhiteSpace(eventItem.Category))
            {
                double categoryAffinity = _userPreferences.GetCategoryAffinity(eventItem.Category);
                score += categoryAffinity * 0.4;
            }

            if (!string.IsNullOrWhiteSpace(eventItem.Location))
            {
                double locationAffinity = _userPreferences.GetLocationAffinity(eventItem.Location);
                score += locationAffinity * 0.2;
            }

            double timeAffinity = 0.0;
            if (_userPreferences.TimePreferences.ContainsKey(eventItem.StartDate.Hour))
            {
                int totalTimeInteractions = _userPreferences.TimePreferences.Values.Sum();
                if (totalTimeInteractions > 0)
                {
                    timeAffinity = (double)_userPreferences.TimePreferences[eventItem.StartDate.Hour] / totalTimeInteractions;
                }
            }
            score += timeAffinity * 0.15;

            double dayAffinity = 0.0;
            if (_userPreferences.DayPreferences.ContainsKey(eventItem.StartDate.DayOfWeek))
            {
                int totalDayInteractions = _userPreferences.DayPreferences.Values.Sum();
                if (totalDayInteractions > 0)
                {
                    dayAffinity = (double)_userPreferences.DayPreferences[eventItem.StartDate.DayOfWeek] / totalDayInteractions;
                }
            }
            score += dayAffinity * 0.15;

            double searchRelevance = CalculateSearchRelevance(eventItem);
            score += searchRelevance * 0.15;

            // Apply explicit feedback boost/penalty
            score += CalculateFeedbackBoost(eventItem);

            return Math.Min(1.0, Math.Max(0.0, score));
        }

        private double CalculateSearchRelevance(Event eventItem)
        {
            double relevance = 0.0;

            var categoryFreq = _searchHistory.GetCategorySearchFrequency();
            if (!string.IsNullOrWhiteSpace(eventItem.Category) && categoryFreq.Count > 0)
            {
                if (categoryFreq.TryGetValue(eventItem.Category, out int catCount))
                {
                    int totalCat = categoryFreq.Values.Sum();
                    if (totalCat > 0)
                    {
                        double categoryScore = (double)catCount / totalCat;
                        relevance += categoryScore * 0.7;
                    }
                }
            }

            var recentFilters = _searchHistory.GetRecentSearches(50);
            int matchCount = 0;
            int totalDateEntries = 0;
            foreach (var entry in recentFilters)
            {
                if (entry.StartDate.HasValue && entry.EndDate.HasValue)
                {
                    totalDateEntries++;
                    var start = entry.StartDate.Value.Date;
                    var end = entry.EndDate.Value.Date;
                    var eventDate = eventItem.StartDate.Date;
                    if (eventDate >= start && eventDate <= end)
                    {
                        matchCount++;
                    }
                }
            }

            if (totalDateEntries > 0)
            {
                double dateScore = (double)matchCount / totalDateEntries;
                relevance += dateScore * 0.3;
            }

            return Math.Min(1.0, relevance);
        }

        private List<RecommendationResult> GetCategoryBasedRecommendations(int count)
        {
            var recommendations = new List<RecommendationResult>();
            var viewedEventIds = new HashSet<int>(_userPreferences.ViewedEventIds);
            var topCategories = _userPreferences.GetTopCategories(5);

            foreach (var category in topCategories)
            {
                if (_eventsByCategory.ContainsKey(category))
                {
                    var categoryEvents = _eventsByCategory[category]
                        .Where(e => !viewedEventIds.Contains(e.Id))
                        .OrderByDescending(e => e.Priority)
                        .ThenBy(e => e.StartDate)
                        .Take(count / topCategories.Count + 1);

                    foreach (var eventItem in categoryEvents)
                    {
                        double score = _userPreferences.GetCategoryAffinity(category) * 0.8 +
                                     (eventItem.Priority / 5.0) * 0.2;

                        recommendations.Add(new RecommendationResult(
                            eventItem,
                            score,
                            $"Popular in {category} category",
                            RecommendationType.CategoryBased));
                    }
                }
            }

            return recommendations.OrderByDescending(r => r.Score).Take(count).ToList();
        }

        private List<RecommendationResult> GetPopularRecommendations(int count)
        {
            var recommendations = new List<RecommendationResult>();
            var viewedEventIds = new HashSet<int>(_userPreferences.ViewedEventIds);

            var popularEvents = _allEvents
                .Where(e => !viewedEventIds.Contains(e.Id))
                .Where(e => e.StartDate >= DateTime.Now)
                .OrderByDescending(e => e.Priority)
                .ThenBy(e => e.StartDate)
                .Take(count * 2);

            foreach (var eventItem in popularEvents.Take(count))
            {
                double score = (eventItem.Priority / 5.0) * 0.6 +
                              GetTimelinessScore(eventItem) * 0.4;

                recommendations.Add(new RecommendationResult(
                    eventItem,
                    score,
                    "Popular upcoming event",
                    RecommendationType.Popular));
            }

            return recommendations;
        }

        private List<RecommendationResult> GetSerendipityRecommendations(int count)
        {
            var recommendations = new List<RecommendationResult>();
            var viewedEventIds = new HashSet<int>(_userPreferences.ViewedEventIds);
            var userCategories = new HashSet<string>(_userPreferences.GetTopCategories(10));

            var unexploredEvents = _allEvents
                .Where(e => !viewedEventIds.Contains(e.Id))
                .Where(e => !userCategories.Contains(e.Category))
                .Where(e => e.StartDate >= DateTime.Now)
                .OrderBy(e => Guid.NewGuid())
                .Take(count);

            foreach (var eventItem in unexploredEvents)
            {
                double score = 0.5 + (eventItem.Priority / 5.0) * 0.3 + GetTimelinessScore(eventItem) * 0.2;

                recommendations.Add(new RecommendationResult(
                    eventItem,
                    score,
                    "Discover something new",
                    RecommendationType.Serendipity));
            }

            return recommendations;
        }

        private double GetTimelinessScore(Event eventItem)
        {
            var daysUntilEvent = (eventItem.StartDate - DateTime.Now).TotalDays;
            if (daysUntilEvent < 0) return 0.0;
            if (daysUntilEvent <= 7) return 1.0;
            if (daysUntilEvent <= 30) return 0.8;
            if (daysUntilEvent <= 90) return 0.6;
            return 0.3;
        }

        public List<string> GetSearchSuggestions(string input, int maxSuggestions = 5)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return _searchHistory.GetMostFrequentQueries(maxSuggestions);
            }

            var suggestions = new List<string>();
            var trieSuggestions = _searchTrie.GetSuggestions(input, maxSuggestions / 2);
            suggestions.AddRange(trieSuggestions);

            if (suggestions.Count < maxSuggestions)
            {
                var fuzzySuggestions = _searchTrie.GetFuzzySuggestions(input, 2, maxSuggestions - suggestions.Count);
                suggestions.AddRange(fuzzySuggestions.Where(s => !suggestions.Contains(s)));
            }

            if (suggestions.Count < maxSuggestions)
            {
                var relatedQueries = _searchHistory.GetRelatedQueries(input, maxSuggestions - suggestions.Count);
                suggestions.AddRange(relatedQueries.Where(s => !suggestions.Contains(s)));
            }

            return suggestions.Take(maxSuggestions).ToList();
        }

        public List<RecommendationResult> GetSimilarEvents(Event targetEvent, int count = 5)
        {
            var recommendations = new List<RecommendationResult>();
            var viewedEventIds = new HashSet<int>(_userPreferences.ViewedEventIds);

            foreach (var eventItem in _allEvents.Where(e => e.Id != targetEvent.Id && !viewedEventIds.Contains(e.Id)))
            {
                double similarity = CalculateEventSimilarity(targetEvent, eventItem);
                if (similarity > 0.3)
                {
                    recommendations.Add(new RecommendationResult(
                        eventItem,
                        similarity,
                        $"Similar to {targetEvent.Title}",
                        RecommendationType.Similar));
                }
            }

            return recommendations.OrderByDescending(r => r.Score).Take(count).ToList();
        }

        private double CalculateEventSimilarity(Event event1, Event event2)
        {
            double similarity = 0.0;
            if (event1.Category == event2.Category)
                similarity += 0.4;
            if (event1.Location == event2.Location)
                similarity += 0.2;
            if (event1.StartDate.DayOfWeek == event2.StartDate.DayOfWeek)
                similarity += 0.1;
            double priorityDiff = Math.Abs(event1.Priority - event2.Priority) / 5.0;
            similarity += (1.0 - priorityDiff) * 0.1;
            similarity += CalculateTextSimilarity(event1.Title + " " + event1.Description,
                                                event2.Title + " " + event2.Description) * 0.2;
            return Math.Min(1.0, similarity);
        }

        private double CalculateTextSimilarity(string text1, string text2)
        {
            if (string.IsNullOrWhiteSpace(text1) || string.IsNullOrWhiteSpace(text2))
                return 0.0;
            var words1 = new HashSet<string>(text1.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries));
            var words2 = new HashSet<string>(text2.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries));
            var intersection = words1.Intersect(words2).Count();
            var union = words1.Union(words2).Count();
            return union > 0 ? (double)intersection / union : 0.0;
        }

        public void UpdateSearchTrie(string query)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                _searchTrie.Insert(query);
                var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words.Where(w => w.Length > 2))
                {
                    _searchTrie.Insert(word);
                }
            }
        }

        public Dictionary<string, double> GetCategoryRecommendationScores()
        {
            var scores = new Dictionary<string, double>();
            var userCategories = _userPreferences.GetTopCategories(10);

            foreach (var category in _eventsByCategory.Keys)
            {
                double score = 0.0;
                if (userCategories.Contains(category))
                {
                    score = _userPreferences.GetCategoryAffinity(category);
                }
                else
                {
                    score = 0.3;
                }
                scores[category] = score;
            }
            return scores;
        }

        public void RecordFeedback(int eventId, bool liked)
        {
            var evt = _allEvents.FirstOrDefault(e => e.Id == eventId);
            if (evt == null) return;
            _userPreferences.RecordFeedback(evt, liked);
        }

        private double CalculateFeedbackBoost(Event e)
        {
            double boost = 0.0;

            // Positive signals: similarity to liked categories/locations
            var likedEventIds = _userPreferences.LikedEventIds;
            if (likedEventIds.Count > 0)
            {
                var likedEvents = _allEvents.Where(ev => likedEventIds.Contains(ev.Id)).ToList();
                if (!string.IsNullOrWhiteSpace(e.Category) && likedEvents.Any(le => le.Category == e.Category))
                {
                    boost += 0.25;
                }
                if (!string.IsNullOrWhiteSpace(e.Location) && likedEvents.Any(le => le.Location == e.Location))
                {
                    boost += 0.15;
                }
            }

            // Negative signals: matches disliked categories/locations
            var dislikedEventIds = _userPreferences.DislikedEventIds;
            if (dislikedEventIds.Count > 0)
            {
                var dislikedEvents = _allEvents.Where(ev => dislikedEventIds.Contains(ev.Id)).ToList();
                if (!string.IsNullOrWhiteSpace(e.Category) && dislikedEvents.Any(de => de.Category == e.Category))
                {
                    boost -= 0.30;
                }
                if (!string.IsNullOrWhiteSpace(e.Location) && dislikedEvents.Any(de => de.Location == e.Location))
                {
                    boost -= 0.20;
                }
            }

            return boost;
        }

        public List<Event> GetAllEvents()
        {
            return _allEvents
                .OrderBy(ev => ev.StartDate)
                .ThenByDescending(ev => ev.Priority)
                .ToList();
        }

        // Announcements helpers
        public List<Event> GetAnnouncements(int max = 10)
        {
            var announcements = _allEvents
                .Where(e => e.IsAnnouncement)
                .OrderBy(e => e.StartDate)
                .ThenByDescending(e => e.CreatedDate)
                .Take(max)
                .ToList();
            return announcements;
        }
    }
}