using personalweb.Models;
using System.Collections.Generic;
using System.Linq;

namespace personalweb.DataAccess
{
    public class DataAccessProvider : IDataAccessProvider
    {
        private readonly PostgreSqlContext _context;

        public DataAccessProvider(PostgreSqlContext context)
        {
            _context = context;
        }

        public void AddSiteCountRecord(SiteCount siteCount)
        {
            _context.SiteCounts.Add(siteCount);
            _context.SaveChanges();
        }

        public List<SiteCount> GetSiteCounts()
        {
            return _context.SiteCounts.ToList();
        }

        public SiteCount GetSiteCountSingleRecord(int id)
        {
            return _context.SiteCounts.Where(s => s.Id ==id).AsEnumerable().DefaultIfEmpty( new SiteCount()).First();
        }

        public SiteCount GetSiteCountSingleRecord(string SiteKey)
        {
            return _context.SiteCounts.Where(s => s.SiteKey == SiteKey).AsEnumerable().DefaultIfEmpty( new SiteCount()).First();
        }

        public void TestConnection()
        {
            try
            {
                _context.SiteCounts.FirstOrDefault();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void UpdateSiteCountRecord(SiteCount siteCount)
        {
            if (!_context.SiteCounts.Any(s => s.SiteKey == siteCount.SiteKey))
            {
                _context.SiteCounts.Add(siteCount);
            }
            else
            {
                _context.SiteCounts.Update(siteCount);
            }
            _context.SaveChanges();
        }
    }
}