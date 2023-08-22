using System.Runtime.Serialization;
using Spore.Login;

namespace Spore.Main;

[DataContract]
public class MainState
{
    [DataMember] public LoginState LoginState { get; set; } = new();
}
