using personalweb.Models;
using System.Collections.Generic;

namespace personalweb.DataAccess
{
    public interface IDataAccessProvider
    {
        void UpdateSiteCountRecord(SiteCount siteCount);
        void AddSiteCountRecord(SiteCount siteCount);
        SiteCount GetSiteCountSingleRecord(int id);
        SiteCount GetSiteCountSingleRecord(string SiteKey);
        List<SiteCount> GetSiteCounts();
        void TestConnection();
    }
}