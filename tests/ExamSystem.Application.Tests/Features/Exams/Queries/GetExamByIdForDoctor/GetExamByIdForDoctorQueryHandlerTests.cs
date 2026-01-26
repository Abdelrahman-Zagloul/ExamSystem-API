using AutoMapper;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Exams.Queries.GetExamByIdForDoctor;
using ExamSystem.Application.Tests.Features.Exams.Mapping;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace ExamSystem.Application.Tests.Features.Exams.Queries.GetExamByIdForDoctor
{
    [Trait("Category", "Application.Exam.GetExamByIdForDoctor.Handler")]
    public class GetExamByIdForDoctorQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly IMapper _mapper;

        private readonly GetExamByIdForDoctorQueryHandler _handler;

        public GetExamByIdForDoctorQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _mapper = MockHelper.CreateMappingProfile<TestExamMappingProfile>();
            _unitOfWorkMock.Setup(x => x.Repository<Exam>()).Returns(_examRepoMock.Object);
            _handler = new GetExamByIdForDoctorQueryHandler(_unitOfWorkMock.Object, _mapper);
        }

        private static GetExamByIdForDoctorQuery CreateQuery() => new("doctor-id", 1);

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenExamDoesNotExist()
        {
            // Arrange
            var query = CreateQuery();

            _examRepoMock.Setup(x => x.GetAsQuery(true))
                .Returns(new List<Exam>().BuildMock());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnForbidden_WhenDoctorIsNotOwnerOfExam()
        {
            // Arrange
            var query = CreateQuery();
            var exam = new Exam
            {
                Id = query.ExamId,
                DoctorId = "another-doctor"
            };

            _examRepoMock.Setup(x => x.GetAsQuery(true))
                .Returns(new List<Exam> { exam }.BuildMock());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.Forbidden);
        }

        [Fact]
        public async Task Handle_ShouldReturnExam_WhenDoctorIsOwner()
        {
            // Arrange
            var query = CreateQuery();
            var exam = new Exam
            {
                Id = query.ExamId,
                DoctorId = query.DoctorId
            };

            _examRepoMock.Setup(x => x.GetAsQuery(true))
                .Returns(new List<Exam> { exam }.BuildMock());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.DoctorId.Should().Be(query.DoctorId);
        }
    }
}
