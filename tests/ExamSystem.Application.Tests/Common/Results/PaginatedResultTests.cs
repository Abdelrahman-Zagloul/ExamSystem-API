using ExamSystem.Application.Common.Results;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Common.Results
{
    [Trait("Category", "Application.Results.PaginatedResult")]
    public class PaginatedResultTests
    {
        [Fact]
        public void CreatePaginatedResult_ShouldSetBasicPaginationProperties()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3 };
            var totalCount = 10;
            var pageNumber = 1;
            var pageSize = 3;
            var baseUrl = "/api/items";
            var queryParams = new Dictionary<string, string>();

            // Act
            var result = PaginatedResult<int>.CreatePaginatedResult(
                items,
                totalCount,
                pageNumber,
                pageSize,
                baseUrl,
                queryParams
            );

            // Assert
            result.Items.Should().BeEquivalentTo(items);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(3);
            result.TotalPages.Should().Be(4);
        }

        [Fact]
        public void CreatePaginatedResult_ShouldCreateNextPageUrl_WhenMorePagesExist()
        {
            // Arrange
            var items = new List<string> { "A", "B" };
            var baseUrl = "/api/items";
            var queryParams = new Dictionary<string, string>
            {
                ["search"] = "test"
            };

            // Act
            var result = PaginatedResult<string>.CreatePaginatedResult(
                items,
                totalCount: 10,
                pageNumber: 1,
                pageSize: 2,
                baseUrl,
                queryParams
            );

            // Assert
            result.NextPageUrl.Should().NotBeNull();
            result.NextPageUrl.Should().Contain("pageNumber=2");
            result.NextPageUrl.Should().Contain("pageSize=2");
            result.NextPageUrl.Should().Contain("search=test");
        }

        [Fact]
        public void CreatePaginatedResult_ShouldNotCreateNextPageUrl_OnLastPage()
        {
            // Arrange
            var items = new List<int> { 9, 10 };

            // Act
            var result = PaginatedResult<int>.CreatePaginatedResult(
                items,
                totalCount: 10,
                pageNumber: 5,
                pageSize: 2,
                baseUrl: "/api/items",
                queryParams: new Dictionary<string, string>()
            );

            // Assert
            result.NextPageUrl.Should().BeNull();
        }

        [Fact]
        public void CreatePaginatedResult_ShouldCreatePreviousPageUrl_WhenPageNumberGreaterThanOne()
        {
            // Arrange
            var items = new List<int> { 3, 4 };

            // Act
            var result = PaginatedResult<int>.CreatePaginatedResult(
                items,
                totalCount: 10,
                pageNumber: 2,
                pageSize: 2,
                baseUrl: "/api/items",
                queryParams: new Dictionary<string, string>()
            );

            // Assert
            result.PreviousPageUrl.Should().NotBeNull();
            result.PreviousPageUrl.Should().Contain("pageNumber=1");
            result.PreviousPageUrl.Should().Contain("pageSize=2");
        }

        [Fact]
        public void CreatePaginatedResult_ShouldNotCreatePreviousPageUrl_OnFirstPage()
        {
            // Arrange
            var items = new List<int> { 1, 2 };

            // Act
            var result = PaginatedResult<int>.CreatePaginatedResult(
                items,
                totalCount: 10,
                pageNumber: 1,
                pageSize: 2,
                baseUrl: "/api/items",
                queryParams: new Dictionary<string, string>()
            );

            // Assert
            result.PreviousPageUrl.Should().BeNull();
        }

        [Fact]
        public void CreatePaginatedResult_ShouldUrlEncodeQueryParameters()
        {
            // Arrange
            var items = new List<string> { "A" };
            var queryParams = new Dictionary<string, string>
            {
                ["search"] = "hello world"
            };

            // Act
            var result = PaginatedResult<string>.CreatePaginatedResult(
                items,
                totalCount: 5,
                pageNumber: 1,
                pageSize: 1,
                baseUrl: "/api/items",
                queryParams
            );

            // Assert
            result.NextPageUrl.Should().Contain("search=hello%20world");
        }
    }
}
