using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthReporter.Models
{
    public class Appraisal_tests
    {
        public byte[] appraisalId { get; set; }
        public byte[] testId { get; set; }
        public decimal score { get; set; }
        public string note { get; set; }
        public decimal trial1 { get; set; }
        public decimal trial2 { get; set; }
        public decimal trial3 { get; set; }
        public string updated { get; set; }


    }
}
