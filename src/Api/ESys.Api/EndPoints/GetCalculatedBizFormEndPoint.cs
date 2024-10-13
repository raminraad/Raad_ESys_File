using ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;
using ESys.Application.Features.BizForm.Queries.GetInitiatedBizForm;
using FastEndpoints;
using MediatR;

namespace ESys.Api.EndPoints
{
    // todo: Replace string result with Dto
    /// <summary>
    /// End point for getting data needed for iniltializing Biz form
    /// </summary>
    public class GetCalculatedBizFormEndPoint : Endpoint<GetCalculatedBizFromQuery, string>
    {
        private readonly IMediator _mediator;

        public GetCalculatedBizFormEndPoint(IMediator mediator)
        {
            _mediator = mediator;

        }
        public override void Configure()
        {
            Post("/api/dyna");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetCalculatedBizFromQuery req, CancellationToken ct)
        {
            var resp = await _mediator.Send(req,ct);

            await SendStringAsync(resp.Result);
        }

    }
}