using System.Runtime.Serialization;
using Cultivator.Login;

namespace Cultivator.Main;

[DataContract]
public class MainState
{
    [DataMember] public LoginState LoginState { get; set; } = new();
}
