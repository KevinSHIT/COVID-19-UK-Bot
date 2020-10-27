namespace COVID_19_UK_Bot
{
    public class Text
    {
        public const string USAGE = "Usage:\n" +
                                    "/ukcovid - Get 🇬🇧 COVID-19 information\n" +
                                    "/nation - Get 🇬🇧 national information\n" +
                                    "/queen - Get ♕ queen's speech";

        public const string QUEEN_SAYS =
            "🇬🇧 We will succeed and that success will belong to every one of us. --Elizabeth II";

        public const string QUEEN_SPEECH_MSG = QUEEN_SAYS + "\n" +
                                               "https://www.youtube.com/watch?v=2klmuggOElE";

        public const string SOMETHING_WRONG = "Oops! Something happened. Please forward this message to the author(@NodaYojiro).";

        public const string UNKNOWN_LOCATION = "Oops! Unknown location.";

        public const string EMPTY_RESPONSE = "Oops! Response is empty. Please contact author(@NodaYojiro).";
    }
}