using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Jobs;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using MediatR;

namespace ExamSystem.Application.Features.Exams.Commands.SubmitExam
{
    public class SubmitExamCommandHandler : IRequestHandler<SubmitExamCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobService _backgroundJobService;

        public SubmitExamCommandHandler(IUnitOfWork unitOfWork, IBackgroundJobService backgroundJobService)
        {
            _unitOfWork = unitOfWork;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<Result> Handle(SubmitExamCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var exam = await _unitOfWork.Repository<Exam>().FindAsync(cancellationToken, request.ExamId);
            if (exam == null)
                return Error.NotFound("ExamNotFound", "Exam with this id not found");

            if (exam.EndAt < now)
                return Error.Conflict("ExamFinished", "You cannot submit the exam after it has finished");

            var examSession = await _unitOfWork.Repository<ExamSession>()
                .FindAsync(cancellationToken, request.StudentId, request.ExamId);

            if (examSession == null)
                return Error.Conflict("ExamNotStarted", "You must start the exam before submitting");

            if (examSession.SubmittedAt != null)
                return Error.Conflict("ExamAlreadySubmitted", "You have already submitted this exam");


            var studentExamTime = (now - examSession.StartedAt).TotalMinutes;
            if (exam.DurationInMinutes < studentExamTime)
                return Error.Conflict("ExamTimeExceeded", "You exceeded the allowed exam duration");

            examSession.SubmittedAt = now;
            var studentAnswers = new List<StudentAnswer>();
            foreach (var answer in request.Answers)
            {
                studentAnswers.Add(new StudentAnswer
                {
                    ExamId = request.ExamId,
                    StudentId = request.StudentId,
                    QuestionId = answer.QuestionId,
                    SelectedOptionId = answer.SelectedOptionId,
                    EvaluationStatus = AnswerEvaluationStatus.Pending,
                });
            }

            await _unitOfWork.Repository<StudentAnswer>().AddRangeAsync(studentAnswers);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _backgroundJobService.Enqueue<ICalculateExamResultJob>(job => job.ExecuteAsync(request.ExamId, request.StudentId));
            return Result.Ok($"Exam submitted Successfully.Your result will be available at : {exam.EndAt}");
        }
    }
}
