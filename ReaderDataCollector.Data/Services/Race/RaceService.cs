using ReaderDataCollector.Data.DataAccess;
using ReaderDataCollector.Data.Repository;
using ReaderDataCollector.Data.Model;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Services
{
    public interface IRaceService
    {
        Task SaveRaceAsync(Race race);
    }

    public class RaceService : IRaceService
    {
        private readonly Context _context;
        private readonly IReaderRepository _readerRepository;
        private readonly IReadingRepository _readingRepository;
        private readonly IReadRepository _readRepository;

        public RaceService(Context context)
        {
            _context = context;
            _readerRepository = new ReaderRepository(_context);
            _readingRepository = new ReadingRepository(_context);
            _readRepository = new ReadRepository(_context);
        }

        public async Task SaveRaceAsync(Race race)
        {
            await _readerRepository.SaveReader(race.Reader);
            await _readingRepository.SaveReading(race.Reading);

            await _readRepository.DeleteByReadingId(race.Reading.Id);
            await _readRepository.SaveReadRangeAsync(race.Reads);
        }
    }
}
