using HelpdeskSystem.Models;

namespace HelpdeskSystem.Services
{
    public class TicketWorkflowService : ITicketWorkflowService
    {
        public bool CanChangeStatus(TicketStatus currentStatus,TicketStatus newStatus)
        {
            if (currentStatus == TicketStatus.New &&   newStatus == TicketStatus.Assigned)
            {
                return true;
            }

            if (currentStatus == TicketStatus.Assigned && newStatus == TicketStatus.InProgress)
            {
                return true;
            }

            if (currentStatus == TicketStatus.InProgress && newStatus == TicketStatus.Resolved)
             {
                return true;
            }

            if (currentStatus == TicketStatus.Resolved && newStatus == TicketStatus.InProgress)
            {
                return true;
            }

            if (currentStatus == TicketStatus.Resolved && newStatus == TicketStatus.Closed)
            {
                return true;
            }
            return false;
        }
    }
}