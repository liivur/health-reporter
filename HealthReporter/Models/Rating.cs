using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Insight.Database;
using HealthReporter.Utilities;

namespace HealthReporter.Models
{
    class RatingRepository
    {
        public void Insert(Rating rating)
        {
            var connection = DatabaseUtility.getConnection();
            var res = connection.InsertSql("INSERT INTO ratings (testId, labelId, age, normF, normM, updated, uploaded) values(@testId, @labelId, @age, @normF, @normM, @updated, @uploaded)", rating);
        }

        public IList<Rating> FindAll()
        {
            return DatabaseUtility.getConnection().QuerySql<Rating>("SELECT * FROM ratings");
        }

        public IList<Rating> getTestRatings(Test test)
        {
            return DatabaseUtility.getConnection().QuerySql<Rating>("SELECT * FROM ratings WHERE testId = @id", test);
        }
        public IList<Rating> getAges(Test test) 
        {
            return DatabaseUtility.getConnection().QuerySql<Rating>("SELECT* FROM ratings INNER JOIN tests ON ratings.testId = @id AND tests.name = @name GROUP BY age", test);
        }
        public IList<Rating> getSameAgeRatings(Rating rating)
        {
            return DatabaseUtility.getConnection().QuerySql<Rating>("SELECT* FROM ratings WHERE age=@age", rating);
        }
    }

    class Rating
    {
        public byte[] testId { get; set; }
        public byte[] labelId { get; set; }
        public int age { get; set; }
        public decimal normF { get; set; }
        public decimal normM { get; set; }
        public string updated { get; set; }
        public string uploaded { get; set; }
    }
}
