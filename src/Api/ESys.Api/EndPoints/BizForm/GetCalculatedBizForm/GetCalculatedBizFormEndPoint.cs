using System.Text.Json;
using System.Text.Json.Nodes;
using ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;
using FastEndpoints;
using MediatR;

namespace ESys.Api.EndPoints.BizForm.GetCalculatedBizForm
{
    // todo: Replace string result with Dto
    /// <summary>
    /// End point for getting data needed for initializing Biz form
    /// </summary>
    public class GetCalculatedBizFormEndPoint : Endpoint<List<JsonObject>,GetCalculatedBizFormResponse>
    {
        private readonly IMediator _mediator;

        public GetCalculatedBizFormEndPoint(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("/dyna");
            AllowAnonymous();
        }

        public override async Task HandleAsync(List<JsonObject> req, CancellationToken ct)
        {
            try
            {
                
                string serializeObject = JsonSerializer.Serialize(req);
                // var serializeObject = JsonConvert.SerializeObject(req, Formatting.None,
                //     new JsonSerializerSettings()
                //     { 
                //         ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //         DefaultValueHandling = DefaultValueHandling.Ignore
                //     });

                var mediatorReq = new GetCalculatedBizFormQuery { Body = serializeObject };

                var resp = await _mediator.Send(mediatorReq, ct);

                await SendAsync(resp);

            }
            catch (Exception e)
            {
                await SendResultAsync(Results.BadRequest(e.Message));
            }
        }

    }
}