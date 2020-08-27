using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebAPIWithSwagger.Models
{
    /// <summary>
    /// Entidade de endereço
    /// </summary>
    public class Endereco
    {
        [Required]
        public string Logradouro { get; set; }
        [DefaultValue(false)]
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
    }
}
