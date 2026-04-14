using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Labs
{
    public class InvestigationParameters
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid InvestigationParametersID { get; set; }
       
        [Required, StringLength(50, MinimumLength = 3)]
        public required string InvestigationParameter { get; set; }

        [Required, Range(0, 100)]
        public short ParameterOrder { get; set; }

        [Required]
        public required int InvestigationsID { get; set; }

        public virtual Investigations? Investigations { get; set; }
    }
}
