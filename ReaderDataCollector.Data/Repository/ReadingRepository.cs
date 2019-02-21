﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using ReaderDataCollector.Data.DataAccess;
using ReaderDataCollector.Data.Model;

namespace ReaderDataCollector.Data.Repository
{
    public interface IReadingRepository
    {
        Task<IEnumerable<Reading>> ReadingsAsync();

        Task SaveReading(Reading reading);

        Reading GetById(Guid Id);
    }

    public class ReadingRepository : IReadingRepository
    {
        private readonly Context _context;

        public ReadingRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reading>> ReadingsAsync()
        {
            try
            {
                var readings = await _context
                    .Readings        
                    .AsNoTracking()
                    .ToListAsync();

                var reads = await _context
                    .Reads
                    .AsNoTracking()
                    .ToListAsync();

                readings.ForEach(x =>
                {
                    x.TotalReads = reads
                    .Where(y => y.ReadingId == x.Id)
                    .Count();
                });

                return readings.AsEnumerable();
            }
            catch
            {
                return null;
            }
        }

        public async Task SaveReading(Reading reading)
        {
            if (reading != null)
            {
                reading.StartedDateTime = DateTime.UtcNow;

                if (reading.Id == Guid.Empty)
                {
                    reading.Id = Guid.NewGuid();
                }

                _context.Readings.Add(reading);
                await _context.SaveChangesAsync();
            }
        }

        public Reading GetById(Guid Id)
        {
            var reading = _context.Readings.FirstOrDefault(x => x.Id == Id);

            return reading;
        }
    }
}
