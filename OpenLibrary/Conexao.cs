using System.Data.SqlClient;

namespace OpenLibrary
{
    internal class Conexao
    {
            public SqlConnection conn = new SqlConnection(
                @"Server=localhost;
                Database=library;
                Trusted_Connection=True;
                TrustServerCertificate=True"
            );
    }
}
