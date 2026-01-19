using AutoMapper;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Users;
using ExamSystem.Domain.Interfaces;
using MediatR;

namespace ExamSystem.Application.Features.Exams.Commands.CreateExam
{
    public class CreateExamCommandHandler : IRequestHandler<CreateExamCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public CreateExamCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(CreateExamCommand request, CancellationToken cancellationToken)
        {
            var doctorExist = await _unitOfWork.Repository<Doctor>()
                .AnyAsync(x => x.Id == _currentUser.UserId, cancellationToken);

            if (!doctorExist)
                return Error.Unauthorized();

            var exam = _mapper.Map<Exam>(request);
            exam.DoctorId = _currentUser.UserId!;

            await _unitOfWork.Repository<Exam>().AddAsync(exam, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok("Exam created Successfully");
        }
    }
}
