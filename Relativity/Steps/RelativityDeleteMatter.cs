﻿using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;

namespace Reductech.EDR.Connectors.Relativity.Steps;

/// <summary>
/// Delete a matter
/// </summary>
[SCLExample("RelativityDeleteMatter 123", ExecuteInTests = false)]
public class RelativityDeleteMatter : RelativityApiRequest<int, IMatterManager1, Unit, Unit>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityDeleteMatter, Unit>();

    /// <inheritdoc />
    public override Result<Unit, IErrorBuilder> ConvertOutput(Unit serviceOutput)
    {
        return serviceOutput;
    }

    /// <inheritdoc />
    public override async Task<Unit> SendRequest(
        IStateMonad stateMonad,
        IMatterManager1 service,
        int requestObject,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(requestObject);
        return Unit.Default;
    }

    /// <inheritdoc />
    public override Task<Result<int, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        return MatterArtifactId.Run(stateMonad, cancellation);
    }

    /// <summary>
    /// The artifact id of the matter to delete
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<int> MatterArtifactId { get; set; } = null!;
}
