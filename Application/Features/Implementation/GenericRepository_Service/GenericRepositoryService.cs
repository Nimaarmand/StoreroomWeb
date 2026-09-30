using Application.Features.Definition.Context;
using Application.Features.Definition.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Features.Implementation.GenericRepository_Service
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly IApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Ù‚ÙÙ„ Ù…Ø­Ø¯ÙˆØ¯ Ø¨Ù‡ scope Ø¬Ø§Ø±ÛŒ. Ú†ÙˆÙ† Ø¨Ù‡ ØµÙˆØ±Øª Scoped Ø«Ø¨Øª Ù…ÛŒâ€ŒØ´ÙˆØ¯ØŒ Ù‡Ø± Ø¯Ø±Ø®ÙˆØ§Ø³Øª ÙˆØ¨
        /// Ù‚ÙÙ„ Ù…Ø³ØªÙ‚Ù„ Ø®ÙˆØ¯ Ø±Ø§ Ø¯Ø§Ø±Ø¯ Ùˆ ØªØ±Ø§ÙÛŒÚ© Ù…ÙˆØ§Ø²ÛŒ Ø±ÙˆÛŒ Ù‡Ù… Ù‚ÙÙ„ Ù†Ù…ÛŒâ€ŒØ´ÙˆØ¯.
        /// </summary>
    

        public GenericRepository(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        public virtual async Task<IEnumerable<T>> TakeAsync(int count)
        {
            return await _dbSet.Take(count).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> TakeAsync(int skip, int take)
        {
            return await _dbSet.Skip(skip).Take(take).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            // Ø§Ú¯Ø± Ù‡Ù…ÛŒÙ† Ø±Ú©ÙˆØ±Ø¯ Ù¾ÛŒØ´â€ŒØªØ± Ø¯Ø± Ø§ÛŒÙ† scope Ø¨Ø§Ø±Ú¯Ø°Ø§Ø±ÛŒ Ø´Ø¯Ù‡ Ø¨Ø§Ø´Ø¯ØŒ Ù†Ù…ÙˆÙ†Ù‡
            // Ù‚Ø¯ÛŒÙ…ÛŒ tracked Ø§Ø³Øª Ùˆ attach Ú©Ø±Ø¯Ù† Ù†Ù…ÙˆÙ†Ù‡ ÙˆØ±ÙˆØ¯ÛŒÙ ØªØ§Ø²Ù‡ Ø¨Ø§ Ø®Ø·Ø§ÛŒ
            // Â«Ø¯Ùˆ Ù†Ù…ÙˆÙ†Ù‡ Ø¨Ø§ Ù‡Ù…Ø§Ù† Ú©Ù„ÛŒØ¯Â» Ø´Ú©Ø³Øª Ù…ÛŒâ€ŒØ®ÙˆØ±Ø¯. Ø§Ù„Ú¯ÙˆÛŒ Ø±Ø§ÛŒØ¬
            // Â«Ø§ÙˆÙ„ Ø±Ú©ÙˆØ±Ø¯ Ø±Ø§ Ø¨Ø®ÙˆØ§Ù†ØŒ Ø¨Ø¹Ø¯ Ù†Ø³Ø®Ù‡ Ø¬Ø¯ÛŒØ¯ Ø±Ø§ Update Ú©Ù†Â» Ø¯Ù‚ÛŒÙ‚Ø§Ù‹
            // Ù‡Ù…ÛŒÙ† Ù…Ø´Ú©Ù„ Ø±Ø§ Ù…ÛŒâ€ŒØ³Ø§Ø²Ø¯ØŒ Ù¾Ø³ Ù†Ù…ÙˆÙ†Ù‡ Ù‚Ø¯ÛŒÙ…ÛŒ Ø±Ø§ Ø¬Ø¯Ø§ Ù…ÛŒâ€ŒÚ©Ù†ÛŒÙ….
            DetachConflictingInstance(entity);
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        private void DetachConflictingInstance(T entity)
        {
            foreach (var entry in _context.ChangeTracker.Entries<T>().ToList())
            {
                if (ReferenceEquals(entry.Entity, entity)) continue;

                var key = entry.Metadata.FindPrimaryKey();
                if (key == null) continue;

                var sameKey = key.Properties.All(property =>
                    Equals(property.GetGetter().GetClrValue(entry.Entity),
                           property.GetGetter().GetClrValue(entity)));

                if (sameKey)
                    entry.State = EntityState.Detached;
            }
        }

        public virtual async Task<T> RemoveAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task RemoveByIdAsync(params object[] keyValues)
        {
            var entity = await _dbSet.FindAsync(keyValues);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
