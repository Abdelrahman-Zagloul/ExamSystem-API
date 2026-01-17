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
                                                               string baseUrl)
        {
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            return new PaginatedResult<T>
            {
                Items = Items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                NextPageUrl = totalPages > pageNumber
                                ? CreatePageUrl(baseUrl, pageNumber + 1, pageSize) : null,
                PreviousPageUrl = pageNumber > 1
                                ? CreatePageUrl(baseUrl, pageNumber - 1, pageSize) : null
            };
        }

        private static string? CreatePageUrl(string baseUrl, int pageNumber, int pageSize)
            => $"{baseUrl}?pageNumber={pageNumber}&pageSize={pageSize}";
    }
}
