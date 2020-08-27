using WebAPIWithSwagger.Models;

namespace WebAPIWithSwagger.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class eDNEController : Controller
    {
        private readonly IConfiguration configuration;

        public eDNEController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        /// <summary>
        /// Consulta um endereço pelo CEP.
        /// </summary>
        /// <param name="cep"></param>
        /// <returns>Um endereço</returns>
        [HttpGet("{cep:long}")]
        public JsonResult GetByCEP(long cep)
        {            
            var execute = new SQLHelper(configuration.GetConnectionString("DefaultConnection").ToString());

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" LOG_LOGRADOURO.TLO_TX + ' ' + LOG_LOGRADOURO.LOG_NO AS LOGRADOURO, ");
            sql.AppendLine(" LOG_LOGRADOURO.LOG_COMPLEMENTO AS COMPLEMENTO, ");
            sql.AppendLine(" LOG_BAIRRO.BAI_NO AS BAIRRO, ");
            sql.AppendLine(" LOG_LOCALIDADE.LOC_NO AS CIDADE, ");
            sql.AppendLine(" LOG_LOGRADOURO.UFE_SG AS UF, ");
            sql.AppendLine(" LOG_LOGRADOURO.CEP AS CEP ");
            sql.AppendLine("FROM  LOG_LOGRADOURO");
            sql.AppendLine(" LEFT JOIN LOG_LOCALIDADE ON LOG_LOCALIDADE.LOC_NU = LOG_LOGRADOURO.LOC_NU");
            sql.AppendLine(" LEFT JOIN LOG_BAIRRO ON LOG_BAIRRO.BAI_NU = LOG_LOGRADOURO.BAI_NU_INI");
            sql.AppendLine("WHERE LOG_LOGRADOURO.CEP = " + cep);
            sql.AppendLine("UNION");
            sql.AppendLine("SELECT");
            sql.AppendLine(" ' ' AS LOGRADOURO,");
            sql.AppendLine(" ' ' AS COMPLEMENTO,");
            sql.AppendLine(" ' ' AS BAIRRO,");
            sql.AppendLine(" LOG_LOCALIDADE.LOC_NO AS CIDADE,");
            sql.AppendLine(" LOG_LOCALIDADE.UFE_SG AS UF,");
            sql.AppendLine(" LOG_LOCALIDADE.CEP AS CEP");
            sql.AppendLine("FROM  LOG_LOCALIDADE ");
            sql.AppendLine("WHERE LOG_LOCALIDADE.CEP = " + cep.ToString());

            DataSet dados = execute.ExecuteDataSet(CommandType.Text, sql.ToString(), null);

            DataRow row = dados.Tables[0].Rows[0];
            Endereco endereco = new Endereco
            {
                Bairro = row["BAIRRO"].ToString(),
                CEP = row["CEP"].ToString(),
                Cidade = row["CIDADE"].ToString(),
                Complemento = row["COMPLEMENTO"].ToString(),
                Logradouro = row["LOGRADOURO"].ToString(),
                UF = row["UF"].ToString()
            };

            return Json(endereco);
        }

        /// <summary>
        /// Consulta um endereço pelo logradouro.
        /// </summary>
        /// <param name="logradouro"></param>
        /// <param name="uf"></param>
        /// <param name="cidade"></param>
        /// <param name="bairro"></param>
        /// <returns>1 ou mais endereços</returns>
        [HttpGet("{logradouro}/{uf?}/{cidade?}/{bairro?}")]
        public JsonResult GetByLogradouro(string logradouro, string uf, string cidade, string bairro)
        {
            var execute = new SQLHelper(configuration.GetConnectionString("DefaultConnection").ToString());

            logradouro = logradouro.Replace("Rua", "").Replace("Av", "").Replace("Avenida", "").Replace("Travessa", "");

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" LOG_LOGRADOURO.TLO_TX + ' ' + LOG_LOGRADOURO.LOG_NO AS LOGRADOURO, ");
            sql.AppendLine(" LOG_LOGRADOURO.LOG_COMPLEMENTO AS COMPLEMENTO, ");
            sql.AppendLine(" LOG_BAIRRO.BAI_NO AS BAIRRO, ");
            sql.AppendLine(" LOG_LOCALIDADE.LOC_NO AS CIDADE, ");
            sql.AppendLine(" LOG_LOGRADOURO.UFE_SG AS UF, ");
            sql.AppendLine(" LOG_LOGRADOURO.CEP AS CEP ");
            sql.AppendLine("FROM LOG_LOGRADOURO, LOG_LOCALIDADE, LOG_BAIRRO");
            sql.AppendLine("WHERE LOG_LOGRADOURO.LOC_NU = LOG_LOCALIDADE.LOC_NU");
            sql.AppendLine("  AND LOG_LOGRADOURO.BAI_NU_INI = LOG_BAIRRO.BAI_NU");
            sql.AppendLine("  AND LOG_LOGRADOURO.LOG_NO = '" + logradouro + "'");
            if (uf != "{uf}")
                sql.AppendLine("  AND LOG_LOGRADOURO.UFE_SG = '" + uf + "'");
            if (cidade != "{cidade}")
                sql.AppendLine("  AND LOG_LOCALIDADE.LOC_NO = '" + cidade + "'");
            if (bairro != "{bairro}")
                sql.AppendLine("  AND LOG_BAIRRO.BAI_NO = '" + bairro + "'");

            DataSet dados = execute.ExecuteDataSet(CommandType.Text, sql.ToString(), null);

            List<Endereco> lista = new List<Endereco>();
            foreach (DataRow row in dados.Tables[0].Rows)
            {
                lista.Add(new Endereco
                {
                    Bairro = row["BAIRRO"].ToString(),
                    CEP = row["CEP"].ToString(),
                    Cidade = row["CIDADE"].ToString(),
                    Complemento = row["COMPLEMENTO"].ToString(),
                    Logradouro = row["LOGRADOURO"].ToString(),
                    UF = row["UF"].ToString()
                });
            }

            return Json(lista);
        }

        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
