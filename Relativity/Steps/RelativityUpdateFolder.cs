﻿using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using OneOf;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

/// <summary>
/// Update a Relativity folder
/// </summary>
[SCLExample(
    "RelativityUpdateFolder Workspace: 11 FolderId: 22 FolderName: \"NewName\"",
    ExecuteInTests = false
)]
public sealed class
    RelativityUpdateFolder : RelativityApiRequest<(Folder folder, int workspaceId), IFolderManager1,
        Unit, Unit>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<RelativityUpdateFolder, Unit>();

    /// <inheritdoc />
    public override Result<Unit, IErrorBuilder> ConvertOutput(Unit serviceOutput)
    {
        return serviceOutput;
    }

    /// <inheritdoc />
    public override async Task<Unit> SendRequest(
        IStateMonad stateMonad,
        IFolderManager1 service,
        (Folder folder, int workspaceId) requestObject,
        CancellationToken cancellationToken)
    {
        await service.UpdateSingleAsync(requestObject.workspaceId, requestObject.folder);
        return Unit.Default;
    }

    /// <inheritdoc />
    public override async Task<Result<(Folder folder, int workspaceId), IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        var r = await stateMonad.RunStepsAsync(
            Workspace.WrapWorkspace(stateMonad, TextLocation),
            FolderId,
            FolderName.WrapStringStream(),
            cancellation
        );

        if (r.IsFailure)
            return r.ConvertFailure<(Folder folder, int workspaceId)>();

        var (workspaceId, folderId, folderName) = r.Value;

        var folder = new Folder { Name = folderName, ArtifactID = folderId };

        return (folder, workspaceId);
    }

    /// <summary>
    /// The Workspace containing the folder.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The Id of the folder you want to update
    /// </summary>
    [StepProperty(2)]
    [Required]
    public IStep<int> FolderId { get; set; } = null!;

    /// <summary>
    /// The new name of the folder.
    /// </summary>
    [StepProperty(3)]
    [Alias("Name")]
    [Required]
    public IStep<StringStream> FolderName { get; set; } = null!;
}

}
