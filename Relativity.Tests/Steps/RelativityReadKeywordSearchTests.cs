﻿using System.Collections.Generic;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Services.Search;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityReadKeywordSearchTests : StepTestBase<RelativityReadKeywordSearch, Entity>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                yield return new StepCase("Read Search",
                        TestHelpers.LogEntity(new RelativityReadKeywordSearch()
                        {
                            WorkspaceId = StaticHelpers.Constant(11),
                            SearchId = StaticHelpers.Constant(12)
                        }), Unit.Default,
                        "(ArtifactTypeID: 0 SearchContainer: (ArtifactID: 0 Name: \"\") Owner: (ArtifactID: 0 Name: \"\") SearchIndex: (ArtifactID: 0 Name: \"\") Includes: \"\" Scope: 0 SearchFolders: \"\" RequiresManualRun: False SearchCriteria: (Conditions: \"\" BooleanOperator: 0) Fields: \"\" Sorts: \"\" QueryHint: \"\" SortByRank: False SearchText: \"\" Keywords: \"\" Notes: \"\" RelativityApplications: \"\" SystemCreatedBy: \"\" SystemCreatedOn: \"\" SystemLastModifiedBy: \"\" SystemLastModifiedOn: \"\" Dashboard: \"\" ArtifactID: 12 Name: \"My Search\" SearchType: \"KeywordSearch\")"
                    )
                    .WithTestRelativitySettings()
                    .WithService(new MockSetup<IKeywordSearchManager, KeywordSearch>(
                        x => x.ReadSingleAsync(11, 12),
                        new KeywordSearch()
                        {
                            Name = "My Search",
                            ArtifactID = 12,
                        }
                    ));
            }
        }
    }
}