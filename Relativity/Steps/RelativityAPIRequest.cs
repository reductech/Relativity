﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

    public abstract class RelativityApiRequest<TRequest, TService, TServiceOutput, TOutput> : CompoundStep<TOutput>
    where TService : IDisposable
    {

        protected static Result<Array<Entity>, IErrorBuilder> TryConvertToEntityArray<T>(IEnumerable<T> stuff)
        {
            var array = new List<Entity>();
            foreach (var v in stuff)
            {
                var r = TryConvertToEntity(v);
                if (r.IsFailure) return r.ConvertFailure<Array<Entity>>();
                array.Add(r.Value);
            }

            return array.ToSCLArray();
        }

        protected static  Result<Entity, IErrorBuilder> TryConvertToEntity<T>(T thing)
        {
            var json = JsonConvert.SerializeObject(thing);

            var responseEntity = JsonConvert.DeserializeObject<Entity>(json,
                EntityJsonConverter.Instance, new VersionConverter());

            if (responseEntity is null)
                return ErrorCode.CouldNotParse.ToErrorBuilder(json, nameof(Entity));

            return responseEntity;

        }

        /// <inheritdoc />
        protected override async Task<Result<TOutput, IError>> Run(IStateMonad stateMonad, CancellationToken cancellationToken)
        {
            var requestObjectResult = await TryCreateRequest(stateMonad, cancellationToken);
            if (requestObjectResult.IsFailure) return requestObjectResult.ConvertFailure<TOutput>();

            var settingsResult = stateMonad.Settings.TryGetRelativitySettings();
            if (settingsResult.IsFailure)
                return settingsResult.MapError(x => x.WithLocation(this)).ConvertFailure<TOutput>();

            var serviceFactoryFactory = stateMonad.ExternalContext.TryGetContext<IServiceFactoryFactory>(ConnectorInjection.ServiceFactoryFactoryKey);

            if (serviceFactoryFactory.IsFailure) 
                return serviceFactoryFactory.MapError(x=>x.WithLocation(this)).ConvertFailure<TOutput>();

            var serviceFactory = serviceFactoryFactory.Value.CreateServiceFactory(settingsResult.Value);

            TServiceOutput serviceOutput;
            using var service = serviceFactory.CreateProxy<TService>();
            try
            {
                serviceOutput = await SendRequest(service, requestObjectResult.Value, cancellationToken);
                
            }
            catch (Exception ex)
            {
                return Result.Failure<TOutput, IError>(ErrorCode.Unknown.ToErrorBuilder(ex).WithLocation(this));
            }

            var output = ConvertOutput(serviceOutput).MapError(x=>x.WithLocation(this));

            return output;
        }

        public abstract Result<TOutput, IErrorBuilder> ConvertOutput(TServiceOutput serviceOutput);

        public abstract Task<TServiceOutput> SendRequest(TService service, TRequest requestObject, CancellationToken cancellationToken);

        public abstract Task<Result<TRequest, IError>> TryCreateRequest(IStateMonad stateMonad,
            CancellationToken cancellation);
    }

    ///// <summary>
    ///// A step which sends a Request to the Relativity API
    ///// </summary>
    //public abstract class RelativityApiRequest<TRequest, TOutput> : CompoundStep<TOutput>
    //{
    //    /// <inheritdoc />
    //    protected override async Task<Result<TOutput, IError>> Run(IStateMonad stateMonad,
    //        CancellationToken cancellationToken)
    //    {
    //        var requestObjectResult = await TryCreateRequest(stateMonad, cancellationToken);
    //        if (requestObjectResult.IsFailure) return requestObjectResult.ConvertFailure<TOutput>();

    //        var settingsResult = stateMonad.Settings.TryGetRelativitySettings();
    //        if (settingsResult.IsFailure)
    //            return settingsResult.MapError(x => x.WithLocation(this)).ConvertFailure<TOutput>();

    //        var flurlClientResult = stateMonad.GetFlurlClientFactory().Map(x => x.FlurlClient);
    //        if (flurlClientResult.IsFailure)
    //            return flurlClientResult.MapError(x => x.WithLocation(this)).ConvertFailure<TOutput>();

    //        var url = CreateURL(settingsResult.Value, requestObjectResult.Value);

    //        var request = flurlClientResult.Value.SetupRelativityRequest(settingsResult.Value, url);
    //        var requestTask = SendRequest(request, requestObjectResult.Value, cancellationToken);

    //        string responseString;

    //        try
    //        {
    //            responseString = await requestTask.ReceiveString();
    //        }
    //        catch (Exception ex)
    //        {
    //            var error = ErrorCode.Unknown.ToErrorBuilder(ex).WithLocation(this);
    //            return Result.Failure<TOutput, IError>(error);
    //        }


    //        var output = TryCreateOutput(responseString);

    //        if(output.HasNoValue)
    //            return Result.Failure<TOutput, IError>(ErrorCode.CouldNotParse.ToErrorBuilder(responseString, typeof(TOutput).Name).WithLocation(this)); 

    //        return output.Value;
    //    }

    //    public abstract Task<IFlurlResponse> SendRequest(IFlurlRequest flurlRequest, TRequest requestObject, CancellationToken cancellationToken);

    //    public abstract string[] CreateURL(RelativitySettings settings, TRequest request);

    //    public abstract Maybe<TOutput> TryCreateOutput(string stringResult);

    //    public abstract Task<Result<TRequest, IError>> TryCreateRequest(IStateMonad stateMonad,
    //        CancellationToken cancellation);
    //}
}