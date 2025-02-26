// Ambev.DeveloperEvaluation.ORM/Repositories/SaleRepository.cs
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;
    
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }
    
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }
    
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
    
    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }
    
    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        var exists = await _context.Sales.AnyAsync(s => s.Id == sale.Id, cancellationToken);
        if (!exists)
        {
            throw new KeyNotFoundException($"Sale with ID {sale.Id} does not exist");
        }
        
        _context.ChangeTracker.Clear();
        
        var existingItems = await _context.SaleItems
            .Where(si => si.SaleId == sale.Id)
            .ToListAsync(cancellationToken);
            
        if (existingItems.Any())
        {
            _context.SaleItems.RemoveRange(existingItems);
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        _context.Sales.Update(sale);
        
        foreach (var item in sale.Items)
        {
            _context.Entry(item).State = EntityState.Added;
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return sale;
    }
    
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales.FindAsync(new object[] { id }, cancellationToken);
        if (sale == null)
            return false;
            
        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    
    public async Task<IEnumerable<Sale>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales.CountAsync(cancellationToken);
    }
}