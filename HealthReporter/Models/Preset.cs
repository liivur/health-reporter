using System.Collections.Generic;
using HealthReporter.Utilities;
using Insight.Database;

namespace HealthReporter.Models
{

    class PresetRepository
    {
        public void Insert(Preset preset)
        {
            var connection = DatabaseUtility.getConnection();
            var res = connection.InsertSql("INSERT INTO presets (id, name, updated, uploaded) values (@id, @name, @updated, @uploaded)", preset);
        }

        public void Delete(Preset preset)
        {
            var res = DatabaseUtility.getConnection().InsertSql("DELETE from presets where id=@id", preset);
        }

        public IList<Preset> FindAll()
        {
            return DatabaseUtility.getConnection().QuerySql<Preset>("SELECT * FROM presets");
        }

        public void Update(Preset preset)
        {
            DatabaseUtility.getConnection().QuerySql<Preset>("UPDATE presets SET name=@name, updated=CURRENT_TIMESTAMP WHERE id=@id", preset);
        }
    }

    class Preset
    {
        public byte[] id { get; set; }
        public string name { get; set; }
        public string updated { get; set; }
        public string uploaded { get; set; }
    }

}
