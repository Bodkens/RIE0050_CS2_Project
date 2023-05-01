using DatabaseLayer;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CureConsultation.Models
{
    public class AddConsultationFormulaeModel
    {

        public AddConsultationFormulaeModel()
        {
            
        }

        [Required]
        public string? ConsultationId { get; set; }
        
    }
}
