namespace HelpdeskSystem.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalTickets { get; set; }
        public int NewTickets { get; set; }
        public int AssignedTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }

        public int HighPriorityTickets { get; set; }
        public int MediumPriorityTickets { get; set; }
        public int LowPriorityTickets { get; set; }

        public int OverdueTickets { get; set; }
    }
}