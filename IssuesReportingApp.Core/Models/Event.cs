using System;

namespace IssuesReportingApp.Models
{
    public class Event : IComparable<Event>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string Organizer { get; set; }
        public int Priority { get; set; }
        public bool IsAnnouncement { get; set; }
        public string ContactInfo { get; set; }
        public decimal? TicketPrice { get; set; }
        public int MaxAttendees { get; set; }
        public int CurrentAttendees { get; set; }
        public DateTime CreatedDate { get; set; }

        public Event()
        {
            CreatedDate = DateTime.Now;
            CurrentAttendees = 0;
            Priority = 2;
        }

        public Event(string title, string description, DateTime startDate, DateTime endDate,
                    string location, string category, string organizer, int priority = 2)
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Location = location;
            Category = category;
            Organizer = organizer;
            Priority = priority;
            CreatedDate = DateTime.Now;
            CurrentAttendees = 0;
        }

        public int CompareTo(Event other)
        {
            if (other == null) return 1;
            int priorityComparison = Priority.CompareTo(other.Priority);
            if (priorityComparison != 0)
                return priorityComparison;
            return StartDate.CompareTo(other.StartDate);
        }

        public bool IsUpcoming => StartDate > DateTime.Now;
        public bool IsOngoing => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
        public bool IsCompleted => EndDate < DateTime.Now;

        public string FormattedDateRange
        {
            get
            {
                if (StartDate.Date == EndDate.Date)
                    return $"{StartDate:MMM dd, yyyy} ({StartDate:HH:mm} - {EndDate:HH:mm})";
                else
                    return $"{StartDate:MMM dd, yyyy HH:mm} - {EndDate:MMM dd, yyyy HH:mm}";
            }
        }

        public string PriorityText
        {
            get
            {
                return Priority switch
                {
                    1 => "High",
                    2 => "Medium",
                    3 => "Low",
                    _ => "Unknown"
                };
            }
        }

        public override string ToString()
        {
            return $"{Title} - {FormattedDateRange} ({Category})";
        }
    }
}