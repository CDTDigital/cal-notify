using System.Runtime.Serialization;

namespace CalNotifyApi.Models.Admins
{
    [DataContract]
    public enum PossibleRegions
    {
        [DataMember(Name="Bay Ara")]
        BayArea,
        [DataMember(Name = "Central Coast")]
        CentralCoast,

        [DataMember(Name="Central Valley")]
        CentralValley,

        [DataMember(Name = "Inland Empire")]
        InlandEmpire, 

        [DataMember(Name="Los Angeles")]
        LosAngles,
        [DataMember(Name = "Orange")]
        Orange,

        [DataMember(Name ="Sacramento")]
        Sacramento,

        [DataMember(Name ="San Diego/Imperial")]
        SandiegoImperial,

        [DataMember(Name="Update California")]
        UpstateCalifornia,

    }
}