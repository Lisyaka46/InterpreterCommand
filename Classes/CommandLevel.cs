using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.Classes
{
    /// <summary>
    /// Уровень доступа выполнения команды
    /// </summary>
    public enum CommandLevel
    {
        LowLevel = -1,
        Basic = 1,
        Managed = 2,
    }
}
