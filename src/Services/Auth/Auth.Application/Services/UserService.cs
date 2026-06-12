using Auth.Application.Contracts;
using Auth.Application.DTOs.Users;
using Auth.Application.Mapping;
using Auth.Domain.Filters;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;

namespace Auth.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<UserSummaryDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _repository.GetById(id, cancellationToken);
            if (user is null) return Result.Failure<UserSummaryDTO>(Domain.Errors.UserErrors.NotFound(id));
            return Result.Success(user.ToSummary());
        }

        public async Task<Result<UserSummaryDTO>> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email)) return Result.Failure<UserSummaryDTO>(Domain.Errors.ContactErrors.EmailRequired);
            var user = await _repository.FindByEmail(email.Trim().ToLowerInvariant(), cancellationToken);
            if (user is null) return Result.Failure<UserSummaryDTO>(Domain.Errors.UserErrors.NotFound(0));
            return Result.Success(user.ToSummary());
        }

        public async Task<Result<IEnumerable<UserSummaryDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _repository.GetAll(cancellationToken);
            return Result.Success(users.Where(u => u is not null).Select(u => u!.ToSummary()));
        }

        public async Task<Result<(IEnumerable<UserSummaryDTO> Items, int TotalCount)>> GetByFilterAsync(UserFilter filter, CancellationToken cancellationToken = default)
        {
            var (items, total) = await _repository.FindByFilter(filter, cancellationToken);
            return Result.Success((items.Select(u => u.ToSummary()), total));
        }

        public async Task<Result> UpdateCurrentUserProfileAsync(int userId, UpdateCurrentUserProfileDTO dto, CancellationToken cancellationToken = default)
        {
            var user = await _repository.GetById(userId, cancellationToken);
            if (user is null) return Result.Failure(Domain.Errors.UserErrors.NotFound(userId));
            user.UpdateProfile(new Name(dto.FullName), new Contact(dto.Email, dto.Phone));
            await _repository.Update(user, cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeleteCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _repository.GetById(userId, cancellationToken);
            if (user is null) return Result.Failure(Domain.Errors.UserErrors.NotFound(userId));
            user.MarkAsDeleted();
            user.Deactivate();
            await _repository.Update(user, cancellationToken);
            return Result.Success();
        }
    }
}
