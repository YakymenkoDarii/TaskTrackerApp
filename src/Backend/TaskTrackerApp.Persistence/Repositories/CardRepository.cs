using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly TaskTrackerDbContext _dbContext;

        public CardRepository(TaskTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Card> GetByIdAsync(int id)
        {
            return await _dbContext.Cards.FindAsync(id);
        }

        public async Task<IReadOnlyList<Card>> GetAllAsync()
        {
            return await _dbContext.Cards.ToListAsync();
        }

        public async Task<Card> AddAsync(Card entity)
        {
            await _dbContext.Cards.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Card entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Card entity)
        {
            _dbContext.Cards.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Card>> GetCardsByBoardIdAsync(int boardId)
        {
            return await _dbContext.Cards
                .Where(c => c.Id == boardId)
                .ToListAsync();
        }
    }
}
