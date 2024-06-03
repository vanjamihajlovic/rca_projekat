using Microsoft.WindowsAzure.Storage.Table;

namespace ServiceData
{
    public class AdminEmail : TableEntity
    {
        public string EmailAdresa { get; set; }

        public AdminEmail() {
            PartitionKey = "AdminEmail";
        }

        public AdminEmail(string adresa) : this()
        {
            RowKey = adresa;
            EmailAdresa = adresa;
        }
    }
}
