using System;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models;

public class EmailModel
{
        [EmailAddress(ErrorMessage = "Give the correct email.")]
        public string? To { get; set; }

        
        public string? Subject { get; set; }
        public string? Body { get; set; }

}
