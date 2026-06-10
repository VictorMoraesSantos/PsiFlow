using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Requests.Commands;
using BuildingBlocks.CQRS.Requests.Queries;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed record ClinicalRecordBody(string? Name);
public sealed record EncryptedDraftBody(string? Ciphertext, string? Nonce, string? Tag);
public sealed record PublishVersionBody(string? Reason);
public sealed record GetClinicalRecordByPatientQuery(int PatientId, int TenantId) : IQuery<object>;
public sealed record CreateClinicalRecordCommand(int PatientId, int TenantId, string? Name) : ICommand<object>;
public sealed record GetClinicalRecordQuery(int RecordId, int TenantId) : IQuery<object>;
public sealed record GetAnamnesisQuery(int RecordId, int TenantId) : IQuery<object>;
public sealed record AutosaveAnamnesisCommand(int RecordId, int TenantId, string? Ciphertext, string? Nonce, string? Tag) : ICommand;
public sealed record PublishAnamnesisVersionCommand(int RecordId, int TenantId, int UserId, string? Reason) : ICommand<object>;
public sealed record GetEvolutionQuery(int SessionId, int TenantId) : IQuery<object>;
public sealed record AutosaveEvolutionCommand(int SessionId, int TenantId, string? Ciphertext, string? Nonce, string? Tag) : ICommand;
public sealed record PublishEvolutionVersionCommand(int SessionId, int TenantId, int UserId, string? Reason) : ICommand<object>;
public sealed record GetEvolutionVersionsQuery(int SessionId, int TenantId) : IQuery<object>;
public sealed record GetClinicalAuditLogQuery(int RecordId, int TenantId) : IQuery<object>;

public sealed class GetClinicalRecordByPatientQueryHandler(IClinicalRecordWorkflowService service) : IQueryHandler<GetClinicalRecordByPatientQuery, object> { public Task<Result<object>> Handle(GetClinicalRecordByPatientQuery q, CancellationToken ct) => service.GetRecordByPatientAsync(q.PatientId, q.TenantId, ct); }
public sealed class CreateClinicalRecordCommandHandler(IClinicalRecordWorkflowService service) : ICommandHandler<CreateClinicalRecordCommand, object> { public Task<Result<object>> Handle(CreateClinicalRecordCommand c, CancellationToken ct) => service.CreateRecordAsync(c.PatientId, c.TenantId, c.Name, ct); }
public sealed class GetClinicalRecordQueryHandler(IClinicalRecordWorkflowService service) : IQueryHandler<GetClinicalRecordQuery, object> { public Task<Result<object>> Handle(GetClinicalRecordQuery q, CancellationToken ct) => service.GetRecordAsync(q.RecordId, q.TenantId, ct); }
public sealed class GetAnamnesisQueryHandler(IClinicalRecordWorkflowService service) : IQueryHandler<GetAnamnesisQuery, object> { public Task<Result<object>> Handle(GetAnamnesisQuery q, CancellationToken ct) => service.GetAnamnesisAsync(q.RecordId, q.TenantId, ct); }
public sealed class AutosaveAnamnesisCommandHandler(IClinicalRecordWorkflowService service) : ICommandHandler<AutosaveAnamnesisCommand> { public Task<Result> Handle(AutosaveAnamnesisCommand c, CancellationToken ct) => service.AutosaveAnamnesisAsync(c.RecordId, c.TenantId, c.Ciphertext, c.Nonce, c.Tag, ct); }
public sealed class PublishAnamnesisVersionCommandHandler(IClinicalRecordWorkflowService service) : ICommandHandler<PublishAnamnesisVersionCommand, object> { public Task<Result<object>> Handle(PublishAnamnesisVersionCommand c, CancellationToken ct) => service.PublishAnamnesisVersionAsync(c.RecordId, c.TenantId, c.UserId, c.Reason, ct); }
public sealed class GetEvolutionQueryHandler(IClinicalRecordWorkflowService service) : IQueryHandler<GetEvolutionQuery, object> { public Task<Result<object>> Handle(GetEvolutionQuery q, CancellationToken ct) => service.GetEvolutionAsync(q.SessionId, q.TenantId, ct); }
public sealed class AutosaveEvolutionCommandHandler(IClinicalRecordWorkflowService service) : ICommandHandler<AutosaveEvolutionCommand> { public Task<Result> Handle(AutosaveEvolutionCommand c, CancellationToken ct) => service.AutosaveEvolutionAsync(c.SessionId, c.TenantId, c.Ciphertext, c.Nonce, c.Tag, ct); }
public sealed class PublishEvolutionVersionCommandHandler(IClinicalRecordWorkflowService service) : ICommandHandler<PublishEvolutionVersionCommand, object> { public Task<Result<object>> Handle(PublishEvolutionVersionCommand c, CancellationToken ct) => service.PublishEvolutionVersionAsync(c.SessionId, c.TenantId, c.UserId, c.Reason, ct); }
public sealed class GetEvolutionVersionsQueryHandler(IClinicalRecordWorkflowService service) : IQueryHandler<GetEvolutionVersionsQuery, object> { public Task<Result<object>> Handle(GetEvolutionVersionsQuery q, CancellationToken ct) => service.GetEvolutionVersionsAsync(q.SessionId, q.TenantId, ct); }
public sealed class GetClinicalAuditLogQueryHandler(IClinicalRecordWorkflowService service) : IQueryHandler<GetClinicalAuditLogQuery, object> { public Task<Result<object>> Handle(GetClinicalAuditLogQuery q, CancellationToken ct) => service.GetAuditLogAsync(q.RecordId, q.TenantId, ct); }
