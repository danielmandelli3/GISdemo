using WaterWatch.Models;
using WaterWatch.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WaterWatch.Repositories
{
    public class WaterConsumptionRepository : IWaterConsumptionRepository
    {
        private readonly IDataContext _context;

        public WaterConsumptionRepository(IDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WaterConsumption>> GetAll()
        {
            SaveData();
            return await _context.Consumptions.ToListAsync();
        }

        public async Task<IEnumerable<WaterConsumption>> GetTopTenConsumers()
        {
            var q = _context.Consumptions
            .OrderByDescending(avgKL => avgKL.averageMonthlyKL)
            .Take(10)
            .ToListAsync();

            return await q;
        }

        public void SaveData()
        {
            // Controlla se la tabella è vuote prima del caricamento dei dat, altrimenti salta il processo di estrazione, trasformazione e caricamento del processo
            var res_dataset = _context.Consumptions.ToList();

            if(res_dataset.Count() == 0)
            {
                Console.WriteLine("No Data");

                var geoJSON = File.ReadAllText("C:\\Users\\Dan\\Desktop\\Project\\WaterWatch\\WaterWatch\\json\\water_consumption.geojson");
                dynamic jsonObj = JsonConvert.DeserializeObject(geoJSON);

                foreach (var feature in jsonObj["features"])
                {
                    // Estrae i valori dal file object usando i campi
                    string str_neighbourhood = feature["properties"]["neighbourhood"];
                    string str_suburb_group = feature["properties"]["suburb_group"];
                    string str_avgMonthlyKL = feature["properties"]["averageMonthlyKL"];
                    string str_geometry = feature["geometry"]["coordinates"].ToString(Newtonsoft.Json.Formatting.None);

                    // Applica le trasformazioni
                    // Rimuovi il .0 dal valore averageMonthlyKL del JSON
                    string conv_avgMthlKl = str_avgMonthlyKL.Replace(".0","");

                    //Converti la string in int
                    int avgMthlKl = Convert.ToInt32(conv_avgMthlKl);

                    // Carica i dati nella nostra tabella
                    WaterConsumption wc = new()
                    {
                        neighbourhood = str_neighbourhood,
                        suburb_group = str_suburb_group,
                        averageMonthlyKL = avgMthlKl,
                        coordinates = str_geometry
                    };

                    _context.Consumptions.Add(wc);
                    _context.SaveChanges();
                }
            }
            else
            {
                Console.WriteLine("Data Loaded");
            }
        }
    }
}