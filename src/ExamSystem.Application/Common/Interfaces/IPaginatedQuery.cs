namespace ExamSystem.Application.Common.Interfaces
{
    public interface IPaginatedQuery
    {
        int PageNumber { get; }
        int PageSize { get; }
        string BaseUrl { get; }
    }
}
