using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Interfaces;
using ExamSystem.Infrastructure.Jobs;
using MockQueryable;
using Moq;
using System.Linq.Expressions;

namespace ExamSystem.Infrastructure.Tests.Jobs
{

    [Trait("Category", "Infrastructure.Jobs.CalculateExamResult")]
    public class CalculateExamResultJobTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<ExamResult>> _examResultRepoMock;
        private readonly Mock<IGenericRepository<StudentAnswer>> _answerRepoMock;
        private readonly Mock<IGenericRepository<Question>> _questionRepoMock;
        private readonly CalculateExamResultJob _job;
        public CalculateExamResultJobTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _examResultRepoMock = new Mock<IGenericRepository<ExamResult>>();
            _answerRepoMock = new Mock<IGenericRepository<StudentAnswer>>();
            _questionRepoMock = new Mock<IGenericRepository<Question>>();

            _unitOfWorkMock.Setup(u => u.Repository<ExamResult>()).Returns(_examResultRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<StudentAnswer>()).Returns(_answerRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Question>()).Returns(_questionRepoMock.Object);
            _job = new CalculateExamResultJob(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturn_WhenResultAlreadyCalculated()
        {
            // Arrange
            _examResultRepoMock
                .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<ExamResult, bool>>>(), default))
                .ReturnsAsync(true);

            // Act
            await _job.ExecuteAsync(1, "student1");

            // Assert
            _examResultRepoMock.Verify(r => r.AddAsync(It.IsAny<ExamResult>(), default), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturn_WhenNoStudentAnswers()
        {
            // Arrange
            _examResultRepoMock
                .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<ExamResult, bool>>>(), default))
                .ReturnsAsync(false);

            var answers = new List<StudentAnswer>().BuildMock();

            _answerRepoMock
                .Setup(r => r.GetAsQuery(false))
                .Returns(answers);

            // Act
            await _job.ExecuteAsync(1, "student1");

            // Assert
            _examResultRepoMock.Verify(r => r.AddAsync(It.IsAny<ExamResult>(), default), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCalculateScoreCorrectlyAndSaveResult()
        {
            // Arrange
            _examResultRepoMock
                .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<ExamResult, bool>>>(), default))
                .ReturnsAsync(false);

            var answers = new List<StudentAnswer>
            {
                new StudentAnswer(1,"student_id1",1,10),
                new StudentAnswer(1,"student_id1",2,20),
            };

            _answerRepoMock
                .Setup(r => r.GetAsQuery(false))
                .Returns(answers.BuildMock());

            var questions = new List<Question>
            {
               new  Question { ExamId=1,Id=1,CorrectOptionId=10,QuestionMark=5},
               new  Question { ExamId=1,Id=2,CorrectOptionId=99,QuestionMark=5},
            };

            _questionRepoMock
                .Setup(r => r.GetAsQuery(true))
                .Returns(questions.BuildMock());

            // Act
            await _job.ExecuteAsync(1, "student_id1");

            // Assert
            _examResultRepoMock.Verify(r =>
                r.AddAsync(It.Is<ExamResult>(x =>
                    x.StudentId == "student_id1" &&
                    x.ExamId == 1 &&
                    x.Score == 5 &&
                    x.TotalMark == 10
                ), default),
                Times.Once);

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(default),
                Times.Once);
        }
    }
}
