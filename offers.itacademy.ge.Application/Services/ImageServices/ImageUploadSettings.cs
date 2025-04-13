namespace offers.itacademy.ge.Application.Services.ImageServices
{
    public class ImageUploadSettings
    {
        public string CompanyImagePath { get; set; }
        public string OfferImagePath { get; set; }
        public  long MaxFileSize { get; set; }
        public string[] AllowedExtensions { get; set; }
        public string DefaultCompanyImagePath { get; set; }
        public string DefaultOfferImagePath { get; set; }
        public string DefaultCompanyImageName { get; set; }
        public string DefaultOfferImageName { get; set; }
    }
}
