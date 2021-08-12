﻿using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;
using Reductech.EDR.Core.Util;
using Relativity.Services.Objects.DataContracts;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class RelativityDeleteDocumentTests : StepTestBase<RelativityDeleteDocument, Entity>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Delete Document",
                    new Log<Entity>()
                    {
                        Value = new RelativityDeleteDocument()
                        {
                            Workspace        = new OneOfStep<int, StringStream>(Constant(11)),
                            ObjectArtifactId = Constant(22)
                        }
                    },
                    Unit.Default,
                    "(Report: (DeletedItems: [(ObjectTypeName: \"document\" Action: \"delete\" Count: 1 Connection: \"object\")]))"
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IObjectManager1, DeleteResult>(
                        x =>
                            x.DeleteAsync(
                                11,
                                It.Is<DeleteRequest>(dr => dr.Object.ArtifactID == 22),
                                It.IsAny<CancellationToken>()
                            ),
                        new DeleteResult()
                        {
                            Report = new DeleteReport()
                            {
                                DeletedItems = new List<DeleteItem>()
                                {
                                    new()
                                    {
                                        Action         = "delete",
                                        Connection     = "object",
                                        Count          = 1,
                                        ObjectTypeName = "document"
                                    }
                                }
                            }
                        }
                    )
                );
        }
    }
}

}
