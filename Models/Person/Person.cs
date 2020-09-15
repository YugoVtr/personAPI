using System;

namespace Globaltec.Models
{
    public class Person
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public string Uf { get; set; }
        public DateTime Birthday { get; set; }
    }
}
