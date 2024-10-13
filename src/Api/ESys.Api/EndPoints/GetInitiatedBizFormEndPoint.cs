using ESys.Application.Features.BizForm.Queries.GetInitiatedBizForm;
using FastEndpoints;
using MediatR;

namespace ESys.Api.EndPoints
{
    // todo: Replace string result with Dto
    /// <summary>
    /// End point for getting data needed for iniltializing Biz form
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
            Get("/api/dyna/{BizId}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetInitiatedBizFormQuery req, CancellationToken ct)
        {
            var resp = await _mediator.Send(req,ct);

            await SendStringAsync(resp.Result);
        }

    }
}