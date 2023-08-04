using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class QuizType
    {
        [Key]
        public int LevelType { get; set; }

        public string NameType { get; set; }

        public ICollection<Question> QuizBanks { get; set; }
    }
}
