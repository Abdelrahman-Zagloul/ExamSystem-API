namespace ExamSystem.Application.Common.Results
{
    public class PaginatedResult<T>
    {
        private PaginatedResult() { }

        public IReadOnlyList<T> Items { get; init; } = [];
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalPages { get; init; }
        public string? NextPageUrl { get; init; }
        public string? PreviousPageUrl { get; init; }

        public static PaginatedResult<T> CreatePaginatedResult(IReadOnlyList<T> Items,
                                                               int totalCount,
                                                               int pageNumber,
                                                               int pageSize,
                                                               string baseUrl,
                                                               Dictionary<string, string> queryParams)
        {
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            return new PaginatedResult<T>
            {
                Items = Items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                NextPageUrl = totalPages > pageNumber
                                ? CreatePageUrl(baseUrl, pageNumber + 1, pageSize, queryParams) : null,
                PreviousPageUrl = pageNumber > 1
                                ? CreatePageUrl(baseUrl, pageNumber - 1, pageSize, queryParams) : null
            };
        }

        private static string? CreatePageUrl(string baseUrl, int pageNumber, int pageSize, Dictionary<string, string> queryParams)
        {
            var query = new Dictionary<string, string>(queryParams)
            {
                ["pageNumber"] = pageNumber.ToString(),
                ["pageSize"] = pageSize.ToString()
            };

            var queryString = string.Join("&", query.Select(q => $"{q.Key}={Uri.EscapeDataString(q.Value!)}"));
            return baseUrl + "?" + queryString;
        }
    }
}
