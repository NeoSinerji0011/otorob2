namespace  OtoRobotWeb2.Models.Inputs
{

    public class RequestInput
    {
        public List<int> SorgulanacakFirmalar { get; set; }
        public int UserId { get; set; }
        public int SirketKod { get; set; }
        public string OemNumarasi { get; set; }
        public string SorguId { get; set; }
        public string GrupID { get; set; }

        public bool StokVarmi { get; set; }

    }
}
