using HelpdeskSystem.Models;

namespace HelpdeskSystem.Services
{
    public class TicketWorkflowService : ITicketWorkflowService
    {
        public bool CanChangeStatus(TicketStatus currentStatus,TicketStatus newStatus)
        {
            return GetNextStatuses(currentStatus)
                .Contains(newStatus);
        }

        public List<TicketStatus> GetNextStatuses(TicketStatus currentStatus)
        {
            return currentStatus switch
            {
                TicketStatus.New =>
                    new List<TicketStatus>
                    {
                        TicketStatus.Assigned
                    },

                TicketStatus.Assigned =>
                    new List<TicketStatus>
                    {
                        TicketStatus.InProgress
                    },

                TicketStatus.InProgress =>
                    new List<TicketStatus>
                    {
                        TicketStatus.Resolved
                    },

                TicketStatus.Resolved =>
                    new List<TicketStatus>
                    {
                        TicketStatus.InProgress, TicketStatus.Closed
                    },

                TicketStatus.Closed =>
                    new List<TicketStatus>(),

                _ => new List<TicketStatus>()
            };
        }
    }
}