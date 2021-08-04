﻿
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Relativity.Kepler.Transport;
using Relativity.Environment.V1.Matter;
using Relativity.Environment.V1.Matter.Models;
using Relativity.Environment.V1.Workspace.Models;
using Relativity.Environment.V1.Workspace;
using Relativity.Shared.V1.Models;

namespace Reductech.EDR.Connectors.Relativity.Managers
{
[GeneratedCode("CodeGenerator", "1")]
public class TemplateMatterManager : ManagerBase, IMatterManager
{
	public TemplateMatterManager(RelativitySettings relativitySettings, IFlurlClient flurlClient)
	:base(relativitySettings, flurlClient) { }
	
	/// <inheritdoc />
	protected override IEnumerable<string> Prefixes
	{
		get
		{
			yield return $"relativity-environment";
			yield return $"v{RelativitySettings.APIVersionNumber}";
			yield break;
		}
	}
	
	
	public Task<Int32> CreateAsync(MatterRequest matterRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/-1/matters/";
		return PostJsonAsync<Int32>(route, new{matterRequest}, cancellationToken);
	}
	
	
	public Task<MatterResponse> ReadAsync(Int32 matterID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/-1/matters/{matterID}";
		return GetJsonAsync<MatterResponse>(route, cancellationToken);
	}
	
	
	public Task<MatterResponse> ReadAsync(Int32 matterID, Boolean includeMetadata, Boolean includeActions)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/-1/matters/{matterID}/{includeMetadata}/{includeActions}";
		return GetJsonAsync<MatterResponse>(route, cancellationToken);
	}
	
	
	public Task UpdateAsync(Int32 matterID, MatterRequest matterRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/-1/matters/{matterID}";
		return PutAsync(route, new{matterRequest}, cancellationToken);
	}
	
	
	public Task UpdateAsync(Int32 matterID, MatterRequest matterRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/-1/matters/{matterID}";
		return PutAsync(route, new{matterRequest}, cancellationToken);
	}
	
	
	public Task DeleteAsync(Int32 matterID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/-1/matters/{matterID}";
		return DeleteAsync(route, cancellationToken);
	}
	
	
	public Task<List<DisplayableObjectIdentifier>> GetEligibleClientsAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/-1/matters/~/workspaces/-1/eligible-clients";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	
	
	public Task<List<DisplayableObjectIdentifier>> GetEligibleStatusesAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/-1/matters/eligible-statuses";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	
}


}
