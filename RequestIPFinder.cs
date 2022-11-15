namespace personalweb;
using Microsoft.Extensions.Primitives;

public interface IRequestIPFinder
{
    string GetIP(HttpContext context);
}

public class RequestIPFinder : IRequestIPFinder
{
    public RequestIPFinder()
    {}
    public string GetIP(HttpContext context)
    {
        string? ip = SplitCsv(GetHeaderValue(context, "X-Forwarded-For")).FirstOrDefault();

        if(string.IsNullOrWhiteSpace(ip) && context.Connection.RemoteIpAddress != null)
            ip = context.Connection.RemoteIpAddress.ToString();

        if(string.IsNullOrWhiteSpace(ip))
            ip = GetHeaderValue(context, "REMOTE_ADDR");

        if(string.IsNullOrWhiteSpace(ip))
            ip = "unknown";

        return ip;
    }

    private static string GetHeaderValue(HttpContext context, string headerName)
    {
        string value = "";
        if (context.Request?.Headers?.TryGetValue(headerName, out StringValues values) ?? false)
        {
            value = values.ToString();   // writes out as Csv when there are multiple.
        }
        return value;
    }

    private static List<string> SplitCsv(string csvList)
    {
        if (string.IsNullOrWhiteSpace(csvList)) {
            return new List<string>();
        }
        else {
        return csvList
            .TrimEnd(',')
            .Split(',')
            .AsEnumerable<string>()
            .Select(s => s.Trim())
            .ToList();
        }
    }
}