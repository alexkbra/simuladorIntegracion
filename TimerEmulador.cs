using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;


using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class TimerEmulador
    {

        private static HttpClient httpClient = new HttpClient();

        [FunctionName("index")]
        public static IActionResult Index([HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req, ExecutionContext context)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "content", "index.html");
            return new ContentResult
            {
                Content = File.ReadAllText(path),
                ContentType = "text/html",
            };
        }

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate( 
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            [SignalRConnectionInfo(HubName = "serverlessSample")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }


        [FunctionName("broadcast")]
        public static async Task Run([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer,
        [SignalR(HubName = "serverlessSample")] IAsyncCollector<SignalRMessage> signalRMessages)

        {
             // */5 * * * * * cada 5 segundos
            // 0 */5 * * * * cada 5 min
            
            int precipitacion1 = new Random().Next(50, 100);
            int temperatura1 = new Random().Next(0, 50);
            int humendad1 = new Random().Next(0, 50);

            int precipitacion2 = new Random().Next(50, 100);
            int temperatura2 = new Random().Next(0, 50);
            int humendad2 = new Random().Next(0, 50);

            int precipitacion3 = new Random().Next(50, 100);
            int temperatura3 = new Random().Next(0, 50);
            int humendad3 = new Random().Next(0, 50);

            int vcturbiedadentrada = new Random().Next(0, 50);
            int vccaudalentrada = new Random().Next(0, 50);
            int vcconductividad = new Random().Next(0, 50);
            int vcph = new Random().Next(0, 50);
            int vcpresion = new Random().Next(0, 50);
            int vccolor = new Random().Next(0, 50);

            int coagulantenivelacutal = new Random().Next(0, 50);
            int coagulantenivelrecomendado = new Random().Next(0, 50);

            int apturbiedadsalida = new Random().Next(0, 50);
            int apcolor = new Random().Next(0, 50);
            int apcaudalsalida = new Random().Next(0, 50);
            int apniveltanques = new Random().Next(0, 50);


            int ncactual = new Random().Next(0, 100);
            int ncrecomendado = new Random().Next(0, 100);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "newMessage",
                    Arguments = new[] { 
                        "[{'Siata':[{'Sensor1':{'precipitacion': "+precipitacion1+",'temperatura':"+temperatura1+",'humedad':"+humendad1+"}},{'Sensor2':{'precipitacion': "+precipitacion2+",'temperatura':"+temperatura2+",'humedad':"+humendad2+"}},{'Sensor3':{'precipitacion': "+precipitacion3+",'temperatura':"+temperatura3+",'humedad':"+humendad3+"}}],"+
                        "{'AguaNatural':{'turbiedadentrada': "+vcturbiedadentrada+","+
                                "'caudalentrada':"+vccaudalentrada+","+
                                "'conductividad':"+vcconductividad+","+
                                "'ph':"+vcph+","+
                                "'presion':"+vcpresion+","+
                                "'color':"+vccolor+"}},"+
                        "{'AguaPotable':{'turbiedadsalida': "+apturbiedadsalida+","+
                                "'caudalsalida':"+apcaudalsalida+","+
                                "'niveltanques':"+apniveltanques+","+
                                "'vccolor':"+apcolor+"}},"+
                        "{'NivelesCoagulacion':{'actual': "+ncactual+","+
                                "'recomendado':"+ncrecomendado+"}}]"
                    }
                });
            
        }
         private class GitResult
        {
            [JsonRequired]
            [JsonProperty("stargazers_count")]
            public string StarCount { get; set; }
        }
    }
}
