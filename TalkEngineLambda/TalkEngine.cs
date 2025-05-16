using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
// using Amazon.Lambda.Serialization.Json;
using System.Net;
using Engine.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace TalkEngineLambda
{
    public class TalkEngineLambda
    {
        public APIGatewayHttpApiV2ProxyResponse Function(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request");
            context.Logger.LogLine(request.Body);

            var serializationOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };
            LambdaRequest lambdaBody = JsonSerializer.Deserialize<LambdaRequest>(request.Body, serializationOptions);


            Lancamento frase = new Lancamento(lambdaBody.Frase, lambdaBody.Usuario);
            frase.ProcessarFrase();
            string fraseTexto = frase.ToString();
            var body = new
            {
                Frase = fraseTexto,
                Dados = frase
            };

            var response = new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonSerializer.Serialize(body),
                Headers = new Dictionary<string, string> { 
                    { "Content-Type","application/json" },
                    //{ "Content-Type","text/plain" },
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Headers", "*" },
                    { "Access-Control-Allow-Methods", "*" }
                }
            };

            return response;
        }
    }
}