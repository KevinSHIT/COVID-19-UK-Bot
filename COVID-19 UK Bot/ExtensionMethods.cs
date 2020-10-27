namespace COVID_19_UK_Bot
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Convert yyyy-MM-DD -> DD/MM/yyyy
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToBritishStyleDate(this string s)
        {
            var v = s.Trim().Split('-');
            return $"{v[0]}/{v[1]}/{v[2]}";
        }
    }
}