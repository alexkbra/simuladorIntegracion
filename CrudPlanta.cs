using Microsoft.Azure.Cosmos;
using signalre.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace signalre
{
    public class CrudPlanta
    {
        #region Base datos
        private string EndpointUrl = "https://emuladorcosmosdbepm.documents.azure.com:443/";
        private string PrimaryKey = "F0LExYfBNnQB808eI1Yy9gD9ERw4aTnIaNolfr77nG976llQOHxe0A2PePfLtiH62mEsK8XSLPmQYX2iFLKYlw==";

        private CosmosClient cosmosClient;
        public Database database;
        public Container container;

        private string databaseId = "CrudPotabilizacion";
        private string containerId = "PotabilizacionContainer";

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

        public async Task GetStartedAsync()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey);
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
        }
        #endregion

        #region CRUD Plantas
        //Create
        public async Task AgregarPlanta(Plantas planta)
        {
            await this.container.CreateItemAsync<Plantas>(planta, new PartitionKey(planta.Id));
        }

        //Read

        public async Task<Plantas[]> ObtenerTodoPlantas()
        {
            var sqlQueryText = "SELECT * FROM c";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Plantas> queryResultSetIterator = this.container.GetItemQueryIterator<Plantas>(queryDefinition);

            List<Plantas> plantas = new List<Plantas>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Plantas> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Plantas planta in currentResultSet)
                {
                    plantas.Add(planta);
                }
            }
            return plantas.ToArray();
        }

        //Update
        public async Task ActualizarPlanta(Plantas planta)
        {
            await this.container.UpsertItemAsync<Plantas>(planta, new PartitionKey(planta.Id));
        }

        //Delete
        public async Task EliminarPlanta(string id)
        {
            await this.container.DeleteItemAsync<Plantas>(id, new PartitionKey(id));
        }
        #endregion
    }
}
