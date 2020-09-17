using System;
using System.ComponentModel.DataAnnotations;

namespace Globaltec.Models
{
    public class Person
    {
        [Key]
        public int Code { get; set; }
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"(^\d{11}$)", ErrorMessage = "Invalid CPF")]
        public string Cpf { get; set; }
        [RegularExpression(@"(^(AC|AL|AM|AP|BA|CE|DF|ES|GO|MA|MG|MS|MT|PA|PB|PE|PI|PR|RJ|RN|RO|RR|RS|SC|SE|SP|TO)$)", ErrorMessage = "Invalid UF")]
        public string Uf { get => this.uf; set { this.uf = value.ToUpper(); } }
        public DateTime Birthday { get; set; }
        private string uf;
    }
}
