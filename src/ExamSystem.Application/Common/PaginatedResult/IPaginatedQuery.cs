namespace ExamSystem.Application.Common.PaginatedResult
{
    public interface IPaginatedQuery
    {
        int PageNumber { get; }
        int PageSize { get; }
        string BaseUrl { get; }
    }
}
