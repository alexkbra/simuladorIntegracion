using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;

using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Company.Function
{
    public static class TimerEmulador
    {

        private static bool swSingalR = true;

        private static bool swDB = true;
        private static HttpClient httpClient = new HttpClient();


        [FunctionName("index")]
        public static IActionResult Index([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "content", "index.html");
            return new ContentResult
            {
                Content = File.ReadAllText(path),
                ContentType = "text/html",
            };
        }

        [FunctionName("consulta")]
        public static async Task<IActionResult> consulta([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            string variable = req.Query["variable"];
            string inittime = req.Query["inittime"];
            string endtime = req.Query["endtime"];

            Mensaje[] result = new Mensaje[0];
            try
            {
                Program programDb = new Program();
                await programDb.GetStartedDemoAsync();
                result = await programDb.QueryItemsAsync(variable, inittime, endtime);

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(result, new IsoDateTimeConverter() { DateTimeFormat = "MM/dd/yy HH:mm:ss" }),
                ContentType = "application/json",
            };
        }

        [FunctionName("stop")]
        public static async Task<IActionResult> stop([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            
            swSingalR = false;
            swDB = false;
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject("Ok"),
                ContentType = "application/json",
            };
        }

        [FunctionName("start")]
        public static async Task<IActionResult> start([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            
            swSingalR = true;
            swDB = true;
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject("Ok"),
                ContentType = "application/json",
            };
        }

        [FunctionName("startDB")]
        public static async Task<IActionResult> startDB([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            
            swDB = true;
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject("Ok"),
                ContentType = "application/json",
            };
        }

        [FunctionName("startsignalr")]
        public static async Task<IActionResult> startSignalr([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            
            swSingalR = true;
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject("Ok"),
                ContentType = "application/json",
            };
        }

        [FunctionName("stopDB")]
        public static async Task<IActionResult> stopDB([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            
            swDB = false;
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject("Ok"),
                ContentType = "application/json",
            };
        }

        [FunctionName("stopsignalr")]
        public static async Task<IActionResult> stopsignalr([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            
            swSingalR = false;
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject("Ok"),
                ContentType = "application/json",
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

            int id = new Random().Next(50, 100);

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

            Mensaje mensaje = new Mensaje
            {
                Id = id.ToString(),
                siata = new Siata
                {
                    sensores = new Sensores[]{
                        new Sensores { nombre="Sensor1",precipitacion = precipitacion1, precipitaciontime = DateTime.Now.AddHours(-precipitacion1), temperatura = temperatura1, temperaturatime= DateTime.Now.AddHours(-temperatura1), humedad = humendad1, humedadtime = DateTime.Now.AddHours(-humendad1)},
                        new Sensores { nombre="Sensor2",precipitacion = precipitacion2, precipitaciontime = DateTime.Now.AddHours(-precipitacion2), temperatura = temperatura2, temperaturatime= DateTime.Now.AddHours(-temperatura2), humedad = humendad2, humedadtime = DateTime.Now.AddHours(-humendad2)},
                        new Sensores { nombre="Sensor3",precipitacion = precipitacion3, precipitaciontime = DateTime.Now.AddHours(-precipitacion3), temperatura = temperatura3, temperaturatime= DateTime.Now.AddHours(-temperatura3), humedad = humendad3, humedadtime = DateTime.Now.AddHours(-humendad3)},
                    }
                },
                aguaNatural = new AguaNatural
                {
                    turbiedadentrada = vcturbiedadentrada,
                    turbiedadentradatime = DateTime.Now.AddHours(-vcturbiedadentrada),
                    caudalentrada = vcconductividad,
                    conductividadtime = DateTime.Now.AddHours(-vcconductividad),
                    ph = vcph,
                    phtime = DateTime.Now.AddHours(-vcph),
                    presion = vcpresion,
                    presiontime = DateTime.Now.AddHours(-vcpresion),
                    color = vccolor,
                    colortime = DateTime.Now.AddHours(-vccolor)
                },
                aguaPotable = new AguaPotable
                {
                    turbiedadsalida = apturbiedadsalida,
                    turbiedadsalidatime = DateTime.Now.AddHours(-apturbiedadsalida),
                    caudalsalida = apcaudalsalida,
                    caudalsalidatime = DateTime.Now.AddHours(-apcaudalsalida),
                    niveltanques = apniveltanques,
                    niveltanquestime = DateTime.Now.AddHours(-apniveltanques),
                    color = apcolor,
                    colortime = DateTime.Now.AddHours(-apcolor)
                },
                nivelesCoagulacion = new NivelesCoagulacion
                {
                    actual = ncactual,
                    actualtime = DateTime.Now.AddHours(-ncactual),
                    recomendado = ncrecomendado,
                    recomendadotime = DateTime.Now.AddHours(-ncrecomendado)
                },
                Partition = "potabilizacion123"
            };
            // Console.WriteLine("mensaje {0}", mensaje.ToString());
            try
            {
                if (swDB)
                {
                    Program programDb = new Program();
                    await programDb.GetStartedDemoAsync();
                    await programDb.AddItemsToContainerAsync(mensaje);
                    mensaje.IsRegistered = true;
                }
                else
                {
                    Console.WriteLine("ya no guarda");
                    mensaje.IsRegistered = false;
                }

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
                mensaje.IsRegistered = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                mensaje.IsRegistered = false;
            }

            String valor = mensaje.ToString();
            if (swSingalR)
            {
                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "newMessage",
                        Arguments = new[] {
                        valor
                        }
                    });
            }
            else
            {
                Console.WriteLine("ya no publica");
            }
        }
        private class GitResult
        {
            [JsonRequired]
            [JsonProperty("stargazers_count")]
            public string StarCount { get; set; }
        }
    }
}
