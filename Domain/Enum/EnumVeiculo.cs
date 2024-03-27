#region IMPORTAÇÕES
using System.Runtime.Serialization;
#endregion

namespace Domain.Enum
{
    public enum TipoVeiculo
    {
        [EnumMember(Value = "Moto")]
        Moto = 1,

        [EnumMember(Value = "Carro")]
        Carro = 2,

        [EnumMember(Value = "Van")]
        Van = 3
    }
}
 