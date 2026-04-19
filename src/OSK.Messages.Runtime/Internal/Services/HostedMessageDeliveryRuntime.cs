using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Runtime.Internal.Services;

internal partial class HostedMessageDeliveryRuntime(IMessageCenter messageCenter, IEnumerable<IMessageDeliveryPipelineFactory> pipelineFactories, ILogger<HostedMessageDeliveryRuntime> logger) : IHostedService
{
    #region Variables

    private readonly List<IMessageDeliveryPipeline> _pipelines = [];

    private bool _running;

    #endregion

    #region IHostedService

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (!pipelineFactories.Any())
        {
            LogEmptyPipelineFactoriesStartInformation(logger);
            return;
        }
        if (_running)
        {
            return;
        }

        if (_pipelines.Count is 0)
        {
            foreach (var recipient in messageCenter.GetRecipientDetails())
            {
                foreach (var factory in pipelineFactories)
                {
                    _pipelines.Add(factory.CreatePipeline(recipient));
                }
            }

            if (_pipelines.Count is 0)
            {
                LogEmptyPipelineStartInformation(logger);
                return;
            }
        }


        foreach (var pipeline in _pipelines)
        {
            try
            {
                await pipeline.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDeliveryPipelineStartupError(logger, pipeline.CourierName, pipeline.Id, ex);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_pipelines.Any())
        {
            LogEmptyPipelineStopInformation(logger);
            return;
        }
        if (!_running)
        {
            return;
        }

        foreach (var pipeline in _pipelines)
        {
            try
            {
                await pipeline.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDeliveryPipelineStoprError(logger, pipeline.CourierName, pipeline.Id, ex);
            }
        }

        _running = false;
    }

    #endregion

    #region Logging

    [LoggerMessage(eventId: 1, level: LogLevel.Information, "There were no message pipeline factories to build pipelines.")]
    private static partial void LogEmptyPipelineFactoriesStartInformation(ILogger logger);

    [LoggerMessage(eventId: 2, level: LogLevel.Information, "There were no message pipelines to start.")]
    private static partial void LogEmptyPipelineStartInformation(ILogger logger);

    [LoggerMessage(eventId: 3, level: LogLevel.Information, "There were no message pipelines to stop.")]
    private static partial void LogEmptyPipelineStopInformation(ILogger logger);

    [LoggerMessage(eventId: 4, level: LogLevel.Error, "An error was encountered when starting the delivery pipeline '{pipelineId}' with courier '{courierName}'")]
    private static partial void LogDeliveryPipelineStartupError(ILogger logger, CourierName courierName, string pipelineId, Exception ex);

    [LoggerMessage(eventId: 5, level: LogLevel.Error, "An error was encountered when stopping the delivery pipeline '{pipelineId}' with courier '{courierName}'")]
    private static partial void LogDeliveryPipelineStoprError(ILogger logger, CourierName courierName, string pipelineId, Exception ex);

    #endregion
}