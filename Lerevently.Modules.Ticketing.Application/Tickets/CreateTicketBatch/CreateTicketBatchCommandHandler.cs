using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Application.Abstractions.Data;
using Lerevently.Modules.Ticketing.Domain.Events;
using Lerevently.Modules.Ticketing.Domain.Orders;
using Lerevently.Modules.Ticketing.Domain.Tickets;

namespace Lerevently.Modules.Ticketing.Application.Tickets.CreateTicketBatch;

internal sealed class CreateTicketBatchCommandHandler(
    IOrderRepository orderRepository,
    ITicketTypeRepository ticketTypeRepository,
    ITicketRepository ticketRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateTicketBatchCommand>
{
    public async Task<Result> Handle(CreateTicketBatchCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(request.OrderId, cancellationToken);

        if (order is null) return Result.Failure(OrderErrors.NotFound(request.OrderId));

        var result = order.IssueTickets();

        if (result.IsFailure) return Result.Failure(result.Error);

        List<Ticket> tickets = [];
        foreach (var orderItem in order.OrderItems)
        {
            var ticketType = await ticketTypeRepository.GetAsync(orderItem.TicketTypeId, cancellationToken);

            if (ticketType is null) return Result.Failure(TicketTypeErrors.NotFound(orderItem.TicketTypeId));

            for (var i = 0; i < orderItem.Quantity; i++)
            {
                var ticket = Ticket.Create(order, ticketType);

                tickets.Add(ticket);
            }
        }

        ticketRepository.InsertRange(tickets);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}