namespace EntitiesBT.CodeGen.Editor
{
    public interface ILogger
    {
        public void Error(string message);
        public void Warning(string message);
        public void Info(string message);
        public void Debug(string message);
    }
}