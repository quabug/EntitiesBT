using System;
using System.Collections.Generic;

namespace EntitiesBT.Editor
{
    public interface INodeCodeTemplate
    {
        string Header { get; }
        string Generate(
            Type nodeType
          , IEnumerable<INodeDataFieldCodeGenerator> fieldGenerators
          , string classNameOverride = ""
        );
    }
}
