using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Insight.Database;
using HealthReporter.Utilities;

namespace HealthReporter.Models
{
    class RatingLabelRepository
    {
        public void Insert(RatingLabel label)
        {
            var connection = DatabaseUtility.getConnection();
            //label.id = System.Guid.NewGuid().ToByteArray();
            var res = connection.InsertSql("INSERT INTO rating_labels (id, name, interpretation, rating, uploaded) values(@id, @name, @interpretation, @rating, @uploaded)", label);
        }

        public void Delete(Test test)
        {
            var res = DatabaseUtility.getConnection().InsertSql("DELETE FROM rating_labels WHERE id in (SELECT labelId FROM ratings WHERE ratings.labelId = rating_labels.id and testId=@id)", test);
        }

        public IList<RatingLabel> FindAll()
        {
            return DatabaseUtility.getConnection().QuerySql<RatingLabel>("SELECT * FROM rating_labels");
        }
        public IList<RatingLabel> getLabel(Rating rating)
        {
            return DatabaseUtility.getConnection().QuerySql<RatingLabel>("SELECT * FROM rating_labels where id = @labelId", rating);
        }
    }

    class RatingLabel
    {
        public byte[] id { get; set; }
        public string name { get; set; }
        public string interpretation { get; set; }
        public string updated { get; set; }
        public string uploaded { get; set; }
        public int rating { get; set; }
    }
}
