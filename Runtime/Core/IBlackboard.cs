using System;

namespace EntitiesBT.Core
{
    public interface IBlackboard
    {
        bool HasData<T>() where T : struct;
        T GetData<T>() where T : struct;
        ref T GetDataRef<T>() where T : struct;
        IntPtr GetDataPtrRO(Type type);
        IntPtr GetDataPtrRW(Type type);
        
        T GetObject<T>() where T : class;
    }
}
