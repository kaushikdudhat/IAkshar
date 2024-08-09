namespace iAkshar.Dto
{
    public class SabhaDto
    {
        public int? Mandalid { get; set; }

        public string? Sabhaname { get; set; }

        public string? Address { get; set; }

        public string? Area { get; set; }

        public string? Sabhaday { get; set; }

        public string? Sabhatime { get; set; }

        public int? Sabhatypeid { get; set; }

        public int? Contactpersonid { get; set; }

        public string? Googlemap { get; set; }
    }

    public class SabhaDashDto
    {
        public string Mandal { get; set; }

        public string? Sabhaname { get; set; }

        public string? Address { get; set; }

        public string? Area { get; set; }

        public string? Sabhaday { get; set; }

        public string? Sabhatime { get; set; }

        public string Sabhatype { get; set; }

        public int? Contactpersonid { get; set; }

        public string? Googlemap { get; set; }
    }
}
