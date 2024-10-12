using ESys.Application.Features.CalcForm.Queries.GetCalcFormInitialData;
using FastEndpoints;
using MediatR;

namespace ESys.Api.EndPoints
{
    // todo: Replace string result with Dto
    /// <summary>
    /// End point for getting data needed for iniltializing calculation form
    /// </summary>
    public class GetCalcFormInitialDataEndPoint : Endpoint<GetCalcFormInitialDataQuery, string>
    {
        private readonly IMediator _mediator;

        public GetCalcFormInitialDataEndPoint(IMediator mediator)
        {
            _mediator = mediator;

        }
        public override void Configure()
        {
            Post("/api/dyna");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetCalcFormInitialDataQuery req, CancellationToken ct)
        {
            var resp = await _mediator.Send(req);

            // todo: Replace string result with Dto
            // await SendAsync(resp);
            await SendStringAsync(resp.Result);
        }

    }
}