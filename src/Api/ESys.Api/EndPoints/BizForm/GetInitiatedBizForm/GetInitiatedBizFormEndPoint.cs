using ESys.Application.Exceptions;
using ESys.Application.Features.BizForm.Queries.GetInitiatedBizForm;
using FastEndpoints;
using MediatR;

namespace ESys.Api.EndPoints.BizForm.GetInitiatedBizForm
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
            Get("/dyna/{BizId}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetInitiatedBizFormQuery req, CancellationToken ct)
        {
            try
            {
                var resp = await _mediator.Send(req,ct);

                await SendStringAsync(resp.Result);
            }
            catch (NotFoundException e)
            {
                await SendNotFoundAsync(ct);
            }
        }

    }
}