using Microsoft.EntityFrameworkCore;
using PickMeApplication.RestaurantService.Domain.Entities;
using PickMeApplication.RestaurantService.Domain.Repositories;
using PickMeApplication.RestaurantService.Infrastructure.Data;

namespace PickMeApplication.RestaurantService.Infrastructure.Repositories;

public class BusinessHoursRepository : IBusinessHoursRepository
{
    private readonly RestaurantDbContext _context;

    public BusinessHoursRepository(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<BusinessHours?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessHours
            .Include(bh => bh.Restaurant)
            .FirstOrDefaultAsync(bh => bh.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<BusinessHours>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BusinessHours
            .Include(bh => bh.Restaurant)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BusinessHours businessHours, CancellationToken cancellationToken = default)
    {
        _context.BusinessHours.Add(businessHours);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(BusinessHours businessHours, CancellationToken cancellationToken = default)
    {
        _context.BusinessHours.Update(businessHours);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var businessHours = await _context.BusinessHours.FindAsync(new object[] { id }, cancellationToken);
        if (businessHours != null)
        {
            _context.BusinessHours.Remove(businessHours);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<BusinessHours>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessHours
            .Where(bh => bh.RestaurantId == restaurantId)
            .OrderBy(bh => bh.DayOfWeek)
            .ToListAsync(cancellationToken);
    }

    public async Task<BusinessHours?> GetRestaurantBusinessHoursForDayAsync(Guid restaurantId, int dayOfWeek, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessHours
            .FirstOrDefaultAsync(bh => bh.RestaurantId == restaurantId && bh.DayOfWeek == dayOfWeek, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<BusinessHours> businessHours, CancellationToken cancellationToken = default)
    {
        _context.BusinessHours.AddRange(businessHours);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<BusinessHours> businessHours, CancellationToken cancellationToken = default)
    {
        _context.BusinessHours.UpdateRange(businessHours);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        var businessHours = await _context.BusinessHours
            .Where(bh => bh.RestaurantId == restaurantId)
            .ToListAsync(cancellationToken);

        if (businessHours.Any())
        {
            _context.BusinessHours.RemoveRange(businessHours);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<BusinessHours>> GetOpenRestaurantsAtTimeAsync(DateTime dateTime, CancellationToken cancellationToken = default)
    {
        var dayOfWeek = (int)dateTime.DayOfWeek;
        var timeOnly = TimeOnly.FromDateTime(dateTime);

        var businessHours = await _context.BusinessHours
            .Include(bh => bh.Restaurant)
            .Where(bh => 
                bh.DayOfWeek == dayOfWeek && 
                bh.Status == Domain.Enums.BusinessDayStatus.Open &&
                bh.Restaurant.Status == Domain.Enums.RestaurantStatus.Active)
            .ToListAsync(cancellationToken);

        // Filter by time in memory (complex time logic)
        return businessHours.Where(bh => bh.IsOpenAt(timeOnly));
    }

    public async Task<bool> IsRestaurantOpenAtAsync(Guid restaurantId, DateTime dateTime, CancellationToken cancellationToken = default)
    {
        var dayOfWeek = (int)dateTime.DayOfWeek;
        var timeOnly = TimeOnly.FromDateTime(dateTime);

        var businessHours = await _context.BusinessHours
            .FirstOrDefaultAsync(bh => 
                bh.RestaurantId == restaurantId && 
                bh.DayOfWeek == dayOfWeek, 
                cancellationToken);

        return businessHours?.IsOpenAt(timeOnly) ?? false;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessHours.AnyAsync(bh => bh.Id == id, cancellationToken);
    }

    public async Task<bool> RestaurantHasBusinessHoursForDayAsync(Guid restaurantId, int dayOfWeek, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessHours.AnyAsync(bh => 
            bh.RestaurantId == restaurantId && bh.DayOfWeek == dayOfWeek, 
            cancellationToken);
    }
}