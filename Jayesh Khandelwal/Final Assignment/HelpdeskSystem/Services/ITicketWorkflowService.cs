namespace HelpdeskSystem.Services
{
    public interface ITicketWorkflowService
    {
        bool CanChangeStatus(Models.TicketStatus currentStatus,Models.TicketStatus newStatus);
    }
}
