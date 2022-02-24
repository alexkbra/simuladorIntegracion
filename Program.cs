using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using System.Linq;

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
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }

        public async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/id");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        public async Task AddItemsToContainerAsync(Mensaje mensaje)
        {
            // Create a family object for the Andersen family
            try
            {
                ItemResponse<Mensaje> mensajeResponse = await this.container.CreateItemAsync<Mensaje>(mensaje, new PartitionKey(mensaje.Id));
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", mensajeResponse.Resource.Id, mensajeResponse.RequestCharge);
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

        public async Task<Sensores[]> QueryItemsSensoresAsync(string sensor, string variable, string inittime, string endtime)
        {
            var sqlQueryText = $"SELECT * from c.{sensor} AS Sensores WHERE Sensores.{variable} >= '{inittime}' and Sensores.{variable} <= '{endtime}'";
            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Sensores> queryResultSetIterator = this.container.GetItemQueryIterator<Sensores>(queryDefinition);

            List<Sensores> sensores = new List<Sensores>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Sensores> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Sensores item in currentResultSet)
                {
                    sensores.Add(item);
                    Console.WriteLine("\tRead {0}\n", item);
                }
            }
            return sensores.ToArray();
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