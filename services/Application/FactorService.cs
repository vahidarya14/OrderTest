﻿using Domain;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace Application;

public class FactorService( UserRepository userRepo ,FactorRepository factorRepo,OrderRepository orderRepo) : IFactorService
{

    public async Task<List<FactorListdto>> ToListAsync(long? CustomerId, int skip, int take)
    {
        var q = factorRepo.Include(x => x.Order).ThenInclude(x => x.Customer).AsQueryable();

        if (CustomerId.HasValue)
            q = q.Where(x => x.Order.CustomerId == CustomerId);

        return await q.AsNoTracking().OrderByDescending(x => x.DueDate)
            .Skip(skip)
            .Take(take)
            .Select(x =>FactorListdto.FromOrder(x))
            .ToListAsync();
    }

    public async Task<Factor> Create(long orderId, long customerId, DateTime dueDate)
    {
        var order = await orderRepo.FirstOrDefaultAsync(x => x.Id == orderId && x.CustomerId == customerId);
        if (order is null)
            throw new Exception("order not found");

        var newFactor = new Factor
        {
            OrderId = orderId,
            DueDate = dueDate,
            Amount = order.TotalAmount,
            Status = FactorStatus.Pending,
            InvoiceId = Guid.NewGuid(),
        };
        await factorRepo.AddAsync(newFactor);
        await factorRepo.SaveChangesAsync(CancellationToken.None);

        return newFactor;
    }

    public async Task<bool> Pay(Guid invoiceId, long customerId)
    {
        var customer = await userRepo.FirstOrDefaultAsync(x => x.Id == customerId);
        if (customer is null)
            throw new Exception("customer not found");

        var factor = await factorRepo.FirstOrDefaultAsync(x => x.InvoiceId == invoiceId && x.Order.CustomerId == customerId);
        if (factor is null)
            throw new Exception("factor not found");

        if (customer.WalletBalance < factor.Amount)
            throw new Exception("WalletBalance is les than factor Amount");

        factor.Status = FactorStatus.Paid;
        customer.WalletBalance -= factor.Amount;

        await factorRepo.SaveChangesAsync(CancellationToken.None);

        return true;
    }
}

public record FactorListdto
{
    public DateTime DueDate { get; set; }
    public FactorStatus Status { get; set; }
    public long Amount { get; set; }
    public string Product { get; set; }
    public int  Quantity { get; set; }
    public string UserName { get; set; }

    public static FactorListdto FromOrder(Factor x) =>
        new FactorListdto
        {
            DueDate = x.DueDate,
            Status = x.Status,
            Amount = x.Amount,
            Product = x.Order.Product,
            Quantity = x.Order.Quantity,
            UserName = x.Order.Customer.UserName
        };
}