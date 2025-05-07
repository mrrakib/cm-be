namespace COLLECTION_MANAGEMENT_API.ViewModels
{
    public class PickupRequestDTO
    {
        public long id { get; set; }
        public long contract_id { get; set; }
        public long company_id { get; set; }
        public string? company_name { get; set; }
        public long project_id { get; set; }
        public string? project_name { get; set; }
        public long service_program_id { get; set; }
        public string? address { get; set; }
        public string? contact_no { get; set; }
        public int status { get; set; }
        public IFormFile file { get; set; } = null!;
    }
}
