using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Company.Function
{
    public class Program
    {

        /// The Azure Cosmos DB endpoint for running this GetStarted sample.
        //private string EndpointUrl = Environment.GetEnvironmentVariable("EndpointUrl");
        private string EndpointUrl = "https://emuladorapicosmosdb.documents.azure.com:443/";//Environment.GetEnvironmentVariable("EndpointUrl");

        /// The primary key for the Azure DocumentDB account.
        //private string PrimaryKey = Environment.GetEnvironmentVariable("PrimaryKey");
        private string PrimaryKey = "shs6huBjg4uORDHyx6c5qv0gphxF5XmZ6DKW4hm9caeBZSobmyiij4xRD8p6Yj5FZxmjDbr9hx9f9ripDtehOg==";//Environment.GetEnvironmentVariable("PrimaryKey");

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        public Database database;

        // The container we will create.
        public Container container;

        // The name of the database and container we will create
        private string databaseId = "MensajesDatabase";
        private string containerId = "MensajesContainer";

        public async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }

        public async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/id");
        }

        public async Task AddItemsToContainerAsync(Mensaje mensaje)
        {
            // Create a family object for the Andersen family
            try
            {
                ItemResponse<Mensaje> mensajeResponse = await this.container.CreateItemAsync<Mensaje>(mensaje, new PartitionKey(mensaje.Id));
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                Console.WriteLine("Item in database with id: {0} already exists\n", mensaje.Id);
            }
        }

        public async Task<Mensaje[]> QueryItemsAsync(string variable, string inittime, string endtime)
        {
            var sqlQueryText = "SELECT * FROM c WHERE c." + variable + " >= '" + inittime + "' and c." + variable + " <= '" + endtime + "'";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Mensaje> queryResultSetIterator = this.container.GetItemQueryIterator<Mensaje>(queryDefinition);

            List<Mensaje> mensajes = new List<Mensaje>();
            List<Mensaje> resp = new List<Mensaje>();


            while (queryResultSetIterator.HasMoreResults)
            {
                resp.AddRange(queryResultSetIterator.ReadNextAsync().Result.ToList());
            }
            mensajes.AddRange(from mensaje in resp.Select((value, index) => new { value, index })
                              where (mensaje.index + 1) % Convert.ToInt32(resp.Count / 100) == 0
                              select mensaje.value);
            return mensajes.ToArray();
        }

        public async Task<HistoricoCaogulante[]> QueryItemsHistoricoAsync(string variable, string inittime, string endtime)
        {
            var sqlQueryText = "SELECT * FROM c WHERE c." + variable + " >= '" + inittime + "' and c." + variable + " <= '" + endtime + "'";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Mensaje> queryResultSetIterator = this.container.GetItemQueryIterator<Mensaje>(queryDefinition);

            List<HistoricoCaogulante> historico = new List<HistoricoCaogulante>();
            List<Mensaje> resp = new List<Mensaje>();


            while (queryResultSetIterator.HasMoreResults)
            {
                resp.AddRange(queryResultSetIterator.ReadNextAsync().Result.ToList());
            }

            var diaInicial = DateTime.Parse(inittime);
            var diaFinal = DateTime.Parse(endtime);
            var cantidadDias = (diaFinal - diaInicial).TotalDays;
            
            for (int dia = 0; dia < cantidadDias; dia++)
            {
                var cantidadDatos = resp.Where(x => x.aguaNatural.turbiedadentradatime.ToString("yyyy-MM-dd") == diaInicial.AddDays(dia).ToString("yyyy-MM-dd")).ToList();

                var promturbiedadentrada = (decimal)(cantidadDatos.Select(x => x.aguaNatural.turbiedadentrada).Average());
                var promcaudalentrada = (decimal)(cantidadDatos.Select(x => x.aguaNatural.caudalentrada).Average());
                var promconductividad = (decimal)(cantidadDatos.Select(x => x.aguaNatural.conductividad).Average());
                var promph = (decimal)(cantidadDatos.Select(x => x.aguaNatural.ph).Average());
                var prompresion = (decimal)(cantidadDatos.Select(x => x.aguaNatural.presion).Average());
                var promcolor = (decimal)(cantidadDatos.Select(x => x.aguaNatural.color).Average());
                var promturbiedadsalida = (decimal)(cantidadDatos.Select(x => x.aguaPotable.turbiedadsalida).Average());
                var promcaudalsalida = (decimal)(cantidadDatos.Select(x => x.aguaPotable.caudalsalida).Average());
                var promniveltanques = (decimal)(cantidadDatos.Select(x => x.aguaPotable.niveltanques).Average());
                var promcolorpotable = (decimal)(cantidadDatos.Select(x => x.aguaPotable.color).Average());

                var historicoDia = new HistoricoCaogulante();
                historicoDia.recomendado = nivelCoagulante(promturbiedadsalida, promconductividad, promph, promcolor, promcaudalentrada);
                historicoDia.AguaNatural.turbiedadentrada = (int)promturbiedadsalida;
                historicoDia.AguaNatural.turbiedadentradatime = diaInicial.AddDays(dia);
                historicoDia.AguaNatural.caudalentrada = (int)promcaudalentrada;
                historicoDia.AguaNatural.caudalentradatime = diaInicial.AddDays(dia);
                historicoDia.AguaNatural.conductividad = (int)promconductividad;
                historicoDia.AguaNatural.conductividadtime = diaInicial.AddDays(dia);
                historicoDia.AguaNatural.ph = (int)promph;
                historicoDia.AguaNatural.phtime = diaInicial.AddDays(dia);
                historicoDia.AguaNatural.presion = (int)prompresion;
                historicoDia.AguaNatural.presiontime = diaInicial.AddDays(dia);
                historicoDia.AguaNatural.color = (int)promcolor;
                historicoDia.AguaNatural.colortime = diaInicial.AddDays(dia);

                historicoDia.AguaPotable.turbiedadsalida = (int)promturbiedadsalida;
                historicoDia.AguaPotable.turbiedadsalidatime = diaInicial.AddDays(dia);
                historicoDia.AguaPotable.caudalsalida = (int)promcaudalsalida;
                historicoDia.AguaPotable.caudalsalidatime = diaInicial.AddDays(dia);
                historicoDia.AguaPotable.niveltanques = (int)promniveltanques;
                historicoDia.AguaPotable.niveltanquestime = diaInicial.AddDays(dia);
                historicoDia.AguaPotable.color = (int)promcolorpotable;
                historicoDia.AguaPotable.colortime = diaInicial.AddDays(dia);

                historico.Add(historicoDia);
            }
            return historico.ToArray();
        }

           public decimal nivelCoagulante(decimal vcturbiedadentrada, decimal vcconductividad,
            decimal vcph, decimal vccolor, decimal vccaudalentrada)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("AccessToken"));
            var ncrecomendado = JsonConvert.DeserializeObject<decimal[]>(client.PostAsJsonAsync(@"http://coagulante-svc.eastus2.azurecontainer.io/score", new ConsultaIA
            {
                Año = DateTime.UtcNow.Year,
                Mes = DateTime.UtcNow.Month,
                Hora = DateTime.UtcNow.Hour,
                Turbieda = vcturbiedadentrada,
                Conductividad = vcconductividad,
                Ph = vcph,
                Color = vccolor,
                Caudal = vccaudalentrada,
            }).Result.Content.ReadAsStringAsync().Result)[0];

            return ncrecomendado;
        }

        public async Task GetStartedDemoAsync()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey);
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
        }
    }
}