namespace TheRecrutmentTool.Helpers
{
    using System;
    using System.Text;
    public class ExceptionMessageCreator
    {
        public static string CreateMessage(Exception ex)
        {
            var exMessagesAsSting = new StringBuilder();
            exMessagesAsSting.Append($"{ex.GetType().Name}: {ex.Message}");
            while ((ex = ex.InnerException) != null)
            {
                exMessagesAsSting.Append(Environment.NewLine + $"{ ex.GetType().Name}: {ex.Message}");
            }

            return exMessagesAsSting.ToString();
        }
    }
}
