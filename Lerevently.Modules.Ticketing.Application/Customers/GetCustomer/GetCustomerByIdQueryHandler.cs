using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Domain.Customers;

namespace Lerevently.Modules.Ticketing.Application.Customers.GetCustomer;

internal sealed class GetCustomerByIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetCustomerQuery, CustomerResponse>
{
    public async Task<Result<CustomerResponse>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionFactory.GetDbConnectionAsync();

        const string sql =
            $"""
             SELECT
                 "Id" AS {nameof(CustomerResponse.Id)},
                 "Email" AS {nameof(CustomerResponse.Email)},
                 "FirstName" AS {nameof(CustomerResponse.FirstName)},
                 "LastName" AS {nameof(CustomerResponse.LastName)}
             FROM ticketing."Customers"
             WHERE "Id" = @CustomerId
             """;

        var customer = await connection.QuerySingleOrDefaultAsync<CustomerResponse>(sql, request);

        if (customer is null) return Result.Failure<CustomerResponse>(CustomerErrors.NotFound(request.CustomerId));

        return customer;
    }
}