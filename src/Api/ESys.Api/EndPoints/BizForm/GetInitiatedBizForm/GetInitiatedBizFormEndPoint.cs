using ESys.Application.Exceptions;
using ESys.Application.Features.BizForm.Queries.GetInitiatedBizForm;
using FastEndpoints;
using MediatR;

namespace ESys.Api.EndPoints.BizForm.GetInitiatedBizForm
{
    /// <summary>
    /// End point for getting data needed for BizFrom initialization
    /// </summary>
    public class GetInitiatedBizFormEndPoint : Endpoint<GetInitiatedBizFormQuery, string>
    {
        private readonly IMediator _mediator;

        public GetInitiatedBizFormEndPoint(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override void Configure()
        {
            Get("/dyna/{BizId}");
            AllowAnonymous();
        }

        /// <summary>
        /// Handles BizForm initialization via mediator 
        /// </summary>
        /// <param name="req">Request containing BizId needed for BizForm initialization</param>
        /// <param name="ct">Cancellation token</param>
        public override async Task HandleAsync(GetInitiatedBizFormQuery req, CancellationToken ct)
        {
            try
            {
                var resp = await _mediator.Send(req,ct);

                await SendStringAsync(resp.Result, cancellation: ct);
            }
            catch (NotFoundException e)
            {
                await SendNotFoundAsync(ct);
            }
        }

    }
}