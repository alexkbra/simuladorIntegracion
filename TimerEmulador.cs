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
using System.Net.Http.Headers;
using signalre.Entities;
using signalre;

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
                Content = JsonConvert.SerializeObject(result),
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

            var id = Guid.NewGuid();
            int precipitacion1 = new Random().Next(15, 20);
            int temperatura1 = new Random().Next(10, 35);
            int humendad1 = new Random().Next(0, 10);

            int precipitacion2 = new Random().Next(25, 35);
            int temperatura2 = new Random().Next(10, 35);
            int humendad2 = new Random().Next(5, 15);

            int precipitacion3 = new Random().Next(35, 45);
            int temperatura3 = new Random().Next(10, 35);
            int humendad3 = new Random().Next(10, 20);

            int vcturbiedadentrada = new Random().Next(0, 5);
            int vccaudalentrada = new Random().Next(0, 50);
            int vcconductividad = new Random().Next(0, 5);
            int vcph = new Random().Next(5, 10);
            int vcpresion = new Random().Next(100, 700);
            int vccolor = new Random().Next(0, 5);

            int coagulantenivelacutal = new Random().Next(0, 50);
            int coagulantenivelrecomendado = new Random().Next(0, 50);

            int apturbiedadsalida = new Random().Next(0, 5);
            int apcolor = new Random().Next(0, 50);
            int apcaudalsalida = new Random().Next(0, 50);
            int apniveltanques = new Random().Next(150, 550);

            int ncactual = new Random().Next(0, 100);

            var program = new Program();

            var ncrecomendado = program.nivelCoagulante(vcturbiedadentrada, vcconductividad, vcph, vccolor, vccaudalentrada);

            Mensaje mensaje = new Mensaje
            {
                Id = id.ToString(),
                siata = new Siata
                {
                    sensores = new Sensores[]{
                        new Sensores {
                            id = "0101010101010",
                            nombre="Sensor #1",
                            estado = true,
                            ubicacion = "boqueron",
                            idPrecipitacion = 12,
                            precipitacion = precipitacion1,
                            precipitaciontime = DateTime.UtcNow,
                            idTemperatura = 12,
                            temperatura = temperatura1,
                            temperaturatime= DateTime.UtcNow,
                            idHumedad = 15,
                            humedad = humendad1,
                            humedadtime = DateTime.UtcNow,
                            fechaActualizacion = DateTime.UtcNow
                        },
                        new Sensores {
                            id = "0202020202020",
                            nombre="Sensor #2",
                            estado = true,
                            ubicacion = "San José",
                            idPrecipitacion = 12,
                            precipitacion = precipitacion2,
                            precipitaciontime = DateTime.UtcNow,
                            idTemperatura = 12,
                            temperatura = temperatura2,
                            temperaturatime= DateTime.UtcNow,
                            idHumedad = 15,
                            humedad = humendad2,
                            humedadtime = DateTime.UtcNow,
                            fechaActualizacion = DateTime.UtcNow
                        },
                        new Sensores {
                            id = "03030303030303",
                            nombre="Sensor #3",
                            estado = true,
                            ubicacion = "Otra ubicación",
                            idPrecipitacion = 12,
                            precipitacion = precipitacion3,
                            precipitaciontime = DateTime.UtcNow,
                            idTemperatura = 12,
                            temperatura = temperatura3,
                            temperaturatime= DateTime.UtcNow,
                            idHumedad = 15,
                            humedad = humendad3,
                            humedadtime = DateTime.UtcNow,
                            fechaActualizacion = DateTime.UtcNow
                        },
                    }
                },
                aguaNatural = new AguaNatural
                {
                    turbiedadentrada = vcturbiedadentrada,
                    turbiedadentradatime = DateTime.UtcNow,
                    caudalentrada = vccaudalentrada,
                    conductividad = vcconductividad,
                    conductividadtime = DateTime.UtcNow,
                    ph = vcph,
                    phtime = DateTime.UtcNow,
                    presion = vcpresion,
                    presiontime = DateTime.UtcNow,
                    color = vccolor,
                    colortime = DateTime.UtcNow,
                    fechaActualizacion = DateTime.UtcNow
                },
                aguaPotable = new AguaPotable
                {
                    turbiedadsalida = apturbiedadsalida,
                    turbiedadsalidatime = DateTime.UtcNow,
                    caudalsalida = apcaudalsalida,
                    caudalsalidatime = DateTime.UtcNow,
                    niveltanques = apniveltanques,
                    niveltanquestime = DateTime.UtcNow,
                    color = apcolor,
                    colortime = DateTime.UtcNow,
                    fechaActualizacion = DateTime.UtcNow
                },
                nivelesCoagulacion = new NivelesCoagulacion
                {
                    actual = ncactual,
                    actualtime = DateTime.UtcNow,
                    recomendado = decimal.Round(ncrecomendado, 1),
                    recomendadotime = DateTime.UtcNow,
                    fechaActualizacion = DateTime.UtcNow
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
            if (swSingalR)
            {
                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "newMessage",
                        Arguments = new[] {
                        JsonConvert.SerializeObject(mensaje)
                        }
                    });
            }
            else
            {
                Console.WriteLine("ya no publica");
            }
        }

        [FunctionName("consultaNivelCoagulante")]
        public static async Task<IActionResult> consultaNivelCoagulante(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            var turbiedad = decimal.Parse(req.Query["turbiedad"]);
            var conductividad = decimal.Parse(req.Query["conductividad"]);
            var ph = decimal.Parse(req.Query["ph"]);
            var color = decimal.Parse(req.Query["color"]);
            var caudal = decimal.Parse(req.Query["caudal"]);
            var program = new Program();

            var nivelCoagulanteResponse = program.nivelCoagulante(turbiedad, conductividad, ph, color, caudal);

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(decimal.Round(nivelCoagulanteResponse, 1)),
                ContentType = "application/json",
            };
        }




        [FunctionName("consultaHistoricoCoagulante")]
        public static async Task<IActionResult> consultaHistoricoCoagulante([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)

        {
            string variable = req.Query["variable"];
            string inittime = req.Query["inittime"];
            string endtime = req.Query["endtime"];

            HistoricoCaogulante[] result = new HistoricoCaogulante[0];
            try
            {
                Program programDb = new Program();
                await programDb.GetStartedDemoAsync();
                result = await programDb.QueryItemsHistoricoAsync(variable, inittime, endtime);

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(result),
                ContentType = "application/json",
            };
        }

        private class GitResult
        {
            [JsonRequired]
            [JsonProperty("stargazers_count")]
            public string StarCount { get; set; }
        }


        #region Crud Platas
        //Create
        [FunctionName("AgregarPlanta")]
        public static async Task<IActionResult> AgregarPlanta(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ExecutionContext context)

        {
            var planta = JsonConvert.DeserializeObject<Plantas>(await new StreamReader(req.Body).ReadToEndAsync());
            CrudPlanta CrudPlanta = new CrudPlanta();
            planta.Id = Guid.NewGuid().ToString();
            await CrudPlanta.GetStartedAsync();
            var plantaResp = await CrudPlanta.AgregarPlanta(planta);

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(plantaResp),
                ContentType = "application/json",
            };
        }


        //Read
        [FunctionName("ListarPlantas")]
        public static async Task<IActionResult> ListarPlantas(
        [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            CrudPlanta CrudPlanta = new CrudPlanta();
            await CrudPlanta.GetStartedAsync();
            var plantas = await CrudPlanta.ObtenerTodoPlantas("SELECT * FROM c");

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(plantas),
                ContentType = "application/json",
            };
        }

        [FunctionName("ObtenerPlantas")]
        public static async Task<IActionResult> ObtenerPlantas(
        [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            var id = req.Query["Id"];
            CrudPlanta CrudPlanta = new CrudPlanta();
            await CrudPlanta.GetStartedAsync();
            var plantas = await CrudPlanta.ObtenerPlanta(id);

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(plantas),
                ContentType = "application/json",
            };
        }

        //Update
        [FunctionName("ActualizarPlanta")]
        public static async Task<IActionResult> ActualizarPlanta(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req, ExecutionContext context)

        {
            var planta = JsonConvert.DeserializeObject<Plantas>(await new StreamReader(req.Body).ReadToEndAsync());
            CrudPlanta CrudPlanta = new CrudPlanta();
            await CrudPlanta.GetStartedAsync();
            var plantaResp = await CrudPlanta.ActualizarPlanta(planta);

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(plantaResp),
                ContentType = "application/json",
            };
        }

        [FunctionName("EliminarPlanta")]
        public static async Task<IActionResult> EliminarPlanta(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)] HttpRequest req, ExecutionContext context)

        {
            var id = req.Query["Id"];
            CrudPlanta CrudPlanta = new CrudPlanta();
            await CrudPlanta.GetStartedAsync();
            var plantaResp =  await CrudPlanta.EliminarPlanta(id);

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(plantaResp),
                ContentType = "application/json",
            };
        }
        #endregion


    }
}