using ESys.Application.Features.BizCalcForm.Queries.GetInitiatedBizForm;
using FastEndpoints;
using MediatR;

namespace ESys.Api.EndPoints
{
    // todo: Replace string result with Dto
    /// <summary>
    /// End point for getting data needed for iniltializing calculation form
    /// </summary>
    public class GetInitiatedBizEndPoint : Endpoint<GetInitiatedBizFormQuery, string>
    {
        private readonly IMediator _mediator;

        public GetInitiatedBizEndPoint(IMediator mediator)
        {
            _mediator = mediator;

        }
        public override void Configure()
        {
            Post("/api/dyna");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetInitiatedBizFormQuery req, CancellationToken ct)
        {
            var resp = await _mediator.Send(req,ct);

            await SendStringAsync(resp.Result);
        }

    }
}